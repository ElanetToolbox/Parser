using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console.Classes
{
    public class UnknownDocument : IDocument
    {
        public string FilePath { get; set; }
		public string FileType => GetType().Name;
    }
}
