using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    //-------------------------------------------------------------------------------------------------------------------------
    public enum eDocumentationContentType
    {
        Text,
        HTML,
        XML,
        CommonMark,
        // Add new content types below
    }

    //-------------------------------------------------------------------------------------------------------------------------
    public class DocumentationWrlkDescriptor
    {
        public string Name;
        public string Content;
        public eDocumentationContentType ContentType;
    }
    //-------------------------------------------------------------------------------------------------------------------------
}
