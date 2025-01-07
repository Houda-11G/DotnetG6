namespace partieWebGDHotel.Models
{
    public class DashboardViewModel
    {
        public List<Chambre> Chambres { get; set; }
        public int? ReservationId { get; set; }
        public string? SuccessMessage { get; set; }
        public UserInfo User { get; set; }
    }

    public class UserInfo
    {
        public string Nom { get; set; }    // Contiendra Prénom + Nom
        public string Email { get; set; }   // Contiendra email
    }

}
