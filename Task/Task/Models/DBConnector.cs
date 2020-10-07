using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;

namespace Task.Models
{
    class DBConnector
    {
        static String connectionString;

        public static void SetConnectionString(String dataSource = null, String initialCatalog = null, String userId = null, String password = null) {
            connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = (dataSource!=null)?dataSource:ConfigurationManager.AppSettings.Get("data_source"),
                InitialCatalog = (initialCatalog != null) ? initialCatalog : ConfigurationManager.AppSettings.Get("initial_catalog"),
                UserID = (userId != null) ? userId : ConfigurationManager.AppSettings.Get("user_id"),
                Password = (password != null) ? password : ConfigurationManager.AppSettings.Get("password")
            }.ConnectionString;
             
        }

        public static DataTable ExecuteQuery(String queryString) {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlCommand cmd = new SqlCommand(queryString, connection);
                DataTable dataTable = new DataTable();
                try
                {
                    connection.Open();

                    using (var da = new SqlDataAdapter(queryString, connectionString))
                    {
                        da.Fill(dataTable);
                    }

                    return dataTable;
                }
                catch (Exception ex) {
                    throw ex;
                }
            }
        }
    }
}
