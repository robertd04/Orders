using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zamowienia
{
    public class Szyfrowanie
    {
        public static string Szyfrowanie1(string wejscie)
        {
            int dlugosc = wejscie.Length;
            char[] tablicaLiter;
            int[] tablicaLiterek;
            char literka2;
            string wyjscie = "";

            tablicaLiter = new char[dlugosc];
            tablicaLiterek = new int[dlugosc];

            for (int j = 0; j < dlugosc; j++)
            {
                literka2 = char.Parse(wejscie.Substring(j, 1));
                tablicaLiter[j] = literka2;
            }
            // Szyfrowanie
            for (int j = 0; j < dlugosc; j++)
            {
                if (j % 2 == 0) tablicaLiterek[j] = (int)tablicaLiter[j] - 7; else tablicaLiterek[j] = (int)tablicaLiter[j] - 9;
            }
            for (int z = 0; z < dlugosc; z++)
            {
                tablicaLiter[z] = (char)tablicaLiterek[z];
                wyjscie += tablicaLiter[z];
            }
            return wyjscie;

        }

        public static string DeSzyfrowanie(string wejscie)
        {
            int dlugosc = wejscie.Length;
            char[] tablicaLiter;
            int[] tablicaLiterek;
            char literka2;
            string wyjscie = "";

            tablicaLiter = new char[dlugosc];
            tablicaLiterek = new int[dlugosc];

            for (int j = 0; j < dlugosc; j++)
            {
                literka2 = char.Parse(wejscie.Substring(j, 1));
                tablicaLiter[j] = literka2;
            }
            // Szyfrowanie
            for (int j = 0; j < dlugosc; j++)
            {
                if (j % 2 == 0) tablicaLiterek[j] = (int)tablicaLiter[j] + 7; else tablicaLiterek[j] = (int)tablicaLiter[j] + 9;
            }
            for (int z = 0; z < dlugosc; z++)
            {
                tablicaLiter[z] = (char)tablicaLiterek[z];
                wyjscie += tablicaLiter[z];
            }
            return wyjscie;
        }

        public static string kodujMD5(string password)
        {
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] passMD5 = md5.ComputeHash(passByte);
            string passString = BitConverter.ToString(passMD5).Replace("-", "").ToLower();

            return passString;
        }


    }
}
