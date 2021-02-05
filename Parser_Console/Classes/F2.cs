using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Newtonsoft.Json;
using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Parser_Console.Classes
{
    public class F2 : IDocument
    {
        public string FilePath { get; set; }
		public string FileType => GetType().Name;
        public string FileName => System.IO.Path.GetFileName(FilePath);
        public string Afm { get; set; }
        public string FormNumber { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public decimal WorkCycle { get; set; }
        public decimal Outflow { get; set; }
        [JsonIgnore]
        public Dictionary<string,Rectangle> Rectangles => Global.GetRectangles();
        public int Year => DateStart.Year;

        public bool ParsingErrorExternal { get; set; }

        public void Scan()
        {
            GetWorkCycle();
            GetOutflow();
            DateStart = DateTime.ParseExact(RectangleScan(Rectangles.Where(x => x.Key == "DateStart").Single().Value), @"dd/MM/yy", CultureInfo.InvariantCulture);
            DateEnd = DateTime.ParseExact(RectangleScan(Rectangles.Where(x => x.Key == "DateEnd").Single().Value), @"dd/MM/yy", CultureInfo.InvariantCulture);
            DateSubmitted = DateTime.ParseExact(RectangleScan(Rectangles.Where(x => x.Key == "DateSubmitted").Single().Value), @"dd/MM/yy", CultureInfo.InvariantCulture);
            Afm = RectangleScan(Rectangles.Where(x=>x.Key == "Afm").Single().Value);
            FormNumber = RectangleScan(Rectangles.Where(x=>x.Key == "FormNumber").Single().Value);
        }

        public void GetOutflow()
        {
            try
            {
                Outflow = decimal.Parse(RectangleScan(Rectangles.Where(x => x.Key == "Outflow").Single().Value).Replace(".", ""), CultureInfo.GetCultureInfo("el-GR"));
            }
            catch
            {
                Outflow = 0;
            }
        }

        public void GetWorkCycle()
        {
            try
            {
                WorkCycle = decimal.Parse(RectangleScan(Rectangles.Where(x => x.Key == "WorkCycle").Single().Value).Replace(".", ""), CultureInfo.GetCultureInfo("el-GR"));
            }
            catch
            {
                WorkCycle = 0;
            }

        }

        public string RectangleScan(Rectangle rect)
        {
            //593*842
            PdfReader reader = new PdfReader(FilePath);
            PdfDocument doc = new PdfDocument(reader);
            var z = doc.GetPage(1).GetPageSize();
            FilteredTextEventListener listener = new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(rect));
            string result = PdfTextExtractor.GetTextFromPage(doc.GetPage(1), listener);
            var r2 = PdfTextExtractor.GetTextFromPage(doc.GetPage(1),new LocationTextExtractionStrategy());
            return result;
        }

    }
}
