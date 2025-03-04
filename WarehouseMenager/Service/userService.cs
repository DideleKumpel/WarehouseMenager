using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using WarehouseMenager.Model;


namespace WarehouseMenager.Service
{
    internal class userService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"].ConnectionString;

        public async Task<userModel> LoginAsync(string username, string password)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "SELECT worker_id, name, lastname, login, password, role FROM worker WHERE login = @username AND password = @password";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Zabezpieczenie przed SQL Injection
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new userModel
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Lastname = reader.GetString(2),
                                    Username = reader.GetString(3),
                                    Password = reader.GetString(4),
                                    Role = reader.GetString(5)
                                };
                            }
                        }
                    }
                }
                catch (Exception NoConnetion) {
                    MessageBox.Show("No connetion", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw ; 
                }
            }
            return null;
        }
    }
}
