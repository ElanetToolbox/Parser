﻿using System;
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
        public DocumentCollection Docs { get; set; }
        public List<Taxis> Taxis => Docs != null ? Docs.TaxisList.ToList() : new List<Taxis>();
        public List<Taxis> TaxisCompany => Docs != null ? Docs.TaxisList.Where(x=>x.DocType == 0).ToList() : new List<Taxis>();
        public List<Taxis> TaxisEstablishment  => Docs != null ? Docs.TaxisList.Where(x=>x.DocType == 1).ToList() : new List<Taxis>();
        public List<E3> E3s =>Docs != null ?  Docs.E3s : new List<E3>();

        public bool Complete => TaxisCompany.Where(x=>x.DocType==0).Where(x => x.Complete).Any() && E3s.Where(x => x.Complete).Any();
        public bool CanUpload => FolderError == false && Docs.corruptDocuments.Count == 0;

        public bool HasTaxisEstablishments => TaxisEstablishment.Where(x=>x.Complete).Any();
        public bool HasTaxisCompany => TaxisCompany.Where(x=>x.Complete).Any();
        public bool HasE3 => E3s.Where(x => x.Complete).Where(x => x.Year == 2019).Any();

        public bool HasNothing => HasE3 == false && HasTaxisCompany == false;
        public bool HasWeirdE3 => E3s.Where(x => x.Year == 2019).Where(x => !x.Complete).Any();

        public bool FolderError { get; set; }
        public bool NotPdf { get; set; }

        public bool Uploaded { get; set; }

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

            //string zipPath = Functions.CompressFolder(path);
            //Functions.UploadFileSCP(Code,Path.GetDirectoryName(zipPath));
            //Directory.Delete(Path.GetDirectoryName(zipPath),true);

            //Functions.ExtractFiles(path);

            var pdfs = Directory.GetFiles(path, "*.pdf",SearchOption.AllDirectories);
            Docs = new DocumentCollection();
            foreach (string pdf in pdfs)
            {
                Docs.AddDocument(pdf);
            }
        }

        public Upload CreateUpload()
        {
            ValidationInfo vInfo = Functions.GetAfmFromCode(Functions.Greekify(Code));
            string Afm = vInfo.Afm;
            List<string> DeclaredKads = vInfo.KadFormatted;
            List<Tuple<string, DateTime>> ValidKads = new List<Tuple<string, DateTime>>();

            Upload newUpload = new Upload();
            newUpload.Log = "";
            newUpload.ProjectFileId = Functions.Greekify(Code);

            E3 correctE3 = Docs.E3s
                .Where(x=>x.Complete)
                .Where(x => x.Year == 2019)
                .Where(x => !string.IsNullOrWhiteSpace(x.FormNumber))
                .OrderByDescending(x=>x.FormNumber)
                .FirstOrDefault();

            Taxis correctTaxisCompany = Docs.TaxisList
                .Where(x => x.Complete)
                .Where(x => x.Afm == Afm)
                .Where(x => x.DocType == 0)
                .Where(x => x.Region == Region)
                .Where(x => DateTime.Compare(new DateTime(2018, 12, 31), x.StartDate) > 0)
                .SingleOrDefault();

            if (correctE3 != null && correctE3.Afm == Afm)
            {
                newUpload.f102E32019  = correctE3.Values.Where(x => x.Key == "102").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f202E32019  = correctE3.Values.Where(x => x.Key == "202").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f181E32019  = correctE3.Values.Where(x => x.Key == "181").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f281E32019  = correctE3.Values.Where(x => x.Key == "281").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f481E32019  = correctE3.Values.Where(x => x.Key == "481").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f185E32019  = correctE3.Values.Where(x => x.Key == "185").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f285E32019  = correctE3.Values.Where(x => x.Key == "285").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.f485E32019  = correctE3.Values.Where(x => x.Key == "485").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.Turnover2019 = correctE3.Values.Where(x => x.Key == "500").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.EBITDA2019  = correctE3.Values.Where(x => x.Key == "524").Single().Value.Value.ToString("N2").Replace(",","");
                newUpload.KadSuggestBiggest = new KadUpload();
                newUpload.KadSuggestBiggest.KadEnumID = Functions.Kadify(correctE3.KadIncome);
                newUpload.KadSuggestMain =new KadUpload();
                newUpload.KadSuggestMain.KadEnumID = Functions.Kadify(correctE3.KadMain);
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
                else if(correctE3.Afm != Afm)
                {
                    newUpload.Log += "Δέν βρέθηκε Ε3 με το ΑΦΜ της εταιρίας";
                }
            }

            if (correctTaxisCompany != null)
            {
                newUpload.TaxCode = correctTaxisCompany.Afm;
                newUpload.LegalName = correctTaxisCompany.CompanyName;
                newUpload.FoundingDate = correctTaxisCompany.StartDate.ToString("dd/MM/yyyy");
                newUpload.PostCode = correctTaxisCompany.PostCode;
                if (correctE3 != null)
                {
                    newUpload.KadSuggestBiggest.StartDate = correctTaxisCompany.Kads.Where(x => x.Code == correctE3.KadIncomeClean && x.DateEnd == null).Single().DateStart.ToString("dd/MM/yyyy");
                    newUpload.KadSuggestBiggest.PostCode = correctTaxisCompany.PostCode;
                    newUpload.KadSuggestMain.StartDate = correctTaxisCompany.Kads.Where(x => x.Code == correctE3.KadMainClean && x.DateEnd == null).SingleOrDefault().DateStart.ToString("dd/MM/yyyy");
                    newUpload.KadSuggestMain.PostCode = correctTaxisCompany.PostCode;
                }
            }

            if(TaxisEstablishment.Where(x=>x.Complete).Count() > 0)
            {
                newUpload.KadImplementationPlaces = new List<KadUpload>();
                foreach (var est in TaxisEstablishment.Where(x=>x.Complete))
                {
                    foreach (var kad in est.Kads)
                    {
                        KadUpload newKad = new KadUpload();
                        newKad.KadEnumID = Functions.Kadify(kad.Code);
                        newKad.StartDate = kad.DateStart.ToString("dd/MM/yyyy");
                        newKad.PostCode = est.PostCode;
                        newUpload.KadImplementationPlaces.Add(newKad);
                    }

                }
            }
            string test = newUpload.ToJsonString();
            return newUpload;
        }

        public void UploadProject()
        {
            if (!CanUpload)
            {
                return;
            }
            Upload newUpload = CreateUpload();
            Functions.UploadStream(Code, Encoding.UTF8.GetBytes(newUpload.Log), "report.txt");
            newUpload.UploadToCloud(Functions.Greekify(Code));
            Uploaded = true;
        }

    }
}
