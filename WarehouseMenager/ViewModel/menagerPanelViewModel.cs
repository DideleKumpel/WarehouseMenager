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
        public ObservableCollection<productModel> Products;
        public List<locationModel> Locations;
        public List<rampModel> Ramps;

        private taskModel selectedTask;
        public taskModel SelectedTask
        {
            get { return selectedTask; }
            set
            {
                selectedTask = value;
                OnPropertChanged();
            }
        }
        private string Username;
        public string UsernameDisplay { get { return Username; } }

        private int FreeSpacesInWarehous;
        public int FreeSpaceInWarehouseDisplay { get { return FreeSpacesInWarehous; } }

        public ICommand LoginOutCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
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
            await LoadLocationsAsync();
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
        }
        private async Task LoadLocationsAsync()
        {
            try
            {
                this.Locations = await _locationsServise.LoadLocationsAsync();
                OnPropertChanged(nameof(Tasks));
                CountFreeWarehousesSpaces();
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }
        private async Task LoadRampsAsync()
        {
            this.Ramps = await _rampService.LoadRampsAsync();
        }
        private async Task AddTasksAsync()
        {

        }
        private async Task DeleteTaskAsync()
        {

        }
        private void LogOut()
        {

        }
        private void CountFreeWarehousesSpaces()
        {
            this.FreeSpacesInWarehous = 0;
            foreach (var space in this.Locations)
            {
                if(space.ItemBarcode == null)
                {
                    this.FreeSpacesInWarehous++;
                }
            }
            OnPropertChanged(nameof(FreeSpaceInWarehouseDisplay));
        }
    }
}
