using System;
using System.Windows;

namespace GestionDHotel
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ouvrir explicitement la fenêtre Login
            Login loginWindow = new Login();
            loginWindow.Show();
        }
    }
}
