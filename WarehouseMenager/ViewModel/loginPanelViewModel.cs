using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WarehouseMenager.MVVM;
using WarehouseMenager.Service;
using WarehouseMenager.Model;

namespace WarehouseMenager.ViewModel
{
    internal class loginPanelViewModel: ViewModelBase
    {
        private readonly userService _userService;

        public string Username { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; }

        public loginPanelViewModel()
        {
            _userService = new userService();
            LoginCommand = new RelayCommand(async (_) => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            userModel user;
            try
            {
                user = await _userService.LoginAsync(Username, Password);
            }
            catch (Exception NoConnetion) {
                return;     
            }

            if (user == null)
            {
                MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Przejście do odpowiedniego widoku na podstawie roli
            if (user.Role == "operator")
            {
                Application.Current.MainWindow.DataContext = new operatorPanelViewModel();
            }
            else if (user.Role == "menager")
            {
                Application.Current.MainWindow.DataContext = new menagerPanelViewModel();
            }
            else
            {
                MessageBox.Show("Unknown role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

