using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    internal class TermReader
    {
        /// <summary>
        /// Id текущего документа
        /// </summary>
        private static int CurrentFileId = 0;

        /// <summary>
        /// Текущая позиция термина
        /// </summary>
        private static int PositionTerm = 0;

        /// <summary>
        /// Текущая позиция символа
        /// </summary>
        private static int PositionSybol = 0;

        /// <summary>
        /// Текущее состояние
        /// </summary>
        private static int CURRENT_STATE = -1;
        /// <summary>
        /// Состояние ожидания
        /// </summary>
        private const int STATE_WAITING = -1;
        /// <summary>
        /// Поиск начала слова
        /// </summary>
        private const int STATE_START_TERM = 0;
        /// <summary>
        /// Поиск конца слова
        /// </summary>
        private const int STATE_END_TERM = 1;

        /// <summary>
        /// Текущее собираемое слово
        /// </summary>
        private static string CurrentTerm = "";

        /// <summary>
        /// Считывание слов из файла
        /// </summary>
        /// <param name="filePath"></param>
        public static void FindTermsInFile(FileReader fileReader, int fileId)
        {
            // Фиксируем Id документа
            CurrentFileId = fileId;

            // Фиксируем позицию термина
            PositionTerm = 0;

            // Фиксируем позицию символа
            PositionSybol= 0;

            // Считываем файл - построчно
            string line = fileReader.ReadLine();
            while (line != null)
            {
                //Console.WriteLine(line);
                line = fileReader.ReadLine();

                // Передаем строки конечному атомату для поиска терминов
                if (line != null)
                {
                    line = line.ToLower();
                    Console.WriteLine("line != null " + "---->" +line);
                    GetTerms(line);
                }
            }

            CURRENT_STATE = STATE_WAITING;
        }

        /// <summary>
        /// Получаем термины из строки
        /// </summary>
        /// <param name="line">Обрабатываемая строка</param>
        private static void GetTerms(string line)
        {
            // Проходим по каждому символу
            for (int i = 0; i < line.Length; i++)
            {
                // Шаг автомата
                Step(line[i]);
                PositionSybol++;
            }
        }

        /// <summary>
        /// Шаг конечного автомата
        /// </summary>
        /// <param name="symbol">Обрабатываемый символ</param>
        private static void Step(char symbol)
        {
            switch (CURRENT_STATE)
            {
                case STATE_WAITING:
                    CURRENT_STATE = STATE_START_TERM;
                    SearchStartTerm(symbol);
                    break;
                case STATE_START_TERM:
                    SearchStartTerm(symbol);
                    break;
                case STATE_END_TERM:
                    SearchEndTerm(symbol);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Поиск начала термина
        /// </summary>
        /// <param name="symbol">Обрабатываемый символ</param>
        private static void SearchStartTerm(char symbol)
        {
            // Термины начинаются только с букв
            // Если символ является буквой
            if (symbol >= 'a' && symbol <= 'z')
            {
                // К собираемому термину добавляем символ
                CurrentTerm += symbol;
                // Устанавливаем состояние поиска конца термина
                CURRENT_STATE = STATE_END_TERM;

                PositionTerm = PositionSybol;
            }
        }

        /// <summary>
        /// Поиск конца термина
        /// </summary>
        /// <param name="symbol">Обрабатываемый символ</param>
        private static void SearchEndTerm(char symbol)
        {
            // При встрече символа являющегося частью термина
            if ((symbol >= 'a' && symbol <= 'z') || symbol == '-' || symbol == '\'')
            {
                // Продолэаем собирать термин
                CurrentTerm += symbol;
            }
            // Иначе окончание слова
            else
            {
                // Добавлям слово в коллекцию
                Console.WriteLine("Нашел: " + CurrentTerm + " Поз: " + PositionTerm.ToString());
                //AddTerm();
                CurrentTerm = "";
                CURRENT_STATE = STATE_WAITING;
            }
        }
    }
}
