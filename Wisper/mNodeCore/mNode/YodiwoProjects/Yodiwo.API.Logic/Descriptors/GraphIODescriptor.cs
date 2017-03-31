using System;

namespace Yodiwo.Logic.Descriptors
{
    public class GraphIODescriptor
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public int UID;

        [DB_IgnoreIfDefault]
        public int Index;

        [DB_IgnoreIfDefault]
        public int ConnectedUID;

        [DB_IgnoreIfDefault]
        public int ConnectedIndex;

        [DB_IgnoreIfDefault]
        public bool IsVisible;

        [DB_IgnoreIfDefault]
        public double X;

        [DB_IgnoreIfDefault]
        public double Y;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public GraphIODescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
