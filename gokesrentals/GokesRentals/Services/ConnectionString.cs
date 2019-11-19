using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;

namespace GokesRentals.Services
{
    public class ConnectionString
    {
        public static SqlConnection GetConnection()
        {
            string connectionString =
                "REDACTED";
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
