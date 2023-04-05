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
        /// Файл (документ) на который ссылаемся
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// Сколько раз данный термин встречается в этом документе
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// Позиции слова в документе
        /// </summary>
        public List<int> Positions { get; set; }
    }

    internal class File
    {
        /// <summary>
        /// Документ (файл)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название файла (документа)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Количество терминов в файле (документе)
        /// </summary>
        public int Frequency { get; set; }
    }
}
