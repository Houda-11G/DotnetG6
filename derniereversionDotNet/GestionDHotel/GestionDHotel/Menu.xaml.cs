using System;
using System.Data;
using System.IO;
using System.Windows;
using OfficeOpenXml;
using MySql.Data.MySqlClient;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

using WpfMessageBox = System.Windows.MessageBox;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Windows.Threading;
namespace GestionDHotel
{
    public partial class Menu : Window
    {
        // Liste pour stocker les chambres
        private List<Chambre> chambres;
        public ObservableCollection<Reservation> Reservations { get; set; }
        private int _previousReservationCount = 0;
        private DispatcherTimer _timer;
        public Menu()
        {
            InitializeComponent();
            chambres = new List<Chambre>();
            this.Closing += Window_Closing;
        }

        private bool hasUnsavedChanges = false;

        private void OnDataModified()
        {
            hasUnsavedChanges = true;
            MessageBox.Show("Les données ont été modifiées.");
        }
        private void TextBoxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            OnDataModified();  // Appelle cette méthode pour marquer les modifications
        }
        private void OpenExcelFile(string filePath)
        {

            // Ouvrir Excel (via Process Start ou COM Interop selon votre préférence)
            System.Diagnostics.Process.Start(filePath);

            // Marquer les modifications comme ayant eu lieu
            OnDataModified();
        }

        // Fonction pour enregistrer l'Excel et mettre à jour la base de données
        private void SaveExcelFileAndDatabase(string filePath)
        {
            EmployeeExcelExporter excelExporter = new EmployeeExcelExporter();
            excelExporter.UpdateDatabaseFromExcel(filePath);
            hasUnsavedChanges = false;  // Réinitialiser l'état après l'enregistrement
        }

        // Ajouter un gestionnaire pour enregistrer ou ignorer les modifications avant de quitter
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show("La fermeture est déclenchée.");
            if (hasUnsavedChanges)  // Vérifie si des modifications ont été faites
            {
                MessageBox.Show("Modifications non enregistrées détectées, afficher l'alerte...");
                // Affiche l'alerte pour demander à l'utilisateur s'il souhaite sauvegarder
                var result = MessageBox.Show("Vous avez des modifications non enregistrées. Voulez-vous enregistrer les changements ?",
                                              "Confirmation",
                                              MessageBoxButton.YesNoCancel,
                                              MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Sauvegarder et fermer
                    string filePath = "C:\\Users\\PC\\Documents\\4emmeannee\\Dot net\\prjt dotnet\\GestionDHotel\\GestionDHotel\\EmployeeList.xlsx"; // Exemple de chemin
                    SaveExcelFileAndDatabase(filePath);  // Sauvegarde les données
                }
                else if (result == MessageBoxResult.No)
                {
                    // Continuer sans enregistrer
                    e.Cancel = false; // Permet à la fenêtre de se fermer
                }
                else
                {
                    // Annuler la fermeture
                    MessageBox.Show("annuler.");
                    e.Cancel = true;  // Annule la fermeture de la fenêtre
                }

            }
            else
            {
                MessageBox.Show("a bientôt!");
            }
            
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (hasUnsavedChanges)
                {
                    var result = MessageBox.Show("Vous avez des modifications non enregistrées. Voulez-vous enregistrer les changements?",
                        "Confirmation", MessageBoxButton.YesNoCancel);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Sauvegarder les modifications
                        string filePath = "C:\\Users\\PC\\Documents\\4emmeannee\\Dot net\\prjt dotnet\\GestionDHotel\\GestionDHotel\\EmployeeList.xlsx";
                        SaveExcelFileAndDatabase(filePath);
                    }
                    // Sinon, rien ne se passe ou vous pouvez implémenter d'autres actions
                }
            }
        }

        private void UpdateDatabase_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\PC\\Documents\\4emmeannee\\Dot net\\prjt dotnet\\GestionDHotel\\GestionDHotel\\EmployeeList.xlsx";
            EmployeeExcelExporter excelExporter = new EmployeeExcelExporter();
            excelExporter.UpdateDatabaseFromExcel(filePath);
        }

        // Fonction pour générer le fichier Excel
        private void ListeEmpl_Click(object sender, RoutedEventArgs e)
        {
            // Créer une instance de EmployeeExcelExporter
            EmployeeExcelExporter excelExporter = new EmployeeExcelExporter();

            // Définir le chemin du fichier Excel
            string filePath = "C:\\Users\\PC\\Documents\\4emmeannee\\Dot net\\prjt dotnet\\GestionDHotel\\GestionDHotel\\EmployeeList.xlsx";  // Remplacez par le chemin que vous souhaitez

            // Appeler la méthode CreateExcelFile pour générer le fichier Excel
            excelExporter.CreateExcelFile(filePath);

            // Ouvrir le fichier Excel après la création
            System.Diagnostics.Process.Start(filePath);
        }

        public class EmployeeExcelExporter
        {
            public void UpdateDatabaseFromExcel(string filePath)
            {
                try
                {
                    // Charger le fichier Excel
                    FileInfo existingFile = new FileInfo(filePath);
                    using (ExcelPackage package = new ExcelPackage(existingFile))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Première feuille Excel
                        int rowCount = worksheet.Dimension.Rows; // Nombre de lignes dans Excel

                        // Connexion à la base de données
                        string connectionString = "Server=localhost;Database=ghotel;User ID=root;Password=;";

                        // Liste pour stocker les EmployeeID à supprimer de la base de données
                        List<string> employeeIdsInExcel = new List<string>();

                        // Parcourir les lignes du fichier Excel (en ignorant l'en-tête)
                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();

                            for (int row = 2; row <= rowCount; row++) // Commence à la ligne 2 pour ignorer l'en-tête
                            {
                                string employeeID = worksheet.Cells[row, 1].Text; // EmployeeID (colonne 1)

                                if (string.IsNullOrEmpty(employeeID)) continue; // Ignore les lignes sans EmployeeID

                                employeeIdsInExcel.Add(employeeID); // Ajouter l'ID à la liste

                                string firstName = worksheet.Cells[row, 2].Text;  // FirstName (colonne 2)
                                string lastName = worksheet.Cells[row, 3].Text;   // LastName (colonne 3)
                                var dateOfBirthValue = worksheet.Cells[row, 4].GetValue<double>(); // DateOfBirth (colonne 4)
                                string gender = worksheet.Cells[row, 5].Text;     // Gender (colonne 5)
                                string position = worksheet.Cells[row, 6].Text;   // Position (colonne 6)
                                string department = worksheet.Cells[row, 7].Text; // Department (colonne 7)
                                var hireDateValue = worksheet.Cells[row, 8].GetValue<double>(); // HireDate (colonne 8)
                                string salaryText = worksheet.Cells[row, 9].Text;  // Salary (colonne 9)
                                string phone = worksheet.Cells[row, 10].Text;     // Phone (colonne 10)
                                string email = worksheet.Cells[row, 11].Text;     // Email (colonne 11)
                                string address = worksheet.Cells[row, 12].Text;   // Address (colonne 12)

                                // Convertir les valeurs numériques en dates
                                DateTime dateOfBirth = DateTime.FromOADate(dateOfBirthValue);
                                DateTime hireDate = DateTime.FromOADate(hireDateValue);

                                decimal salary;
                                if (!decimal.TryParse(salaryText, out salary))
                                {
                                    WpfMessageBox.Show($"Salaire invalide à la ligne {row}.");
                                    continue;
                                }

                                // Créer une commande pour rechercher l'employé
                                string checkQuery = @"SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";

                                // Ouvrir une connexion et effectuer la vérification dans une boucle
                                using (MySqlCommand cmd = new MySqlCommand(checkQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                                    using (MySqlDataReader reader = cmd.ExecuteReader())
                                    {
                                        if (reader.HasRows) // L'employé existe
                                        {
                                            // Lire les données existantes dans la base de données
                                            reader.Read();

                                            // Comparer chaque champ pour déterminer si une mise à jour est nécessaire
                                            bool needsUpdate = false;
                                            if (firstName != reader["FirstName"].ToString()) needsUpdate = true;
                                            if (lastName != reader["LastName"].ToString()) needsUpdate = true;
                                            if (dateOfBirth != Convert.ToDateTime(reader["DateOfBirth"])) needsUpdate = true;
                                            if (gender != reader["Gender"].ToString()) needsUpdate = true;
                                            if (position != reader["Position"].ToString()) needsUpdate = true;
                                            if (department != reader["Department"].ToString()) needsUpdate = true;
                                            if (hireDate != Convert.ToDateTime(reader["HireDate"])) needsUpdate = true;
                                            if (salary != Convert.ToDecimal(reader["Salary"])) needsUpdate = true;
                                            if (phone != reader["Phone"].ToString()) needsUpdate = true;
                                            if (email != reader["Email"].ToString()) needsUpdate = true;
                                            if (address != reader["Address"].ToString()) needsUpdate = true;

                                            // Si des différences sont trouvées, mettre à jour la base de données
                                            if (needsUpdate)
                                            {
                                                reader.Close(); // Fermer le DataReader avant d'effectuer la mise à jour

                                                // Requête de mise à jour
                                                string updateQuery = @"UPDATE Employees SET 
                                    FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, 
                                    Gender = @Gender, Position = @Position, Department = @Department, 
                                    HireDate = @HireDate, Salary = @Salary, Phone = @Phone, 
                                    Email = @Email, Address = @Address WHERE EmployeeID = @EmployeeID";

                                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                                {
                                                    updateCmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                                                    updateCmd.Parameters.AddWithValue("@FirstName", firstName);
                                                    updateCmd.Parameters.AddWithValue("@LastName", lastName);
                                                    updateCmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                                                    updateCmd.Parameters.AddWithValue("@Gender", gender);
                                                    updateCmd.Parameters.AddWithValue("@Position", position);
                                                    updateCmd.Parameters.AddWithValue("@Department", department);
                                                    updateCmd.Parameters.AddWithValue("@HireDate", hireDate);
                                                    updateCmd.Parameters.AddWithValue("@Salary", salary);
                                                    updateCmd.Parameters.AddWithValue("@Phone", phone);
                                                    updateCmd.Parameters.AddWithValue("@Email", email);
                                                    updateCmd.Parameters.AddWithValue("@Address", address);

                                                    updateCmd.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                        else // Si l'employé n'existe pas, insérer un nouvel enregistrement
                                        {
                                            reader.Close(); // Fermer le DataReader avant d'effectuer l'insertion

                                            string insertQuery = @"INSERT INTO Employees 
                                (EmployeeID, FirstName, LastName, DateOfBirth, Gender, Position, Department, 
                                 HireDate, Salary, Phone, Email, Address)
                                 VALUES (@EmployeeID, @FirstName, @LastName, @DateOfBirth, @Gender, @Position, 
                                 @Department, @HireDate, @Salary, @Phone, @Email, @Address)";

                                            using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                            {
                                                insertCmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                                                insertCmd.Parameters.AddWithValue("@FirstName", firstName);
                                                insertCmd.Parameters.AddWithValue("@LastName", lastName);
                                                insertCmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                                                insertCmd.Parameters.AddWithValue("@Gender", gender);
                                                insertCmd.Parameters.AddWithValue("@Position", position);
                                                insertCmd.Parameters.AddWithValue("@Department", department);
                                                insertCmd.Parameters.AddWithValue("@HireDate", hireDate);
                                                insertCmd.Parameters.AddWithValue("@Salary", salary);
                                                insertCmd.Parameters.AddWithValue("@Phone", phone);
                                                insertCmd.Parameters.AddWithValue("@Email", email);
                                                insertCmd.Parameters.AddWithValue("@Address", address);

                                                insertCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }

                            // Étape 2: Supprimer les employés qui sont dans la base de données mais pas dans l'Excel
                            // Récupérer tous les EmployeeID dans la base de données
                            string selectAllQuery = "SELECT EmployeeID FROM Employees";
                            List<string> employeeIdsInDatabase = new List<string>();

                            using (MySqlCommand selectCmd = new MySqlCommand(selectAllQuery, conn))
                            {
                                using (MySqlDataReader reader = selectCmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        employeeIdsInDatabase.Add(reader["EmployeeID"].ToString());
                                    }
                                }
                            }

                            // Comparer les IDs et supprimer ceux qui ne sont plus dans l'Excel
                            foreach (var employeeIdInDatabase in employeeIdsInDatabase)
                            {
                                if (!employeeIdsInExcel.Contains(employeeIdInDatabase)) // Si l'ID n'est pas dans l'Excel
                                {
                                    string deleteQuery = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";
                                    using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                                    {
                                        deleteCmd.Parameters.AddWithValue("@EmployeeID", employeeIdInDatabase);
                                        deleteCmd.ExecuteNonQuery();
                                    }

                                    WpfMessageBox.Show($"L'employé avec l'ID {employeeIdInDatabase} a été supprimé.");
                                }
                            }

                            conn.Close();
                            WpfMessageBox.Show("Mise à jour de la base de données terminée.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show($"Erreur lors de la mise à jour de la base de données : {ex.Message}");
                }
            }

            // Crée un fichier Excel et l'enregistre à l'emplacement spécifié
            public void CreateExcelFile(string filePath)
            {
                try
                {
                    // Charger les données des employés depuis la base de données
                    DataTable employeeData = LoadEmployeeData();
                    if (employeeData == null || employeeData.Rows.Count == 0)
                    {
                        WpfMessageBox.Show("Aucune donnée d'employé à exporter.");
                        return;
                    }

                    // Créer un nouveau fichier Excel
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // Vérifier que l'objet package n'est pas null
                    ExcelPackage package = new ExcelPackage();
                    if (package == null)
                    {
                        WpfMessageBox.Show("Erreur lors de la création du fichier Excel : l'initialisation du package a échoué.");
                        return;
                    }

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Employees");

                    // Ajouter l'en-tête
                    // Ajouter les en-têtes des nouvelles colonnes
                    worksheet.Cells[1, 1].Value = "EmployeeID";
                    worksheet.Cells[1, 2].Value = "FirstName";
                    worksheet.Cells[1, 3].Value = "LastName";
                    worksheet.Cells[1, 4].Value = "DateOfBirth";
                    worksheet.Cells[1, 5].Value = "Gender";
                    worksheet.Cells[1, 6].Value = "Position";
                    worksheet.Cells[1, 7].Value = "Department";
                    worksheet.Cells[1, 8].Value = "HireDate";
                    worksheet.Cells[1, 9].Value = "Salary";
                    worksheet.Cells[1, 10].Value = "Phone";
                    worksheet.Cells[1, 11].Value = "Email";
                    worksheet.Cells[1, 12].Value = "Address";


                    // Ajouter les données des employés
                    for (int i = 0; i < employeeData.Rows.Count; i++)
                    {
                        for (int j = 0; j < employeeData.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1].Value = employeeData.Rows[i][j];
                        }
                    }

                    // Sauvegarder le fichier
                    FileInfo file = new FileInfo(filePath);
                    package.SaveAs(file);

                    WpfMessageBox.Show("Le fichier Excel a été créé avec succès.");
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show($"Erreur lors de la création du fichier Excel: {ex.Message}");
                }
            }

                // Charger les données des employés depuis la base de données
                private DataTable LoadEmployeeData()
            {
                DataTable dataTable = new DataTable();
                string connectionString = "Server=localhost;Database=ghotel;User ID=root;Password=;";
                string query = "SELECT * FROM Employees";

                try
                {
                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count == 0)
                        {
                            WpfMessageBox.Show("Aucun employé trouvé dans la base de données.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show("Erreur de connexion à la base de données: " + ex.Message);
                }

                return dataTable;
            }
        }

        // Événement de changement de sélection dans le ComboBox
        private void TypeChambreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Charger les chambres en fonction du type sélectionné
            LoadChambres();
        }

        // Charger les chambres de la base de données
        private void LoadChambres()
        {
            string connectionString = "Server=localhost;Database=ghotel;User ID=root;Password=;Pooling=true;";
            MySqlConnection conn = new MySqlConnection(connectionString);

            string query = "SELECT * FROM Chambre";

            // Appliquer un filtre si un type est sélectionné
            if (TypeChambreComboBox.SelectedItem != null)
            {
                string selectedType = (TypeChambreComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                query += " WHERE TypeChambre = @TypeChambre";
            }

            try
            {
                conn.Open();
                MySqlCommand command = new MySqlCommand(query, conn);

                // Ajouter un paramètre si un type est sélectionné
                if (TypeChambreComboBox.SelectedItem != null)
                {
                    string selectedType = (TypeChambreComboBox.SelectedItem as ComboBoxItem).Content.ToString();
                    command.Parameters.AddWithValue("@TypeChambre", selectedType);
                }

                MySqlDataReader reader = command.ExecuteReader();
                List<Chambre> chambres = new List<Chambre>();

                while (reader.Read())
                {
                    chambres.Add(new Chambre
                    {
                        NumChambre = reader["NumChambre"].ToString(),
                        TypeChambre = reader["TypeChambre"].ToString(),
                        Capacite = reader.IsDBNull(reader.GetOrdinal("Capacite")) ? 0 : (int)reader["Capacite"],
                        PrixParNuit = reader.IsDBNull(reader.GetOrdinal("PrixParNuit")) ? 0 : (decimal)reader["PrixParNuit"],
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader["Description"].ToString(),
                        Etage = reader.IsDBNull(reader.GetOrdinal("Etage")) ? 0 : (int)reader["Etage"],
                        Statut = reader.IsDBNull(reader.GetOrdinal("Statut")) ? "" : reader["Statut"].ToString(),
                        Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : ConvertToImageSource((byte[])reader["Image"])
                    });
                }

                ChambresDataGrid.ItemsSource = chambres; // Mettre à jour la source des données
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show("Erreur : " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Convertir un tableau d'octets en ImageSource
        private ImageSource ConvertToImageSource(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                return BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        private void GestionChambres_Click(object sender, RoutedEventArgs e)
        {
            // Exemple d'action à exécuter lors du clic sur le bouton
            // Vous pouvez ici appeler la méthode LoadChambres ou ouvrir une nouvelle fenêtre pour gérer les chambres
            LoadChambres();
        }

        // Afficher un formulaire de chambre
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Chambre formChambre = new Chambre();
            formChambre.ShowDialog();
        }
    

        private void NotificationIcon_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de notifications
            Notif notifWindow = new Notif();
            notifWindow.ShowDialog();
        }
    }
    public class Reservation
    {
        public int ID_Reservation { get; set; }
        public int NumChambre { get; set; }
        public int ClientId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string Statut { get; set; }
        public DateTime DateCreation { get; set; }
        public decimal Prix { get; set; }
    }
}