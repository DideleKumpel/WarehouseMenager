using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using MySql.Data.MySqlClient;
using WarehouseMenager.Model;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace WarehouseMenager.Service
{
    internal class locationsServise
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"].ConnectionString;
        public async Task<int> LoadNumberEmptyLocationsAsync()
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT COUNT(*) FROM locations WHERE products_products_id IS NULL;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
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

        public async Task<int> LoadNumberOfLocationsAsync()
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT COUNT(*) FROM locations;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        object result = await command.ExecuteScalarAsync();
                        int NumOfSpaces = Convert.ToInt32(result);
                        return NumOfSpaces;
                    }
                }
                catch (Exception NoConnection)
                {
                    throw;
                }
            }
        }

        public async Task<List<int>> XIdsOfEmptySpacesAsync(int number)
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT locations_id FROM locations WHERE products_products_id IS NULL LIMIT " + number + ";";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            List<int> EmptySpacesIds = new List<int> { };
                            while (await reader.ReadAsync())
                            {
                                int Id = reader.GetInt32(0);
                                EmptySpacesIds.Add(Id);
                            }
                            return EmptySpacesIds;
                        }
                    }
                }
                catch (Exception NoConnection)
                {
                    throw;
                }
            }
        }

        public async Task<bool> FillLocationAsync(int LoactionId, string Barcode)
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "UPDATE locations SET products_products_id = @Barcode WHERE locations_id = @LocationId ;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        command.Parameters.AddWithValue("@Barcode", Barcode);
                        command.Parameters.AddWithValue("@LocationId", LoactionId);

                        int rowsAffected = await command.ExecuteNonQueryAsync(); //Execute update and return number of rows affected

                        return rowsAffected > 0; //if more then 0 rows were affected return true

                    }
                }
                catch (Exception NoConnection)
                {
                    Console.WriteLine("Error updating data do DB: " + NoConnection.Message);
                    return false;
                }

            }
        }
        //public async Task<bool> ReleaseLocation(int LocationId)
        //{
            
        //}
    }
}
