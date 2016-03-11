using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseSharp.Portable;
using Yodiwo.API.Plegma;

namespace Yodiwo.Gateway.GatewayMain.Nest
{
    public static class NestSmokeCoAlarm
    {

        public static List<Thing> Convert2YodiwoThing(IDataSnapshot snap)
        {

            List<Thing> Things = new List<Thing>();
            foreach (var smokelarm in snap.Children)
            {
                SmokeCoAlarm smokecoalarmdescriptor = null;
                Thing thing = null;
                smokecoalarmdescriptor = new SmokeCoAlarm();

                foreach (var smokecoalarmattribute in smokelarm.Children)
                {
                    var typr = smokecoalarmdescriptor.GetType();
                    var props = typr.GetProperties();
                    System.Reflection.PropertyInfo propertyinfo = null;
                    foreach (var prop in props)
                    {
                        if (prop.Name == smokecoalarmattribute.Key)
                        {
                            propertyinfo = prop;
                            break;
                        }
                    }
                    try
                    {
                        var val = smokecoalarmattribute.Value();
                        propertyinfo.SetValue(smokecoalarmdescriptor, smokecoalarmattribute.Value(), null);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                smokecoalarmdescriptor.device_id = smokelarm.Key.Replace("-", "_");
                try
                {
                    if (!NestModule.nestdescriptors.ContainsKey(smokelarm.Key))
                        NestModule.nestdescriptors.Add(smokelarm.Key.Replace("-", "_"), new NestDescriptor() { type = NestSensor.Smoke_Co_Alarms, nestthing = smokecoalarmdescriptor });
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }
                try
                {
                    var thingkeystr = ThingKey.BuildFromArbitraryString("$NodeKey$", smokecoalarmdescriptor.device_id);
                    thing = new Thing();
                    thing.ThingKey = thingkeystr;
                    thing.Name = smokecoalarmdescriptor.name_long;
                    thing.Ports.Add(new Port()
                    {

                        Name = nameof(smokecoalarmdescriptor.co_alarm_state),
                        State = smokecoalarmdescriptor.co_alarm_state.ToString(),
                        Type = ePortType.Integer,
                        ioDirection = ioPortDirection.Output,
                        PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(smokecoalarmdescriptor.co_alarm_state))

                    });
                    thing.Ports.Add(new Port()
                    {

                        Name = nameof(smokecoalarmdescriptor.smoke_alarm_state),
                        State = smokecoalarmdescriptor.smoke_alarm_state.ToString(),
                        Type = ePortType.Integer,
                        ioDirection = ioPortDirection.Output,
                        PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(smokecoalarmdescriptor.smoke_alarm_state))

                    });
#if false
                    thing.Ports.Add(new Port()
                    {

                        Name = nameof(smokecoalarmdescriptor.battery_health),
                        State = smokecoalarmdescriptor.battery_health.ToString(),
                        Type = ePortType.String,
                        ioDirection = ioPortDirection.Output,
                        PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(smokecoalarmdescriptor.battery_health))

                    });
#endif


                    Things.Add(thing);
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }


            }
            return Things;
        }

    }
}
