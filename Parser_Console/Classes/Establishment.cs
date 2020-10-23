using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console.Classes
{
    public class Establishment
    {
        public int Number { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public List<string> Types => new List<string> 
        { "ΥΠΟΚΑΤΑΣΤΗΜΑ", 
            "ΑΠΟΘΗΚΗ", 
            "ΓΡΑΦΕΙΟ", 
            "ΕΡΓΑΣΤΗΡΙΟ",
            "ΕΚΘΕΣΗ ΠΡΟΣΩΡΙΝΗ", 
            "ΕΚΘΕΣΗ ΜΟΝΙΜΗ", 
            "ΕΡΓΟΤΑΞΙΟ", 
            "ΕΡΓΟΣΤΑΣΙΟ", 
            "ΧΩΡΟΣ\nΣΥΓΚΕΝΤΡΩΣΗΣ,ΕΠΕΞΕΡΓΑΣΙΑΣ,ΣΥΣΚΕΥΑΣΙΑΣ\nΑΓΑΘΩΝ\n", 
            "ΑΚΙΝΗΤΟ ΑΡΘΡΟΥ 6\nΚΩ∆ΙΚΑ ΦΠΑ\n",
            "ΥΠΟ ΚΑΤΑΣΚΕΥΗ \nΕΓΚΑΤΑΣΤΑΣΗ \nΕΣΩΤΕΡΙΚΟΥ\n",
            "ΕΝΟΙΚΙΑΖΟΜΕΝΑ\nΕΠΙΠΛΩΜΕΝΑ\n∆ΩΜΑΤΙΑ ΕΩΣ ΕΠΤΑ\n",
            "ΕΝΟΙΚΙΑΖΟΜΕΝΑ \nΕΠΙΠΛΩΜΕΝΑ \n∆ΩΜΑΤΙΑ ΕΩΣ \nΕΠΤΑ\n",
            "ΥΠΟ ΚΑΤΑΣΚΕΥΗ\nΕΓΚΑΤΑΣΤΑΣΗ\nΕΣΩΤΕΡΙΚΟΥ\n",
            "ΥΠΟ ΑΝΑΣΤΟΛΗ\nΑΚΙΝΗΤΟ ΤΟΥ\nΑΡΘΡΟΥ 6 ΤΟΥ\nΚΩ∆ΙΚΑ ΦΠΑ\n",
        };

        public void Create(string text)
        {
            int nlIndex = text.IndexOf("\n");
            int wsIndex = text.IndexOf(" ");
            int aIndex = 0;
            if(nlIndex == -1)
            {
                aIndex = wsIndex;
            }
            else
            {
                aIndex = Math.Min(nlIndex, wsIndex);
            }
            string numberString = text.Substring(0, aIndex);
            Number = Int32.Parse(numberString);
            text = text.Substring(aIndex).Trim();
            var typePair = Functions.GetIndexFromList(text,Types);
            Type = typePair.Value;
            Address = text.Replace(typePair.Value,"").Replace("\n"," ").Trim();
        }

        //private KeyValuePair<int,string> GetTypeIndex(string text)
        //{
        //    foreach (var type in Types)
        //    {
        //        int index = text.IndexOf(type);
        //        if(index != -1)
        //        {
        //            return new KeyValuePair<int, string>(index,type);
        //        }
        //    }
        //    return new KeyValuePair<int, string>(-1,"");
        //}

    }
}
