using System;

namespace KlientuUzsakymuValdymas
{
    public class Uzsakymas
    {
        public int UzsakymoID { get; set; }
        public int KlientoID { get; set; }
        public string Preke { get; set; }
        public int Kiekis { get; set; }
        public DateTime UzsakymoData { get; set; }
        public double KainaUzVnt { get; set; }

        public Uzsakymas(int uzsakymoID, int klientoID, string preke, int kiekis, DateTime uzsakymoData, double kainaUzVnt)
        {
            UzsakymoID = uzsakymoID;
            KlientoID = klientoID;
            Preke = preke;
            Kiekis = kiekis;
            UzsakymoData = uzsakymoData;
            KainaUzVnt = kainaUzVnt;
        }

        public double BendraKaina()
        {
            return Kiekis * KainaUzVnt;
        }
    }
}
