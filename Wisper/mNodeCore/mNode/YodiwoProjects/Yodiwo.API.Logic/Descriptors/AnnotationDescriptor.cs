using System;

namespace Yodiwo.Logic.Descriptors
{

    public class AnnotationDescriptor
    {
        #region variables
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public String Text;

        [DB_IgnoreIfDefault]
        public String TextAttributes;

        [DB_IgnoreIfDefault]
        public String BoxAttributes;

        [DB_IgnoreIfDefault]
        public double X;

        [DB_IgnoreIfDefault]
        public double Y;

        [DB_IgnoreIfDefault]
        public int Angle;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public AnnotationDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
