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
        /// <summary>
        /// Класс для обработки запроса
        /// </summary>
        private RequestProcessing requestProcessing;// Возможно стоит сделать статичным 




        public Index(List<File> files) 
        {
            Files = files;
            GetTerms();
            requestProcessing = new RequestProcessing(Files.ToDictionary(i => i.Id, i => i.Name));

            
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