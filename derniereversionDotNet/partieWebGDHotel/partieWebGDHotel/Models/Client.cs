using System.ComponentModel.DataAnnotations;

namespace partieWebGDHotel.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le prénom ne doit pas dépasser 50 caractères.")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Veuillez entrer un email valide.")]
        [StringLength(100)] // Limite de longueur de l'email
        public string email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string mot_de_passe { get; set; }

        // Date d'inscription, la valeur par défaut est la date actuelle
        public DateTime Date_Inscription { get; set; } = DateTime.Now;
    }
}
