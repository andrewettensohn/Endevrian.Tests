using Endevrian.Controllers;
using Endevrian.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Endevrian.Data
{
    public class MockQueryHelper
    {
        private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=aspnet-Endevrian-5E06C235-D29D-4E9D-8A06-5EE32B599278;Trusted_Connection=True;MultipleActiveResultSets=true";
        //private readonly SystemLogController _logger;

        public MockQueryHelper()
        {
            //_connectionString = config.GetConnectionString("DefaultConnection");
            //_logger = systemLogController;
        }

        public string SelectQuery(string query, string field)
        {

            string result = "No Results";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        result = (reader[field].ToString());
                    }
                }
                catch(Exception exc)
                {
                    //_logger.AddSystemLog($"Failed to read query results: {exc}");
                }
                finally
                {

                    reader.Close();
                    connection.Dispose();

                }

            }

            return result;
        }

        public AdventureLog SelectAdventureLogQuery(int id)
        {

            string query = $"SELECT * FROM AdventureLogs WHERE AdventureLogID = {id}";
            AdventureLog adventureLog = new AdventureLog();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            adventureLog = new AdventureLog()
                            {
                                AdventureLogID = id,
                                UserId = reader["UserId"].ToString(),
                                LogTitle = reader["LogTitle"].ToString(),
                                LogBody = reader["LogBody"].ToString(),
                                LogDate = DateTime.Parse(reader["LogDate"].ToString())
                            };
                        }                        
                    }
                }
                catch (Exception exc)
                {
                    //_logger.AddSystemLog($"Failed to read query results: {exc}");
                }

                return adventureLog;
            }
        }

        public void UpdateQuery(string query)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }

            return;
        }
    }
}
