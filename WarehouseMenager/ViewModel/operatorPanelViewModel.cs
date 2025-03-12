using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WarehouseMenager.Model;
using WarehouseMenager.MVVM;
using WarehouseMenager.Service;

namespace WarehouseMenager.ViewModel
{
    public class operatorPanelViewModel: ViewModelBase
    {
        public userModel User;

        //Services
        private userService _userService;
        private taskService _taskService;

        //DISPLAY AND DB DATA VARIABULES
        public ObservableCollection<taskModel> DisplayTasks { get; set; }

        private ObservableCollection<taskModel> _selectedTasks = new ObservableCollection<taskModel>();
        public ObservableCollection<taskModel> SelectedTasks
        {
            get
            {
                return _selectedTasks;
            }
            set
            {
                _selectedTasks = value;
            }
        }

        private ObservableCollection<taskModel> assignedTasks;
        private ObservableCollection<taskModel> freeToTakeTasks;
        private string Username;
        public string UsernameDisplay
        {
            get { return Username; }
            set { }
        }

        public ObservableCollection<string> ComboBoxDataGridMode { get; set; } = new ObservableCollection<string> { "Free to take", "Assigned" };
        private string _selectedMode;
        public string SelectedMode
        {
            get 
            {
                OnTaskDisplayModeChagned();
                return _selectedMode; 
            }
            set
            {
                _selectedMode = value;
                if( value == "Free to take")
                {
                    FreeToTakeButtonsRender = "Visible";
                    AssignedButtonsRender = "Collapsed";
                }
                else if( value == "Assigned")
                {
                    FreeToTakeButtonsRender = "Collapsed";
                    AssignedButtonsRender = "Visible";
                }
                OnPropertChanged(nameof(SelectedMode));
                OnPropertChanged(nameof(FreeToTakeButtonsRender));
                OnPropertChanged(nameof(AssignedButtonsRender));
                OnTaskDisplayModeChagned();
                OnDataForAddTaskChagned();
                
                if (_selectedMode != null)
                {
                    Console.WriteLine("Mode seleted " + _selectedMode);
                }
            }
        }
        public string FreeToTakeButtonsRender { get; set; } //Biding for buttons visiblity that depends of selected datagrid mode in combobox
        public string AssignedButtonsRender { get; set; }

        //BUTTONS
        public AsyncRelayCommand FinishTaskCommand { get; }
        public AsyncRelayCommand AssigneTaskCommand { get; }
        public AsyncRelayCommand AbandonTaskCommand { get; }
        public ICommand SwitchToTaskManagerCommand;
        public ICommand SwitchToProductManagerCommand;
        public ICommand LogOutCommand => new RelayCommand(execute => LogOut());
        public ICommand RefreshCommand => new RelayCommand (execute => RefreshDataAsync());

        //Flags
        private bool FinishTaskIsBusy = false;
        private bool AssigneTaskIsBusy = false;
        private bool AbanTaskIsBusy = false;

        //CONSTRUCTOR
        public operatorPanelViewModel() {
            Mediator.UserDataPass += UserDataTransfer;

            _userService = new userService();
            _taskService = new taskService();

            //selecting start dataGrid mode and visiblity on buttons
            _selectedMode = ComboBoxDataGridMode[0];
            FreeToTakeButtonsRender = "Visible";
            AssignedButtonsRender = "Collapsed";

            FinishTaskCommand = new AsyncRelayCommand ( async () => await FinishTaskAsync(), () => CanExecuteFinishTaskCommand());
            AssigneTaskCommand = new AsyncRelayCommand(async () => await AssingedToTaskAsync(), () => CanExecuteAssingedToTaskCommand());
            AbandonTaskCommand = new AsyncRelayCommand(async () => await AbandonTaskAsync(), () => CanExecuteAbandonTaskCommand());
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
            await LoadAssignedTaskAsync();
            await LoadFreeToTakeTaskAsync();
            OnTaskDisplayModeChagned();
        }

        private async Task LoadAssignedTaskAsync()
        {
            await VerifyUser();
            try
            {
                assignedTasks = await _taskService.LoadTaskByAssignedEmployeeId(User.Id);
                OnPropertChanged(nameof(DisplayTasks));
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }

        private async Task LoadFreeToTakeTaskAsync()
        {
            await VerifyUser();
            try
            {
                freeToTakeTasks = await _taskService.LoadTaskFreeToTakeAsync();
                OnPropertChanged(nameof(DisplayTasks));
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
        }

        //METHODS FOR FINISH TASKS

        private async Task FinishTaskAsync()
        {
            //todo
        }

        private bool CanExecuteFinishTaskCommand()
        {
            bool canExecute = true;
            if(_selectedMode != "Assigned")
            {
                canExecute = false;
            }
            if (this._selectedTasks.Count <= 0)
            {
                canExecute = false;
            }
            return canExecute;
        }

        private void OnDataForAddTaskChagned()
        {
            FinishTaskCommand.RaiseCanExecuteChanged();
        }

        
        //METHODS FOR ABANDON THE TASK

        private async Task AbandonTaskAsync()
        {
            if (AbanTaskIsBusy == true)
            {
                MessageBox.Show("You are arleady abanding the tasks.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await VerifyUser();
            AbanTaskIsBusy = true;
            ObservableCollection<taskModel> taskToAbandon = new ObservableCollection<taskModel>(_selectedTasks);
            try
            {
                int AmmountsofErrors = 0;
                foreach (taskModel task in taskToAbandon)
                {
                    bool succes = await _taskService.UnassingEmployeeFormTaskAsync(task.Id);
                    if (succes == false)
                    {
                        AmmountsofErrors++;
                    }
                }
                if (AmmountsofErrors > 0)
                {
                    MessageBox.Show(AmmountsofErrors + " tasks were not Abandoned. Check your internet connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Abandoned to task with succes.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
            RefreshDataAsync();
            AbanTaskIsBusy = false;
        }

        private bool CanExecuteAbandonTaskCommand()
        {
            bool canExecute = true;
            if (_selectedMode != "Assigned")
            {
                canExecute = false;
            }
            if (this._selectedTasks.Count <= 0)
            {
                canExecute = false;
            }
            return canExecute;
        }

        private void OnDataForAbandonTaskChagned()
        {
            AbandonTaskCommand.RaiseCanExecuteChanged();
        }

        //METHODS FOR ASSINGED TO TASKS
        private async Task AssingedToTaskAsync()
        {
            if (AssigneTaskIsBusy == true)
            {
                MessageBox.Show("You are arleady adding task.", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await VerifyUser();
            AssigneTaskIsBusy = true;
            ObservableCollection<taskModel> taskToAssinged = new ObservableCollection<taskModel>(_selectedTasks);
            try
            {
                int AmmountsofErrors = 0;
                foreach (taskModel task in taskToAssinged)
                {
                    bool succes = await _taskService.AssingEmployeeToTaskAsync(User.Id, task.Id);
                    if (succes == false)
                    {
                        AmmountsofErrors++;
                    }
                }
                if (AmmountsofErrors > 0)
                {
                    MessageBox.Show(AmmountsofErrors + " tasks were not get assigned. Check your internet connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Assigned to task with succes.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception NoConnect)
            {
                MessageBox.Show("No connetion. Chech your internet and log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogOut();
            }
            RefreshDataAsync();
            AssigneTaskIsBusy = false;
        }
        private bool CanExecuteAssingedToTaskCommand()
        {
            bool canExecute = true;
            if (_selectedMode != "Free to take")
            {
                canExecute = false;
            }
            if (this._selectedTasks.Count <= 0)
            {
                canExecute = false;
            }
            return canExecute;
        }

        private void OnDataForAssingedToTaskChagned()
        {
            AssigneTaskCommand.RaiseCanExecuteChanged();
        }

        //OTHER
        private void OnTaskDisplayModeChagned()
        {
            _selectedTasks.Clear(); //clear selected task list
            Console.WriteLine("Selected task list cleared");
            if(_selectedMode == "Free to take")
            {
                DisplayTasks = freeToTakeTasks;
            }else if(_selectedMode == "Assigned")
            {
                DisplayTasks = assignedTasks;
            }
            OnDataForAddTaskChagned();
            OnPropertChanged(nameof(DisplayTasks));
        }

        private void LogOut()
        {
            Application.Current.MainWindow.DataContext = new loginPanelViewModel();
        }
    }
}
