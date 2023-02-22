
namespace SearchSystem
{
    public class SearchSystemL1
    {
        /// <summary>
        /// ������ ����������
        /// </summary>
        //private List<Documents> Documents;
        // ������ ���������� ������ � �������� ��������
        private SM_ToSearchTerms sM_ToSearchTerms = new SM_ToSearchTerms();


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
        /// ���������� ���� �� �����
        /// </summary>
        /// <param name="filePath"></param>
        private void FindTermsInFile(string filePath, int fileId)
        {
            FileReader fileReader = new FileReader(filePath);

            // ��������� ���� - ���������

            string line = fileReader.ReadLine();
            int line_position = 0;
            while (line != null)
            {

                // Console.WriteLine(line);
                line = fileReader.ReadLine();

                // �������� ������ ��������� ������� ��� ������ ����
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
                Console.Write("������ ������:");
                string line_query = Console.ReadLine();


            }
        }
    }
}