using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarehouseMenager.Model;
using WarehouseMenager.MVVM;
using WarehouseMenager.Service;
using WarehouseMenager.ViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WarehouseMenager
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new loginPanelViewModel();

            //For testing operatorPanel
            //userService _userService = new userService();
            //userModel user = new userModel { Name = "Mike", Lastname = "Johnson", Username = "mjohnson", Password = "secure123" };
            //operatorPanelViewModel VM = new operatorPanelViewModel();
            //DataContext = VM;
            //Mediator.NotifyViewModel1FullNameChanged(user);
            //VM.RefreshDataAsync();
        }
    }
}
