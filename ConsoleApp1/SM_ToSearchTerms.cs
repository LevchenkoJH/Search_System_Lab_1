using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    public class SM_ToSearchTerms
    {
        // Состояния
        /// <summary>
        /// Текущее состояние
        /// </summary>
        private int CURRENT_STATE = int.MaxValue;
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
        /// Текущая позиция слова (номер строки)
        /// </summary>
        private int CurrentPosition_Line = 0;
        /// <summary>
        /// Текущая позиция слова (номер символа)
        /// </summary>
        private int CurrentPosition_Symbol = 0;
        /// <summary>
        /// Id текущего документа
        /// </summary>
        private int CurrentDocumentId = 0;
        //private Document CurrentDocument = new Document();

        /// <summary>
        /// Собранные слова
        /// </summary>
        private List<Term> terms = new List<Term>();

        public SM_ToSearchTerms()
        {
            CURRENT_STATE = STATE_WAITING;
        }

        /// <summary>
        /// Получаем термины из строки
        /// </summary>
        /// <param name="line">Обрабатываемая строка</param>
        /// <param name="position">Номер строки</param>
        /// <param name="documentId">Id документа</param>
        public void GetTerms(string line, int position, int documentId)
        {
            // ДУМАЮ термину обнулять не нужно 
            // Все термины будет накапливать конечный автомат

            // Фиксируем позицию (номер) строки в документе
            CurrentPosition_Line = position;
            // Фиксируем Id документа
            CurrentDocumentId = documentId;

            // Проходим по каждому символу
            for (int i = 0; i < line.Length; i++)
            {
                // Шаг автомата
                Step(line[i], i);
            }
        }

        /// <summary>
        /// Шаг конечного автомата
        /// </summary>
        /// <param name="symbol">Обрабатываемый символ</param>
        /// <param name="position">Его позиция в строке</param>
        private void Step(char symbol, int position)
        {
            // Фиксируем текущую позицию символа относительно строки
            CurrentPosition_Symbol = position;
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
            if (symbol >= 'a' && symbol <= 'z')
            {
                // К собираемому термину добавляем символ
                CurrentTerm += symbol;
                // Устанавливаем состояние поиска конца термина
                CURRENT_STATE = STATE_END_TERM;
            }
        }

        /// <summary>
        /// Поиск конца термина
        /// </summary>
        /// <param name="symbol">Обрабатываемый символ</param>
        private void SearchEndTerm(char symbol)
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
            // Ищем данный термин в коллекции
            int term_index = terms.FindIndex(i => i.Name == CurrentTerm);

            // Если данный термин уже был найден ранее
            if (term_index != -1)
            {
                // Нужно что-то сделать с его статистикой

                // В очередной раз встретили это слово
                terms[term_index].Frequency += 1;

                // и со списком документов
                // Проверяем записан ли текущий документ в списке термина
                int document_index = terms[term_index].Documents.FindIndex(i => i.Id == CurrentDocumentId);
                
                // Если уже есть
                if (document_index != -1)
                {
                    // Добавляем позицию термина
                    terms[term_index].Documents[document_index].Positions.Add(KeyValuePair.Create(key: CurrentPosition_Line, value: CurrentPosition_Symbol));
                }
                else
                {
                    // Иначе создаем новый
                    Document _document = new Document()
                    {
                        Id = CurrentDocumentId,
                        Positions = new List<KeyValuePair<int, int>>() { KeyValuePair.Create(key: CurrentPosition_Line, value: CurrentPosition_Symbol) }
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
                            Id = CurrentDocumentId,
                            Positions = new List<KeyValuePair<int, int>>() { KeyValuePair.Create(key: CurrentPosition_Line, value: CurrentPosition_Symbol) }
                        }
                    }
                };

                terms.Add(_term);
            }
        }

        public void PrintIndex()
        {
            Console.WriteLine("-----------------------PrintIndex()-----------------------");
            Console.WriteLine(terms.Count);
            foreach (Term term in terms)
            {
                Console.WriteLine(term.Name);
                foreach (Document document in term.Documents)
                {
                    Console.WriteLine("Документ Id: " + document.Id.ToString());
                    foreach (KeyValuePair<int, int> position in document.Positions)
                    {
                        Console.WriteLine($"Позиция -> Строка: {position.Key} :: Символ: {position.Value}");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
