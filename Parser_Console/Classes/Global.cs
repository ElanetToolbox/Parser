using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Parser_Console.Classes
{
    public static class Global
    {
        public static Project_Collection Project_Collection;
        public static int current;
        public static string currentPath;
        public static string[] exitCodes;

        public static Dictionary<string,Rectangle> GetRectangles()
        {
            Dictionary<string, Rectangle> list = new Dictionary<string, Rectangle>() 
            {
                { "WorkCycle",new Rectangle(140,842-470,42,15) },
                { "DateStart",new Rectangle(90,842-115,55,15) },
                { "DateEnd",new Rectangle(200,842-115,55,15) },
                { "DateSubmitted",new Rectangle(480,842-50,55,15) },
                { "Afm",new Rectangle(430,842-218,65,15) },
                { "test",new Rectangle(90,842-115,55,15) },
            };

            return list;
        }
    }

    public static class RegexCollection
    {
        public static Regex PrintType1Header = new Regex(@"Ο λογαριασμός μου {1,}Εφαρμογές TAXISnet {1,}Προσωπ.Πληρ/ση {1,}Αποσύνδεση");
        public static Regex PrintType2Header = new Regex(@"\d{1,2}\d{1,2}\d{2} {1,}TaxisNET");
        public static Regex Kad = new Regex(@"\d{6,8}( |\n)");
        public static Regex DateShortYear = new Regex(@"\d{1,2}/\d{1,2}/\d{2}");
        public static Regex DateLongYear = new Regex(@"\d{1,2}/\d{1,2}/\d{4}");
        public static Regex KadHeader = new Regex(@"Κωδικός.*?∆ραστηριότητα.*?Είδος.*?Ημ/νία.*?έναρξης.*?Ημ/νία.*?διακοπής",RegexOptions.Singleline);
        public static Regex EstablishmentHeader = new Regex(@"Αριθμός.*Είδος.*Τίτλος.*∆ιεύθυνση.", RegexOptions.Singleline);
        public static Regex EstablishmentEnd = new Regex(@"Αναλυτικά Στοιχεία");
        public static Regex PostCode = new Regex(@"ΤΚ: {1}\d{5}");

    }
}
