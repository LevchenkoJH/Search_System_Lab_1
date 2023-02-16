// See https://aka.ms/new-console-template for more information
using SearchSystem;

Console.WriteLine("Hello, World!");

FileReader TEST_FILE = new FileReader("C:\\Users\\Nikita\\Desktop\\andrey\\Search_System_Lab_1\\ConsoleApp1\\TERMS.txt");

string TEST_STRING = TEST_FILE.ReadLine();
Console.WriteLine(TEST_STRING);

TEST_STRING = TEST_FILE.ReadLine();
Console.WriteLine(TEST_STRING);

TEST_STRING = TEST_FILE.ReadLine();
Console.WriteLine(TEST_STRING);

TEST_FILE.FileClose();

Console.ReadLine();