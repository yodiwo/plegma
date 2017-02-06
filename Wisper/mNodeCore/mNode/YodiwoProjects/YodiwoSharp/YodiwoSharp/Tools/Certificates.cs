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
        static DateTime cache_Certificate_timestamp = default(DateTime);

        public static X509CertificateCollection CollectCertificates()
        {
            lock (locker)
            {
                //check cache
                if (cached_Certificates == null || (DateTime.Now - cache_Certificate_timestamp).TotalSeconds > 30)
                {
                    var certs = new X509CertificateCollection();

                    //collect auth root store certs
                    try
                    {
                        //open auth root store
                        var store = new X509Store(StoreName.AuthRoot);
                        store.Open(OpenFlags.ReadOnly);

                        //collect
                        certs.AddFromSource(store.Certificates);

                        //close
                        store.Close();
                    }
                    catch (Exception ex) { DebugEx.Assert(ex, "Could not collect certificates"); }

                    //collect personal store
                    try
                    {
                        //open personal store
                        var store = new X509Store(StoreName.My);
                        store.Open(OpenFlags.ReadOnly);

                        //collect certs                
                        certs.AddFromSource(store.Certificates);

                        //close stores
                        store.Close();
                    }
                    catch (Exception ex) { DebugEx.Assert(ex, "Could not collect certificates"); }

                    //cache for future use
                    cached_Certificates = certs;
                    cache_Certificate_timestamp = DateTime.Now;
                }

                //return cached certs
                return cached_Certificates;
            }
        }
    }
}
