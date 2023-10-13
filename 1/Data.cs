using System;
using System.Data.SqlClient;

namespace _1
{
    class Data
    {
        SqlConnection SqlConnection = new SqlConnection(@"Data Source=DESKTOP-PB4FEQ9;Initial Catalog=base;Integrated Security=True");

        // Открывает соединение с базой данных, если оно закрыто.
        public void openConnection()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }

        // Закрывает соединение с базой данных, если оно открыто.
        public void closeConnection()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Open)
            {
                SqlConnection.Close();
            }
        }

        // Возвращает объект SqlConnection, представляющий соединение с базой данных.
        public SqlConnection GetConnection()
        {
            return SqlConnection;
        }
    }
}
