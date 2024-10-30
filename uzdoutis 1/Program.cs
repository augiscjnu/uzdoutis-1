using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KlientuUzsakymuValdymas
{
    public class Program
    {
        private static List<Klientas> klientai = new List<Klientas>();
        private static List<Uzsakymas> uzsakymai = new List<Uzsakymas>();

        public static void Main(string[] args)
        {
            string klientaiFailas = "klientai.txt";
            string uzsakymaiFailas = "uzsakymai.txt";

            NuskaitytiKlientusIsFailo(klientaiFailas);
            NuskaitytiUzsakymusIsFailo(uzsakymaiFailas);

            while (true)
            {
                Console.WriteLine("Pasirinkite veiksmą:");
                Console.WriteLine("1. Peržiūrėti visus klientus ir jų užsakymus");
                Console.WriteLine("2. Peržiūrėti užsakymą pagal ID");
                Console.WriteLine("3. Pridėti naują klientą");
                Console.WriteLine("4. Pridėti naują užsakymą klientui");
                Console.WriteLine("5. Ištrinti klientą pagal ID");
                Console.WriteLine("6. Ištrinti užsakymą pagal ID");
                Console.WriteLine("7. Išsaugoti visus pakeitimus į failą");
                Console.WriteLine("8. Išeiti");

                var pasirinkimas = Console.ReadLine();
                switch (pasirinkimas)
                {
                    case "1":
                        PerziuretiKlientusIrUzsakymus();
                        break;
                    case "2":
                        PerziuretiUzsakymusPagalID();
                        break;
                    case "3":
                        PridetiNaujaKlienta();
                        break;
                    case "4":
                        PridetiNaujaUzsakyma();
                        break;
                    case "5":
                        IstrintiKlienta();
                        break;
                    case "6":
                        IstrintiUzsakyma();
                        break;
                    case "7":
                        IšsaugotiPakeitimus();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Neteisingas pasirinkimas. Bandykite dar kartą.");
                        break;
                }
            }
        }

        private static void NuskaitytiKlientusIsFailo(string failoKelias)
        {
            try
            {
                using (var reader = new StreamReader(failoKelias))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var dalys = line.Split(',');

                        if (dalys.Length != 5) continue;

                        int id = int.Parse(dalys[0]);
                        string vardas = dalys[1];
                        string pavarde = dalys[2];
                        string tel = dalys[3];
                        string el = dalys[4];

                        klientai.Add(new Klientas(id, vardas, pavarde, tel, el));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Klaida: failas '{failoKelias}' nerastas.");
            }
        }

        private static void NuskaitytiUzsakymusIsFailo(string failoKelias)
        {
            try
            {
                using (var reader = new StreamReader(failoKelias))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var dalys = line.Split(',');

                        if (dalys.Length != 6) continue;

                        int id = int.Parse(dalys[0]);
                        int klientoID = int.Parse(dalys[1]);
                        string preke = dalys[2];
                        int kiekis = int.Parse(dalys[3]);
                        DateTime data = DateTime.Parse(dalys[4]);
                        var kaina = double.Parse(dalys[5]);

                        uzsakymai.Add(new Uzsakymas(id, klientoID, preke, kiekis, data, kaina));
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Klaida: failas '{failoKelias}' nerastas.");
            }
        }

        private static void PerziuretiKlientusIrUzsakymus()
        {
            foreach (var klientas in klientai)
            {
                Console.WriteLine($"Klientas: {klientas.Vardas}  {klientas.Pavarde}");
                var klientoUzsakymai = uzsakymai.Where(u => u.KlientoID == klientas.KlientoID).ToList();

                if (!klientoUzsakymai.Any())
                {
                    Console.WriteLine("Klientas šiuo metu neturi aktyvių užsakymų.");
                }
                else
                {
                    foreach (var uzsakymas in klientoUzsakymai)
                    {
                        Console.WriteLine($"  Užsakymas ID: {uzsakymas.UzsakymoID}, Prekė: {uzsakymas.Preke}, Kiekis: {uzsakymas.Kiekis}, " +
                                          $"Data: {uzsakymas.UzsakymoData.ToShortDateString()}, Kaina už vnt: {uzsakymas.KainaUzVnt} EUR");
                        if (uzsakymas.BendraKaina() > 1000)
                        {
                            Console.WriteLine($"Klientas {klientas.Vardas} {klientas.Pavarde} turi užsakymą su bendra verte virš 1000 eurų.");
                        }
                        if (uzsakymas.UzsakymoData < DateTime.Now.AddYears(-1))
                        {
                            Console.WriteLine($"Užsakymas {uzsakymas.Preke} klientui {klientas.Vardas} {klientas.Pavarde} pateiktas daugiau nei prieš metus.");
                        }
                    }
                }
            }

            var bendraSuma = uzsakymai.Where(u => u.UzsakymoData < DateTime.Now.AddYears(-1)).Sum(u => u.BendraKaina());
            Console.WriteLine($"Bendra užsakymų, kurie yra senesni nei metai, suma: {bendraSuma} EUR");

            var klientuKiekis = klientai.Count(k => uzsakymai.Where(u => u.KlientoID == k.KlientoID).Sum(u => u.BendraKaina()) > 5000);
            Console.WriteLine($"Klientų, kurių bendra užsakymų suma viršija 5000 EUR: {klientuKiekis}");
        }

        private static void PerziuretiUzsakymusPagalID()
        {
            Console.Write("Įveskite užsakymo ID: ");
            int uzsakymoID = int.Parse(Console.ReadLine());

            var uzsakymas = uzsakymai.FirstOrDefault(u => u.UzsakymoID == uzsakymoID);
            if (uzsakymas != null)
            {
                var klientas = klientai.FirstOrDefault(k => k.KlientoID == uzsakymas.KlientoID);
                Console.WriteLine($"Užsakymas ID: {uzsakymas.UzsakymoID}, Klientas: {klientas?.Vardas} {klientas?.Pavarde}, Prekė: {uzsakymas.Preke}, " +
                                  $"Kiekis: {uzsakymas.Kiekis}, Data: {uzsakymas.UzsakymoData.ToShortDateString()}, Kaina už vnt: {uzsakymas.KainaUzVnt} EUR");
            }
            else
            {
                Console.WriteLine("Užsakymas nerastas.");
            }
        }

        private static void PridetiNaujaKlienta()
        {
            Console.Write("Įveskite kliento ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Įveskite kliento vardą: ");
            string vardas = Console.ReadLine();
            Console.Write("Įveskite kliento pavardę: ");
            string pavarde = Console.ReadLine();
            Console.Write("Įveskite kliento telefono numerį: ");
            string telefonoNumeris = Console.ReadLine();
            Console.Write("Įveskite kliento el. pašto adresą: ");
            string elPastas = Console.ReadLine();

            klientai.Add(new Klientas(id, vardas, pavarde, telefonoNumeris, elPastas));
            Console.WriteLine("Naujas klientas pridėtas.");
        }

        private static void PridetiNaujaUzsakyma()
        {
            Console.Write("Įveskite užsakymo ID: ");
            int uzsakymoID = int.Parse(Console.ReadLine());
            Console.Write("Įveskite kliento ID: ");
            int klientoID = int.Parse(Console.ReadLine());
            Console.Write("Įveskite prekės pavadinimą: ");
            string preke = Console.ReadLine();
            Console.Write("Įveskite užsakymo kiekį: ");
            int kiekis = int.Parse(Console.ReadLine());
            Console.Write("Įveskite užsakymo datą (yyyy-mm-dd): ");
            DateTime uzsakymoData = DateTime.Parse(Console.ReadLine());
            Console.Write("Įveskite kainą už vnt (EUR): ");
            double kainaUzVnt = double.Parse(Console.ReadLine());

            uzsakymai.Add(new Uzsakymas(uzsakymoID, klientoID, preke, kiekis, uzsakymoData, kainaUzVnt));
            Console.WriteLine("Naujas užsakymas pridėtas.");
        }

        private static void IstrintiKlienta()
        {
            Console.Write("Įveskite kliento ID: ");
            int klientoID = int.Parse(Console.ReadLine());

            var klientas = klientai.FirstOrDefault(k => k.KlientoID == klientoID);
            if (klientas != null)
            {
                klientai.Remove(klientas);
                uzsakymai.RemoveAll(u => u.KlientoID == klientoID);
                Console.WriteLine("Klientas ir visi jo užsakymai ištrinti.");
            }
            else
            {
                Console.WriteLine("Klientas nerastas.");
            }
        }

        private static void IstrintiUzsakyma()
        {
            Console.Write("Įveskite užsakymo ID: ");
            int uzsakymoID = int.Parse(Console.ReadLine());

            var uzsakymas = uzsakymai.FirstOrDefault(u => u.UzsakymoID == uzsakymoID);
            if (uzsakymas != null)
            {
                uzsakymai.Remove(uzsakymas);
                Console.WriteLine("Užsakymas ištrintas.");
            }
            else
            {
                Console.WriteLine("Užsakymas nerastas.");
            }
        }

        private static void IšsaugotiPakeitimus()
        {
            using (var writer = new StreamWriter("atnaujinti_duomenys.txt"))
            {

                foreach (var klientas in klientai)
                {
                    writer.WriteLine($"{klientas.KlientoID},{klientas.Vardas},{klientas.Pavarde},{klientas.TelefonoNumeris},{klientas.ElPastas}");
                }

                
                foreach (var uzsakymas in uzsakymai)
                {
                    writer.WriteLine($"{uzsakymas.UzsakymoID},{uzsakymas.KlientoID},{uzsakymas.Preke},{uzsakymas.Kiekis},{uzsakymas.UzsakymoData:yyyy-MM-dd},{uzsakymas.KainaUzVnt}");
                }
            }
            Console.WriteLine("Visi pakeitimai išsaugoti į failą 'atnaujinti_duomenys.txt'.");
        }

    }
}

