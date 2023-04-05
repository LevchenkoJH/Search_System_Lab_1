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

        private Dictionary<Guid, string> FileNames;

        public RequestProcessing(Dictionary<Guid, string> fileNames) 
        {
            FileNames = fileNames;
        }

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

            List<string> stackTerms = new List<string>();
            List<List<Document>> stackDocuments = new List<List<Document>>();
            List<string> stackOperations = new List<string>();

            int index = 0;
            for (int i = 0; i < words.Count(); i++)
            {
                //Term + Term
                if (i + 1 < words.Count() && !IsOperation(words[i]) && !IsOperation(words[i + 1]))
                {
                    stackTerms.Add(words[i]);
                    stackDocuments.Add(SearchTerm(words[i], stackNots[index]));
                    stackOperations.Add(OPER_AND);
                    index++;
                }
                else if (!IsOperation(words[i]))
                {
                    stackTerms.Add(words[i]);
                    stackDocuments.Add(SearchTerm(words[i], stackNots[index]));
                    index++;
                }
                else
                {
                    stackOperations.Add(words[i]);
                }
            }

            //List<List<Document>> _stackDocuments = new List<List<Document>>();
            List<string> _stackOperations = new List<string>();
            // Сначала выполняем AND
            for (int i = 0; i < stackOperations.Count(); i++)
            {
                if (stackOperations[i] == OPER_AND)
                {
                    // Выполняем операцию между i и i + 1
                    var result = OperationAnd(stackDocuments[i], stackDocuments[i + 1]);

                    stackDocuments[i] = result;
                    stackDocuments[i + 1] = result;
                }
                else
                {
                    _stackOperations.Add(stackOperations[i]);
                }
            }

            stackDocuments = stackDocuments.Distinct().ToList();
            stackOperations = _stackOperations;

            // Теперь выполняем OR
            for (int i = 0; i < stackOperations.Count(); i++)
            {
                if (stackOperations[i] == OPER_OR)
                {
                    // Выполняем операцию между i и i + 1
                    var result = i + 1 < stackDocuments.Count() ? OperationOr(stackDocuments[i], stackDocuments[i + 1]) : OperationOr(stackDocuments[i], stackDocuments[i]);

                    stackDocuments[i] = result;
                    if (i + 1 < stackDocuments.Count())
                        stackDocuments[i + 1] = result;
                }
            }

            stackDocuments = stackDocuments.Distinct().ToList();

            foreach (Document document in stackDocuments[0])
            {
                Console.WriteLine($"File: {FileNames[document.FileId]}\nFrequency: {document.Frequency}");
                Console.Write("Position:");
                foreach(int position in document.Positions)
                {
                    Console.Write($"{position}, ");
                }
                Console.Write("\n");
            }

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

        private List<Document> SearchTerm(string term, bool isNot)
        {
            // Сначала находим нужный термин
            if (!isNot)
            {
                Term? _term = TermReader.GetTerms().Where(i => i.Name == term).FirstOrDefault();

                if (_term != null)
                    return _term.Documents;
                else
                    return new List<Document>();
            }
            else
            {
                return OperationNot(term);
            }
        }

        private List<Document> OperationNot(string term)
        {
            List<Document> result = new List<Document>();
            List<List<Document>> list = TermReader.GetTerms().Where(i => i.Name != term).Select(i => i.Documents).ToList();

            foreach (var li in list)
            {
                result.AddRange(li);
            }
            return result;
        }

        private List<Document> OperationAnd(List<Document> list1, List<Document> list2)
        {
            List <Document> result = new List<Document>();
            foreach (var li1 in list1)
            {
                if (list2.Where(i => i.FileId == li1.FileId).Count() != 0)
                {

                    var li2 = list2.Where(i => i.FileId == li1.FileId).FirstOrDefault();

                    li1.Frequency += li2.Frequency;
                    li1.Positions.AddRange(li2.Positions);
                    li1.Positions = li1.Positions.Distinct().ToList();

                    result.Add(li1);
                }
            }
            return result;
        }

        private List<Document> OperationOr(List<Document> list1, List<Document> list2)
        {
            List<Document> result = new List<Document>();
            foreach (var li1 in list1)
            {
                if (list2.Where(i => i.FileId == li1.FileId).Count() != 0)
                {

                    var li2 = list2.Where(i => i.FileId == li1.FileId).FirstOrDefault();

                    li1.Frequency += li2.Frequency;
                    li1.Positions.AddRange(li2.Positions);
                    li1.Positions = li1.Positions.Distinct().ToList();

                    result.Add(li1);
                }
                else
                {
                    result.Add(li1);
                }
            }

            foreach (var li2 in list2)
            {
                if (list1.Where(i => i.FileId == li2.FileId).Count() == 0)
                {
                    result.Add(li2);
                }
            }
            return result;
        }
    }
}
