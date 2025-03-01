using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WarehouseMenager.Model;

namespace WarehouseMenager.Service
{
    class rampService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"].ConnectionString;
        public async Task<List<rampModel>> LoadRampsAsync()
        {
            using (var connetion = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connetion.OpenAsync();
                    string query = "SELECT * FROM ramps;";
                    using (var command = new MySqlCommand(query, connetion))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            List<rampModel> rampList = new List<rampModel> { };
                            while (await reader.ReadAsync())
                            {
                                rampModel ramp = new rampModel
                                {
                                    Name = reader.GetString(0)
                                };
                                rampList.Add(ramp);
                            }
                            return rampList;
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
