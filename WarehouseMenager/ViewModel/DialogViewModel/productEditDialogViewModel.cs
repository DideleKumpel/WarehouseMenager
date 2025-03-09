using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WarehouseMenager.Model;
using WarehouseMenager.MVVM;
using WarehouseMenager.Service;


namespace WarehouseMenager.ViewModel.DialogViewModel
{
    class productEditDialogViewModel: ViewModelBase
    {
        public ObservableCollection<productModel> OrginalProduct { get; set; }
        public ObservableCollection<productModel> EditProduct { get; set; }

        public ICommand CancelCommand => new RelayCommand(execute => Cancel());
        public ICommand SaveCommand => new RelayCommand(execute => Save(), canExecute => DataWAsChanged());

        private Window _dialog;

        public productEditDialogViewModel (productModel orginal, Window dialog)
        {
            OrginalProduct = new ObservableCollection<productModel>();
            EditProduct = new ObservableCollection<productModel>();
            this._dialog = dialog;
            this.OrginalProduct.Add(orginal);
            this.EditProduct.Add(new productModel
            {
                Barcode = orginal.Barcode,
                Category = orginal.Category,
                Name = orginal.Name,
                Description = orginal.Description,
                Weight = orginal.Weight
            });
            OnPropertChanged(nameof(EditProduct));
        }
        
        public void UpdateDataGrids()
        {
            OnPropertChanged(nameof(EditProduct));
            OnPropertChanged(nameof(OrginalProduct));
        }

        public void Cancel()
        {
            _dialog.DialogResult = false;
            _dialog.Close();
        }

        public void Save()
        {
            _dialog.DialogResult = true;
            _dialog.Close();
        }

        public bool DataWAsChanged()  //check if user changed any data in the form (barcode cant be changed)
        {
            bool NameEqual = OrginalProduct[0].Name == EditProduct[0].Name;
            bool CategoryEqual = OrginalProduct[0].Category == EditProduct[0].Category;
            bool DescrptionEqual = OrginalProduct[0].Description == EditProduct[0].Description;
            bool WeightEquual = OrginalProduct[0].Weight == EditProduct[0].Weight;
            if(NameEqual && CategoryEqual && DescrptionEqual && WeightEquual)
            {
                return false;
            }
            return true;
        }

    }
}
