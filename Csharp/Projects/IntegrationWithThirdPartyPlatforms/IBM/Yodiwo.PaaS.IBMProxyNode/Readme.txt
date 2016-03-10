Connect an IBM edge node with Yodiwo Cloud Platform

IBM:

Section A: Setup BlueMix application
 
1.) Sign up in the BlueMix site: http://ibm.biz/bluestart
2.) Catalog->Internet of Things Application Foundation-> put the name of your application(it should me unique i.e sofdimgre) -> Create -> View Application Overview->
The screen gives the status of the application, how many instances are active, what is the memory it is using and what back-end it is connected to. In this case, it’s a Cloudant NoSQL database.
Check the following information
ii.) Route Url (right top. i.e sofdimgre.eu-gb.mybluemix.net/)
i.)App Health: should be running
3.) click on the route url->Node red flow editor
4.) Node red starts with a sample iot application
5.) You can make tests with NodeRed using a simulator node https://quickstart.internetofthings.ibmcloud.com/iotsensor/
6.)Please note that a unique Device Id (it a  MAC address)  is generated for you , the IoT node in Bluemix will communicate to this sensor via this Device id.
7.) Got to the node red window, double click on IBM IoT App In and change device id with the previous id from the simulated node.
8.) Deploy and see debug messages

Section B: Testing Node-Red
1.) Go to the Application Dashboard(https://console.eu-gb.bluemix.net/?direct=classic)->click the application that you have created in Section A
2.) click Add Service, then select the Internet of Things Foundation (near the bottom)
3.) On the next screen, give the service a name
4.) Next, click on the Internet of Things service in the application dashboard. This will open the configuration page for the service. From here, you can click the 'launch' button to open the Internet of Things Foundation dashboard.


Section C: Internet of Things Foundation

1.)you should now see your Internet of Things Foundation dashboard.
2.)First, look for the 6-character string next to the organization name (ex. e3dp4x)— this is your deviceOrg, which you will use when connecting to IoT.
3.) Next, you will register a device with your organization. A device is used to identify a unique connection from the IoT Starter application. Click Add Device and create a new device type.
For the iOS application, use 'iPhone'.
For the Android application, use 'Android'.
Note that for the IoT Starter application, these device types are case sensitive and must be entered as they appear here.
4.)Specify a unique deviceId (ex. AAA), then click Continue.
You will now see the credentials for your device— COPY THESE DOWN. Make note of the auth-token, this will be used when connecting to IoT, and you cannot access it in the future. (
In the example images to the right, we have: 

deviceOrg: e3dp4x
deviceType: iPhone
deviceId: AAA
auth-token: 18FA+etA*cNq9CJjHz

5.) It isn't required to create a Node-RED application to interact with IoT Foundation. In your IoT dashboard, you can create an API Key and Token(Access). With these, you can connect any application with an MQTT client to your IoT service and perform all the same functions you would from a Node-RED application. 
6.) For the interconnection with Yodiwo Cloud Platform Step 5 is essential

Yodiwo:

1.) Create a node from UI and get a nodekey and secretkey(missing) 
2.) configure config.xml, located in Yodiwo.PaaS.IBMProxyNode.
Information for IBM and Yodiwo are required.
3.) Start Worker and Start Yodiwo.PaaS.IBMProxyNode
4.) A new node should appear on the UI


For the Node Edge Device, you can use DevClient Wrapper(tested on windowsa-10 desktop and on raspbian)