using iText;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Parser_Console.Classes;
using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser_Console
{
    public static class DebugFunctions
    {
        public static void ScanDocument(string path)
        {
            DocumentCollection c = new DocumentCollection();
            c.AddDocument(path);
        }

        public static void strDiff(string s1, string s2)
        {
            char c = (char)916;
            char[] a1 = s1.ToCharArray();
            char[] a2 = s2.ToCharArray();
            int d = 0;
            for (int i = 0; i < a1.Length; i++)
            {
                if(a1[i] == a2[i])
                {
                    d++;
                }
                else
                {
                    char c1 = a1[i];
                    char c2 = a2[i];
                }
            }
        }
        
        public static void CopyToDebugFolder(List<IDocument> collection,string folderName)
        {
            var copyFolder = @"T:\ToolboxStorage\Testing\Documents\" + folderName + @"\";
            foreach (var item in collection)
			{
				File.Copy(item.FilePath, copyFolder + System.IO.Path.GetFileName(item.FilePath));
			}
        }
        
        public static void LocScan(string path)
        {
            PdfReader reader = new PdfReader(path);
            PdfDocument doc = new PdfDocument(reader);
            ITextExtractionStrategy strat;
            for (int i = 1; i <= doc.GetNumberOfPages(); i++)
            {
                PdfPage d = doc.GetPage(i);
                string full = PdfTextExtractor.GetTextFromPage(d, new LocationTextExtractionStrategy());
                Rectangle t = d.GetPageSize();
                Rectangle z = d.GetPageSize();
                //z.SetY(-450);
                z.SetHeight(15);
                z.SetWidth(200);
                z.SetY(375);
                TextRegionEventFilter filter = new TextRegionEventFilter(z);
                FilteredTextEventListener list = new FilteredTextEventListener(new LocationTextExtractionStrategy(), filter);
                string half = PdfTextExtractor.GetTextFromPage(d, list);
            }
        }
    }
}
