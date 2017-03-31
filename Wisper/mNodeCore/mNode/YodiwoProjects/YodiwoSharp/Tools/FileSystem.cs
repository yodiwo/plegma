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
#if NETFX
            if (path.StartsWith("~/"))
                return Path.Combine(EnvironmentEx.UserHomePath, path.Substring(2));
            else
#endif
            if (path.StartsWith("/")) //Linux root path
                return path;
            else if (path.Length > 3 && (path.Substring(1, 2) == ":\\" || path.Substring(1, 2) == ":/")) //Windows root path
                return path;
            //other valid URIs
            else if (path.StartsWith("http://", false) || path.StartsWith("http:\\", false) ||
                     path.StartsWith("ftp://", false) || path.StartsWith("ftp:\\", false))
                return path;
            else
            {
#if UNIVERSAL
                return path;
#else
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
#endif
            }
        }

        public static string FixUserHomePath(string path)
        {
#if UNIVERSAL
            return path;
#else
            if (!path.StartsWith("~/"))
                return path;
            else
                return Path.Combine(EnvironmentEx.UserHomePath, path.Substring(2));
#endif
        }

        public enum FileOverwriteModes
        {
            NoOverwrite,
            Overwrite,
            IfDestinationIsNewer,
            IfLastWriteMismatch,
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, FileOverwriteModes FileOverwriteMode = FileOverwriteModes.Overwrite, bool ignoreCopyErrors = false, Func<FileInfo, bool> FileFilter = null, Func<DirectoryInfo, bool> DirectoryFilter = null)
        {
            //check if the source directory does exists
            if (!Directory.Exists(sourceDirName))
                return;

            //get directory info
            var dir = new DirectoryInfo(sourceDirName);

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            // Copy files
            Parallel.ForEach(dir.GetFiles(), file =>
            {
                if (FileFilter == null || FileFilter(file))
                    try
                    {
                        var dstFileName = Path.Combine(destDirName, file.Name);
                        if (FileOverwriteMode == FileOverwriteModes.Overwrite || !File.Exists(dstFileName))
                            file.CopyTo(dstFileName, true);
                        else if (FileOverwriteMode == FileOverwriteModes.IfDestinationIsNewer && (file.LastWriteTime - new FileInfo(dstFileName).LastWriteTime) > TimeSpan.Zero)
                            file.CopyTo(dstFileName, true);
                        else if (FileOverwriteMode == FileOverwriteModes.IfLastWriteMismatch && file.LastWriteTime != new FileInfo(dstFileName).LastWriteTime)
                            file.CopyTo(dstFileName, true);
                    }
                    catch (Exception ex)
                    {
                        if (!ignoreCopyErrors)
                            throw ex;
                    }
            });

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                //copy subdirectories
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                    if (DirectoryFilter == null || DirectoryFilter(subdir))
                    {
                        // Create the subdirectory.
                        string temppath = Path.Combine(destDirName, subdir.Name);

                        // Copy the subdirectories.
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
            }
        }

        public static IEnumerable<KeyValuePair<string, FileInfo[]>> DirectoryWalker(string sourceDirName, string fileSearchPattern = null)
        {
            //check if the source directory does exists
            if (!Directory.Exists(sourceDirName))
                yield break;

            //get directory info
            var dir = new DirectoryInfo(sourceDirName);

            //get files
            yield return new KeyValuePair<string, FileInfo[]>(sourceDirName, fileSearchPattern == null ? dir.GetFiles() : dir.GetFiles(fileSearchPattern, SearchOption.TopDirectoryOnly));

            //get subdirectories
            foreach (DirectoryInfo subdir in dir.GetDirectories())
                foreach (var subentry in DirectoryWalker(subdir.FullName))
                    yield return subentry;
        }

        public static long GetDirectorySize(string p, bool recursive, string pattern = "*")
        {
            // Get array of all file names.
            string[] files = Directory.GetFiles(p, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            // Return total size
            return files.Sum(f => { try { return new FileInfo(f).Length; } catch { return 0; } });
        }
    }
}
