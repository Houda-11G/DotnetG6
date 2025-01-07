using Microsoft.AspNetCore.Mvc;
using partieWebGDHotel.Data;
using partieWebGDHotel.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;


namespace partieWebGDHotel.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly DatabaseContext _dbContext;

        public DashboardController(ILogger<DashboardController> logger, DatabaseContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // Action Index pour afficher les chambres filtrées par type
        public IActionResult Index(string typeChambre)
        {
            try
            {
                // Récupérer l'ID du client à partir de la session
                int? clientId = HttpContext.Session.GetInt32("ClientId");
                if (clientId == null)
                {
                    _logger.LogWarning("Client non trouvé dans la session.");
                    return RedirectToAction("Login", "Client");
                }

                var clientInfo = clientId.HasValue
                    ? _dbContext.Client
                        .Where(c => c.Id == clientId.Value)
                        .Select(c => new UserInfo
                        {
                            Nom = $"{c.Prenom} {c.Nom}",
                            Email = c.email.ToLower()
                        })
                        .FirstOrDefault()
                    : null;


                var chambres = _dbContext.Chambre.AsQueryable();
                if (!string.IsNullOrEmpty(typeChambre))
                {
                    chambres = chambres.Where(c => c.TypeChambre == typeChambre);
                }

                var reservation = _dbContext.Reservation.FirstOrDefault(r => r.ClientId == clientId);

                return View(new DashboardViewModel
                {
                    Chambres = chambres.ToList(),
                    ReservationId = reservation?.ID_Reservation,
                    User = clientInfo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de l'exécution de l'index : {ex.Message}");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        // Action pour afficher la chambre à réserver
        public IActionResult Reserve(int numChambre)
        {
            try
            {
                var chambre = _dbContext.Chambre.FirstOrDefault(c => c.NumChambre == numChambre);
                if (chambre == null)
                {
                    _logger.LogWarning($"Chambre {numChambre} non trouvée.");
                    return NotFound();
                }

                return View(chambre);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de la récupération de la chambre {numChambre}: {ex.Message}");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        // Action pour afficher les détails d'une réservation
        public IActionResult Details(int id)
        {
            try
            {
                var reservation = _dbContext.Reservation
                    .Include(r => r.Chambre)
                    .FirstOrDefault(r => r.ID_Reservation == id);

                if (reservation == null)
                {
                    _logger.LogWarning($"Réservation {id} non trouvée.");
                    return NotFound("Réservation introuvable.");
                }

                // Gestion des dates nulles (DateDebut et DateFin)
                ViewData["StartDate"] = reservation.DateDebut?.ToString("dd/MM/yyyy") ?? "Non spécifié";
                ViewData["EndDate"] = reservation.DateFin?.ToString("dd/MM/yyyy") ?? "Non spécifié";

                return View(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de la récupération des détails de la réservation {id}: {ex.Message}");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        // Méthode Reserve pour effectuer la réservation en POST
        [HttpPost]
        public async Task<IActionResult> Reserve(int numChambre, DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                var chambre = await _dbContext.Chambre
                                              .FirstOrDefaultAsync(c => c.NumChambre == numChambre);
                if (chambre == null)
                {
                    _logger.LogWarning($"Chambre {numChambre} non trouvée pour la réservation.");
                    return NotFound();
                }

                // Vérifier si la date de fin est après la date de début
                if (dateFin <= dateDebut)
                {
                    _logger.LogWarning("La date de fin doit être postérieure à la date de début.");
                    return BadRequest("La date de fin doit être postérieure à la date de début.");
                }

                // Calculer le nombre de jours entre la date de début et la date de fin
                int nbrJours = (dateFin - dateDebut).Days;
                if (nbrJours <= 0)
                {
                    _logger.LogWarning("La période de réservation est trop courte.");
                    return BadRequest("La période de réservation doit durer au moins un jour.");
                }

                // Récupérer l'ID du client à partir de la session
                int? clientId = HttpContext.Session.GetInt32("ClientId");
                if (clientId == null)
                {
                    _logger.LogWarning("Client non trouvé dans la session.");
                    return RedirectToAction("Login", "Client");
                }

                // Calcul du prix total de la réservation
                decimal prixTotal = chambre.PrixParNuit * nbrJours;

                // Créer un nouvel objet Reservation
                var reservation = new Reservation
                {
                    NumChambre = numChambre,
                    ClientId = clientId.Value, // Utiliser l'ID du client de la session
                    DateDebut = dateDebut,
                    DateFin = dateFin,
                    Statut = "Non confirmé", // Statut par défaut
                    DateCreation = DateTime.Now,
                    Prix = prixTotal, // Prix calculé en fonction du nombre de nuits
                };

                // Ajouter la réservation dans la base de données
                _dbContext.Reservation.Add(reservation);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Réservation créée avec succès pour la chambre {numChambre}, client {clientId}. Prix total: {prixTotal}");

                return RedirectToAction("Index", "Home"); // Redirige vers la page d'accueil ou une autre page appropriée
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"Erreur lors de l'accès à la base de données pour la réservation de la chambre {numChambre}: {dbEx.Message}");
                _logger.LogError($"Détails : {dbEx.StackTrace}");
                return StatusCode(500, "Erreur de base de données. Veuillez réessayer plus tard.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de la création de la réservation pour la chambre {numChambre}: {ex.Message}");
                _logger.LogError($"Détails de l'erreur : {ex.StackTrace}");
                return StatusCode(500, "Erreur interne du serveur. Veuillez réessayer plus tard.");
            }
        }

        // Méthode pour annuler une réservation
        [HttpPost]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            try
            {
                var reservation = await _dbContext.Reservation.FindAsync(reservationId);

                if (reservation == null)
                {
                    _logger.LogWarning($"Réservation {reservationId} non trouvée.");
                    return NotFound("Réservation introuvable.");
                }

                var chambre = await _dbContext.Chambre.FirstOrDefaultAsync(c => c.NumChambre == reservation.NumChambre);
                if (chambre != null)
                {
                    chambre.Statut = "Libre";
                }

                _dbContext.Reservation.Remove(reservation);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Réservation {reservationId} annulée avec succès.");

                return Ok("Réservation annulée avec succès.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de l'annulation de la réservation {reservationId}: {ex.Message}");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        // Méthode pour mettre une chambre en maintenance
        [HttpPost]
        public async Task<IActionResult> SetMaintenance(int numChambre)
        {
            try
            {
                var chambre = await _dbContext.Chambre.FirstOrDefaultAsync(c => c.NumChambre == numChambre);
                if (chambre == null)
                {
                    _logger.LogWarning($"Chambre {numChambre} non trouvée pour mise en maintenance.");
                    return NotFound("Chambre introuvable.");
                }

                chambre.Statut = "EnMaintenance";
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Chambre {numChambre} mise en maintenance avec succès.");

                return Ok("La chambre a été mise en maintenance.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de la mise en maintenance de la chambre {numChambre}: {ex.Message}");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }

        // Action pour afficher les réservations du client
        public IActionResult MesReservations()
        {
            try
            {
                // Récupérer l'ID du client à partir de la session
                int? clientId = HttpContext.Session.GetInt32("ClientId");

                // Vérifier si l'ID du client est nul
                if (clientId == null)
                {
                    _logger.LogWarning("Client non trouvé dans la session.");
                    return RedirectToAction("Login", "Client");
                }

                // Récupérer les réservations du client
                var reservations = _dbContext.Reservation
                    .Where(r => r.ClientId == clientId)
                    .Include(r => r.Chambre) // Inclure les informations sur la chambre si nécessaire
                    .ToList();

                // Si aucune réservation n'est trouvée, retourner une vue avec un message
                if (!reservations.Any())
                {
                    _logger.LogInformation("Aucune réservation trouvée pour ce client.");
                    ViewBag.Message = "Aucune réservation disponible.";
                }

                return View(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erreur lors de l'affichage des réservations : {ex.Message}");
                return StatusCode(500, "Erreur interne du serveur.");
            }
        }
        [HttpGet]
        public IActionResult ExportToPdf(int id)
        {
            var reservations = _dbContext.Reservation
                .Where(r => r.ClientId == id) // Récupérer les réservations pour un client donné
                .Include(r => r.Chambre)
                .ToList();

            if (reservations == null || !reservations.Any())
            {
                return NotFound(); // Si aucune réservation n'est trouvée
            }

            return new ViewAsPdf("MesRéservations", reservations)
            {
                FileName = "MesReservations.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }



    }
}
