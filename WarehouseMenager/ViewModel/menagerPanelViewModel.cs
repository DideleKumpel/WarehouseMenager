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
        //SERVICE METHODS FOR DB CONECTION
        private readonly taskService _taskService;
        private readonly locationsServise _locationsServise;
        private readonly rampService _rampService;
        private readonly productService _productService;
        private readonly userService _userService;
        //DISPLAY AND DB DATA VARIABULES
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
                OnPropertChanged(nameof(SelectedTasks));
                OnDataForAddTaskChagned();
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
                OnDataForAddTaskChagned();
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
                OnDataForAddTaskChagned();
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
                OnDataForAddTaskChagned();
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
                OnDataForAddTaskChagned();
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
                OnDataForAddTaskChagned();
                Console.WriteLine("Amount- " + _amountInput);
            }
        }

        private string Username;
        public string UsernameDisplay { get { return Username; } }
        private int FreeSpacesInWarehous;
        private int AllSpacesInWarehouse;
        public string SpaceRatioInWarehouseDisplay { get; set; }
        public double FillnesProcentage { get; set; }
        //BUTTONS
        public ICommand LoginOutCommand { get; }
        public AsyncRelayCommand AddCommand { get; } 
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand => new RelayCommand(execute => RefreshDataAsync());
        public ICommand SwitchPorductMengerView { get; }
        public ICommand SwitchOperatorPanelView { get; }
        //FLAGS FOR BTN SO ONLY 1 CAN RUN IN THE SAME TIME
        private bool AddTaskAsyncBusy = false;
        private bool DeleteTasAsynckBusy = false;
        private bool RefreshTaskAsyncBusy = false;
        private bool SwitchPorductMengerViewBusy = false;
        private bool SwitchOperatorPanelViewBusy = false;

        public menagerPanelViewModel() {
            Mediator.UserDataPass += UserDataTransfer; //add fuction to event 
            _taskService = new taskService();
            _locationsServise = new locationsServise();
            _rampService = new rampService();
            _productService = new productService();
            _userService = new userService();
            AddCommand = new AsyncRelayCommand(async () => await AddTasksAsync(), () => AddTaskInputFilled());
        }
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
            try
            {
                this.Products = await _productService.LoadProductsAsync();
                OnPropertChanged(nameof(Products));
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }
        private async Task LoadRampsAsync()
        {
            try
            {
                this.Ramps = await _rampService.LoadRampsAsync();
                OnPropertChanged(nameof(Ramps));
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }
        // METHODS FOR ADDING TASKS
        private async Task AddTasksAsync()
        {
            if (AddTaskAsyncBusy == true)
            {
                MessageBox.Show("You are arleady adding task.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
      
             AddTaskAsyncBusy = true;
             await VerifyUser();   //verification of user before adding task
                string TaskType;
                if (_loadBtn == true)
                {
                    TaskType = "load";
                }
                else
                {
                    TaskType = "unload";
                }

                MessageBoxResult result = MessageBox.Show(
                    $"Task:" + " Type-"+ TaskType + " Ramp-" + _selectedRamp.Name + " Product-" + _selectedProduct.Barcode + " \nAre you sure you want to add " + this._amountInput + " tasks?",
                    "Confirm Task Addition",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                // Jeśli użytkownik kliknie "No", zakończ metodę
                if (result != MessageBoxResult.Yes)
                {
                    AddTaskAsyncBusy = false;
                    return;
                }

                await CountFreeWarehousesSpaces();  //update number off free spaces in warehouse
                if (this._amountInput <= this.FreeSpacesInWarehous) //check if in warehouse is enough space
                {
                    List<int> EmptySpaces = await _locationsServise.XIdsOfEmptySpacesAsync(this._amountInput); //get list of empty spaces
                    int AmmountOfErrors = 0;
                    foreach (int space in EmptySpaces)
                    {
                        bool TaskInsertSucces = await _taskService.InsertTaskAsync(TaskType, _selectedRamp.Name, _selectedProduct.Barcode, space); //add task to DB
                        //zupdatuj zeby lokalizacej sie tez aktualizowaly
                        if (TaskInsertSucces == false)
                        {
                            AmmountOfErrors++;
                            Console.WriteLine("Error adding task to DB. Task-" + TaskType + " " + _selectedRamp.Name + " " + _selectedProduct.Barcode + " " + space);
                        }
                        if (TaskType == "Unload")
                        {
                            bool LocationUpdateSucces = await _locationsServise.FillLocationAsync(space, _selectedProduct.Barcode);
                            if (LocationUpdateSucces == false)
                            {
                                AmmountOfErrors++;
                                Console.WriteLine("Error updating to DB. Location-" + space);
                            }
                        }
                    }
                    if (AmmountOfErrors > 0)
                    {
                        MessageBox.Show("Error adding tasks or updating locatn" + AmmountOfErrors + "to DB.", "Error", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Task adding complet with succes", "Info", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("No enough space in warehouse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                RefreshDataAsync();
                AddTaskAsyncBusy = false;
            
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
        private void OnDataForAddTaskChagned()
        {
            AddCommand.RaiseCanExecuteChanged();
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
