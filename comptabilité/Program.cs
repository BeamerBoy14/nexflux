using comptabilité.Components;
using comptabilité.Data;
using comptabilité.Services;
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

            var app = builder.Build();

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

            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Initialiser les utilisateurs par défaut et assigner les données existantes
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ComptabiliteDbContext>();
                await context.Database.MigrateAsync(); // Appliquer les migrations
                await Data.SeedUtilisateurs.SeedAsync(context); // Initialiser les utilisateurs
                await Data.AssignDataToTransEraLegend.AssignExistingDataAsync(context); // Assigner les données existantes à TransEraLegend
            }

            await app.RunAsync();
        }
    }
}
