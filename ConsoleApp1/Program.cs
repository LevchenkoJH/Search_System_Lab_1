using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace search_engine
{
    internal struct Term
    {
        public string term;
        public int frenq;
    }


    internal class Program
    {

        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "text_documents\\";



            using (StreamReader sr = File.OpenText(path + "text_1.txt"))
            {
                while (sr.Peek() != -1)
                {
                    char c = (char)sr.Read();

                    Console.WriteLine(c);

                }
            }

            Console.WriteLine(path);

        }
    }
}