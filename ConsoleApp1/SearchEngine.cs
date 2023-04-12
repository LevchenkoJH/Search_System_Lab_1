using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    class SearchEngine
    {
        private Index index;


        SearchSystem(string folderPath)
        {
            List<File> files = GetFiles(folderPath);
            index = new Index(files);


            WaitingRequest();
        }

        /// <summary>
        /// Получаем файлы из папки
        /// </summary>
        private List<File> GetFiles(string folderPath)
        {
            try
            {
                List<File> files = new List<File>();
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
        }








        private void WaitingRequest()
        {
            Console.Write(">");
            string request = Console.ReadLine();
            if (request != null)
            {
                if (!requestProcessing.Request(request))
                    WaitingRequest();
            }
        }
    }
}
