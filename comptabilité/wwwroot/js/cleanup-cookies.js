// Script pour supprimer les anciens cookies persistants au chargement de la page
(function() {
    // Supprimer tous les cookies d'authentification existants (y compris les anciens avec expiration)
    var cookiesToDelete = ['UtilisateurId', 'UtilisateurNom'];
    
    cookiesToDelete.forEach(function(cookieName) {
        // Supprimer le cookie avec différents chemins possibles
        document.cookie = cookieName + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Lax;';
        document.cookie = cookieName + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
        document.cookie = cookieName + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/login;';
    });
})();
