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
        string Name;
        /// <summary>
        /// 
        /// </summary>
        int Frequency;
        /// <summary>
        /// 
        /// </summary>
        List<Documents> Documents;
    }

    internal class Documents
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
