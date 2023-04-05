using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SearchSystem
{
    internal class RequestProcessing
    {
        private const char COMAND_SYMBOL = '!';

        private const string OPER_AND = "AND";
        private const string OPER_OR = "OR";
        private const string OPER_NOT = "NOT";




        public RequestProcessing() { }




        









        public bool Request(string request)
        {
            if (request.Length == 0)
                return false;

            if (request[0] == COMAND_SYMBOL)
            {
                return CommandProcessing(request.Remove(0, 1));
            }
            else
            {
                return SearchProcessing(request);
            }
        }

        private bool CommandProcessing(string comand)
        {
            Console.WriteLine($"COMAND: {comand}");
            switch (comand)
            {
                case "exit":
                    return true;

                default:
                    return false;
            }
        }



        private bool SearchProcessing(string search)
        {
            Console.WriteLine($"SEARCH: {search}");

            // Разбиваем на слова
            List<string> words = search.Split().Where(i => i != "").ToList();

            List<string> _words = new List<string>();
            List<bool> stackNots = new List<bool>();
            for (int i = 0; i < words.Count(); i++)
            {
                // NOT + term
                if (i - 1 >= 0 && words[i - 1] == OPER_NOT && !IsOperation(words[i]))
                {
                    _words.Add(words[i]);
                    stackNots.Add(true);
                }
                else if (words[i] != OPER_NOT)
                {
                    _words.Add(words[i]);
                    if (!IsOperation(words[i]))
                        stackNots.Add(false);
                }
            }

            words = _words;
            Console.WriteLine("-------------");
            foreach (var t in _words)
            {
                Console.WriteLine(t);
            }

            foreach (var t in stackNots)
            {
                Console.WriteLine(t);
            }


            List<string> stackTerms = new List<string>();
            List<List<Document>> stackDocuments = new List<List<Document>>();
            List<string> stackOperations = new List<string>();

            for (int i = 0; i < words.Count(); i++)
            {
                //Term + Term
                if (i + 1 < words.Count() && !IsOperation(words[i]) && !IsOperation(words[i + 1]))
                {
                    stackTerms.Add(words[i]);
                    stackDocuments.Add(SearchTerm(words[i]));
                    stackOperations.Add(OPER_AND);
                }
                else if (!IsOperation(words[i]))
                {
                    stackTerms.Add(words[i]);
                    stackDocuments.Add(SearchTerm(words[i]));
                }
                else
                {
                    stackOperations.Add(words[i]);
                }
            }



            Console.WriteLine("-------------");
            foreach (var t in stackTerms)
            {
                Console.WriteLine(t);
            }

            foreach (var t in stackOperations)
            {
                Console.WriteLine(t);
            }

            foreach (var t in stackDocuments)
            {
                Console.WriteLine($"COUNT stackDocuments{t.Count}");
            }

            Console.WriteLine($"COUNT {stackDocuments.Count}");

            //List<List<Document>> _stackDocuments = new List<List<Document>>();
            // Сначала выполняем AND
            for (int i = 0; i < stackOperations.Count(); i++)
            {
                if (stackOperations[i] == OPER_AND)
                {
                    // Выполняем операцию между i и i + 1
                    var result = OperationAnd(stackDocuments[i], stackDocuments[i + 1]);



                    /*Console.WriteLine($"result {result.Count()}");
                    foreach (var t in result)
                    {
                        Console.WriteLine($"result {t.Positions}");
                        foreach (var g in t.Positions)
                        {
                            Console.WriteLine($"pos {g}");
                        }
                    }*/




                    stackDocuments[i] = result;
                    stackDocuments[i + 1] = result;





                }
            }

            stackDocuments = stackDocuments.Distinct().ToList();
            Console.WriteLine($"COUNT {stackDocuments.Count}");

            //Term? terms = TermReader.GetTerms().Where(i => i.Name == search).FirstOrDefault();


            // Теперь выполняем OR





            return false;
        }

        private bool IsOperation(string word)
        {
            if (word == OPER_OR || word == OPER_AND || word == OPER_NOT)
            {
                return true;
            }
            return false;
        }

        private List<Document> SearchTerm(string term)
        {
            // Сначала находим нужный термин
            Term? _term = TermReader.GetTerms().Where(i => i.Name == term).FirstOrDefault();



            if (_term != null)
                return _term.Documents;
            else
                return new List<Document>();
            

        }

        private List<Document> OperationAnd(List<Document> list1, List<Document> list2)
        {
            List <Document> result = new List<Document>();
            foreach (var li1 in list1)
            {
                if (list2.Where(i => i.FileId == li1.FileId).Count() != 0)
                {

                    var li2 = list2.Where(i => i.FileId == li1.FileId).FirstOrDefault();


                    //Console.WriteLine($"{li1.FileId} {li1.Frequency}");
                    //Console.WriteLine($"{li2.FileId} {li2.Frequency}");

                    li1.Frequency += li2.Frequency;
                    li1.Positions.AddRange(li2.Positions);
                    li1.Positions = li1.Positions.Distinct().ToList();

                    result.Add(li1);
                }
            }
            return result;
        }


        

    }
}
