using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    /*
     * TODOS
     * 
     * - currently if two similar Things (e.g. two different Leds) with different ePortTypes (e.g. boolean + integer)
     * then they are grouped under the same ThingType but in different modelTypes.
     * 
     * - write xml on ThingTypeLibrary's class
    */
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// 
    /// </summary>
    public static class ThingTypeLibrary
    {
        public static DictionaryTS<string, ThingType> Type2ThingType = new DictionaryTS<string, ThingType>();

        static ThingTypeLibrary()
        {
            try
            {
                var fields = typeof(ThingTypeLibrary).GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (var field in fields)
                {
                    var name = field.Name;
                    if (field.FieldType == typeof(ThingType))
                    {
                        try
                        {
                            var thingType = (ThingType)field.GetValue(null);
                            var strType = thingType.Type;
                            Type2ThingType.Add(strType, thingType);
                        }
                        catch (Exception ex) { DebugEx.TraceErrorException(ex); }
                    }
                }
            }
            catch (Exception ex) { DebugEx.Assert(ex, "ThingTypeLibrary"); }
        }

        #region Thing Types

        #region GPIO
        public const string Gpio_Type = "com.yodiwo.inout.gpios";
        public static ThingType Gpio = new ThingType
        {
            Type = Gpio_Type,
            Description = "GPIOs",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.GpioModel },
            },
        };

        #endregion

        #region Text in and Out
        public const string TextIO_Type = "com.yodiwo.inout.text";
        public static ThingType TextIO = new ThingType
        {
            Type = TextIO_Type,
            Description = "Text in and Text Out",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.TextIO },
            },
        };
        #endregion

        #region simple ThingIn ON-OFF switches

        public const string SwitchActuator_Type = "com.yodiwo.out.switches.onoff";
        public const string SwitchActuator_CheckBoxModelType = "checkbox";
        public const string SwitchActuator_RelayModelType = "relay";
        public static ThingType SwitchActuator = new ThingType
        {
            Type = SwitchActuator_Type,
            Description = "On/Off Switch Actuators",
            Models = new Dictionary<string, ThingModelType>()
            {
                 { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.OnOffSwitchActuatorModel },
                 { SwitchActuator_CheckBoxModelType, ModelTypeLibrary.CheckboxActuatorModel },
                 { SwitchActuator_RelayModelType, ModelTypeLibrary.RelayModel },
            },
        };

        #endregion

        #region simple ThingOut ON-OFF switches

        public const string Switch_Type = "com.yodiwo.in.switches.onoff";
        public const string Switch_CheckBoxModelType = "checkbox";
        public static ThingType Switch = new ThingType
        {
            Type = Switch_Type,
            Description = "On/Off Switch Sinks",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.OnOffSwitchSinkModel },
                { Switch_CheckBoxModelType, ModelTypeLibrary.CheckboxSinkModel },

            },
        };

        #endregion

        #region LightControls ThingIn
        public const string LightControls_Type = "com.yodiwo.out.light.controls";
        public const string LightControls_DimmerModelType = "dimmer";
        public static ThingType LightControls = new ThingType
        {
            Type = LightControls_Type,
            Description = "Light Controls",
            Models = new Dictionary<string, ThingModelType>()
            {
                { LightControls_DimmerModelType, ModelTypeLibrary.DimmerActuatorModel },
            },
        };

        #endregion

        #region Lights ThingOut

        public const string Lights_Type = "com.yodiwo.in.lights";
        public const string Lights_DimmerModelType = "dimmer";
        public const string Lights_BooleanModelType = "lights";
        //???
        public static ThingType Lights = new ThingType
        {
            Type = Lights_Type,
            Description = "Light",
            Models = new Dictionary<string, ThingModelType>()
            {
              { Lights_DimmerModelType,  ModelTypeLibrary.DimmerSinkModel },
              { Lights_BooleanModelType, ModelTypeLibrary.OnOffLightModel },
            },
        };

        #endregion

        #region typewriter ThingIn
        public const string TextOut_Type = "com.yodiwo.out.text";
        public const string TextOut_ConsoleModelType = "Console";
        public static ThingType TextOut = new ThingType
        {
            Type = TextOut_Type,
            Description = "TypeWriter",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.TypeWriterModel },
                { TextOut_ConsoleModelType, ModelTypeLibrary.ConsoleModel }
            },
        };

        #endregion

        #region textable ThingOut
        public const string TextIn_Type = "com.yodiwo.in.text";
        public const string TextIn_ConsoleModelType = "Console";
        public static ThingType TextIn = new ThingType
        {
            Type = TextIn_Type,
            Description = "Text",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeSeparatorPlusDefault, ModelTypeLibrary.TextableModel },
                { TextIn_ConsoleModelType, ModelTypeLibrary.ConsoleModel }
            },
        };

        #endregion

        #region slider ThingIn

        public const string Slider_Type = "com.yodiwo.out.seekbars";
        public static ThingType Slider = new ThingType
        {
            Type = Slider_Type,
            Description = "Slider",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.SliderModel },
            },
        };

        #endregion

        #region progressbar ThingOut

        public const string Progressbar_Type = "com.yodiwo.in.seekbars";
        public static ThingType Progressbar = new ThingType
        {
            Type = Progressbar_Type,
            Description = "Progress bars",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.ProgressbarModel },
            },
        };

        #endregion

        #region ON-OFF buttons ThingIn
        public const string Button_Type = "com.yodiwo.out.buttons";
        public static ThingType Button = new ThingType
        {
            Type = Button_Type,
            Description = "Button Actuators",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.ButtonModel },
            },
        };

        #endregion

        #region Location ThingIn
        public const string LocationCoordinates_Type = "com.yodiwo.out.location.coordinates";
        public static ThingType LocationCoordinates = new ThingType
        {
            Type = LocationCoordinates_Type,
            Description = "Location Coordinates",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.LocationCoordinatesModel },
            },
        };

        public const string LocationAddress_Type = "com.yodiwo.out.location.address";
        public static ThingType LocationAddress = new ThingType
        {
            Type = LocationAddress_Type,
            Description = "Location Address",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.LocationAddressModel },
        },
        };

        #endregion

        #region Wifi ThingIn
        public const string WifiInfo_Type = "com.yodiwo.out.wifi.info";
        public static ThingType WifiInfo = new ThingType
        {
            Type = WifiInfo_Type,
            Description = "Wifi Info",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.WifiInfoModel },
            },
        };
        public const string WifiStrength_Type = "com.yodiwo.out.wifi.strength";
        public static ThingType WifiStrength = new ThingType
        {
            Type = WifiStrength_Type,
            Description = "Wifi Strength",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.WifiStrengthModel },
            },
        };

        #endregion

        #region Bluetooth ThingIn 

        public const string Bluetooth_Type = "com.yodiwo.out.bluetooth";
        public static ThingType Bluetooth = new ThingType
        {
            Type = Bluetooth_Type,
            Description = "Bluetooth",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.BluetoothModel },
            },
        };

        #endregion

        #region Nfc ThingIn
        public const string Nfc_Type = "com.yodiwo.out.nfc";
        public static ThingType Nfc = new ThingType
        {
            Type = Nfc_Type,
            Description = "Nfc",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.NfcModel },
            },
        };

        #endregion

        #region Shake detector ThingIn
        public const string ShakeDetector_Type = "com.yodiwo.out.shakedetectors";
        public static ThingType ShakeDetector = new ThingType
        {
            Type = ShakeDetector_Type,
            Description = "Shake Detectors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.ShakeDetectorModel },
            },
        };

        #endregion

        #region Android Intent ThingOut
        public const string AndroidIntent_Type = "com.yodiwo.in.androidintent";
        public static ThingType AndroidIntent = new ThingType
        {
            Type = AndroidIntent_Type,
            Description = "Android Intents",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.AndroidIntentModel },
            },
        };

        #endregion

        #region Camera ThingIn
        public const string Camera_Type = "com.yodiwo.out.cameras";
        public static ThingType Camera = new ThingType
        {
            Type = Camera_Type,
            Description = "Cameras",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.CameraModel },
            },
        };

        #endregion

        #region Microphone ThingIn
        public const string Microphone_Type = "com.yodiwo.out.microphones";
        public static ThingType Microphone = new ThingType
        {
            Type = Microphone_Type,
            Description = "Microphones",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.MicrophoneModel },
            },
        };

        #endregion

        #region Buzzer ThingOut
        public const string Buzzer_Type = "com.yodiwo.in.buzzers";
        public static ThingType Buzzer = new ThingType
        {
            Type = Buzzer_Type,
            Description = "Buzzers",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.BuzzerModel },
            },
        };

        #endregion

        #region LCD thingOut
        public const string Lcd_Type = "com.yodiwo.in.lcds";
        public static ThingType Lcd = new ThingType
        {
            Type = Lcd_Type,
            Description = "LCDs",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.LcdModel },
            },
        };

        #endregion

        #region SipPhone thingOut

        public const string SipPhone_Type = "com.yodiwo.in.sipphones";
        public static ThingType SipPhone = new ThingType
        {
            Type = SipPhone_Type,
            Description = "SipPhone",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.SipPhoneModel },
            },
        };

        #endregion

        #region Speech recognition thingIn

        public const string SpeechRecognition_Type = "com.yodiwo.out.speechrecognition";
        public static ThingType SpeechRecognition = new ThingType
        {
            Type = SpeechRecognition_Type,
            Description = "SpeechRecognition",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.SpeechRecognitionModel },
            },
        };

        #endregion

        #region Text2Speech thingOut
        public const string Text2Speech_Type = "com.yodiwo.in.text2speech";
        public static ThingType Text2Speech = new ThingType
        {
            Type = Text2Speech_Type,
            Description = "Text To Speech",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.Text2SpeechModel },
            },
        };

        #endregion

        #region Gesture ThingIn

        public const string GestureSensor_Type = "com.yodiwo.out.sensors.gesture";
        public static ThingType GestureSensor = new ThingType
        {
            Type = GestureSensor_Type,
            Description = "Gesture Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.GestureSensorModel },
            },
        };

        #endregion

        #region Position sensor ThingIn

        public const string PositionSensor_Type = "com.yodiwo.out.sensors.position";
        public static ThingType PositionSensor = new ThingType
        {
            Type = PositionSensor_Type,
            Description = "Position Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.PositionSensorModel },
            },
        };

        #endregion

        #region Beacon detector ThingIn

        public const string BeaconDetector_Type = "com.yodiwo.out.beacon";
        public static ThingType BeaconDetector = new ThingType
        {
            Type = BeaconDetector_Type,
            Description = "Beacon Detector",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.BeaconDetectorModel },
            },
        };

        #endregion

        #region Accelerometer Sensor ThingIn
        public const string AccelerometerSensor_Type = "com.yodiwo.out.sensors.accelerometer";
        public static ThingType AccelerometerSensor = new ThingType
        {
            Type = AccelerometerSensor_Type,
            Description = "Accelerometer Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.AccelerometerSensorModel },
            },
        };

        #endregion

        #region Rotation Sensor ThingIn
        public const string RotationSensor_Type = "com.yodiwo.out.sensors.rotation";
        public static ThingType RotationSensor = new ThingType
        {
            Type = RotationSensor_Type,
            Description = "Rotation Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.RotationSensorModel },
            },
        };

        #endregion

        #region Rotation  EulerSensor ThingIn
        public const string RotationEulerSensor_Type = "com.yodiwo.out.sensors.rotationeuler";
        public static ThingType RotationEulerSensor = new ThingType
        {
            Type = RotationEulerSensor_Type,
            Description = "Rotation Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.RotationEulerSensorModel },

            },
        };
        #endregion

        #region Gyroscope Sensor ThingIn
        public const string GyroscopeSensor_Type = "com.yodiwo.out.sensors.gyroscope";
        public static ThingType GyroscopeSensor = new ThingType
        {
            Type = GyroscopeSensor_Type,
            Description = "Gyroscope Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.GyroscopeSensorModel },
            },
        };

        #endregion

        #region Light sensors ThingIn
        public const string LightSensor_Type = "com.yodiwo.out.sensors.light";
        public const string LightSensor_NonNormalizedModelType = "nonNormalized";
        public static ThingType LightSensor = new ThingType
        {
            Type = LightSensor_Type,
            Description = "Light Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.BrightnessNormalizedModel },
                { LightSensor_NonNormalizedModelType, ModelTypeLibrary.BrightnessModel },
            },
        };

        #endregion

        #region Magnetometer Sensor ThingIn

        public const string MagnetometerSensor_Type = "com.yodiwo.out.sensors.magnetometer";
        public static ThingType MagnetometerSensor = new ThingType
        {
            Type = MagnetometerSensor_Type,
            Description = "Magnetometers",
            Models = new Dictionary<string, ThingModelType>()
             {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.MagnetometerSensorModel },
            },
        };

        #endregion

        #region Proximity Sensor ThingIn

        public const string ProximitySensor_Type = "com.yodiwo.out.sensors.proximity";
        public const string ProximitySensor_UltrasonicModelType = "ultrasonic";
        public static ThingType ProximitySensor = new ThingType
        {
            Type = ProximitySensor_Type,
            Description = "Proximity Sensors",
            Models = new Dictionary<string, ThingModelType>()
             {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.ProximitySensorModel },
                { ProximitySensor_UltrasonicModelType, ModelTypeLibrary.UltrasonicSensorModel },
            },
        };

        #endregion

        #region Sound sensor ThingIn

        public const string SoundSensor_Type = "com.yodiwo.out.sensors.sound";
        public static ThingType SoundSensor = new ThingType
        {
            Type = SoundSensor_Type,
            Description = "Sound Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.SoundSensorModel },
            },
        };

        #endregion

        #region Humidity sensor ThingIn

        public const string HumiditySensor_Type = "com.yodiwo.out.sensors.humidity";
        public static ThingType HumiditySensor = new ThingType
        {
            Type = HumiditySensor_Type,
            Description = "Humidity Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.HumiditySensorModel },
            },
        };

        #endregion

        #region Temperature sensor ThingIn
        public const string TemperatureSensor_Type = "com.yodiwo.out.sensors.temperature";
        public static ThingType TemperatureSensor = new ThingType
        {
            Type = TemperatureSensor_Type,
            Description = "Temperature Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.TemperatureSensorModel },
            },
        };

        #endregion

        #region Enviromental sensor ThingIn
        public const string HTSensor_Type = "com.yodiwo.out.sensors.ht";
        public static ThingType HTSensor = new ThingType
        {
            Type = HTSensor_Type,
            Description = "Humidity and Temperature Sensor",
            Models = new Dictionary<string, ThingModelType>()
            {
                { PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.HTSensorModel },
            },
        };

        #endregion

        #region grouped connectivity ThingIn
        public const string Connectivity_Type = "com.yodiwo.out.connectivity";
        public static ThingType Connectivity = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Connectivity",
            Models = new Dictionary<string, ThingModelType>()
            {
                {"Nfc", ModelTypeLibrary.NfcModel },
                {"WifiInfo", ModelTypeLibrary.WifiInfoModel },
                {"WifiStrength", ModelTypeLibrary.WifiStrengthModel },
                {"Bluetooth", ModelTypeLibrary.BluetoothModel },
            },
        };
        #endregion

        #region FleetMgr ThingOut
        public const string FleetMgr_Type = "com.yodiwo.in.fleetmgr.default";
        public static ThingType FleetMgr = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Fleet Manager",
            Models = new Dictionary<string, ThingModelType>()
            {
                {PlegmaAPI.ThingModelTypeDefault, ModelTypeLibrary.FleetMgrModel },

            },
        };
        #endregion

        #region Displayer ThingOut
        public const string Displayer_Type = "com.yodiwo.in.displayer.default";
        public static ThingType Displayer = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Displayer",
        };
        #endregion

        #region Displayer ThingOut
        public const string Gallery_Type = "com.yodiwo.in.gallery.default";
        public static ThingType Gallery = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Gallery",
        };
        #endregion

        #region Flickr ThingOut
        public const string Flickr_Type = "com.yodiwo.flickr.default";
        public static ThingType Flickr = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Flickr",
        };
        #endregion

        #region RegionViewer ThingOut
        public const string RegionViewer_Type = "com.yodiwo.in.regionviewer.default";
        public static ThingType RegionViewer = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Region Viewer",
        };
        #endregion

        #region RegionViewer ThingIn
        public const string Label_Type = "com.yodiwo.out.label.default";
        public static ThingType Label = new ThingType
        {
            Type = Connectivity_Type,
            Description = "Label",
        };
        #endregion


        #endregion //of Thing Types
    }


    public class ModelTypeLibrary
    {
        #region Model Types

        public const string OnOffSwitchActuatorModel_Id = "OnOffSwitchActuator";
        public static ThingModelType OnOffSwitchActuatorModel = new ThingModelType
        {
            Name = "Simple On/Off Switch Actuator",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple On/Off Switch Actuator",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    OnOffSwitchActuatorModel_Id,
                    new PortDescription()
                    {
                        Description = "On/Off",
                        Id = OnOffSwitchActuatorModel_Id,
                        Label = "On/Off",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        },
                    }
                }
            }
        };


        public const string OnOffSwitchSinkModel_Id = "OnOffSwitchSink";
        public static ThingModelType OnOffSwitchSinkModel = new ThingModelType
        {
            Name = "Simple On/Off Switch Sink",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "On/Off Switch Sink",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    OnOffSwitchSinkModel_Id,
                    new PortDescription()
                    {
                        Description = "On/Off",
                        Id = OnOffSwitchSinkModel_Id,
                        Label = "On/Off",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        },
                    }
                }
            }
        };

        public const string DimmerActuatorModel_Id = "DimmerActuator";
        public static ThingModelType DimmerActuatorModel = new ThingModelType
        {
            Name = "Simple Dimmer Actuator",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Dimmer Actuator",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    DimmerActuatorModel_Id,
                    new PortDescription()
                    {
                        Description = "Dimmer Actuator",
                        Id = DimmerActuatorModel_Id,
                        Label = "Dimmer Actuator",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.Decimal,
                        }
                    }
                }
            }
        };

        public const string DimmerSinkModel_Id = "Dimmer";
        public const string DimmerSinkModel_NormalizedId = "NormalizedDimmer";
        public static ThingModelType DimmerSinkModel = new ThingModelType
        {
            Name = "Simple Dimmer Sink",
            Id = ThingTypeLibrary.Lights_DimmerModelType,
            Description = "Simple Dimmer Sink",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    DimmerSinkModel_Id,
                    new PortDescription()
                    {
                        Description = "Dimmer Sink",
                        Id = DimmerSinkModel_Id,
                        Label = "Dimmer Sink",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 100.0,
                            Step = 1.0,
                            Type = ePortType.Decimal,
                        }
                    }
                },
                {
                    DimmerSinkModel_NormalizedId,
                    new PortDescription()
                    {
                        Description = "Dimmable Light",
                        Id = DimmerSinkModel_NormalizedId,
                        Label = "Dimmable Light",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.Decimal,
                        }
                    }
                }
            }
        };


        public const string ButtonModel_OnOffActuatorId = "OnOffActuator";
        public static ThingModelType ButtonModel = new ThingModelType
        {
            Name = "Simple On/Off Actuator",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple On/Off Actuator",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    ButtonModel_OnOffActuatorId,
                    new PortDescription
                    {
                        Description = "On/Off Actuator",
                        Id = ButtonModel_OnOffActuatorId,
                        Label = "On/Off Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string TypeWriterModel_Id = "TypeWriter";
        public static ThingModelType TypeWriterModel = new ThingModelType
        {
            Name = "Simple Typewriter",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Typewriter",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    TypeWriterModel_Id,
                    new PortDescription()
                    {
                        Description = "Typewriter",
                        Id = TypeWriterModel_Id,
                        Label = "Typewriter",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string TextableModel_Id = "Textable";
        public static ThingModelType TextableModel = new ThingModelType
        {
            Name = "Simple Textable",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Textable",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    TextableModel_Id,
                    new PortDescription()
                    {
                        Description = "Textable",
                        Id = TextableModel_Id,
                        Label = "Textable",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string ConsoleModel_Id = "Console";                //TODO: how does this differ from the prev. model?
        public static ThingModelType ConsoleModel = new ThingModelType
        {
            Name = "Simple Console",
            Id = ThingTypeLibrary.TextIn_ConsoleModelType,
            Description = "Simple Console",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    ConsoleModel_Id,
                    new PortDescription()
                    {
                        Description = "Console",
                        Id = ConsoleModel_Id,
                        Label = "Console",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string CheckboxActuatorModel_Id = "CheckBoxActuator";
        public static ThingModelType CheckboxActuatorModel = new ThingModelType
        {
            Name = "Simple Checkbox Actuator",
            Id = ThingTypeLibrary.SwitchActuator_CheckBoxModelType,
            Description = "Simple Checkbox Actuator",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    CheckboxActuatorModel_Id,
                    new PortDescription()
                    {
                        Description = "Checkbox Actuator",
                        Id = CheckboxActuatorModel_Id,
                        Label = "Checkbox Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string CheckboxSinkModel_Id = "CheckBoxSink";
        public static ThingModelType CheckboxSinkModel = new ThingModelType
        {
            Name = "Simple Checkbox Sink",
            Id = ThingTypeLibrary.Switch_CheckBoxModelType,
            Description = "Simple Checkbox Sink",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    CheckboxSinkModel_Id,
                    new PortDescription()
                    {
                        Description = "Checkbox Sink",
                        Id = CheckboxSinkModel_Id,
                        Label = "Checkbox Sink",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string SliderModel_Id = "Slider";
        public static ThingModelType SliderModel = new ThingModelType
        {
            Name = "Simple Slider",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Slider",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    SliderModel_Id,
                    new PortDescription()
                    {
                        Description = "Slider",
                        Id = SliderModel_Id,
                        Label = "Slider",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.Decimal,
                        }
                    }
                }
            }
        };

        public const string ProgressbarModel_Id = "ProgressBar";
        public static ThingModelType ProgressbarModel = new ThingModelType
        {
            Name = "Simple Progress Bar",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Progress Bar",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    ProgressbarModel_Id,
                    new PortDescription()
                    {
                        Description = "Progress bar",
                        Id = ProgressbarModel_Id,
                        Label = "Progress bar",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string OnOffLightModel_Id = "OnOffLight";
        public static ThingModelType OnOffLightModel = new ThingModelType
        {
            Name = "Simple On/Off Light",
            Id = ThingTypeLibrary.Lights_BooleanModelType,
            Description = "Simple On/Off Light",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    OnOffLightModel_Id,
                    new PortDescription()
                    {
                        Description = "On/Off Light",
                        Id = PlegmaAPI.ThingModelTypeDefault,
                        Label = "On/Off Light",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string LocationCoordinatesModel_Id = "LocationCoordinates";
        public static ThingModelType LocationCoordinatesModel = new ThingModelType
        {
            Name = "Mobile Location Coordinates",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Mobile Location Coordinates",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    LocationCoordinatesModel_Id,
                    new PortDescription()
                    {
                        Description = "Location Coordinates",
                        Id = LocationCoordinatesModel_Id,
                        Label = "Location Coordinates",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string LocationAddressModel_Id = "LocationAddress";
        public static ThingModelType LocationAddressModel = new ThingModelType
        {
            Name = "Mobile Location Address",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Mobile Location Address",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    LocationAddressModel_Id,
                    new PortDescription()
                    {
                        Description = "Location Address",
                        Id = LocationAddressModel_Id,
                        Label = "Location Address",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string BluetoothModel_Id = "Bluetooth";
        public static ThingModelType BluetoothModel = new ThingModelType
        {
            Name = "Simple Bluetooth",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Bluetooth",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    BluetoothModel_Id,
                    new PortDescription()
                    {
                        Description = "Bluetooth",
                        Id = BluetoothModel_Id,
                        Label = "Bluetooth",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string AccelerometerSensorModel_XId = "X";
        public const string AccelerometerSensorModel_YId = "Y";
        public const string AccelerometerSensorModel_ZId = "Z";
        public static ThingModelType AccelerometerSensorModel = new ThingModelType
        {
            Name = "Simple Accelerometer",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Accelerometer",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    AccelerometerSensorModel_XId,
                    new PortDescription()
                    {
                        Description = "Accelerometer",
                        Id = AccelerometerSensorModel_XId,
                        Label = "Accelerometer",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    AccelerometerSensorModel_YId,
                    new PortDescription()
                    {
                        Description = "Accelerometer",
                        Id = AccelerometerSensorModel_YId,
                        Label = "Accelerometer",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    AccelerometerSensorModel_ZId,
                    new PortDescription()
                    {
                        Description = "Accelerometer",
                        Id = AccelerometerSensorModel_ZId,
                        Label = "Accelerometer",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string GyroscopeSensorModel_XId = "X";
        public const string GyroscopeSensorModel_YId = "Y";
        public const string GyroscopeSensorModel_ZId = "Z";
        public static ThingModelType GyroscopeSensorModel = new ThingModelType
        {
            Name = "Simple Gyroscope",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Gyroscope",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    GyroscopeSensorModel_XId,
                    new PortDescription()
                    {
                        Description = "Gyroscope",
                        Id = GyroscopeSensorModel_XId,
                        Label = "Gyroscope",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    GyroscopeSensorModel_YId,
                    new PortDescription()
                    {
                        Description = "Gyroscope",
                        Id = GyroscopeSensorModel_YId,
                        Label = "Gyroscope",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    GyroscopeSensorModel_ZId,
                    new PortDescription()
                    {
                        Description = "Gyroscope",
                        Id = GyroscopeSensorModel_ZId,
                        Label = "Gyroscope",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string RotationSensorModel_WId = "W";
        public const string RotationSensorModel_XId = "X";
        public const string RotationSensorModel_YId = "Y";
        public const string RotationSensorModel_ZId = "Z";
        public static ThingModelType RotationSensorModel = new ThingModelType
        {
            Name = "Simple Rotation",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simpleoattion",
            Ports = new Dictionary<string, PortDescription>
            {
                {
                    RotationSensorModel_WId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorModel_WId,
                        Label = "Rotation",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    RotationSensorModel_XId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorModel_XId,
                        Label = "Rotation",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    RotationSensorModel_YId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorModel_YId,
                        Label = "Rotation",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    RotationSensorModel_ZId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorModel_ZId,
                        Label = "Rotation",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string RotationEulerSensorModel_PhiId = "Phi";
        public const string RotationEulerSensorModel_ThetaId = "Theta";
        public const string RotationEulerSensorModel_PsiId = "Psi";

        public static ThingModelType RotationEulerSensorModel = new ThingModelType
        {
            Name = "Simple Rotation Euler",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Rotation Euler",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    RotationEulerSensorModel_PhiId,
                    new PortDescription()
                    {
                        Description = "Rotation EUler",
                        Id = RotationEulerSensorModel_PhiId,
                        Label = "Rotation Euler",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    RotationEulerSensorModel_ThetaId,
                    new PortDescription()
                    {
                        Description = "Rotation EUler",
                        Id = RotationEulerSensorModel_ThetaId,
                        Label = "Rotation Euler",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    RotationEulerSensorModel_PsiId,
                    new PortDescription()
                    {
                        Description = "Rotation EUler",
                        Id = RotationEulerSensorModel_PsiId,
                        Label = "Rotation Euler",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string NfcModel_Id = "Nfc";
        public static ThingModelType NfcModel = new ThingModelType
        {
            Name = "Simple Nfc",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Nfc",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    NfcModel_Id,
                    new PortDescription()
                    {
                        Description = "Nfc",
                        Id = NfcModel_Id,
                        Label = "Nfc",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string BrightnessNormalizedModel_Id = "BrightnessNormalized";
        public static ThingModelType BrightnessNormalizedModel = new ThingModelType
        {
            Name = "Simple Brightness",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Brightness",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    BrightnessNormalizedModel_Id,
                    new PortDescription()
                    {
                        Description = "Brightness",
                        Id = BrightnessNormalizedModel_Id,
                        Label = "Brightness",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string Brightness_Id = "Brightness";
        public static ThingModelType BrightnessModel = new ThingModelType
        {
            Name = "Simple Light Sensor",
            Id = ThingTypeLibrary.LightSensor_NonNormalizedModelType,
            Description = "Simple Light Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    Brightness_Id,
                    new PortDescription()
                    {
                        Description = "Light Sensor",
                        Id = Brightness_Id,
                        Label = "Light Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0,
                            Maximum = 255,  //TODO: This very much depends on actual sensor so it either has to be overriden and/or we need to set it much higher
                            Step = 1,
                            Type = ePortType.Integer,
                        }
                    }
                }
            }
        };

        public const string ProximitySensorModel_Id = "Proximity";
        public static ThingModelType ProximitySensorModel = new ThingModelType
        {
            Name = "Simple Proximity",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Proximity",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    ProximitySensorModel_Id,
                    new PortDescription()
                    {
                        Description = "Proximity",
                        Id = ProximitySensorModel_Id,
                        Label = "Proximity",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string WifiInfoModel_Id = "WifiInfo";
        public static ThingModelType WifiInfoModel = new ThingModelType
        {
            Name = "Simple Wifi Info",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Wifi Info",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    WifiInfoModel_Id,
                    new PortDescription()
                    {
                        Description = "Wifi Info",
                        Id = WifiInfoModel_Id,
                        Label = "Wifi Info",
                        State = new StateDescription()
                        {
                            Type = ePortType.Integer,
                        }
                    }
                }
            }
        };

        public const string WifiStrengthModel_Id = "WifiStrength";
        public static ThingModelType WifiStrengthModel = new ThingModelType
        {
            Name = "Simple Wifi Strength",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Mobile Wifi Strength",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    WifiStrengthModel_Id,
                    new PortDescription()
                    {
                        Description = "Wifi Strength",
                        Id = WifiStrengthModel_Id,
                        Label = "Wifi Strength",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string ShakeDetectorModel_Id = "ShakeDetector";
        public static ThingModelType ShakeDetectorModel = new ThingModelType
        {
            Name = "Simple Shake Detector",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Shake Detector",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    ShakeDetectorModel_Id, new PortDescription()
                    {
                        Description = "Shake Detector",
                        Id = ShakeDetectorModel_Id,
                        Label = "Shake Detector",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string CameraModel_Id = "Camera Feed";
        public static ThingModelType CameraModel = new ThingModelType
        {
            Name = "Simple Camera",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Camera",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    CameraModel_Id,
                    new PortDescription()
                    {
                        Description = "Camera",
                        Id = CameraModel_Id,
                        Label = "Camera",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string MicrophoneModel_Id = "Microphone";
        public static ThingModelType MicrophoneModel = new ThingModelType
        {
            Name = "Simple Microphone",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Microphone",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    MicrophoneModel_Id,
                    new PortDescription()
                    {
                        Description = "Microphone",
                        Id = MicrophoneModel_Id,
                        Label = "Microphone",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string SipPhoneModel_Id = "SipPhone";
        public static ThingModelType SipPhoneModel = new ThingModelType
        {
            Name = "Simple SipPhone",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple SipPhone",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    SipPhoneModel_Id,
                    new PortDescription()
                    {
                        Description = "SipPhone",
                        Id = SipPhoneModel_Id,
                        Label = "SipPhone",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string SpeechRecognitionModel_Id = "SpeechRecognition";
        public static ThingModelType SpeechRecognitionModel = new ThingModelType
        {
            Name = "Simple Speech Recognition",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple SpeechRecognition",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    SpeechRecognitionModel_Id,
                    new PortDescription()
                    {
                        Description = "SpeechRecognition",
                        Id = SpeechRecognitionModel_Id,
                        Label = "SpeechRecognition",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string Text2SpeechModel_Id = "Text2Speech";
        public static ThingModelType Text2SpeechModel = new ThingModelType
        {
            Name = "Simple Text to Speech",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Text to Speech",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    Text2SpeechModel_Id,
                    new PortDescription()
                    {
                        Description = "Text to Speech",
                        Id = Text2SpeechModel_Id,
                        Label = "Text to Speech",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string SoundSensorModel_Id = "SoundSensor";
        public static ThingModelType SoundSensorModel = new ThingModelType
        {
            Name = "Simple Sound Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Sound Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    SoundSensorModel_Id,
                    new PortDescription()
                    {
                        Description = "Sound Sensor",
                        Id = SoundSensorModel_Id,
                        Label = "Sound Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0,
                            Maximum = 255,
                            Step = 1,
                            Type = ePortType.Integer,
                        }
                    }
                }
            }
        };

        public const string HumiditySensorModel_Id = "HumiditySensor";
        public static ThingModelType HumiditySensorModel = new ThingModelType
        {
            Name = "Simple Humidity Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Humidity Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    HumiditySensorModel_Id,
                    new PortDescription()
                    {
                        Description = "Humidity Sensor",
                        Id = HumiditySensorModel_Id,
                        Label = "Humidity Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 100.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string BuzzerModel_Id = "Buzzer";
        public static ThingModelType BuzzerModel = new ThingModelType
        {
            Name = "Simple Buzzer",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Buzzer",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    BuzzerModel_Id,
                    new PortDescription()
                    {
                        Description = "Buzzer",
                        Id = BuzzerModel_Id,
                        Label = "Buzzer",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.Decimal,   //TODO: decimal buzzer?
                        }
                    }
                }
            }
        };

        public const string HTSensorModel_TemperatureSensorId = "TemperatureSensor";
        public const string HTSensorModel_HumiditySensorId = "HumiditySensor";

        public static ThingModelType HTSensorModel = new ThingModelType
        {
            Name = "Humidity and Temperature Sensor Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "HT Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    HTSensorModel_TemperatureSensorId,
                    new PortDescription()
                    {
                        Description = "Temperature Sensor",
                        Id = HTSensorModel_TemperatureSensorId,
                        Label = "Temperature Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    HTSensorModel_HumiditySensorId,
                    new PortDescription()
                    {
                        Description = "Humidity Sensor",
                        Id =  HTSensorModel_HumiditySensorId,
                        Label = "Humidity Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string TemperatureSensorModel_Id = "Temperature";
        public static ThingModelType TemperatureSensorModel = new ThingModelType
        {
            Name = "Temperature Sensor Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Temperature Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    TemperatureSensorModel_Id,
                    new PortDescription()
                    {
                        Description = "Temperature Sensor",
                        Id = TemperatureSensorModel_Id,
                        Label = "Temperature Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string UltrasonicSensorModel_Id = "UltrasonicSensor";
        public static ThingModelType UltrasonicSensorModel = new ThingModelType
        {
            Name = "Simple Ultrasonic Sensor",
            Id = ThingTypeLibrary.ProximitySensor_UltrasonicModelType,
            Description = "Simple Ultrasonic Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    UltrasonicSensorModel_Id,
                    new PortDescription()
                    {
                        Description = "Ultrasonic Sensor",
                        Id = UltrasonicSensorModel_Id,
                        Label = "Ultrasonic Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0,
                            Maximum = 255,  //TODO: Verify
                            Step = 1,
                            Type = ePortType.Integer,
                        }
                    }
                }
            }
        };

        public const string LcdModel_Id = "LCD";
        public static ThingModelType LcdModel = new ThingModelType
        {
            Name = "Simple Lcd",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Lcd",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    LcdModel_Id,
                    new PortDescription()
                    {
                        Description = "Lcd",
                        Id = LcdModel_Id,
                        Label = "Lcd",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string RelayModel_Id = "Relay";
        public static ThingModelType RelayModel = new ThingModelType
        {
            Name = "Simple Relay",
            Id = ThingTypeLibrary.SwitchActuator_RelayModelType,
            Description = "Simple Relay",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    RelayModel_Id,
                    new PortDescription()
                    {
                        Description = "Relay",
                        Id = RelayModel_Id ,
                        Label = "Relay",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string GestureSensorModel_TapId = "Tap";
        public const string GestureSensorModel_TouchId = "Touch";
        public const string GestureSensorModel_AirwheelId = "Airwheel";
        public const string GestureSensorModel_DoubleTapId = "DoubleTap";
        public const string GestureSensorModel_FlickId = "Flick";

        public static ThingModelType GestureSensorModel = new ThingModelType
        {
            Name = "Simple Gesture Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Gesture Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    GestureSensorModel_TapId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorModel_TapId,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorModel_TouchId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorModel_TouchId,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorModel_DoubleTapId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorModel_DoubleTapId,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorModel_FlickId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorModel_FlickId,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorModel_AirwheelId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorModel_AirwheelId,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
            }
        };

        public const string PositionSensorModel_XId = "X";
        public const string PositionSensorModel_YId = "Y";
        public const string PositionSensorModel_ZId = "Z";
        public static ThingModelType PositionSensorModel = new ThingModelType
        {
            Name = "Simple Position Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Position Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    PositionSensorModel_XId,
                    new PortDescription()
                    {
                        Description = "Position Sensor",
                        Id = PositionSensorModel_XId,
                        Label = "Position Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    PositionSensorModel_YId,
                    new PortDescription()
                    {
                        Description = "Position Sensor",
                        Id = PositionSensorModel_YId,
                        Label = "Position Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    PositionSensorModel_ZId,
                    new PortDescription()
                    {
                        Description = "Position Sensor",
                        Id = PositionSensorModel_ZId,
                        Label = "Position Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string BeaconDetectorModel_Id = "BeaconDetector";
        public static ThingModelType BeaconDetectorModel = new ThingModelType
        {
            Name = "Simple Beacon Detector",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Beacon Detector",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    BeaconDetectorModel_Id,
                    new PortDescription()
                    {
                        Description = "Beacon Detector",
                        Id = BeaconDetectorModel_Id,
                        Label = "Beacon Detector",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public const string MagnetometerSensorModel_XId = "X";
        public const string MagnetometerSensorModel_YId = "Y";
        public const string MagnetometerSensorModel_ZId = "Z";

        public static ThingModelType MagnetometerSensorModel = new ThingModelType
        {
            Name = "Simple Magnetometer Sensor",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Magnetometer Sensor",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    MagnetometerSensorModel_XId ,
                    new PortDescription()
                    {
                        Description = "Magnetometer Sensor",
                        Id = MagnetometerSensorModel_XId ,
                        Label = "Magnetometer Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    MagnetometerSensorModel_YId ,
                    new PortDescription()
                    {
                        Description = "Magnetometer Sensor",
                        Id = MagnetometerSensorModel_YId ,
                        Label = "Magnetometer Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    MagnetometerSensorModel_ZId ,
                    new PortDescription()
                    {
                        Description = "Magnetometer Sensor",
                        Id = MagnetometerSensorModel_ZId ,
                        Label = "Magnetometer Sensor",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Step = 0.01,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        public const string GpioModel_Id = "GPIO";
        public static ThingModelType GpioModel = new ThingModelType
        {
            Name = "Simple Gpio",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Gpio",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    GpioModel_Id,
                    new PortDescription()
                    {
                        Description = "Gpio",
                        Id = GpioModel_Id,
                        Label = "Gpio",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string TextIO_Id = "Text";
        public static ThingModelType TextIO = new ThingModelType
        {
            Name = "Text Input and Output",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Text IN OUT",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    TextIO_Id,
                    new PortDescription()
                    {
                        Description = "Text",
                        Id = TextIO_Id,
                        Label = "Text",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public const string AndroidIntentModel_Id = "AndroidIntent";
        public static ThingModelType AndroidIntentModel = new ThingModelType
        {
            Name = "Simple Android Intent",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Simple Android Intent",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    AndroidIntentModel_Id,
                    new PortDescription()
                    {
                        Description = "Android Intent",
                        Id = AndroidIntentModel_Id,
                        Label = "Android Intent",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };


        public const string FleetMgrModel_Id = "Coordinates";
        public static ThingModelType FleetMgrModel = new ThingModelType
        {
            Name = "Fleet Manager Info",
            Id = PlegmaAPI.ThingModelTypeDefault,
            Description = "Fleet Manager Info",
            Ports = new Dictionary<string, PortDescription>()
            {
                {
                    AndroidIntentModel_Id,
                    new PortDescription()
                    {
                        Description = "Fleet Manager Info",
                        Id = AndroidIntentModel_Id,
                        Label = "Fleet Manager Info",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        #endregion
    }


    public static class ThingTypesMigrator
    {
        public static Dictionary<string, string> OldTypeToNewType = new Dictionary<string, string>()
        {
            { "com.yodiwo.input.lights.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_DimmerModelType },
            { "com.yodiwo.input.console", ThingTypeLibrary.TextIn_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.text", ThingTypeLibrary.TextIn_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.seekbars", ThingTypeLibrary.Slider_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.camera", ThingTypeLibrary.Camera_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.inputoutput.text", ThingTypeLibrary.TextIO_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.lights.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_DimmerModelType },
            { "yodiwo.output.seekbars", ThingTypeLibrary.Slider_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.buttons", ThingTypeLibrary.Button_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.buttons", ThingTypeLibrary.Button_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.microphones", ThingTypeLibrary.Microphone_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.lcds", ThingTypeLibrary.Lcd_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.light", ThingTypeLibrary.LightSensor_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightSensor_NonNormalizedModelType },
            { "com.yodiwo.input.androidintent", ThingTypeLibrary.AndroidIntent_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.leds.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_DimmerModelType },
            { "com.yodiwo.output.sensors.rotation", ThingTypeLibrary.RotationEulerSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.beacon", ThingTypeLibrary.BeaconDetector + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.lcds", ThingTypeLibrary.Lcd_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.torches", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_BooleanModelType },
            { "com.yodiwo.output.nfc", ThingTypeLibrary.Nfc_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.proximity", ThingTypeLibrary.ProximitySensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.gyroscope", ThingTypeLibrary.GyroscopeSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.seekbars", ThingTypeLibrary.Progressbar_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.checkboxes", ThingTypeLibrary.SwitchActuator_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.checkboxes", ThingTypeLibrary.SwitchActuator_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.switches.onoff", ThingTypeLibrary.SwitchActuator_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.switches.onoff", ThingTypeLibrary.Switch_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.switches.onoff", ThingTypeLibrary.SwitchActuator_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.switches.onoff", ThingTypeLibrary.Switch_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.inputoutput.gpios", ThingTypeLibrary.Gpio_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.buzzers", ThingTypeLibrary.Buzzer_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.lights.onoff", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_BooleanModelType },
            { "com.yodiwo.output.sensors.sound", ThingTypeLibrary.SoundSensor_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_BooleanModelType },
            { "yodiwo.output.sensors.position", ThingTypeLibrary.PositionSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.gesture", ThingTypeLibrary.GestureSensor_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_BooleanModelType },
            { "com.yodiwo.output.shakedetectors", ThingTypeLibrary.ShakeDetector_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.text", ThingTypeLibrary.TextOut_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "bulb", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_DimmerModelType },
            { "slider", ThingTypeLibrary.Slider_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.console", ThingTypeLibrary.TextIn_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault  },
            { "yodiwo.input.text", ThingTypeLibrary.TextIn_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.webconsole", ThingTypeLibrary.TextIn_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.slider", ThingTypeLibrary.Slider_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.speechrecognition", ThingTypeLibrary.SpeechRecognition_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.text2speech", ThingTypeLibrary.Text2Speech_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.accelerometer", ThingTypeLibrary.AccelerometerSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.nfc", ThingTypeLibrary.Nfc_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.proximity", ThingTypeLibrary.ProximitySensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.leds.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_DimmerModelType },
            { "yodiwo.input.seekbars", ThingTypeLibrary.Progressbar_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.androidintent", ThingTypeLibrary.AndroidIntent_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.gyroscope", ThingTypeLibrary.GyroscopeSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.lcds", ThingTypeLibrary.Lcd_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.rotation", ThingTypeLibrary.RotationEulerSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.light", ThingTypeLibrary.LightSensor_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightSensor_NonNormalizedModelType },
            { "yodiwo.input.torches", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.Lights_BooleanModelType },
            { "yodiwo.output.text", ThingTypeLibrary.TextOut_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.enviromental", ThingTypeLibrary.HTSensor_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.ultrasonic", ThingTypeLibrary.ProximitySensor_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.ProximitySensor_UltrasonicModelType },
            { "yahooweather:weather", "org.openhab.yahooweather.weather" },
            { "hue:bridge", "org.openhab.hue.bridge" },
            { "hue:LWB004", "org.openhab.hue.LWB004" },
            { "iOSSensor", "" },            //start: remove ios generic types from db; different migration then needs to happen for ios
            { "iOSVirtual", "" },
            { "iOSLocation", "" },
            { "iOSActuator", "" },
            { "iOSWiFiStatus", "" },
            { "iOSActivityTracker", "" },
            { "iOSBlouetoothStatus", "" },  //end
            { "yodiwo.input.fleet", "com.yodiwo.in.fleetmgr.default" },         //TODO: either make proper types/models for the web apps, or delete them entirely
            { "yodiwo.input.displayer", "com.yodiwo.in.displayer.default" },
            { "yodiwo.input.gallery", "com.yodiwo.in.gallery.default" },
            { "yodiwo.input.flickr", "com.yodiwo.flickr.default" },
            { "yodiwo.input.regionviewer", "com.yodiwo.in.regionviewer.default" },
            { "yodiwo.output.labels", "com.yodiwo.out.label.default" },         //TODOEND
        };
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
