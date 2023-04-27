using search_engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class RequestEntities
    {
        

        public static List<Term> RequestObjects(List<string> parts, List<Term> terms)
        {
            // Сбор структур для слов
            List<Term> requestStatistic = new List<Term>();
            Term blancTerm = new Term();
            Stemmer stemmer = new Stemmer();
            foreach (string word in parts)
            {
                //Console.WriteLine(word);
                string wordStem = "";
                if (Regex.IsMatch(word, "^[a-zA-Z0-9]*$"))
                {
                    //Console.WriteLine("word = ", word);
                    //wordStem = stemmer.Stem(word);
                    wordStem = word;
                }
                else
                {
                    wordStem = stemmer.Stem(word);
                    
                }
                //string wordStem = word;//stemmer.Stem(word);

                int index = terms.FindIndex(i => i.name == wordStem);
                if (index == -1)
                {
                    // Если встретился оператор
                    blancTerm.name = wordStem;
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
