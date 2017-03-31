using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.Warlock;

namespace Yodiwo.API.Warlock
{
    //------------------------------------------------------------------------------------------------------------------------
    public class DatabaseInfoDescriptorCommon
    {
        //------------------------------------------------------------------------------------------------------------------------
        public string DbName;
        public string DbUsername;
        public List<string> Privileges;
        public string ConnectionString;
        //------------------------------------------------------------------------------------------------------------------------
        public DatabaseInfoDescriptorCommon() { }
        //------------------------------------------------------------------------------------------------------------------------
        public DatabaseInfoDescriptorCommon(DatabaseInfoDescriptorCommon obj)
        {
            this.DbName = obj.DbName;
            this.DbUsername = obj.DbUsername;
            this.Privileges = obj.Privileges;
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class DatabaseInfoDescriptor : DatabaseInfoDescriptorCommon
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public string Type;
        public int LinkedUsersCnt;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public DatabaseInfoDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public DatabaseInfoDescriptor(DatabaseInfoDescriptorCommon obj) : base(obj) { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class DatabaseUserInfoDescriptor : DatabaseInfoDescriptorCommon
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        public int LinkedSchemasCnt;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public DatabaseUserInfoDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public DatabaseUserInfoDescriptor(DatabaseInfoDescriptorCommon obj) : base(obj) { }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
    //------------------------------------------------------------------------------------------------------------------------
}


