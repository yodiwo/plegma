using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic.Descriptors
{
    public class IOCategoryDescriptor : IEquatable<IOCategoryDescriptor>
    {
        #region variables
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public String Name = "";
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public int MinVisibleIO;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        public int NumVisibleIO;
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public int[] InputsIndex = new int[0];
        //-------------------------------------------------------------------------------------------------------------------------
        [DB_IgnoreIfDefault]
        [DB_IgnoreIfEmpty]
        public int[] OutputIndex = new int[0];
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Contructor
        //-------------------------------------------------------------------------------------------------------------------------
        public IOCategoryDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //-------------------------------------------------------------------------------------------------------------------------
        // Use Name to distinguish an iocategory
        public bool Equals(IOCategoryDescriptor other)
        {
            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            return other.Name == this.Name;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        //-------------------------------------------------------------------------------------------------------------------------

        public void Update(IOCategoryDescriptor newCat)
        {
            this.MinVisibleIO = newCat.MinVisibleIO;
            this.InputsIndex = newCat.InputsIndex;
            this.OutputIndex = newCat.OutputIndex;
            // don't change 'NumVisibleIO' unless 
            // -- it is greatear than InputsIndex.Length | OutputIndex.Length
            // -- is less than 'MinVisibleIO'
            if ((NumVisibleIO > InputsIndex.Length && InputsIndex.Length > 0) ||
                (NumVisibleIO > OutputIndex.Length && OutputIndex.Length > 0) ||
                NumVisibleIO < MinVisibleIO)
            {
                this.NumVisibleIO = newCat.NumVisibleIO;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}