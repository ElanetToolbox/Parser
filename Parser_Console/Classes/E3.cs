using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Layout.Element;
using Parser_Console.Interfaces;

namespace Parser_Console.Classes
{
	public class E3 :IDocument
	{
		public string FilePath { get; set; }
		public string FileType => GetType().Name;
		public string FileName => Path.GetFileName(FilePath);
		public string Afm { get; set; }
		public int Year { get; set; }
		public string FormNumber { get; set; }
		public string KadMain { get; set; }
		public string KadIncome { get; set; }
		public List<KeyValuePair<string, decimal?>> Values;
		public bool ParsingErrorInternal { get; set; }
		public bool ParsingErrorExternal { get; set; }
        public DocumentCollection Collection { get; set; }
		public bool Complete => Afm != null && Values.Count == 10 && Year == 2019 && !Values.Where(x => x.Value == null).Any();

		public E3()
		{
		}

		public void ScanE3(string path)
		{
			FilePath = path;
			Values = new List<KeyValuePair<string, decimal?>>();
			PdfReader reader = new PdfReader(path);
			PdfDocument doc = new PdfDocument(reader);
			string p1 = PdfTextExtractor.GetTextFromPage(doc.GetFirstPage(), new LocationTextExtractionStrategy());

			Afm = GetAfm(p1);
			if(Afm == null)
			{
				ParsingErrorInternal = true;
				return;
			}
			Year = GetYear(p1, 7);
			FormNumber = GetFormNo(p1, 10);
			KadIncome = GetKadIncome(p1);
			KadMain = GetKadIncome(p1);

			string p2 = PdfTextExtractor.GetTextFromPage(doc.GetPage(2),new LocationTextExtractionStrategy());
			Values.Add(new KeyValuePair<string, decimal?>("102", GetSingle(p2,9,102)));
			Values.Add(new KeyValuePair<string, decimal?>("202", GetBetween(p2,20,202,302)));
			Values.Add(new KeyValuePair<string, decimal?>("500", GetSingle(p2,6,500)));
			Values.Add(new KeyValuePair<string, decimal?>("524", GetSingle(p2,56,524)));

			reader = new PdfReader(path);
			string p3 = PdfTextExtractor.GetTextFromPage(doc.GetPage(3),new LocationTextExtractionStrategy());
			Values.Add(new KeyValuePair<string, decimal?>("181", GetBetween(p3,75,181,281)));
			Values.Add(new KeyValuePair<string, decimal?>("281", GetBetween(p3,75,281,381)));
			Values.Add(new KeyValuePair<string, decimal?>("481", GetBetween(p3,75,481,581)));
			Values.Add(new KeyValuePair<string, decimal?>("185", GetBetween(p3,79,185,285)));
			Values.Add(new KeyValuePair<string, decimal?>("285", GetBetween(p3,79,285,385)));
			Values.Add(new KeyValuePair<string, decimal?>("485", GetBetween(p3,79,485,585)));
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

		string GetKadMain(string page)
		{
			string lineText = Functions.GetLine(page, 29);
			int i1No = lineText.IndexOf("021");
			if(i1No == -1)
			{
				return null;
			}
			int i2No = lineText.IndexOf(" 022");
			int length = i2No - i1No - 3;
			string n = lineText.Substring(i1No +3, length).Replace(" ","");
			return n;
		}

		string GetKadIncome(string page)
		{
			string lineText = Functions.GetLine(page, 29);
			int i1No = lineText.IndexOf("022");
			if(i1No == -1)
			{
				return null;
			}
			string n = lineText.Substring(i1No +3).Replace(" ","");
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
			int i2No = lineText.IndexOf(" "+f2No.ToString(),i1No);
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
		
		int GetYear(string page,int line)
		{
			string lineText = Functions.GetLine(page, line);
			string yearText = lineText.Replace("Φορολογικό έτος", "");

			return int.Parse(yearText);
		}

		string GetFormNo(string page,int line)
		{
			return Functions.GetLine(page, line).Substring(3).Replace(" ","");
		}

	}
}
