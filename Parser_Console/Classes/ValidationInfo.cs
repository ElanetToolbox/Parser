using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser_Console.Classes
{
    public class ValidationInfo
    {
        public string Afm { get; set; }
        public List<string> Kad { get; set; }
        public List<string> KadFormatted => Kad.Select(x=>FormatKad(x)).ToList();

        private string FormatKad(string kad)
        {
            string result = kad.Replace(".", "");
            while(result.Length < 8)
            {
                result += "0";
            }
            return result;
        }
    }
}
