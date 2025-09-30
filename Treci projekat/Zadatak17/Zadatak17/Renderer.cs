using System;
using System.Collections.Generic;
using System.Linq;

namespace Zadatak17
{
    public sealed class Renderer
    {
        public void Banner()
        {
            Console.WriteLine("=== CocktailsRx – Reaktivna pretraga TheCocktailDB ===");
            Console.WriteLine("Upisi ime koktela (npr. margarita, mojito, negroni). 'exit' za izlaz.");
        }

        public void ShowDrinks(string query, IReadOnlyList<Drink> drinks)
        {
            Console.WriteLine($"\n--- Rezultati za \"{query}\" (pronadjeno: {drinks.Count}) ---");
            if (drinks.Count == 0)
            {
                Console.WriteLine("Nema rezultata.");
                return;
            }

            for (int i = 0; i < drinks.Count; i++)
            {
                Drink d = drinks[i];
                Console.WriteLine("\n• " + d.strDrink);
                if (!string.IsNullOrWhiteSpace(d.strInstructions))
                {
                    Console.WriteLine("  " + d.strInstructions);
                }
            }
        }

        public void ShowTopWords(IEnumerable<KeyValuePair<string, int>> freq, int top = 10)
        {
            List<KeyValuePair<string, int>> topList = freq.Take(top).ToList();

            if (topList.Count == 0)
            {
                Console.WriteLine("\n(Nema reci za word cloud)");
                return;
            }

            Console.WriteLine("\n--- Najcesce reci (Top {0}) ---", topList.Count);
            for (int i = 0; i < topList.Count; i++)
            {
                KeyValuePair<string, int> kv = topList[i];
                Console.WriteLine(kv.Key + " - " + kv.Value + " puta");
            }
        }

        public void Prompt()
        {
            Console.Write("\nUnesi ime koktela: ");
        }

        public void Goodbye()
        {
            Console.WriteLine("\nPozdrav");
        }

        public void Error(string msg)
        {
            Console.WriteLine("Greska: " + msg);
        }
    }
}
