// Fichier : Services/ReservationService.cs
using MySql.Data.MySqlClient;

public class ReservationService : IReservationService
{
    public bool UpdateReservationStatus(int reservationId, string status)
    {
        // Exemple de mise à jour dans la base de données
        using (var connection = new MySqlConnection("Server=localhost;Database=ghotel;User ID=root;Password=;Pooling=true;"))
        {
            string query = "UPDATE Reservations SET Status = @Status WHERE ID_Reservation = @ReservationId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@ReservationId", reservationId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
