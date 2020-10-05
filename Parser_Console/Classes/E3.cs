using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout.Element;

namespace Parser_Console.Classes
{
	public class E3
	{
		public string FileName { get; set; }
		public string Afm { get; set; }
		List<KeyValuePair<string, decimal?>> Values;
		//public decimal? n102 { get; set; }
		//public decimal? n202 { get; set; }
		//public decimal? n181 { get; set; }
		//public decimal? n281 { get; set; }
		//public decimal? n481 { get; set; }
		//public decimal? n185 { get; set; }
		//public decimal? n285 { get; set; }
		//public decimal? n485 { get; set; }
		public bool error { get; set; }
		public bool noText { get; set; }
		public bool notE3 { get; set; }
		public bool weirdE3 { get; set; }

		public E3(string path)
		{
			FileName = Path.GetFileName(path);
			Values = new List<KeyValuePair<string, decimal?>>();
			PdfReader reader = new PdfReader(path);
			PdfDocument doc = new PdfDocument(reader);
			string p1 = PdfTextExtractor.GetTextFromPage(doc.GetFirstPage(), new LocationTextExtractionStrategy());
			int iCheck = p1.IndexOf("ΚΑΤΑΣΤΑΣΗ ΟΙΚΟΝΟΜΙΚΩΝ ΣΤΟΙΧΕΙΩΝ");
			if (string.IsNullOrWhiteSpace(p1))
			{
				error = true;
				noText = true;
				return;
			}
			if (iCheck == -1)
			{
				error = true;
				notE3 = true;
				return;
			}

			Afm = GetAfm(p1);

			string p2 = PdfTextExtractor.GetTextFromPage(doc.GetPage(2),new LocationTextExtractionStrategy());
			//n102 = GetSingle(p2,9,102);
			Values.Add(new KeyValuePair<string, decimal?>("102", GetSingle(p2,9,102)));
			//n202 = GetBetween(p2,20,202,302);
			Values.Add(new KeyValuePair<string, decimal?>("202", GetBetween(p2,20,202,302)));

			reader = new PdfReader(path);
			string p3 = PdfTextExtractor.GetTextFromPage(doc.GetPage(3),new LocationTextExtractionStrategy());
			//n181 = GetBetween(p3,75, 181, 281);
			Values.Add(new KeyValuePair<string, decimal?>("181", GetBetween(p3,75,181,281)));
			//n281 = GetBetween(p3,75, 281, 381);
			Values.Add(new KeyValuePair<string, decimal?>("281", GetBetween(p3,75,281,381)));
			//n481 = GetBetween(p3,75, 481, 581);
			Values.Add(new KeyValuePair<string, decimal?>("481", GetBetween(p3,75,481,581)));
			//n185 = GetBetween(p3,79, 185, 285);
			Values.Add(new KeyValuePair<string, decimal?>("185", GetBetween(p3,79,185,285)));
			//n285 = GetBetween(p3,79, 285, 385);
			Values.Add(new KeyValuePair<string, decimal?>("285", GetBetween(p3,79,285,385)));
			//n485 = GetBetween(p3,79, 485, 585);
			Values.Add(new KeyValuePair<string, decimal?>("485", GetBetween(p3,79,485,585)));

			//if(n102==null && n202==null && n181==null && n281==null && n481==null && n185==null && n285==null && n485==null)
			//{
			//	error = true;
			//	weirdE3 = true;
			//}
			if(Values.Where(x=>x.Value == null).Count() > 5)
			{
				error = true;
				weirdE3 = true;
			}

			reader = new PdfReader(path);
		}

		string GetAfm(string page)
		{
			string lineText = Functions.GetLine(page, 29);
			int i1No = lineText.IndexOf("020");
			if(i1No == -1)
			{
				return null;
			}
			int i2No = lineText.IndexOf(" 021");
			int length = i2No - i1No - 3;
			string n = lineText.Substring(i1No +3, length).Replace(" ","");
			return n;
		}

		decimal? GetSingle(string page,int line,int fNo)
		{
			string lineText = Functions.GetLine(page, line);
			int iNo = lineText.IndexOf(fNo.ToString());
			if(iNo == -1)
			{
				return null;
			}
			string n = lineText.Substring(iNo +fNo.ToString().Length).Replace(" ","").Replace(".","").Replace(",",".");
			try
			{
				decimal result = decimal.Parse(n);
				return result;
			}
			catch
			{
				return 0;
			}
		}

		decimal? GetBetween(string page,int line,int f1No,int f2No)
		{
			string lineText = Functions.GetLine(page, line);
			int i1No = lineText.IndexOf(f1No.ToString());
			if(i1No == -1)
			{
				return null;
			}
			int i2No = lineText.IndexOf(" "+f2No.ToString());
			int length = i2No - i1No - f1No.ToString().Length;
			string n = lineText.Substring(i1No +f1No.ToString().Length,length).Replace(" ","").Replace(".","").Replace(",",".");
			try
			{
				decimal result = decimal.Parse(n);
				return result;
			}
			catch
			{
				return 0;
			}
		}
	}
}
