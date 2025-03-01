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
        public async Task<List<locationModel>> LoadLocationsAsync()
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT * FROM locations;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        using (var reader = await command.ExecuteReaderAsync()) 
                        {
                            List<locationModel> locationList = new List<locationModel> { };
                            while (await reader.ReadAsync())
                            {
                                locationModel location = new locationModel
                                {
                                    Shelf = reader.GetString(0),
                                    Row = reader.GetString(1),
                                    Level = reader.GetString(2),
                                    MaxCapacity = reader.GetDouble(3),
                                    Id = reader.GetInt32(4),
                                    ItemBarcode = reader.IsDBNull(5) ? null : reader.GetString(5)
                                };
                                locationList.Add(location);
                            }
                            return locationList;
                        }
                    }
                }catch(Exception NoConnection)
                {
                    throw;
                }
            }
        }
        }
}
