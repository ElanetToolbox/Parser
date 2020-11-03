using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.VisualBasic;
using Parser_Console.Classes;
using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			//Project_Collection projects = new Project_Collection();
			Project_Collection projects = Functions.LoadFromFile(@"C:\Users\chatziparadeisis.i\Documents\covid\athens.fol");

			//DebugFunctions.LocScan(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΑΤΤΕ3-ΒΑΡΕ6-ΝΑΙΕ2\ATTE3-0155220\Φ2_04.2019__11501366.pdf");
		}
    }
}
