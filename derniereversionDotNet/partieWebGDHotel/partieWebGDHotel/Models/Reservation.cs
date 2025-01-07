using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace partieWebGDHotel.Models
{
    public class Reservation
    {
        [Key]
        public int ID_Reservation { get; set; }

        [Required]
        public int NumChambre { get; set; }

        [Required]
        public int ClientId { get; set; }

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }

        [Required]
        [StringLength(50)]
        public string Statut { get; set; }

        public DateTime DateCreation { get; set; }
        public decimal Prix { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        // Ajout de la colonne RecuPhotoPath
        public byte[]? RecuPhotoPath { get; set; }


        // Relations
        [ForeignKey("NumChambre")]
        public Chambre Chambre { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }
    }
}
