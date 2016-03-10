using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace Yodiwo.PaaS.Azure.Device
{
    public static class RegisterDevice
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        static RegistryManager registryManager;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public static string AddDevice(string devname, string connectionstring)
        {
            //create registermanager
            registryManager = RegistryManager.CreateFromConnectionString(connectionstring);
            //add device  to registry identity -- get devkey
            var res = AddDeviceAsync(devname).Result;
            return res;
        }
        //------------------------------------------------------------------------------------------------------------------------
        private async static Task<string> AddDeviceAsync(string deviceId)
        {
            Microsoft.Azure.Devices.Device device = null;
            bool registeredDevicesucceeded;
            try
            {
                device = await registryManager.AddDeviceAsync(new Microsoft.Azure.Devices.Device(deviceId));
                registeredDevicesucceeded = true;
            }
            catch (DeviceAlreadyExistsException)
            {
                registeredDevicesucceeded = false;
            }
            if (!registeredDevicesucceeded)
                device = await registryManager.GetDeviceAsync(deviceId);
            return device.Authentication.SymmetricKey.PrimaryKey;
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
