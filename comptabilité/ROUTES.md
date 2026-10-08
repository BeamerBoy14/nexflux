# Routes de l'application NexFlux

## Pages publiques

### Page de connexion
- **URL** : `/login`
- **Description** : Page de connexion pour s'authentifier
- **Protection** : Accessible uniquement si non connecté (redirection automatique si connecté)

### Page d'erreur
- **URL** : `/Error`
- **Description** : Page d'erreur générique
- **Protection** : Accessible à tous

---

## Pages protégées (nécessitent une connexion)

### Page d'accueil
- **URL** : `/`
- **Description** : Page d'accueil avec statistiques et vue d'ensemble
- **Protection** : Redirection vers `/login` si non connecté

### Clients
- **URL** : `/clients`
- **Description** : Liste de tous les clients
- **Protection** : Redirection vers `/login` si non connecté

### Détails d'un client
- **URL** : `/clients/{clientId:int}`
- **Description** : Page de détails d'un client spécifique
- **Exemple** : `/clients/1`, `/clients/5`
- **Protection** : Redirection vers `/login` si non connecté

### Transactions
- **URL** : `/transactions`
- **Description** : Gestion des transactions (dépenses et gains)
- **Protection** : Redirection vers `/login` si non connecté

### Réservations
- **URL** : `/reservations`
- **Description** : Gestion des réservations
- **Protection** : Redirection vers `/login` si non connecté

### Postes
- **URL** : `/postes`
- **Description** : Gestion des postes comptables
- **Protection** : Redirection vers `/login` si non connecté

### Voitures
- **URL** : `/voitures`
- **Description** : Gestion des voitures
- **Protection** : Redirection vers `/login` si non connecté

### Détails d'une voiture
- **URL** : `/voitures/{voitureId:int}`
- **Description** : Fiche d'une voiture (année, kilométrage, statistiques, dépenses par catégorie, bilan par année, historique des transactions)
- **Exemple** : `/voitures/1`
- **Protection** : Redirection vers `/login` si non connecté (seules les voitures de l'utilisateur sont accessibles)

### Graphiques
- **URL** : `/graphiques`
- **Description** : Graphiques et analyses de données
- **Protection** : Redirection vers `/login` si non connecté

---

## Liens complets (exemples)

Selon votre configuration, les URLs complètes sont :

- **HTTPS** : `https://localhost:7192/[route]`
- **HTTP** : `http://localhost:5005/[route]`

### Exemples d'URLs complètes :

1. **Connexion** : `https://localhost:7192/login`
2. **Accueil** : `https://localhost:7192/`
3. **Clients** : `https://localhost:7192/clients`
4. **Client ID 1** : `https://localhost:7192/clients/1`
5. **Transactions** : `https://localhost:7192/transactions`
6. **Réservations** : `https://localhost:7192/reservations`
7. **Postes** : `https://localhost:7192/postes`
8. **Voitures** : `https://localhost:7192/voitures`
9. **Graphiques** : `https://localhost:7192/graphiques`
10. **Erreur** : `https://localhost:7192/Error`

---

## Navigation dans l'application

Une fois connecté, vous pouvez naviguer via :
- Le menu de navigation latéral (sidebar)
- Les liens directs dans l'application
- La barre d'adresse du navigateur (URLs ci-dessus)
