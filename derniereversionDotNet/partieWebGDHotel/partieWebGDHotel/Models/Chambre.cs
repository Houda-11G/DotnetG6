using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace partieWebGDHotel.Models
{
    public class Chambre
    {
        [Key]  // Spécifie que c'est la clé primaire
      

        public int NumChambre { get; set; }
        public string TypeChambre { get; set; }
        public int Capacite { get; set; }
        public decimal PrixParNuit { get; set; }
        public string Description { get; set; }
        public int Etage { get; set; }
        public string Statut { get; set; }
        public byte[] Image { get; set; }
       

        public ICollection<Reservation> Reservation { get; set; }
    }

   
}
