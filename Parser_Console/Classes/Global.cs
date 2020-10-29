using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console.Classes
{
    public static class Global
    {
        public static Project_Collection Project_Collection;
        public static int current;
        public static string currentPath;
        public static string[] exitCodes;
    }

    public static class RegexPatterns
    {
        public static string PrintType1Header = @"Ο λογαριασμός μου {1,}Εφαρμογές TAXISnet {1,}Προσωπ.Πληρ/ση {1,}Αποσύνδεση";
        public static string PrintType2Header = @"\d{1,2}\d{1,2}\d{2} {1,}TaxisNET";
        public static string Kad = @"\d{8}( |\n)";
        public static string DateShortYear = @"\d{1,2}/\d{1,2}/\d{2}";
        public static string DateLongYear = @"\d{1,2}/\d{1,2}/\d{4}";
        public static string KadHeader = @"Κωδικός.*?∆ραστηριότητα.*?Είδος.*?Ημ/νία.*?έναρξης.*?Ημ/νία.*?διακοπής";
        public static string EstablishmentHeader = @"Αριθμός.*Είδος.*Τίτλος.*∆ιεύθυνση.";
        public static string EstablishmentEnd = @"Αναλυτικά Στοιχεία";
        public static string PostCode = @"ΤΚ: {1}\d{5}";

    }
}
