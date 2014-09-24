using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zamowienia
{
    public class Metody
    {
        private static string connectionString = @"Server=.\;Database=PRODUKCJA_1;User ID=sa;Password=haslo;";

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

        public static pracownik Logowanie(string haslo)
        {//LOGOWANIE
            SetConnect();
            pracownik pracownik1 = new pracownik(-1, "-1", -1, "-1");
            string hasloMD5 = Szyfrowanie.kodujMD5(haslo);
            string zapytanie = "SELECT ID_PRACOWNIK, PRACOWNIK, ADMIN, HASLO, ZMIANA FROM Table_pracownik WHERE HASLO = '" + hasloMD5 + "' ";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr;
                dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        pracownik1 = new pracownik(int.Parse(dr["ID_PRACOWNIK"].ToString()), dr["PRACOWNIK"].ToString(), int.Parse(dr["ADMIN"].ToString()), dr["ZMIANA"].ToString());
                    }
                    return pracownik1;
                }
                else
                {
                    return new pracownik(-1, "-1", -1, "-1");//gdy logowanie zakonczone niepowodzeniem
                }
            }

        }
        public static void OdczytajDoCombobox(ComboBox cmbx1, string nazwaKolumny, string nazwaTabeli)
        {
            cmbx1.Items.Clear();
            string zapytanie = "SELECT DISTINCT " + nazwaKolumny + " FROM " + nazwaTabeli;
            SqlConnection conDatabase = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
            conDatabase.Open();
            SqlDataReader dr = myCommand.ExecuteReader();
            while (dr.Read()) { cmbx1.Items.Add(dr[nazwaKolumny].ToString().Trim()); }
            conDatabase.Close();

        }
        public static bool SprLiczby(string n)//spr poprawnosci liczby
        {
            Regex r = new Regex("[^0-9]");
            if (r.IsMatch(n))
            {
                return false;
            }
            return true;
        }
        public static DataTable GetZlecenia(bool zatwierdzone, bool wykonane)
        {
            string zapytanie = string.Empty;
            zapytanie = "SELECT dbo.Table_zlecenia_prod.id_zlecenia, dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_zlecenia_prod.data, dbo.Table_katalog.KATALOG, dbo.Table_rozmiar.ROZMIAR, dbo.Table_przeznaczenie.przeznaczenie, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_w_worku, dbo.Table_zlecenia_prod.ilosc_wykonana, dbo.Table_zlecenia_prod.zatwierdzone, dbo.Table_zlecenia_prod.wykonane, dbo.Table_zlecenia_prod.ean13_zlecenia, dbo.Table_zlecenia_prod.uwagi_zlecenia FROM dbo.Table_zlecenia_prod LEFT OUTER JOIN dbo.Table_przeznaczenie ON dbo.Table_zlecenia_prod.id_przeznaczenie = dbo.Table_przeznaczenie.id_przeznaczenie LEFT OUTER JOIN dbo.Table_katalog ON dbo.Table_zlecenia_prod.id_katalog = dbo.Table_katalog.ID_KATALOG LEFT OUTER JOIN dbo.Table_rozmiar ON dbo.Table_zlecenia_prod.id_rozmiar = dbo.Table_rozmiar.ID_ROZMIAR WHERE (dbo.Table_zlecenia_prod.zatwierdzone = '" + zatwierdzone + "') AND (dbo.Table_zlecenia_prod.wykonane = '" + wykonane + "') ORDER BY dbo.Table_zlecenia_prod.id_zlecenia DESC;";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            conDatabase.Open();
            SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conDatabase.Close();
            return dt;
        }
        public static DataTable GetZleceniaPrzeznaczenieID(bool zatwierdzone, bool wykonane)
        {
            string zapytanie = string.Empty;
            zapytanie = "SELECT dbo.Table_zlecenia_prod.id_zlecenia, dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_zlecenia_prod.data, dbo.Table_katalog.KATALOG, dbo.Table_rozmiar.ROZMIAR, dbo.Table_przeznaczenie.id_przeznaczenie, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_w_worku, dbo.Table_zlecenia_prod.ilosc_wykonana, dbo.Table_zlecenia_prod.zatwierdzone, dbo.Table_zlecenia_prod.wykonane, dbo.Table_zlecenia_prod.ean13_zlecenia, dbo.Table_zlecenia_prod.uwagi_zlecenia FROM dbo.Table_zlecenia_prod LEFT OUTER JOIN dbo.Table_przeznaczenie ON dbo.Table_zlecenia_prod.id_przeznaczenie = dbo.Table_przeznaczenie.id_przeznaczenie LEFT OUTER JOIN dbo.Table_katalog ON dbo.Table_zlecenia_prod.id_katalog = dbo.Table_katalog.ID_KATALOG LEFT OUTER JOIN dbo.Table_rozmiar ON dbo.Table_zlecenia_prod.id_rozmiar = dbo.Table_rozmiar.ID_ROZMIAR WHERE (dbo.Table_zlecenia_prod.zatwierdzone = '" + zatwierdzone + "') AND (dbo.Table_zlecenia_prod.wykonane = '" + wykonane + "') ORDER BY dbo.Table_zlecenia_prod.id_zlecenia DESC;";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            conDatabase.Open();
            SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conDatabase.Close();
            return dt;
        }
        public static DataTable GetZleceniaAll()
        {
            string zapytanie = string.Empty;
            zapytanie = "SELECT dbo.Table_zlecenia_prod.id_zlecenia, dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_zlecenia_prod.data, dbo.Table_katalog.KATALOG, dbo.Table_rozmiar.ROZMIAR, dbo.Table_zlecenia_prod.id_przeznaczenie, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_w_worku, dbo.Table_zlecenia_prod.ilosc_wykonana, dbo.Table_zlecenia_prod.zatwierdzone, dbo.Table_zlecenia_prod.wykonane, dbo.Table_zlecenia_prod.ean13_zlecenia, dbo.Table_zlecenia_prod.uwagi_zlecenia FROM dbo.Table_zlecenia_prod LEFT OUTER JOIN dbo.Table_przeznaczenie ON dbo.Table_zlecenia_prod.id_przeznaczenie = dbo.Table_przeznaczenie.id_przeznaczenie LEFT OUTER JOIN dbo.Table_katalog ON dbo.Table_zlecenia_prod.id_katalog = dbo.Table_katalog.ID_KATALOG LEFT OUTER JOIN dbo.Table_rozmiar ON dbo.Table_zlecenia_prod.id_rozmiar = dbo.Table_rozmiar.ID_ROZMIAR ORDER BY dbo.Table_zlecenia_prod.id_zlecenia DESC;";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            conDatabase.Open();
            SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conDatabase.Close();
            return dt;
        }
        public static DataTable GetZleceniaByEAN13Zlecenia(string ean13Zlecenia)
        {
            string zapytanie = string.Empty;
            zapytanie = "SELECT NR_WORKA, NR_KARTY, DATA, NAZWISKO, KATALOG, ROZMIAR, ILOSC, MASZYNA, WAGA, ZMIANA, EAN13, MASZYNA_RODZAJ, ean13_zlecenia FROM Table_glowna WHERE (ean13_zlecenia = '" + ean13Zlecenia.Trim() + "');";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            conDatabase.Open();
            SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conDatabase.Close();
            return dt;
        }
        public static DataTable GetPrzeznaczenie()
        {
            string zapytanie = "SELECT id_przeznaczenie, przeznaczenie FROM Table_przeznaczenie;";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            conDatabase.Open();
            SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conDatabase.Close();
            return dt;
        }
        public static DataTable ZestawienieZlecenia1(int userStatus)
        {
            string zapytanie = string.Empty;
            SqlConnection conDatabase = new SqlConnection(connectionString);
            DataTable dt = new DataTable();
            try
            {
                if (userStatus == 10 || userStatus == 1) // gdy loguje sie Szef Produkcji lub Admin
                {
                    zapytanie = "SELECT  dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona AS IloscZlecona, dbo.Table_zlecenia_prod.ilosc_wykonana AS IloscWykon, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'PALCE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Palce, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KARUZELE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Karuzele,  SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KLINÓWKI' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Klinowki, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'ROZCINANIE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Rozcinanie, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'AUTOLAPY' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Autolapy, dbo.Table_zlecenia_prod.id_przeznaczenie AS Przeznaczenie FROM dbo.Table_glowna LEFT OUTER JOIN   dbo.Table_glowna_s ON dbo.Table_glowna.EAN13 = dbo.Table_glowna_s.EAN13 LEFT OUTER JOIN   dbo.Table_zlecenia_prod ON dbo.Table_glowna.ean13_zlecenia = dbo.Table_zlecenia_prod.ean13_zlecenia  WHERE (dbo.Table_zlecenia_prod.ilosc_zlecona = dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona < dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona > dbo.Table_zlecenia_prod.ilosc_wykonana)  GROUP BY dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_wykonana,   dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_zlecenia_prod.id_przeznaczenie ORDER BY  dbo.Table_zlecenia_prod.id_przeznaczenie, dbo.Table_glowna.KATALOG;";
                }
                else
                    if (userStatus == 11)//gdy loguje sie Koordynator Produkcji 
                    {
                        zapytanie = "SELECT dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona AS IloscZlecona, dbo.Table_zlecenia_prod.ilosc_wykonana AS IloscWykon, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'PALCE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Palce, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KARUZELE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Karuzele,  SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KLINÓWKI' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Klinowki, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'ROZCINANIE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Rozcinanie, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'AUTOLAPY' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Autolapy, dbo.Table_przeznaczenie.przeznaczenie AS Przeznaczenie FROM dbo.Table_przeznaczenie RIGHT OUTER JOIN    dbo.Table_zlecenia_prod ON dbo.Table_przeznaczenie.id_przeznaczenie = dbo.Table_zlecenia_prod.id_przeznaczenie RIGHT OUTER JOIN     dbo.Table_glowna LEFT OUTER JOIN  dbo.Table_glowna_s ON dbo.Table_glowna.EAN13 = dbo.Table_glowna_s.EAN13 ON     dbo.Table_zlecenia_prod.ean13_zlecenia = dbo.Table_glowna.ean13_zlecenia WHERE  (dbo.Table_zlecenia_prod.ilosc_zlecona = dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona < dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona > dbo.Table_zlecenia_prod.ilosc_wykonana) GROUP BY dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_wykonana, dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_przeznaczenie.przeznaczenie ORDER BY  dbo.Table_przeznaczenie.przeznaczenie, dbo.Table_glowna.KATALOG;";
                    }
                //SqlConnection conDatabase = new SqlConnection(connectionString);
                conDatabase.Open();
                SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
                //DataTable dt = new DataTable();
                da.Fill(dt);
                //conDatabase.Close();
            }
            catch { }
            finally { conDatabase.Close(); }
            return dt;
        }
        public static DataTable ZestawienieZlecenia2(int userStatus)
        {
            string zapytanie = string.Empty;
            if (userStatus == 11)//gdy loguje sie Koordynator Produkcji 
                zapytanie = "SELECT nr_zlecenia, Katalog, Rozmiar, IloscZlecona, IloscWykon, Palce, Karuzele, Klinowki, Rozcinanie, Autolapy, Overlock, Przeznaczenie FROM  Table_zlecenia_prod_zestawienie";
            else
                if (userStatus == 10 || userStatus == 1) // gdy loguje sie Szef Produkcji lub Admin
                    zapytanie = "SELECT nr_zlecenia, Katalog, Rozmiar, IloscZlecona, IloscWykon, Palce, Karuzele, Klinowki, Rozcinanie, Autolapy, Overlock, Table_przeznaczenie.id_przeznaczenie FROM  Table_zlecenia_prod_zestawienie LEFT OUTER JOIN Table_przeznaczenie ON Table_zlecenia_prod_zestawienie.Przeznaczenie = Table_przeznaczenie.przeznaczenie";

            //string zapytanie = "SELECT dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona AS IloscZlecona, dbo.Table_zlecenia_prod.ilosc_wykonana AS IloscWykon, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'PALCE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Palce, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KARUZELE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Karuzele,  SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KLINÓWKI' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Klinowki, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'ROZCINANIE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Rozcinanie, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'AUTOLAPY' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Autolapy, dbo.Table_przeznaczenie.przeznaczenie AS Przeznaczenie FROM dbo.Table_przeznaczenie RIGHT OUTER JOIN    dbo.Table_zlecenia_prod ON dbo.Table_przeznaczenie.id_przeznaczenie = dbo.Table_zlecenia_prod.id_przeznaczenie RIGHT OUTER JOIN     dbo.Table_glowna LEFT OUTER JOIN  dbo.Table_glowna_s ON dbo.Table_glowna.EAN13 = dbo.Table_glowna_s.EAN13 ON dbo.Table_zlecenia_prod.ean13_zlecenia = dbo.Table_glowna.ean13_zlecenia WHERE  (dbo.Table_zlecenia_prod.ilosc_zlecona = dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona < dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona > dbo.Table_zlecenia_prod.ilosc_wykonana) GROUP BY dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_wykonana, dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_przeznaczenie.przeznaczenie ORDER BY  dbo.Table_przeznaczenie.przeznaczenie, dbo.Table_glowna.KATALOG;";
            
            SqlConnection conDatabase = new SqlConnection(connectionString);
            conDatabase.Open();
            SqlDataAdapter da = new SqlDataAdapter(zapytanie, conDatabase);
            DataTable dt = new DataTable();
            da.Fill(dt);
            conDatabase.Close();
            return dt;
        }
        public static int ZwiekszenieLicznikaZleceniaProdukcyjnego()
        {
            int licznik = 0;
            string zapytanie = "SELECT Licznik_zlecenia FROM Table_licznik_zlecenia";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
            conDatabase.Open();
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
            while (dr.Read()) { licznik = int.Parse(dr["Licznik_zlecenia"].ToString().Trim()); }
            dr.Close();

            int licznik1 = licznik + 1;
            string zapytanie1 = "UPDATE Table_licznik_zlecenia SET Licznik_zlecenia='" + licznik1 + "' WHERE Licznik_zlecenia='" + licznik + "';";
            SqlCommand command = new SqlCommand(zapytanie1, conDatabase);
            command.ExecuteNonQuery();
            conDatabase.Close();
            return licznik1;
        }
        public static int ZwiekszenieLicznikaIloscStronWydrukuZlecProd(int iloscStronDlaZlecenia, string ean13Zlecenie)
        {
            int licznik = 0;
            string zapytanie1 = "";
            string zapytanie = "SELECT LICZNIK FROM  Table_licznik_stron_zlecenia";
            SqlConnection conDatabase = new SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
            conDatabase.Open();
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
            while (dr.Read()) { licznik = int.Parse(dr["LICZNIK"].ToString().Trim()); }
            dr.Close();

            int licznik1 = licznik + iloscStronDlaZlecenia;
            int licz = licznik;
            for (int i = ++licz; i <= licznik1; i++)
                zapytanie1 += "INSERT INTO Table_licznik_stron_wydruk (licznik_stron, ean13_zlecenie) VALUES ("+i+",'"+ean13Zlecenie+"');";

            zapytanie1 += "UPDATE  Table_licznik_stron_zlecenia SET LICZNIK='" + licznik1 + "' WHERE LICZNIK='" + licznik + "';";
            SqlCommand command = new SqlCommand(zapytanie1, conDatabase);
            command.ExecuteNonQuery();
            conDatabase.Close();
            return licznik1;
        }
        public static string OdczytajBiezacyRok()
        {//sprawdzenie czy user ma niezatwierdzone wpisy w tabeli glowna_f
            string biezacyRok = "-";
            string zapytanie = "SELECT biezacy_rok FROM Table_biezacy_rok;";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        biezacyRok = dr["biezacy_rok"].ToString();
                    }
                    return biezacyRok;
                }
            }
            return biezacyRok;
        }
        public static string OdczytajProcedureZestawienieZlecenCzyJuzKoniec()
        {//sprawdzenie czy procedura juz zakonczyla wykonywanie zestawienia
            string wynik = "-";
            string zapytanie = "SELECT zestawienie_zlecen FROM  Table_zestaw_zlecen_OK;";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        wynik = dr["zestawienie_zlecen"].ToString();
                    }
                    return wynik;
                }
            }
            return wynik;
        }
        public static int GetLicznikStronByEAN13Zlecenie(string ean13Zlecenie)
        {
            int licznikStron = 0;
            string zapytanie = "SELECT  MAX(licznik_stron) AS licznik_stron FROM Table_licznik_stron_wydruk WHERE  ean13_zlecenie='" + ean13Zlecenie + "' ;";
            try
            {
                using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                    conDatabase.Open();
                    SqlDataReader dr = myCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            licznikStron = int.Parse(dr["licznik_stron"].ToString());
                        }
                        return licznikStron;
                    }
                }
            }
            catch { }
            return licznikStron;
        }
        public static int GetIdRozmiar(string rozmiar)
        {
            int idRozmiar = 0;
            string zapytanie = "SELECT ID_ROZMIAR FROM Table_rozmiar WHERE ROZMIAR ='" + rozmiar + "'";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr;
                dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    int.TryParse(dr["ID_ROZMIAR"].ToString(), out idRozmiar);
                }

            }
            return idRozmiar;
        }
        public static int GetIdKatalog(string katalog)
        {
            int idKatalog = 0;
            string zapytanie = "SELECT ID_KATALOG FROM Table_katalog WHERE KATALOG ='" + katalog + "'";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr;
                dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    int.TryParse(dr["ID_KATALOG"].ToString(), out idKatalog);
                }

            }
            return idKatalog;
        }
        public static int GetIdPrzeznaczenie(string przeznaczenie)
        {
            int idPrzeznaczenie = 0;
            string zapytanie = "SELECT id_przeznaczenie FROM Table_przeznaczenie WHERE przeznaczenie ='" + przeznaczenie + "'";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr;
                dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    int.TryParse(dr["id_przeznaczenie"].ToString(), out idPrzeznaczenie);
                }

            }
            return idPrzeznaczenie;
        }
        public static int GetIloscFromZlecenie(string ean13_zlecenie)
        {
            int iloscWykonana = 0;
            string zapytanie = "SELECT id_zlecenia, nr_zlecenia, data, id_katalog, id_rozmiar, id_przeznaczenie, ilosc_zlecona, ilosc_w_worku, ilosc_wykonana, zatwierdzone, wykonane, ean13_zlecenia, uwagi_zlecenia FROM  Table_zlecenia_prod WHERE  (ean13_zlecenia = '" + ean13_zlecenie.Trim() + "')";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                SqlDataReader dr;
                dr = myCommand.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    int.TryParse(dr["ilosc_wykonana"].ToString(), out iloscWykonana);
                }

            }
            return iloscWykonana;
        }
        #region EAN13zlecenia
        public static ulong ZwiekszenieLicznika_ean13Zlecenie()
        {
            ulong licznik = 0;
            string zapytanie = "SELECT LICZNIK FROM  Table_licznik_ean13_zlecenia";
            SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString);
            SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
            conDatabase.Open();
            SqlDataReader dr;
            dr = myCommand.ExecuteReader();
            while (dr.Read()) { licznik = ulong.Parse(dr["LICZNIK"].ToString().Trim()); }
            dr.Close();
            ulong licznik1 = licznik + 1;
            string zapytanie1 = "UPDATE  Table_licznik_ean13_zlecenia SET LICZNIK='" + licznik1 + "' WHERE LICZNIK='" + licznik + "';";
            SqlCommand command = new SqlCommand(zapytanie1, conDatabase);
            command.ExecuteNonQuery();
            conDatabase.Close();
            return licznik1;
        }
        
        public static string DopelnienieEAN13Zlecenie(string ean13)
        {
            string _ean13 = ean13;
            for (int i = 0; i < 11 - ean13.Length; i++)
            {
                _ean13 = "0" + _ean13;
            }
            return "6" + _ean13;
        }
        public static string ObliczEAN13Zlecenie(string KodZakladu, string Rok, string KodProduktu)
        {
            string sTemp = KodZakladu + Rok + KodProduktu;
            int iSum = 0;
            int iDigit = 0;
            string wynikEAN13 = "";

            // Calculate the checksum digit here.
            for (int i = sTemp.Length; i >= 1; i--)
            {
                iDigit = Convert.ToInt32(sTemp.Substring(i - 1, 1));
                // This appears to be backwards but the 
                // EAN-13 checksum must be calculated
                // this way to be compatible with UPC-A.
                if (i % 2 == 0)
                { // odd  
                    iSum += iDigit * 3;
                }
                else
                { // even
                    iSum += iDigit * 1;
                }
            }
            int iCheckSum = (10 - (iSum % 10)) % 10;
            wynikEAN13 = sTemp + iCheckSum.ToString();
            return wynikEAN13;
        }
        public static int ZestawienieZlecenProdukcyjnych()
        {
            int wynik = -1;
            string zapytanie = "DELETE FROM PRODUKCJA_1.dbo.Table_zlecenia_prod_zestawienie; INSERT INTO dbo.Table_zlecenia_prod_zestawienie(nr_zlecenia, Katalog, Rozmiar, IloscZlecona, IloscWykon, Palce, Karuzele, Klinowki, Rozcinanie, Autolapy, Overlock, Przeznaczenie) SELECT dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona AS IloscZlecona, dbo.Table_zlecenia_prod.ilosc_wykonana AS IloscWykon, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'PALCE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Palce, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KARUZELE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Karuzele,  SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'KLINÓWKI' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Klinowki, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'ROZCINANIE' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Rozcinanie, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'AUTOLAPY' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Autolapy, SUM(CASE WHEN dbo.Table_glowna_s.NazwaMaszyny = 'OVERLOCKI' THEN dbo.Table_glowna_s.ILOSC ELSE 0 END) AS Overlocki, dbo.Table_przeznaczenie.przeznaczenie AS Przeznaczenie FROM dbo.Table_przeznaczenie RIGHT OUTER JOIN dbo.Table_zlecenia_prod ON dbo.Table_przeznaczenie.id_przeznaczenie = dbo.Table_zlecenia_prod.id_przeznaczenie RIGHT OUTER JOIN     dbo.Table_glowna LEFT OUTER JOIN  dbo.Table_glowna_s ON dbo.Table_glowna.EAN13 = dbo.Table_glowna_s.EAN13 ON dbo.Table_zlecenia_prod.ean13_zlecenia = dbo.Table_glowna.ean13_zlecenia WHERE  (dbo.Table_zlecenia_prod.ilosc_zlecona = dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona < dbo.Table_zlecenia_prod.ilosc_wykonana) OR (dbo.Table_zlecenia_prod.ilosc_zlecona > dbo.Table_zlecenia_prod.ilosc_wykonana) GROUP BY dbo.Table_glowna.KATALOG, dbo.Table_glowna.ROZMIAR, dbo.Table_zlecenia_prod.ilosc_zlecona, dbo.Table_zlecenia_prod.ilosc_wykonana, dbo.Table_zlecenia_prod.nr_zlecenia, dbo.Table_przeznaczenie.przeznaczenie ORDER BY  dbo.Table_przeznaczenie.przeznaczenie, dbo.Table_glowna.KATALOG;";
            using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                conDatabase.Open();
                wynik = myCommand.ExecuteNonQuery();               
            }
            return wynik;
        }
        #endregion
        public static void SetGridZleceniaLista(DataGridView dgv)
        {
            if (dgv.RowCount > 0)
            {

                dgv.Columns[0].Width = 60;
                dgv.Columns[1].Width = 110;
                //dgv.Columns[2].Width = 270;
                dgv.Columns[3].Width = 250;
                dgv.Columns[5].Width = 70;
                //dgv.Columns[7].Width = 150;
                //dgv.Columns[8].Width = 150;
                dgv.Columns[9].Width = 60;
                dgv.Columns[10].Width = 60;
                dgv.Columns[11].Width = 140;

                //dgv.Columns[0].Visible = false;
                dgv.Columns[0].ReadOnly = true;
                dgv.Columns[1].ReadOnly = true;
                dgv.Columns[2].ReadOnly = true;
                dgv.Columns[3].ReadOnly = true;
                dgv.Columns[4].ReadOnly = true;
                dgv.Columns[5].ReadOnly = true;
                dgv.Columns[6].ReadOnly = true;
                dgv.Columns[7].ReadOnly = true;
                dgv.Columns[8].ReadOnly = true;

                dgv.Columns[9].ReadOnly = true;
                dgv.Columns[10].ReadOnly = true;
                //dgv.Columns[9].ReadOnly = false;
                //dgv.Columns[10].ReadOnly = false;

                dgv.Columns[11].ReadOnly = true;
                dgv.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            for (int i = 0; i < dgv.RowCount - 1; i++)
            {
                try
                {
                                    
                    if (int.Parse(dgv.Rows[i].Cells[6].Value.ToString()) - int.Parse(dgv.Rows[i].Cells[8].Value.ToString()) <= int.Parse(dgv.Rows[i].Cells[6].Value.ToString())*0.1)
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = Color.Orange;//zaznaczam dok gdy ilosci zaplanowane i wykonane sa PRAWIE rowne
                    }
                    if (int.Parse(dgv.Rows[i].Cells[6].Value.ToString())+50 <= int.Parse(dgv.Rows[i].Cells[8].Value.ToString()))
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = Color.Red;//zaznaczam dok gdy ilosci zaplanowane i wykonane sa rowne
                    }
                    if (bool.Parse(dgv.Rows[i].Cells[10].Value.ToString()) == true)
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = Color.Green;//zaznaczam dok zatwierdzone na zielono
                    }  
                }
                catch { }
            }
        }
        public static void SetGridZestawienieZlecenia1(DataGridView dgv)
        {
            if (dgv.RowCount > 0)
            {

                dgv.Columns[0].Width = 170;
                dgv.Columns[1].Width = 260;
                dgv.Columns[2].Width = 150;
                dgv.Columns[3].Width = 105;
                dgv.Columns[4].Width = 105;
                dgv.Columns[5].Width = 75;
                dgv.Columns[6].Width = 75;
                dgv.Columns[7].Width = 80;
                dgv.Columns[8].Width = 70;
                dgv.Columns[9].Width = 60;
                dgv.Columns[10].Width = 60;
                dgv.Columns[11].Width = 130;
                dgv.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
        public static void SetGridZestawienieZleceniaEdycja(DataGridView dgv)
        {
            if (dgv.RowCount > 0)
            {

                dgv.Columns[0].Width = 80;
                dgv.Columns[1].Width = 80;
                dgv.Columns[2].Width = 80;
                //dgv.Columns[3].Width = 105;
                dgv.Columns[4].Width = 200;
                //dgv.Columns[5].Width = 70;
                dgv.Columns[6].Width = 90;
                dgv.Columns[7].Width = 90;
                dgv.Columns[8].Width = 90;
                dgv.Columns[9].Width = 70;
                //dgv.Columns[10].Width = 160;
            }

        }



    }
}
