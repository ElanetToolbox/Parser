using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser_Console.Classes
{
    public class Project_Collection
    {
        public List<Project> Projects { get; set; }
        public List<IDocument> Docs => Projects.Where(x=>x.Docs != null).Select(x => x.Docs).SelectMany(x => x.Documents).ToList();
        public List<E3> E3s => Docs != null ? Docs.Where(x => x is E3).Select(x => (E3)x).ToList() : new List<E3>();
        public List<CorruptDocument> corruptDocuments => Docs != null ? Docs.Where(x => x is CorruptDocument).Select(x => (CorruptDocument)x).ToList() : new List<CorruptDocument>();
        public List<Taxis> Taxis => Docs != null ? Docs.Where(x => x is Taxis).Select(x => (Taxis)x).ToList() : new List<Taxis>();
        public List<Taxis> TaxisCompany => Docs != null ? Taxis.Where(x=>x.DocType==0).ToList() : new List<Taxis>();
        public List<Taxis> TaxisEst => Docs != null ? Taxis.Where(x=>x.DocType==1).ToList() : new List<Taxis>();
        public List<E3> ErrorE3s => E3s.Where(x => x.ParsingErrorExternal).ToList();
        public List<Taxis> ErrorTaxis => Taxis.Where(x => x.ParsingErrorExternal).ToList();
        public List<E3> CompleteE3s => E3s.Where(x => x.Complete).ToList();
        public List<Project> CompleteProjects => Projects.Where(x => x.Complete).ToList();

        public int HaveE3 => Projects.Where(x => x.HasE3).Count();
        public int HaveTaxisC => Projects.Where(x => x.HasTaxisCompany).Count();
        public int HaveTaxisE => Projects.Where(x => x.HasTaxisEstablishments).Count();

        public int EmptyProjects => Projects.Where(x => x.HasNothing).Count();


        public Project_Collection()
        {
            Projects = new List<Project>();
        }
        
        public void ScanPath(string path,bool serial = false)
        {
			var folders = Directory.GetDirectories(path).ToList();
            IEnumerable<string> existing = Projects.Select(x => x.ProjectPath);
            folders = folders.Except(existing).Take(100).ToList();
			DateTime start = DateTime.Now;

            if (serial)
            {
                foreach (var folder in folders)
                {
                    ScanFolder(folder);
                    string cd = Path.GetFileName(folder);
                    Global.current++;
                    Console.WriteLine(Global.current.ToString() + "-->" + cd);
                }
            }
            else
            {
                Parallel.ForEach(folders, folder =>
                {
                    ScanFolder(folder);
                });
            }

            DateTime end = DateTime.Now;
			TimeSpan totalTime = end - start;
            Console.WriteLine(totalTime);
            string[] s = Global.exitCodes;

            Functions.SaveToFile(this,@"C:\Users\chatziparadeisis.i\Documents\covid\athens.fol");
        }

        private void ScanFolder(string folder)
        {
            Project newProject = new Project();
            newProject.ScanPath(folder);
            Projects.Add(newProject);
        }
    }
}
