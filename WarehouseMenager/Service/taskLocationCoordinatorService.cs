using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseMenager.Model;

namespace WarehouseMenager.Service
{
    internal class taskLocationCoordinatorService
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WarehouseDb"].ConnectionString;
        public async Task<bool> AddUnloadTaskAsync(string Type, string Ramp, string Product, int LocationID)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var insertCommand = new MySqlCommand(@"INSERT INTO tasks (type, status, upload_dateTime, finish_dateTime, ramp_name, worker_worker_id, locations_locations_id, products_products_id)
                                    VALUES (@Type, 'toDo', NOW(), NULL, @Ramp, NULL, @Location, @Product);", connection, transaction);

                            insertCommand.Parameters.AddWithValue("@Type", Type);
                            insertCommand.Parameters.AddWithValue("@Ramp", Ramp);
                            insertCommand.Parameters.AddWithValue("@Location", LocationID);
                            insertCommand.Parameters.AddWithValue("@Product", Product);

                            insertCommand.ExecuteNonQuery();

                            var updateCommand = new MySqlCommand(@"UPDATE locations SET products_products_id = @Barcode WHERE locations_id = @LocationId;", connection, transaction);

                            updateCommand.Parameters.AddWithValue("@Barcode", Product);
                            updateCommand.Parameters.AddWithValue("@LocationId", LocationID);

                            updateCommand.ExecuteNonQuery();

                            transaction.Commit();   //commit changes to DB

                            Console.WriteLine("Transaction completed successfully!");
                            return true;
                        }
                        catch (Exception ex)   //if transaction failed 
                        {
                            transaction.Rollback();   //rollback changes
                            Console.WriteLine($"Transaction failed: {ex.Message}");
                            return false;
                        }
                    }
                }
                catch (Exception NoConnection)
                {
                    Console.WriteLine("Error instering data do DB: " + NoConnection.Message);
                    return false;
                }
            }
        }
        public async Task<bool> DeleteUnloadTaskAsync(taskModel task)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var deleteCommand = new MySqlCommand(@"DELETE FROM tasks WHERE tasks_id = @TaskID;", connection, transaction);
                            deleteCommand.Parameters.AddWithValue("@TaskID", task.Id);
                            deleteCommand.ExecuteNonQuery();
                            var updateCommand = new MySqlCommand(@"UPDATE locations SET products_products_id = NULL WHERE locations_id = @LocationId;", connection, transaction);
                            updateCommand.Parameters.AddWithValue("@LocationId", task.Location.Id);
                            updateCommand.ExecuteNonQuery();
                            transaction.Commit();   //commit changes to DB
                            Console.WriteLine("Transaction completed successfully!");
                            return true;
                        }
                        catch (Exception ex)   //if transaction failed 
                        {
                            transaction.Rollback();   //rollback changes
                            Console.WriteLine($"Transaction failed: {ex.Message}");
                            return false;
                        }
                    }
                }
                catch (Exception NoConnection)
                {
                    Console.WriteLine("Error instering data do DB: " + NoConnection.Message);
                    return false;
                }
            }
        }
    }
}
