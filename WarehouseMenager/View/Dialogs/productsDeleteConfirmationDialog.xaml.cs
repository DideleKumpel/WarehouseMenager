using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WarehouseMenager.View.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy productsDeleteConfirmationDialog.xaml
    /// </summary>
    public partial class productsDeleteConfirmationDialog : Window
    {
        private ObservableCollection<productModel> _selectedProducts = new ObservableCollection<productModel>();
        public ObservableCollection<productModel> SelectedProducts
        {
            get
            {
                return _selectedProducts;
            }
            set
            {
                _selectedProducts = value;
            }
        }
        public bool UserConfirmed { get; private set; } = false;

        public productsDeleteConfirmationDialog(ObservableCollection<productModel> selectedProducts)
        {
            InitializeComponent();
            SelectedProducts = selectedProducts;
            DataContext = this;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = true;
            this.DialogResult = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            UserConfirmed = false;
            this.DialogResult = false;
            this.Close();
        }
    }
}
