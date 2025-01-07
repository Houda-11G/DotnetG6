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
builder.Services.AddDistributedMemoryCache();  // Utilisation de la m�moire pour stocker les sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Dur�e de la session avant expiration
    options.Cookie.HttpOnly = true;  // Emp�che l'acc�s au cookie par le client JavaScript
    options.Cookie.IsEssential = true;  // N�cessaire pour les cookies dans certaines r�gions (par exemple, GDPR)
    options.Cookie.SameSite = SameSiteMode.Lax;  // S�curit� suppl�mentaire pour les cookies
});


// Ajoutez les services n�cessaires pour les contr�leurs et les vues
builder.Services.AddControllersWithViews();

// Enregistrement des services pour les r�servations
builder.Services.AddScoped<IReservationService, ReservationService>();

var app = builder.Build();

// Configurer le pipeline de requ�tes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Pour l'environnement de d�veloppement
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

// Ajouter l'autorisation si n�cessaire
app.UseAuthorization();

// Configurer la route par d�faut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ex�cuter l'application
app.Run();
