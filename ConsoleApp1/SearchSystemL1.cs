
namespace SearchSystem
{
    public class SearchSystemL1
    {
        /// <summary>
        /// Список документов
        /// </summary>
        //private List<Documents> Documents;

        /// <summary>
        /// Список слов
        /// </summary>
        private List<Term> Terms = new List<Term>();

        /// <summary>
        /// Базовый класс (Поиск только по одной папке)
        /// </summary>
        /// <param name="filesDirectory">Каталог с файлами для поиска (каталог соседствует с exe файлом)</param>
        public SearchSystemL1(string filesDirectory)
        {
            try
            {
                string filesPath = Path.Join(Directory.GetCurrentDirectory(), filesDirectory);
                string[] fileNames = Directory.GetFiles(filesPath);

                Console.WriteLine("Файлы каталога " + filesPath);
                foreach (string fileName in fileNames)
                {
                    Console.WriteLine(fileName);
                }

                FindTermsInFile(fileNames[0]);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {

            }

            

        }

        /// <summary>
        /// Считывание слов из файла
        /// </summary>
        /// <param name="filePath"></param>
        private void FindTermsInFile(string filePath)
        {
            FileReader fileReader = new FileReader(filePath);

            // Считываем файл - построчно

            string line = fileReader.ReadLine();
            while (line != null)
            {

                // Console.WriteLine(line);
                line = fileReader.ReadLine();

                // Передаем строки конечному атомату для поиска слов
            }

        }
    }
}