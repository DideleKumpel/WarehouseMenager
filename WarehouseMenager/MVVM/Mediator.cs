using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseMenager.Model;


namespace WarehouseMenager.MVVM
{
    //Class to communicate between ViewModels
    class Mediator
    {
        public delegate void UserDataPasserEventHandler(userModel userData);
        public static event UserDataPasserEventHandler UserDataPass;

        public static void NotifyViewModel1FullNameChanged(userModel userData)
        {
            UserDataPass?.Invoke(userData);
        }
    }
}
