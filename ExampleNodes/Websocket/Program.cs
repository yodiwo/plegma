using System;
using Nancy.Hosting.Self;

namespace Yodiwo.Projects.WebsocketNode
{
    static class Program
    {
        static void Main(string[] args)
        {
            var port = 3400;
            HostConfiguration hostConfigs = new HostConfiguration();
            hostConfigs.UrlReservations.CreateAutomatically = true;

            while (true)
            {
                try
                {
                    using (var host = new NancyHost(hostConfigs, new Uri("http://localhost:" + port)))
                    {
                        host.Start();

                        Console.WriteLine("Your application is running on port " + port);
                        Console.WriteLine("Press any key to close the host.");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    port++;
                }
            }
        }
    }
}

