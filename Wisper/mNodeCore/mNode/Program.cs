#define UseAppDomainHosting
//#define CleanShadowCacheOnExit
//#define CleanShadowCacheOnStart

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yodiwo.mNode
{
    class Program
    {
        public static readonly bool IsRunningOnMono = Type.GetType("Mono.Runtime") != null;
        public static readonly bool IsRunningOnUnix = Environment.OSVersion.Platform == PlatformID.Unix ||
                                                      Environment.OSVersion.Platform == PlatformID.MacOSX;
#if UseAppDomainHosting || DEBUG
        static AppDomain appDomain;
#endif
        static Process p;
        static bool IsRunning = true;

        [STAThread]
        static void Main(string[] args)
        {
            //declares
            var rnd = new Random((int)(Environment.TickCount % int.MaxValue));

            //find if another instance is already running with some mutex magic
            bool result;
            var simpleID = simpleHashCode(AppDomain.CurrentDomain.BaseDirectory);
            var appID = "mNode-" + simpleID;
            var mutex = new System.Threading.Mutex(true, appID, out result);
            if (!result)
            {
                System.Windows.Forms.MessageBox.Show("Another instance of the application is already running.", "Yodiwo mNode", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                return;
            }

            if (IsRunningOnUnix)
                Console.WriteLine("mNode linux mode");

            //hook exit events
            try { Console.CancelKeyPress += (s, e) => Exit(); } catch { }
            try { System.Diagnostics.Process.GetCurrentProcess().Exited += (s, e) => Exit(); } catch { }
            try { System.Windows.Forms.Application.ApplicationExit += (s, e) => Exit(); } catch { }
            try { System.AppDomain.CurrentDomain.ProcessExit += (s, e) => Exit(); } catch { }

            //get temporary path
            var tempPath = Path.GetTempPath();

            //get paths
            var OriginBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var ShadowBaseDirectory = Path.Combine(tempPath, "mNode.shadow." + simpleID);// + "." + rnd.Next());

            while (IsRunning)
            {
                //Shadow copy files
                {
#if !DEBUG && !CleanShadowCacheOnStart
                    if (rnd.Next(0, 10) == 1)   //clean once every 10 restart
#endif
                    {
                        //clean previous shadow files
                        for (int n = 0; n < 20; n++)
                            try
                            {
                                if (Directory.Exists(ShadowBaseDirectory))
                                    Directory.Delete(ShadowBaseDirectory, true);
                                break;
                            }
                            catch { Thread.Sleep(100); }
                    }

                    //shadow copy files
                    try
                    {
                        DirectoryCopy(OriginBaseDirectory,
                                        ShadowBaseDirectory,
                                        true,
                                        overwrite: true,
                                        OverwriteOnlyIfNewer: rnd.Next(0, 10) != 1, //usualy overwrite if newer.. but every now and then just overwrite everything
                                        ignoreCopyErrors: true,
                                        FileFilter: fileInfo => fileInfo.Name.ToLowerInvariant() != "yodiwo.mnode.exe"
                                    );
                    }
                    catch (Exception) { }
                }

                //bootstrap
#if UseAppDomainHosting || DEBUG
                if (IsRunningOnUnix)
#endif
                {
                    //linux(mono) sucks at handling(unloading) appdomains..
                    //so make the bootstrapping process a simple exe launching and watching..
                    try
                    {
                        //start process
                        var pi = new ProcessStartInfo();
                        pi.FileName = Path.Combine(ShadowBaseDirectory, "Yodiwo.mNode.AppHost.exe");
                        pi.Arguments = $@"""{OriginBaseDirectory}""";
                        pi.WorkingDirectory = ShadowBaseDirectory;
                        pi.UseShellExecute = false;
                        p = Process.Start(pi);
                        //wait for exit
                        p.WaitForExit();
                        //get exit code and dispose
                        var exitcode = p.ExitCode;
                        try { p.Dispose(); } catch { }
                        p = null;
                        //check ecit code
                        Console.WriteLine("mNode loader exited with " + exitcode);
                        if (exitcode == 12345)
                            goto Exit;
                    }
                    catch (Exception ex) { Console.WriteLine("mNode loading exception (" + ex.Message + ")"); }
                    //sleep to avoid catastropic spinning
                    Thread.Sleep(500);
                }
#if UseAppDomainHosting || DEBUG
                else
                {
                    //Setting the AppDomainSetup. 
                    var adSetup = new AppDomainSetup();
                    adSetup.ApplicationName = "Yodiwo.mNode";
                    adSetup.ApplicationBase = Path.GetFullPath(ShadowBaseDirectory);
                    adSetup.PrivateBinPath = Path.GetFullPath(ShadowBaseDirectory);
                    adSetup.ShadowCopyFiles = "false";

                    //unrestricted
                    var permSet = new PermissionSet(PermissionState.Unrestricted);

                    //Now we have everything we need to create the AppDomain, so let's create it.
                    appDomain = AppDomain.CreateDomain("CoreHost", AppDomain.CurrentDomain.Evidence, adSetup, permSet, new System.Security.Policy.StrongName[] { });

                    //execute assembly
                    try { appDomain.ExecuteAssembly(Path.Combine(ShadowBaseDirectory, "Yodiwo.mNode.AppHost.exe"), new string[] { OriginBaseDirectory }); }
                    catch (Exception ex)
                    {
                        //exit?
                        if (ex.Message == "Exit")
                            goto Exit;

                        //sleep to avoid catastropic spinning
                        Thread.Sleep(500);
#if DEBUG
                        //rethrow exception
                        throw ex;
#endif
                    }

                    //unload domain
                    Task.Run(() => { try { AppDomain.Unload(appDomain); } catch { } }).Wait(5000);
                }
#endif
            }

            //-------------------------
            // Shutting Down
            //-------------------------
            Exit:

#if CleanShadowCacheOnExit
            //clean shadow files
            for (int n = 0; n < 10; n++)
                try
                {
                    if (Directory.Exists(ShadowBaseDirectory))
                        Directory.Delete(ShadowBaseDirectory, true);
                    break;
                }
                catch { Thread.Sleep(100); }
#endif

            GC.KeepAlive(mutex);    // mutex shouldn't be released
            Environment.Exit(0);
            return;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //TODO: more gracefull shutdown?
        static void Exit()
        {
            IsRunning = false;
            try
            {
                if (p != null)
                    p.Close();
#if UseAppDomainHosting || DEBUG
                else if (appDomain != null)
                    Task.Run(() => { try { AppDomain.Unload(appDomain); } catch { } }).Wait(2000);
#endif
            }
            catch { }
            Environment.Exit(0);
        }
        //------------------------------------------------------------------------------------------------------------------------
        static void AppendException(Exception ex)
        {
            try
            {
                var stack = ex?.StackTrace;
                var msges = new List<string>();

                while (ex != null)
                {
                    msges.Add(ex.Message);
                    ex = ex.InnerException;
                }
                var msg = "Unhandled Exception caught : " + Environment.NewLine
                    + string.Join(Environment.NewLine, msges) + Environment.NewLine
                    + "StackTrace:" + stack;

                File.WriteAllText("unhandled_exception.log", msg);
            }
            catch { }
        }
        //------------------------------------------------------------------------------------------------------------------------
        static int simpleHashCode(string str)
        {
            unchecked
            {
                int res = 2833101;
                foreach (var c in str)
                    res = (res ^ c) * 387;
                return res & 0x7FFFFFFF;
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool overwrite = true, bool ignoreCopyErrors = false, Func<FileInfo, bool> FileFilter = null, Func<DirectoryInfo, bool> DirectoryFilter = null, bool OverwriteOnlyIfNewer = false)
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
                        if (overwrite || !File.Exists(dstFileName))
                            file.CopyTo(dstFileName, true);
                        else if (OverwriteOnlyIfNewer && file.LastWriteTime != new FileInfo(dstFileName).LastWriteTime)
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
        //------------------------------------------------------------------------------------------------------------------------
    }
}
