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

        // ---------------------------------------------------------------------------------------------

        #region Thing Types

        #region GPIO
        public const string Gpio_Type = "com.yodiwo.gpios";
        public const string GpioDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public const string GpioInput_ModelType = "input";
        public const string GpioOutput_ModelType = "output";
        public static ThingType Gpio = new ThingType
        {
            Type = Gpio_Type,
            Description = "GPIOs",
            Models = new Dictionary<string, ThingModelType>()
            {
                { GpioDefault_ModelType, ModelTypeLibrary.GpioModel },
                { GpioInput_ModelType, ModelTypeLibrary.GpioInputModel },
                { GpioOutput_ModelType, ModelTypeLibrary.GpioOutputModel },
            },
        };

        #endregion

        #region Text
        public const string Text_Type = "com.yodiwo.text";
        public const string TextIO_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public const string TextTypewriter_ModelType = "typewriter";
        public const string TextConsole_ModelType = "console";
        public static ThingType Text = new ThingType
        {
            Type = Text_Type,
            Description = "TypeWriter",
            Models = new Dictionary<string, ThingModelType>()
            {
                { TextIO_ModelType, ModelTypeLibrary.TextIOModel },
                { TextTypewriter_ModelType, ModelTypeLibrary.TypeWriterModel },
                { TextConsole_ModelType, ModelTypeLibrary.ConsoleModel }
            },
        };
        #endregion

        #region Switches
        public const string Switch_Type = "com.yodiwo.switches";
        public const string SwitchActuator_ModelType = "actuator.onoff";
        public const string SwitchActuatorCheckBox_ModelType = "actuator.checkbox";
        public const string SwitchActuatorRelay_ModelType = "actuator.relay";
        public const string SwitchActuatorDimmer_ModelType = "actuator.dimmer";
        public const string SwitchActuatorAndroid_ModelType = "actuator.android";
        public const string SwitchSink_ModelType = "sink.onoff";
        public const string SwitchSinkCheckBox_ModelType = "sink.checkbox";
        public const string SwitchSinkAndroid_ModelType = "sink.android";
        public static ThingType Switches = new ThingType
        {
            Type = Switch_Type,
            Description = "Switches",
            Models = new Dictionary<string, ThingModelType>()
            {
                 { SwitchActuator_ModelType, ModelTypeLibrary.SwitchOnOffActuatorModel },
                 { SwitchActuatorCheckBox_ModelType, ModelTypeLibrary.CheckboxActuatorModel },
                 { SwitchActuatorRelay_ModelType, ModelTypeLibrary.RelayActuatorModel },
                 { SwitchActuatorDimmer_ModelType, ModelTypeLibrary.SwitchDimmerActuatorModel },
                 { SwitchActuatorAndroid_ModelType, ModelTypeLibrary.SwitchAndroidActuatorModel },
                 { SwitchSink_ModelType, ModelTypeLibrary.SwitchOnOffSinkModel },
                 { SwitchSinkCheckBox_ModelType, ModelTypeLibrary.CheckboxSinkModel },
                 { SwitchSinkAndroid_ModelType, ModelTypeLibrary.SwitchAndroidSinkModel },
            },
        };
        #endregion

        #region Lights
        public const string Lights_Type = "com.yodiwo.lights";
        public const string LightsDimmer_ModelType = "dimmer";
        public const string LightsDimmerNormalized_ModelType = "dimmer.normalized";
        public const string LightsAndroid_ModelType = "android";
        public const string LightsBoolean_ModelType = "onoff";
        public static ThingType Lights = new ThingType
        {
            Type = Lights_Type,
            Description = "Lights",
            Models = new Dictionary<string, ThingModelType>()
            {
                { LightsDimmer_ModelType,  ModelTypeLibrary.LightsDimmerModel },
                { LightsDimmerNormalized_ModelType,  ModelTypeLibrary.LightsDimmerNormalizedModel },
                { LightsAndroid_ModelType,  ModelTypeLibrary.LightsAndroidModel },
                { LightsBoolean_ModelType, ModelTypeLibrary.LightOnOffModel },
            },
        };

        #endregion

        #region Seekbars

        public const string Seekbar_Type = "com.yodiwo.seekbars";
        public const string SeekbarSlider_ModelType = "slider";
        public const string SeekbarProgressbar_ModelType = "progressbar";
        public static ThingType Seekbar = new ThingType
        {
            Type = Seekbar_Type,
            Description = "Seekbars",
            Models = new Dictionary<string, ThingModelType>()
            {
                { SeekbarSlider_ModelType, ModelTypeLibrary.SliderModel },
                { SeekbarProgressbar_ModelType, ModelTypeLibrary.ProgressbarModel },
            },
        };

        #endregion        

        #region Buttons
        public const string Button_Type = "com.yodiwo.buttons";
        public const string ButtonFlic_ModelType = "flic";
        public const string ButtonAndroid_ModelType = "android";
        public const string ButtonDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Button = new ThingType
        {
            Type = Button_Type,
            Description = "Button Actuators",
            Models = new Dictionary<string, ThingModelType>()
            {
                { ButtonDefault_ModelType, ModelTypeLibrary.ButtonModel },
                { ButtonFlic_ModelType, ModelTypeLibrary.FlicButtonModel },
                { ButtonAndroid_ModelType, ModelTypeLibrary.ButtonAndroidModel },
            },
        };

        #endregion

        #region Location
        public const string Location_Type = "com.yodiwo.location";
        public const string LocationCoordinates_ModelType = "coordinates";
        public const string LocationInfo_ModelType = "info";
        public const string LocationCoordinatesTriggered_ModelType = "coordinates.triggered";
        public const string LocationInfoTriggered_ModelType = "info.triggered";
        public static ThingType Location = new ThingType
        {
            Type = Location_Type,
            Description = "Location",
            Models = new Dictionary<string, ThingModelType>()
            {
                { LocationCoordinates_ModelType, ModelTypeLibrary.LocationCoordinatesModel },
                { LocationInfo_ModelType, ModelTypeLibrary.LocationInfoModel },
                { LocationCoordinatesTriggered_ModelType, ModelTypeLibrary.TriggeredLocationCoordinatesModel },
                { LocationInfoTriggered_ModelType, ModelTypeLibrary.TriggeredLocationInfoModel },
            },
        };
        #endregion

        #region Wifi
        public const string Wifi_Type = "com.yodiwo.wifi";
        public const string WifiStatus_ModelType = "status";
        public const string WifiRssi_ModelType = "rssi";
        public const string WifiSsid_ModelType = "ssid";
        public const string WifiBssid_ModelType = "bssid";
        public const string WifiInfo_ModelType = "info";
        public static ThingType WifiInfo = new ThingType
        {
            Type = WifiInfo_ModelType,
            Description = "Wifi Info",
            Models = new Dictionary<string, ThingModelType>()
            {
                { WifiStatus_ModelType, ModelTypeLibrary.WifiStatusModel },
                { WifiRssi_ModelType, ModelTypeLibrary.WifiRssiModel },
                { WifiSsid_ModelType, ModelTypeLibrary.WifiSsidModel },
                { WifiBssid_ModelType, ModelTypeLibrary.WifiBssidModel },
                { WifiInfo_ModelType, ModelTypeLibrary.WifiInfoModel },
            },
        };
        #endregion

        #region Bluetooth
        public const string Bluetooth_Type = "com.yodiwo.bluetooth";
        public const string BluetoothDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public const string BluetoothTriggered_ModelType = "triggered";
        public static ThingType Bluetooth = new ThingType
        {
            Type = Bluetooth_Type,
            Description = "Bluetooth",
            Models = new Dictionary<string, ThingModelType>()
            {
                { BluetoothDefault_ModelType, ModelTypeLibrary.BluetoothModel },
                { BluetoothTriggered_ModelType, ModelTypeLibrary.TriggeredBluetoothModel },
            },
        };
        #endregion

        #region Camera
        public const string Camera_Type = "com.yodiwo.cameras";
        public const string CameraDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public const string CameraTriggered_ModelType = "trigger";
        public const string CameraOnVif_ModelType = "onvif";
        public static ThingType Camera = new ThingType
        {
            Type = Camera_Type,
            Description = "Cameras",
            Models = new Dictionary<string, ThingModelType>()
            {
                { CameraDefault_ModelType, ModelTypeLibrary.CameraModel },
                { CameraOnVif_ModelType, ModelTypeLibrary.OnVifCameraModel },
                { CameraTriggered_ModelType, ModelTypeLibrary.TriggeredCameraModel },
            },
        };

        #endregion

        #region Beacon reader
        public const string BeaconReader_Type = "com.yodiwo.beacon";
        public const string BeaconReaderDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public const string BeaconReaderSensors_ModelType = "withsensors";
        public static ThingType BeaconDetector = new ThingType
        {
            Type = BeaconReader_Type,
            Description = "Beacon Reader",
            Models = new Dictionary<string, ThingModelType>()
            {
                { BeaconReaderDefault_ModelType, ModelTypeLibrary.BeaconReaderModel },
                { BeaconReaderSensors_ModelType, ModelTypeLibrary.BeaconReaderSensorsModel },
            },
        };
        #endregion

        #region Nfc
        public const string Nfc_Type = "com.yodiwo.nfc";
        public const string NfcDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Nfc = new ThingType
        {
            Type = Nfc_Type,
            Description = "Nfc",
            Models = new Dictionary<string, ThingModelType>()
            {
                { NfcDefault_ModelType, ModelTypeLibrary.NfcModel },
            },
        };

        #endregion        

        #region Android Intent
        public const string AndroidIntent_Type = "com.yodiwo.androidintent";
        public const string AndroidIntentDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType AndroidIntent = new ThingType
        {
            Type = AndroidIntent_Type,
            Description = "Android Intents",
            Models = new Dictionary<string, ThingModelType>()
            {
                { AndroidIntentDefault_ModelType, ModelTypeLibrary.AndroidIntentModel },
            },
        };

        #endregion

        #region Microphone
        public const string Microphone_Type = "com.yodiwo.microphones";
        public const string MicrophoneDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Microphone = new ThingType
        {
            Type = Microphone_Type,
            Description = "Microphones",
            Models = new Dictionary<string, ThingModelType>()
            {
                { MicrophoneDefault_ModelType, ModelTypeLibrary.MicrophoneModel },
            },
        };

        #endregion

        #region Buzzer
        public const string Buzzer_Type = "com.yodiwo.buzzers";
        public const string BuzzerDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Buzzer = new ThingType
        {
            Type = Buzzer_Type,
            Description = "Buzzers",
            Models = new Dictionary<string, ThingModelType>()
            {
                { BuzzerDefault_ModelType, ModelTypeLibrary.BuzzerModel },
            },
        };

        #endregion

        #region SipPhone
        public const string SipPhone_Type = "com.yodiwo.sipphones";
        public const string SipPhoneDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType SipPhone = new ThingType
        {
            Type = SipPhone_Type,
            Description = "SipPhone",
            Models = new Dictionary<string, ThingModelType>()
            {
                { SipPhoneDefault_ModelType, ModelTypeLibrary.SipPhoneModel },
            },
        };
        #endregion

        #region Speech recognition
        public const string SpeechRecognition_Type = "com.yodiwo.speechrecognition";
        public const string SpeechRecognitionDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType SpeechRecognition = new ThingType
        {
            Type = SpeechRecognition_Type,
            Description = "SpeechRecognition",
            Models = new Dictionary<string, ThingModelType>()
            {
                { SpeechRecognitionDefault_ModelType, ModelTypeLibrary.SpeechRecognitionModel },
            },
        };
        #endregion

        #region Text2Speech
        public const string Text2Speech_Type = "com.yodiwo.text2speech";
        public const string Text2SpeechDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Text2Speech = new ThingType
        {
            Type = Text2Speech_Type,
            Description = "Text To Speech",
            Models = new Dictionary<string, ThingModelType>()
            {
                { Text2SpeechDefault_ModelType, ModelTypeLibrary.Text2SpeechModel },
            },
        };

        #endregion

        #region Sensors 
        public const string Sensor_Type = "com.yodiwo.sensors";
        public const string GestureSensor_ModelType = "gesture";
        public const string PositionSensor_ModelType = "position";
        public const string AccelerometerSensor_ModelType = "accelerometer";
        public const string RotationSensor_ModelType = "rotation";
        public const string RotationEulerSensor_ModelType = "rotation.euler";
        public const string GyroscopeSensor_ModelType = "gyroscope";
        public const string MagnetometerSensor_ModelType = "magnetometer";
        public const string SoundSensor_ModelType = "sound";
        public const string HumiditySensor_ModelType = "humidity";
        public const string TemperatureSensor_ModelType = "temperature";
        public const string HTSensor_ModelType = "humiditytemperature";
        public const string LightSensor_ModelType = "brightness";
        public const string LightSensorNonNormalized_ModelType = "brightness.nonNormalized";
        public const string ProximitySensor_ModelType = "proximity";
        public const string ProximityUltrasonicSensor_ModelType = "proximity.ultrasonic";
        public const string DoorSensor_ModelType = "door";
        public const string SmartPlugSensor_ModelType = "smart.plug";
        public const string DoorlockSensor_ModelType = "doorlock";
        public const string AirConditionSensor_ModelType = "air.condition";
        public const string ZWaveSensor_ModelType = "z-wave";
        public const string ShakeDetectorSensor_ModelType = "shakedetector";
        public static ThingType Sensors = new ThingType
        {
            Type = Sensor_Type,
            Description = "Sensors",
            Models = new Dictionary<string, ThingModelType>()
            {
                { GestureSensor_ModelType, ModelTypeLibrary.GestureSensorModel },
                { PositionSensor_ModelType, ModelTypeLibrary.PositionSensorModel },
                { AccelerometerSensor_ModelType, ModelTypeLibrary.AccelerometerSensorModel },
                { RotationSensor_ModelType, ModelTypeLibrary.RotationSensorModel },
                { RotationEulerSensor_ModelType, ModelTypeLibrary.RotationEulerSensorModel },
                { GyroscopeSensor_ModelType, ModelTypeLibrary.GyroscopeSensorModel },
                { MagnetometerSensor_ModelType, ModelTypeLibrary.MagnetometerSensorModel },
                { SoundSensor_ModelType, ModelTypeLibrary.SoundSensorModel },
                { HumiditySensor_ModelType, ModelTypeLibrary.HumiditySensorModel },
                { TemperatureSensor_ModelType, ModelTypeLibrary.TemperatureSensorModel },
                { HTSensor_ModelType, ModelTypeLibrary.HTSensorModel },
                { LightSensor_ModelType, ModelTypeLibrary.BrightnessNormalizedModel },
                { LightSensorNonNormalized_ModelType, ModelTypeLibrary.BrightnessModel },
                { ProximitySensor_ModelType, ModelTypeLibrary.ProximitySensorModel },
                { ProximityUltrasonicSensor_ModelType, ModelTypeLibrary.UltrasonicSensorModel },
                { DoorlockSensor_ModelType, ModelTypeLibrary.DoorlockSensorModel },
                { AirConditionSensor_ModelType, ModelTypeLibrary.AirConditionSensorModel },
                { DoorSensor_ModelType, ModelTypeLibrary.DoorSensorModel },
                { SmartPlugSensor_ModelType, ModelTypeLibrary.SmartPlugSensorModel },
                { ZWaveSensor_ModelType, ModelTypeLibrary.ZWaveSensorModel },
                { ShakeDetectorSensor_ModelType, ModelTypeLibrary.ShakeDetectorModel },
            },
        };
        #endregion

        // TODO 666: merge the below ThingTypes

        #region LCD
        public const string Lcd_Type = "com.yodiwo.lcds";
        public const string LcdDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Lcd = new ThingType
        {
            Type = Lcd_Type,
            Description = "LCDs",
            Models = new Dictionary<string, ThingModelType>()
            {
                { LcdDefault_ModelType, ModelTypeLibrary.LcdModel },
            },
        };

        #endregion

        #region FleetMgr
        public const string FleetMgr_Type = "com.yodiwo.fleetmgr";
        public const string FleetMgrDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType FleetMgr = new ThingType
        {
            Type = FleetMgr_Type,
            Description = "Fleet Manager",
            Models = new Dictionary<string, ThingModelType>()
            {
                {FleetMgrDefault_ModelType, ModelTypeLibrary.FleetMgrModel },

            },
        };
        #endregion

        #region Displayer
        public const string Displayer_Type = "com.yodiwo.displayer";
        public const string DisplayerDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Displayer = new ThingType
        {
            Type = Displayer_Type,
            Description = "Displayer",
            Models = new Dictionary<string, ThingModelType>()
            {
                {DisplayerDefault_ModelType, ModelTypeLibrary.DisplayerModel },

            },
        };
        #endregion

        #region Gallery
        public const string Gallery_Type = "com.yodiwo.gallery";
        public const string GalleryDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Gallery = new ThingType
        {
            Type = Gallery_Type,
            Description = "Gallery",
            Models = new Dictionary<string, ThingModelType>()
            {
                {GalleryDefault_ModelType, ModelTypeLibrary.GalleryModel },

            },
        };
        #endregion

        #region Flickr
        public const string Flickr_Type = "com.yodiwo.flickr";
        public const string FlickrDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType Flickr = new ThingType
        {
            Type = Flickr_Type,
            Description = "Flickr",
            Models = new Dictionary<string, ThingModelType>()
            {
                {FlickrDefault_ModelType, ModelTypeLibrary.FlickrModel },

            },
        };
        #endregion

        #region RegionViewer
        public const string RegionViewer_Type = "com.yodiwo.regionviewer";
        public const string RegionViewerDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType RegionViewer = new ThingType
        {
            Type = RegionViewer_Type,
            Description = "Region Viewer",
            Models = new Dictionary<string, ThingModelType>()
            {
                {RegionViewerDefault_ModelType, ModelTypeLibrary.RegionViewerModel },

            },
        };
        #endregion

        #region Label
        public const string Label_Type = "com.yodiwo.label.default";
        public static ThingType Label = new ThingType
        {
            Type = Label_Type,
            Description = "Label",
        };
        #endregion       

        #region Data plotter
        public const string DataPlotter_Type = "com.yodiwo.dataplotter";
        public const string DataPlotterDefault_ModelType = PlegmaAPI.ThingModelTypeDefault;
        public static ThingType DataPlotter = new ThingType
        {
            Type = DataPlotter_Type,
            Description = "Data plotter",
            Models = new Dictionary<string, ThingModelType>()
            {
                {DataPlotterDefault_ModelType, ModelTypeLibrary.DataPlotterModel },

            },
        };
        #endregion
        // end of TODO 666
        #endregion //of Thing Types
    }

    // ---------------------------------------------------------------------------------------------

    public class ModelTypeLibrary
    {
        #region Model Types

        // -----------------------
        // switch actuator
        public const string SwitchOnOffActuator_PortModelId = "OnOffSwitchActuator";
        public static ThingModelType SwitchOnOffActuatorModel = new ThingModelType
        {
            Name = "Simple On/Off Switch Actuator",
            Id = ThingTypeLibrary.SwitchActuator_ModelType,
            Description = "Simple On/Off Switch Actuator",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SwitchOnOffActuator_PortModelId,
                    new PortDescription()
                    {
                        Description = "On/Off",
                        Id = SwitchOnOffActuator_PortModelId,
                        Label = "On/Off",
                        ioDirection = ioPortDirection.Output,
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        },
                    }
                }
            }
        };

        // -----------------------
        // android switch actuator
        public static ThingModelType SwitchAndroidSinkModel = new ThingModelType
        {
            Name = "Simple On/Off Switch Sink",
            Id = ThingTypeLibrary.SwitchSinkAndroid_ModelType,
            Description = "Simple On/Off Switch Sink",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SwitchOnOffSink_PortModelId,
                    new PortDescription()
                    {
                        Description = "On/Off",
                        Id = SwitchOnOffSink_PortModelId,
                        Label = "On/Off",
                        ioDirection = ioPortDirection.Input,
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        },
                    }
                }
            },
            NumPorts = new Dictionary<string, int>()
            {
                { SwitchOnOffSink_PortModelId, 3}, // 3 ports with the same PortModel
            }
        };

        // -----------------------
        // android switch sink
        public static ThingModelType SwitchAndroidActuatorModel = new ThingModelType
        {
            Name = "Simple On/Off Switch Actuator",
            Id = ThingTypeLibrary.SwitchActuator_ModelType,
            Description = "Simple On/Off Switch Actuator",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SwitchOnOffActuator_PortModelId,
                    new PortDescription()
                    {
                        Description = "On/Off",
                        Id = SwitchOnOffActuator_PortModelId,
                        Label = "On/Off",
                        ioDirection = ioPortDirection.Output,
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        },
                    }
                }
            },
            NumPorts = new Dictionary<string, int>()
            {
                { SwitchOnOffActuator_PortModelId, 3} // 3 ports with the same PortModel
            }
        };

        // -----------------------
        // switch sink
        public const string SwitchOnOffSink_PortModelId = "OnOffSwitchSink";
        public static ThingModelType SwitchOnOffSinkModel = new ThingModelType
        {
            Name = "Simple On/Off Switch Sink",
            Id = ThingTypeLibrary.SwitchSink_ModelType,
            Description = "On/Off Switch Sink",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SwitchOnOffSink_PortModelId,
                    new PortDescription()
                    {
                        Description = "On/Off",
                        Id = SwitchOnOffSink_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "On/Off",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        },
                    }
                }
            }
        };

        // -----------------------
        // dimmer actuator
        public const string SwitchDimmerActuator_PortModelId = "DimmerActuator";
        public static ThingModelType SwitchDimmerActuatorModel = new ThingModelType
        {
            Name = "Simple Dimmer Actuator",
            Id = ThingTypeLibrary.SwitchActuatorDimmer_ModelType,
            Description = "Simple Dimmer Actuator",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SwitchDimmerActuator_PortModelId,
                    new PortDescription()
                    {
                        Description = "Dimmer Actuator",
                        Id = SwitchDimmerActuator_PortModelId,
                        ioDirection = ioPortDirection.Output,
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

        // -----------------------
        // dimmer sink
        public const string LightsDimmer_PortModelId = "DimmableLight";
        public static ThingModelType LightsDimmerModel = new ThingModelType
        {
            Name = "Simple Dimmer Sink",
            Id = ThingTypeLibrary.LightsDimmer_ModelType,
            Description = "Simple Dimmer Sink",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    LightsDimmer_PortModelId,
                    new PortDescription()
                    {
                        Description = "Dimmer Sink",
                        Id = LightsDimmer_PortModelId,
                        ioDirection = ioPortDirection.Input,
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
            }
        };

        public const string LightsDimmerNormalized_PortModelId = "NormalizedDimmableLight";
        public static ThingModelType LightsDimmerNormalizedModel = new ThingModelType
        {
            Name = "Simple normalized Dimmable Lights",
            Id = ThingTypeLibrary.LightsDimmerNormalized_ModelType,
            Description = "Simple normalized Dimmable Lights",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    LightsDimmerNormalized_PortModelId,
                    new PortDescription()
                    {
                        Description = "Dimmable Light",
                        Id = LightsDimmerNormalized_PortModelId,
                        ioDirection = ioPortDirection.Input,
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

        public static ThingModelType LightsAndroidModel = new ThingModelType
        {
            Name = "Simple normalized Dimmable Lights",
            Id = ThingTypeLibrary.LightsAndroid_ModelType,
            Description = "Simple normalized Dimmable Lights",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    LightsDimmerNormalized_PortModelId,
                    new PortDescription()
                    {
                        Description = "Dimmable Light",
                        Id = LightsDimmerNormalized_PortModelId,
                        ioDirection = ioPortDirection.Input,
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
            },
            NumPorts = new Dictionary<string, int>()
            {
                { LightsDimmerNormalized_PortModelId, 3}
            }
        };

        // -----------------------
        // button
        public const string Button_PortModelId = "Button";
        public static ThingModelType ButtonModel = new ThingModelType
        {
            Name = "Button",
            Id = ThingTypeLibrary.ButtonDefault_ModelType,
            Description = "Simple Button",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Button_PortModelId,
                    new PortDescription
                    {
                        Description = "Button",
                        Id = Button_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "Button",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // flic button model type
        public const string ButtonFlicSingleClick_PortModelId = "FlicButtonSingleClick";
        public const string ButtonFlicDoubleClick_PortModelId = "FlicButtonDoubleClick";
        public const string ButtonFlicLongClick_PortModelId = "FlicButtonLongClick";
        public static ThingModelType FlicButtonModel = new ThingModelType
        {
            Name = "Flic Actuator",
            Id = ThingTypeLibrary.ButtonFlic_ModelType,
            Description = "Flick Actuator",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    ButtonFlicSingleClick_PortModelId,
                    new PortDescription
                    {
                        Description = "Single-click On/Off Actuator",
                        Id = ButtonFlicSingleClick_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "Single-click Actuator On/Off Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                },
                {
                    ButtonFlicDoubleClick_PortModelId,
                    new PortDescription
                    {
                        Description = "Double-click On/Off Actuator",
                        Id = ButtonFlicDoubleClick_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "Double-click Actuator On/Off Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                },
                {
                    ButtonFlicLongClick_PortModelId,
                    new PortDescription
                    {
                        Description = "Long-click On/Off Actuator",
                        Id = ButtonFlicLongClick_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "Long-click Actuator On/Off Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // android button
        public const string ButtonAndroid_PortModelId = "AndroidButton";
        public static ThingModelType ButtonAndroidModel = new ThingModelType
        {
            Name = "Simple Button",
            Id = ThingTypeLibrary.ButtonAndroid_ModelType,
            Description = "Simple Button",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    ButtonAndroid_PortModelId,
                    new PortDescription
                    {
                        Description = "On/Off Actuator",
                        Id = ButtonAndroid_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "On/Off Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            },
            NumPorts = new Dictionary<string, int>()
            {
                { ButtonAndroid_PortModelId, 3}
            }
        };

        // -----------------------
        // textIO
        public const string TextIO_PortModelId = "TextIO";
        public static ThingModelType TextIOModel = new ThingModelType
        {
            Name = "Text I/O",
            Id = ThingTypeLibrary.TextIO_ModelType,
            Description = "Text I/O",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    TextIO_PortModelId,
                    new PortDescription()
                    {
                        Description = "TextIO",
                        Id = TextIO_PortModelId,
                        ioDirection = ioPortDirection.InputOutput,
                        Label = "TextIO",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        //typewriter
        public const string TypeWriter_PortModelId = "TypeWriter";
        public static ThingModelType TypeWriterModel = new ThingModelType
        {
            Name = "Simple Typewriter",
            Id = ThingTypeLibrary.TextTypewriter_ModelType,
            Description = "Simple Typewriter",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    TypeWriter_PortModelId,
                    new PortDescription()
                    {
                        Description = "Typewriter",
                        Id = TypeWriter_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Typewriter",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // console
        public const string Console_PortModelId = "Console";
        public static ThingModelType ConsoleModel = new ThingModelType
        {
            Name = "Simple Console",
            Id = ThingTypeLibrary.TextConsole_ModelType,
            Description = "Simple Console",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Console_PortModelId,
                    new PortDescription()
                    {
                        Description = "Console",
                        Id = Console_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Console",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // checkbox actuator
        public const string CheckboxActuator_PortModelId = "CheckBoxActuator";
        public static ThingModelType CheckboxActuatorModel = new ThingModelType
        {
            Name = "Simple Checkbox Actuator",
            Id = ThingTypeLibrary.SwitchActuatorCheckBox_ModelType,
            Description = "Simple Checkbox Actuator",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    CheckboxActuator_PortModelId,
                    new PortDescription()
                    {
                        Description = "Checkbox Actuator",
                        Id = CheckboxActuator_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Checkbox Actuator",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // checkbox sink
        public const string CheckboxSink_PortModelId = "CheckBoxSink";
        public static ThingModelType CheckboxSinkModel = new ThingModelType
        {
            Name = "Simple Checkbox Sink",
            Id = ThingTypeLibrary.SwitchSinkCheckBox_ModelType,
            Description = "Simple Checkbox Sink",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    CheckboxSink_PortModelId,
                    new PortDescription()
                    {
                        Description = "Checkbox Sink",
                        Id = CheckboxSink_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Checkbox Sink",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // slider
        public const string Slider_PortModelId = "Slider";
        public static ThingModelType SliderModel = new ThingModelType
        {
            Name = "Simple Slider",
            Id = ThingTypeLibrary.SeekbarSlider_ModelType,
            Description = "Simple Slider",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    Slider_PortModelId,
                    new PortDescription()
                    {
                        Description = "Slider",
                        Id = Slider_PortModelId,
                        ioDirection = ioPortDirection.Output,
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

        // -----------------------
        // progressbar
        public const string Progressbar_PortModelId = "ProgressBar";
        public static ThingModelType ProgressbarModel = new ThingModelType
        {
            Name = "Simple Progress Bar",
            Id = ThingTypeLibrary.SeekbarProgressbar_ModelType,
            Description = "Simple Progress Bar",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    Progressbar_PortModelId,
                    new PortDescription()
                    {
                        Description = "Progress bar",
                        Id = Progressbar_PortModelId,
                        Label = "Progress bar",
                        ioDirection = ioPortDirection.Input,
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

        // -----------------------
        // on-off light
        public const string LightOnOff_PortModelId = "OnOffLight";
        public static ThingModelType LightOnOffModel = new ThingModelType
        {
            Name = "Simple On/Off Light",
            Id = ThingTypeLibrary.LightsBoolean_ModelType,
            Description = "Simple On/Off Light",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    LightOnOff_PortModelId,
                    new PortDescription()
                    {
                        Description = "On/Off Light",
                        Id = LightOnOff_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "On/Off Light",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // location coordinates
        public const string LocationLatitude_PortModelId = "LocationLatitude";
        public const string LocationLongitude_PortModelId = "LocationLongitude";
        public static ThingModelType LocationCoordinatesModel = new ThingModelType
        {
            Name = "Location Coordinates",
            Id = ThingTypeLibrary.LocationCoordinates_ModelType,
            Description = "Location Coordinates",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    LocationLatitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Latitude",
                        Id = LocationLatitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Latitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    LocationLongitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Longitude",
                        Id = LocationLongitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Longitude",
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

        // -----------------------
        // location info
        public const string LocationInfo_PortModelId = "LocationInfo";
        public const string LocationAddress_PortModelId = "LocationAddress";
        public const string LocationCountry_PortModelId = "LocationCountry";
        public const string LocationPostalCode_PortModelId = "LocationPostalCode";
        public static ThingModelType LocationInfoModel = new ThingModelType
        {
            Name = "Location Info",
            Id = ThingTypeLibrary.LocationInfo_ModelType,
            Description = "Location Info",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    LocationLatitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Latitude",
                        Id = LocationLatitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Latitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    LocationLongitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Longitude",
                        Id = LocationLongitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Longitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    LocationAddress_PortModelId,
                    new PortDescription()
                    {
                        Description = "Address",
                        Id = LocationAddress_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Address",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    LocationCountry_PortModelId,
                    new PortDescription()
                    {
                        Description = "Country",
                        Id = LocationCountry_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Country",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    LocationPostalCode_PortModelId,
                    new PortDescription()
                    {
                        Description = "PostalCode",
                        Id = LocationPostalCode_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "PostalCode",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // triggered location coordinates
        public const string TriggeredLocation_PortModelId = "Location trigger";
        public static ThingModelType TriggeredLocationCoordinatesModel = new ThingModelType
        {
            Name = "Location Coordinates",
            Id = ThingTypeLibrary.LocationCoordinatesTriggered_ModelType,
            Description = "Location Coordinates",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    LocationLatitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Latitude",
                        Id = LocationLatitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Latitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    LocationLongitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Longitude",
                        Id = LocationLongitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Longitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    TriggeredLocation_PortModelId,
                    new PortDescription()
                    {
                        Description = "Location Trigger",
                        Id = TriggeredLocation_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Location Trigger",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // triggered location info
        public static ThingModelType TriggeredLocationInfoModel = new ThingModelType
        {
            Name = "Location Info with trigger",
            Id = ThingTypeLibrary.LocationInfoTriggered_ModelType,
            Description = "Location Info with trigger",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    LocationLatitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Latitude",
                        Id = LocationLatitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Latitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    LocationLongitude_PortModelId,
                    new PortDescription()
                    {
                        Description = "Longitude",
                        Id = LocationLongitude_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Longitude",
                        State = new StateDescription()
                        {
                            Minimum = 0.0,
                            Maximum = 1.0,
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    LocationAddress_PortModelId,
                    new PortDescription()
                    {
                        Description = "Address",
                        Id = LocationAddress_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Address",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    LocationCountry_PortModelId,
                    new PortDescription()
                    {
                        Description = "Country",
                        Id = LocationCountry_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Country",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    LocationPostalCode_PortModelId,
                    new PortDescription()
                    {
                        Description = "PostalCode",
                        Id = LocationPostalCode_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "PostalCode",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    TriggeredLocation_PortModelId,
                    new PortDescription()
                    {
                        Description = "Location Trigger",
                        Id = TriggeredLocation_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Location Trigger",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // bluetooth
        public const string BluetoothPowerStatus_PortModelId = "BluetoothPowerStatus";
        public const string BluetoothConnectionStatus_PortModelId = "BluetoothConnectionStatus";
        public const string BluetoothPairedDevices_PortModelId = "BluetoothPairedDevices";
        public const string BluetoothDiscoveredDevices_PortModelId = "BluetoothDiscoveredDevices";
        public static ThingModelType BluetoothModel = new ThingModelType
        {
            Name = "Bluetooth info",
            Id = ThingTypeLibrary.BluetoothDefault_ModelType,
            Description = "Bluetooth info",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    BluetoothPowerStatus_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Power Status",
                        Id = BluetoothPowerStatus_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Power Status",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BluetoothConnectionStatus_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Connection Status",
                        Id = BluetoothConnectionStatus_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Connection Status",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BluetoothPairedDevices_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Paired Devices",
                        Id = BluetoothPairedDevices_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Paired Devices",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BluetoothDiscoveredDevices_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Discovered Devices",
                        Id = BluetoothDiscoveredDevices_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Discovered Devices",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // bluetooth with trigger
        public const string TriggeredBluetoothDiscovery_PortModelId = "TriggeredBluetoothDiscovery";
        public const string TriggeredBluetoothPairedDevices_PortModelId = "TriggeredBluetoothPairedDevices";
        public static ThingModelType TriggeredBluetoothModel = new ThingModelType
        {
            Name = "Bluetooth info with trigger",
            Id = ThingTypeLibrary.BluetoothTriggered_ModelType,
            Description = "Bluetooth info with trigger",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    BluetoothPowerStatus_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Power Status",
                        Id = BluetoothPowerStatus_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Power Status",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BluetoothConnectionStatus_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Connection Status",
                        Id = BluetoothConnectionStatus_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Connection Status",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BluetoothPairedDevices_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Paired Devices",
                        Id = BluetoothPairedDevices_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Paired Devices",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BluetoothDiscoveredDevices_PortModelId,
                    new PortDescription()
                    {
                        Description = "Bluetooth Discovered Devices",
                        Id = BluetoothDiscoveredDevices_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Bluetooth Discovered Devices",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    TriggeredBluetoothDiscovery_PortModelId,
                    new PortDescription()
                    {
                        Description = "Trigger device discovery",
                        Id = TriggeredBluetoothDiscovery_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Trigger device discovery",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                },
                {
                    TriggeredBluetoothPairedDevices_PortModelId,
                    new PortDescription()
                    {
                        Description = "Request paired devices",
                        Id = TriggeredBluetoothPairedDevices_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Request paired devices",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // accelerometer
        public const string AccelerometerSensorX_PortModelId = "X";
        public const string AccelerometerSensorY_PortModelId = "Y";
        public const string AccelerometerSensorZ_PortModelId = "Z";
        public static ThingModelType AccelerometerSensorModel = new ThingModelType
        {
            Name = "Simple Accelerometer",
            Id = ThingTypeLibrary.AccelerometerSensor_ModelType,
            Description = "Simple Accelerometer",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    AccelerometerSensorX_PortModelId,
                    new PortDescription()
                    {
                        Description = "Accelerometer",
                        Id = AccelerometerSensorX_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    AccelerometerSensorY_PortModelId,
                    new PortDescription()
                    {
                        Description = "Accelerometer",
                        Id = AccelerometerSensorY_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    AccelerometerSensorZ_PortModelId,
                    new PortDescription()
                    {
                        Description = "Accelerometer",
                        Id = AccelerometerSensorZ_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // gyro
        public const string GyroscopeSensorX_PortModelId = "X";
        public const string GyroscopeSensorY_PortModelId = "Y";
        public const string GyroscopeSensorZ_PortModelId = "Z";
        public static ThingModelType GyroscopeSensorModel = new ThingModelType
        {
            Name = "Simple Gyroscope",
            Id = ThingTypeLibrary.GyroscopeSensor_ModelType,
            Description = "Simple Gyroscope",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    GyroscopeSensorX_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gyroscope",
                        Id = GyroscopeSensorX_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    GyroscopeSensorY_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gyroscope",
                        Id = GyroscopeSensorY_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    GyroscopeSensorZ_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gyroscope",
                        Id = GyroscopeSensorZ_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // rotation
        public const string RotationSensorW_PortModelId = "W";
        public const string RotationSensorX_PortModelId = "X";
        public const string RotationSensorY_PortModelId = "Y";
        public const string RotationSensorZ_PortModelId = "Z";
        public static ThingModelType RotationSensorModel = new ThingModelType
        {
            Name = "Simple Rotation",
            Id = ThingTypeLibrary.RotationSensor_ModelType,
            Description = "Simpleoattion",
            PortModels = new Dictionary<string, PortDescription>
            {
                {
                    RotationSensorW_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorW_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    RotationSensorX_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorX_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    RotationSensorY_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorY_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    RotationSensorZ_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation",
                        Id = RotationSensorZ_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // rotation euler
        public const string RotationEulerSensorPhi_PortModelId = "Phi";
        public const string RotationEulerSensorTheta_PortModelId = "Theta";
        public const string RotationEulerSensorPsi_PortModelId = "Psi";
        public static ThingModelType RotationEulerSensorModel = new ThingModelType
        {
            Name = "Simple Rotation Euler",
            Id = ThingTypeLibrary.RotationEulerSensor_ModelType,
            Description = "Simple Rotation Euler",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    RotationEulerSensorPhi_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation EUler",
                        Id = RotationEulerSensorPhi_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    RotationEulerSensorTheta_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation EUler",
                        Id = RotationEulerSensorTheta_PortModelId,
                        ioDirection =ioPortDirection.Output,
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
                    RotationEulerSensorPsi_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rotation EUler",
                        Id = RotationEulerSensorPsi_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // nfc model
        public const string Nfc_PortModelId = "Nfc";
        public static ThingModelType NfcModel = new ThingModelType
        {
            Name = "Simple Nfc",
            Id = ThingTypeLibrary.NfcDefault_ModelType,
            Description = "Simple Nfc",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Nfc_PortModelId,
                    new PortDescription()
                    {
                        Description = "Nfc",
                        Id = Nfc_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "Nfc",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // brigthness normalized model
        public const string BrightnessNormalized_PortModelId = "BrightnessNormalized";
        public static ThingModelType BrightnessNormalizedModel = new ThingModelType
        {
            Name = "Simple Brightness",
            Id = ThingTypeLibrary.LightSensor_ModelType,
            Description = "Simple Brightness",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    BrightnessNormalized_PortModelId,
                    new PortDescription()
                    {
                        Description = "Brightness",
                        Id = BrightnessNormalized_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // brigthness model
        public const string Brightness_PortModelId = "Brightness";
        public static ThingModelType BrightnessModel = new ThingModelType
        {
            Name = "Simple Light Sensor",
            Id = ThingTypeLibrary.LightSensorNonNormalized_ModelType,
            Description = "Simple Light Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Brightness_PortModelId,
                    new PortDescription()
                    {
                        Description = "Light Sensor",
                        Id = Brightness_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // proximity sensor model
        public const string ProximitySensor_PortModelId = "Proximity";
        public static ThingModelType ProximitySensorModel = new ThingModelType
        {
            Name = "Simple Proximity",
            Id = ThingTypeLibrary.ProximitySensor_ModelType,
            Description = "Simple Proximity",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    ProximitySensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Proximity",
                        Id = ProximitySensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Proximity",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // wifi status model
        public const string WifiStatus_PortModelId = "WifiStatus";
        public static ThingModelType WifiStatusModel = new ThingModelType
        {
            Name = "Wifi Status",
            Id = ThingTypeLibrary.WifiStatus_ModelType,
            Description = "Wifi Status",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    WifiStatus_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Status",
                        Id = WifiStatus_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Status",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // wifi rssi model
        public const string WifiRssi_PortModelId = "WifiRssi";
        public static ThingModelType WifiRssiModel = new ThingModelType
        {
            Name = "Wifi Rssi",
            Id = ThingTypeLibrary.WifiRssi_ModelType,
            Description = "Wifi Rssi",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    WifiRssi_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Rssi",
                        Id = WifiRssi_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Rssi",
                        State = new StateDescription()
                        {
                            Type = ePortType.Integer,
                        }
                    }
                }
            }
        };

        // -----------------------
        // wifi ssid model
        public const string WifiSsid_PortModelId = "WifiSsid";
        public static ThingModelType WifiSsidModel = new ThingModelType
        {
            Name = "Wifi Ssid",
            Id = ThingTypeLibrary.WifiSsid_ModelType,
            Description = "Wifi Ssid",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    WifiSsid_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Ssid",
                        Id = WifiSsid_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Ssid",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // wifi Bssid model
        public const string WifiBssid_PortModelId = "WifiBssid";
        public static ThingModelType WifiBssidModel = new ThingModelType
        {
            Name = "Wifi Bssid",
            Id = ThingTypeLibrary.WifiBssid_ModelType,
            Description = "Wifi Bssid",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    WifiBssid_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Bssid",
                        ioDirection = ioPortDirection.Output,
                        Id = WifiBssid_PortModelId,
                        Label = "Wifi Bssid",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // wifi info model
        public static ThingModelType WifiInfoModel = new ThingModelType
        {
            Name = "Wifi Info",
            Id = ThingTypeLibrary.WifiInfo_ModelType,
            Description = "Wifi Info",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    WifiStatus_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Status",
                        Id = WifiStatus_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Status",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    WifiRssi_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Rssi",
                        Id = WifiRssi_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Rssi",
                        State = new StateDescription()
                        {
                            Type = ePortType.Integer,
                        }
                    }
                },
                {
                    WifiSsid_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Ssid",
                        Id = WifiSsid_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Ssid",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    WifiBssid_PortModelId,
                    new PortDescription()
                    {
                        Description = "Wifi Bssid",
                        Id = WifiBssid_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Wifi Bssid",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // shake detector model
        public const string ShakeDetector_PortModelId = "ShakeDetector";
        public static ThingModelType ShakeDetectorModel = new ThingModelType
        {
            Name = "Simple Shake Detector",
            Id = ThingTypeLibrary.ShakeDetectorSensor_ModelType,
            Description = "Simple Shake Detector",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    ShakeDetector_PortModelId, new PortDescription()
                    {
                        Description = "Shake Detector",
                        Id = ShakeDetector_PortModelId,
                        ioDirection=ioPortDirection.Output,
                        Label = "Shake Detector",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // camera model
        public const string Camera_PortModelId = "Camera Feed";
        public static ThingModelType CameraModel = new ThingModelType
        {
            Name = "Simple Camera",
            Id = ThingTypeLibrary.CameraDefault_ModelType,
            Description = "Simple Camera",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Camera_PortModelId,
                    new PortDescription()
                    {
                        Description = "Camera",
                        Id = Camera_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Camera",
                        State = new StateDescription()
                        {
                            Type = ePortType.BinaryResourceDescriptor,
                        }
                    }
                }
            }
        };

        // -----------------------
        // camera trigger model
        public const string TriggeredCamera_PortModelId = "Camera trigger";
        public static ThingModelType TriggeredCameraModel = new ThingModelType
        {
            Name = "Simple Camera with trigger",
            Id = ThingTypeLibrary.CameraTriggered_ModelType,
            Description = "Simple Camera with trigger",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Camera_PortModelId,
                    new PortDescription()
                    {
                        Description = "Camera",
                        Id = Camera_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Camera",
                        State = new StateDescription()
                        {
                            Type = ePortType.BinaryResourceDescriptor,
                        }
                    }
                },
                {
                    TriggeredCamera_PortModelId,
                    new PortDescription()
                    {
                        Description = "Camera Trigger",
                        Id = TriggeredCamera_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Camera Trigger",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // OnVif camera model
        public const string OnVifCamera_PortModelId = "OnVifCamera";
        public static ThingModelType OnVifCameraModel = new ThingModelType
        {
            Name = "OnVif Camera",
            Id = ThingTypeLibrary.CameraOnVif_ModelType,
            Description = "OnVif Camera",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Camera_PortModelId,
                    new PortDescription()
                    {
                        Description = "Camera",
                        Id = Camera_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Camera",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    OnVifCamera_PortModelId,
                    new PortDescription()
                    {
                        Description = "Streaming",
                        Id = OnVifCamera_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Streaming",
                        State = new StateDescription()
                        {
                            Type = ePortType.VideoDescriptor,
                        }
                    }
                }
            }
        };

        // -----------------------
        // microphone model
        public const string Microphone_PortModelId = "Microphone";
        public static ThingModelType MicrophoneModel = new ThingModelType
        {
            Name = "Simple Microphone",
            Id = ThingTypeLibrary.MicrophoneDefault_ModelType,
            Description = "Simple Microphone",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Microphone_PortModelId,
                    new PortDescription()
                    {
                        Description = "Microphone",
                        Id = Microphone_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Microphone",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // sip model
        public const string SipPhone_PortModelId = "SipPhone";
        public static ThingModelType SipPhoneModel = new ThingModelType
        {
            Name = "Simple SipPhone",
            Id = ThingTypeLibrary.SipPhoneDefault_ModelType,
            Description = "Simple SipPhone",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SipPhone_PortModelId,
                    new PortDescription()
                    {
                        Description = "SipPhone",
                        Id = SipPhone_PortModelId,
                        ioDirection =ioPortDirection.Input,
                        Label = "SipPhone",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // speech recognition model
        public const string SpeechRecognition_PortModelId = "SpeechRecognition";
        public static ThingModelType SpeechRecognitionModel = new ThingModelType
        {
            Name = "Simple Speech Recognition",
            Id = ThingTypeLibrary.SpeechRecognitionDefault_ModelType,
            Description = "Simple SpeechRecognition",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SpeechRecognition_PortModelId,
                    new PortDescription()
                    {
                        Description = "SpeechRecognition",
                        Id = SpeechRecognition_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "SpeechRecognition",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // txt2speech model
        public const string Text2Speech_PortModelId = "Text2Speech";
        public static ThingModelType Text2SpeechModel = new ThingModelType
        {
            Name = "Simple Text to Speech",
            Id = ThingTypeLibrary.Text2SpeechDefault_ModelType,
            Description = "Simple Text to Speech",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Text2Speech_PortModelId,
                    new PortDescription()
                    {
                        Description = "Text to Speech",
                        Id = Text2Speech_PortModelId,
                        ioDirection =ioPortDirection.Input,
                        Label = "Text to Speech",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // sound sensor model
        public const string SoundSensor_PortModelId = "SoundSensor";
        public static ThingModelType SoundSensorModel = new ThingModelType
        {
            Name = "Simple Sound Sensor",
            Id = ThingTypeLibrary.SoundSensor_ModelType,
            Description = "Simple Sound Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SoundSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Sound Sensor",
                        Id = SoundSensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // humidity sensor model
        public const string HumiditySensor_PortModelId = "HumiditySensor";
        public static ThingModelType HumiditySensorModel = new ThingModelType
        {
            Name = "Simple Humidity Sensor",
            Id = ThingTypeLibrary.HumiditySensor_ModelType,
            Description = "Simple Humidity Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    HumiditySensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Humidity Sensor",
                        Id = HumiditySensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // buzzer model
        public const string Buzzer_PortModelId = "Buzzer";
        public static ThingModelType BuzzerModel = new ThingModelType
        {
            Name = "Simple Buzzer",
            Id = ThingTypeLibrary.BuzzerDefault_ModelType,
            Description = "Buzzer",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Buzzer_PortModelId,
                    new PortDescription()
                    {
                        Description = "Buzzer",
                        Id = Buzzer_PortModelId,
                        ioDirection =ioPortDirection.Input,
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

        // -----------------------
        // ht sensor model
        public static ThingModelType HTSensorModel = new ThingModelType
        {
            Name = "Humidity and Temperature Sensor",
            Id = ThingTypeLibrary.HTSensor_ModelType,
            Description = "HT Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    TemperatureSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Temperature Sensor",
                        Id = TemperatureSensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Temperature Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.DecimalHigh,
                        }
                    }
                },
                {
                    HumiditySensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Humidity Sensor",
                        Id =  HumiditySensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Humidity Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        // -----------------------
        // temperature sensor model
        public const string TemperatureSensor_PortModelId = "Temperature";
        public static ThingModelType TemperatureSensorModel = new ThingModelType
        {
            Name = "Temperature Sensor",
            Id = ThingTypeLibrary.TemperatureSensor_ModelType,
            Description = "Temperature Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    TemperatureSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Temperature Sensor",
                        Id = TemperatureSensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Temperature Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.DecimalHigh,
                        }
                    }
                }
            }
        };

        // -----------------------
        // ultrasonic sensor model
        public const string UltrasonicSensor_PortModelId = "UltrasonicSensor";
        public static ThingModelType UltrasonicSensorModel = new ThingModelType
        {
            Name = "Simple Ultrasonic Sensor",
            Id = ThingTypeLibrary.ProximityUltrasonicSensor_ModelType,
            Description = "Simple Ultrasonic Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    UltrasonicSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Ultrasonic Sensor",
                        Id = UltrasonicSensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // lcd model
        public const string Lcd_PortModelId = "LCD";
        public static ThingModelType LcdModel = new ThingModelType
        {
            Name = "Simple Lcd",
            Id = ThingTypeLibrary.LcdDefault_ModelType,
            Description = "Simple Lcd",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Lcd_PortModelId,
                    new PortDescription()
                    {
                        Description = "Lcd",
                        Id = Lcd_PortModelId,
                        ioDirection =ioPortDirection.Input,
                        Label = "Lcd",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // relay model
        public const string Relay_PortModelId = "Relay";
        public static ThingModelType RelayActuatorModel = new ThingModelType
        {
            Name = "Simple Relay",
            Id = ThingTypeLibrary.SwitchActuatorRelay_ModelType,
            Description = "Simple Relay",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Relay_PortModelId,
                    new PortDescription()
                    {
                        Description = "Relay",
                        Id = Relay_PortModelId ,
                        ioDirection = ioPortDirection.Output,
                        Label = "Relay",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // gesture sensor model
        public const string GestureSensorTap_PortModelId = "Tap";
        public const string GestureSensorTouch_PortModelId = "Touch";
        public const string GestureSensorAirwheel_PortModelId = "Airwheel";
        public const string GestureSensorDoubleTap_PortModelId = "DoubleTap";
        public const string GestureSensorFlick_PortModelId = "Flick";
        public static ThingModelType GestureSensorModel = new ThingModelType
        {
            Name = "Simple Gesture Sensor",
            Id = ThingTypeLibrary.GestureSensor_ModelType,
            Description = "Simple Gesture Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    GestureSensorTap_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorTap_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorTouch_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorTouch_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorDoubleTap_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorDoubleTap_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorFlick_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorFlick_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    GestureSensorAirwheel_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gesture Sensor",
                        Id = GestureSensorAirwheel_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Gesture Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
            }
        };

        // -----------------------
        // position sensor model
        public const string PositionSensorX_PortModelId = "X";
        public const string PositionSensorY_PortModelId = "Y";
        public const string PositionSensorZ_PortModelId = "Z";
        public static ThingModelType PositionSensorModel = new ThingModelType
        {
            Name = "Simple Position Sensor",
            Id = ThingTypeLibrary.PositionSensor_ModelType,
            Description = "Simple Position Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    PositionSensorX_PortModelId,
                    new PortDescription()
                    {
                        Description = "Position Sensor",
                        Id = PositionSensorX_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Position Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    PositionSensorY_PortModelId,
                    new PortDescription()
                    {
                        Description = "Position Sensor",
                        Id = PositionSensorY_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Position Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    PositionSensorZ_PortModelId,
                    new PortDescription()
                    {
                        Description = "Position Sensor",
                        Id = PositionSensorZ_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Position Sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // beacon reader model
        public const string BeaconReaderUuid_PortModelId = "BeaconUUID";
        public const string BeaconReaderMajorValue_PortModelId = "BeaconMajorValue";
        public const string BeaconReaderMinorValue_PortModelId = "BeaconMinorValue";
        public const string BeaconReaderTxPower_PortModelId = "BeaconTxPower";
        public const string BeaconReaderDistance_PortModelId = "BeaconDistance";
        public const string BeaconReaderRssi_PortModelId = "BeaconRssi";
        public const string BeaconReaderTypeCode_PortModelId = "BeaconTypeCode";
        public const string BeaconReaderTemperature_PortModelId = "BeaconTemperature";
        public static ThingModelType BeaconReaderModel = new ThingModelType
        {
            Name = "Beacon Reader",
            Id = ThingTypeLibrary.BeaconReaderDefault_ModelType,
            Description = "Beacon Reader",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    BeaconReaderUuid_PortModelId,
                    new PortDescription()
                    {
                        Description = "UUID",
                        Id = BeaconReaderUuid_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "UUID",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderMajorValue_PortModelId,
                    new PortDescription()
                    {
                        Description = "Major value",
                        Id = BeaconReaderMajorValue_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Major value",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderMinorValue_PortModelId,
                    new PortDescription()
                    {
                        Description = "Minor Value",
                        Id = BeaconReaderMinorValue_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "MinorvValue",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderTxPower_PortModelId,
                    new PortDescription()
                    {
                        Description = "Tx Power",
                        Id = BeaconReaderTxPower_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Tx Power",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderRssi_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rssi",
                        Id = BeaconReaderRssi_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Rssi",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderDistance_PortModelId,
                    new PortDescription()
                    {
                        Description = "Distance",
                        Id = BeaconReaderDistance_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Distance",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderTypeCode_PortModelId,
                    new PortDescription()
                    {
                        Description = "Type Code",
                        Id = BeaconReaderTypeCode_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Type Code",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        public static ThingModelType BeaconReaderSensorsModel = new ThingModelType
        {
            Name = "Beacon Reader with sensors",
            Id = ThingTypeLibrary.BeaconReaderSensors_ModelType,
            Description = "Beacon Reader with sensors",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    BeaconReaderUuid_PortModelId,
                    new PortDescription()
                    {
                        Description = "UUID",
                        Id = BeaconReaderUuid_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "UUID",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderMajorValue_PortModelId,
                    new PortDescription()
                    {
                        Description = "Major value",
                        Id = BeaconReaderMajorValue_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Major value",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderMinorValue_PortModelId,
                    new PortDescription()
                    {
                        Description = "Minor Value",
                        Id = BeaconReaderMinorValue_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "MinorvValue",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderTxPower_PortModelId,
                    new PortDescription()
                    {
                        Description = "Tx Power",
                        Id = BeaconReaderTxPower_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Tx Power",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderRssi_PortModelId,
                    new PortDescription()
                    {
                        Description = "Rssi",
                        Id = BeaconReaderRssi_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Rssi",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderDistance_PortModelId,
                    new PortDescription()
                    {
                        Description = "Distance",
                        Id = BeaconReaderDistance_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Distance",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderTypeCode_PortModelId,
                    new PortDescription()
                    {
                        Description = "Type Code",
                        Id = BeaconReaderTypeCode_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Type Code",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                },
                {
                    BeaconReaderTemperature_PortModelId,
                    new PortDescription()
                    {
                        Description = "Temperature",
                        Id = BeaconReaderTemperature_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Temperature",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // magnetometer model
        public const string MagnetometerSensorX_PortModelId = "X";
        public const string MagnetometerSensorY_PortModelId = "Y";
        public const string MagnetometerSensorZ_PortModelId = "Z";
        public static ThingModelType MagnetometerSensorModel = new ThingModelType
        {
            Name = "Simple Magnetometer Sensor",
            Id = ThingTypeLibrary.MagnetometerSensor_ModelType,
            Description = "Simple Magnetometer Sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    MagnetometerSensorX_PortModelId ,
                    new PortDescription()
                    {
                        Description = "Magnetometer Sensor",
                        Id = MagnetometerSensorX_PortModelId ,
                        ioDirection =ioPortDirection.Output,
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
                    MagnetometerSensorY_PortModelId ,
                    new PortDescription()
                    {
                        Description = "Magnetometer Sensor",
                        Id = MagnetometerSensorY_PortModelId ,
                        ioDirection =ioPortDirection.Output,
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
                    MagnetometerSensorZ_PortModelId ,
                    new PortDescription()
                    {
                        Description = "Magnetometer Sensor",
                        Id = MagnetometerSensorZ_PortModelId ,
                        ioDirection =ioPortDirection.Output,
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

        // -----------------------
        // gpio model
        public const string Gpio_PortModelId = "GPIO";
        public const string GpioInput_PortModelId = "InputGPIO";
        public const string GpioOutput_PortModelId = "OutputGPIO";
        public static ThingModelType GpioModel = new ThingModelType
        {
            Name = "Simple Gpio",
            Id = ThingTypeLibrary.GpioDefault_ModelType,
            Description = "Simple Gpio",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Gpio_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gpio",
                        Id = Gpio_PortModelId,
                        Label = "Gpio",
                        ioDirection = ioPortDirection.InputOutput,
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public static ThingModelType GpioInputModel = new ThingModelType
        {
            Name = "Simple Gpio",
            Id = ThingTypeLibrary.GpioInput_ModelType,
            Description = "Simple Gpio",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    GpioInput_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gpio",
                        Id = GpioInput_PortModelId,
                        Label = "Gpio",
                        ioDirection = ioPortDirection.Input,
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        public static ThingModelType GpioOutputModel = new ThingModelType
        {
            Name = "Simple Gpio",
            Id = ThingTypeLibrary.GpioOutput_ModelType,
            Description = "Simple Gpio",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    GpioOutput_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gpio",
                        Id = GpioOutput_PortModelId,
                        Label = "Gpio",
                        ioDirection = ioPortDirection.Output,
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // android intent model
        public const string AndroidIntent_PortModelId = "AndroidIntent";
        public static ThingModelType AndroidIntentModel = new ThingModelType
        {
            Name = "Simple Android Intent",
            Id = ThingTypeLibrary.AndroidIntentDefault_ModelType,
            Description = "Simple Android Intent",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    AndroidIntent_PortModelId,
                    new PortDescription()
                    {
                        Description = "Android Intent",
                        Id = AndroidIntent_PortModelId,
                        ioDirection=ioPortDirection.Input,
                        Label = "Android Intent",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // fleet mgr model
        public const string FleetMgr_PortModelId = "FleetMgr";
        public static ThingModelType FleetMgrModel = new ThingModelType
        {
            Name = "Fleet Manager Info",
            Id = ThingTypeLibrary.FleetMgrDefault_ModelType,
            Description = "Fleet Manager Info",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    FleetMgr_PortModelId,
                    new PortDescription()
                    {
                        Description = "Fleet Manager Info",
                        Id = FleetMgr_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Fleet Manager Info",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // displayer model
        public const string Displayer_PortModelId = "Displayer";
        public static ThingModelType DisplayerModel = new ThingModelType
        {
            Name = "Displayer",
            Id = ThingTypeLibrary.DisplayerDefault_ModelType,
            Description = "Displayer",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Displayer_PortModelId,
                    new PortDescription()
                    {
                        Description = "Displayer",
                        Id = Displayer_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Displayer",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // gallery model
        public const string Gallery_PortModelId = "Gallery";
        public static ThingModelType GalleryModel = new ThingModelType
        {
            Name = "Gallery",
            Id = ThingTypeLibrary.GalleryDefault_ModelType,
            Description = "Gallery",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Gallery_PortModelId,
                    new PortDescription()
                    {
                        Description = "Gallery",
                        Id = Gallery_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Gallery",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        //  flickr model
        public const string Flickr_PortModelId = "Flickr";
        public static ThingModelType FlickrModel = new ThingModelType
        {
            Name = "Flickr",
            Id = ThingTypeLibrary.FlickrDefault_ModelType,
            Description = "Flickr",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    Flickr_PortModelId,
                    new PortDescription()
                    {
                        Description = "Flickr",
                        Id = Flickr_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "Flickr",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // Region viewer model
        public const string RegionViewer_PortModelId = "RegionViewer";
        public static ThingModelType RegionViewerModel = new ThingModelType
        {
            Name = "RegionViewer",
            Id = ThingTypeLibrary.RegionViewerDefault_ModelType,
            Description = "RegionViewer",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    RegionViewer_PortModelId,
                    new PortDescription()
                    {
                        Description = "RegionViewer",
                        Id = RegionViewer_PortModelId,
                        ioDirection = ioPortDirection.Input,
                        Label = "RegionViewer",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // Doorlock sensor model
        public const string DoorlockSensor_PortModelId = "Doorlock sensor";
        public static ThingModelType DoorlockSensorModel = new ThingModelType
        {
            Name = "Doorlock sensor",
            Id = ThingTypeLibrary.DoorlockSensor_ModelType,
            Description = "Doorlock sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    DoorlockSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Doorlock sensor",
                        Id = DoorlockSensor_PortModelId,
                        ioDirection =ioPortDirection.Input,
                        Label = "Doorlock sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // door sensor model
        public const string AirConditionSensor_PortModelId = "Air condition sensor";
        public static ThingModelType AirConditionSensorModel = new ThingModelType
        {
            Name = "Air condition sensor",
            Id = ThingTypeLibrary.AirConditionSensor_ModelType,
            Description = "Air condition sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    AirConditionSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Air condition sensor",
                        Id = AirConditionSensor_PortModelId,
                        ioDirection =ioPortDirection.InputOutput,
                        Label = "Air condition sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.String,
                        }
                    }
                }
            }
        };

        // -----------------------
        // door sensor model
        public const string DoorSensor_PortModelId = "Door sensor";
        public static ThingModelType DoorSensorModel = new ThingModelType
        {
            Name = "Door sensor",
            Id = ThingTypeLibrary.DoorSensor_ModelType,
            Description = "Door sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    DoorSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Door sensor",
                        Id = DoorSensor_PortModelId,
                        ioDirection =ioPortDirection.Output,
                        Label = "Door sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // Smart plug sensor model
        public const string SmartPlugSensor_PortModelId = "Smart plug sensor";
        public static ThingModelType SmartPlugSensorModel = new ThingModelType
        {
            Name = "Smart plug sensor",
            Id = ThingTypeLibrary.SmartPlugSensor_ModelType,
            Description = "Smart plug sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    SmartPlugSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "Smart plug sensor",
                        Id = SmartPlugSensor_PortModelId,
                        ioDirection =ioPortDirection.Input,
                        Label = "Smart plug sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.Boolean,
                        }
                    }
                }
            }
        };

        // -----------------------
        // Z-Wave sensor model
        public const string ZWaveSensor_PortModelId = "ZWave sensor";
        public static ThingModelType ZWaveSensorModel = new ThingModelType
        {
            Name = "ZWave sensor",
            Id = ThingTypeLibrary.ZWaveSensor_ModelType,
            Description = "ZWave sensor",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    ZWaveSensor_PortModelId,
                    new PortDescription()
                    {
                        Description = "ZWave sensor",
                        Id = ZWaveSensor_PortModelId,
                        ioDirection =ioPortDirection.Undefined,
                        Label = "ZWave sensor",
                        State = new StateDescription()
                        {
                            Type = ePortType.Undefined,
                        }
                    }
                }
            }
        };

        // -----------------------
        // data plotter model
        public const string DataPlotter_PortModelId = "DataPlotter";
        public static ThingModelType DataPlotterModel = new ThingModelType
        {
            Name = "Data Plotter",
            Id = ThingTypeLibrary.DataPlotterDefault_ModelType,
            Description = "Data Plotter",
            PortModels = new Dictionary<string, PortDescription>()
            {
                {
                    DataPlotter_PortModelId,
                    new PortDescription()
                    {
                        Description = "Data Plotter",
                        Id = DataPlotter_PortModelId,
                        ioDirection = ioPortDirection.Output,
                        Label = "Data Plotter",
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

    // ---------------------------------------------------------------------------------------------

    public static class ThingTypesMigrator
    {
        public static Dictionary<string, string> OldTypeToNewType = new Dictionary<string, string>()
        {
            { "com.yodiwo.input.lights.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsDimmer_ModelType },
            { "com.yodiwo.input.console", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.TextConsole_ModelType },
            { "com.yodiwo.input.text", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.TextConsole_ModelType },
            { "com.yodiwo.output.seekbars", ThingTypeLibrary.Seekbar_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.camera", ThingTypeLibrary.Camera_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.inputoutput.text", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.lights.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsDimmer_ModelType },
            { "yodiwo.output.seekbars", ThingTypeLibrary.Seekbar_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.buttons", ThingTypeLibrary.Button_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.buttons", ThingTypeLibrary.Button_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.microphones", ThingTypeLibrary.Microphone_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.lcds", ThingTypeLibrary.Lcd_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.light", ThingTypeLibrary.LightSensor_ModelType + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightSensorNonNormalized_ModelType },
            { "com.yodiwo.input.androidintent", ThingTypeLibrary.AndroidIntent_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.leds.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsDimmer_ModelType },
            { "com.yodiwo.output.sensors.rotation", ThingTypeLibrary.RotationEulerSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.beacon", ThingTypeLibrary.BeaconDetector + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.lcds", ThingTypeLibrary.Lcd_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.torches", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsBoolean_ModelType },
            { "com.yodiwo.output.nfc", ThingTypeLibrary.Nfc_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.proximity", ThingTypeLibrary.ProximitySensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.gyroscope", ThingTypeLibrary.GyroscopeSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.seekbars", ThingTypeLibrary.Seekbar_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.SeekbarProgressbar_ModelType },
            { "com.yodiwo.output.checkboxes", ThingTypeLibrary.SwitchActuator_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.checkboxes", ThingTypeLibrary.SwitchActuator_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.switches.onoff", ThingTypeLibrary.SwitchActuator_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.switches.onoff", ThingTypeLibrary.Switch_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.switches.onoff", ThingTypeLibrary.SwitchActuator_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.switches.onoff", ThingTypeLibrary.Switch_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.inputoutput.gpios", ThingTypeLibrary.Gpio_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.buzzers", ThingTypeLibrary.Buzzer_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.input.lights.onoff", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsBoolean_ModelType },
            { "com.yodiwo.output.sensors.sound", ThingTypeLibrary.SoundSensor_ModelType + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsBoolean_ModelType },
            { "yodiwo.output.sensors.position", ThingTypeLibrary.PositionSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.gesture", ThingTypeLibrary.GestureSensor_ModelType + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsBoolean_ModelType },
            { "com.yodiwo.output.shakedetectors", ThingTypeLibrary.Sensor_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.ShakeDetectorSensor_ModelType },
            { "com.yodiwo.output.text", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "bulb", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsDimmer_ModelType },
            { "slider", ThingTypeLibrary.Seekbar_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.console", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.TextConsole_ModelType  },
            { "yodiwo.input.text", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.TextConsole_ModelType },
            { "yodiwo.input.webconsole", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.TextConsole_ModelType },
            { "yodiwo.output.slider", ThingTypeLibrary.Seekbar_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.speechrecognition", ThingTypeLibrary.SpeechRecognition_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.text2speech", ThingTypeLibrary.Text2Speech_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.accelerometer", ThingTypeLibrary.AccelerometerSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.nfc", ThingTypeLibrary.Nfc_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.proximity", ThingTypeLibrary.ProximitySensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.leds.dimmable", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsDimmer_ModelType },
            { "yodiwo.input.seekbars", ThingTypeLibrary.Seekbar_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.SeekbarProgressbar_ModelType},
            { "yodiwo.input.androidintent", ThingTypeLibrary.AndroidIntent_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.gyroscope", ThingTypeLibrary.GyroscopeSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.input.lcds", ThingTypeLibrary.Lcd_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.rotation", ThingTypeLibrary.RotationEulerSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "yodiwo.output.sensors.light", ThingTypeLibrary.LightSensor_ModelType + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightSensorNonNormalized_ModelType },
            { "yodiwo.input.torches", ThingTypeLibrary.Lights_Type + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.LightsBoolean_ModelType },
            { "yodiwo.output.text", ThingTypeLibrary.Text_Type + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.enviromental", ThingTypeLibrary.HTSensor_ModelType + PlegmaAPI.ThingModelTypeSeparatorPlusDefault },
            { "com.yodiwo.output.sensors.ultrasonic", ThingTypeLibrary.ProximitySensor_ModelType + PlegmaAPI.ThingModelTypeSeparator + ThingTypeLibrary.ProximityUltrasonicSensor_ModelType },
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
