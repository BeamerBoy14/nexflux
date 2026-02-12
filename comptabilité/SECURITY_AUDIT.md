# 🔒 Audit de Sécurité - Application Comptabilité

**Date:** 2026-01-11  
**Version:** 1.0  
**Statut:** ⚠️ **CRITIQUE - Corrections nécessaires avant mise en production**

---

## 📋 Résumé Exécutif

Cet audit a identifié **8 problèmes de sécurité critiques** et **5 problèmes de sécurité moyens** qui doivent être corrigés avant la mise en production. L'application présente des vulnérabilités importantes dans l'authentification, la gestion des sessions, et la protection des données.

---

## 🚨 Problèmes Critiques

### 1. **Hachage de mot de passe faible (CRITIQUE)**
**Fichier:** `Services/AuthService.cs:138-145`

**Problème:**
- Utilisation de SHA256 seul sans salt ni itérations
- SHA256 est rapide et vulnérable aux attaques par dictionnaire/rainbow tables
- Pas de protection contre les attaques par force brute

**Impact:** 
- Les mots de passe peuvent être crackés rapidement
- En cas de fuite de base de données, tous les mots de passe sont compromis

**Solution recommandée:**
```csharp
// Utiliser BCrypt.Net-Next ou PBKDF2
// Exemple avec BCrypt:
using BCrypt.Net;

public static string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
}

public static bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```

**Priorité:** 🔴 **URGENTE**

---

### 2. **Gestion de session non sécurisée (CRITIQUE)**
**Fichier:** `Services/AuthService.cs:14-15, 23-32`

**Problème:**
- Session basée uniquement sur IP + User-Agent (facilement falsifiable)
- Pas de token de session sécurisé
- Pas de timeout automatique
- Dictionnaire en mémoire (perdu au redémarrage, pas scalable)
- Pas de protection CSRF appropriée

**Impact:**
- Session hijacking possible
- Pas de déconnexion automatique
- Problèmes de scalabilité (multi-instances)

**Solution recommandée:**
- Utiliser des cookies HttpOnly + Secure avec un token de session aléatoire
- Implémenter un timeout de session
- Utiliser un cache distribué (Redis) pour la production

**Priorité:** 🔴 **URGENTE**

---

### 3. **Mots de passe par défaut exposés (CRITIQUE)**
**Fichier:** `Components/Pages/Login.razor:87`, `Data/SeedUtilisateurs.cs:23,32`

**Problème:**
- Les mots de passe par défaut sont affichés sur la page de login
- Mots de passe hardcodés dans le code source
- Pas de changement de mot de passe obligatoire au premier login

**Impact:**
- Accès non autorisé si le code source est compromis
- Violation de sécurité si quelqu'un voit la page de login

**Solution recommandée:**
- Retirer l'affichage des comptes de test de la page de login
- Forcer le changement de mot de passe au premier login
- Utiliser des variables d'environnement pour les mots de passe initiaux

**Priorité:** 🔴 **URGENTE**

---

### 4. **Absence de rate limiting (CRITIQUE)**
**Fichier:** `Components/Pages/Login.razor`

**Problème:**
- Pas de protection contre les attaques par force brute
- Un attaquant peut essayer des milliers de mots de passe par seconde

**Impact:**
- Attaques par force brute possibles
- Risque de compromission des comptes

**Solution recommandée:**
- Implémenter un rate limiting (ex: 5 tentatives par IP, blocage de 15 minutes)
- Utiliser `AspNetCoreRateLimit` ou `Microsoft.AspNetCore.RateLimiting`

**Priorité:** 🔴 **URGENTE**

---

### 5. **Cookies non sécurisés (CRITIQUE)**
**Fichier:** `Program.cs:27-29`, `wwwroot/js/auth.js:11`

**Problème:**
- Cookies de session sans flag `Secure` (pas forcés en HTTPS)
- Cookies JavaScript accessibles (vulnérables au XSS)
- Pas de protection CSRF token dans les cookies

**Impact:**
- Interception des cookies en HTTP
- Vol de session via XSS

**Solution recommandée:**
```csharp
options.Cookie.HttpOnly = true;
options.Cookie.Secure = CookieSecurePolicy.Always; // En production
options.Cookie.SameSite = SameSiteMode.Strict; // Plus strict que Lax
```

**Priorité:** 🔴 **URGENTE**

---

### 6. **Exposition d'informations sensibles dans les erreurs (CRITIQUE)**
**Fichier:** `Components/Pages/Clients.razor:327-328`, `Components/Pages/Graphiques.razor`

**Problème:**
- `Console.WriteLine` avec stack traces dans le code
- Messages d'erreur détaillés exposés aux utilisateurs
- Stack traces visibles en production

**Impact:**
- Fuite d'informations sur la structure de l'application
- Aide les attaquants à comprendre le système

**Solution recommandée:**
- Utiliser un système de logging (Serilog, NLog)
- Ne jamais exposer les stack traces aux utilisateurs
- Logger les erreurs côté serveur uniquement

**Priorité:** 🔴 **URGENTE**

---

### 7. **Connection string exposée (CRITIQUE)**
**Fichier:** `appsettings.json:3`

**Problème:**
- Connection string dans le fichier de configuration
- Risque d'exposition si le fichier est commit dans Git

**Impact:**
- Accès non autorisé à la base de données

**Solution recommandée:**
- Utiliser des variables d'environnement ou Azure Key Vault
- Ajouter `appsettings.json` au `.gitignore` (garder `appsettings.Development.json`)

**Priorité:** 🔴 **URGENTE**

---

### 8. **Absence de validation côté serveur (CRITIQUE)**
**Fichier:** `Components/Pages/*.razor`

**Problème:**
- Validation uniquement côté client (peut être contournée)
- Pas de validation explicite côté serveur dans les méthodes de sauvegarde

**Impact:**
- Injection de données malveillantes
- Corruption de données

**Solution recommandée:**
- Ajouter des DataAnnotations sur tous les modèles
- Valider explicitement dans les méthodes de service avant sauvegarde

**Priorité:** 🔴 **URGENTE**

---

## ⚠️ Problèmes Moyens

### 9. **HTTPS non forcé en production**
**Fichier:** `Program.cs:46`

**Problème:**
- HTTPS redirection activée mais pas forcée
- Pas de HSTS configuré correctement

**Solution recommandée:**
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}
```

**Priorité:** 🟡 **MOYENNE**

---

### 10. **Absence de système de rôles**
**Fichier:** `Models/Utilisateur.cs:22`

**Problème:**
- Champ `Role` existe mais n'est pas utilisé
- Pas de différenciation des permissions

**Solution recommandée:**
- Implémenter un système d'autorisation basé sur les rôles
- Utiliser `[Authorize(Roles = "Admin")]` pour protéger les routes

**Priorité:** 🟡 **MOYENNE**

---

### 11. **Timeout de session trop long**
**Fichier:** `Program.cs:26`

**Problème:**
- Timeout de 8 heures est très long
- Risque si l'utilisateur oublie de se déconnecter

**Solution recommandée:**
- Réduire à 2-4 heures maximum
- Implémenter un timeout d'inactivité (30 minutes)

**Priorité:** 🟡 **MOYENNE**

---

### 12. **Pas de logging des tentatives de connexion**
**Fichier:** `Services/AuthService.cs:34-65`

**Problème:**
- Pas de log des tentatives de connexion échouées
- Impossible de détecter les attaques

**Solution recommandée:**
- Logger toutes les tentatives de connexion (succès/échec)
- Logger les IP et timestamps

**Priorité:** 🟡 **MOYENNE**

---

### 13. **Validation d'email faible**
**Fichier:** `Models/Client.cs:8`, `Models/Utilisateur.cs:18`

**Problème:**
- Pas de validation d'email avec regex ou DataAnnotation `[EmailAddress]`

**Solution recommandée:**
```csharp
[EmailAddress]
[StringLength(100)]
public string Email { get; set; } = string.Empty;
```

**Priorité:** 🟡 **MOYENNE**

---

## ✅ Points Positifs

1. ✅ Utilisation d'Entity Framework Core (protection SQL injection)
2. ✅ Blazor Server protège contre XSS par défaut
3. ✅ Antiforgery tokens activés
4. ✅ HttpOnly cookies pour les sessions
5. ✅ Filtrage des données par utilisateur (isolation des données)

---

## 📝 Plan d'Action Recommandé

### Phase 1 - Corrections Critiques (Avant mise en production)
1. ✅ Remplacer SHA256 par BCrypt
2. ✅ Sécuriser les cookies (Secure flag)
3. ✅ Retirer les mots de passe de la page de login
4. ✅ Ajouter rate limiting
5. ✅ Améliorer la gestion des erreurs
6. ✅ Sécuriser la connection string

### Phase 2 - Améliorations (Après mise en production)
7. ✅ Implémenter un système de rôles
8. ✅ Ajouter logging des tentatives de connexion
9. ✅ Réduire le timeout de session
10. ✅ Améliorer la validation des entrées

---

## 🔗 Ressources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [ASP.NET Core Security Best Practices](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [BCrypt.Net](https://github.com/BcryptNet/bcrypt.net)

---

**⚠️ IMPORTANT:** Ne pas mettre l'application en production avant d'avoir corrigé au minimum les problèmes critiques (1-8).
