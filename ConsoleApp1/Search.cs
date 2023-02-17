using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    internal class Term
    {
        /// <summary>
        /// Название термина
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Сколько раз встречался во всех документах
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// Документы в которых встречался
        /// </summary>
        public List<Document> Documents { get; set; }
    }

    internal class Document
    {
        /// <summary>
        /// Идентификатор документа
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Частота 
        /// </summary>
        //public int Frequency { get; set; }
        /// <summary>
        /// Позиции слова в документе (key - строка, value - символ)
        /// </summary>
        public List<KeyValuePair<int, int>> Positions { get; set; }
    }
}
