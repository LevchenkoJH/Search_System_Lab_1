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
        /// Собранные слова
        /// </summary>
        private List<Term> terms = new List<Term>();

        public SM_ToSearchTerms()
        {
            CURRENT_STATE = STATE_WAITING;
        }

        public void GetTerms(string line)
        {
            terms = new List<Term>();

            foreach(char symbol in line)
            {
                Step(symbol);
            }
        }

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

        private void SearchStartTerm(char symbol)
        {
            if (symbol >= 'a' && symbol <= 'z')
            {
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
            Term _term = new Term();
            _term.Name = CurrentTerm;
            //terms.Add(_term);

            // Если данный термин уже был найден ранее
            if (terms.Where(i => i.Name == _term.Name).Any())
            {
                // Нужно что-то сделать с его статистикой

                // и со списком документов
            }
            else
            {
                // Иначе просто добаляем
                terms.Add(_term);
            }
        }
    }
}
