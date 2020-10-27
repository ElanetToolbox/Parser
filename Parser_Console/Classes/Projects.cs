using Parser_Console.Interfaces;
using System;
using System.Collections.Generic;
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
        public List<Taxis> CompleteTaxis => Taxis.Where(x => x.Complete).ToList();
        public List<Project> CompleteProjects => Projects.Where(x => x.Complete).ToList();


        public Project_Collection()
        {
            Projects = new List<Project>();
        }
        
        public void ScanPath(string path,List<string> existing = null)
        {
			var folders = Directory.GetDirectories(path).ToList();
            if(existing != null)
            {
                folders = folders.Except(existing).Take(2000).ToList();
            }
			DateTime start = DateTime.Now;
            Global.current = 0;

            //foreach (var folder in folders)
            //{
            //    ScanFolder(folder);
            //    string cd = Path.GetFileName(folder);
            //    Global.current++;
            //    Console.WriteLine(Global.current.ToString() + "-->" + cd);
            //}

            Parallel.ForEach(folders, folder =>
            {
                ScanFolder(folder);
            });
            DateTime end = DateTime.Now;
			TimeSpan totalTime = end - start;
        }

        private void ScanFolder(string folder)
        {
            Project newProject = new Project();
            newProject.ScanPath(folder);
            Projects.Add(newProject);
            //Functions.SaveToFile(this);
        }
    }
}
