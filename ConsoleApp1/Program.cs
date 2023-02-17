using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace search_engine
{
    internal struct Term
    {
        public string name;
        public int frenq;

        public Term()
        {
            name = "";
            frenq = 0;
        }
    }

    internal enum State
    {
        Word,
        Symbol
    }


    internal class Program
    {
        static List<Term> terms = new List<Term>();

        static void Main(string[] args)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString() + "text_documents\\";
            State state = State.Word;
            Term blanc = new Term();

            using (StreamReader sr = File.OpenText(path + "text_1.txt"))
            {
                while (sr.Peek() != -1)
                {
                    char letter = (char)sr.Read();
                    if (char.IsLetter((char)letter))
                    {
                        blanc.name += letter.ToString();
                    }
                    else
                    {
                        if (blanc.name != "")
                        {
                            bool found = false;
                            foreach (var term in terms)
                            {
                                if (blanc.name == term.name)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                terms.Add(blanc);
                                blanc = new Term();
                            }
                            else
                            {
                                blanc = new Term(); 
                            }
                        }
                        
                    }
                    /*switch (state)
                    {
                        case State.Word:
                        {
                            if (char.IsLetter((char)sr.Peek()))
                            {
                                blanc.name += sr.Peek().ToString();
                                break;
                            }
                            else
                            {
                                state = State.Symbol;
                                terms.Add(blanc);
                                blanc = new Term();
                                break;
                            }
                        }
                            

                        case State.Symbol:
                            if (char.IsLetter((char)sr.Peek()))
                            {
                                state
                                break;
                            }
                            else
                            {
                                state = State.Word;
                                break;
                            }
                    }*/
                }
            }
            foreach(Term term in terms)
            {
                Console.WriteLine(term.name.ToString() + "\n");
            }
            Console.WriteLine(path);

        }
    }
}