using IronXL;
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
			var folders = Directory.GetDirectories(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΑΤΤΕ3-ΒΑΡΕ6-ΝΑΙΕ2\EPIPLEON_NAIE2").ToList();

            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\covid\athens.fol";
            Project_Collection projects = Functions.LoadFromFile(docPath);

            var nProj = projects.Projects.Where(x => x.Code.Contains("NAIE")).ToList();
			var proj = nProj.Where(x => x.F2s.Where(y => y.Outflow != 0).Any()).ToList();
			foreach (var item in proj)
			{
				item.UploadProject();
			}
			var f2s = nProj.SelectMany(x=>x.F2s).ToList();
			int i = 0;
			foreach (var f in f2s)
			{
				i++;
				f.GetOutflow();
				Console.WriteLine(i);
			}

   //         int i = 0;
   //         foreach (var p in nProj)
			//{
			//	i++;
			//	try
			//	{
			//		p.UploadProject();
			//	}
			//	catch { }
			//	Console.WriteLine(i.ToString());
			//}

            Functions.SaveToFile(projects, docPath);
        }
    }
}
