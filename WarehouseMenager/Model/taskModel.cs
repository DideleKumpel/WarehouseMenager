using System;

namespace WarehouseMenager.Model
{
    internal class taskModel
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string RampNumber { get; set; }
        public  int WorkerId { get; set; }
        public int LocationId { get; set; }
    }
}
