using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public static class DebugEx
    {
        [Conditional("TRACE"), Conditional("TRACE_ASSERT")]
        public static void Assert(bool Condition, string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            if (!Condition)
            {
                Debug.Assert(false, Error_Message + Environment.NewLine);
#if DEBUG
                TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#else
                TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#endif
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_ASSERT")]
        public static void Assert(string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            Debug.Assert(false, Error_Message + Environment.NewLine);
            TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
        }

        [Conditional("TRACE")]
        public static void Assert(Exception ex)
        {
            string innerException = "";
            var stack = ex.StackTrace;
            if (ex.InnerException != null)
                innerException = "InnerException : " + ex.InnerException.Message + Environment.NewLine + Environment.NewLine;
            Debug.Assert(false, "Messsage:" + ex.Message + Environment.NewLine + Environment.NewLine + innerException + "StackTrace:" + stack);
#if DEBUG
            TraceError("Assert failed, Messsage:" + ex.Message); //add more info?
#else
            TraceError("Assert failed, Messsage:" + ex.Message + Environment.NewLine + Environment.NewLine + innerException + "StackTrace:" + stack);
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_ASSERT")]
        public static void Assert(Exception ex, string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            string innerException = "";
            var stack = ex.StackTrace;
            if (ex.InnerException != null)
                innerException = "InnerException : " + ex.InnerException.Message + Environment.NewLine + Environment.NewLine;
            Debug.Assert(false, Error_Message + Environment.NewLine + "Messsage:" + ex.Message + Environment.NewLine + Environment.NewLine + innerException + "StackTrace:" + stack);
#if DEBUG
            TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#else
            TraceError(Error_Message + Environment.NewLine + "Messsage:" + ex.Message + Environment.NewLine + Environment.NewLine + innerException + "StackTrace:" + stack, filePath: filePath, lineNumber: lineNumber, method: method);
#endif
        }

        public static void AssertAndThrow(bool Condition, string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
            if (!Condition)
            {
#if TRACE
                Debug.Assert(false, Error_Message + Environment.NewLine);
                TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#endif
                throw new Exception(Error_Message);
            }
        }

        public static void AssertAndThrow(string Error_Message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if TRACE
            Debug.Assert(false, Error_Message + Environment.NewLine);
            TraceError(Error_Message, filePath: filePath, lineNumber: lineNumber, method: method);
#endif
            throw new Exception(Error_Message);
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLog(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after info not a typo
            while (ex != null)
            {
                var stack = ex.StackTrace;
#if NETFX
                Trace.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack));
#elif UNIVERSAL
                Debug.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack));
#endif
                ex = ex.InnerException;
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLog(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after info not a typo
            Trace.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#elif UNIVERSAL
            Debug.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLogInlineBegin(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after info not a typo
            Trace.Write(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#elif UNIVERSAL
            Debug.WriteLine(String.Format("{0} Info  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLogInline(string message)
        {
#if NETFX
            Trace.Write(message);
#elif UNIVERSAL
            Debug.WriteLine(message);
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_LOG")]
        public static void TraceLogInlineEnd(string message)
        {
#if NETFX
            Trace.WriteLine(message);
#elif UNIVERSAL
            Debug.WriteLine(message);
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_WARNING")]
        public static void TraceWarning(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after warn not a typo
            Trace.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#elif UNIVERSAL
            Debug.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_WARNING")]
        public static void TraceWarning(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            //two spaces after warn not a typo
            while (ex != null)
            {
                var stack = ex.StackTrace;
#if NETFX
                Trace.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack));
#elif UNIVERSAL
                Debug.WriteLine(String.Format("{0} Warn  {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack));
#endif
                ex = ex.InnerException;
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_ERROR")]
        public static void TraceError(string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if NETFX
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            Trace.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#elif UNIVERSAL
            Debug.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message));
#endif
        }

        [Conditional("TRACE"), Conditional("TRACE_ERROR")]
        public static void TraceError(Exception ex, string message, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            while (ex != null)
            {
                var stack = ex.StackTrace;
#if NETFX
                Trace.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack));
#elif UNIVERSAL
                Debug.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, message + " (" + ex.Message + ")" + Environment.NewLine + "StackTrace : " + stack));
#endif
                ex = ex.InnerException;
            }
        }

        [Conditional("TRACE"), Conditional("TRACE_ERROR")]
        public static void TraceErrorException(Exception ex, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberNameAttribute] string method = "")
        {
#if DEBUG
            try { var p = Path.GetFileName(filePath); } catch { Debug.Assert(false, "Invalid Log message"); };
#endif
            while (ex != null)
            {
                var stack = ex.StackTrace;
#if NETFX
                Trace.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, "Exception : " + ex.Message + "" + Environment.NewLine + "StackTrace: " + stack));
#elif UNIVERSAL
                Debug.WriteLine(String.Format("{0} ERROR {1} [{2}:{3}] :: {4}", DateTime.Now, Path.GetFileName(filePath), method, lineNumber, "Exception : " + ex.Message + "" + Environment.NewLine + "StackTrace: " + stack));
#endif
                ex = ex.InnerException;
            }
        }
    }
}