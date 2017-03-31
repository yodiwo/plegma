using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public class LinuxConfigParser
    {

        public static IDictionary<string, IList<KeyValuePair<string, string>>> ParseConfig(string contents)
        {
            var top = new Dictionary<string, IList<KeyValuePair<string, string>>>();
            var lines = Regex.Split(contents, "\r\n|\r|\n");
            IList<KeyValuePair<string, string>> currentSection = null;
            foreach (var line in lines)
            {
                var hashIndex = line.IndexOf("#", StringComparison.Ordinal);
                var nocomment = (hashIndex >= 0) ? line.Substring(0, hashIndex) : line;
                var match = Regex.Match(nocomment, @"^\[(\w+)\]");
                if (match.Success)
                {
                    //new section
                    var sectionId = match.Groups[1].Value;
                    if (!top.TryGetValue(sectionId, out currentSection))
                    {
                        currentSection = new List<KeyValuePair<string, string>>();
                        top[sectionId] = currentSection;
                    }
                }
                match = Regex.Match(nocomment, @"(\w+)=([\w\d\./]+)");
                if (match.Success && currentSection != null)
                {
                    //new section
                    var key = match.Groups[1].Value;
                    var val = match.Groups[2].Value;
                    currentSection.Add(new KeyValuePair<string, string>(key, val));
                }
            }
            return top;
        }

        public static string PrintConfig(IDictionary<string, IList<KeyValuePair<string, string>>> conf)
        {
            string text = "";
            if (conf == null)
                return "";
            foreach (var section in conf)
            {
                text += "[" + section.Key + "]" + Environment.NewLine;
                if (section.Value == null)
                    continue;
                foreach (var keyval in section.Value)
                {
                    text += keyval.Key + "=" + keyval.Value + Environment.NewLine;
                }
            }
            return text;
        }
    }
}
