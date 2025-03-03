using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                }catch(Exception NoConnection)
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

        }
}
