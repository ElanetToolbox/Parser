﻿using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
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
        public string FileType { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public decimal WorkCycle { get; set; }
        public Dictionary<string,Rectangle> Rectangles => GetRectangles();

        public bool ParsingErrorExternal { get; set; }

        public void Scan()
        {
            WorkCycle = decimal.Parse(RectangleScan(Rectangles.Where(x => x.Key == "WorkCycle").Single().Value).Replace(".", ""),CultureInfo.GetCultureInfo("el-GR"));
            DateStart = DateTime.ParseExact(RectangleScan(Rectangles.Where(x => x.Key == "DateStart").Single().Value), @"dd/MM/yy", CultureInfo.InvariantCulture);
            DateEnd = DateTime.ParseExact(RectangleScan(Rectangles.Where(x => x.Key == "DateEnd").Single().Value), @"dd/MM/yy", CultureInfo.InvariantCulture);
            DateSubmitted = DateTime.ParseExact(RectangleScan(Rectangles.Where(x => x.Key == "DateSubmitted").Single().Value), @"dd/MM/yy", CultureInfo.InvariantCulture);
        }

        public void Test()
        {
            RectangleScan(Rectangles.Where(x => x.Key == "test").Single().Value);
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

        public Dictionary<string,Rectangle> GetRectangles()
        {
            Dictionary<string, Rectangle> list = new Dictionary<string, Rectangle>() 
            {
                { "WorkCycle",new Rectangle(0,375,200,15) },
                { "DateStart",new Rectangle(0,710,200,30) },
                { "DateEnd",new Rectangle(200,710,200,30) },
                { "DateSubmitted",new Rectangle(0,770,600,30) },
            };

            return list;
        }
    }
}