
namespace WarehouseMenager.Model
{
    public class productModel
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public string Category { get; set; }
        public string Barcode { get; set;}
        public string Description { get; set; }
        public int NumberOfItemsInWarehouse { get; set; }
        public string NumberOfITemInWarehouseDisplay
        {
            get
            {
                if(NumberOfItemsInWarehouse < 0)
                {
                    return "Error";
                }
                else{
                    return NumberOfItemsInWarehouse.ToString();
                }
            }
        }
    }
}
