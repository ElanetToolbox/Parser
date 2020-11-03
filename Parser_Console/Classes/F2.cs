using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console.Classes
{
    public class F2 : IDocument
    {
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public List<int> Months { get; set; }
        public decimal WorkCycle { get; set; }
    }
}
