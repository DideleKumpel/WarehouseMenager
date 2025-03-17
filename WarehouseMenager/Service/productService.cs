using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseMenager.Model;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace WarehouseMenager.Service
{
    internal class productService
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
                                product.NumberOfItemsInWarehouse = await CountNumberOfItemInWarehouse(product.Barcode);
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
        public async Task<bool> ProductBarcodeExistAsync(string Barcode)
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT COUNT(*) FROM products WHERE products_id = @Barcode;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        command.Parameters.AddWithValue("@Barcode", Barcode);
                        object result = await command.ExecuteScalarAsync();
                        int NumOfSpaces = Convert.ToInt32(result);
                        bool succes = true;
                        if (NumOfSpaces == 0)
                        {
                            succes = false;
                        }

                        return succes;
                    }
                }
                catch (Exception NoConnection)
                {
                    throw;
                }
            }
        }
        public async Task<bool> ProductAlreadyExistAsync(string Name, string Category, string Description, double Weight)
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT COUNT(*) FROM products WHERE productname = @Name AND weight = @Weight and category = @Category and description = @Description;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Weight", Weight);
                        command.Parameters.AddWithValue("@Category", Category);
                        command.Parameters.AddWithValue("@Description", Description);
                        object result = await command.ExecuteScalarAsync();
                        int NumOfSpaces = Convert.ToInt32(result);
                        bool succes = true;
                        if (NumOfSpaces == 0)
                        {
                            succes = false;
                        }

                        return succes;
                    }
                }
                catch (Exception NoConnection)
                {
                    throw;

                }
            }
        }
        public async Task<bool> ProductInsertAsync(string Barcode, string Name, string Category, string Description, double Weight)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO products (productname, weight, category, products_id, description) " +
                        "VALUES (@Name, @Weight, @Category, @Barcode, @Description)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Weight", Weight);
                        command.Parameters.AddWithValue("@Category", Category);
                        command.Parameters.AddWithValue("@Description", Description);
                        command.Parameters.AddWithValue("@Barcode", Barcode);

                        int rowsAffected = await command.ExecuteNonQueryAsync(); //Execute insert and return number of rows affected

                        return rowsAffected > 0; //if more then 0 rows were affected return true
                    }
                }
                catch (Exception NoConnection)
                {
                    Console.WriteLine("Error instering data do DB: " + NoConnection.Message);
                    return false;
                }
            }
        }
        public async Task<bool> DeleteProductsAsync(string Barcode)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM products WHERE products_id = @Barcode;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Barcode", Barcode);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception NoConnection)
                {
                    Console.WriteLine("Error deleting data from DB: " + NoConnection.Message);
                    return false;
                }
            }
        }
        public async Task<bool> UpdateProductAsync(string Barcode, string Name, string Category, string Description, double Weight)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "UPDATE products SET productname = @Name, weight = @Weight, category = @Category, description = @Description WHERE products_id = @Barcode;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@Weight", Weight);
                        command.Parameters.AddWithValue("@Category", Category);
                        command.Parameters.AddWithValue("@Description", Description);
                        command.Parameters.AddWithValue("@Barcode", Barcode);
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception NoConnection)
                {
                    Console.WriteLine("Error updating data in DB: " + NoConnection.Message);
                    return false;
                }
            }
        }
        public async Task<int> CountNumberOfItemInWarehouse(string Barcode)
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT COUNT(*) FROM locations WHERE products_products_id = @Barcode;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        command.Parameters.AddWithValue("@Barcode", Barcode);
                        object result = await command.ExecuteScalarAsync();
                        int NumOfFreeSpaces = Convert.ToInt32(result);
                        return NumOfFreeSpaces;
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
