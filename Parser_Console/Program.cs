using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Parser_Console.Classes;
using System;
using System.IO;

namespace Parser_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			string d_path = @"C:\Users\Kostas\Downloads\e3\Ε3_2019";
			var pdfs = Directory.GetFiles(d_path, "*.pdf");
			var e3col = new E3_Collection();
			foreach (string path in pdfs)
			{
				e3col.AddE3(path);
			}
		}
	}
}
