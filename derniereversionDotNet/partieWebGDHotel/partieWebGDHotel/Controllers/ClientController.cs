using Microsoft.AspNetCore.Mvc;
using partieWebGDHotel.Data;
using partieWebGDHotel.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ClientController : Controller
{
    private readonly DatabaseContext _dbContext;

    // Constructeur avec injection de dépendances
    public ClientController(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Action Register (affiche le formulaire d'inscription)
    public IActionResult Register()
    {
        return View();
    }

    // Action POST Register (gère l'inscription)
    [HttpPost]
    public async Task<IActionResult> Register(Client client)
    {
        if (ModelState.IsValid)
        {
            // Ajoute le client à la base de données
            _dbContext.Client.Add(client);
            await _dbContext.SaveChangesAsync();

            // Redirige vers une autre page après l'inscription
            return RedirectToAction("Index", "Home");
        }

        // Si le modèle n'est pas valide, renvoie le formulaire avec les erreurs
        return View(client);
    }

    // Action Login (affiche le formulaire de connexion)
    public IActionResult Login()
    {
        return View();
    }

    // Action POST Login (gère la connexion)
    [HttpPost]
    public async Task<IActionResult> Login(string email, string motDePasse)
    {
        // Recherche le client dans la base de données
        var client = await _dbContext.Client.FirstOrDefaultAsync(c => c.email == email);

        if (client != null)
        {
            // Vérifier le mot de passe (ici on compare en clair, mais à sécuriser avec un hash)
            if (client.mot_de_passe == motDePasse)
            {
                // Connexion réussie, stocke l'ID du client dans la session
                HttpContext.Session.SetInt32("ClientId", client.Id);

                return RedirectToAction("Index", "Dashboard");
            }
        }

        // Si l'email ou le mot de passe est incorrect
        ViewData["ErrorMessage"] = "Email ou mot de passe incorrect.";
        return View();

    }

    // Action Logout (déconnexion)
    public IActionResult Logout()
    {
        // Supprime les données de session
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
