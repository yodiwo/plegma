using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.IO;

namespace Yodiwo.Tools
{
    public static partial class WindowsNative
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Load a native DLL
        /// </summary>
        /// <param name="librayName"></param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public extern static IntPtr LoadLibrary(string librayName);
        //usage :
        //var dllhandle = LoadLibrary(dll_path);
        //System.Diagnostics.Debug.Assert(DllHandle != IntPtr.Zero, "Unable to load library " + dll_path);

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the HANDLE of the desktop window.
        /// </summary>
        /// <returns>HANDLE of the Desktop window</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);


        // A function that lets us peeks at the first message avaliable from the OS
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        // Checks is there are any messages avaliable from the OS
        public static bool Is_App_Idle
        {
            get
            {
                Message msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        //Commands
        public const int WM_CHAR = 0x0102;
        public const int WM_SETTEXT = 0x0C;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_CLOSE = 0x010;
        public const int WM_COMMAND = 0x0111;
        public const int WM_CLEAR = 0x0303;
        public const int WM_DESTROY = 0x02;
        public const int WM_GETTEXT = 0x0D;
        public const int WM_GETTEXTLENGTH = 0x0E;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int WMP9_PLAY = 0x04978;

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        /// Return Type: BOOL->int
        ///X: int
        ///Y: int
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
        [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int X, int Y);

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Win32Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return w32Mouse;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Data structure that represents a message
        /// sent from the OS to the program.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Win32Point p;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        public static void CreateShortcut(string targetPath, string shortcutPath)
        {
            //create path
            if (Directory.Exists(Path.GetDirectoryName(shortcutPath)) == false)
                Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath));

            using (StreamWriter writer = new StreamWriter(shortcutPath))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + targetPath);
                writer.WriteLine("IconIndex=0");
                string icon = targetPath.Replace('\\', '/');
                writer.WriteLine("IconFile=" + icon);
                writer.Flush();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------------------------------------


    }
}
