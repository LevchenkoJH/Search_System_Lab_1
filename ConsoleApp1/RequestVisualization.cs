using search_engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class RequestVisualization
    {
        public static void Visualization(List<Term> requestWithOr)
        {
            foreach (Term rq in requestWithOr)
            {
                Console.WriteLine(rq.name);
                Console.WriteLine("Встречаемость " + rq.count);
                foreach (int id in rq.vectorIdDocuments)
                {
                    Console.WriteLine("Документ " + id);
                    int index = rq.docs.FindIndex(i => i.id == id);
                    if (index != -1)
                    {
                        Console.WriteLine("Адрес в тексте");
                        Console.WriteLine(string.Join(",", rq.docs[index].pos));
                    }

                    Console.WriteLine("\n");
                }
                Console.WriteLine("\n");
            }
        }
    }
}
