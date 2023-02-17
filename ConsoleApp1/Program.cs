using System.Collections.Generic;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace search_engine
{
    internal struct Document
    {
        public int id;
        public int frenq;
        public List<int> pos;

        public Document()
        {
            pos = new List<int>();
            frenq = 0;
            id = 0;
        }
    }

    internal struct Term
    {
        public string name;
        public int count;
        public List<Document> doc;

        public Term()
        {
            name = "";
            count = 0;
            doc = new List<Document>();
        }
    }

    /*internal enum State
    {
        Word,
        Symbol
    }*/


    internal class Program
    {
        static public List<Term> terms = new List<Term>();

        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "text_documents\\";
            //State state = State.Word;
            Term blancTerm = new Term();
            Document blancDoc = new Document();
            //string[] allfiles = Directory.GetFiles(path);
            int documentId = 1;
            using (StreamReader sr = File.OpenText(path + documentId.ToString() + ".txt"))
            {

                int position = 0; // Позиция в документе
                while (sr.Peek() != -1)
                {
                    //Term blancTerm = new Term();
                    //Document blancDoc = new Document();
                    char letter = (char)sr.Read();
                    if (char.IsLetter((char)letter)) // Собираем слово по буквам
                    {
                        blancTerm.name += letter.ToString();
                    }
                    else // Если символ это не буква
                    {
                        if (blancTerm.name != "")
                        {
                            bool found = false;
                            foreach (var term in terms)
                            {
                                if (blancTerm.name == term.name)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            
                            if (!found) // Если слова ещё не было, то добавляем его в список как новое, а также заводим под неё структуру из документов
                            {
                                blancDoc.id = (int)documentId; // id документа это его имя (в данный момент)
                                blancDoc.frenq++; // Добавление числа вхождений слова в документ
                                blancDoc.pos.Add(position - (blancTerm.name.Length)); // Добавление позиции вхождения слова в текущий документ

                                blancTerm.doc.Add(blancDoc); // Добавление информации о документе в сущность текущего слова
                                blancTerm.count++; // Увеличение числа вхождений слова вообще в какой либо документ

                                terms.Add(blancTerm);

                                // Обнуление всех вспомогательных объектов:
                                blancDoc = new Document();
                                blancTerm = new Term();
                            }
                            else // Если слово уже встречалось, то нужно просто найти текущий документ в списке, добавить туда индекс встречи и число раз
                            {
                                int index = terms.FindIndex(i => i.name == blancTerm.name);
                                
                                Term dummy = terms[index];

                                Document dummyDoc = dummy.doc[documentId-1]; // Текущий документ для данного слова
                                dummyDoc.pos.Add(position - (blancTerm.name.Length)); // Добавление новой позиции для данного слова в текущем документе
                                dummyDoc.frenq++; // Увеличение числа вхождений данного слова в текущий документ
                                dummy.doc[documentId-1] = dummyDoc; // Изменение List с документом данного слова из-за особенностей языка 

                                dummy.count++; // Увеличение числа вхождений слова вообще в какой либо документ
                                terms[index] = dummy;

                                // Обнуление всех вспомогательных структур:
                                blancTerm = new Term(); 
                                blancDoc = new Document();
                            }
                        }
                        
                    }
                    position++;
                    /*switch (state)
                    {
                        case State.Word:
                        {
                            if (char.IsLetter((char)sr.Peek()))
                            {
                                blanc.name += sr.Peek().ToString();
                                break;
                            }
                            else
                            {
                                state = State.Symbol;
                                terms.Add(blanc);
                                blanc = new Term();
                                break;
                            }
                        }
                            

                        case State.Symbol:
                            if (char.IsLetter((char)sr.Peek()))
                            {
                                state
                                break;
                            }
                            else
                            {
                                state = State.Word;
                                break;
                            }
                    }*/
                }
            }
            foreach(Term term in terms)
            {
                Console.WriteLine(term.name.ToString() + " " + term.doc[0].pos[0].ToString() + "\n");
            }
            Console.WriteLine(path);

        }
    }
}