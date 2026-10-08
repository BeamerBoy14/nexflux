using comptabilité.Components;
using comptabilité.Data;
using comptabilité.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

namespace comptabilité
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Configuration de la base de données
            builder.Services.AddDbContext<ComptabiliteDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Cles de protection des donnees (cookies, antiforgery) enregistrees dans le volume Docker (/app/data) :
            // sans cela, elles sont perdues a chaque redemarrage du conteneur et les sessions deviennent invalides.
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddDataProtection()
                    .SetApplicationName("NexFlux")
                    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "data", "keys")));
            }

            // Derriere Traefik, l'application recoit du HTTP : le schema et l'IP d'origine sont lus dans les en-tetes
            // X-Forwarded-*. Le port 8080 n'est pas publie, seul le proxy peut donc joindre l'application.
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // Configuration des sessions
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(8);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            // Services d'authentification
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddSingleton<VoitureLogoCatalog>();
            builder.Services.AddScoped<BreadcrumbState>();

            var app = builder.Build();

            // Doit passer en premier pour que la suite du pipeline voie le bon schema (https)
            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            // Utiliser les sessions AVANT UseAntiforgery
            app.UseSession();

            // Établit le cookie de session dès le premier chargement de page (requête HTTP classique),
            // avant que Blazor Server ne bascule sur le circuit SignalR : une fois le circuit actif,
            // la réponse HTTP initiale est déjà terminée et il n'est plus possible d'émettre un nouveau
            // cookie (erreur "session cannot be established after the response has started").
            app.Use(async (context, next) =>
            {
                if (string.IsNullOrEmpty(context.Session.GetString("_sessionInitialisee")))
                {
                    context.Session.SetString("_sessionInitialisee", "1");
                }
                await next();
            });

            app.UseAntiforgery();

            // Logo d'une voiture : réservé à l'utilisateur propriétaire, servi depuis la base de données
            app.MapGet("/api/voitures/{id:int}/logo", async (int id, HttpContext http, ComptabiliteDbContext db, AuthService auth) =>
            {
                var userId = auth.GetUtilisateurId();
                if (!userId.HasValue)
                {
                    return Results.Unauthorized();
                }

                var logo = await db.VoitureLogos.AsNoTracking()
                    .Where(l => l.VoitureId == id && l.Voiture!.UtilisateurId == userId.Value)
                    .Select(l => new { l.Data, l.ContentType })
                    .FirstOrDefaultAsync();

                if (logo is null)
                {
                    return Results.NotFound();
                }

                http.Response.Headers["Cache-Control"] = "private, max-age=86400";
                http.Response.Headers["X-Content-Type-Options"] = "nosniff";
                return Results.File(logo.Data, logo.ContentType);
            });

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Initialiser les utilisateurs par défaut et assigner les données existantes
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ComptabiliteDbContext>();
                await context.Database.MigrateAsync(); // Appliquer les migrations
                await Data.SeedUtilisateurs.SeedAsync(context, app.Configuration, app.Logger); // Creer/synchroniser les comptes depuis la configuration
                await Data.AssignDataToTransEraLegend.AssignExistingDataAsync(context); // Assigner les données existantes à TransEraLegend
            }

            await app.RunAsync();
        }
    }
}
