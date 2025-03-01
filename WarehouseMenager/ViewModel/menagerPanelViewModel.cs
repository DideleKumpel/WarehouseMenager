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

        public string Username { get; set; }
        public ObservableCollection<taskModel> Tasks { get; set; }
        public List<productModel> Products;
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

        public ICommand LoginOutCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public menagerPanelViewModel() {
            Mediator.UserDataPass += UserDataTransfer; //add fuction to event 
            _taskService = new taskService();
        }

        public void UserDataTransfer(userModel userData) //function to save user data from loginPanel
        {
            this.User = userData;
            Username = User.Name + " " + User.Lastname;
            OnPropertChanged(nameof(Username));
        }

        public async void RefreshData() //refresh and download new data
        {
            await LoadTasks();
            await LoadProducts();
            await LoadLocations();
            await LoadRamps();
        }
        private async Task LoadTasks()
        {
            this.Tasks = await _taskService.LoadTaskAsync();
            OnPropertChanged(nameof(Tasks));
        }
        private async Task LoadProducts()
        {

        }
        private async Task LoadLocations()
        {

        }
        public async Task LoadRamps()
        {

        }
        private async Task AddTasks()
        {

        }
        private async Task DeleteTask()
        {

        }
        private void LogOut()
        {

        }



    }
}
