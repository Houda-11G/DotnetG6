// Fichier : Interfaces/IReservationService.cs
public interface IReservationService
{
    bool UpdateReservationStatus(int reservationId, string status);
}
