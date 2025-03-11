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
   
    public partial class menagerPanelView : UserControl
    {
        public menagerPanelView()
        {
            InitializeComponent();
        }

        public void IsNumber(object sender, TextCompositionEventArgs e) //method for preventing input not a number in textbox for task amount input
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //method for selecting multiple rows in datagrid
        {
            if (DataContext is menagerPanelViewModel vm)
            {
                // Usunięcie odznaczonych elementów
                foreach (taskModel removed in e.RemovedItems)
                {

                    Console.WriteLine("Removed form selected list " + removed.Id);
                    vm.SelectedTasks.Remove(removed);
                    vm.DeleteCommand.RaiseCanExecuteChanged();
                }

                // Dodanie nowych zaznaczonych elementów
                foreach (taskModel added in e.AddedItems)
                {
                    if (!vm.SelectedTasks.Contains(added))
                    {
                        Console.WriteLine("Added to selected list " + added.Id);
                        vm.SelectedTasks.Add(added);
                        vm.DeleteCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }
    }
}