using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using FirebaseSharp.Portable;
using System.Net.Http;
using System.Reflection;

namespace Yodiwo.Gateway.GatewayMain.Nest
{
    public static class NestThermostat
    {
        public static List<Thing> Convert2YodiwoThing(IDataSnapshot snap)
        {
            List<Thing> _Things = new List<Thing>();
            foreach (var thermostat in snap.Children)
            {
                Thermostat thermostatdescriptor = null;
                Thing thing = null;
                thermostatdescriptor = new Thermostat();
                foreach (var thermostatattribute in thermostat.Children)
                {
                    var typr = thermostatdescriptor.GetType();
                    var props = typr.GetProperties();
                    System.Reflection.PropertyInfo propertyinfo = null;
                    foreach (var prop in props)
                    {
                        if (prop.Name == thermostatattribute.Key)
                        {
                            propertyinfo = prop;
                            break;
                        }
                    }
                    //var propertyInfo = thermostatdescriptor.GetType().GetProperty(thermostatattribute.Key, BindingFlags.Public | BindingFlags.IgnoreReturn);
                    try
                    {
                        var val = thermostatattribute.Value();
                        propertyinfo.SetValue(thermostatdescriptor, thermostatattribute.Value(), null);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                thermostatdescriptor.device_id = thermostat.Key.Replace("-", "_");

                try
                {
                    if (!NestModule.nestdescriptors.ContainsKey(thermostat.Key.Replace("-", "_")))
                        NestModule.nestdescriptors.Add(thermostat.Key.Replace("-", "_"), new NestDescriptor() { type = NestSensor.Thermostats, nestthing = thermostatdescriptor });
                }
                catch (Exception ex)
                {
                    DebugEx.TraceErrorException(ex);
                }

                var thingkeystr = ThingKey.BuildFromArbitraryString("$NodeKey$", thermostatdescriptor.device_id);
                thing = new Thing();
                thing.ThingKey = thingkeystr;
                thing.Name = thermostatdescriptor.name_long;
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.ambient_temperature_c),
                    State = (thermostatdescriptor.ambient_temperature_c != null) ? thermostatdescriptor.ambient_temperature_c.ToString() : "0",
                    Type = ePortType.DecimalHigh,
                    ioDirection = ioPortDirection.Output,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.ambient_temperature_c))
                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.ambient_temperature_f),
                    State = (thermostatdescriptor.ambient_temperature_f != null) ? thermostatdescriptor.ambient_temperature_f.ToString() : "0",
                    ioDirection = ioPortDirection.Output,
                    Type = ePortType.DecimalHigh,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.ambient_temperature_f))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.target_temperature_c),
                    State = (thermostatdescriptor.target_temperature_c != null) ? thermostatdescriptor.target_temperature_c.ToString() : "0",
                    Type = ePortType.DecimalHigh,
                    ioDirection = ioPortDirection.InputOutput,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.target_temperature_c))
                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.target_temperature_f),
                    State = (thermostatdescriptor.target_temperature_f != null) ? thermostatdescriptor.target_temperature_f.ToString() : "0",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.Decimal,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.target_temperature_f))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.humidity),
                    State = (thermostatdescriptor.humidity != null) ? thermostatdescriptor.humidity.ToString() : "0",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.Integer,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.humidity))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.has_fan),
                    State = (thermostatdescriptor.has_fan != null) ? thermostatdescriptor.has_fan.ToString() : "false",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.Boolean,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.has_fan))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.can_cool),
                    State = (thermostatdescriptor.can_cool != null) ? thermostatdescriptor.can_cool.ToString() : "false",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.Boolean,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.can_cool))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.can_heat),
                    State = (thermostatdescriptor.can_heat != null) ? thermostatdescriptor.can_heat.ToString() : "false",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.Boolean,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.can_heat))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.hvac_mode),
                    State = (thermostatdescriptor.hvac_mode != null) ? thermostatdescriptor.hvac_mode.ToString() : "false",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.String,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.hvac_mode))

                });
                thing.Ports.Add(new Port()
                {
                    Name = nameof(thermostatdescriptor.hvac_state),
                    State = (thermostatdescriptor.hvac_state != null) ? thermostatdescriptor.hvac_state.ToString() : "heating",
                    ioDirection = ioPortDirection.InputOutput,
                    Type = ePortType.String,
                    PortKey = PortKey.BuildFromArbitraryString(thingkeystr, nameof(thermostatdescriptor.hvac_state))

                });

                _Things.Add(thing);
            }
            return _Things;
        }
    }

}

