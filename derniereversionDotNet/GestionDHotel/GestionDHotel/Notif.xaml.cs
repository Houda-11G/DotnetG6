using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO; // Ajoutez cette ligne
using System.Net;
using System.Net.Mail;
namespace GestionDHotel
{
    public partial class Notif : Window
    {
        private string connectionString = "Server=localhost;Database=ghotel;User ID=root;Password=;Pooling=true;";
        private ObservableCollection<ReservationInfo> reservationInfos = new ObservableCollection<ReservationInfo>();

        public Notif()
        {
            InitializeComponent();
            LoadNotifications(); // Charger les notifications dès l'ouverture de la fenêtre
            UpdateExpiredReservations();
        }
        private void UpdateExpiredReservations()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID_Reservation, NumChambre, DateFin FROM Reservation WHERE DateFin < CURDATE() AND Statut != 'Libre'";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int reservationId = reader.GetInt32("ID_Reservation");
                                int numChambre = reader.GetInt32("NumChambre");
                                DateTime dateFin = reader.GetDateTime("DateFin");

                                // Vérifier si la réservation est expirée
                                if (dateFin < DateTime.Now)
                                {
                                    // Mettre à jour le statut de la chambre à "Libre"
                                    UpdateRoomStatus(numChambre, "Libre");

                                    // Mettre à jour le statut de la réservation à "Expirée"
                                    UpdateReservationStatus(reservationId, "Expirée");
                                }
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur de connexion : {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
        }

        private int CountTodayNotif()
        {
            int count = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Reservation WHERE DATE(DateCreation) = CURDATE()";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur de connexion : {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
            return count;
        }

        private void LoadNotifications()
        {
            reservationInfos.Clear(); // Clear the ObservableCollection

            int todayNotificationCount = CountTodayNotif();
            if (todayNotificationCount > 0)
            {
                LoadReservationDetails(); // Load today's reservations
            }
            else
            {
                reservationInfos.Add(new ReservationInfo
                {
                    ID_Reservation = 0,
                    NumChambre = 0,
                    ClientId = 0,
                    Statut = "Aucune nouvelle réservation aujourd'hui",
                    DateCreation = DateTime.Now,
                    Prix = 0,
                    RecuPhotoPath = "default_image.jpg"  // Placeholder for missing photo
                });
            }

            NotificationsListView.ItemsSource = reservationInfos; // Rebind the ListView
        }
        private BitmapImage ConvertBase64ToBitmapImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            var base64Parts = base64String.Split(',');
            if (base64Parts.Length < 2)
            {
                // Gérer le cas où il n'y a pas de données Base64
                MessageBox.Show("Données d'image non valides.");
                return null;
            }

            var base64Data = base64Parts[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            using (var stream = new MemoryStream(imageBytes))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze(); // Pour rendre l'image immuable
                return bitmap;
            }
        }



        private void LoadReservationDetails()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ID_Reservation, NumChambre, ClientId, DateDebut, DateFin, Statut, DateCreation, Prix, RecuPhotoPath FROM Reservation WHERE DATE(DateCreation) = CURDATE();";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] photoBytes = reader.IsDBNull(reader.GetOrdinal("RecuPhotoPath")) ? null : (byte[])reader["RecuPhotoPath"];
                                string RecuPhotoPath = ConvertToBase64(photoBytes); // Convertir le tableau de bytes en Base64

                                reservationInfos.Add(new ReservationInfo
                                {
                                    ID_Reservation = reader.GetInt32("ID_Reservation"),
                                    NumChambre = reader.GetInt32("NumChambre"),
                                    ClientId = reader.GetInt32("ClientId"),
                                    DateDebut = reader.IsDBNull(reader.GetOrdinal("DateDebut")) ? (DateTime?)null : reader.GetDateTime("DateDebut"),
                                    DateFin = reader.IsDBNull(reader.GetOrdinal("DateFin")) ? (DateTime?)null : reader.GetDateTime("DateFin"),
                                    Statut = reader.GetString("Statut"),
                                    DateCreation = reader.GetDateTime("DateCreation"),
                                    Prix = reader.GetDecimal("Prix"),
                                    RecuPhotoPath = RecuPhotoPath, // Chemin d'origine
                                    PhotoUrl = ConvertBase64ToBitmapImage(RecuPhotoPath) // Image convertie
                                });
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Erreur de connexion : {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
            }
        }



        private string ConvertToBase64(byte[] imageBytes)
        {
            if (imageBytes == null)
                return null;

            return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}"; // Ajoutez le préfixe pour le type MIME
        }



        private string GetClientEmail(int reservationId)
        {
            string email = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open(); // Ouverture de la connexion
                string query = @"
        SELECT c.email
        FROM client c
        JOIN reservation r ON c.Id = r.ClientId
        WHERE r.ID_Reservation = @reservationId";

                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@reservationId", reservationId);  // Utilisation de reservationId
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        email = result.ToString();  // Récupération de l'email
                    }
                }
            }

            return email;  // Retour de l'email ou null si non trouvé
        }


        private void UpdateReservationStatus(int reservationId, string statutReservation)
        {
            string query = "UPDATE Reservation SET Statut = @StatutReservation WHERE ID_Reservation = @ReservationId";
            try
            {
                int rowsAffected = ExecuteNonQuery(query, new Dictionary<string, object>
                {
                    { "@StatutReservation", statutReservation },
                    { "@ReservationId", reservationId }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Statut de la réservation mis à jour avec succès.");
                }
                else
                {
                    MessageBox.Show("Aucune réservation trouvée avec cet ID.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du statut : {ex.Message}");
            }
        }

        private int ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(query, connection))
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'exécution de la requête : {ex.Message}");
                return 0;  // Retourne 0 si aucune ligne n'est affectée
            }
        }

        private void AcceptReservation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ReservationInfo reservation)
            {
                try
                {
                    // Utilisation de GetClientEmail avec ID_Reservation pour récupérer l'email du client
                    string emailClient = GetClientEmail(reservation.ID_Reservation);
                    if (string.IsNullOrEmpty(emailClient))
                    {
                        MessageBox.Show("L'email du client est introuvable.");
                        return;
                    }

                    // Construction des URLs pour accepter ou refuser la réservation
                    string acceptUrl = $"http://localhost:5000/Reservation/accept?reservationId={reservation.ID_Reservation}";
                    string rejectUrl = $"http://localhost:5000/Reservation/reject?reservationId={reservation.ID_Reservation}";

                    // Sujet et message HTML
                    string sujet = "Action requise : Réservation";
                    string message = $@"
            <html>
            <body>
                <p>Bonjour,</p>
                <p>Nous avons reçu votre demande de réservation pour la chambre numéro {reservation.NumChambre} 
                pour la période du {reservation.DateDebut:dd/MM/yyyy} au {reservation.DateFin:dd/MM/yyyy}.</p>
                <p>Veuillez confirmer ou annuler votre réservation en utilisant les boutons ci-dessous :</p>
                <a href='{acceptUrl}' style='padding: 10px 15px; background-color: green; color: white; text-decoration: none; border-radius: 5px;'>Accepter</a>
                <a href='{rejectUrl}' style='padding: 10px 15px; background-color: red; color: white; text-decoration: none; border-radius: 5px; margin-left: 10px;'>Annuler</a>
                <p>Merci pour votre choix !</p>
            </body>
            </html>";

                    // Envoi de l'email
                    EmailUtility.EnvoyerEmail(emailClient, sujet, message, isHtml: true);

                    // Mise à jour du statut de la réservation
                    UpdateReservationStatus(reservation.ID_Reservation, "En attente");

                    // Message de confirmation
                    MessageBox.Show("Email avec boutons d'action envoyé au client.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Une erreur s'est produite : {ex.Message}");
                }
            }
        }

        private void RejectReservation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int reservationId = GetSelectedReservationId();
                if (reservationId == -1)
                {
                    MessageBox.Show("Veuillez sélectionner une réservation.");
                    return;
                }

                string checkReservationQuery = "SELECT COUNT(*) FROM Reservation WHERE ID_Reservation = @ReservationId";
                int count = Convert.ToInt32(ExecuteScalar(checkReservationQuery, new Dictionary<string, object>
                {
                    { "@ReservationId", reservationId }
                }));

                if (count == 0)
                {
                    MessageBox.Show("La réservation n'existe pas.");
                    return;
                }

                string getRoomQuery = "SELECT NumChambre FROM Reservation WHERE ID_Reservation = @ReservationId";
                int numChambre = Convert.ToInt32(ExecuteScalar(getRoomQuery, new Dictionary<string, object>
                {
                    { "@ReservationId", reservationId }
                }));

                UpdateRoomStatus(numChambre, "Disponible");
                string deleteQuery = "DELETE FROM Reservation WHERE ID_Reservation = @ReservationId";
                int rowsAffected = ExecuteNonQuery(deleteQuery, new Dictionary<string, object>
                {
                    { "@ReservationId", reservationId }
                });

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Réservation annulée.");
                }
                else
                {
                    MessageBox.Show("Aucune réservation supprimée.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'annulation de la réservation : {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNotifications(); // Recharger les notifications
        }

        private void ValidateReservation_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ReservationInfo reservation)
            {
                try
                {
                    UpdateReservationStatus(reservation.ID_Reservation, "Confirmée");
                    UpdateRoomStatus(reservation.NumChambre, "Réservée");

                    // Récupérer l'email du client
                    string clientEmail = GetClientEmail(reservation.ID_Reservation);

                    // Vérifiez si l'email est valide
                    if (!string.IsNullOrEmpty(clientEmail))
                    {
                        string subject = "Confirmation de votre réservation";
                        string body = $"<h1>Confirmation de votre réservation</h1>" +
                                      $"<p>Bonjour, {reservation.ClientId},</p>" +
                                      $"<p>Votre réservation pour la chambre {reservation.NumChambre} a été confirmée.</p>" +
                                      $"<p>Durée: {(reservation.DateDebut.HasValue ? reservation.DateDebut.Value.ToShortDateString() : "Non spécifiée")} au " +
                                      $"{(reservation.DateFin.HasValue ? reservation.DateFin.Value.ToShortDateString() : "Non spécifiée")}</p>" +
                                      $"<p>Total: {reservation.Prix} €</p>" +
                                      $"<p>Merci de votre confiance.</p>";

                        // Envoyer l'email
                        SendEmail(clientEmail, subject, body);
                    }
                    else
                    {
                        MessageBox.Show("L'adresse email est manquante. Impossible d'envoyer la confirmation.");
                    }

                    button.IsEnabled = false; // Désactive le bouton "Valider"
                    var acceptButton = FindName("AcceptReservation") as Button;  // Trouve le bouton "Accepter"
                    var rejectButton = FindName("RejectReservation") as Button;  // Trouve le bouton "Rejeter"
                    if (acceptButton != null) acceptButton.IsEnabled = false;
                    if (rejectButton != null) rejectButton.IsEnabled = false;

                    MessageBox.Show("Réservation confirmée et statut de la chambre mis à jour.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la validation de la réservation : {ex.Message}");
                }
            }
        }



        private void SendEmail(string email, string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com"))

            {
                smtpClient.Port = 587; // ou 25, selon votre serveur
                smtpClient.Credentials = new NetworkCredential("houdafaty86@gmail.com", "ludp rfsk xwuk sglq");
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("houdafaty86@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
            }
        }

        private void UpdateRoomStatus(int numChambre, string roomStatus)
        {
            string query = "UPDATE Chambre SET Statut = @RoomStatus WHERE NumChambre = @NumChambre";
            try
            {
                ExecuteNonQuery(query, new Dictionary<string, object>
                {
                    { "@RoomStatus", roomStatus },
                    { "@NumChambre", numChambre }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du statut de la chambre : {ex.Message}");
            }
        }

        private object ExecuteScalar(string query, Dictionary<string, object> parameters)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    return command.ExecuteScalar();
                }
            }
        }

        private int GetSelectedReservationId()
        {
            if (NotificationsListView.SelectedItem is ReservationInfo selectedReservation)
            {
                return selectedReservation.ID_Reservation;
            }
            return -1; // Return an invalid value if nothing is selected
        }
    }

    public class ReservationInfo
    {
        public int ID_Reservation { get; set; }
        public int NumChambre { get; set; }
        public int ClientId { get; set; }
        public string Email { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string Statut { get; set; }
        public DateTime DateCreation { get; set; }
        public decimal Prix { get; set; }
        public string RecuPhotoPath { get; set; } // Chemin de l'image en chaîne
        public BitmapImage PhotoUrl { get; set; } // Image convertie en BitmapImage
    }


}
