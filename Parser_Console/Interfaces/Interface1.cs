using Parser_Console.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console.Interfaces
{
    public interface IDocument
    {
		string FilePath { get; set; }
        string FileType { get; }
    }
}
