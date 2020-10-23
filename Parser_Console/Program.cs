using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Parser_Console.Classes;
using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Parser_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			Functions.GetKadEligibility("47717100");

			if(args.Count() > 0)
			{
                Functions.UploadDocument(@"ΚΜΕ7-0079786", args[0]);
				return;
			}
			//DebugFunctions.LocScan(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0079786\ΦΠΑ 4.2019__10785915.pdf");
			//Functions.UploadDocument(@"ΚΜΕ7-0079786", @"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0079786\TaxisNET__10783766.pdf");
			//Functions.UnzipFiles(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0079788 - Copy");
			//Functions.UploadDocument("ΚΜΕ7-0079786", @"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0079786\Ε3 2019__10785945.pdf");

			//Project_Collection projects = new Project_Collection();
			Project_Collection projects = Functions.LoadFromFile(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\collection.fol");
			Global.Project_Collection = projects;
			var p = projects.Projects.Where(x => x.Docs == null).ToList();
			List<string> existingProjectPaths = projects.Projects.Select(x => x.ProjectPath).Distinct().ToList();

			string corrupts = string.Join("\n", projects.Projects.Where(x=>x.Docs == null).Select(x=>x.ProjectPath).Distinct());
            //projects.ScanPath(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7", existingProjectPaths);

            //DebugFunctions.ScanDocument(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0085004\02 ΠΡΟΣΩΠΟΠΟΙΗΜΕΝΗ TAXISNET - ΜΠΙΝΤΖΙΝΑΣΒΙΛΙ ΣΟΦΙΑ__10896365.pdf");

            //Project_Collection projects = Functions.LoadFromFile(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\collection.fol");
            //Global.Project_Collection = projects;
            //projects.Taxis.Where(x => x.DocType == 1).ToList().ForEach(x => x.Scan());
            //projects.ScanPath(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7");
            //Functions.SaveToFile(projects);
        }
    }
}
