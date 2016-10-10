using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;

namespace Yodiwo.Projects.SkyWriter
{
    public static class Helper
    {
        public static Yodiwo.NodeLibrary.Node node;
        //things
        public static ListTS<Thing> Things = new ListTS<Thing>();
        public static Thing PositionThing;
        public static Thing GestureThing;
        //dictionary thing.name, skywriter sensor
        public static Dictionary<string, object> SkyWriterSensors = new Dictionary<string, object>();
        //dictionary thing,skywritersensor
        public static Dictionary<Thing, SkyWriterSensor> Lookup = new Dictionary<Thing, SkyWriterSensor>();

        public static void CreateThings(Transport trans, Node node)
        {
            //setup Position  thing
            //3 ports (x,y,z)
            #region Setup Position thing
            {
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "PositionThing"),
                    Type = ThingTypeLibrary.PositionSensor.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "Position",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/Generic/position.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Position x State",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.PositionSensorModel_XId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Position y State",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.PositionSensorModel_YId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Position z State",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.PositionSensorModel_ZId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "2")
                    },
                };
                PositionThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup Gesture thing
            // 5 ports(i.e tap:'center','flick':north2south,'touch':west...)
            #region Setup Gesture thing
            {
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "GestureThing"),
                    Type = ThingTypeLibrary.GestureSensor.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "Gesture",
                    Config = null,
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/Generic/motion.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Tap",
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.GestureSensorModel_TapId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Touch",
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.GestureSensorModel_TouchId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "1")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "DoubleTap",
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.GestureSensorModel_DoubleTapId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "2")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Airwheel",
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.GestureSensorModel_AirwheelId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "3")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Flick",
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortModelId = ModelTypeLibrary.GestureSensorModel_FlickId,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "4")
                    },
                };
                GestureThing = thing = node.AddThing(thing);
            }
            #endregion

            //add things
            Things.Add(PositionThing);
            Things.Add(GestureThing);

            //create skywriter sensors
            PositionSensor positionsensor = new PositionSensor(trans);
            GestureSensor gesturesensor = new GestureSensor(trans);
            //update dictinaries
            Lookup.Add(PositionThing, positionsensor);
            Lookup.Add(GestureThing, gesturesensor);
            SkyWriterSensors.Add(PositionThing.Name, positionsensor);
            SkyWriterSensors.Add(GestureThing.Name, gesturesensor);

            //register events
            positionsensor.OnGetContinuousDatacb += p => OnGetPositionDatacb(positionsensor, p);
            gesturesensor.OnGetContinuousDatacb += p => OnGetGestureDatacb(gesturesensor, p);
        }


        static void OnGetPositionDatacb(SkyWriterSensor sensor, object data)
        {
            List<TupleS<Port, string>> pevents = new List<TupleS<Port, string>>();

            var payload = data as PositionData;
            //construct port events
            if (!String.IsNullOrEmpty(payload.x))
            {
                pevents.Add(new TupleS<Port, string>(PositionThing.Ports[0], payload.x));
            }
            if (!String.IsNullOrEmpty(payload.y))
            {
                pevents.Add(new TupleS<Port, string>(PositionThing.Ports[1], payload.y));
            }
            if (!String.IsNullOrEmpty(payload.z))
            {
                pevents.Add(new TupleS<Port, string>(PositionThing.Ports[2], payload.z));
            }
            //send port events message
            Helper.node.SetState(pevents);
        }
        static void OnGetGestureDatacb(SkyWriterSensor sensor, object data)
        {
            List<TupleS<Port, string>> pevents = new List<TupleS<Port, string>>();

            var payload = data as GestureEvents;
            //construct port events
            if (!String.IsNullOrEmpty(payload.airwheel))
            {
                pevents.Add(new TupleS<Port, string>(GestureThing.Ports[3], payload.airwheel));
            }
            if (!String.IsNullOrEmpty(payload.tap))
            {
                pevents.Add(new TupleS<Port, string>(GestureThing.Ports[0], payload.tap));
            }
            if (!String.IsNullOrEmpty(payload.touch))
            {
                pevents.Add(new TupleS<Port, string>(GestureThing.Ports[1], payload.touch));
            }
            if (!String.IsNullOrEmpty(payload.doubletap))
            {
                pevents.Add(new TupleS<Port, string>(GestureThing.Ports[2], payload.doubletap));
            }
            if (!String.IsNullOrEmpty(payload.flick))
            {
                pevents.Add(new TupleS<Port, string>(GestureThing.Ports[4], payload.flick));
            }
            //send portevent message
            Helper.node.SetState(pevents);
        }
    }
}
