using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseMenager.Model;

namespace WarehouseMenager.Service
{
    class productService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"].ConnectionString;
        public async Task<ObservableCollection<productModel>> LoadProductsAsync()
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT * FROM products;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            ObservableCollection<productModel> productList = new ObservableCollection<productModel> { };
                            while (await reader.ReadAsync())
                            {
                                productModel product = new productModel
                                {
                                    Name = reader.GetString(0),
                                    Weight = reader.GetDouble(1),
                                    Category = reader.GetString(2),
                                    Description = reader.GetString(3),
                                    Barcode = reader.GetString(4)
                                };
                                productList.Add(product);
                            }
                            return productList;
                        }
                    }
                }
                catch (Exception NoConnection)
                {
                    throw;
                }
            }
        }
    }
}
