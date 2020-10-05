using System;
using System.Collections.Generic;
using System.Text;

namespace Parser_Console
{
	public static class Functions
	{
		public static string GetLine(string text, int lineNo)
		{
		  string[] lines = text.Replace("\r","").Split('\n');
		  return lines.Length >= lineNo ? lines[lineNo-1] : null;
		}
	}
}
