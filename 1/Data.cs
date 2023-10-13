using System;
using System.Data.SqlClient;

namespace _1
{
    class Data
    {
        SqlConnection SqlConnection = new SqlConnection(@"Data Source=DESKTOP-PB4FEQ9;Initial Catalog=base;Integrated Security=True");

        public void openConnection()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }

        public void closeConnection()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Open)
            {
                SqlConnection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return SqlConnection;
        }
    }
}
