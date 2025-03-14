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
using System.Security.AccessControl;

namespace WarehouseMenager.ViewModel
{
    public class loginPanelViewModel: ViewModelBase
    {
        private readonly userService _userService;

        public string Username { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; }
        private bool LoginAsyncBusy = false;  // flag to check if LoginAsync are still working 

        public loginPanelViewModel()
        {
            _userService = new userService();
            LoginCommand = new RelayCommand(async (_) => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            if (LoginAsyncBusy == true)
            {
                MessageBox.Show("You are logging in.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoginAsyncBusy = true; //put flag up so you cant run more than 1 LoginAsync() at same time
                userModel user;
                try
                {
                    user = await _userService.LoginAsync(Username, Password);
                }
                catch (Exception NoConnetion)
                {
                    LoginAsyncBusy=false; //put flag down
                    return;
                }

                if (user == null)
                {
                    MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoginAsyncBusy = false;  //put flag down
                    return;
                }

                // Switch to the right view based on the role
                if (user.Role == "operator")
                {
                    operatorPanelViewModel viewModel = new operatorPanelViewModel();
                    Application.Current.MainWindow.DataContext = viewModel;
                    Mediator.NotifyViewModel1FullNameChanged(user);
                    viewModel.RefreshDataAsync();  //loads tasks form DB
                }
                else if (user.Role == "manager")
                {
                    menagerPanelViewModel viewModel = new menagerPanelViewModel();
                    Application.Current.MainWindow.DataContext = viewModel;
                    Mediator.NotifyViewModel1FullNameChanged(user);
                    viewModel.RefreshDataAsync();   //loads tasks, products, ramps, locations data from databese
                }
                else
                {
                    MessageBox.Show("Unknown role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                LoginAsyncBusy = false;  //put flog down
            }
        }
    }
}

