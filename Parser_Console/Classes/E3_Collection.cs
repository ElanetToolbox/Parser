using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser_Console.Classes
{
	public class E3_Collection
	{
		public List<E3> E3s { get; set; }
		public List<E3> ValidE3s => E3s.Where(x => x.error == false).ToList();
		public List<E3> NoTextDocs => E3s.Where(x => x.noText == true).ToList();
		public List<E3> InvalidDocs => E3s.Where(x => x.notE3 == true).ToList();
		public List<E3> WeirdE3s => E3s.Where(x => x.weirdE3 == true).ToList();

		public E3_Collection()
		{
			E3s = new List<E3>();
		}

		public void AddE3(string path)
		{
			E3 newE3 = new E3(path);
			E3s.Add(newE3);
		}
	}
}
