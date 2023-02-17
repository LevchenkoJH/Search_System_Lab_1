using SearchSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search_System
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

        public void GetTerms(string line, int position, int documentId)
        {
            terms = new List<Term>();
            CurrentPosition_Line = position;
            CurrentDocumentId = documentId;
            /*CurrentDocument = new Document()
            {
                Id = documentId,
                Positions = new List<KeyValuePair<int, int>>()
            };*/

            for (int i = 0; i < line.Length; i++)
            {
                Step(line[i], i);

            }
        }

        private void Step(char symbol, int position)
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

        private void SearchStartTerm(char symbol)
        {
            if (symbol >= 'a' && symbol <= 'z')
            {
                CurrentPosition_Symbol = symbol;
                CurrentTerm += symbol;
                CURRENT_STATE = STATE_END_TERM;
            }
            else
            {

            }
        }

        private void SearchEndTerm(char symbol)
        {
            if ((symbol >= 'a' && symbol <= 'z') || symbol == '-' || symbol == '\'')
            {
                CurrentTerm += symbol;
            }
            else
            {
                AddTerm();
                CurrentTerm = "";
                CURRENT_STATE = STATE_WAITING;
            }
        }

        private void AddTerm()
        {
            
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
    }
}
