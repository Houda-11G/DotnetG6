using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using partieWebGDHotel.Data;
using partieWebGDHotel.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

[Route("[controller]")]
public class ReservationController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IReservationService _reservationService;

    public ReservationController(DatabaseContext context, IReservationService reservationService)
    {
        _context = context;
        _reservationService = reservationService;
    }

    // Affiche les détails de la réservation pour acceptation
    [HttpGet("accept")]
    public IActionResult Accept(int reservationId)
    {
        var reservation = _context.Reservation
            .Include(r => r.Client)
            .Include(r => r.Chambre)
            .FirstOrDefault(r => r.ID_Reservation == reservationId);

        if (reservation == null)
        {
            return NotFound("Réservation introuvable.");
        }

        // Passer les données à la vue
        ViewBag.ClientFirstName = reservation.Client.Prenom;
        ViewBag.ClientLastName = reservation.Client.Nom;
        ViewBag.RoomReserved = reservation.Chambre.TypeChambre;
        ViewBag.Duration = (reservation.DateFin - reservation.DateDebut)?.Days ?? 0;
        ViewBag.TotalAmount = reservation.Prix;
        ViewBag.ReservationId = reservation.ID_Reservation;

        // Vérifier si la photo du reçu est présente
        if (reservation.RecuPhotoPath != null && reservation.RecuPhotoPath.Length > 0)
        {
            ViewBag.UploadMessage = "La photo du reçu est présente.";
        }
        else
        {
            ViewBag.UploadMessage = "Aucune photo du reçu n'a été téléchargée.";
        }

        return View("Accept");
    }

    // Générer le PDF de la réservation
    [HttpGet("DownloadPdf")]
    public IActionResult DownloadPdf(int reservationId)
    {
        var reservation = _context.Reservation
            .Include(r => r.Client)
            .Include(r => r.Chambre)
            .FirstOrDefault(r => r.ID_Reservation == reservationId);

        if (reservation == null)
        {
            return NotFound("Réservation introuvable.");
        }

        // Créer un PDF de la réservation
        string pdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reservations", $"Reservation_{reservationId}.pdf");

        using (var writer = new PdfWriter(pdfPath))
        using (var pdf = new PdfDocument(writer))
        {
            Document document = new Document(pdf);
            document.Add(new Paragraph($"Confirmation de réservation - {reservationId}"));
            document.Add(new Paragraph($"Client: {reservation.Client.Prenom} {reservation.Client.Nom}"));
            document.Add(new Paragraph($"Chambre: {reservation.Chambre.TypeChambre}"));
            document.Add(new Paragraph($"Durée: {(reservation.DateFin - reservation.DateDebut)?.Days ?? 0} nuits"));
            document.Add(new Paragraph($"Montant total: {reservation.Prix} €"));
        }

        // Retourner le PDF pour le téléchargement
        var fileBytes = System.IO.File.ReadAllBytes(pdfPath);
        return File(fileBytes, "application/pdf", $"Reservation_{reservationId}.pdf");
    }

    // Afficher la liste des réservations avec possibilité de télécharger les PDF
    [HttpGet("list")]
    public IActionResult List()
    {
        var reservations = _context.Reservation
            .Include(r => r.Client)
            .Include(r => r.Chambre)
            .ToList();

        return View("List", reservations);
    }

    // Upload du reçu de la réservation
    [HttpPost("UploadReceipt")]
    public IActionResult UploadReceipt(int reservationId, IFormFile recuPhoto)
    {
        if (recuPhoto != null && recuPhoto.Length > 0)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    recuPhoto.CopyTo(memoryStream);
                    byte[] photoData = memoryStream.ToArray(); // Tableau de bytes de l'image

                    var reservation = _context.Reservation
                        .FirstOrDefault(r => r.ID_Reservation == reservationId);

                    if (reservation != null)
                    {
                        reservation.RecuPhotoPath = photoData;
                        _context.SaveChanges();

                        ViewBag.UploadMessage = "Reçu téléchargé et enregistré avec succès.";
                        return RedirectToAction("Accept", new { reservationId = reservationId });
                    }
                    else
                    {
                        return NotFound("Réservation introuvable.");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.UploadError = $"Erreur lors de l'enregistrement du fichier : {ex.Message}";
                return RedirectToAction("Accept", new { reservationId = reservationId });
            }
        }

        return BadRequest("Aucun fichier sélectionné.");
    }

    // Rejeter une réservation
    [HttpGet("reject")]
    public IActionResult Reject(int reservationId)
    {
        var reservation = _context.Reservation.FirstOrDefault(r => r.ID_Reservation == reservationId);

        if (reservation == null)
        {
            TempData["ErrorMessage"] = "Réservation non trouvée.";
            return View("Accept");
        }

        _context.Reservation.Remove(reservation);
        _context.SaveChanges();

        TempData["SuccessMessage"] = "Votre réservation a été annulée avec succès. À la prochaine !";
        return View("reject");
    }
}
