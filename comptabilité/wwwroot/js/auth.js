// Fonctions d'authentification côté client
window.authHelper = {
    setCookie: function(name, value, days) {
        var expires = "";
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        // Si days est null ou undefined, le cookie sera une session cookie (supprimé à la fermeture du navigateur)
        document.cookie = name + "=" + (value || "") + expires + "; path=/; SameSite=Lax";
    },
    
    getCookie: function(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    },
    
    deleteCookie: function(name) {
        document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; SameSite=Lax;";
    }
};

// Fonction globale de déconnexion
window.performLogout = function() {
    // Rediriger simplement vers la page de login
    // Le nettoyage de la session se fera côté serveur dans Login.razor
    window.location.href = '/login';
};
