using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Yodiwo.API.Plegma
{
    public static class Constants
    {
        public const int YPChannelPort = 14623;
        public const string InvalidKeyString = "(Invalid Key)";

        public const string ExtIpAddressKey = "ExternalIP";
        public const string IntIpAddressKey = "InternalIP";

        public const string HierarchySeparator = "/";
    }

    public static class A2mcuConfigConstants
    {
        public const string ConfigNameSamplingPeriod = "INPUT_ACQUISITION";
        public const string ConfigNameIrqTrigger = "IRQ_TRIGGER";
        public const string ConfigNameSamplingSlave = "SAMPLING_SLAVE";
        public const string ConfigNameSamplingRegister = "SAMPLING_REG";
        public const string ConfigNameSamplingBytes = "SAMPLING_BYTES";
    }

}
