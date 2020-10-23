using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Parser_Console.Classes
{
    public class Kad
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public List<string> Types => new List<string> { "Κύρια", "∆ευτερεύουσα", "Λοιπή","Βοηθητική"};

        public Kad()
        {

        }

        public void Create(string text)
        {
            int nlIndex = text.IndexOf("\n");
            if(nlIndex != -1)
            {
                CreateFromMulti(text);
            }
            else
            {
                CreateFromSingle(text);
            }
        }

        private void CreateFromSingle(string text)
        {
            Code = text.Substring(0, text.IndexOf(" ")).Trim();
            text = text.Replace(Code, "").Trim();
            var typePair = Functions.GetIndexFromList(text, Types);
            //int tIndex = text.IndexOf("Κύρια");
            //if(tIndex == -1)
            //{
            //    tIndex = text.IndexOf("∆ευτερεύουσα");
            //    if(tIndex == -1)
            //    {
            //        tIndex = text.IndexOf("Λοιπή");
            //    }
            //}
            Description = text.Substring(0, typePair.Key).Trim();
            text = text.Replace(Description, "").Trim();
            Type = text.Substring(0, text.IndexOf(" ")).Trim();
            text = text.Replace(Type, "").Trim();
            GetDates(text);
        }

        private void CreateFromMulti(string text)
        {
            Code = text.Substring(0, text.IndexOf("\n")).Trim();
            if (!Functions.IsAllDigits(Code))
            {
                Code = text.Substring(0, text.IndexOf(" ")).Trim();
            }
            text = text.Replace(Code, "").Trim();
            var typePair = Functions.GetIndexFromList(text, Types);
            int tIndex = typePair.Key;
            //int tIndex = text.IndexOf("Κύρια");
            //if(tIndex == -1)
            //{
            //    tIndex = text.IndexOf("∆ευτερεύουσα");
            //    if(tIndex == -1)
            //    {
            //        tIndex = text.IndexOf("Λοιπή");
            //    }
            //}
            Description = text.Substring(0, tIndex).Replace("\n"," ").Trim();
            text = text.Substring(tIndex);
            Type = text.Substring(0, text.IndexOf(" ")).Trim();
            text = text.Replace(Type, "").Trim();
            GetDates(text);
        }

        public void GetDates(string text)
        {
            Regex regex = new Regex(RegexPatterns.DateLongYear);
            MatchCollection matches = regex.Matches(text);
            DateStart = DateTime.ParseExact(matches[0].Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if(matches.Count == 2)
            {
                DateEnd = DateTime.ParseExact(matches[1].Value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

        }
    }
}
