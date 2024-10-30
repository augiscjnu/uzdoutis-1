using System;

namespace KlientuUzsakymuValdymas
{
    public class Klientas
    {
        public int KlientoID { get; set; }
        public string Vardas { get; set; }
        public string Pavarde { get; set; }
        public string TelefonoNumeris { get; set; }
        public string ElPastas { get; set; }

        public Klientas(int klientoID, string vardas, string pavarde, string telefonoNumeris, string elPastas)
        {
            KlientoID = klientoID;
            Vardas = vardas;
            Pavarde = pavarde;
            TelefonoNumeris = telefonoNumeris;
            ElPastas = elPastas;
        }
    }
}
