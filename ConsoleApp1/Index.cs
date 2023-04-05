using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchSystem
{
    public class Index
    {
        private List<Term> Terms = new List<Term>();

        private List<File> Files = new List<File>();

        public Index(string folderPath) 
        {
            GetFiles(folderPath);
            GetTerms();
        }

        /// <summary>
        /// Получаем файлы из папки
        /// </summary>
        private void GetFiles(string folderPath)
        {
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
                    Files.Add(file);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void GetTerms()
        {
            try
            {
                for (int i = 0; i < Files.Count(); i++)
                {
                    // Открываем файл
                    var fileReader = new FileReader(Files[i].Name);

                    // Собираем термины
                    TermReader.FindTermsInFile(fileReader, Files[i].Id);

                    // Статистика файла
                    Files[i].Frequency = TermReader.GetFileFrequency(Files[i].Id);

                    // Закрываем файл
                    fileReader.FileClose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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