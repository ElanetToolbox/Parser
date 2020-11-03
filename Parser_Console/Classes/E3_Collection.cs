using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Parser_Console.Classes
{
	public class DocumentCollection
	{
		public List<IDocument> Documents { get; set; }
		public List<CorruptDocument> corruptDocuments => Documents.Where(x => x is CorruptDocument).Select(x => (CorruptDocument)x).ToList();
		public List<E3> E3s => Documents.Where(x => x is E3).Select(x => (E3)x).ToList();
		public List<F2> F2s => Documents.Where(x => x is F2).Select(x => (F2)x).ToList();
		public List<E3> ValidE3s => Documents.Where(x => x is E3).Select(x => (E3)x).Where(x=>x.Year == 2019).ToList();
		public List<Taxis> TaxisList => Documents.Where(x => x is Taxis).Select(x => (Taxis)x).ToList();
		public List<Taxis> ValidTaxisList => TaxisList.Where(x=>x.Complete).ToList();
		public Project Project { get; set; }

		public DocumentCollection()
		{
			Documents = new List<IDocument>();
		}

		public void AddDocument(string path)
		{
            PdfReader reader;
            PdfDocument doc;
			try
			{
                reader = new PdfReader(path);
                doc = new PdfDocument(reader);
			}
			catch
			{
				AddCorrupt(path);
				return;
			}
			string p1 = "";
			try
			{
				p1 = PdfTextExtractor.GetTextFromPage(doc.GetFirstPage(), new LocationTextExtractionStrategy());
			}
			catch
			{
				AddUnknown(path);
			}
			int e3Check = p1.IndexOf("ΚΑΤΑΣΤΑΣΗ ΟΙΚΟΝΟΜΙΚΩΝ ΣΤΟΙΧΕΙΩΝ");
			int taxisCheck = p1.IndexOf("TaxisNET");
			if(e3Check != -1)
			{
				AddE3(path);
				return;
			}
			if(taxisCheck != -1)
			{
				AddTaxis(path);
				return;
			}
			string line1 = Functions.GetLine(p1, 2);
			string line2 = Functions.GetLine(p1, 3);
			if(Functions.IsStringNumeric(line1) && Functions.IsStringNumeric(line2))
			{
				AddF2(path);
				return;
			}
			AddUnknown(path);
		}

		private void AddF2(string path)
		{
			F2 newF2 = new F2();
			newF2.FilePath = path;
			Documents.Add(newF2);
			try
			{
				newF2.Scan();
			}
			catch
			{
				newF2.ParsingErrorExternal = true;
			}
		}

		public void AddE3(string path)
		{
			E3 newE3 = new E3();
			newE3.FilePath = path;
			Documents.Add(newE3);
			try
			{
				newE3.ScanE3(path);
			}
			catch
			{
				newE3.ParsingErrorExternal = true;
			}
		}
		
		public void AddTaxis(string path)
		{
			Taxis newTaxis = new Taxis();
			newTaxis.FilePath = path;
			Documents.Add(newTaxis);
			try
			{
				newTaxis.Scan();
			}
			catch
			{
				newTaxis.ParsingErrorExternal = true;
			}
		}

		public void AddUnknown(string path)
		{
			UnknownDocument newUnknown = new UnknownDocument();
			newUnknown.FilePath = path;
			Documents.Add(newUnknown);
		}

		private void AddCorrupt(string path)
		{
			CorruptDocument newCorrupt = new CorruptDocument();
			newCorrupt.FilePath = path;
			Documents.Add(newCorrupt);
		}
	}
}
