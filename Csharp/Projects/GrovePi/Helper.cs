using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo.NodeLibrary;

namespace Yodiwo.Projects.GrovePi
{
    public static class Helper
    {
        public static Yodiwo.NodeLibrary.Node node;
        public static ListTS<Thing> Things = new ListTS<Thing>();
        public static Thing BuzzerThing;
        public static Thing RgbLedThing;
        public static Thing SoundSensorThing;
        public static Thing LightSensorThing;
        public static Thing ButtonSensorThing;
        public static Thing RotaryAngleSensorThing;
        public static Thing RelaySensorThing;
        public static Thing HTSensorThing;
        public static Thing UltrasonicSensorThing;
        public static Thing LCDThing;
        //public static Dictionary<string, object> GroveSensors = new Dictionary<string, object>();
        public static Dictionary<Thing, GrovePiSensor> Lookup = new Dictionary<Thing, GrovePiSensor>();
        public static Dictionary<Thing, GPIO> GPIOs = new Dictionary<Thing, GPIO>();

        private static readonly string ThingsConfFileName = "conf_things.json";

        public static void CreateThings(Transport trans, Node node)
        {
            #region READ CONFIGURATION

            var SavedThingsConfig = ReadConfig();
            if (SavedThingsConfig == null)
            {
                DebugEx.Assert("Could not retrieve or create config; startup failed");
                return;
            }

            //use pin configuration from saved conf file
            var buzzerPin = SavedThingsConfig[typeof(Buzzer)].Pin;
            var rgbLedPin = SavedThingsConfig[typeof(RgbLed)].Pin;
            var soundSensorPin = SavedThingsConfig[typeof(SoundSensor)].Pin;
            var lightSensorPin = SavedThingsConfig[typeof(LightSensor)].Pin;
            var buttonPin = SavedThingsConfig[typeof(Button)].Pin;
            var rotaryAnglePin = SavedThingsConfig[typeof(RotaryAngleSensor)].Pin;
            var relayPin = SavedThingsConfig[typeof(Relay)].Pin;
            var htSensorPin = SavedThingsConfig[typeof(TempAndHumidity)].Pin;
            var ultrasonicPin = SavedThingsConfig[typeof(UltraSonicRanger)].Pin;
            var lcdPin = SavedThingsConfig[typeof(LCD)].Pin;

            //use sampling period configuration from saved conf file
            var gpio_Sampling = SavedThingsConfig[typeof(GPIO)].Period;
            var soundSensorSampling = SavedThingsConfig[typeof(SoundSensor)].Period;
            var lightSensorSampling = SavedThingsConfig[typeof(LightSensor)].Period;
            var buttonSampling = SavedThingsConfig[typeof(Button)].Period;
            var rotaryAngleSampling = SavedThingsConfig[typeof(RotaryAngleSensor)].Period;
            var relaySampling = SavedThingsConfig[typeof(Relay)].Period;
            var htSensorSampling = SavedThingsConfig[typeof(TempAndHumidity)].Period;
            var ultrasonicSampling = SavedThingsConfig[typeof(UltraSonicRanger)].Period;

            #endregion


            #region CREATE SENSORS

            /* A2MCU */
            GPIO GPIO_2 = new GPIO(Pin.DigitalPin2, trans, gpio_Sampling);
            GPIO GPIO_3 = new GPIO(Pin.DigitalPin3, trans, gpio_Sampling);
            GPIO GPIO_4 = new GPIO(Pin.DigitalPin4, trans, gpio_Sampling);
            GPIO GPIO_5 = new GPIO(Pin.DigitalPin5, trans, gpio_Sampling);
            GPIO GPIO_6 = new GPIO(Pin.DigitalPin6, trans, gpio_Sampling);
            /* /A2MCU */

            Buzzer buzzer = new Buzzer(GrovePiSensor.PinNameToPin[buzzerPin], trans);
            RgbLed rgbled = new RgbLed(GrovePiSensor.PinNameToPin[rgbLedPin], trans);
            SoundSensor soundSensor = new SoundSensor(GrovePiSensor.PinNameToPin[soundSensorPin], trans, soundSensorSampling);
            LightSensor lightSensor = new LightSensor(GrovePiSensor.PinNameToPin[lightSensorPin], trans, lightSensorSampling);
            Button button = new Button(GrovePiSensor.PinNameToPin[buttonPin], trans, buttonSampling);
            RotaryAngleSensor rotaryAngleSensor = new RotaryAngleSensor(GrovePiSensor.PinNameToPin[rotaryAnglePin], trans, rotaryAngleSampling);
            Relay relaySensor = new Relay(GrovePiSensor.PinNameToPin[relayPin], trans, relaySampling);
            TempAndHumidity htSensor = new TempAndHumidity(GrovePiSensor.PinNameToPin[htSensorPin], trans, htSensorSampling);
            UltraSonicRanger ultrasonicSensor = new UltraSonicRanger(GrovePiSensor.PinNameToPin[ultrasonicPin], trans, ultrasonicSampling);
            LCD lcd = new LCD(GrovePiSensor.PinNameToPin[lcdPin], trans);

            #endregion


            #region SETUP THINGS

            #region Setup A2MCU GPIO things

            List<Thing> gpio_things = new List<Thing>();
            for (int i = 2; i <= 6; i++)
            {
                //var pinConfig = new ConfigParameter() { Name = "Pin", Value = buzzerPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = gpio_Sampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "gpio_thing" + i),
                    Type = ThingTypeLibrary.Gpio.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "GPIO_" + i,
                    Config = new List<ConfigParameter>() { samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/Generic/gpio.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "GPO",
                        PortModelId = ModelTypeLibrary.GpioModel_Id,
                        State = "false",
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "GPO")
                    },
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "GPI",
                        State = "false",
                        PortModelId = ModelTypeLibrary.GpioModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "GPI")
                    }
                };
                thing = node.AddThing(thing);
                gpio_things.Add(thing);
                Things.Add(thing);
            }

            //gpio thing --> gpio sensor class
            GPIOs.Add(gpio_things.ElementAt(0), GPIO_2);
            GPIOs.Add(gpio_things.ElementAt(1), GPIO_3);
            GPIOs.Add(gpio_things.ElementAt(2), GPIO_4);
            GPIOs.Add(gpio_things.ElementAt(3), GPIO_5);
            GPIOs.Add(gpio_things.ElementAt(4), GPIO_6);

            //gpio thing --> generic sensor class
            Lookup.Add(gpio_things.ElementAt(0), GPIO_2);
            Lookup.Add(gpio_things.ElementAt(1), GPIO_3);
            Lookup.Add(gpio_things.ElementAt(2), GPIO_4);
            Lookup.Add(gpio_things.ElementAt(3), GPIO_5);
            Lookup.Add(gpio_things.ElementAt(4), GPIO_6);

            gpio_things.Clear();

            #endregion

            //setup Buzzer  thing
            #region Setup Buzzer thing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = buzzerPin };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "buzzerthing"),
                    Type = ThingTypeLibrary.Buzzer.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "Buzzer",
                    Config = new List<ConfigParameter>() { pinConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/buzzer.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "BuzzerState",
                        State = "false",
                        PortModelId = ModelTypeLibrary.BuzzerModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                BuzzerThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup RgbLed thing 
            #region SetUp RgbLedThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = rgbLedPin };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "RgbLedThing"),
                    Type = ThingTypeLibrary.Lights.Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_BooleanModelType,
                    Name = "RgbLed",
                    Config = new List<ConfigParameter>() { pinConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/Generic/thing-genericlight.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "RgbLedState",
                        State = "false",
                        PortModelId = ModelTypeLibrary.OnOffLightModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                RgbLedThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup SoundSensorThing 
            #region SetUp SoundSensorThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = soundSensorPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = soundSensorSampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "SoundSensorThing"),
                    Type = ThingTypeLibrary.SoundSensor.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "SoundSensor",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/sound.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Value",
                        State = "0",
                        PortModelId=ModelTypeLibrary.SoundSensorModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                SoundSensorThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup Light Sensor thing 
            #region SetUp LightSensorThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = lightSensorPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = lightSensorSampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "LightSensorThing"),
                    Type = ThingTypeLibrary.LightSensor.Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightSensor_NonNormalizedModelType,
                    Name = "LightSensor",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/light.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "Lumens",
                        State = "0",
                        PortModelId = ModelTypeLibrary.Brightness_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                LightSensorThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup Button thing 
            #region SetUp ButtonThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = buttonPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = buttonSampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "ButtonSensorThing"),
                    Type = ThingTypeLibrary.Button.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "Button",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/Generic/thing-genericbutton.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "ButtonState",
                        State = "0",
                        PortModelId= ModelTypeLibrary.ButtonModel_OnOffActuatorId,
                        Type = Yodiwo.API.Plegma.ePortType.Boolean,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                ButtonSensorThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup RotaryAngle Sensor thing 
            #region SetUp RotaryAngleSensorThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = rotaryAnglePin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = rotaryAngleSampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "RotaryAngleSensorThing"),
                    Type = ThingTypeLibrary.Slider.Type + PlegmaAPI.ThingModelTypeSeparatorStr,
                    Name = "Rotary Angle Sensor",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/Generic/thing-slider.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "RotaryAngle",
                        State = "0",
                        PortModelId =ModelTypeLibrary.SliderModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Decimal,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                RotaryAngleSensorThing = thing = node.AddThing(thing);
            }
            #endregion

#if false
            //setup Relay thing 
            #region SetUp RelayThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = relayPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = relaySampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "RelaySensorThing"),
                    Type = ThingTypeLibrary.SwitchActuator.Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.SwitchActuator_RelayModelType,
                    Name = "Relay",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/relay.jpg",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "RelayState",
                        PortModelId = ModelTypeLibrary.RelayModel_RelayId,
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                RelaySensorThing = thing = node.AddThing(thing);
            }
            #endregion
#endif
            //setup Temperature and Humidity Sensor thing 
            #region SetUp HTSensorThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = htSensorPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = htSensorSampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "HTSensorThing"),
                    Type = ThingTypeLibrary.HTSensor.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "Temperature and Humidity Sensor",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/ht.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        PortModelId = ModelTypeLibrary.HTSensorModel_TemperatureSensorId,
                        Name = "HT",
                        State = "0",
                        Type = Yodiwo.API.Plegma.ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                HTSensorThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup UltraSonic Sensor thing 
            #region SetUp UltrasonicThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = ultrasonicPin };
                var samplePeriodConfig = new ConfigParameter() { Name = "SamplePeriod", Value = ultrasonicSampling.ToStringInvariant() };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "UltrasonicSensorThing"),
                    Type = ThingTypeLibrary.ProximitySensor.Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.ProximitySensor_UltrasonicModelType,
                    Name = "Ultrasonic Sensor",
                    Config = new List<ConfigParameter>() { pinConfig, samplePeriodConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/proximity.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Output,
                        Name = "cm",
                        State = "0",
                        PortModelId = ModelTypeLibrary.UltrasonicSensorModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.Integer,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                UltrasonicSensorThing = thing = node.AddThing(thing);
            }
            #endregion

            //setup LCD Sensor thing 
            #region SetUp LCDThing
            {
                var pinConfig = new ConfigParameter() { Name = "Pin", Value = lcdPin };
                var thing = new Yodiwo.API.Plegma.Thing()
                {
                    ThingKey = ThingKey.BuildFromArbitraryString("$NodeKey$", "LCDThing"),
                    Type = ThingTypeLibrary.Lcd.Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault,
                    Name = "LCD",
                    Config = new List<ConfigParameter>() { pinConfig },
                    UIHints = new ThingUIHints()
                    {
                        IconURI = "/Content/img/icons/lcd.png",
                    },
                };
                thing.Ports = new List<Yodiwo.API.Plegma.Port>()
                {
                    new Yodiwo.API.Plegma.Port()
                    {
                        ioDirection = Yodiwo.API.Plegma.ioPortDirection.Input,
                        Name = "Message",
                        State = "0",
                        PortModelId= ModelTypeLibrary.LcdModel_Id,
                        Type = Yodiwo.API.Plegma.ePortType.String,
                        PortKey = PortKey.BuildFromArbitraryString("$ThingKey$", "0")
                    }
                };
                LCDThing = thing = node.AddThing(thing);
            }
            #endregion

            #endregion


            #region LUT CRETION

            Things.Add(BuzzerThing);
            Things.Add(RgbLedThing);
            Things.Add(SoundSensorThing);
            Things.Add(LightSensorThing);
            Things.Add(ButtonSensorThing);
            Things.Add(RotaryAngleSensorThing);
            //Things.Add(RelaySensorThing);
            Things.Add(HTSensorThing);
            Things.Add(UltrasonicSensorThing);
            Things.Add(LCDThing);


            Lookup.Add(BuzzerThing, buzzer);
            Lookup.Add(RgbLedThing, rgbled);
            Lookup.Add(SoundSensorThing, soundSensor);
            Lookup.Add(LightSensorThing, lightSensor);
            Lookup.Add(ButtonSensorThing, button);
            Lookup.Add(RotaryAngleSensorThing, rotaryAngleSensor);
            //Lookup.Add(RelaySensorThing, relaySensor);
            Lookup.Add(HTSensorThing, htSensor);
            Lookup.Add(UltrasonicSensorThing, ultrasonicSensor);
            Lookup.Add(LCDThing, lcd);


            /*
            GroveSensors.Add(BuzzerThing.Name, buzzer);
            GroveSensors.Add(RgbLedThing.Name, rgbled);
            GroveSensors.Add(SoundSensorThing.Name, soundSensor);
            GroveSensors.Add(LightSensorThing.Name, lightSensor);
            GroveSensors.Add(ButtonSensorThing.Name, button);
            GroveSensors.Add(RotaryAngleSensorThing.Name, rotaryAngleSensor);
            GroveSensors.Add(RelaySensorThing.Name, relaySensor);
            GroveSensors.Add(HTSensorThing.Name, htSensor);
            GroveSensors.Add(UltrasonicSensorThing.Name, ultrasonicSensor);
            GroveSensors.Add(LCDThing.Name, lcd);
            */

            #endregion


            #region REGISTER EVENTS

            foreach (var gpio in GPIOs.Values)
                gpio.OnGetContinuousDatacb += data => OnGetContinuousDatacb(gpio, data);

            soundSensor.OnGetContinuousDatacb += data => OnGetContinuousDatacb(soundSensor, data);
            lightSensor.OnGetContinuousDatacb += data => OnGetContinuousDatacb(lightSensor, data);
            button.OnGetContinuousDatacb += data => OnGetContinuousDatacb(button, data);
            rotaryAngleSensor.OnGetContinuousDatacb += data => OnGetContinuousDatacb(rotaryAngleSensor, data);
            relaySensor.OnGetContinuousDatacb += data => OnGetContinuousDatacb(relaySensor, data);
            htSensor.OnGetContinuousDatacb += data => OnGetContinuousDatacb(htSensor, data);
            ultrasonicSensor.OnGetContinuousDatacb += data => OnGetContinuousDatacb(ultrasonicSensor, data);

            #endregion
        }


        static void OnGetContinuousDatacb(GrovePiSensor sensor, object data)
        {
            var thing = Lookup.FirstOrDefault(i => i.Value == sensor).Key;
            if (thing == null)
                return;
            foreach (var port in thing.Ports)
                if (port.ioDirection == ioPortDirection.Output || port.ioDirection == ioPortDirection.InputOutput)
                {
                    Helper.node.SetState(thing, port, data as string);
                    break; //optimization cheat
                }
        }


        #region CONFIG METHODS

        class ThingConfig
        {
            public string Pin;
            public int Period;
        }

        static Dictionary<Type, ThingConfig> ReadConfig()
        {
            Dictionary<Type, ThingConfig> config = null;
            try
            {
                if (File.Exists(ThingsConfFileName))
                {
                    var content = File.ReadAllText(ThingsConfFileName);
                    //deserialize into List of Configurations and pick the active one
                    config = content.FromJSON<Dictionary<Type, ThingConfig>>();
                }
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
            }

            if (config == null)
            {
                config = new Dictionary<Type, ThingConfig>()
                    {
                        { typeof(GPIO),              new ThingConfig() { Pin = "",    Period = 100  } },
                        { typeof(Buzzer),            new ThingConfig() { Pin = "D7",  Period = 0    } },
                        { typeof(RgbLed),            new ThingConfig() { Pin = "D4",  Period = 0    } },
                        { typeof(SoundSensor),       new ThingConfig() { Pin = "A2",  Period = 1000 } },
                        { typeof(LightSensor),       new ThingConfig() { Pin = "A1",  Period = 500  } },
                        { typeof(Button),            new ThingConfig() { Pin = "D2",  Period = 200  } },
                        { typeof(RotaryAngleSensor), new ThingConfig() { Pin = "A0",  Period = 200  } },
                        { typeof(Relay),             new ThingConfig() { Pin = "D3",  Period = 1000 } },
                        { typeof(TempAndHumidity),   new ThingConfig() { Pin = "D6",  Period = 500  } },
                        { typeof(UltraSonicRanger),  new ThingConfig() { Pin = "D8",  Period = 100  } },
                        { typeof(LCD),               new ThingConfig() { Pin = "I2C", Period = 0    } },
                    };

                try
                {
                    File.WriteAllText(ThingsConfFileName, config.ToJSON());
                }
                catch (Exception ex) { DebugEx.TraceErrorException(ex); }
            }
            return config;
        }

        static bool WriteConfig()
        {
            try
            {
                var config = new Dictionary<Type, ThingConfig>();

                foreach (var kv in Lookup)
                {
                    var pinConf = kv.Key.Config.Find(cp => cp.Name == "Pin").Value;
                    var sensor = kv.Value;
                    config.Add(sensor.GetType(), new ThingConfig() { Pin = pinConf, Period = (int)sensor.Watcher.SamplingPeriod });
                }

                File.WriteAllText(ThingsConfFileName, config.ToJSON());
                return true;
            }
            catch (Exception ex)
            {
                DebugEx.TraceErrorException(ex);
                return false;
            }
        }

        public static void CommitConfig()
        {
            WriteConfig();
        }

        #endregion
    }
}
