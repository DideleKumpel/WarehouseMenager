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
    }
}