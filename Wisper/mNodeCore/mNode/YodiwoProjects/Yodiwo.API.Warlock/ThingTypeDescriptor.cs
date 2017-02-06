using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;

namespace Yodiwo.API.Warlock
{
    public class ThingTypeDescriptor
    {
        //-------------------------------------------------------------------------------------------------------------------------
        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string Img;
        public string Type;
        public bool Searchable;
        public string Description;
        public Dictionary<string, ThingModelType> Model;
        public List<NodeDescriptor> Nodes;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------------------------------------
        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public ThingTypeDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public ThingTypeDescriptor(ThingType ThingType, List<NodeDescriptor> Nodes = null)
        {
            this.Type = ThingType.Type;
            this.Description = ThingType.Description;
            this.Searchable = ThingType.Searchable;
            this.Model = ThingType.Models;
            // TODO: remove Img field from this model view
            // Img was used in Grid Layout of ThingsManager
            this.Img = "/Content/ThingsManager/img/" + ThingType.Type + ".png";
            this.Nodes = Nodes;
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------


    }
}
