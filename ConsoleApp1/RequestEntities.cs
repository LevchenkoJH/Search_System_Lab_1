using search_engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class RequestEntities
    {
        internal RequestToCatalog RequestToCatalog
        {
            get => default;
            set
            {
            }
        }

        public static List<Term> RequestObjects(List<string> parts, List<Term> terms)
        {
            // Сбор структур для слов
            List<Term> requestStatistic = new List<Term>();
            Term blancTerm = new Term();

            foreach (string word in parts)
            {
                int index = terms.FindIndex(i => i.name == word);
                if (index == -1)
                {
                    // Если встретился оператор
                    blancTerm.name = word;
                    requestStatistic.Add(blancTerm);
                    blancTerm = new Term();
                }
                else
                {
                    requestStatistic.Add(terms[index]);
                }
            }

            return requestStatistic;
        }
    }
}
