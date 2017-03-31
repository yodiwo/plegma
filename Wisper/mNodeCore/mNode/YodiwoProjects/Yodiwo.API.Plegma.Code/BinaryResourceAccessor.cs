using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.API.Plegma;
using Yodiwo;

namespace Yodiwo.API.Plegma.Code
{
    public static class BinaryResourceAccessor
    {
        private static DictionaryTS<eBinaryResourceLocationType, Func<BinaryResourceDescriptor, Byte[]>> FetchHandlers;
        static BinaryResourceAccessor()
        {
            FetchHandlers = new DictionaryTS<eBinaryResourceLocationType, Func<BinaryResourceDescriptor, byte[]>>();

            FetchHandlers.Add(eBinaryResourceLocationType.Http, HttpFetchHandler);
            FetchHandlers.Add(eBinaryResourceLocationType.RedisDB, RedisDBFetchHandler);
        }

        /// <summary>
        /// Get resource based on given descriptor
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static byte[] GetResource(BinaryResourceDescriptor descriptor)
        {
            byte[] resource = null;

            try
            {
                if (FetchHandlers.ContainsKey(descriptor.LocationType))
                {
                    resource = FetchHandlers[descriptor.LocationType](descriptor);
                }
                else
                {
                    DebugEx.TraceError("No handler for resource descriptor of type: " + descriptor.LocationType.ToString());
                }
            }
            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Error getting resource from specified resource descriptor");
            }

            return resource;
        }

        public static MemoryStream GetResourceAsStream(BinaryResourceDescriptor descriptor)
        {
            byte[] byteResource = GetResource(descriptor);

            return new MemoryStream(byteResource);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BinaryResourceDescriptor CreateDescriptor(byte[] resource, eBinaryResourceLocationType locationType, object locationDescriptor)
        {
            BinaryResourceDescriptor descriptor = new BinaryResourceDescriptor();

            try
            {
                // Construct descriptor
                if (locationType.Equals(eBinaryResourceLocationType.Http))
                {
                    var locDesc = locationDescriptor as HttpLocationDescriptor;
                    descriptor.LocationDescriptorJson = (new HttpLocationDescriptor { Uri = locDesc.Uri }).ToJSON();

                    // Use Dropbox's REST API to upload resource
                }
                else if (locationType.Equals(eBinaryResourceLocationType.RedisDB))
                {
                    var locDesc = locationDescriptor as RedisDBLocationDescriptor;
                    descriptor.LocationDescriptorJson = (new RedisDBLocationDescriptor()).ToJSON();
                }
            }
            catch (Exception ex)
            {
                descriptor = null;
                DebugEx.Assert(ex, "Error constructing descriptor for specified resource");
            }

            return descriptor;
        }

        #region Handlers

        public static byte[] HttpFetchHandler(BinaryResourceDescriptor descriptor)
        {
            byte[] resource = null;

            // Fetch resource from HTTP location
            try
            {
                var uri = (descriptor.LocationDescriptor as HttpLocationDescriptor).Uri;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (Stream stream = response.GetResponseStream())
                using (var ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        byte[] buf = new byte[1024];
                        count = stream.Read(buf, 0, 1024);
                        ms.Write(buf, 0, count);
                    } while (stream.CanRead && count > 0);
                    resource = ms.ToArray();
                }
            }

            catch (Exception ex)
            {
                DebugEx.Assert(ex, "Error fetching resource from specified HTTP location");
            }

            return resource;
        }

        public static byte[] RedisDBFetchHandler(BinaryResourceDescriptor descriptor)
        {
            byte[] resource = null;

            // Fetch resource from redisDB location


            return resource;
        }

        #endregion
    }
}
