using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace GestionDHotel
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs d'entrée
            string email = inputLog.Text.Trim();

            string password = inputLog2.Text.Trim();

            // Vérifier que les champs ne sont pas vides
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez entrer un email et un mot de passe.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Connexion à la base de données
            string connectionString = "Server=localhost;Database=ghotel;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Requête SQL pour vérifier les informations d'identification
                    string query = "SELECT COUNT(*) FROM users WHERE email = @Email AND password = @Password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    // Ajouter des paramètres sécurisés
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    // Vérifier si l'utilisateur existe
                    int userCount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (userCount > 0)
                    {
                        // Connexion réussie
                        Menu menuWindow = new Menu(); // Ouvre Menu.xaml
                        menuWindow.Show();

                        this.Close(); // Ferme la fenêtre Login
                    }

                    else
                    {
                        MessageBox.Show("Email ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Logique additionnelle si nécessaire
        }

        private void inputLog2_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Logique additionnelle si nécessaire
        }
    }
}
