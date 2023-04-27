﻿using System;
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

        private TreeForIndex treeIndex = new TreeForIndex();

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



            treeIndex = termReader.GetTree();












        }

        public List<Document> Search(string search)
        {
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
                    ////////////////////////////////////////////////////////////////////////////////////////////
                    // Добавляем в очередь терминов
                    queue_terms.Add(SearchTerm(words[i], flag_not));
                    ////////////////////////////////////////////////////////////////////////////////////////////
                    // Обнуляем флаг отрицательности
                    flag_not = false;

                    // Проверяем идет ли два термина подряд
                    if (flag_double_term)
                        // Тогда добавляем операцию AND в очередь
                        queue_operations.Add(RequestProcessing.OPER_AND);
                    flag_double_term = true;
                }
                // Нашли ОR или AND
                else if (RequestProcessing.IsOperation(words[i]))
                {
                    // Добавляем в очередь операций
                    queue_operations.Add(words[i]);

                    flag_double_term = false;
                }
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
            }

            return queue_terms[queue_terms.Count - 1];
        }

        private List<Document> SearchTerm(string term, bool isNot)
        {
            // Применить Стеминг к term
            //////////////////////////////////////////////////////////////////////////////////////







            //////////////////////////////////////////////////////////////////////////////////////


            // Очередь терминов (списков документов которые им соответствуют)
            var queue_terms = new List<List<Document>>();


            // Пройтись по триграммам и составить очередь для попарного И 
            term = "$" + term + "$";

            for (int i = 0; i < term.Length; i++)
            {
                if (i + 2 < term.Length)
                {
                    Console.WriteLine("ПОИСК:" + term.Substring(i, 3));
                    queue_terms.Add(treeIndex.SearchTrigram(term.Substring(i, 3)));
                }
            }


            // Проходим по очереди операций 
            // Сначала исполняем только AND
            for (int i = 1; i < queue_terms.Count; i++)
            {
                
                // Выполняем операцию между i и i + 1
                var result = RequestProcessing.OperationAnd(queue_terms[i - 1], queue_terms[i]);

                queue_terms[i - 1] = result;
                queue_terms[i] = result;
                
            }

            return queue_terms[queue_terms.Count - 1];

            //////////////////////////////////////////////////////////////////////////////////////////////////
            // Сначала находим нужный термин
            /*if (!isNot)
            {
                Term? _term = Terms.Where(i => i.Name == term).FirstOrDefault();

                if (_term != null)
                    return _term.Documents;
                else
                    return new List<Document>();
            }
            else
            {
                return RequestProcessing.OperationNot(term, Terms);
            }*/
            ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        public void PrintTermStatistics()
        {
            Console.WriteLine("-----------------------PrintTermStatistics()-----------------------");
            Console.WriteLine(Terms.Count);
            foreach (Term term in Terms)
            {
                Console.WriteLine($"{term.Name} ** Frequency: {term.Frequency}");
                foreach (Document document in term.Documents)
                {
                    Console.WriteLine($"Документ Id: {document.FileId} ** Frequency: {document.Frequency}");
                    foreach (int position in document.Positions)
                    {
                        Console.WriteLine($"Позиция -> {position}");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}