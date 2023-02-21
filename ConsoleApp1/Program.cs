using ConsoleApp1;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace search_engine
{
    public struct Document
    {
        public int id;
        public int frenq;
        public List<int> pos;

        public Document()
        {
            this.pos = new List<int>();
            this.frenq = 0;
            this.id = 0;
        }
    }

    public struct Term
    {
        public string name;
        public int count;
        public List<Document> doc;

        public Term()
        {
            this.name = "";
            this.count = 0;
            this.doc = new List<Document>();
        }
    }

    public class Catalog
    {
        public List<Term> terms = new List<Term>();
        public Term blancTerm = new Term();
        public Document blancDoc = new Document();


        

        private bool WordPresence(string word)
        {
            // Проверка на наличие слова в словаре
            foreach (var term in this.terms)
            {
                if (term.name == word)
                {
                    return true;
                }
            }
            return false;
        }

        private void ZeroStruct()
        {
            // Обнуление вспомогательных структур
            this.blancDoc = new Document();
            this.blancTerm = new Term();
        }

        private void AddNewDoc(int documentId, int position)
        {
            // Создание нового документа

            this.blancDoc.id = (int)documentId; // id документа это его имя (в данный момент)
            this.blancDoc.frenq++; // Добавление числа вхождений слова в документ
            this.blancDoc.pos.Add(position - (this.blancTerm.name.Length)); // Добавление позиции вхождения слова в текущий документ
        }

        private void AddNewWord(int documentId, int position)
        {
            // Добавление нового слова в список структур

            this.AddNewDoc(documentId, position); //Создание нового документа

            this.blancTerm.doc.Add(this.blancDoc); // Добавление информации о документе в сущность текущего слова
            this.blancTerm.count++; // Увеличение числа вхождений слова вообще в какой либо документ

            this.terms.Add(this.blancTerm);

            this.ZeroStruct(); // Обнуление всех вспомогательных структур
        }

        private void DataCollection(string file, string path)
        {
            // Сбор данных из текущего файла открытого в потоке sr
            int documentId = int.Parse(Path.GetFileName(file)[0].ToString());
            using (StreamReader sr = File.OpenText(path + documentId.ToString() + ".txt"))
            {
                int position = 0;

                while (sr.Peek() != -1)
                {
                    char letter = (char)sr.Read();
                    if (char.IsLetter((char)letter)) // Собираем слово по буквам
                    {
                        this.blancTerm.name += letter.ToString();
                    }
                    else
                    {
                        if (this.blancTerm.name != "")
                        {
                            bool found = this.WordPresence(this.blancTerm.name);
                            if (!found)
                            {
                                // Если слово не найдено, то добавляем его
                                this.AddNewWord(documentId, position);
                                this.ZeroStruct();
                            }
                            else
                            {
                                int index = this.terms.FindIndex(i => i.name == this.blancTerm.name); // Индекс сущности данного слова в списке
                                Term dummy = this.terms[index]; // Копия сущности слова
                                int indexDoc = dummy.doc.FindIndex(i => i.id == (int)documentId); // Индекс текущего документа

                                if (indexDoc == -1) // Если слово встречалось, но не в текущем документе
                                {
                                    this.AddNewDoc(documentId, position); // Создание нового документа
                                    dummy.doc.Add(this.blancDoc);
                                }
                                else // Если слово встречалось в текущем документе
                                {
                                    Document dummyDoc = dummy.doc[indexDoc]; // Текущий документ для данного слова
                                    dummyDoc.pos.Add(position - (blancTerm.name.Length)); // Добавление новой позиции для данного слова в текущем документе
                                    dummyDoc.frenq++; // Увеличение числа вхождений данного слова в текущий документ

                                    dummy.doc[indexDoc] = dummyDoc; // Изменение List с документом данного слова из-за особенностей языка 
                                }
                                dummy.count++; // Увеличение числа вхождений слова вообще в какой либо документ
                                terms[index] = dummy;
                                
                                this.ZeroStruct(); // Обнуление всех вспомогательных структур
                            }
                        }
                    }
                    position++;
                }
            }
        }

        public Catalog(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                this.DataCollection(file, path); // Сбор данных из текущего файла
            }
        }        
    }

    internal class Program
    {
        

        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "text_documents\\";
            Catalog catalog = new Catalog(path);
            RequestToCatalog rtk = new RequestToCatalog(catalog.terms);
            foreach(Term term in rtk.terms)
            {
                Console.WriteLine(term.name);
                foreach(Document document in term.doc)
                {
                    Console.WriteLine(document.id);
                }
                Console.WriteLine("\n");
            }            
        }
    }
}