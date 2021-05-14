using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Digger.Server.Tests.Helpers
{
    public static class BddDiStock
    {
        static string _connectionString = @"Server=.\SQLEXPRESS;Database=Digger.DiStock;Trusted_Connection=True;";

        public static void CreateUser(string pseudo, string role)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into tUser values ('" + pseudo + "', 0x01000000010000271000000010B737B48AB8A334F949F7AAA04C872ACC0EE269E0BFCF8D0CB6BDF39561B042456780F504686569827F5FC62E336AC5B1, '" + role + "');", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public static void DeleteUser(string pseudo)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("delete tUser where Pseudo = '" + pseudo + "'", con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
