using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    public class SearchEngine
    {
        private const char COMAND_SYMBOL = '!';

        private Index index;

        public SearchEngine(string folderPath)
        {
            // Собираем файлы из директории
            List<File> files = GetFiles(folderPath);
            // На основании этих файлов создаем индекс
            // Классы поддержки используется в индексе для считывания файлов
            index = new Index(files);
            // Ожидаем запрос пользователя
            WaitingRequest();
        }

        /// <summary>
        /// Получаем файлы из папки
        /// </summary>
        private List<File> GetFiles(string folderPath)
        {
            List<File> files = new List<File>();
            try
            {
                string filesPath = Path.Join(Directory.GetCurrentDirectory(), folderPath);
                string[] fileNames = Directory.GetFiles(filesPath);

                Console.WriteLine("Документы:");
                foreach (string fileName in fileNames)
                {
                    Console.WriteLine(fileName);
                }

                for (int i = 0; i < fileNames.Length; i++)
                {
                    var file = new File
                    {
                        Id = Guid.NewGuid(),
                        Name = fileNames[i],
                        Frequency = 0
                    };
                    files.Add(file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return files;
        }

        private void WaitingRequest()
        {
            Console.Write(">");
            string request = Console.ReadLine();
            if (request != null)
            {
                if (!Request(request))
                    WaitingRequest();
            }
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
                case "files":
                    index.PrintFileStatistics();
                    return false;
                case "terms":
                    index.PrintTermStatistics();
                    return false;
                default:
                    return false;
            }
        }

        private bool SearchProcessing(string search)
        {
            Console.WriteLine($"-----------------------SEARCH:\"{search}\"-----------------------");

            // Запрос отправляется в индекс
            List<Document> result = index.Search(search).OrderByDescending(i => i.Frequency).ToList();
            
            foreach (Document document in result)
            {
                Console.WriteLine($"File: {index.GetFileName(document.FileId)}\nFrequency: {document.Frequency}");
                Console.Write("Position:");
                foreach (int position in document.Positions)
                {
                    Console.Write($"{position}, ");
                }
                Console.Write("\n");
            }

            return false;
        }
    }
}
