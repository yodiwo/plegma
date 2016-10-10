using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public class Markup
    {
        public readonly string Header;
        public readonly string Footer;
        public readonly string Heading;
        public readonly string B_on;     //bold
        public readonly string B_off;    //unbold
        public readonly string I_on;     //italic
        public readonly string I_off;    //unitalic
        public readonly string Break;    //break/newline
        public readonly string HR;       //horizontal line
        public readonly string MS_on;    //monospace
        public readonly string MS_off;   //monospace off

        public enum eType
        {
            None,
            HTML,
            Markdown,
            Slack
        }
        private eType Type;

        public string Boldify(string str) { return B_on + str + B_off; }
        public string Italify(string str) { return I_on + str + I_off; }
        public string Headingify(string str)
        {
            switch (Type)
            {
                case eType.HTML:
                    return "</p><h2>" + str + "</h2><p>";
                case eType.Markdown:
                    return "*** " + str + " ***";
                case eType.Slack:
                case eType.None:
                default:
                    return "";
            }
        }

        public Markup(eType type)
        {
            Type = type;
            Header = Footer = Heading = B_off = B_on = I_off = I_on = Break = HR = MS_off = MS_on = "";

            switch (type)
            {
                case eType.HTML:
                    Header = @"<!DOCTYPE html>
                             <html lang=""en"">
                             <head>
                                <meta charset = ""utf-8""/>
                                <style>
                                p {
                                    font-family: verdana;
                                    font-size: 90%;
                                }
                                p.mono {
                                    font-family: monospace;
                                    font-size: 110%;
                                }
                                </style>
                             </head>
                             <body>
                             <p>";
                    Footer = @"</p></body></html>";
                    B_on = " <strong>";
                    B_off = "</strong>";
                    I_on = "<em>";
                    I_off = "</em>";
                    Break = "<br />";
                    HR = "</p><hr><p>";
                    MS_on = "</p><p class=\"mono\">";
                    MS_off = "</p><p>";
                    break;
                case eType.Markdown:
                    B_on = B_off = "**";
                    I_on = I_off = "*";
                    Break = Environment.NewLine;
                    MS_on = "```";
                    MS_off = "```";
                    break;
                case eType.Slack:
                    B_on = B_off = "*";
                    I_on = I_off = "_";
                    Break = Environment.NewLine;
                    MS_on = "```";
                    MS_off = "```";
                    break;
                case eType.None:
                default:
                    break;
            }
        }

        public static string NewLine(eType type)
        {
            switch (type)
            {
                case eType.HTML:
                    return "<br />";
                case eType.None:
                case eType.Markdown:
                case eType.Slack:
                default:
                    return Environment.NewLine;
            }
        }
    }
}
