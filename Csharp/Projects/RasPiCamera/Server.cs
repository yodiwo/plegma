using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;
using Yodiwo.Services.Media;
using Yodiwo.Media.Video.Sink.MjpegServer;
using Yodiwo;
using Yodiwo.YPChannel;
using Yodiwo.API.Plegma;
using Yodiwo.API.MediaStreaming;

namespace Yodiwo.Projects.RasPiCamera
{
    public class TCPImageServer
    {
        public bool Started = false;
        Socket server;
        public delegate void OnRasPiImage(Bitmap bmp);
        public OnRasPiImage OnRasPiImagecb = null;

        public void Start(int port)
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, (int)port));
            server.Listen(10);
            Started = true;
            Task.Run(() =>
            {
                while (true)
                    WaitForClients();
            });
        }

        public void WaitForClients()
        {
            Socket client = server.Accept();
            DebugEx.TraceLog("Server Accepted new Connection");
            NetworkStream stream = new NetworkStream(client);
            int counter = 0;
            Console.WriteLine("Started Receiving from python socket");
            while (true)
            {
                counter++;
                byte[] filesize = new byte[4];
                var readbytes = stream.Read(filesize, 0, 4);
                int size = BitConverter.ToInt32(filesize, 0);
                if (size > 0 && size < 10 * 1000 * 1000)
                {
                    //Console.WriteLine("int: {0} {1}", size, readbytes);
                    byte[] imgData = new byte[size];
                    int offset = 0;
                    while (offset < size)
                    {
                        //DebugEx.TraceLog("Reading until size");
                        var read = stream.Read(imgData, offset, size - offset);
                        offset += read;
                    }

                    var ms = new MemoryStream();
                    ms.Write(imgData, 0, imgData.Length);
                    ms.Position = 0;
                    //DebugEx.TraceLog("construct bitmap");
                    try
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        OnRasPiImagecb(bitmap);
                    }
                    catch (Exception ex)
                    {
                        DebugEx.TraceError(ex.Message + " cannot construct bitmap");
                    }

                }

            }
        }
    }


}

