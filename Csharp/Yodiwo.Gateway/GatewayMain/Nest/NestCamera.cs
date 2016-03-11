using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using FirebaseSharp.Portable;

namespace Yodiwo.Gateway.GatewayMain.Nest
{
    public static class NestCamera
    {
        public static List<Thing> Convert2YodiwoThing(IDataSnapshot snap)
        {
            List<Thing> _Things = new List<Thing>();
            foreach (var camera in snap.Children)
            {
                Camera cameradescriptor = null;
                Thing thing = null;
                cameradescriptor = new Camera();
                foreach (var cameratattribute in camera.Children)
                {
                    var typr = cameradescriptor.GetType();
                    var props = typr.GetProperties();
                    System.Reflection.PropertyInfo propertyinfo = null;
                    foreach (var prop in props)
                    {
                        if (prop.Name == cameratattribute.Key)
                        {
                            propertyinfo = prop;
                            break;
                        }
                    }
                    //var propertyInfo = thermostatdescriptor.GetType().GetProperty(thermostatattribute.Key, BindingFlags.Public | BindingFlags.IgnoreReturn);
                    try
                    {
                        var val = cameratattribute.Value();
                        propertyinfo.SetValue(cameradescriptor, cameratattribute.Value(), null);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                cameradescriptor.device_id = camera.Key.Replace("-", "_");
                try
                {
                    if (!NestModule.nestdescriptors.ContainsKey(camera.Key))
                        NestModule.nestdescriptors.Add(camera.Key.Replace("-", "_"), new NestDescriptor() { type = NestSensor.Cameras, nestthing = cameradescriptor });
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }
                var thingkeystr = ThingKey.BuildFromArbitraryString("$NodeKey$", cameradescriptor.device_id);
                thing = new Thing();
                thing.ThingKey = thingkeystr;
                thing.Name = cameradescriptor.name_long;
                thing.Ports.Add(new Port()
                {

                    Name = nameof(cameradescriptor.is_audio_input_enabled),
                    State = cameradescriptor.is_audio_input_enabled.ToString(),
                    Type = ePortType.Boolean,
                    ioDirection = ioPortDirection.InputOutput,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(cameradescriptor.is_audio_input_enabled))

                });
                thing.Ports.Add(new Port()
                {

                    Name = nameof(cameradescriptor.is_online),
                    State = cameradescriptor.is_online.ToString(),
                    Type = ePortType.Boolean,
                    ioDirection = ioPortDirection.InputOutput,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(cameradescriptor.is_online))

                });
                thing.Ports.Add(new Port()
                {

                    Name = nameof(cameradescriptor.is_video_history_enabled),
                    State = cameradescriptor.is_video_history_enabled.ToString(),
                    Type = ePortType.Boolean,
                    ioDirection = ioPortDirection.InputOutput,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(cameradescriptor.is_video_history_enabled))

                });
                thing.Ports.Add(new Port()
                {

                    Name = nameof(cameradescriptor.is_streaming),
                    State = cameradescriptor.is_streaming.ToString(),
                    Type = ePortType.Boolean,
                    ioDirection = ioPortDirection.InputOutput,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(cameradescriptor.is_streaming))

                });
                thing.Ports.Add(new Port()
                {

                    Name = nameof(cameradescriptor.web_url),
                    State = cameradescriptor.web_url.ToString(),
                    Type = ePortType.String,
                    ioDirection = ioPortDirection.Output,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(cameradescriptor.web_url))

                });
                thing.Ports.Add(new Port()
                {

                    Name = nameof(cameradescriptor.app_url),
                    State = cameradescriptor.app_url.ToString(),
                    Type = ePortType.String,
                    ioDirection = ioPortDirection.Output,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(cameradescriptor.app_url))

                });
                _Things.Add(thing);

            }
            return _Things;

        }
    }
}
