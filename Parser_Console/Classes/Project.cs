using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Parser_Console.Classes
{
    public class Project
    {
        public string Code { get; set; }
        public string Prefix => Code.Substring(0, Code.IndexOf("-"));
        public string Region => Functions.GetRegionByPrefix(Prefix);
        public string ProjectPath { get; set; }
        public DateTime LastEdit { get; set; }
        public DocumentCollection Docs { get; set; }
        public List<Taxis> Taxis => Docs.Documents != null ? Docs.TaxisList : new List<Taxis>();
        public List<E3> E3s =>Docs.Documents != null ?  Docs.E3s : new List<E3>();
        public bool Complete => Taxis.Where(x=>x.DocType==0).Where(x => x.Complete).Any() && E3s.Where(x => x.Complete).Any();
        //public int TaxisCount => Docs.TaxisList.Count;
        //public int E3Count => Docs.E3s.Count;
        //public int E3_2019 => Docs.E3s.Where(x => x.Year == 2019).Count();
        //public bool Complete => CheckCompletion();
        //public bool ValidE3 => Docs.ValidE3s.Count > 0;
        //public bool ValidTaxis => Docs.ValidTaxisList.Count > 0;

        public bool FolderError { get; set; }

        public Project()
        {
        }

        public bool CheckCompletion()
        {
            if(Docs.ValidTaxisList.Count > 0 && Docs.ValidE3s.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ScanPath(string path)
        {
            ProjectPath = path;
			Code = Path.GetFileName(path);
            try
            {
                Functions.FixFolder(path);
            }
            catch 
            { 
                FolderError = true;
                return;
            }
            LastEdit = Directory.GetLastWriteTime(path);
            var pdfs = Directory.GetFiles(path, "*.pdf");
            Docs = new DocumentCollection();
            foreach (string pdf in pdfs)
            {
                //Functions.UploadDocument(Code, pdf);
                Docs.AddDocument(pdf);
                //try
                //{
                //    Docs.AddDocument(pdf);
                //}
                //catch { }
            }
        }

        public Upload CreateUpload()
        {
            Upload newUpload = new Upload();
            newUpload.ProjectFileId = Code;
            E3 correctE3 = Docs.E3s
                .Where(x => x.Year == 2019)
                .Where(x => !string.IsNullOrWhiteSpace(x.FormNumber))
                .SingleOrDefault();
            Taxis correctTaxis = Docs.TaxisList
                .Where(x => x.DocType == 0)
                .Where(x => DateTime.Compare(new DateTime(2018, 12, 31), x.StartDate) > 0)
                .SingleOrDefault();
            if(correctE3 != null)
            {
                newUpload.f102E32019  = correctE3.Values.Where(x => x.Key == "102").Single().Value.ToString();
                newUpload.f202E32019  = correctE3.Values.Where(x => x.Key == "202").Single().Value.ToString();
                newUpload.f181E32019  = correctE3.Values.Where(x => x.Key == "181").Single().Value.ToString();
                newUpload.f281E32019  = correctE3.Values.Where(x => x.Key == "281").Single().Value.ToString();
                newUpload.f481E32019  = correctE3.Values.Where(x => x.Key == "481").Single().Value.ToString();
                newUpload.f185E32019  = correctE3.Values.Where(x => x.Key == "185").Single().Value.ToString();
                newUpload.f285E32019  = correctE3.Values.Where(x => x.Key == "285").Single().Value.ToString();
                newUpload.f485E32019  = correctE3.Values.Where(x => x.Key == "485").Single().Value.ToString();
                newUpload.Turnover2019 = correctE3.Values.Where(x => x.Key == "500").Single().Value.ToString();
                newUpload.EBITDA2019  = correctE3.Values.Where(x => x.Key == "524").Single().Value.ToString();
            }
            else
            {
                if(Docs.E3s.Count == 0)
                {
                    newUpload.Log += "Δεν βρέθηκαν Ε3 σε αναγνώσιμη μορφή";
                }
                else if(Docs.E3s.Where(x=>x.Year == 2019).Count() == 0)
                {
                    newUpload.Log += "Δέν βρέθηκε Ε3 του 2019";
                }
                else if(Docs.E3s.Where(x=>!string.IsNullOrWhiteSpace(x.FormNumber)).Count() == 0)
                {
                    newUpload.Log += "Δέν βρέθηκε Ε3 του 2019 με αριθμό δήλωσης";
                }
            }
            if(correctTaxis != null)
            {
                newUpload.TaxCode = correctTaxis.Afm;
                newUpload.LegalName = correctTaxis.CompanyName;
                newUpload.FoundingDate = correctTaxis.StartDate.ToString("dd/MM/yyyy");
            }
            return newUpload;
        }
        
    }
}
