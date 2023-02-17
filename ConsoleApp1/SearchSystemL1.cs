
namespace SearchSystem
{
    public class SearchSystemL1
    {
        /// <summary>
        /// ������ ����������
        /// </summary>
        //private List<Documents> Documents;

        /// <summary>
        /// ������ ����
        /// </summary>
        private List<Term> Terms = new List<Term>();

        /// <summary>
        /// ������� ����� (����� ������ �� ����� �����)
        /// </summary>
        /// <param name="filesDirectory">������� � ������� ��� ������ (������� ����������� � exe ������)</param>
        public SearchSystemL1(string filesDirectory)
        {
            try
            {
                string filesPath = Path.Join(Directory.GetCurrentDirectory(), filesDirectory);
                string[] fileNames = Directory.GetFiles(filesPath);

                Console.WriteLine("����� �������� " + filesPath);
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
        /// ���������� ���� �� �����
        /// </summary>
        /// <param name="filePath"></param>
        private void FindTermsInFile(string filePath)
        {
            FileReader fileReader = new FileReader(filePath);

            // ��������� ���� - ���������

            string line = fileReader.ReadLine();
            while (line != null)
            {

                // Console.WriteLine(line);
                line = fileReader.ReadLine();

                // �������� ������ ��������� ������� ��� ������ ����
            }

        }
    }
}