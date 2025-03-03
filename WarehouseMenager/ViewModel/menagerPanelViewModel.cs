using Mysqlx.Prepare;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WarehouseMenager.Model;
using WarehouseMenager.MVVM;
using WarehouseMenager.Service;
using WarehouseMenager.View;

namespace WarehouseMenager.ViewModel
{
    internal class menagerPanelViewModel: ViewModelBase
    {
        public userModel User;
        private readonly taskService _taskService;
        private readonly locationsServise _locationsServise;
        private readonly rampService _rampService;
        private readonly productService _productService;
        public ObservableCollection<taskModel> Tasks { get; set; }
        public ObservableCollection<productModel> Products { get; set; }
        public ObservableCollection<rampModel> Ramps { get; set; }

        private ObservableCollection<taskModel> _selectedTasks;
        public ObservableCollection<taskModel> SelectedTasks
        {
            get { return _selectedTasks; }
            set
            {
                _selectedTasks = value;
                OnPropertChanged();
                Console.WriteLine("Task selected " + _selectedTasks[0].Id + " ");
            }
        }
        private rampModel _selectedRamp;
        public rampModel SelectedRamp
        {
            get { return _selectedRamp; }
            set
            {
                _selectedRamp = value;
                OnPropertChanged(nameof(SelectedRamp));
                Console.WriteLine("Ramp seleted " + _selectedRamp.Name);
            }
        }
        private productModel _selectedProduct;
        public productModel SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertChanged(nameof(SelectedProduct));
                Console.WriteLine("Product selected " + _selectedProduct.Barcode);
            }
        }
        
        private bool _unloadBtn;
        public bool UnloadBtn { 
            get 
            {
                return _unloadBtn; 
            } 
            set 
            { 
                _unloadBtn = value; 
                Console.WriteLine("Unload button checked- " + _unloadBtn); 
            }
        }
        private bool _loadBtn;
        public bool LoadBtn
        {
            get
            {
                return _loadBtn;
            }
            set
            {
                _loadBtn = value;
                Console.WriteLine("Load buttnon checked- " + _loadBtn);
            }
        }
        private int _amountInput;
        public string AmountInput
        {
            get
            {
                return _amountInput.ToString();
            }
            set
            {
                _amountInput = Int32.Parse(value);
                Console.WriteLine("Amount- " + _amountInput);

            }
        }

        private string Username;
        public string UsernameDisplay { get { return Username; } }

        private int FreeSpacesInWarehous;
        private int AllSpacesInWarehouse;
        public string SpaceRatioInWarehouseDisplay { get; set; }
        public double FillnesProcentage { get; set; }

        public ICommand LoginOutCommand { get; }
        public ICommand AddCommand => new RelayCommand(execute => AddTasksAsync(), canExecute => AddTaskInputFilled());
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand => new RelayCommand(execute => RefreshDataAsync());
        public ICommand SwitchPorductMengerView { get; }
        public ICommand SwitchOperatorPanelView { get; }
        public menagerPanelViewModel() {
            Mediator.UserDataPass += UserDataTransfer; //add fuction to event 
            _taskService = new taskService();
            _locationsServise = new locationsServise();
            _rampService = new rampService();
            _productService = new productService();
        }

        private void UserDataTransfer(userModel userData) //function to save user data from loginPanel
        {
            this.User = userData;
            Username = User.Name + " " + User.Lastname;
            OnPropertChanged(nameof(UsernameDisplay));
        }

        public async void RefreshDataAsync() //refresh and download new data
        {
            await LoadTasksAsync();
            await LoadProductsAsync();
            await CalculateLocationInfoForDisplay();
            await LoadRampsAsync();
        }
        private async Task LoadTasksAsync()
        {
            try
            {
                this.Tasks = await _taskService.LoadTaskAsync();
                OnPropertChanged(nameof(Tasks));
            }catch(Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }
        private async Task LoadProductsAsync()
        {
            this.Products = await _productService.LoadProductsAsync();
            OnPropertChanged(nameof(Products));
        }

        private async Task LoadRampsAsync()
        {
            this.Ramps = await _rampService.LoadRampsAsync();
            OnPropertChanged(nameof(Ramps));
        }
        private async Task AddTasksAsync()
        {
            string TaskType;
            if(_loadBtn != true)
            {
                TaskType = "load";
            }
            else
            {
                TaskType = "unload";
            }
            

        }
        private bool AddTaskInputFilled() //function to check is all input fields are filled with valid data
        {
            bool AllFieldsFilled=true;
            if(this._amountInput <= 0) //check if amount input isn't valid
            {
                AllFieldsFilled = false;
            }
            if(this._selectedRamp == null) //ramp isn't selected
            {
                AllFieldsFilled = false;
            }
            if (this._selectedProduct == null) //product isn't selected
            {
                AllFieldsFilled = false;
            }
            if((_unloadBtn == false || _loadBtn == false) && (_unloadBtn == _loadBtn) ) //radio button isn't check
            {
                AllFieldsFilled = false;
            }
            return AllFieldsFilled;
        }
        private async Task DeleteTaskAsync()
        {

        }
        private void LogOut()
        {

        }
        private async Task CalculateLocationInfoForDisplay() //function to prepare data for display of free to all ratio and filness bar
        {
            await CountFreeWarehousesSpaces();
            await CountNumberOfSpaces();
            int OcucpiedSpaces = this.AllSpacesInWarehouse - this.FreeSpacesInWarehous;
            this.SpaceRatioInWarehouseDisplay = OcucpiedSpaces + "/" + this.AllSpacesInWarehouse + " - " + this.FreeSpacesInWarehous + " Free spaces";
            this.FillnesProcentage = ((double)OcucpiedSpaces / (double)this.AllSpacesInWarehouse)*100;
            OnPropertChanged(nameof(SpaceRatioInWarehouseDisplay));
            OnPropertChanged(nameof(FillnesProcentage));
        }
        private async Task CountFreeWarehousesSpaces()
        {
            this.FreeSpacesInWarehous = await _locationsServise.LoadNumberEmptyLocationsAsync();
        }
        private async Task CountNumberOfSpaces()
        {
            this.AllSpacesInWarehouse = await _locationsServise.LoadNumberOfLocationsAsync();
        }
    }
}
