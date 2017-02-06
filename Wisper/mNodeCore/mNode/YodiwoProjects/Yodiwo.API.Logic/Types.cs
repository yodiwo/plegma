using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Logic
{

    public enum Residency : int
    {
        Unkown = 0,
        Cloud = 1,
        Node = 2,
        //... 3,4,5 could be a hierachy
    }

    public class IOUIHints
    {
        public enum eTriggerPlacement : byte
        {
            Bottom,
            Top,
            Left
        }

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        [Newtonsoft.Json.JsonIgnore]
        [DB_ExtraElements]
        // AtTop 
        public IDictionary<string, object> ExtraElements { get; set; }
        //-------------------------------------------------------------------------------------------------------------------------
        // IsVisible is used for showing/hiding ports
        [DB_IgnoreIfDefault]
        public bool IsVisible = true;
        //-------------------------------------------------------------------------------------------------------------------------
        // IsTrigger is used for placing the port at a position far from the non-trigger ports
        // default -> false
        [DB_IgnoreIfDefault]
        public bool IsTrigger;
        //-------------------------------------------------------------------------------------------------------------------------
        // used for defining the trigger placement position (bottom| left| top)
        // default -> bottom
        [DB_IgnoreIfDefault]
        public eTriggerPlacement TriggerPlacement;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors 
        //-------------------------------------------------------------------------------------------------------------------------
        public IOUIHints() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public IOUIHints(bool IsVisible, bool isTrigger, eTriggerPlacement TriggerPlacement)
        {
            this.IsVisible = IsVisible;
            this.IsTrigger = isTrigger;
            this.TriggerPlacement = TriggerPlacement;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //-------------------------------------------------------------------------------------------------------------------------
        public void Update(IOUIHints other)
        {
            // update only 'IsTrigger'
            this.IsTrigger = other.IsTrigger;

            // if 'IsTrigger' is disabled
            if (!IsTrigger)
                // assing default value so we don't need to write this value into DB
                this.TriggerPlacement = eTriggerPlacement.Bottom;
        }
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
    }

}
