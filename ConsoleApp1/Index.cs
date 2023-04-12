using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;

namespace SearchSystem
{
    public class Index
    {
        /// <summary>
        /// Термины индекса
        /// </summary>
        private List<Term> Terms = new List<Term>();
        /// <summary>
        /// Файлы к которым относится индекс
        /// </summary>
        private List<File> Files = new List<File>();

        public Index(List<File> files) 
        {
            // Сохраняем в индексе список файлов
            Files = files;
            // Собираем термины
            GetTerms();
        }

        private void GetTerms()
        {
            // Вспомогательный класс сбора терминов
            TermReader termReader = new TermReader();
            try
            {
                for (int i = 0; i < Files.Count(); i++)
                {
                    // Открываем файл
                    var fileReader = new FileReader(Files[i].Name);

                    // Собираем термины из файла
                    termReader.FindTermsInFile(fileReader, Files[i].Id);

                    // Статистика файла (сколько терминов в документе)
                    Files[i].Frequency = termReader.GetFileFrequency(Files[i].Id);

                    // Закрываем файл
                    fileReader.FileClose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            // Вспомогательный класс выдает список найденных терминов
            // Их сохраняем в индексе
            Terms = termReader.GetTerms();
        }

        public List<Document> Search(string search)
        {
            Console.WriteLine($"SEARCH: {search}");

            // Разбиваем запрос на слова
            List<string> words = search.Split().Where(i => i != "").ToList();

            // Очередь терминов (списков документов которые им соответствуют)
            var queue_terms = new List<List<Document>>();
            // Очередь операций (i-ая операция всегда проходит над queue_terms[i] и queue_terms[i + 1])
            var queue_operations = new List<string>();

            // Флаг для отрицания
            bool flag_not = false;
            // Флаг двух подряд идущих термина
            bool flag_double_term = false;
            // Составляем эти очереди
            for (int i = 0; i < words.Count(); i++)
            {
                // Нашли NOT
                if (words[i] == RequestProcessing.OPER_NOT)
                {
                    // Меняем значение флага (следующий термин будет отрицательным)
                    flag_not = true;
                }
                // Нашли термин
                else if (!RequestProcessing.IsOperation(words[i]))
                {
                    // Добавляем в очередь терминов
                    queue_terms.Add(SearchTerm(words[i], flag_not));
                    // Обнуляем флаг отрицательности
                    flag_not = false;

                    // Проверяем идет ли два термина подряд
                    if (flag_double_term)
                        // Тогда добавляем операцию AND в очередь
                        queue_operations.Add(RequestProcessing.OPER_AND);
                    flag_double_term = true;
                    Console.WriteLine(words[i]);
                    Console.WriteLine("Нашли термин");
                    
                }
                // Нашли ОR или AND
                else if (RequestProcessing.IsOperation(words[i]))
                {
                    // Добавляем в очередь операций
                    queue_operations.Add(words[i]);

                    flag_double_term = false;
                    Console.WriteLine(words[i]);
                    Console.WriteLine("Нашли ОR или AND");
                }


                // Для отладки
                else 
                {
                    Console.WriteLine("ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR !!!");
                }
            }


            Console.WriteLine("AND OR---");
            foreach (var test in queue_terms)
            {
                Console.WriteLine(test.Count);
            }

            foreach (var test in queue_operations)
            {
                Console.WriteLine(test);
            }



            // Проходим по очереди операций 
            // Сначала исполняем только AND
            var and_result = new List<List<Document>>();
            for (int i = 0; i < queue_operations.Count; i++)
            {
                if (queue_operations[i] == RequestProcessing.OPER_AND)
                {
                    // Выполняем операцию между i и i + 1
                    var result = RequestProcessing.OperationAnd(queue_terms[i], queue_terms[i + 1]);

                    queue_terms[i] = result;
                    queue_terms[i + 1] = result;
                }
                else
                {
                    and_result.Add(queue_terms[i]);
                }
            }
            and_result.Add(queue_terms[queue_terms.Count - 1]);

            queue_terms = and_result;

            queue_operations = queue_operations.Where(i => i != RequestProcessing.OPER_AND).ToList();

            Console.WriteLine("OR----");
            foreach (var test in queue_terms)
            {
                Console.WriteLine(test.Count);
            }

            foreach (var test in queue_operations)
            {
                Console.WriteLine(test);
            }



            // Проходим по очереди операций 
            // Теперь исполняем только OR
            for (int i = 0; i < queue_operations.Count; i++)
            {
                if (queue_operations[i] == RequestProcessing.OPER_OR)
                {
                    // Выполняем операцию между i и i + 1
                    var result = RequestProcessing.OperationOr(queue_terms[i], queue_terms[i + 1]);

                    queue_terms[i] = result;
                    queue_terms[i + 1] = result;
                }
                else
                {
                    Console.WriteLine("ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR !!!");
                }
            }

            Console.WriteLine("----");
            foreach (var test in queue_terms)
            {
                Console.WriteLine(test.Count);
            }

            return queue_terms[queue_terms.Count - 1];
        }

        private List<Document> SearchTerm(string term, bool isNot)
        {
            // Сначала находим нужный термин
            if (!isNot)
            {
                Term? _term = Terms.Where(i => i.Name == term).FirstOrDefault();

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
            List<List<Document>> list = Terms.Where(i => i.Name != term).Select(i => i.Documents).ToList();

            foreach (var li in list)
            {
                result.AddRange(li);
            }
            return result;
        }

        public string GetFileName(Guid fileId)
        {
            File? file = Files.Where(i => i.Id == fileId).FirstOrDefault();
            if (file != null)
                return file.Name;
            else
                return "";
        }


        public void PrintFileStatistics()
        {
            Console.WriteLine("-----------------------PrintFileStatistics()-----------------------");
            Console.WriteLine(Files.Count);
            foreach (File file in Files)
            {
                Console.WriteLine($"{file.Id} ** Frequency: {file.Frequency}");
                Console.WriteLine();
            }
        }

    }
}