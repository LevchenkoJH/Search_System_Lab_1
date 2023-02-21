using search_engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class RequestToCatalog
    {
        public List<Term> terms = new List<Term>();

        /*public void RequestCatalog(string request)
        {
            string[] parts = request.Split(" ");
            foreach (string part in parts)
            {
                if (part != "and" && part != "or")
                {
                    int index = this.terms.FindIndex(i => i.name == this.blancTerm.name); // Индекс сущности данного слова в списке
                    Term dummy = this.terms[index]; // Копия сущности слова
                    
                }   
            }
        }*/

        public void Request(string request)
        {

        }

        public RequestToCatalog(List<Term> terms)
        {
            this.terms = terms;
        }
    }
}
