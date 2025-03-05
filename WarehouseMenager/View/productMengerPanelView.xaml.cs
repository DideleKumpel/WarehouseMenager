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
    /// Logika interakcji dla klasy productMengerPanelView.xaml
    /// </summary>
    public partial class productMengerPanelView : UserControl
    {
        public productMengerPanelView()
        {
            InitializeComponent();
        }

        public void IsNumber(object sender, TextCompositionEventArgs e) //method for preventing input not a number in textbox for task amount input
        {
            TextBox textBox = sender as TextBox;

            bool isNumber = e.Text.All(char.IsDigit);
            bool isDot = e.Text == ",";

            // check if comma already are in text
            bool containsDot = textBox.Text.Contains(",");

            // accept of number or comma if text doesnt already conatin it
            e.Handled = !(isNumber || (isDot && !containsDot));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) //method for selecting multiple rows in datagrid
        {
            if (DataContext is productMengerPanelViewModel vm)
            {
                // Usunięcie odznaczonych elementów
                foreach (productModel removed in e.RemovedItems)
                {

                    Console.WriteLine("Removed form selected list " + removed.Barcode);
                    vm.SelectedProducts.Remove(removed);
                    //vm.DeleteCommand.RaiseCanExecuteChanged();
                }

                // Dodanie nowych zaznaczonych elementów
                foreach (productModel added in e.AddedItems)
                {
                    if (!vm.SelectedProducts.Contains(added))
                    {
                        Console.WriteLine("Added to selected list " + added.Barcode);
                        vm.SelectedProducts.Add(added);
                        //vm.DeleteCommand.RaiseCanExecuteChanged();
                    }
                }
            }
        }
    }
}
