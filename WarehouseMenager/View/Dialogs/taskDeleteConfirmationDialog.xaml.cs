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
using System.Windows.Shapes;
using WarehouseMenager.Model;

namespace WarehouseMenager.View.Dialogs
{
    public partial class taskDeleteConfirmationDialog : Window
    {

        private ObservableCollection<taskModel> _selectedTasks = new ObservableCollection<taskModel>();
        public ObservableCollection<taskModel> SelectedTasks
        {
            get
            {
                return _selectedTasks;
            }
            set
            {
                _selectedTasks = value;
            }
        }
        public bool UserConfirmed { get; private set; } = false;

        internal taskDeleteConfirmationDialog(ObservableCollection<taskModel> selectedTasks)
        {
            InitializeComponent();
            SelectedTasks = selectedTasks;
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
