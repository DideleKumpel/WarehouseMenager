
using Mysqlx.Crud;
using System;

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
        public int NumberOfItemsOnShelf { get; set; }
        public string NumberOfItemsOnShelfDisplay
        {
            get
            {
                if (NumberOfItemsOnShelf < 0)
                {
                    return "Error";
                }
                else
                {
                    return NumberOfItemsOnShelf.ToString();
                }
            }
        }

        public productModel(){}
        public productModel(productModel other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Name = other.Name;
            Weight = other.Weight;
            Category = other.Category;
            Barcode = other.Barcode;
            Description = other.Description;
            NumberOfItemsInWarehouse = other.NumberOfItemsInWarehouse;
        }
    }
}
