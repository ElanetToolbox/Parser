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
			//Functions.UnRar(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0079795 - Copy\ΣΥΝΝΗΜΕΝΑ__11127541.rar",@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\ΚΜΕ7-0079795 - Copy");
			//Functions.UploadFileSCP();
			//Functions.UploadStream("ΚΜΕ7-0079786", File.ReadAllBytes(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\test.zip"), "test.zip");

			Project_Collection projects = new Project_Collection();
			//Project_Collection projects = Functions.LoadFromFile(@"C:\Users\chatziparadeisis.i\Documents\covid\athens.fol");
			//projects.Projects.ForEach(x => x.Uploaded = false);
			projects.ScanPath(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΑΤΤΕ3-ΒΑΡΕ6-ΝΑΙΕ2");
			//string NothingPaths = string.Join("\n", projects.Projects.Where(x => x.HasNothing).Select(x => x.ProjectPath));
			//var p2 = projects.Projects.Where(x => !x.Uploaded).ToList();
			//var p3 = projects.Projects.Where(x => x.Uploaded).ToList();
			//projects.Projects.ForEach(x => x.UploadProject());
			//DateTime start = DateTime.Now;
			//p2.ForEach(x => x.UploadProject());
			//Parallel.ForEach(p2, project =>
			//{
			//	try
			//	{
			//		project.UploadProject();
			//	}
			//	catch
			//	{
			//	}
			//});
			//DateTime end = DateTime.Now;
			//TimeSpan totalTime = end - start;
			//int x = projects.Projects.Where(x => x.Uploaded).Count();
			//Project_Collection projects = Functions.LoadFromFile(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\collection.fol");
			//var e = projects.Projects.Where(x => x.Establishments).ToList();
			//var c = projects.Projects.Where(x => x.Complete).ToList();
			//List<string> existingProjectPaths = projects.Projects.Select(x => x.ProjectPath).Distinct().ToList();

			//string corrupts = string.Join("\n", projects.Projects.Where(x=>x.Docs == null).Select(x=>x.ProjectPath).Distinct());
			//projects.ScanPath(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7", existingProjectPaths);

			//Project_Collection projects = Functions.LoadFromFile(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\collection.fol");
			//Global.Project_Collection = projects;
			//projects.Taxis.Where(x => x.DocType == 1).ToList().ForEach(x => x.Scan());
			//projects.ScanPath(@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7");
			//Functions.SaveToFile(projects,@"T:\ToolboxStorage\Υλοποίηση\Προγράμματα\ΚΜΕ7\collection.fol");
			Functions.SaveToFile(projects, @"C:\Users\chatziparadeisis.i\Documents\covid\athens.fol");
		}
    }
}
