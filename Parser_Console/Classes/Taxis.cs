using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Parser_Console.Classes
{
    public class Taxis : IDocument
    {
        public string FilePath { get; set; }
		public string FileType => GetType().Name;
        public string Afm { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public string Address { get; set; }
        public string EstablishmentType { get; set; }
        public string Books { get; set; }
        public string Fpa { get; set; }
        public string FpaStatus { get; set; }
        public List<Kad> Kads { get; set; }
        public List<Establishment> Establishments { get; set; }
        public int PrintType { get; set; }
        public int DocType { get; set; }
        public DateTime PrintDate { get; set; }
        public bool ParsingErrorInternal { get; set; }
        public bool ParsingErrorExternal { get; set; }
        public bool NotKad { get; set; }
        public string MainKadCode => Kads != null && Kads.Count > 0 ? Kads.Where(x => x.Type == x.Types.First()).Select(x => x.Code).First() : "";

        //public Project Project => GetProject();
        public bool Complete => GetCompletion();

        private Project GetProject()
        {
            try
            {
                return Global.Project_Collection.Projects.Where(x => x.ProjectPath == Path.GetDirectoryName(FilePath)).Single();
            }
            catch
            {
                return null;
            }

        }

        private bool GetCompletion()
        {
            if(Afm != null && Kads != null /*&& Kads.Where(x=>x.Description != null).Any()*/)
            {
                if (Kads.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void Scan()
        {
            PdfReader reader = new PdfReader(FilePath);
			PdfDocument doc = new PdfDocument(reader);
            string pdfText = "";
            bool checkSecondPage = false;
            for (int i = 1; i <= doc.GetNumberOfPages(); i++)
            {
                string page = PdfTextExtractor.GetTextFromPage(doc.GetPage(i), new SimpleTextExtractionStrategy());
                page = Functions.CleanWeirdChars(page);
                if (i == 1)
                {
                    if(page.Length < 500)
                    {
                        checkSecondPage = true;
                    }
                    CheckHasKad(page);
                    if (NotKad && !checkSecondPage)
                    {
                        return;
                    }
                    SetType(page);
                    if (ParsingErrorInternal)
                    {
                        return;
                    }
                }
                if(i==2 && checkSecondPage)
                {
                    CheckHasKad(page);
                    if (NotKad)
                    {
                        return;
                    }
                }
                page = ClearPage(page);
                pdfText += page + "\n";
            }
            SetDocType(pdfText);
            if(DocType == 0)
            {
                ParseTextCompany(pdfText);
            }
            else if(DocType == 1)
            {
                ParseTextEstablishment(pdfText);
            }
        }

        public void ParseTextCompany(string pdfText)
        {
            GetAfmAndName(pdfText);
            StartDate = GetDateTimeByLabel(pdfText, "Ημ/νία Έναρξης").Value;
            Address = GetStringByLabel(pdfText, "∆ιεύθυνση άσκησης δραστηριότητας");
            Books = GetStringByLabel(pdfText, "Κατηγορία βιβλίων");
            Fpa = GetStringByLabel(pdfText, "Υπαγωγή ΦΠΑ");
            FpaStatus = GetStringByLabel(pdfText, "Καθεστώς ΦΠΑ");
            StopDate = GetDateTimeByLabel(pdfText, "Ημ/νία διακοπής");
            GetKads(pdfText);
            GetEstablishments(pdfText);
        }

        public void ParseTextEstablishment(string pdfText)
        {
            GetAfmAndName(pdfText);
            EstablishmentType = GetStringByLabel(pdfText, "Είδος εγκατάστασης");
            StartDate = GetDateTimeByLabel(pdfText, "Ημ/νία Έναρξης εγκατάστασης").Value;
            Address = GetStringByLabel(pdfText, "∆ιεύθυνση");
            GetKads(pdfText);
        }

        private void SetDocType(string pdfText)
        {
            int j = pdfText.IndexOf("∆ραστηριότητες Επιχείρησης");
            int k = pdfText.IndexOf("∆ραστηριότητες Εγκατάστασης");
            if (j != -1 && k !=-1)
            {
                DocType = 2;
            }
            else if(j!=-1)
            {
                DocType = 0;
            }
            else
            {
                DocType = 1;
            }
        }

        public void CheckHasKad(string page)
        {
            int i = page.IndexOf("∆ραστηριότητες");
            if (i == -1)
            {
                NotKad = true;
            }
            else
            {
                NotKad = false;
            }
        }

        private string ClearPage(string page)
        {
            string result = "";
            switch (PrintType)
            {
                case 0:
                    result = Functions.ClearLines(page, -2);
                    break;
                case 1:
                    result = Functions.ClearLines(page, 2);
                    break;
                default:
                    break;
            }
            return result;
        }

        public void SetType(string text)
        {
            //string finder = "  Ο λογαριασμός μου    Εφαρμογές TAXISnet    Προσωπ.Πληρ/ση    Αποσύνδεση  ";
            Regex regex1 = new Regex(RegexPatterns.PrintType1Header);
            Regex regex2 = new Regex(RegexPatterns.PrintType2Header);
            string line1 = Functions.GetLine(text, 1);
            //int fIndex = line1.IndexOf(finder);
            if (regex1.IsMatch(line1))
            {
                PrintType = 0;
                var date = Functions.GetDatesInText(Functions.GetLine(text,-1));
                if(date.Count ==0)
                {
                    date = Functions.GetDatesInText(Functions.GetLine(text,-2));
                }
                PrintDate = date.First();
            }
            else if(regex2.IsMatch(line1))
            {
                PrintType = 1;
                PrintDate = Functions.GetDatesInText(line1).First();
            }
            else
            {
                ParsingErrorInternal = true;
            }
        }


        public void GetKads(string text)
        {
            Kads = new List<Kad>();
            string allKadText = GetKadText(text);
            if(allKadText == null)
            {
                return;
            }
            Regex reg = new Regex(RegexPatterns.Kad);
            MatchCollection matches = reg.Matches(allKadText);
            allKadText = allKadText.Substring(allKadText.IndexOf(matches[0].Value));
            for (int i = 0; i < matches.Count; i++)
            {
                string singleKadText = "";
                if(i < matches.Count-1)
                {
                    string match1 = matches[i].Value;
                    string match2 = matches[i + 1].Value;
                    int index1 = allKadText.IndexOf(match1);
                    int index2 = allKadText.IndexOf(match2,index1+1);
                    singleKadText = allKadText.Substring(index1, index2);
                }
                else
                {
                    singleKadText = allKadText;
                }
                allKadText = allKadText.Replace(singleKadText, "");
                singleKadText = singleKadText.Trim();
                Kad newKad = new Kad();
                newKad.Create(singleKadText);
                Kads.Add(newKad);
            }
        }

        public void GetEstablishments(string text)
        {
            string allEstablishmentText = GetEstablishmentText(text);
            if(allEstablishmentText == null)
            {
                return;
            }
            Establishments = new List<Establishment>();
            Regex r = new Regex(RegexPatterns.EstablishmentEnd);
            MatchCollection matches = r.Matches(allEstablishmentText);
            for (int i = 1; i <= matches.Count; i++)
            {
                string startText = i.ToString();
                int startIndex = allEstablishmentText.IndexOf(startText);
                string endText = matches[i - 1].Value;
                int endIndex = allEstablishmentText.IndexOf(endText,startIndex);
                int textLength = endIndex - startIndex;
                string singleEstablishmentText = allEstablishmentText.Substring(startIndex, textLength);
                allEstablishmentText = allEstablishmentText.Replace(singleEstablishmentText, "");
                Establishment newEstablishment = new Establishment();
                newEstablishment.Create(singleEstablishmentText);
                Establishments.Add(newEstablishment);
            }
        }

        public string GetEstablishmentText(string text)
        {
            Regex r = new Regex(RegexPatterns.EstablishmentHeader,RegexOptions.Singleline);
            MatchCollection m = r.Matches(text);
            string endText = "Στοιχεία Έδρας Αλλοδαπής";
            if(m.Count == 0)
            {
                return null;
            }
            string startText = m[0].Value;
            int startIndex = text.IndexOf(startText);
            int endIndex = text.IndexOf(endText,startIndex + 1);
            int subLength = endIndex - startIndex - startText.Length;
            string result = text.Substring(startIndex + startText.Length, subLength);
            return result;
        }


        public string GetKadText(string text)
        {
            Regex r = new Regex(RegexPatterns.KadHeader,RegexOptions.Singleline);
            MatchCollection m = r.Matches(text);
            string endText = "";
            if (DocType == 0)
            {
                endText = "Στοιχεία Εγκαταστάσεων Εσωτερικού";
            }
            if(DocType == 1)
            {
                endText = "Επιστροφή";
            }
            
            if(m.Count == 0)
            {
                return null;
            }
            string startText = m[0].Value;
            int startIndex = text.IndexOf(startText);
            int endIndex = text.IndexOf(endText,startIndex + 1);
            int subLength = endIndex - startIndex - startText.Length;
            string result = text.Substring(startIndex + startText.Length, subLength);
            //string sub = text.Substring(startIndex + startText.Length);
            //return sub.Substring(0, sub.IndexOf(endText));
            return result;
        }

        public string GetStringByLabel(string text,string label)
        {
            string line = Functions.GetLineByText(text, label);
            return line.Replace(label, "").Trim();
        }

        public DateTime? GetDateTimeByLabel(string text,string label)
        {
            string dateLine = Functions.GetLineByText(text, label);
            string dateText = dateLine.Replace(label, "").Trim();
            try
            {
                return DateTime.ParseExact(dateText, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch { return null; }
        }

        public void GetStartDate(string text)
        {
            string dateFindText = "Ημ/νία Έναρξης";
            string dateLine = Functions.GetLineByText(text, dateFindText);
            string dateText = dateLine.Replace(dateFindText, "").Trim();
            StartDate = DateTime.ParseExact(dateText, "dd/MM/yyyy",CultureInfo.InvariantCulture);
        }

        public void GetAfmAndName(string text)
        {
            string afmText = "Α.Φ.Μ.:";
            string dashText = " - ";
            string afmLine = Functions.GetLineByText(text,afmText );
            string leftPart = afmLine.Substring(0, afmLine.IndexOf(dashText));
            string rightPart = afmLine.Substring(afmLine.IndexOf(dashText) + dashText.Length);
            Afm = leftPart.Replace(afmText,"").Trim();
            CompanyName = rightPart.Trim();
        }
    }
}
