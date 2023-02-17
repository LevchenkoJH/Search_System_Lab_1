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
        /// 
        /// </summary>
        public string Name;
        /// <summary>
        /// 
        /// </summary>
        int Frequency;
        /// <summary>
        /// 
        /// </summary>
        List<Document> Documents;
    }

    internal class Document
    {
        /// <summary>
        /// 
        /// </summary>
        int Id;
        /// <summary>
        /// 
        /// </summary>
        int Frequency;
        /// <summary>
        /// 
        /// </summary>
        List<int> Positions;
    }
}
