using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Descriptors
{
    public class IODescriptor : IEquatable<IODescriptor>
    {
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public String Name;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public String FriendlyName;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public String Desc;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public String IOType;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public int Index;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public IOUIHints UIHints;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public IODescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------

        #endregion

        #region Functions
        //-------------------------------------------------------------------------------------------------------------------------
        // Use Name and Index to distinguish an io descriptor
        public bool Equals(IODescriptor other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            return other.Name == this.Name && other.Index == this.Index;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            int nameHashcode = Name.IsNullOrEmpty() ? 0 : Name.GetHashCode();
            int DescHashcode = Desc.IsNullOrEmpty() ? 0 : Desc.GetHashCode();
            return nameHashcode ^ DescHashcode ^ Index.GetHashCode();
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public void Update(IODescriptor other)
        {
            this.Name = other.Name;
            this.Desc = other.Desc;
            this.Index = other.Index;
            this.IOType = other.IOType;
            this.UIHints.Update(other.UIHints);
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
