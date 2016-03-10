Connect an Azure edge node with Yodiwo Cloud Platform

Azure IOT hub

Section A: Setup Azure IoT Application
 1. Sign in to the Azure portal.
 2. In the Jumpbar, click New, then click Internet of Things, and then click Azure IoT Hub.
 In the IoT hub blade, choose the configuration for your IoT hub, and click create
 3.When the IoT hub has been created successfully, open the blade of the new IoT hub, make a note of the Hostname, and then click the Keys icon.
4. Click the iothubowner policy, then copy and make note of the connection string in the iothubowner blade. (make sure all permissions are selected)
5. Click the service policy (make sure all permissions are selected except of device connect)
6. Click the service policy (make sure all permissions are selected except of serviceconnect)


Create a device entity

Process A
1. Use the RegisterDevice.cs to create a new device identity in the identity registry in your IoT hub. 
2. When you run this console application, it generates a unique device ID and key that your device can identify itself with when it sends device-to-cloud messages to IoT Hub.
3. make a note of this device key

Process B

Use the Device Explorer Tool. The Device Explorer tool (included with the Azure IoT device SDK) uses the Azure IoT service libraries to perform various functions on IoT Hub, including adding devices. If you use Device Explorer to add a device, you’ll get a corresponding connection string. You need this connection string to make the sample applications run.

1. A pre-built version of the Device Explorer application for Windows can be downloaded by clicking on this link: https://github.com/Azure/azure-iot-sdks/releasesDownloads (Scroll down for SetupDeviceExplorer.msi). The default installation directory for this application is "C:\Program Files (x86)\Microsoft\DeviceExplorer". 

2. Enter your IoT Hub Connection String in the first field and click Update. This configures the tool so that it can communicate with IoT Hub.

3.Once the IoT Hub connection string is configured click the Management tab:
You can create a device by clicking the Create button. A dialog is displayed with a set of pre-populated keys (primary and secondary). All you have to do is enter a Device ID and then click Create.
Once the device is created, the Devices list is updated with all registered devices, including the one you just created.  Generate (right tab) a SAS (Shared Access Security) for the device you crated (TTL=365d). A new connection string is generated for the selected device. Keep a copy of the connection string. You’ll need it when running the device sample applications.

Simulate A  device.

Use process A + AzureIOTDev . (c# desktop application)
Use process B + Universal App in Test Repository (universal app in windows iot) 


Follow the steps described in IBM Readme for interconnection with Yodiwo Cloud.

