using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Parser_Console.Classes;
using RestSharp;
using SharpCompress.Archives.Rar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Parser_Console
{
	public static class Functions
	{
		public static string GetLine(string text, int lineNo)
		{
            string[] lines = text.Replace("\r","").Split('\n');
            if (lineNo > 0)
            {
                return lines.Length >= lineNo ? lines[lineNo - 1] : null;
            }
            else
            {
                lineNo = Math.Abs(lineNo);
                return lines.Length >= lineNo ? lines[lines.Length - lineNo] : null;
            }
		}
        
        public static string ClearLines(string text,int lineCount)
        {
            List<string> lines = text.Replace("\r","").Split('\n').ToList();
            int times = Math.Abs(lineCount);
            for (int i = 0; i < times; i++)
            {
                if (lineCount > 0)
                {
                    lines.RemoveAt(0);
                }
                else
                {
                    lines.RemoveAt(lines.Count - 1);
                }

            }
            return string.Join("\n", lines);
        }

        public static string GetLineByText(string text,string searchText)
        {
            //text = CleanWeirdChars(text);
            char[] st = searchText.ToCharArray();
            string[] lines = text.Replace("\r","").Split('\n');
            return lines.Where(x => x.Contains(searchText)).First();
        }

        public static void SaveToFile(Project_Collection projects,string path)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Serialize(writer, projects);
            }
            string data = Convert.ToBase64String(ms.ToArray());
            File.WriteAllText(path, data);
        }

        public static Project_Collection LoadFromFile(string path)
        {
            Project_Collection projects = new Project_Collection();
            string contents = File.ReadAllText(path);
            byte[] data = Convert.FromBase64String(contents);

            MemoryStream ms = new MemoryStream(data);
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                projects = serializer.Deserialize<Project_Collection>(reader);
            }
            return projects;
        }

        public static bool IsAllDigits(string s)
        {
            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public static string CleanWeirdChars(string text)
        {
            //text = text.Replace((char)8710, (char)916);//perierga delta
            text = text.Replace((char)916, (char)8710);//perierga delta
            text = text.Replace((char)160, (char)32);//perierga kena
            text = text.Replace((char)181, (char)956);//perierga μ
            text = text.Replace((char)173, (char)45);//perierga -
            return text;
        }

        public static List<DateTime> GetDatesInText(string text)
        {
            List<DateTime> result = new List<DateTime>();
            Regex regex = new Regex(RegexPatterns.DateShortYear);
            if (!regex.IsMatch(text))
            {
                regex = new Regex(RegexPatterns.DateLongYear);
            }
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                try
                {
                    result.Add(DateTime.ParseExact(match.Value, "d/M/yy", CultureInfo.InvariantCulture));
                }
                catch
                {
                    result.Add(DateTime.ParseExact(match.Value, "M/d/yy", CultureInfo.InvariantCulture));
                }
            }
            return result;
        }

        public static KeyValuePair<int,string> GetIndexFromList(string text,List<string> list)
        {
            foreach (string item in list)
            {
                int index = text.IndexOf(item);
                if(index != -1)
                {
                    return new KeyValuePair<int, string>(index,item);
                }
            }
            return new KeyValuePair<int, string>(-1,"");
        }

        public static void UploadStream(string Code,byte[] array,string name,string folder = "")
        {
            var client = new RestClient("https://api.elanet.gr/wp-json/covid-app/v1/projects/pdffiles/" + Code);
            client.Timeout = -1;
            var request = new RestRequest(Method.PUT);
            if (string.IsNullOrEmpty(folder))
            {
                request.AddFile("upload", array, name);
            }
            else
            {
                request.AddFile("upload", array, folder + "/" + name);
            }
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
        }

        public static bool IsSameDate(DateTime d1,DateTime d2)
        {
            if(d1.Year == d2.Year)
            {
                if(d1.Month == d2.Month)
                {
                    if (d1.Day == d2.Day)
                    {
                        if (d1.Day == d2.Day)
                        {
                            if (d1.Hour == d2.Hour)
                            {
                                if (d1.Minute == d2.Minute)
                                {
                                    if (d1.Second == d2.Second)
                                    {
                                        return true;
                                    }
                                }

                            }

                        }
                    }
                }
            }
            return false;
        }

        public static void UnzipFiles2(string path)
        {
            var zips = Directory.GetFiles(path, "*.zip");
            var rars = Directory.GetFiles(path, "*.rar");
            zips = zips.Concat(rars).ToArray();
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var zip in zips)
            {
                string zipName = Path.GetFileNameWithoutExtension(zip);
                ZipFile.ExtractToDirectory(zip, path + @"/" + zipName, Encoding.GetEncoding(737));
                File.Delete(zip);
            }
        }

        public static void ExtractFiles(string path)
        {
            var zips = Directory.GetFiles(path, "*.zip");
            var rars = Directory.GetFiles(path, "*.rar");
            UnZip(path, zips);
            UnRarArray(path, rars);
        }

        private static void UnZip(string path, string[] zips)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            foreach (var zip in zips)
            {
                string zipName = Path.GetFileNameWithoutExtension(zip);
                try
                {
                    ZipFile.ExtractToDirectory(zip, path + @"/" + zipName, Encoding.GetEncoding(737));
                }
                catch { }
            }
        }

        private static void UnRarArray(string path,string[] rars)
        {
            foreach (var rar in rars)
            {
                UnRar(rar, path);
            }
        }

        public static void UnRar(string source,string destinationFolder)
        {
            string rarName = Path.GetFileNameWithoutExtension(source);
            destinationFolder = destinationFolder + @"\" + rarName;
            if (Directory.Exists(destinationFolder))
            {
                Directory.Delete(destinationFolder, true);
            }
            string arguments = string.Format(@"x -s ""{0}"" *.* ""{1}\""", source, destinationFolder);
            Process.Start(@"C:\Program Files\WinRAR\UnRAR.exe", arguments);
        }

        public static string CompressFolder(string path)
        {
            Directory.CreateDirectory(@"C:\Users\chatziparadeisis.i\Documents\covid\tmp\" + Path.GetFileName(path));
            string zipFile = @"C:\Users\chatziparadeisis.i\Documents\covid\tmp\" + Path.GetFileName(path) + @"\documents.zip";
            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }
            ZipFile.CreateFromDirectory(path, zipFile);
            return zipFile;
        }

        public static string GetRegionByPrefix(string prefix)
        {
            prefix = Greekify(prefix);
            switch (prefix)
            {
                case "ΚΜΕ7":
                    return "Μακεδονία";
                case "ΑΤΤΕ3":
                    return "Αττική";
                case "ΒΑΡΕ6":
                    return "Βόρειο Αιγαίο";
                case "ΝΑΙΕ2":
                    return "Νότιο Αιγαίο";
                default:
                    return null;
            }
        }

        public static string  GetRegionByPostCode(string postCode)
        {
            switch (postCode.Substring(0,2))
            {
                case "10":
                case "11":
                case "12":
                case "13":
                case "14":
                case "15":
                case "16":
                case "17":
                case "18":
                case "19":
                case "80":
                    return "Αττική";
                case "50":
                case "51":
                case "52":
                case "53":
                case "54":
                case "55":
                case "56":
                case "57":
                case "58":
                case "59":
                case "60":
                case "61":
                case "62":
                case "63":
                case "64":
                case "65":
                case "66":
                    return "Μακεδονία";
                case "81":
                case "82":
                case "83":
                    return "Βόρειο Αιγαίο";
                case "84":
                case "85":
                    return "Νότιο Αιγαίο";
                default:
                    return null;
            }
        }

        public static bool GetKadEligibility(string kad)
        {
            while(kad.Substring(kad.Length-2) == "00")
            {
                kad = kad.Substring(0, kad.Length - 2);
            }
            var client = new RestClient("https://api.elanet.gr/wp-json/covid-app/v1/kad/");
            client.Encoding = Encoding.UTF8;
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", "__cfduid=d701eff1105ff2e9f0494fcc62073c6131601277950; LOhNClQmXjeGsv=eWZzKu2DNihQrV; xBowmpAyJ_=hJEAfOvB3G; wlDxodLWRmQ=%5Bqr%5DnMAdlb.");
            request.AddParameter("term", kad);
            IRestResponse response = client.Execute(request);
            var result = JsonConvert.DeserializeObject(response.Content);
            if (result.ToString().Contains("ΝΑΙ"))
            {
                return true;
            }
            return false;
        }

        public static ValidationInfo GetAfmFromCode(string Code)
        {
            var client = new RestClient("https://api.elanet.gr/wp-json/covid-app/v1/projects/info/" + Greekify(Code));
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "__cfduid=d701eff1105ff2e9f0494fcc62073c6131601277950; LOhNClQmXjeGsv=eWZzKu2DNihQrV; xBowmpAyJ_=hJEAfOvB3G; wlDxodLWRmQ=%5Bqr%5DnMAdlb.");
            try
            {
                IRestResponse response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(response.Content).First();
                ValidationInfo vInfo = new ValidationInfo();
                vInfo.Afm = result.Where(x=>x.Key == "TaxCode").Single().Value;
                vInfo.Kad = result.Where(x=>x.Key == "KadPool").Single().Value.Split(",").ToList();
                return vInfo;
            }
            catch
            {
                return null;
            }
        }

        public static string GetPostCodeFromAddress(string address)
        {
            Regex r = new Regex(RegexPatterns.PostCode);
            Match m = r.Match(address);
            string result = m.Value.Replace("ΤΚ:","").Trim();
            return result;
        }

        public static void UploadFileSCP(string Code,string path)
        {
            string command = @"gcloud compute scp --recurse " + path + @" bitnami@api-vm:/opt/bitnami/apps/covidpdf/staging";
            var proc1 = new ProcessStartInfo();
            proc1.UseShellExecute = true;

            proc1.WorkingDirectory = @"C:\Windows\System32";

            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            proc1.Verb = "runas";
            proc1.Arguments = "/c " + command;
            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc1).WaitForExit();
        }

        public static string Greekify(string str)
        {
            return str.Replace("A", "Α")
                .Replace("T", "Τ")
                .Replace("E", "Ε")
                .Replace("B", "Β")
                .Replace("P", "Ρ")
                .Replace("N", "Ν")
                .Replace("I", "Ι");
        }

        public static string Englify(string str)
        {
            return str.Replace("Α","A")
                .Replace( "Τ","T")
                .Replace( "Ε","E")
                .Replace( "Β","B")
                .Replace( "Ρ","P")
                .Replace( "Ν","N")
                .Replace( "Ι","I");
        }

        public static string Kadify(string kad)
        {
            string result = "";
            int i = 1;
            foreach (var c in kad)
            {
                result += c;
                if (i % 2 == 0 && i != kad.Length)
                {
                    result += '.';
                }
                i++;
            }
            return result;
        }

    }
}
