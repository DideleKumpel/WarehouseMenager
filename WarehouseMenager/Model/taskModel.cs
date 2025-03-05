using System;

namespace WarehouseMenager.Model
{
    public class taskModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime FinishDate { get; set; }
        public  rampModel Ramp { get; set; }
        public  employeeModel Employee { get; set; }
        public locationModel Location { get; set; }
        public productModel Product { get; set; }

        public string AssingedEmployeeDisplay
        {
            get 
            {
                if (Employee.Id == -1) return "None";
                return $"{Employee.Id} - {Employee?.Name} {Employee?.Lastname}";
            }
        }
    }
}
