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
			Project_Collection projects = Functions.LoadFromFile(@"C:\Users\chatziparadeisis.i\Documents\covid\athens.fol");
			//projects.Projects.ForEach(x => x.Uploaded = false);
			int x = projects.Projects.Where(x => x.Uploaded).Count();
			int y = projects.Projects.Where(x => x.CanUpload).Count();
			int z = projects.Projects.Where(x => x.Removed).Count();
			projects.UploadData();
        }
    }
}
