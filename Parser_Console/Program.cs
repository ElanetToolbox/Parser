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
			//Project_Collection projects = Functions.LoadFromFile(@"C:\Users\chatziparadeisis.i\Documents\covid\athens.fol");
			//projects.UploadData();

			F2 newF2 = new F2();
			newF2.FilePath = @"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΑΤΤΕ3-ΒΑΡΕ6-ΝΑΙΕ2\ATTE3-0155268\Φ2_06.2020 Τροποποιητική Α__11598014.pdf";
			//newF2.FilePath = @"\\FS\vol1\1. Προγραμματική Περίοδος 2014-2020\1. Ενίσχυση Πτυχιούχων\Docs\Φ2\ΔΗΛΩΣΗ Φ.Π.Α ΙΑΝΟΥΑΡΙΟΥ.pdf";
			newF2.Scan();

			//DebugFunctions.LocScan(@"\\FS\vol1\1. Προγραμματική Περίοδος 2014-2020\1. Ενίσχυση Πτυχιούχων\Docs\Φ2\ΔΗΛΩΣΗ Φ.Π.Α ΙΑΝΟΥΑΡΙΟΥ.pdf");
		}
    }
}
