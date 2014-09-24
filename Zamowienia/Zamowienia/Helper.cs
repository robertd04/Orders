using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Zamowienia
{
    public class Helper
    {
        private static string connectionString = string.Empty;

        public static void SetConnect()
        {//pobranie danych z pliku konfiguracyjnego txt
            string adresServ = "", haslo = "";
            var file = System.IO.File.OpenText("set.txt");
            adresServ = file.ReadLine();
            haslo = file.ReadLine();
            //connectionString = @"Server=" + adresServ + ";Database=Produkcja_1; User ID=sa;Password=" + haslo + ";";
            connectionString = @"Server=" + Szyfrowanie.DeSzyfrowanie(adresServ) + ";Database=Produkcja_1; User ID=sa;Password=" + Szyfrowanie.DeSzyfrowanie(haslo) + ";";
            file.Close();
        }

        public static SqlConnection GetDBConnection()
        {
            if(string.IsNullOrEmpty(connectionString))
                SetConnect();
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                if (conn == null)
                {
                    throw new ArgumentNullException();
                }
                conn.Open();
            }
            catch (Exception ex)
            {
                //     Error<Helper>.RegError(ex);
            }
            return conn;
        }



    }
}
