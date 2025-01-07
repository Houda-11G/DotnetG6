using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace GestionDHotel
{
    public partial class UploadReceiptWindow : Window
    {
       
        public UploadReceiptWindow(int reservationId)
        {
            InitializeComponent();
            // Traiter l'argument comme nécessaire
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tous les fichiers|*.*",
                Title = "Sélectionnez une image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Veuillez sélectionner un fichier avant de continuer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "UploadedReceipts");

                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                string fileName = Path.GetFileName(filePath);
                string newFilePath = Path.Combine(destinationPath, fileName);

                File.Copy(filePath, newFilePath, true);

                MessageBox.Show($"Reçu téléchargé avec succès vers : {newFilePath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors du téléchargement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
