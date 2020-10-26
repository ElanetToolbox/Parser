using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Parser_Console.Classes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

        public static void MakeFolderShallow(string folderPath)
        {
            string currentfolder = folderPath;
            int i;
            bool moved;
            var mainFolderDirectories = Directory.GetDirectories(currentfolder);
            while (mainFolderDirectories.Count() > 0)
            {
                foreach (var dir in mainFolderDirectories)
                {
                    var childFolderDirectories = Directory.GetDirectories(dir);
                    var childFolderFiles = Directory.GetFiles(dir);
                    foreach (var f in childFolderFiles)
                    {
                        moved = false;
                        string fileNameOriginal = Path.GetFileName(f);
                        string fileNameCurrent = fileNameOriginal;
                        i = 1;
                        do
                        {
                            try
                            {
                                File.Move(f, Path.Combine(currentfolder, fileNameCurrent));
                                moved = true;
                            }
                            catch
                            {
                                string xt = Path.GetExtension(fileNameCurrent);
                                string name = Path.GetFileNameWithoutExtension(fileNameCurrent);
                                fileNameCurrent = name + " - " + i.ToString() + xt;
                            }

                        } while (!moved);
                    }
                    foreach (var d in childFolderDirectories)
                    {
                        moved = false;
                        string dirNameOriginal = Path.GetFileName(d);
                        string dirNameCurrent = dirNameOriginal;
                        i = 1;
                        do
                        {
                            if (i == 1000)
                            {
                                throw new Exception();
                            }
                            try
                            {
                                Directory.Move(d,currentfolder+@"\"+dirNameCurrent);
                                moved = true;
                            }
                            catch(Exception ex)
                            {
                                dirNameCurrent = dirNameOriginal + " - " + i.ToString();
                                i++;
                            }

                        } while (!moved);
                    }
                    Directory.Delete(dir);
                }
                mainFolderDirectories = Directory.GetDirectories(currentfolder);

            }
        }

        public static void UnzipFiles(string path)
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

        public static void FixFolder(string path)
        {
            MakeFolderShallow(path);
            var zips = Directory.GetFiles(path, "*.zip");
            while (zips.Count() > 0)
            {
                UnzipFiles(path);
                MakeFolderShallow(path);
                zips = Directory.GetFiles(path, "*.zip");
            }
        }

        public static string GetRegionByPrefix(string prefix)
        {
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

        public static string GetAfmFromCode(string Code)
        {
            var client = new RestClient("https://api.elanet.gr/wp-json/covid-app/v1/projects/afm/" + Code);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cookie", "__cfduid=d701eff1105ff2e9f0494fcc62073c6131601277950; LOhNClQmXjeGsv=eWZzKu2DNihQrV; xBowmpAyJ_=hJEAfOvB3G; wlDxodLWRmQ=%5Bqr%5DnMAdlb.");
            try
            {
                IRestResponse response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>[]>(response.Content).First();
                return result.Where(x=>x.Key == "TaxCode").Single().Value;
            }
            catch
            {
                return "";
            }
        }

    }
}
