using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BarcoderLib;

namespace Zamowienia
{
    public partial class Form1 : Form
    {
        string connectionString = @"Server=.\;Database=PRODUKCJA_1; User ID=sa;Password=qwer;";//Server=MK1\MSSQLSERVER2012
        pracownik pracownik1 = new pracownik(-1, "-1", -1, "-1");//klasa pracownik
        int selectedIDZamowienia = -1, selectedRowZamowienia = -1, selectedIDPrzeznaczenie = -1;
        string nrZlecenia { get; set; }
        string ean13Zlecenie { get; set; }
        double ileStronDlaZlecenia = 0, ileStronDlaZlecenia1 = 0, licznik = 0;
        int licznikDrukStronZlecProd = 0;
        int licznikStrony = 0;
        int iloscZlecenie = 0;
        int nrWorka = -1;
        bool isEqecuting;

        public Form1()
        {
            InitializeComponent();
            string adresServ = "", haslo = "";
            var file = System.IO.File.OpenText("set.txt");
            adresServ = file.ReadLine();
            haslo = file.ReadLine();
            connectionString = @"Server=" + Szyfrowanie.DeSzyfrowanie(adresServ) + ";Database=Produkcja_1; User ID=sa;Password=" + Szyfrowanie.DeSzyfrowanie(haslo) + ";Asynchronous Processing=true;";
            file.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //Metody.SetConnect();
                tabControl1.TabPages.Remove(tabPage1);
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);
                label2.Text = DateTime.Now.ToShortDateString();
                //dateTimePicker1.Value = DateTime.Now.Date;                
            }
            catch
            {
                tabControl1.TabPages.Remove(tabPage1);
                tabControl1.TabPages.Remove(tabPage2);
                tabControl1.TabPages.Remove(tabPage3);
                MessageBox.Show("Nieprawidłowe dane w pliku konfiguracyjnym, brak połączenia z bazą", "Błąd połączenia z baza");
            }
        }
        #region Logowanie
        private void button1_Click(object sender, EventArgs e)
        {//logowanie
            if (textBox1.Text != "")
            {
                pracownik1 = Metody.Logowanie(textBox1.Text);
                if (pracownik1.get_idPracownik() != -1)
                {
                    if (pracownik1.get_user_status() == 1)//gdy loguje sie admin 
                    {
                        label3.Text = pracownik1.get_pracownik();
                        tabControl1.Controls.Add(tabPage1);
                        //tabControl1.Controls.Add(tabPage2);
                        tabControl1.Controls.Add(tabPage3);
                        dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                        Metody.OdczytajDoCombobox(cmbxKatalog, "katalog", "Table_katalog");
                        Metody.OdczytajDoCombobox(cmbxRozmiar, "rozmiar", "Table_rozmiar");
                        Metody.OdczytajDoCombobox(cmbxPrzeznaczenie, "id_przeznaczenie", "Table_przeznaczenie");
                        Metody.OdczytajDoCombobox(cmbxZlecenia, "ean13_zlecenia", "Table_zlecenia_prod");
                        //dataGridView1.DataSource = Metody.GetZlecenia(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
                        dataGridView1.DataSource = Metody.GetZleceniaAll();
                        Metody.SetGridZleceniaLista(dataGridView1);
                        dataGridView2.DataSource = Metody.GetPrzeznaczenie();
                                //dataGridView3.DataSource = Metody.ZestawienieZlecenia1(pracownik1.get_user_status());
                        if (Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec() == "1")
                        {
                            dataGridView3.DataSource = Metody.ZestawienieZlecenia2(pracownik1.get_user_status());//odswiezanie tabeli wykonywanej za pomoca PROCEDURE
                            Metody.SetGridZestawienieZlecenia1(dataGridView3);
                        }
                        groupBox4.Visible = true;
                    }
                    else
                        if (pracownik1.get_user_status() == 10)//gdy loguje sie szef produkcji
                        {
                            label3.Text = pracownik1.get_pracownik();
                            tabControl1.Controls.Add(tabPage1);
                            tabControl1.Controls.Add(tabPage2);
                            tabControl1.Controls.Add(tabPage3);
                            dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                            Metody.OdczytajDoCombobox(cmbxKatalog, "katalog", "Table_katalog");
                            Metody.OdczytajDoCombobox(cmbxRozmiar, "rozmiar", "Table_rozmiar");
                            Metody.OdczytajDoCombobox(cmbxPrzeznaczenie, "id_przeznaczenie", "Table_przeznaczenie");
                            Metody.OdczytajDoCombobox(cmbxZlecenia, "ean13_zlecenia", "Table_zlecenia_prod");
                            //dataGridView1.DataSource = Metody.GetZlecenia(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
                            dataGridView1.DataSource = Metody.GetZleceniaAll();
                            Metody.SetGridZleceniaLista(dataGridView1);
                            dataGridView2.DataSource = Metody.GetPrzeznaczenie();
                            if (Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec() == "1")
                            {
                                dataGridView3.DataSource = Metody.ZestawienieZlecenia2(pracownik1.get_user_status());//odswiezanie tabeli wykonywanej za pomoca PROCEDURE
                                Metody.SetGridZestawienieZlecenia1(dataGridView3);
                            }
                            Metody.SetGridZestawienieZlecenia1(dataGridView3);
                            groupBox4.Visible = false;
                        }
                        else
                            if (pracownik1.get_user_status() == 11)//gdy loguje sie dzial sprzedazy Koordynator Produkcji
                            {
                                label3.Text = pracownik1.get_pracownik();
                                dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                                tabControl1.Controls.Add(tabPage2);
                                tabControl1.Controls.Add(tabPage3);
                                Metody.OdczytajDoCombobox(cmbxZlecenia, "ean13_zlecenia", "Table_zlecenia_prod");
                                        //dataGridView3.DataSource = Metody.ZestawienieZlecenia1(pracownik1.get_user_status());
                                if (Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec() == "1")
                                {
                                    dataGridView3.DataSource = Metody.ZestawienieZlecenia2(pracownik1.get_user_status());//odswiezanie tabeli wykonywanej za pomoca PROCEDURE
                                    Metody.SetGridZestawienieZlecenia1(dataGridView3);
                                }
                                //dataGridView1.DataSource = Metody.GetZlecenia(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
                                //dataGridView1.DataSource = Metody.GetZleceniaAll();
                                //Metody.SetGridZleceniaLista(dataGridView1);
                                dataGridView2.DataSource = Metody.GetPrzeznaczenie();
                                groupBox4.Visible = true;
                            }

                    button1.Visible = false;
                    button2.Visible = true;
                    textBox1.Clear();
                }
                else
                {
                    MessageBox.Show("Podałeś nieprawidłowe hasło", "Logowanie");
                    textBox1.Clear();
                }

            }
            else
            {
                MessageBox.Show("Wpisz hasło do logowania", "Logowanie");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {//wylogowanie
            button1.Visible = true;
            button2.Visible = false;
            label3.Text = "-";
            pracownik1.set_pracownik_logout();
            tabControl1.TabPages.Remove(tabPage1);
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage3);
        }
        #endregion

        #region Zarzadzanie Zleceniami Produkcyjnymi
        private void button24_Click(object sender, EventArgs e)
        {//zatwierdz-zapisz zamowienie
            if (cmbxKatalog.SelectedIndex != -1 && cmbxRozmiar.SelectedIndex != -1 && cmbxPrzeznaczenie.SelectedIndex != -1 && txbIloscZlecona.Text.Length > 0)
            {
                nrZlecenia = "ZP/" + Metody.ZwiekszenieLicznikaZleceniaProdukcyjnego() + "/" + Metody.OdczytajBiezacyRok();
                ean13Zlecenie = "";
                ulong ean13ZlecenieLicznik = Metody.ZwiekszenieLicznika_ean13Zlecenie();
                ean13Zlecenie = Metody.ObliczEAN13Zlecenie("", "", Metody.DopelnienieEAN13Zlecenie(ean13ZlecenieLicznik.ToString()));
                string zapytanie = "INSERT INTO Table_zlecenia_prod ( nr_zlecenia, data, id_katalog, id_rozmiar, id_przeznaczenie, ilosc_zlecona, ilosc_w_worku, ilosc_wykonana, zatwierdzone, wykonane, ean13_zlecenia, uwagi_zlecenia) " + "VALUES ('" + nrZlecenia + "','" + dateTimePicker1.Text + "', " + Metody.GetIdKatalog(cmbxKatalog.Text) + ", " + Metody.GetIdRozmiar(cmbxRozmiar.Text) + ", " + cmbxPrzeznaczenie.Text + ", " + txbIloscZlecona.Text + ", " + (txbIloscwWorku.Text == string.Empty ? "0" : txbIloscwWorku.Text) + ", 0 , '" + (chbxZatwierdzone.Checked == true ? "True" : "False") + "', '" + (chbxWykonane.Checked == true ? "True" : "False") + "' , '" + ean13Zlecenie + "',@uwagi1);";

                using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conDatabase.Open();
                    SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                    myCommand.Parameters.AddWithValue("@uwagi1", tbUwagi.Text);
                    myCommand.ExecuteNonQuery();
                }

                ileStronDlaZlecenia = Math.Ceiling(double.Parse(txbIloscZlecona.Text) / double.Parse((txbIloscwWorku.Text == string.Empty ? txbIloscZlecona.Text : txbIloscwWorku.Text)));
                ileStronDlaZlecenia1 = ileStronDlaZlecenia;
                licznik = 0;
                ileStronDlaZlecenia = Math.Ceiling(ileStronDlaZlecenia / 2);
                licznikDrukStronZlecProd = Metody.ZwiekszenieLicznikaIloscStronWydrukuZlecProd(Convert.ToInt32(ileStronDlaZlecenia), ean13Zlecenie);
                licznikStrony = licznikDrukStronZlecProd - Convert.ToInt32(ileStronDlaZlecenia.ToString());
                if (chbxZatwierdzone.Checked)
                {
                    for (int i = 0; i < ileStronDlaZlecenia; i++)
                    {
                        //wydruk 
                        licznikStrony += 1;
                        PrintDialog printDialog = new PrintDialog();
                        PrintDocument printDocument = new PrintDocument();
                        printDocument.PrintPage += new PrintPageEventHandler(pd_PrintPage2);
                        printDocument.PrinterSettings = printDialog.PrinterSettings;
                        printDocument.Print();
                    }
                }
                //dataGridView1.DataSource = Metody.GetZlecenia(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
                dataGridView1.DataSource = Metody.GetZleceniaAll();
                Metody.SetGridZleceniaLista(dataGridView1);
                cmbxPrzeznaczenie.SelectedIndex = -1; cmbxKatalog.SelectedIndex = -1; cmbxRozmiar.SelectedIndex = -1;
                txbIloscwWorku.Clear(); txbIloscZlecona.Clear(); chbxWykonane.Checked = false; chbxZatwierdzone.Checked = false;
                dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            }
            else
            { MessageBox.Show("Wprowadz wszystkie dane", "Uwaga"); }
        }

        private void button3_Click(object sender, EventArgs e)
        {//edycja zamowienia
            if (selectedIDZamowienia != -1)
            {
                string zapytanie = "UPDATE  Table_zlecenia_prod SET  data='" + dateTimePicker1.Text + "',  id_katalog='" + Metody.GetIdKatalog(cmbxKatalog.Text) + "',  id_rozmiar='" + Metody.GetIdRozmiar(cmbxRozmiar.Text) + "',  id_przeznaczenie='" + cmbxPrzeznaczenie.Text + "',  ilosc_zlecona='" + txbIloscZlecona.Text + "',  ilosc_w_worku='" + txbIloscwWorku.Text + "',  zatwierdzone='" + (chbxZatwierdzone.Checked == true ? "True" : "False") + "',  wykonane='" + (chbxWykonane.Checked == true ? "True" : "False") + "',uwagi_zlecenia = @uwagi1 WHERE  id_zlecenia=" + selectedIDZamowienia + ";";
                SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString);
                conDatabase.Open();
                SqlCommand command = new SqlCommand(zapytanie, conDatabase);
                command.Parameters.AddWithValue("@uwagi1", tbUwagi.Text);
                command.ExecuteNonQuery();
                conDatabase.Close();

                //double ileStron = Math.Ceiling(double.Parse(txbIloscZlecona.Text) / double.Parse((txbIloscwWorku.Text == "0" ? txbIloscZlecona.Text : txbIloscwWorku.Text)));
                ileStronDlaZlecenia = Math.Ceiling(double.Parse(txbIloscZlecona.Text) / double.Parse((txbIloscwWorku.Text == "0" ? txbIloscZlecona.Text : txbIloscwWorku.Text)));
                if (checkBox1.Checked)
                {
                    ileStronDlaZlecenia1 = double.Parse(numericUpDown1.Value.ToString());
                    ileStronDlaZlecenia = ileStronDlaZlecenia1;
                }
                else
                {
                    ileStronDlaZlecenia1 = ileStronDlaZlecenia;
                }
                licznik = 0;
                ileStronDlaZlecenia = Math.Ceiling(ileStronDlaZlecenia / 2);
                licznikDrukStronZlecProd = Metody.GetLicznikStronByEAN13Zlecenie(ean13Zlecenie);
                licznikStrony = licznikDrukStronZlecProd - Convert.ToInt32(ileStronDlaZlecenia.ToString());
                if (MessageBox.Show("Czy chcesz wydruk zlecenia " + nrZlecenia + " ?", "Wydruk", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (chbxZatwierdzone.Checked)
                    {
                        for (int i = 0; i < ileStronDlaZlecenia; i++)
                        {
                            //wydruk 
                            licznikStrony += 1;
                            PrintDialog printDialog = new PrintDialog();
                            PrintDocument printDocument = new PrintDocument();
                            printDocument.PrintPage += new PrintPageEventHandler(pd_PrintPage2);
                            printDocument.PrinterSettings = printDialog.PrinterSettings;
                            printDocument.Print();
                        }
                    }
                }
                selectedIDZamowienia = -1;
                dataGridView1.DataSource = Metody.GetZleceniaAll();
                Metody.SetGridZleceniaLista(dataGridView1);
                cmbxPrzeznaczenie.SelectedIndex = -1; cmbxKatalog.SelectedIndex = -1; cmbxRozmiar.SelectedIndex = -1;
                txbIloscwWorku.Clear(); txbIloscZlecona.Clear(); chbxWykonane.Checked = false; chbxZatwierdzone.Checked = false;
                tbUwagi.Clear();
                button24.Enabled = true; button3.Enabled = false; button4.Enabled = false;
                dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                label10.Text = "--";
                checkBox1.Checked = false;
                cmbxKatalog.Enabled = true;
                cmbxRozmiar.Enabled = true;
                cmbxPrzeznaczenie.Enabled = true;
                txbIloscwWorku.Enabled = true;
                txbIloscZlecona.Enabled = true;
                button4.Enabled = true;
            }
            else { MessageBox.Show("Wybierz wiersz do edycji", "Uwaga"); }
            if (dataGridView1.RowCount > 1)
            {
                dataGridView1.ClearSelection();
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[selectedRowZamowienia].Index;
                dataGridView1.Refresh();
                dataGridView1.CurrentCell = dataGridView1.Rows[selectedRowZamowienia].Cells[1];
                dataGridView1.Rows[selectedRowZamowienia].Selected = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {//kasowanie zamowienia
            if (MessageBox.Show("Czy na pewno chcesz usunąć dane?", "Potwierdzenie usunięcia", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (selectedIDZamowienia != -1)
                {
                    string zapytanie = "DELETE FROM Table_zlecenia_prod WHERE id_zlecenia=" + selectedIDZamowienia + " AND ilosc_wykonana = 0;";
                    SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString);
                    conDatabase.Open();
                    SqlCommand command = new SqlCommand(zapytanie, conDatabase);
                    int wynik = command.ExecuteNonQuery();
                    conDatabase.Close();
                    selectedIDZamowienia = -1;
                    dataGridView1.DataSource = Metody.GetZleceniaAll();
                    Metody.SetGridZleceniaLista(dataGridView1);
                    cmbxPrzeznaczenie.SelectedIndex = -1; cmbxKatalog.SelectedIndex = -1; cmbxRozmiar.SelectedIndex = -1;
                    txbIloscwWorku.Clear(); txbIloscZlecona.Clear(); chbxWykonane.Checked = false; chbxZatwierdzone.Checked = false;
                    button24.Enabled = true; button3.Enabled = false; button4.Enabled = false;
                    dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    label10.Text = "--"; tbUwagi.Clear();
                    if (wynik > 0) MessageBox.Show("Usunięto zlecenie " + nrZlecenia, "Usuniecie zlecenia");
                    else MessageBox.Show("Usuniecie zlecenia " + nrZlecenia +" jest niemożliwe", "Usuniecie zlecenia");
                }
                else { MessageBox.Show("Wybierz wiersz do edycji", "Uwaga"); }
                if (dataGridView1.RowCount > 1)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows[selectedRowZamowienia].Index;
                    dataGridView1.Refresh();
                    dataGridView1.CurrentCell = dataGridView1.Rows[selectedRowZamowienia].Cells[1];
                    dataGridView1.Rows[selectedRowZamowienia].Selected = true;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {//anuluj
            //dataGridView1.DataSource = Metody.GetZlecenia(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
            dataGridView1.DataSource = Metody.GetZleceniaAll();
            Metody.SetGridZleceniaLista(dataGridView1);
            cmbxPrzeznaczenie.SelectedIndex = -1; cmbxKatalog.SelectedIndex = -1; cmbxRozmiar.SelectedIndex = -1;
            txbIloscwWorku.Clear(); txbIloscZlecona.Clear(); chbxWykonane.Checked = false; chbxZatwierdzone.Checked = false;
            button24.Enabled = true; button3.Enabled = false; button4.Enabled = false;
            selectedIDZamowienia = -1;
            dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            label10.Text = "--"; tbUwagi.Clear();
            checkBox1.Checked = false;
            cmbxKatalog.Enabled = true;
            cmbxRozmiar.Enabled = true;
            cmbxPrzeznaczenie.Enabled = true;
            txbIloscwWorku.Enabled = true;
            txbIloscZlecona.Enabled = true;
            button4.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (!Metody.SprLiczby(txbIloscZlecona.Text))
            {
                //MessageBox.Show("Nieprawidłowy wpis", "Uwaga");
                txbIloscZlecona.Text = txbIloscZlecona.Text.Substring(0, txbIloscZlecona.TextLength - 1);
                txbIloscZlecona.Select(txbIloscZlecona.Text.Length, 0);
            }
        }
        private void txbIloscwWorku_TextChanged(object sender, EventArgs e)
        {
            if (!Metody.SprLiczby(txbIloscwWorku.Text))
            {
                //MessageBox.Show("Nieprawidłowy wpis", "Uwaga");
                txbIloscwWorku.Text = txbIloscwWorku.Text.Substring(0, txbIloscwWorku.TextLength - 1);
                txbIloscwWorku.Select(txbIloscwWorku.Text.Length, 0);
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            if (pracownik1.get_user_status() != 10)
                dataGridView1.DataSource = Metody.GetZlecenia(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
            else
                dataGridView1.DataSource = Metody.GetZleceniaPrzeznaczenieID(chxbZatwierdzoneFiltr.Checked, chxbWykonaneFiltr.Checked);
            Metody.SetGridZleceniaLista(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {//klik w tabele z Zleceniami
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            DataGridViewCellCollection cellInRow = row.Cells;
            try
            {
                selectedRowZamowienia = row.Index;//selected row
                selectedIDZamowienia = int.Parse(cellInRow[0].Value.ToString());//id_zamowienia
                label10.Text = cellInRow[1].Value.ToString();//nr zlecenia
                nrZlecenia = cellInRow[1].Value.ToString();//nr zlecenia
                dateTimePicker1.Text = cellInRow[2].Value.ToString();//data
                cmbxKatalog.Text = cellInRow[3].Value.ToString();//katalog
                cmbxRozmiar.Text = cellInRow[4].Value.ToString();//rozmiar
                cmbxPrzeznaczenie.Text = cellInRow[5].Value.ToString();//przeznaczenie
                txbIloscZlecona.Text = cellInRow[6].Value.ToString();//ilosc zlecona
                txbIloscwWorku.Text = cellInRow[7].Value.ToString();//ilosc w worku
                ean13Zlecenie = cellInRow[11].Value.ToString();//ean13
                tbUwagi.Text = cellInRow[12].Value.ToString();//uwagi zlecenia

                chbxZatwierdzone.Checked = (cellInRow[9].Value.ToString() == string.Empty ? false : (bool.Parse(cellInRow[9].Value.ToString()) == true ? true : false));
                chbxWykonane.Checked = (cellInRow[10].Value.ToString() == string.Empty ? false : (bool.Parse(cellInRow[10].Value.ToString()) == true ? true : false));
                button24.Enabled = false; button3.Enabled = true; button4.Enabled = true;
                if (cellInRow[8].Value.ToString() == "0")
                {
                    cmbxKatalog.Enabled = true;
                    cmbxRozmiar.Enabled = true;
                    cmbxPrzeznaczenie.Enabled = true;
                    txbIloscwWorku.Enabled = true;
                    txbIloscZlecona.Enabled = true;
                    button4.Enabled = true;
                }else
                    if (int.Parse(cellInRow[8].Value.ToString()) > 0)
                    {
                        cmbxKatalog.Enabled = false;
                        cmbxRozmiar.Enabled = false;                        
                        cmbxPrzeznaczenie.Enabled = false;
                        txbIloscwWorku.Enabled = true;
                        txbIloscZlecona.Enabled = true;
                        button4.Enabled = false;
                    }
            }
            catch { MessageBox.Show("Wybierz wiersz", "Uwaga"); }
        }

        void pd_PrintPage2(object sender, PrintPageEventArgs e)//wydruk do Zlecen Produkcyjnych
        {
            Graphics g = e.Graphics;
            int down = 590;
            try
            {
                using (Font normalFont = new Font("Arial", 10, GraphicsUnit.Point),
                           boldFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point),
                           boldFontBig = new Font("Arial", 18, FontStyle.Bold, GraphicsUnit.Point),
                           boldFontBig1 = new Font("Arial", 26, FontStyle.Bold, GraphicsUnit.Point)
                           )
                {
                    licznik += 1;
                    BarcodeEAN13 encoderEAN13 = new BarcodeEAN13();

                    g.DrawString("ARTYKUŁ: " + cmbxKatalog.Text, boldFontBig, Brushes.Black, 20, 10);
                    g.DrawString("ROZMIAR: " + cmbxRozmiar.Text, boldFontBig, Brushes.Black, 420, 10);
                    g.DrawString(nrZlecenia, normalFont, Brushes.Black, 660, 10);
                    g.DrawString("ILOSC: " + txbIloscZlecona.Text, normalFont, Brushes.Black, 660, 25);

                    //g.DrawString("Page:" + licznikStrony.ToString(), boldFont, Brushes.Black, 20, 45);
                    //g.DrawString(dziewialniaDrukEAN13_1.getNrWorka().ToString(), boldFontBig, Brushes.Black, 20, 55);
                    g.DrawString("DATA: ", normalFont, Brushes.Black, 115, 45);
                    g.DrawString(dateTimePicker1.Text, boldFont, Brushes.Black, 115, 60);
                    g.DrawString("NAZWISKO: ", normalFont, Brushes.Black, 220, 45);
                    //g.DrawString(dziewialniaDrukEAN13_1.getNazwisko().ToString(), boldFont, Brushes.Black, 220, 50);
                    g.DrawString("MASZYNA: ", normalFont, Brushes.Black, 600, 45);
                    //g.DrawString(dziewialniaDrukEAN13_1.getMaszyna().ToString(), boldFont, Brushes.Black, 600, 50);
                    g.DrawString("Ilosc wor:", normalFont, Brushes.Black, 700, 45);
                    g.DrawString(txbIloscwWorku.Text, normalFont, Brushes.Black, 700, 60);

                    g.DrawString("DZIANIE: ", normalFont, Brushes.Black, 20, 90);
                    g.DrawString("SZYCIE: ", normalFont, Brushes.Black, 20, 140);
                    g.DrawString("PALCY: ", normalFont, Brushes.Black, 20, 160);
                    g.DrawString("SZYCIE: ", normalFont, Brushes.Black, 20, 200);
                    g.DrawString("MAJTEK: ", normalFont, Brushes.Black, 20, 220);
                    g.DrawString("SZYCIE: ", normalFont, Brushes.Black, 20, 250);
                    g.DrawString("KLINA: ", normalFont, Brushes.Black, 20, 270);
                    g.DrawString("WYSYŁKA: ", normalFont, Brushes.Black, 20, 310);
                    g.DrawString("DO FARBY: ", normalFont, Brushes.Black, 20, 330);

                    RectangleF rect = new RectangleF(18, 9, 750, normalFont.Height + 20);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect));
                    RectangleF rect1 = new RectangleF(18, 44, 750, normalFont.Height + 20);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect1));
                    RectangleF rect2 = new RectangleF(18, 80, 750, normalFont.Height + 40);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect2));
                    RectangleF rect3 = new RectangleF(18, 136, 750, normalFont.Height + 40);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect3));
                    RectangleF rect4 = new RectangleF(18, 192, 750, normalFont.Height + 40);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect4));
                    RectangleF rect5 = new RectangleF(18, 248, 750, normalFont.Height + 40);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect5));
                    RectangleF rect6 = new RectangleF(18, 304, 750, normalFont.Height + 40);
                    g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect6));

                    g.DrawLine(Pens.Black, 112, 45, 112, 360);
                    g.DrawLine(Pens.Black, 218, 45, 218, 360);
                    g.DrawLine(Pens.Black, 598, 45, 598, 360);
                    g.DrawLine(Pens.Black, 698, 45, 698, 360);

                    Bitmap b = new Bitmap(700, 700);
                    //Image img;
                    b = encoderEAN13.Encode(ean13Zlecenie.Substring(0, ean13Zlecenie.Length - 1));
                    //Image img = encoderEAN13.Encode(dziewialniaDrukEAN13_1.getEAN13Licznik().ToString().Trim());
                    //g.DrawString("EAN13: " + dziewialniaDrukEAN13_1.getEAN13().ToString(), boldFont, Brushes.Black, 20, 70);
                    //picOutput.Image = encoderEAN13.Encode(dziewialnia1.getEAN13().ToString().Trim();
                    g.DrawImage(b, 20, 370);
                    g.DrawImage(b, 220, 370);
                    g.DrawImage(b, 420, 370);
                    g.DrawImage(b, 620, 370);
                    //g.DrawImage(b, 220, 70);
                    //g.DrawImage(b, 420, 70);
                    g.DrawString(cmbxKatalog.Text + ", " + cmbxRozmiar.Text, boldFontBig1, Brushes.Black, 20, 460);
                    g.DrawString("Uwagi:" + tbUwagi.Text, normalFont, Brushes.Black, 20, 500);

                    //--------------------druga kopia ponizej----------------------------------------------
                    g.DrawString("-----------------------------------------------------------------------------------------------------------------------------------------------------------", normalFont, Brushes.Black, 20, down - 20);
                    licznik += 1;
                    if (ileStronDlaZlecenia1 % 2 != 0 && ileStronDlaZlecenia1 == (licznik - 1))
                    { }
                    else
                    {
                        g.DrawString("ARTYKUŁ: " + cmbxKatalog.Text, boldFontBig, Brushes.Black, 20, 10 + down);
                        g.DrawString("ROZMIAR: " + cmbxRozmiar.Text, boldFontBig, Brushes.Black, 420, 10 + down);
                        g.DrawString(nrZlecenia, normalFont, Brushes.Black, 660, 10 + down);
                        g.DrawString("ILOSC:" + txbIloscZlecona.Text, normalFont, Brushes.Black, 660, 25 + down);

                        //g.DrawString("Page:" + licznikStrony.ToString(), boldFont, Brushes.Black, 20, 45 + down);
                        //g.DrawString(dziewialniaDrukEAN13_1.getNrWorka().ToString(), boldFontBig, Brushes.Black, 20, 55);
                        g.DrawString("DATA: ", normalFont, Brushes.Black, 115, 45 + down);
                        g.DrawString(dateTimePicker1.Text, boldFont, Brushes.Black, 115, 60 + down);
                        g.DrawString("NAZWISKO: ", normalFont, Brushes.Black, 220, 45 + down);
                        //g.DrawString(dziewialniaDrukEAN13_1.getNazwisko().ToString(), boldFont, Brushes.Black, 220, 50);
                        g.DrawString("MASZYNA: ", normalFont, Brushes.Black, 600, 45 + down);
                        //g.DrawString(dziewialniaDrukEAN13_1.getMaszyna().ToString(), boldFont, Brushes.Black, 600, 50);
                        g.DrawString("Ilosc wor:", normalFont, Brushes.Black, 700, 45 + down);
                        g.DrawString(txbIloscwWorku.Text, normalFont, Brushes.Black, 700, 60 + down);

                        g.DrawString("DZIANIE: ", normalFont, Brushes.Black, 20, 90 + down);
                        g.DrawString("SZYCIE: ", normalFont, Brushes.Black, 20, 140 + down);
                        g.DrawString("PALCY: ", normalFont, Brushes.Black, 20, 160 + down);
                        g.DrawString("SZYCIE: ", normalFont, Brushes.Black, 20, 200 + down);
                        g.DrawString("MAJTEK: ", normalFont, Brushes.Black, 20, 220 + down);
                        g.DrawString("SZYCIE: ", normalFont, Brushes.Black, 20, 250 + down);
                        g.DrawString("KLINA: ", normalFont, Brushes.Black, 20, 270 + down);
                        g.DrawString("WYSYŁKA: ", normalFont, Brushes.Black, 20, 310 + down);
                        g.DrawString("DO FARBY: ", normalFont, Brushes.Black, 20, 330 + down);

                        RectangleF rect_2 = new RectangleF(18, 9 + down, 750, normalFont.Height + 20);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect_2));
                        RectangleF rect1_2 = new RectangleF(18, 44 + down, 750, normalFont.Height + 20);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect1_2));
                        RectangleF rect2_2 = new RectangleF(18, 80 + down, 750, normalFont.Height + 40);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect2_2));
                        RectangleF rect3_2 = new RectangleF(18, 136 + down, 750, normalFont.Height + 40);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect3_2));
                        RectangleF rect4_2 = new RectangleF(18, 192 + down, 750, normalFont.Height + 40);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect4_2));
                        RectangleF rect5_2 = new RectangleF(18, 248 + down, 750, normalFont.Height + 40);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect5_2));
                        RectangleF rect6_2 = new RectangleF(18, 304 + down, 750, normalFont.Height + 40);
                        g.DrawRectangle(Pens.Black, Rectangle.Truncate(rect6_2));

                        g.DrawLine(Pens.Black, 112, 45 + down, 112, 360 + down);
                        g.DrawLine(Pens.Black, 218, 45 + down, 218, 360 + down);
                        g.DrawLine(Pens.Black, 598, 45 + down, 598, 360 + down);
                        g.DrawLine(Pens.Black, 698, 45 + down, 698, 360 + down);

                        Bitmap b_2 = new Bitmap(700, 700);
                        //Image img;
                        b_2 = encoderEAN13.Encode(ean13Zlecenie.Substring(0, ean13Zlecenie.Length - 1));
                        //Image img = encoderEAN13.Encode(dziewialniaDrukEAN13_1.getEAN13Licznik().ToString().Trim());
                        //g.DrawString("EAN13: " + dziewialniaDrukEAN13_1.getEAN13().ToString(), boldFont, Brushes.Black, 20, 70);
                        //picOutput.Image = encoderEAN13.Encode(dziewialnia1.getEAN13().ToString().Trim();
                        g.DrawImage(b_2, 20, 370 + down);
                        g.DrawImage(b_2, 220, 370 + down);
                        g.DrawImage(b_2, 420, 370 + down);
                        g.DrawImage(b_2, 620, 370 + down);
                        //g.DrawImage(b, 220, 70);
                        //g.DrawImage(b, 420, 70);
                        g.DrawString(cmbxKatalog.Text + ", " + cmbxRozmiar.Text, boldFontBig1, Brushes.Black, 20, 1050);
                        g.DrawString("Uwagi:" + tbUwagi.Text, normalFont, Brushes.Black, 20, 1090);
                    }
                }
            }
            catch { MessageBox.Show("Brak danych do drukowania", "Uwaga"); }

        }
        #endregion

        private void button7_Click(object sender, EventArgs e)
        {//dodanie wiersza do tabeli przeznaczenie
            if (textBox2.TextLength > 0)
            {
                string zapytanie = "INSERT INTO Table_przeznaczenie (przeznaczenie) VALUES (@przeznaczenie1);";
                using (SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conDatabase.Open();
                    SqlCommand myCommand = new SqlCommand(zapytanie, conDatabase);
                    myCommand.Parameters.AddWithValue("@przeznaczenie1", textBox2.Text);
                    myCommand.ExecuteNonQuery();
                }
                dataGridView2.DataSource = Metody.GetPrzeznaczenie();
                textBox2.Clear();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {//klik w tabele przeznaczenie
            DataGridViewRow row = dataGridView2.SelectedRows[0];
            DataGridViewCellCollection cellInRow = row.Cells;
            //selectedRowZamowienia = row.Index;//selected row
            selectedIDPrzeznaczenie = int.Parse(cellInRow[0].Value.ToString());//id_zamowienia
            textBox2.Text = cellInRow[1].Value.ToString();
            button7.Enabled = false; button8.Enabled = true; button11.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {//kasowanie z tabeli przeznaczenie
            if (MessageBox.Show("Czy na pewno chcesz usunąć dane?", "Potwierdzenie usunięcia", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (selectedIDPrzeznaczenie != -1)
                {
                    string zapytanie = "DELETE FROM  Table_przeznaczenie WHERE  id_przeznaczenie=" + selectedIDPrzeznaczenie + ";";
                    SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString);
                    conDatabase.Open();
                    SqlCommand command = new SqlCommand(zapytanie, conDatabase);
                    command.ExecuteNonQuery();
                    conDatabase.Close();
                    selectedIDPrzeznaczenie = -1;
                    dataGridView2.DataSource = Metody.GetPrzeznaczenie();
                    button7.Enabled = true; button8.Enabled = false; button11.Enabled = false;
                    textBox2.Clear();
                }
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {//edycja przeznaczenie
            if (MessageBox.Show("Czy na pewno chcesz edytowac dane?", "Potwierdzenie edycji", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (selectedIDPrzeznaczenie != -1)
                {
                    string zapytanie = "UPDATE Table_przeznaczenie SET przeznaczenie=@przeznaczenie1 WHERE  id_przeznaczenie=" + selectedIDPrzeznaczenie + ";";
                    SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString);
                    conDatabase.Open();
                    SqlCommand command = new SqlCommand(zapytanie, conDatabase);
                    command.Parameters.AddWithValue("@przeznaczenie1", textBox2.Text);
                    command.ExecuteNonQuery();
                    conDatabase.Close();
                    selectedIDPrzeznaczenie = -1;
                    dataGridView2.DataSource = Metody.GetPrzeznaczenie();
                    button7.Enabled = true; button8.Enabled = false; button11.Enabled = false;
                    textBox2.Clear();
                }
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {//anuluj - przeznaczenie
            selectedIDPrzeznaczenie = -1;
            dataGridView2.DataSource = Metody.GetPrzeznaczenie();
            button7.Enabled = true; button8.Enabled = false; button11.Enabled = false;
            textBox2.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Metody.GetZleceniaAll();
            Metody.SetGridZleceniaLista(dataGridView1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //DataTable dt = Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec();
            if (Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec() == "1")
            {
                label16.Text = "--";
                isEqecuting = false;
                timer1.Enabled = false;
                dataGridView3.DataSource = Metody.ZestawienieZlecenia2(pracownik1.get_user_status());
                Metody.SetGridZestawienieZlecenia1(dataGridView3);
            }
            //dataGridView3.DataSource = Metody.ZestawienieZlecenia1(pracownik1.get_user_status());
            //Metody.SetGridZestawienieZlecenia1(dataGridView3);
        }

        private void cmbxZlecenia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbxZlecenia.SelectedIndex != -1)
            {
                dataGridView4.DataSource = Metody.GetZleceniaByEAN13Zlecenia(cmbxZlecenia.Text.Trim());
                Metody.SetGridZestawienieZleceniaEdycja(dataGridView4);
            }
        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {//klik w tabele z zleceniami
            try
            {
                DataGridViewRow row = dataGridView4.SelectedRows[0];
                DataGridViewCellCollection cellInRow = row.Cells;
                selectedRowZamowienia = row.Index;//selected row
                nrWorka = int.Parse(cellInRow[0].Value.ToString());//NR_WORKA
                textBox3.Text = cellInRow[6].Value.ToString();//ILOSC
                iloscZlecenie = int.Parse(cellInRow[6].Value.ToString());
                button12.Enabled = true;
            }
            catch { }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            cmbxZlecenia.SelectedIndex = -1;
            textBox3.Clear();
            button12.Enabled = false;
            dataGridView4.DataSource = null;
        }

        private void button12_Click(object sender, EventArgs e)
        {//edycja ilosci w zleceniu i tabeli glowna
            if (textBox3.TextLength > 0 && selectedRowZamowienia!=-1 && nrWorka!=-1 && cmbxZlecenia.SelectedIndex!=-1)
            {
                if (MessageBox.Show("Czy na pewno chcesz edytowac dane zlecenia: "+cmbxZlecenia.Text+" ?", "Potwierdzenie edycji", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int roznicaZlecenie = int.Parse(textBox3.Text) - iloscZlecenie;
                    int iloscWykonanaZlecenie = 0;
                    string zapytanie = string.Empty;
                    if (roznicaZlecenie != 0)
                    {
                        zapytanie += " UPDATE Table_glowna SET ILOSC = " + textBox3.Text + " WHERE NR_WORKA=" + nrWorka + "; ";
                        iloscWykonanaZlecenie = Metody.GetIloscFromZlecenie(cmbxZlecenia.Text);
                        zapytanie += " UPDATE Table_zlecenia_prod SET ilosc_wykonana = " + (iloscWykonanaZlecenie + roznicaZlecenie) + " WHERE ean13_zlecenia='" + cmbxZlecenia.Text + "';";
                        try
                        {
                            using (var conDatabase = Helper.GetDBConnection())
                            {
                                var cmd = new SqlCommand(zapytanie, conDatabase);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            //     Error<Helper>.RegError(ex);
                        }
                        dataGridView4.DataSource = Metody.GetZleceniaByEAN13Zlecenia(cmbxZlecenia.Text.Trim());
                        Metody.SetGridZestawienieZleceniaEdycja(dataGridView4);
                    }
                }
            }
            if (dataGridView4.RowCount > 1)
            {
                dataGridView4.ClearSelection();
                dataGridView4.FirstDisplayedScrollingRowIndex = dataGridView4.Rows[selectedRowZamowienia].Index;
                dataGridView4.Refresh();
                dataGridView4.CurrentCell = dataGridView4.Rows[selectedRowZamowienia].Cells[6];
                dataGridView4.Rows[selectedRowZamowienia].Selected = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 )
            {
                e.Handled = true;
                return;
            }
            //if (e.KeyChar == 46 || e.KeyChar == 45)
            //{
            //    if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
            //        e.Handled = true;
            //}
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //if (Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec() == "1" && e.KeyCode == Keys.F5)
            //{
            //    //dataGridView3.DataSource = Metody.ZestawienieZlecenia1(pracownik1.get_user_status());
            //    dataGridView3.DataSource = Metody.ZestawienieZlecenia2(pracownik1.get_user_status());
            //    Metody.SetGridZestawienieZlecenia1(dataGridView3);
            //}
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Metody.SetGridZleceniaLista(dataGridView1);
        }

        private void dataGridView3_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Metody.SetGridZestawienieZlecenia1(dataGridView3);
        }

        #region Async Metoda
        private void button14_Click(object sender, EventArgs e)
        {//uruchomienie Stored Procedure - odswiezenie tabeli z zestawieniami
            SqlConnection conDatabase = new System.Data.SqlClient.SqlConnection(connectionString);
            if (isEqecuting || Metody.OdczytajProcedureZestawienieZlecenCzyJuzKoniec() == "0")
            {
                MessageBox.Show(this,"Trwa procedura zapisu zlecen.");
            }
            else
            {
                try
                {
                    if (MessageBox.Show("Czy na pewno chcesz rozpocząć procedure zapisu zleceń: " + cmbxZlecenia.Text + " ?", "Potwierdzenie edycji", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        conDatabase.Open();
                        SqlCommand cmd = new SqlCommand("Table_zlecenia_prod_zestawienie_PROCEDURE", conDatabase);

                        timer1.Enabled = true;
                        isEqecuting = true;
                        label16.Text = "Trwa procedura zapisu zlecen.";

                        AsyncCallback callback = new AsyncCallback(HandleCallback);
                        var zm1 = cmd.BeginExecuteNonQuery(callback, cmd);
                        //cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.ExecuteNonQuery();
                        //Metody.ZestawienieZlecenProdukcyjnych();
                        //----------------------------------------------------------
                        //dataGridView3.DataSource = Metody.ZestawienieZlecenia2(pracownik1.get_user_status());
                        //Metody.SetGridZestawienieZlecenia1(dataGridView3);
                    }
                }
                catch (Exception ex)
                {
                    isEqecuting = false;
                    DisplayStatus(string.Format("Ready (last error: {0})", ex.Message));
                    if (conDatabase != null)
                    {
                        conDatabase.Close();
                    }
                }
                finally
                {
                    //isEqecuting = false;
                    //if (conDatabase != null)
                    //{
                    //    conDatabase.Close();
                    //}
                }
            }
        }
        
        private void HandleCallback(IAsyncResult result)
        {
            try
            {                
                SqlCommand command = (SqlCommand)result.AsyncState;
                int rowCount = command.EndExecuteNonQuery(result);
                string rowText = " rows affected.";
                if (rowCount == 1)
                {
                    rowText = " row affected.";
                }
                rowText = rowCount + rowText;

                DisplayInfoDelegate del = new DisplayInfoDelegate(DisplayResults);
                this.Invoke(del, rowText);

            }
            catch (Exception ex)
            {                
                this.Invoke(new DisplayInfoDelegate(DisplayStatus),
                    String.Format("Ready(last error: {0}", ex.Message));
            }
            finally
            {
                ////isExecuting = false;
                //if (connection != null)
                //{
                //    connection.Close();
                //}
            }
        }
        private delegate void DisplayInfoDelegate(string Text);
        private void DisplayStatus(string Text)
        {
            //this.label16.Text = Text;
        }
        private void DisplayResults(string Text)
        {
            //this.label16.Text = Text;
            //DisplayStatus("Ready");
        }
        #endregion


    }
}
