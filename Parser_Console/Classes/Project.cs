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
        public DocumentCollection Docs { get; set; }
        public List<Taxis> Taxis => Docs != null ? Docs.TaxisList.ToList() : new List<Taxis>();
        public List<Taxis> TaxisCompany => Docs != null ? Docs.TaxisList.Where(x=>x.DocType == 0).ToList() : new List<Taxis>();
        public List<Taxis> TaxisEstablishment  => Docs != null ? Docs.TaxisList.Where(x=>x.DocType == 1).ToList() : new List<Taxis>();
        public List<E3> E3s =>Docs != null ?  Docs.E3s : new List<E3>();
        public List<F2> F2s =>Docs != null ?  Docs.F2s : new List<F2>();

        public bool Complete => TaxisCompany.Where(x=>x.DocType==0).Where(x => x.Complete).Any() && E3s.Where(x => x.Complete).Any();
        public bool CanUpload => FolderError == false && Docs.corruptDocuments.Count == 0;

        public bool HasTaxisEstablishments => TaxisEstablishment.Where(x=>x.Complete).Any();
        public bool HasTaxisCompany => TaxisCompany.Where(x=>x.Complete).Any();
        public bool HasE3 => E3s.Where(x => x.Complete).Where(x => x.Year == 2019).Any();

        public bool HasNothing => HasE3 == false && HasTaxisCompany == false;
        public bool HasWeirdE3 => E3s.Where(x => x.Year == 2019).Where(x => !x.Complete).Any();

        public bool FolderError { get; set; }
        public bool NotPdf { get; set; } 

        public bool Removed { get; set; }
        public bool Uploaded { get; set; }

        public Upload Upload { get; set; }

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

            string zipPath = Functions.CompressFolder(path);
            Functions.UploadFileSCP(Code, Path.GetDirectoryName(zipPath));
            Directory.Delete(Path.GetDirectoryName(zipPath), true);

            try
            {
                Functions.ExtractFiles(path);
            }
            catch { FolderError = true; }

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
            try
            {
                string afmtest = vInfo.Afm;
            }
            catch
            {
                Removed = true;
            }
            string Afm = vInfo.Afm;
            List<string> DeclaredKads = vInfo.KadFormatted;
            List<Tuple<string, DateTime>> ValidKads = new List<Tuple<string, DateTime>>();

            Upload newUpload = new Upload();
            newUpload.Log = "";
            newUpload.ProjectFileId = Functions.Greekify(Code);

            E3 correctE3 = GetCorrectE3(Afm, newUpload);

            Taxis correctTaxisCompany = GetCorrectTaxis(Afm, newUpload);

            F2_Info f2info2020 = GetCorrectF2(Afm, 2020, newUpload);
            F2_Info f2info2019 = GetCorrectF2(Afm, 2019, newUpload);

            if (correctE3 != null && correctE3.Afm == Afm)
            {
                newUpload.f102E32019 = correctE3.Values.Where(x => x.Key == "102").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f202E32019 = correctE3.Values.Where(x => x.Key == "202").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f181E32019 = correctE3.Values.Where(x => x.Key == "181").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f281E32019 = correctE3.Values.Where(x => x.Key == "281").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f481E32019 = correctE3.Values.Where(x => x.Key == "481").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f185E32019 = correctE3.Values.Where(x => x.Key == "185").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f285E32019 = correctE3.Values.Where(x => x.Key == "285").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.f485E32019 = correctE3.Values.Where(x => x.Key == "485").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.Turnover2019 = correctE3.Values.Where(x => x.Key == "500").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.EBITDA2019 = correctE3.Values.Where(x => x.Key == "524").Single().Value.Value.ToString("N2").Replace(",", "");
                newUpload.KadSuggestBiggest = new KadUpload();
                newUpload.KadSuggestBiggest.a = Functions.Kadify(correctE3.KadIncome);
                newUpload.KadSuggestMain = new KadUpload();
                newUpload.KadSuggestMain.a = Functions.Kadify(correctE3.KadMain);
            }

            if (correctTaxisCompany != null)
            {
                newUpload.TaxCode = correctTaxisCompany.Afm;
                newUpload.LegalName = correctTaxisCompany.CompanyName;
                newUpload.FoundingDate = correctTaxisCompany.StartDate.ToString("dd/MM/yyyy");
                newUpload.PostCode = correctTaxisCompany.PostCode;
                if (correctE3 != null)
                {
                    newUpload.KadSuggestBiggest.b = correctTaxisCompany.Kads.Where(x => x.Code == correctE3.KadIncome && x.DateEnd == null).Single().DateStart.ToString("dd/MM/yyyy");
                    newUpload.KadSuggestBiggest.c = correctTaxisCompany.PostCode;
                    newUpload.KadSuggestMain.b = correctTaxisCompany.Kads.Where(x => x.Code == correctE3.KadMain && x.DateEnd == null).SingleOrDefault().DateStart.ToString("dd/MM/yyyy");
                    newUpload.KadSuggestMain.c = correctTaxisCompany.PostCode;
                }

                if(correctTaxisCompany.Establishments == null)
                {
                    return newUpload;
                }

                if ( correctTaxisCompany.Establishments.Count > 0)
                {
                    if (TaxisEstablishment.Where(x => x.Complete).Count() > 0)
                    {
                        newUpload.KadImplementationPlaces = new List<KadUpload>();
                        foreach (var est in TaxisEstablishment.Where(x => x.Complete))
                        {
                            foreach (var kad in est.Kads)
                            {
                                KadUpload newKad = new KadUpload();
                                newKad.a = Functions.Kadify(kad.Code);
                                newKad.b = kad.DateStart.ToString("dd/MM/yyyy");
                                newKad.c = est.PostCode;
                                newUpload.KadImplementationPlaces.Add(newKad);
                            }
                        }
                        newUpload.Log += "Τα στοιχεία εγκαταστάσεων εσωτερικού (μέσω TaxisNET) αντλήθηκαν από τα αρχεία "
                            + string.Join(", ", TaxisEstablishment.Where(x => x.Complete).Select(x => Path.GetFileName(x.FilePath)))
                            + "\n";
                    }
                    else
                    {
                        newUpload.Log += "Δεν βρέθηκε εκτύπώση εγκαταστάσεων εσωτερικού (μέσω TaxisNET) σε αναγνώσιμη μορφή" + "\n";
                    }
                }
            }

            if(f2info2019 != null)
            {
                newUpload.Turnover2019A = f2info2019.WorkCycle.ToString("N2").Replace(",", "");
                if(f2info2019.FilePaths.Count == 1)
                {
                    newUpload.Log += "Τα στοιχεία Φ2 2019 αντλήθηκαν από το αρχείο " + f2info2019.FilePaths.First() + "\n";
                }
                else
                {
                    newUpload.Log += "Τα στοιχεία Φ2 2019 αντλήθηκαν από τα αρχεία " + string.Join(",", f2info2019.FilePaths) + "\n";
                }
            }

            if(f2info2020 != null)
            {
                newUpload.Turnover2020B = f2info2020.WorkCycle.ToString("N2").Replace(",", "");
                if(f2info2020.FilePaths.Count == 1)
                {
                    newUpload.Log += "Τα στοιχεία Φ2 2020 αντλήθηκαν από το αρχείο " + f2info2020.FilePaths.First() + "\n";
                }
                else
                {
                    newUpload.Log += "Τα στοιχεία Φ2 2020 αντλήθηκαν από τα αρχεία " + string.Join(",", f2info2020.FilePaths) + "\n";
                }
            }


            return newUpload;
        }

        private Taxis GetCorrectTaxis(string Afm, Upload newUpload)
        {
            IEnumerable<Taxis> possibleTaxis = Docs.TaxisList.Where(x => x.Complete).Where(x => x.DocType == 0);
            if (possibleTaxis.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε εκτύπωση προσωποποιημένης πληροφόρησης (μέσω TaxisNET) σε αναγνώσιμη μορφή" + "\n";
                return null;
            }
            possibleTaxis = possibleTaxis.Where(x => x.Afm == Afm);
            if(possibleTaxis.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμη εκτύπωση προσωποποιημένης πληροφόρησης (μέσω TaxisNET) με το ΑΦΜ της εταιρίας" + "\n";
                return null;
            }
            possibleTaxis = possibleTaxis.Where(x => x.Region == Region);
            if(possibleTaxis.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμη εκτύπωση προσωποποιημένης πληροφόρησης (μέσω TaxisNET) με το ΑΦΜ της εταιρίας στην περιφέρεια υλοποίησης" + "\n";
                return null;
            }

            Taxis correctTaxisCompany = possibleTaxis.FirstOrDefault();
            newUpload.Log += "Τα στοιχεία προσωποποιημένης πληροφόρησης (μέσω TaxisNET) αντλήθηκαν από το αρχείο " + Path.GetFileName(correctTaxisCompany.FilePath) + "\n";

            return correctTaxisCompany;
        }

        private E3 GetCorrectE3(string Afm, Upload newUpload)
        {
            IEnumerable<E3> possibleE3s = Docs.E3s.Where(x => x.Complete);
            if(possibleE3s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε Ε3 σε αναγνώσιμη μορφή" + "\n";
                return null;
            }
            possibleE3s = possibleE3s.Where(x => x.Year == 2019);
            if(possibleE3s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμος Ε3 για το φορολογικό έτος 2019" + "\n";
                return null;
            }
            possibleE3s = possibleE3s.Where(x => x.Afm == Afm);
            if(possibleE3s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμος Ε3 για το φορολογικό έτος 2019 με το ΑΦΜ της εταιρίας" + "\n";
                return null;
            }
            possibleE3s = possibleE3s.Where(x => !string.IsNullOrWhiteSpace(x.FormNumber));
            if (possibleE3s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμος Ε3 για το φορολογικό έτος 2019 με το ΑΦΜ της εταιρίας και αριθμό υποβολής" + "\n";
                return null;
            }

            E3 correctE3 = possibleE3s.OrderByDescending(x => x.FormNumber).FirstOrDefault();
            newUpload.Log += "Τα οικονομικά στοιχεία (Ε3) αντλήθηκαν από το αρχείο " + Path.GetFileName(correctE3.FilePath) + "\n";

            return correctE3;
        }

        private F2_Info GetCorrectF2(string Afm, int year,Upload newUpload)
        {
            IEnumerable<F2> possibleF2s = Docs.F2s.Where(x => x.Year == year);
            if(possibleF2s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμος Φ2 για το φορολογικό έτος " + year + "\n";
                return null;
            }
            possibleF2s = possibleF2s.Where(x => x.Afm == Afm);
            if(possibleF2s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμος Φ2 για το φορολογικό έτος " + year + " με το ΑΦΜ της εταιρίας" + "\n";
                return null;
            }
            possibleF2s = possibleF2s.Where(x => !string.IsNullOrWhiteSpace(x.FormNumber));
            if (possibleF2s.Count() == 0)
            {
                newUpload.Log += "Δεν βρέθηκε αναγνώσιμος Φ2 για το φορολογικό έτος " + year + " με το ΑΦΜ της εταιρίας και αριθμό υποβολής" + "\n";
                return null;
            }

            F2_Info info = GetF2Info(year, possibleF2s);
            newUpload.Log += info.Log + "\n";
            //if (info != null)
            //{
            //}
            return info;
        }

        public void UploadProject()
        {
            if (!CanUpload || Uploaded || Removed)
            {
                return;
            }
            //Upload newUpload = CreateUpload();
            Upload = CreateUpload();
            Functions.UploadStream(Functions.Greekify(Code), Encoding.UTF8.GetBytes(Upload.Log), "report.txt");
            Upload.UploadToCloud(Functions.Greekify(Code));
            Uploaded = true;
        }

        public F2_Info GetF2Info(int year, IEnumerable<F2> f2s)
        {
            F2_Info info = new F2_Info();
            info.Year = year;
            info.FilePaths = new List<string>();
            List<F2> correctF2s = new List<F2>();
            if(f2s.Count() == 1)
            {
                var f2 = f2s.First();
                if(f2.DateStart.Month == 4 && f2.DateEnd.Month == 6)
                {
                    correctF2s.Add(f2);
                }
                info.FilePaths.Add(f2.FileName);
                info.WorkCycle = f2.WorkCycle;
                return info;
            }
            if(f2s.Count() == 3)
            {
                f2s = f2s.OrderBy(x => x.DateStart);
                if(f2s.First().DateStart.Month == 4 && f2s.Last().DateEnd.Month == 6)
                {
                    correctF2s.AddRange(f2s);
                }
                info.FilePaths = correctF2s.Select(x => x.FilePath).ToList();
                info.WorkCycle = correctF2s.Sum(x => x.WorkCycle);
                return info;
            }
            return null;
        }

    }
}
