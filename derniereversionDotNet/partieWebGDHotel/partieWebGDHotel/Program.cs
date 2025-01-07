using Microsoft.EntityFrameworkCore;
using partieWebGDHotel.Data;
using partieWebGDHotel.Models;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Ajouter les services au conteneur.

// Configuration du DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Si vous utilisez des sessions, assurez-vous d'ajouter les services de session
builder.Services.AddDistributedMemoryCache();  // Utilisation de la mémoire pour stocker les sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Durée de la session avant expiration
    options.Cookie.HttpOnly = true;  // Empêche l'accès au cookie par le client JavaScript
    options.Cookie.IsEssential = true;  // Nécessaire pour les cookies dans certaines régions (par exemple, GDPR)
    options.Cookie.SameSite = SameSiteMode.Lax;  // Sécurité supplémentaire pour les cookies
});


// Ajoutez les services nécessaires pour les contrôleurs et les vues
builder.Services.AddControllersWithViews();

// Enregistrement des services pour les réservations
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

// Configurer le pipeline de requêtes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Pour l'environnement de développement
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseStaticFiles();
// Activer la session si vous en avez besoin
app.UseSession();

// Ajouter l'autorisation si nécessaire
app.UseAuthorization();

// Configurer la route par défaut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Exécuter l'application
app.Run();
