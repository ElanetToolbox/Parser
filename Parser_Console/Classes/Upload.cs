using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console.Classes
{
    public class Upload
    {
        public string ProjectFileId { get; set; }

        //E3
        public string f102E32019  { get; set; } //102
        public string f202E32019  { get; set; } //202
        public string f181E32019  { get; set; } //181
        public string f281E32019  { get; set; } //281
        public string f481E32019  { get; set; } //481
        public string f185E32019  { get; set; } //185
        public string f285E32019  { get; set; } //285
        public string f485E32019  { get; set; } //485
        public string Turnover2019 { get; set; } //500
        public string EBITDA2019  { get; set; } //524

        //Taxis
        public string TaxCode { get; set; }
        public string LegalName { get; set; }
        public string LegalType { get; set; }
        public string FoundingDate { get; set; }
        public string TaxOffice { get; set; }
        public string CountryOfResidence { get; set; }
        public string CivicCompartment  { get; set; }

        public string Log { get; set; }
    }
}
