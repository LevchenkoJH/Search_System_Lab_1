
namespace SearchSystem
{
    public class SearchSystemL1
    {
        /// <summary>
        /// Список документов
        /// </summary>
        //private List<Documents> Documents;
        // Список документов теперь в конечном автомате
        private SM_ToSearchTerms sM_ToSearchTerms = new SM_ToSearchTerms();


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

                for (int i = 0; i < fileNames.Length; i++)
                {
                    FindTermsInFile(filePath: fileNames[i],fileId: i);
                }
                sM_ToSearchTerms.PrintIndex();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Считывание слов из файла
        /// </summary>
        /// <param name="filePath"></param>
        private void FindTermsInFile(string filePath, int fileId)
        {
            FileReader fileReader = new FileReader(filePath);

            // Считываем файл - построчно

            string line = fileReader.ReadLine();
            int line_position = 0;
            while (line != null)
            {

                // Console.WriteLine(line);
                line = fileReader.ReadLine();

                // Передаем строки конечному атомату для поиска слов
                if (line != null)
                {
                    Console.WriteLine("line != null");
                    sM_ToSearchTerms.GetTerms(line, line_position, fileId);
                }
            }
        }

        public void Search()
        {
            while (true)
            {
                Console.Write("Строка поиска:");
                string line_query = Console.ReadLine();


            }
        }
    }
}