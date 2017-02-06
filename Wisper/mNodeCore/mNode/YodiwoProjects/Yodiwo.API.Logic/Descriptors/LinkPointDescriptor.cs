using System;

namespace Yodiwo.Logic.Descriptors
{

    public class LinkPointDescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public int UID;

        [DB_IgnoreIfDefault]
        public int ConnectedPointUID;

        [DB_IgnoreIfDefault]
        public String Type;

        [DB_IgnoreIfDefault]
        public double X;

        [DB_IgnoreIfDefault]
        public double Y;

        [DB_IgnoreIfDefault]
        public String Color;

        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public LabelDescriptor LabelAttributes;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public LinkPointDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        public class LabelDescriptor
        {
            [DB_IgnoreIfDefault]
            public string Text;

            [DB_IgnoreIfDefault]
            public int Size;

            [DB_IgnoreIfDefault]
            public String Position;
        }
    }
}
