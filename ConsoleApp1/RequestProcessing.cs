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
        public const string OPER_AND = "AND";
        public const string OPER_OR = "OR";
        public const string OPER_NOT = "NOT";

        public static bool IsOperation(string word)
        {
            if (word == OPER_OR || word == OPER_AND || word == OPER_NOT)
            {
                return true;
            }
            return false;
        }

        public static List<Document> OperationNot(string term, List<Term> terms)
        {
            List<Document> result = new List<Document>();
            List<List<Document>> list = terms.Where(i => i.Name != term).Select(i => i.Documents).ToList();

            foreach (var li in list)
            {
                result.AddRange(li);
            }
            return result;
        }

        public static List<Document> OperationAnd(List<Document> list1, List<Document> list2)
        {
            List <Document> result = new List<Document>();
            foreach (var li1 in list1)
            {
                if (list2.Where(i => i.FileId == li1.FileId).Count() != 0)
                {
                    var li2 = list2.Where(i => i.FileId == li1.FileId).FirstOrDefault();

                    var buf = new Document()
                    {
                        FileId = li1.FileId,
                        Frequency = li1.Frequency + li2.Frequency,
                        Positions= li1.Positions,
                    };

                    buf.Positions.AddRange(li2.Positions);

                    result.Add(buf);
                }
            }
            return result;
        }

        public static List<Document> OperationOr(List<Document> list1, List<Document> list2)
        {
            List<Document> result = new List<Document>();
            foreach (var li1 in list1)
            {
                if (list2.Where(i => i.FileId == li1.FileId).Count() != 0)
                {
                    var li2 = list2.Where(i => i.FileId == li1.FileId).FirstOrDefault();
                    var buf = new Document()
                    {
                        FileId = li1.FileId,
                        Frequency = li1.Frequency + li2.Frequency,
                        Positions = li1.Positions,
                    };

                    buf.Positions.AddRange(li2.Positions);
                    buf.Positions = li1.Positions.Distinct().ToList();

                    result.Add(buf);
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
