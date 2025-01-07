using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows.Media;

namespace GestionDHotel
{
    public partial class Chambre : Window
    {
        public string NumChambre { get; set; }
        public string TypeChambre { get; set; }
        public int Capacite { get; set; }
        public decimal PrixParNuit { get; set; }
        public string Description { get; set; }
        public int Etage { get; set; }
        public string Statut { get; set; }
        public ImageSource Image { get; set; }

        public Chambre()
        {
            InitializeComponent();
        }

        private void ChoisirImage_Click(object sender, RoutedEventArgs e)
        {
            // Créer un OpenFileDialog pour sélectionner une image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"; // Types d'images autorisés

            // Si l'utilisateur sélectionne une image et appuie sur OK
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;  // Récupérer le chemin de l'image
                                                            // Afficher l'image dans le contrôle Image
                BitmapImage bitmap = new BitmapImage(new Uri(filePath));
                ChambreImage.Source = bitmap;

                // Vous pouvez stocker le chemin de l'image dans un champ pour l'utiliser plus tard, si nécessaire
                // Par exemple :
                // imagePath = filePath;
            }
        }

        private void AjouterChambre_Click(object sender, RoutedEventArgs e)
        {
            string numChambre = NumChambreTextBox.Text;
            string typeChambre = ((ComboBoxItem)TypeChambreComboBox.SelectedItem).Content.ToString();
            int capacite = int.Parse(CapaciteTextBox.Text);
            decimal prixParNuit = decimal.Parse(PrixParNuitTextBox.Text);
            string description = DescriptionTextBox.Text;
            int etage = int.Parse(EtageTextBox.Text);
            string statut = ((ComboBoxItem)StatutComboBox.SelectedItem).Content.ToString();

            // Récupérer l'image sous forme binaire
            byte[] imageBytes = null;
            if (ChambreImage.Source != null)
            {
                BitmapImage bitmapImage = (BitmapImage)ChambreImage.Source;
                using (MemoryStream ms = new MemoryStream())
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(ms);
                    imageBytes = ms.ToArray(); // Image sous forme binaire
                }
            }

            // Connexion à la base de données MySQL
            string connectionString = "Server=localhost;Database=ghotel;User ID=root;Password=;Pooling=true;";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();

                string query = "INSERT INTO Chambre (NumChambre, TypeChambre, Capacite, PrixParNuit, Description, Etage, Statut, Image) " +
                               "VALUES (@NumChambre, @TypeChambre, @Capacite, @PrixParNuit, @Description, @Etage, @Statut, @Image)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NumChambre", numChambre);
                cmd.Parameters.AddWithValue("@TypeChambre", typeChambre);
                cmd.Parameters.AddWithValue("@Capacite", capacite);
                cmd.Parameters.AddWithValue("@PrixParNuit", prixParNuit);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Etage", etage);
                cmd.Parameters.AddWithValue("@Statut", statut);
                cmd.Parameters.AddWithValue("@Image", imageBytes); // Insérer l'image binaire

                cmd.ExecuteNonQuery();
                MessageBox.Show("Chambre ajoutée avec succès !");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
