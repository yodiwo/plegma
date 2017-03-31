using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo
{
    public interface IDBObject : ISupportInitialize
    {
        /// <summary>
        /// Returns ths latest (target) class version
        /// </summary>
        /// <returns></returns>
        int GetLatestClassVersion();

        /// <summary>
        /// Upon deserialization of an IDBObject, all members found in databse
        /// that have been modified in the targetted class version, should be 
        /// migration framework in the ExtraElements dedicated property of type
        /// IDictionary<string, object>. In cases where custom migration code is
        /// neccessary, this should be provided by implementing this method.
        /// Example migration code for a renamed memeber is shown below:
        ///
        /// >> object tmp;
        /// >> if (ExtraElements.TryGetValue("OldMemberName", out tmp) == false)
        /// >> {
        /// >>   return;
        /// >> }
        /// >>
        /// >> var element = (OldMemberType)tmp;
        /// >>
        /// >> NewMemberName = element;
        /// 
        /// </summary>
        void Migrate();
    }
}