using ConsoleApp1;
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
        private Guid CurrentFileId = Guid.Empty;

        /// <summary>
        /// Текущая позиция термина
        /// </summary>
        private int PositionTerm = 0;

        /// <summary>
        /// Текущая позиция символа
        /// </summary>
        private int PositionSybol = 0;

        /// <summary>
        /// Текущее состояние
        /// </summary>
        private int CURRENT_STATE = -1;
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
        private string CurrentTerm = "";

        /// <summary>
        /// Собранные термины
        /// </summary>
        private List<Term> terms = new List<Term>();


        //private Stemmer stem = new Stemmer();


        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public TreeForIndex treeIndex = new TreeForIndex();

        /// <summary>
        /// Считывание слов из файла
        /// </summary>
        /// <param name="filePath"></param>
        public void FindTermsInFile(FileReader fileReader, Guid fileId)
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
                    GetTerms(line);
                }
            }

            CURRENT_STATE = STATE_WAITING;
        }

        /// <summary>
        /// Получаем термины из строки
        /// </summary>
        /// <param name="line">Обрабатываемая строка</param>
        private void GetTerms(string line)
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
        private void Step(char symbol)
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
        private void SearchStartTerm(char symbol)
        {
            // Термины начинаются только с букв
            // Если символ является буквой
            if (symbol >= 'а' && symbol <= 'я')
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
        private void SearchEndTerm(char symbol)
        {
            // При встрече символа являющегося частью термина
            if ((symbol >= 'а' && symbol <= 'я') || symbol == '-' || symbol == '\'')
            {
                // Продолжаем собирать термин
                CurrentTerm += symbol;
            }
            // Иначе окончание слова
            else
            {
                // Добавлям слово в коллекцию
                AddTerm();
                CurrentTerm = "";
                CURRENT_STATE = STATE_WAITING;
            }
        }

        /// <summary>
        /// Добавление нового термина в коллекцию + её обслуживание
        /// </summary>
        private void AddTerm()
        {
            //if (CurrentTerm == "AND")
            //Console.WriteLine("TERM = " + CurrentTerm);
            Stemmer stem = new Stemmer();
            string CurrTerm = stem.Stem(CurrentTerm);
            CurrTerm = "$" + CurrTerm + "$";
            Console.WriteLine("NEW WORD = " + CurrTerm);
            
            for (int i = 0; i < CurrTerm.Length; i++)
            {
                if (i + 2 < CurrTerm.Length)
                {
                    treeIndex.AddTrigram(CurrTerm.Substring(i, 3), CurrentFileId, PositionTerm);
                }
            }

            //treeIndex.Add

            // Ищем данный термин в коллекции
            /*int term_index = terms.FindIndex(i => i.Name == CurrentTerm);

            // Если данный термин уже был найден ранее
            if (term_index != -1)
            {
                // Нужно что-то сделать с его статистикой

                // В очередной раз встретили это слово
                terms[term_index].Frequency += 1;

                // и со списком документов
                // Проверяем записан ли текущий документ в списке термина
                int document_index = terms[term_index].Documents.FindIndex(i => i.FileId == CurrentFileId);

                // Если уже есть
                if (document_index != -1)
                {
                    // Добавляем позицию термина
                    terms[term_index].Documents[document_index].Positions.Add(PositionTerm);

                    terms[term_index].Documents[document_index].Frequency++;
                }
                else
                {
                    // Иначе создаем новый
                    Document _document = new Document()
                    {
                        FileId = CurrentFileId,
                        Positions = new List<int>() { PositionTerm },

                        Frequency = 1
                    };
                    terms[term_index].Documents.Add(_document);
                }
            }
            else
            {
                // Иначе добаляем новый термин
                Term _term = new Term()
                {
                    Name = CurrentTerm,
                    Frequency = 1,
                    // C добавленным документом
                    Documents = new List<Document>()
                    {
                        new Document()
                        {
                            FileId = CurrentFileId,
                            Positions = new List<int>() { PositionTerm },

                            Frequency = 1
                        }
                    }
                };

                terms.Add(_term);
            }*/
        }

        /// <summary>
        /// Сколько терминов в себе содержит документ
        /// </summary>
        public int GetFileFrequency(Guid fileId)
        {
            return terms
                .Where(i => i.Documents
                                .Where(j => j.FileId == fileId)
                                    .Count() != 0
                ).Count();
        }

        public List<Term> GetTerms()
        {
            return terms;
        }

        public TreeForIndex GetTree()
        {
            return treeIndex;
        }
    }
}
