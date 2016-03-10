using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.PaaS.AWSProxyNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Yodiwo.PaaS.AWSProxyNode.AWSProxy proxy = new Yodiwo.PaaS.AWSProxyNode.AWSProxy();
            proxy.Init();
            while (true)
                Task.Delay(200);
        }
    }
}
