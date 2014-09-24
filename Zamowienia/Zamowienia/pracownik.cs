using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zamowienia
{
    public class pracownik
    {
        private int idPracownik;
        private string pracownik1;
        private int admin;
        private string zmiana;

        public pracownik(int _idPracownik, string _pracownik, int _admin, string _zmiana)
        {
            idPracownik = _idPracownik;
            pracownik1 = _pracownik;
            admin = _admin;
            zmiana = _zmiana;
        }

        public int get_idPracownik()
        { return idPracownik; }

        public string get_pracownik()
        { return pracownik1; }

        public int get_user_status()
        { return admin; }

        public string get_zmiana()
        { return zmiana; }

        public void set_pracownik_logout()
        {
            idPracownik = -1;
            pracownik1 = "-1";
            admin = -1;
            zmiana = "-1";
        }
    }
}
