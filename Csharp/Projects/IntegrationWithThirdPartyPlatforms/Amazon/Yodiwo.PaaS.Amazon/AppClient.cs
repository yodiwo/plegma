using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Yodiwo.PaaS.Amazon
{
    public class AWSApplicationClient
    {
        #region Variables
        //------------------------------------------------------------------------------------------------------------------------
        MqttClient _mqttClient;
        public delegate void OnRxMessage(string mqttmessage, string topic);
        public OnRxMessage OnRxMessagecb = null;
        //------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructor
        //------------------------------------------------------------------------------------------------------------------------
        public AWSApplicationClient(string brokerHostName, string clientpfx, string rootcert)
        {
            //create pfc certificate from cert and private key, using openssl
            //openssl pkcs12 -export -out YOURPFXFILE.pfx -inkey -private.pem.key -in -certificate.pem.crt
            var clientCert = new X509Certificate(clientpfx);
            //this is the AWS root.pem file
            var caCert = X509Certificate.CreateFromSignedFile(rootcert);
            //create mqtt client
            _mqttClient = new MqttClient(brokerHostName,
                                            MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT,
                                            true,
                                            caCert,
                                            clientCert,
                                            MqttSslProtocols.TLSv1_2,
                                            UserCertificateValidationCallback);
        }
        #endregion

        #region Functions
        //------------------------------------------------------------------------------------------------------------------------
        public void Start(string clientId)
        {
            //register callbacks of mqtt client
            _mqttClient.MqttMsgPublishReceived += _mqttClient_MqttMsgPublishReceived;
            _mqttClient.MqttMsgSubscribed += _mqttClient_MqttMsgSubscribed;
            _mqttClient.MqttMsgUnsubscribed += _mqttClient_MqttMsgUnsubscribed;
            _mqttClient.ConnectionClosed += _mqttClient_ConnectionClosed;
            var rc = _mqttClient.Connect(clientId);
            if (rc != 0)
            {
                DebugEx.TraceError("Mqtt Connection Error: " + rc.ToString());
                _mqttClient = null;
                return;
            }
            else
                _mqttClient.Subscribe(new string[] { "$aws/things/raspi/shadow/update/#" }, new byte[] { 1 });
        }

        //------------------------------------------------------------------------------------------------------------------------
        private void _mqttClient_ConnectionClosed(object sender, EventArgs e)
        {
            DebugEx.TraceError("Mqtt Connection Closed");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void _mqttClient_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
        {
            DebugEx.TraceLog("MqttClient:: Message id " + e.MessageId + " unsubscribed successfully");
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void _mqttClient_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
        {
            DebugEx.TraceLog("MqttClient:: Message id " + e.MessageId + " subscribed successfully with QoS levels" + e.GrantedQoSLevels.ToStringEx());
        }
        //------------------------------------------------------------------------------------------------------------------------
        private void _mqttClient_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var mqttMsg = Encoding.UTF8.GetString(e.Message);
            DebugEx.TraceLog(System.DateTime.Now
                             + ": MqttClient: Received msg: " + mqttMsg
                             + " on topic: " + e.Topic
                             + ", retained: " + e.Retain
                             + ", QoS level: " + e.QosLevel
                             );
            if (OnRxMessagecb != null)
                OnRxMessagecb(System.Text.Encoding.Default.GetString(e.Message), e.Topic);
        }
        //------------------------------------------------------------------------------------------------------------------------
        private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors != SslPolicyErrors.None
                //TODO: for testing only! ignore name mismatch (i.e. server hosname being other than what's on the certificate)
                && sslPolicyErrors != SslPolicyErrors.RemoteCertificateNameMismatch
                )
            {
                DebugEx.TraceError("Ssl Policy errors found: " + sslPolicyErrors);
                return false;
            }
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        public void Publish(string topic, string data)
        {
            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(data));
        }
        //------------------------------------------------------------------------------------------------------------------------
        #endregion
    }
}
