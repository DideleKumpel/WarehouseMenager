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
using WarehouseMenager.ViewModel;

namespace WarehouseMenager.View
{
    /// <summary>
    /// Logika interakcji dla klasy operatorPanelView.xaml
    /// </summary>
    public partial class operatorPanelView : UserControl
    {
        public operatorPanelView()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //method for selecting multiple rows in datagrid
        {
            if (DataContext is operatorPanelViewModel vm)
            {
                // Delete uncheck elements
                foreach (taskModel removed in e.RemovedItems)
                {

                    Console.WriteLine("Removed form selected list " + removed.Id);
                    vm.SelectedTasks.Remove(removed);
                    // notify commands that can execute has changed
                    vm.FinishTaskCommand.RaiseCanExecuteChanged();
                    vm.AbandonTaskCommand.RaiseCanExecuteChanged();
                    vm.AssigneTaskCommand.RaiseCanExecuteChanged();
                }

                // Add check element
                foreach (taskModel added in e.AddedItems)
                {
                    if (!vm.SelectedTasks.Contains(added))
                    {
                        Console.WriteLine("Added to selected list " + added.Id);
                        vm.SelectedTasks.Add(added);
                        // notify commands that can execute has changed
                        vm.FinishTaskCommand.RaiseCanExecuteChanged();
                        vm.AbandonTaskCommand.RaiseCanExecuteChanged();
                        vm.AssigneTaskCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }
    }
}
