﻿using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
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
			Project_Collection projects = Functions.LoadFromFile(@"C:\Users\chatziparadeisis.i\Documents\covid\collection.fol");
			var p = projects.Projects.Take(50).ToList();
			DateTime start = DateTime.Now;
			p.ForEach(x => x.UploadProject());
            DateTime end = DateTime.Now;
			TimeSpan totalTime = end - start;
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
			Functions.SaveToFile(projects,@"C:\Users\chatziparadeisis.i\Documents\covid\collection.fol");
		}
    }
}
