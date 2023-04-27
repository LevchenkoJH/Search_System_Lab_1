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
        public List<Document> docs;
        public List<int> vectorIdDocuments;
        public Term()
        {
            this.name = "";
            this.count = 0;
            this.docs = new List<Document>();
            this.vectorIdDocuments = new List<int>();
        }
    }

    public class Index
    {
        public List<Term> terms = new List<Term>();
        public Term blancTerm = new Term();
        public Document blancDoc = new Document();
        Object locker = new();
        

        public List<int> AllDocuments() // Получение List с id всех документов
        {
            List<int> allDocuments = new List<int>();
            foreach (Term term in this.terms)
            {
                List<int> docsId = new List<int>();
                foreach (Document document in term.docs)
                {
                    docsId.Add(document.id);
                }
                allDocuments = Enumerable.Union(allDocuments, docsId).ToList();
            }
            return allDocuments;
        }

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

            this.blancTerm.docs.Add(this.blancDoc); // Добавление информации о документе в сущность текущего слова
            this.blancTerm.count++; // Увеличение числа вхождений слова вообще в какой либо документ
            this.blancTerm.vectorIdDocuments.Add(this.blancDoc.id);

            this.terms.Add(this.blancTerm);
            
            this.ZeroStruct(); // Обнуление всех вспомогательных структур
        }

        private void DataCollection(string file, string path)
        {
            // Сбор данных из текущего файла открытого в потоке sr
            int documentId = int.Parse(Path.GetFileName(file)[0].ToString());
            //SimpleStemmer stemmer = new SimpleStemmer();
            Stemmer stemmer = new Stemmer();
            using (StreamReader sr = File.OpenText(path + documentId.ToString() + ".txt"))
            {
                int position = 0;

                while (sr.Peek() != -1)
                {
               
                        char letter = ' ';
                        lock (locker)
                        {
                            letter = (char)sr.Read();
                        }

                        if (char.IsLetter((char)letter)) // Собираем слово по буквам
                        {
                            lock (locker)
                            {
                                this.blancTerm.name += letter.ToString();
                            }
                        }
                        else
                        {
                            string dummyName = "";
                            lock(locker)
                            {
                                dummyName = this.blancTerm.name;
                            }

                            if (dummyName != "")//this.blancTerm.name != "")
                            {
                                //Console.WriteLine(dummyName);
                                dummyName = stemmer.Stem(dummyName);//this.blancTerm.name);
                                //dummyName = this.blancTerm.name;
                                lock (locker)
                                {
                                    this.blancTerm.name = dummyName;
                                    bool found = this.WordPresence(this.blancTerm.name);  // this.blancTerm.name);
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
                                        int indexDoc = dummy.docs.FindIndex(i => i.id == (int)documentId); // Индекс текущего документа

                                        if (indexDoc == -1) // Если слово встречалось, но не в текущем документе
                                        {
                                            this.AddNewDoc(documentId, position); // Создание нового документа
                                            dummy.docs.Add(this.blancDoc);
                                            dummy.vectorIdDocuments.Add(documentId);
                                        }
                                        else // Если слово встречалось в текущем документе
                                        {
                                            Document dummyDoc = dummy.docs[indexDoc]; // Текущий документ для данного слова
                                            dummyDoc.pos.Add(position - (blancTerm.name.Length)); // Добавление новой позиции для данного слова в текущем документе
                                            dummyDoc.frenq++; // Увеличение числа вхождений данного слова в текущий документ

                                            dummy.docs[indexDoc] = dummyDoc; // Изменение List с документом данного слова из-за особенностей языка C#
                                        }

                                        dummy.count++; // Увеличение числа вхождений слова вообще в какой либо документ

                                        terms[index] = dummy;

                                        this.ZeroStruct(); // Обнуление всех вспомогательных структур

                                    }
                                }
                            }

                        
                    }
                    position++;
                }
            }
        }

        public Index(string path)
        {
            string[] files = Directory.GetFiles(path);
            /*foreach (string file in files)
            {
                this.DataCollection(file, path); // Сбор данных из текущего файла
            }*/
            Parallel.ForEach(files, file =>
            {
                
                this.DataCollection(file, path);
                
            });
        }        
    }

    internal class SearchMachine
    {
        
        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "text_documents\\";
            Index catalog = new Index(path);

            string request = "как and дела or not вас";

            List<string> parts = StringHandler.HandlerString(request);

            List<Term> requestWithStatistic = RequestEntities.RequestObjects(parts, catalog.terms);
            List<int> allDocuments = catalog.AllDocuments();

            RequestToCatalog rtk = new RequestToCatalog(requestWithStatistic, allDocuments);
            List<Term> result = rtk.Request();

            RequestVisualization.Visualization(result);

            
            //PorterStemmer stemmer = new PorterStemmer();

            /*Stemmer stemmer = new Stemmer();
            string word = "";

            Console.WriteLine(stemmer.Stem(word));*/


            //RequestToCatalog rtk = new RequestToCatalog(catalog.terms, allDocuments);

            /*foreach(Term term in rtk.terms)
            {
                Console.WriteLine(term.name);
                foreach(Document document in term.docs)
                {
                    Console.WriteLine(document.id);
                }
                Console.WriteLine("\n");
            }*/
            //rtk.Request("как and дела or not вас");
        }
    }
}
