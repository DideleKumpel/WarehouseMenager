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
using WarehouseMenager.View.Dialogs;
using WarehouseMenager.ViewModel.DialogViewModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WarehouseMenager.ViewModel
{
    public class productMengerPanelViewModel : ViewModelBase
    {
        public userModel User;

        //SERVICE METHODS FOR DB CONECTION
        private readonly productService _productService;
        private readonly userService _userService;

        //DISPLAY AND DB DATA VARIABULES
        public ObservableCollection<productModel> Products { get; set; }
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
        private string Username;
        public string UsernameDisplay
        {
            get { return Username; }
            set { }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value.ToUpper();
                OnPropertChanged(nameof(Name));
                OnDataForAddProductChanged();
                if (_name != null)
                {
                    Console.WriteLine("Inputed name- " + _name);
                }
            }
        }
        private double weightDouble;
        private string _weight;
        public string Weight
        {
            get { return _weight; }
            set
            {
                _weight = value.ToUpper();
                OnPropertChanged(nameof(Weight));
                OnDataForAddProductChanged();
                Console.WriteLine("Inputed weight- " + _weight);
            }
        }
        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value.ToUpper();
                OnPropertChanged(nameof(Category));
                OnDataForAddProductChanged();
                if (_category != null)
                {
                    Console.WriteLine("Inputed category- " + _category);
                }

            }
        }
        private string _barcode;
        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value.ToUpper();
                OnPropertChanged(nameof(Barcode));
                OnDataForAddProductChanged();
                if (_barcode != null)
                {
                    Console.WriteLine("Inputed barcode- " + _barcode);
                }
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertChanged(nameof(Description));
                OnDataForAddProductChanged();
                if (_description != null)
                {
                    Console.WriteLine("Inputed description- " + _description);
                }
            }
        }

        //BUTTONS

        public ICommand LogOutCommand => new RelayCommand(execute => LogOut());
        public AsyncRelayCommand AddCommand { get; }
        public AsyncRelayCommand DeleteCommand { get; }
        public AsyncRelayCommand EditCommand { get; }
        public ICommand RefreshCommand => new RelayCommand(execute => RefreshDataAsync());
        public ICommand SwitchTaskMengerView => new RelayCommand(execute => SwitchViewToTaskPanel());
        public ICommand SwitchOperatorPanelView => new RelayCommand(execute => SwitchViewToOperatorPanel());


        //FLAGS FOR BTN SO ONLY 1 CAN RUN IN THE SAME TIME
        private bool AddProductAsyncBusy = false;
        private bool DeleteProductAsyncBusy = false;
        private bool EditProductAsyncBusy = false;


        //CONSTURCTOR
        public productMengerPanelViewModel()
        {
            Mediator.UserDataPass += UserDataTransfer; //add fuction to event 

            _productService = new productService();
            _userService = new userService();

            AddCommand = new AsyncRelayCommand(async () => await AddProductAsync(), () => AddProductInputFilled());
            DeleteCommand = new AsyncRelayCommand(async () => await DeleteProductsAsync(), () => ProductsAreSeltected());
            EditCommand = new AsyncRelayCommand(async () => await EditProductAsync(), () => EditProductInputFilled());

        }

        //USER DATA AND VERIFICATION
        private void UserDataTransfer(userModel userData) //method to save user data from loginPanel and verificate it with DB
        {
            this.User = userData;
            Username = User.Name + " " + User.Lastname;
            OnPropertChanged(nameof(UsernameDisplay));
        }
        private async Task VerifyUser()
        {
            try
            {
                this.User = await _userService.LoginAsync(User.Username, User.Password);
                if (this.User.Role != "manager")
                {
                    MessageBox.Show("Invalid role. Loging out", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogOut();
                }
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }

        // METHODS TO LOAD DATA FROM DB
        public async void RefreshDataAsync() //refresh and download new data
        {
            await VerifyUser();
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                Products = await _productService.LoadProductsAsync();
                OnPropertChanged(nameof(Products));
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }

        // METHODS FOR ADDING PRODUCTS
        private async Task AddProductAsync()
        {
            if(AddProductAsyncBusy == true)
            {
                MessageBox.Show("You are arleady adding product.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            await VerifyUser();
            AddProductAsyncBusy = true;

            try
            {
                if (await _productService.ProductBarcodeExistAsync(_barcode))
                {
                    MessageBox.Show("Item with this barcode already exist", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddProductAsyncBusy = false;
                    return;
                }
                if (await _productService.ProductAlreadyExistAsync(_name, _category, _description, weightDouble))
                {
                    MessageBox.Show("Item already exist", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddProductAsyncBusy = false;
                    return;
                }
            }
            catch (Exception NoConnect) {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                AddProductAsyncBusy = false;
                LogOut();
            }
            //open messebox with confrmtaion of acction
            MessageBoxResult result = MessageBox.Show(
               $"Product:" + " Name-" + _name + " Weight-" + _weight + " Category-" + _category + " Barcode-" + _barcode + " \nAre you sure you want to add this product?",
               "Confirm Task Addition",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            // If user press 'No"
            if (result != MessageBoxResult.Yes)
            {
                AddProductAsyncBusy = false;
                return;
            }

            bool AddProductSucces = await _productService.ProductInsertAsync(_barcode, _name, _category, _description, weightDouble);
            if (AddProductSucces  == false) //If error occured
            {
                MessageBox.Show("Error adding product to DB.", "Error", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Product adding complet with succes", "Info", MessageBoxButton.OK);
            }
            AddProductAsyncBusy = false;
            RefreshDataAsync();
        }

        private bool AddProductInputFilled()
        {
            bool AllFieldFilled = true;
            if( String.IsNullOrEmpty( _name) )
            {
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            if(String.IsNullOrEmpty(_category))
            {
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            if (String.IsNullOrEmpty(_barcode)) 
            { 
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            if (String.IsNullOrEmpty(_category)) { 
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            if(String.IsNullOrEmpty(_description))
            {
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            try
            {
                weightDouble = double.Parse(_weight);
            }
            catch (Exception ex)
            {
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            if (weightDouble <= 0)
            {
                AllFieldFilled = false;
                return AllFieldFilled;
            }
            return AllFieldFilled;
        }
        private void OnDataForAddProductChanged()
        {
            AddCommand.RaiseCanExecuteChanged();
        }

        // METHODS FOR DELETING PRODUCTS

        private async Task DeleteProductsAsync()
        {
            if (DeleteProductAsyncBusy == true)
            {
                MessageBox.Show("You are arleady deleting product.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await VerifyUser();
            ObservableCollection<productModel> Products = this.SelectedProducts; 
            DeleteProductAsyncBusy = true;

            productsDeleteConfirmationDialog conformationDialog = new productsDeleteConfirmationDialog(Products);
            bool? result = conformationDialog.ShowDialog();
            if (result == false) 
            {
                DeleteProductAsyncBusy = false;
                return;
            }

            int AmountOfErrors = 0;
            List<string> NotDeletedBarcodes = new List<string> { };

            foreach(var item in Products)
            {
                bool succes = await _productService.DeleteProductsAsync(item.Barcode);
                if(succes == false)
                {
                    AmountOfErrors++;
                    NotDeletedBarcodes.Add(item.Barcode);
                }
            }
            if (AmountOfErrors > 0) //If error occured
            {
                string Message = "Not deleted products barcodes: ";
                foreach(var item in NotDeletedBarcodes)
                {
                    Message += (item + ", ");
                }
                MessageBox.Show("Error with deleting " + AmountOfErrors  + " products. \n" + Message, "Error", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Products removal was sucesful", "Info", MessageBoxButton.OK);
            }
            DeleteProductAsyncBusy = false;
            RefreshDataAsync();

        }
        private bool ProductsAreSeltected()
        {
            if (this._selectedProducts.Count > 0)
            {
                return true;
            }
            return false;
        }
        private void OnDataForProductDeleteChange()
        {
            DeleteCommand.RaiseCanExecuteChanged();
        }


        // METHODS FOR EDITING PRODUCTS
        private async Task EditProductAsync()
        {
            if (DeleteProductAsyncBusy == true)
            {
                MessageBox.Show("You are arleady editing product.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            productModel ProductToEdit = this._selectedProducts[0];
            await VerifyUser();
            EditProductAsyncBusy = true;
            var editWindow = new productEditDialog();
            var editViewModel = new productEditDialogViewModel(ProductToEdit, editWindow);
            editWindow.DataContext = editViewModel;

            bool? dialogResult = editWindow.ShowDialog();

            if (dialogResult == true) // user accept changes
            {
                productModel updatedProduct = editViewModel.EditProduct[0];
                bool succes = await _productService.UpdateProductAsync( ProductToEdit.Barcode, updatedProduct.Name, updatedProduct.Category, updatedProduct.Description, updatedProduct.Weight);
                if (succes == false)
                {
                    MessageBox.Show("Error with updating product.", "Error", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Product update was sucesful", "Info", MessageBoxButton.OK);
                }
            }

            RefreshDataAsync(); //refresh data
            EditProductAsyncBusy = false; //flag down


        }
        private bool EditProductInputFilled()
        {
            if (this._selectedProducts.Count == 1)
            {
                return true;
            }
            return false;
        }
        private void OnDataForEditProductChanged()
        {
            EditCommand.RaiseCanExecuteChanged();
        }

        // OTHER METHODS
        private void LogOut()
        {
            Application.Current.MainWindow.DataContext = new loginPanelViewModel();
        }
        private void SwitchViewToTaskPanel() 
        {
            menagerPanelViewModel viewModel = new menagerPanelViewModel();
            Application.Current.MainWindow.DataContext = viewModel;
            Mediator.NotifyViewModel1FullNameChanged(User);
            viewModel.RefreshDataAsync();   //loads tasks, products, ramps, locations data from databese
        }
        private void SwitchViewToOperatorPanel() 
        {
            
        }

    }
}
