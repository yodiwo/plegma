using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.API.MediaStreaming;
using Yodiwo;

namespace Yodiwo.Projects.RasPiCamera
{
    public static class Helper
    {
        public static Yodiwo.NodeLibrary.Node node;
        //things
        internal static List<Thing> Things = new List<Thing>();
        public static Thing CameraThing;


        public static List<Thing> GatherThings(Transport trans)
        {
            //setup Camera  thing
            #region Setup Camera thing
            {
                ConfigParameter conf = new ConfigParameter()
                {
                    Name = "AccessUri",
                    Value = "RasPiCamera://10.30.254.223",
                };
                var thing = CameraThing = new Yodiwo.API.Plegma.Thing()
                {
                    Type = "yodiwo.output.camera",
                    Name = "RasPiCamera ",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/VirtualGateway/img/icon-thing-genericwebcam.png",
                    },
                };
                VideoMediaDescriptor video = new VideoMediaDescriptor()
                {
                    uri = "node://$NodeKey$/" + conf.Value,
                    protocol = "http://",
                    videoDevice = VideoIn.Node,
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Camera Feed",
                        State = video.ToJSON(HtmlEncode: false),
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "Camera Filter",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                    }
                };
            }
            #endregion

            Things.Add(CameraThing);

            return Things;
        }
    }
}
