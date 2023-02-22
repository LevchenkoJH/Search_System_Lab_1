// See https://aka.ms/new-console-template for more information
using SearchSystem;

// Console.WriteLine("Hello, World!");

//FileReader TEST_FILE = new FileReader(".\\TERMS.txt");

//string TEST_STRING = TEST_FILE.ReadLine();
//Console.WriteLine(TEST_STRING);

//TEST_STRING = TEST_FILE.ReadLine();
//Console.WriteLine(TEST_STRING);

//TEST_STRING = TEST_FILE.ReadLine();
//Console.WriteLine(TEST_STRING);

//TEST_FILE.FileClose();

//Console.ReadLine();

//Documents A = new Documents();



//Console.WriteLine(Path.Join(Directory.GetCurrentDirectory(), "input"));



SearchSystemL1 A = new SearchSystemL1(filesDirectory: "input");

A.Search();

/*while (true)
{
    string test_str = Console.ReadLine();
    Console.WriteLine(test_str);
}*/

//List<string> list = new List<string>() { "a", "b" };

//var a = list.Where(i => i == "d").Select((item, index) => index).FirstOrDefault();
//var a = list.FindIndex(x => x.Equals("D"));
//Console.WriteLine(a);
//list[a] = "b";
//Console.WriteLine(list[0] + " " + list[1]);