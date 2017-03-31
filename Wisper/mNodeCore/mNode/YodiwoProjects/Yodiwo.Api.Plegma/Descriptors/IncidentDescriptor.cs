using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yodiwo;

namespace Yodiwo.API.Plegma
{
    /// <summary>
    /// Descriptor class for Edge Node Incidents (crashes, unexpected restarts, etc)
    /// </summary>
    public class IncidentDescriptor
    {
        /// <summary>timestamp in msec since Unix Epoch. Allowed to be empty</summary>
        public ulong Timestamp;

        /// <summary>optional file path of entity producing the incident</summary>
        public string FilePath;

        /// <summary>optional file name of entity producing the incident</summary>
        public string FileName;

        /// <summary>optional name of method/function producing the incident</summary>
        public string Method;

        /// <summary>optional line number of code producing the incident</summary>
        public int LineNumber;

        /// <summary>optional name of platform running</summary>
        public string Platform;

        /// <summary>comma separated Tag cloud</summary>
        public string Tags;

        /// <summary>ID of sender</summary>
        public string Sender;

        /// <summary>newline-separated (although not enforced) stack trace of incident</summary>
        public string StackTrace;

        /// <summary>one or more name-value pairs with extra, custom, information</summary>
        public List<ConfigParameter> Parameters;

        /// <summary>one or more messages describing incident</summary>
        public List<string> Messages;

        /// <summary>Severity of incident (follows Unix Syslog levels), <see cref="eSeverity"/></summary>
        public eSeverity Severity;

        /// <summary>optional User Agent string for client side errors</summary>
        public string UserAgent;

        /// <summary>Severity levels enum</summary>
        public enum eSeverity
        {
            /// <summary>System is unusable</summary>
            Emergency = 0,
            /// <summary>Should be corrected immediately</summary>
            Alert,
            /// <summary>Critical conditions, primary function failures</summary>
            Critical,
            /// <summary>Error conditions, limits exceeded, improper behavior</summary>
            Error,
            /// <summary>May indicate unexpected behavior or that an error will occur if action is not taken</summary>
            Warning,
            /// <summary>Signify events that are unusual, but not necessarily error conditions</summary>
            Notice,
            /// <summary>Normal operational messages that require no action.</summary>
            Informational,
            /// <summary>Information useful to developers for debugging the application</summary>
            Debug
        }

        /// <summary>incident constructor</summary>
        public IncidentDescriptor()
        {
            Messages = new List<string>();
        }

#pragma warning disable CS1591
        public bool IsFailure { get { return Severity <= eSeverity.Critical; } }

        public bool IsEmergency { get { return Severity <= eSeverity.Emergency; } }
        public bool IsAlert { get { return Severity <= eSeverity.Alert; } }
        public bool IsCritical { get { return Severity <= eSeverity.Critical; } }
        public bool IsError { get { return Severity <= eSeverity.Error; } }
        public bool IsWarning { get { return Severity <= eSeverity.Warning; } }
        public bool IsNotice { get { return Severity <= eSeverity.Notice; } }
        public bool IsInfo { get { return Severity <= eSeverity.Informational; } }
        public bool IsDebug { get { return Severity == eSeverity.Debug; } }

        public virtual bool HasStackTrace { get { return !string.IsNullOrWhiteSpace(StackTrace); } }
        public virtual string GetStaskTrace() { return StackTrace; }

        public virtual string ToMarkup(Markup.eType type)
        {
            var M = new Markup(type);

            string msg = "===== " + Yodiwo.Extensions.FromUnixMilli(Timestamp) + " =====" + M.Break;

            if (!string.IsNullOrWhiteSpace(this.FileName))
            {
                msg += M.Boldify("Incident at: ") + this.FilePath + ":" + this.LineNumber;
                msg += M.Break;
            }

            if (!string.IsNullOrWhiteSpace(this.Method))
            {
                msg += M.Boldify("Method: ") + this.Method + ":" + this.LineNumber;
                msg += M.Break;
            }

            msg += M.Boldify("Platform: ") + this.Platform + M.Boldify(" Severity: ") + this.Severity;
            msg += M.Break;

            msg += M.Boldify("Message(s): ") + M.Break + string.Join(M.Break + "  ", this.Messages);
            msg += M.Break;

            if (HasStackTrace)
            {
                msg += M.Boldify("Stack trace:");
                msg += M.MS_on + GetStaskTrace() + M.MS_off;
                msg += M.Break;
            }
            return msg;
        }

        public override string ToString()
        {
            return ToMarkup(Markup.eType.None);
        }
#pragma warning restore CS1591
    }
}
