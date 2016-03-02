using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.Tools
{
    public static class Certificates
    {
        static object locker = new object();
        static X509CertificateCollection cached_Certificates;
        static DateTime cache_Certificate_timestamp;

        public static X509CertificateCollection CollectCertificates()
        {
            lock (locker)
            {
                if (cached_Certificates == null || (DateTime.Now - cache_Certificate_timestamp).TotalSeconds > 100)
                {
                    var certs = new X509CertificateCollection();

                    try
                    {
                        //open auth root store
                        X509Store authstore = new X509Store(StoreName.AuthRoot);
                        authstore.Open(OpenFlags.ReadOnly);

                        //open personal store
                        var mystore = new X509Store(StoreName.My);
                        mystore.Open(OpenFlags.ReadOnly);

                        //collect certs                
                        certs.AddFromSource(authstore.Certificates);
                        certs.AddFromSource(mystore.Certificates);

                        //close stores
                        mystore.Close();
                        authstore.Close();
                    }
                    catch (Exception ex) { DebugEx.Assert(ex, "Could not collect certificates"); }

                    //cache for future use
                    cached_Certificates = certs;
                }
                return cached_Certificates;
            }
        }
    }
}
