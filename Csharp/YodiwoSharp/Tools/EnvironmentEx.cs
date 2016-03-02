using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static class EnvironmentEx
    {
        //NOTE : for build-time detection of mono-compiler use #if __MonoCS__
        public static readonly bool IsRunningOnMono = Type.GetType("Mono.Runtime") != null;
#if NETFX
        public static readonly bool IsRunningOnUnix = Environment.OSVersion.Platform == PlatformID.Unix ||
                                                      Environment.OSVersion.Platform == PlatformID.MacOSX;

        public static readonly bool IsRunningOnWindows = Environment.OSVersion.Platform == PlatformID.Win32NT ||
                                                         Environment.OSVersion.Platform == PlatformID.Win32S ||
                                                         Environment.OSVersion.Platform == PlatformID.Win32Windows;

        public static readonly string UserHomePath;
#endif

        static EnvironmentEx()
        {
#if NETFX
            //find home user path
            UserHomePath = IsRunningOnUnix ? Environment.GetEnvironmentVariable("HOME")
                                             : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            //check
            if (string.IsNullOrWhiteSpace(UserHomePath))
            {
                DebugEx.Assert("Could not find use home path");
                UserHomePath = string.Empty;
            }
#endif
        }
    }
}
