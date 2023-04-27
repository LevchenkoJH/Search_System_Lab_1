using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class StringHandler
    {
        

        public static List<string> HandlerString(string request) // Добавление and в строку запроса вместо пробелов между словами
        {
            List<string> partsRequest = request.Split(" ").ToList<string>();
            for (int i = 0; i < partsRequest.Count() - 1; i++)
            {
                if (partsRequest[i] != "and" && partsRequest[i] != "or" && partsRequest[i] != "not" && partsRequest[i + 1] != "and" && partsRequest[i + 1] != "or" && partsRequest[i + 1] != "not")
                {
                    partsRequest.Insert(i + 1, "and");
                }
            }
            return partsRequest;
        }
    }
}
