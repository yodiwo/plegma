using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Gateway.GatewayMain.Nest
{
    public class Thermostat
    {
        public string humidity { get; set; }
        public string locale { get; set; }
        public string temperature_scale { get; set; }
        public string is_using_emergency_heat { get; set; }
        public string has_fan { get; set; }
        public string software_version { get; set; }
        public string has_leaf { get; set; }
        public string where_id { get; set; }
        public string device_id { get; set; }
        public string name { get; set; }
        public string can_heat { get; set; }
        public string can_cool { get; set; }
        public string hvac_mode { get; set; }
        public string target_temperature_c { get; set; }
        public string target_temperature_f { get; set; }
        public string target_temperature_high_c { get; set; }
        public string target_temperature_high_f { get; set; }
        public string target_temperature_low_c { get; set; }
        public string target_temperature_low_f { get; set; }
        public string ambient_temperature_c { get; set; }
        public string ambient_temperature_f { get; set; }
        public string away_temperature_high_c { get; set; }
        public string away_temperature_high_f { get; set; }
        public string away_temperature_low_c { get; set; }
        public string away_temperature_low_f { get; set; }
        public string structure_id { get; set; }
        public string fan_timer_active { get; set; }
        public string fan_timer_timeout { get; set; }
        public string name_long { get; set; }
        public string is_online { get; set; }
        public string hvac_state { get; set; }
    }

    public class SmokeCoAlarm
    {
        public string structure_id { get; set; }
        public string where_id { get; set; }
        public string name { get; set; }
        public string name_long { get; set; }
        public string is_online { get; set; }
        public string device_id { get; set; }
        public string co_alarm_state { get; set; }
        public string smoke_alarm_state { get; set; }
        public string battery_health { get; set; }
        public string is_manual_test_active { get; set; }
    }

    public class CameraLastEvent
    {
        public string has_sound { get; set; }
        public string has_motion { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string urls_expire_time { get; set; }
        public string web_url { get; set; }
        public string app_url { get; set; }
    }

    public class Camera
    {
        public string name { get; set; }
        public string software_version { get; set; }
        public string where_id { get; set; }
        public string device_id { get; set; }
        public string structure_id { get; set; }
        public bool is_online { get; set; }
        public bool is_streaming { get; set; }
        public bool is_audio_input_enabled { get; set; }
        public string last_is_online_change { get; set; }
        public bool is_video_history_enabled { get; set; }
        public CameraLastEvent last_event { get; set; }
        public string name_long { get; set; }
        public string web_url { get; set; }
        public string app_url { get; set; }
    }



    public class NestDevices
    {
        public Dictionary<string, Thermostat> thermostats { get; set; }
        public Dictionary<string, SmokeCoAlarm> smoke_co_alarms { get; set; }
        public Dictionary<string, Camera> cameras { get; set; }
    }

    public class NestDescriptor
    {
        public NestSensor type;
        public object nestthing;
    }

    public enum NestSensor
    {
        None,
        Thermostats,
        Cameras,
        Smoke_Co_Alarms
    }
}

