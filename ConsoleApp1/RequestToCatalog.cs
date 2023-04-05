using search_engine;

namespace ConsoleApp1
{
    internal class RequestToCatalog
    {
        private List<Term> terms = new List<Term>();
        private List<int> allDocuments = new List<int>();


        private List<Term> RequestObjects(List<string> parts)
        {
            // Сбор структур для слов
            List<Term> requestStatistic = new List<Term>();
            Term blancTerm = new Term();

            foreach (string word in parts)
            {
                int index = this.terms.FindIndex(i => i.name == word);
                if (index == -1)
                {
                    // Если встретился оператор
                    blancTerm.name = word;
                    requestStatistic.Add(blancTerm);
                    blancTerm = new Term();
                }
                else
                {
                    requestStatistic.Add(this.terms[index]);
                }
            }

            /*foreach(Term rq in requestStatistic)
            {
                Console.WriteLine(rq.name);
                foreach(int i in rq.vectorIdDocuments)
                {
                    Console.Write(i);
                }
                Console.WriteLine("\n");
            }*/
            return requestStatistic;
        }

        private List<string> HandlerString(string request) // Добавление and в строку запроса вместо пробелов между словами
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

        private List<int> AllDocuments() // Получение List с id всех документов
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

        private List<Term> NotHandler(List<Term> requestWithStatistic)
        {
            List<Term> requestWithNot = requestWithStatistic;
            Term blancTerm = new Term();
            for (int i = 0; i < requestWithNot.Count; i++)
            {
                if (requestWithNot[i].name == "not")
                {
                    blancTerm.vectorIdDocuments = Enumerable.Except(this.allDocuments, requestWithNot[i + 1].vectorIdDocuments).ToList();
                    blancTerm.docs = new List<Document>(); // Так как в этих документах слово не встречалось, то и смысла хранить позиции нет
                    blancTerm.name = requestWithNot[i + 1].name;
                    blancTerm.count = requestWithNot[i + 1].count;


                    requestWithNot[i + 1] = blancTerm;
                    requestWithNot.RemoveAt(i);
                }
            }

            /*foreach (Term rq in requestWithNot)
            {
                Console.WriteLine(rq.name);
                foreach (int i in rq.vectorIdDocuments)
                {
                    Console.Write(i);
                }
                Console.WriteLine("\n");
            }*/
            return requestWithNot;
        }


        private List<Term> AndHandler(List<Term> requestWithNot)
        {
            List<Term> requestWithAnd = requestWithNot;
            Term blancTerm = new Term();
            for (int i = 0; i < requestWithAnd.Count; i++)
            {
                if (requestWithAnd[i].name == "and")
                {
                    blancTerm.docs = requestWithAnd[i - 1].docs; // можно будет выводить нужные согласно vectorIdDocuments
                    blancTerm.name = requestWithAnd[i - 1].name;
                    blancTerm.vectorIdDocuments = Enumerable.Intersect(requestWithAnd[i - 1].vectorIdDocuments, requestWithAnd[i + 1].vectorIdDocuments).ToList();
                    blancTerm.count = blancTerm.vectorIdDocuments.Count;
                    requestWithAnd[i - 1] = blancTerm;

                    blancTerm.docs = requestWithAnd[i + 1].docs;
                    blancTerm.name = requestWithAnd[i + 1].name; // остальные поля будут одинаковые
                    requestWithAnd[i + 1] = blancTerm;

                    requestWithAnd.RemoveAt(i); // Удаление and
                    blancTerm = new Term();
                }
            }

            /*foreach (Term rq in requestWithAnd)
            {
                Console.WriteLine(rq.name);
                foreach (int i in rq.vectorIdDocuments)
                {
                    Console.Write("документ " + i + " ");
                }
                Console.WriteLine("\n");
            }*/

            return requestWithAnd;
        }

        /*private List<Term> OrHandler(List<Term> requestWithAnd)
        {
            List<Term> requestWithOr = requestWithAnd;
            Term blancTerm = new Term();
            for (int i = 0; i < requestWithOr.Count; i++)
            {
                if (requestWithOr[i].name == "or")
                {
                    blancTerm.docs = requestWithOr[i - 1].docs;
                    blancTerm.name = requestWithOr[i - 1].name;
                    blancTerm.count = requestWithOr[i - 1].count;
                    blancTerm.vectorIdDocuments = Enumerable.Union(requestWithOr[i - 1].vectorIdDocuments, requestWithOr[i + 1].vectorIdDocuments).ToList();
                    requestWithOr[i - 1] = blancTerm;

                    requestWithOr.RemoveAt(i);
                    requestWithOr.RemoveAt(i);

                    blancTerm = new Term();
                }
            }

            foreach (Term rq in requestWithOr)
            {
                Console.WriteLine(rq.name);
                foreach (int i in rq.vectorIdDocuments)
                {
                    Console.Write(i);
                }
                Console.WriteLine("\n");
            }

            return requestWithOr;
        }*/

        private void RequestVisualization(List<Term> requestWithAnd)
        {
            foreach (Term term in requestWithAnd)
            {
                if (term.name != "or")
                {
                    Console.WriteLine(term.name);
                    Console.WriteLine("Встречаемость " + term.count);
                    foreach(int id in term.vectorIdDocuments)
                    {
                        Console.WriteLine("Документ " + id);
                        int index = term.docs.FindIndex(i => i.id == id);
                        if (index != -1)
                        {
                            Console.WriteLine("Адрес в тексте");
                            Console.WriteLine(string.Join(",", term.docs[index].pos));
                        }

                        Console.WriteLine("\n");
                    }
                    Console.WriteLine("\n");
                }
            }
        }

        public void Request(string request)
        {
            List<string> parts = this.HandlerString(request); // Добавление в строку and вместо пробелов между словами
            List<Term> requestWithStatistic = this.RequestObjects(parts); // Сбор структур для слов
            List<Term> requestWithNot = this.NotHandler(requestWithStatistic); // Обработка not
            List<Term> requestWithAnd = this.AndHandler(requestWithNot); // Обработка and
            //List<Term> requestWithOr = this.OrHandler(requestWithAnd);
            RequestVisualization(requestWithAnd);
            //Console.WriteLine(string.Join(",", this.allDocuments));

            //List<string> operators = new List<string>();

            /*foreach (string part in parts)
            {
                if (part != "and" && part != "or")
                {
                    int index = this.terms.FindIndex(i => i.name == part);
                    Term dummy = this.terms[index];
                    blancWord.word = part;
                    foreach(Document document in dummy.docs)
                    {
                        blancWord.docs.Add(document.id);
                    }
                    words.Add(blancWord);
                    blancWord = new QueryWord();
                }
                else
                {
                    blancWord.word = part;
                    words.Add(blancWord);
                    blancWord = new QueryWord();
                }
            }*/
            
            /*foreach(QueryWord word in words)
            {
                Console.WriteLine(word.word);
                foreach(int i in word.docs)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("\n");
            }*/
        }

        public RequestToCatalog(List<Term> terms)
        {
            this.terms = terms; // Получение всего инвертированного индекса (полного каталога)
            this.allDocuments = AllDocuments(); // Получение полного списка id документов в каталоге
        }
    }
}
