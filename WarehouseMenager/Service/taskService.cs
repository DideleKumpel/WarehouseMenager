using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using WarehouseMenager.Model;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace WarehouseMenager.Service
{
    internal class taskService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"].ConnectionString;
        public async Task<ObservableCollection<taskModel>> LoadTaskAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                        //query retrieves all tasks along with their associated location and product details.
                        //It includes tasks that are either not yet completed or were completed within the last 7 days.
                    string query = "SELECT t.tasks_id, t.type, t.status, t.upload_dateTime, t.finish_dateTime, t.ramp_name, t.worker_worker_id, " +
                        "w.name, w.lastname, w.role, " +
                        "l.locations_id, l.shelf, l.`Row`, l.`Level`, l.maxcapacity, " +
                        "p.productname, p.weight, p.category, p.products_id, p.description " +
                        "FROM tasks t " +
                        "JOIN products p ON t.products_products_id = p.products_id " +
                        "LEFT JOIN locations l ON t.locations_locations_id = l.locations_id " +
                        "LEFT JOIN worker w ON t.worker_worker_id = w.worker_id " +
                        "WHERE t.status IN ('toDo', 'taken') OR (t.status = 'done' AND t.finish_dateTime >= NOW() - INTERVAL 7 DAY);";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            ObservableCollection<taskModel> TaskList = new ObservableCollection<taskModel> { };
                            while (await reader.ReadAsync())
                            {
                                taskModel task = new taskModel
                                {
                                    Id = reader.GetInt32(0),
                                    Type = reader.GetString(1),
                                    Status = reader.GetString(2),
                                    UploadDate = reader.GetDateTime(3),
                                    FinishDate = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4), //DB column can have null DataTime cant be null so FinishDate is DateTime.MinValue (1/1/0001 12:00:00 AM)
                                    Ramp = new rampModel { Name = reader.GetString(5) },
                                    Employee = new employeeModel
                                    {
                                        Id = reader.IsDBNull(6) ? -1 : reader.GetInt32(6), //DB column can have null int cant be null so WorekrID is -1
                                        Name = reader.IsDBNull(7) ? null : reader.GetString(7),
                                        Lastname = reader.IsDBNull(8) ? null : reader.GetString(8),
                                        Role = reader.IsDBNull(9) ? null : reader.GetString(9)
                                    },
                                    Location = new locationModel
                                    {
                                        Id = reader.GetInt32(10),
                                        Shelf = reader.GetString(11),
                                        Row = reader.GetString(12),
                                        Level = reader.GetString(13),
                                        MaxCapacity = reader.GetDouble(14),
                                        ItemBarcode = reader.IsDBNull(18) ? null : reader.GetString(18) //DB column can have null value then ItemBarcode is null
                                    },
                                    Product = new productModel
                                    {
                                        Name = reader.GetString(15),
                                        Weight = reader.GetDouble(16),
                                        Category = reader.GetString(17),
                                        Barcode = reader.GetString(18),
                                        Description = reader.GetString(19)
                                    }
                                };
                                TaskList.Add(task);
                            }
                            return TaskList;
                        }
                    }
                }
                catch (Exception NoConnetion)
                {
                    throw;
                }
            }
        }
        public async Task<bool> InsertTaskAsync(string Type, string Ramp, string Product, int LocationID)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "INSERT INTO tasks(type, status, upload_dateTime, finish_dateTime, ramp_name, worker_worker_id, locations_locations_id, products_products_id) " +
                        "VALUES(@Type, 'toDo', NOW(), NULL, @Ramp, NULL, @Location, @Product);";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Type", Type);
                        command.Parameters.AddWithValue("@Ramp", Ramp);
                        command.Parameters.AddWithValue("@Product", Product);
                        command.Parameters.AddWithValue("@Location", LocationID);

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
        public async Task<bool> DeleteTaskByIdAsync(int TaskId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM tasks WHERE tasks_id = @Id;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", TaskId);
                        
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }catch(Exception NoConnection)
                {
                    Console.WriteLine("Error deleting data from DB: " + NoConnection.Message);
                    return false;
                }
            }
        }
        public async Task<ObservableCollection<taskModel>> LoadTaskByAssignedEmployeeId(int EmployeeID)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    //query retrieves all tasks along with their associated location and product details.
                    //It includes tasks that are either not yet completed or were completed within the last 7 days.
                    string query = "SELECT t.tasks_id, t.type, t.status, t.upload_dateTime, t.finish_dateTime, t.ramp_name, t.worker_worker_id, " +
                        "w.name, w.lastname, w.role, " +
                        "l.locations_id, l.shelf, l.`Row`, l.`Level`, l.maxcapacity, " +
                        "p.productname, p.weight, p.category, p.products_id, p.description " +
                        "FROM tasks t " +
                        "JOIN products p ON t.products_products_id = p.products_id " +
                        "LEFT JOIN locations l ON t.locations_locations_id = l.locations_id " +
                        "LEFT JOIN worker w ON t.worker_worker_id = w.worker_id " +
                        "WHERE t.worker_worker_id = @WorkerId and t.status != 'done' ;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@WorkerId", EmployeeID);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            ObservableCollection<taskModel> TaskList = new ObservableCollection<taskModel> { };
                            while (await reader.ReadAsync())
                            {
                                taskModel task = new taskModel
                                {
                                    Id = reader.GetInt32(0),
                                    Type = reader.GetString(1),
                                    Status = reader.GetString(2),
                                    UploadDate = reader.GetDateTime(3),
                                    FinishDate = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4), //DB column can have null DataTime cant be null so FinishDate is DateTime.MinValue (1/1/0001 12:00:00 AM)
                                    Ramp = new rampModel { Name = reader.GetString(5) },
                                    Employee = new employeeModel
                                    {
                                        Id = reader.IsDBNull(6) ? -1 : reader.GetInt32(6), //DB column can have null int cant be null so WorekrID is -1
                                        Name = reader.IsDBNull(7) ? null : reader.GetString(7),
                                        Lastname = reader.IsDBNull(8) ? null : reader.GetString(8),
                                        Role = reader.IsDBNull(9) ? null : reader.GetString(9)
                                    },
                                    Location = new locationModel
                                    {
                                        Id = reader.GetInt32(10),
                                        Shelf = reader.GetString(11),
                                        Row = reader.GetString(12),
                                        Level = reader.GetString(13),
                                        MaxCapacity = reader.GetDouble(14),
                                        ItemBarcode = reader.IsDBNull(18) ? null : reader.GetString(18) //DB column can have null value then ItemBarcode is null
                                    },
                                    Product = new productModel
                                    {
                                        Name = reader.GetString(15),
                                        Weight = reader.GetDouble(16),
                                        Category = reader.GetString(17),
                                        Barcode = reader.GetString(18),
                                        Description = reader.GetString(19)
                                    }
                                };
                                TaskList.Add(task);
                            }
                            return TaskList;
                        }
                    }
                }
                catch (Exception NoConnetion)
                {
                    throw;
                }
            }
        }

        public async Task<ObservableCollection<taskModel>> LoadTaskFreeToTakeAsync()
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    //query retrieves all tasks along with their associated location and product details.
                    //It includes tasks that are either not yet completed or were completed within the last 7 days.
                    string query = "SELECT t.tasks_id, t.type, t.status, t.upload_dateTime, t.finish_dateTime, t.ramp_name, t.worker_worker_id, " +
                        "w.name, w.lastname, w.role, " +
                        "l.locations_id, l.shelf, l.`Row`, l.`Level`, l.maxcapacity, " +
                        "p.productname, p.weight, p.category, p.products_id, p.description " +
                        "FROM tasks t " +
                        "JOIN products p ON t.products_products_id = p.products_id " +
                        "LEFT JOIN locations l ON t.locations_locations_id = l.locations_id " +
                        "LEFT JOIN worker w ON t.worker_worker_id = w.worker_id " +
                        "WHERE t.worker_worker_id IS NULL ;";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            ObservableCollection<taskModel> TaskList = new ObservableCollection<taskModel> { };
                            while (await reader.ReadAsync())
                            {
                                taskModel task = new taskModel
                                {
                                    Id = reader.GetInt32(0),
                                    Type = reader.GetString(1),
                                    Status = reader.GetString(2),
                                    UploadDate = reader.GetDateTime(3),
                                    FinishDate = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4), //DB column can have null DataTime cant be null so FinishDate is DateTime.MinValue (1/1/0001 12:00:00 AM)
                                    Ramp = new rampModel { Name = reader.GetString(5) },
                                    Employee = new employeeModel
                                    {
                                        Id = reader.IsDBNull(6) ? -1 : reader.GetInt32(6), //DB column can have null int cant be null so WorekrID is -1
                                        Name = reader.IsDBNull(7) ? null : reader.GetString(7),
                                        Lastname = reader.IsDBNull(8) ? null : reader.GetString(8),
                                        Role = reader.IsDBNull(9) ? null : reader.GetString(9)
                                    },
                                    Location = new locationModel
                                    {
                                        Id = reader.GetInt32(10),
                                        Shelf = reader.GetString(11),
                                        Row = reader.GetString(12),
                                        Level = reader.GetString(13),
                                        MaxCapacity = reader.GetDouble(14),
                                        ItemBarcode = reader.IsDBNull(18) ? null : reader.GetString(18) //DB column can have null value then ItemBarcode is null
                                    },
                                    Product = new productModel
                                    {
                                        Name = reader.GetString(15),
                                        Weight = reader.GetDouble(16),
                                        Category = reader.GetString(17),
                                        Barcode = reader.GetString(18),
                                        Description = reader.GetString(19)
                                    }
                                };
                                TaskList.Add(task);
                            }
                            return TaskList;
                        }
                    }
                }
                catch (Exception NoConnetion)
                {
                    throw;
                }
            }
        }
    }
}
