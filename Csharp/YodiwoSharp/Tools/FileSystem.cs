using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class FileSystem
    {
        public static string GetPhysicalPath(string filepath)
        {
            if (File.Exists(filepath))
                return Directory.GetFiles(Path.GetDirectoryName(filepath), Path.GetFileName(filepath)).FirstOrDefault();
            else
                return null;
        }

        public static long GetFileSize(string fileName)
        {
            // Create new FileInfo object and get the Length.
            return new FileInfo(fileName).Length;
        }

        public static string FixPath(string path, bool MakeAbsolute = false)
        {
            if (path.StartsWith("~/"))
                return Path.Combine(EnvironmentEx.UserHomePath, path.Substring(2));
            else if (path.StartsWith("/")) //Linux root path
                return path;
            else if (path.Length > 3 && (path.Substring(1, 2) == ":\\" || path.Substring(1, 2) == ":/")) //Windows root path
                return path;
            //other valid URIs
            else if (path.StartsWith("http://", false) || path.StartsWith("http:\\", false) ||
                     path.StartsWith("ftp://", false) || path.StartsWith("ftp:\\", false))
                return path;
            else
            {
                //must be relative
                if (MakeAbsolute)
                {
                    //try to find best assembly location
                    var assem = System.Reflection.Assembly.GetEntryAssembly();
                    if (assem == null)
                        assem = System.Reflection.Assembly.GetExecutingAssembly();
                    //combine
                    return Path.Combine(assem != null ? assem.Location : Environment.CurrentDirectory, path);
                }
                else
                    return path;
            }
        }

        public static string FixUserHomePath(string path)
        {
            if (!path.StartsWith("~/"))
                return path;
            else
                return Path.Combine(EnvironmentEx.UserHomePath, path.Substring(2));
        }
    }
}
