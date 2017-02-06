using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    #region Top-level classes and types

    /// <summary>
    /// Content type of a binary resource.
    /// </summary>
    public enum eBinaryResourceContentType : byte
    {
        Undefined = 0,
        Data = 1,
        Text = 2,
        Image = 3,
        Audio = 4,
        Video = 5,
    }

    /// <summary>
    /// Location type of a binary resource.
    /// </summary>
    public enum eBinaryResourceLocationType : byte
    {
        Undefined = 0,
        Http = 1,
        RedisDB = 2,
    }

    /// <summary>
    /// Descriptor of a binary resource.
    /// </summary>
    public class BinaryResourceDescriptor
    {
        public string Key;  //type: BinaryResourceDescriptorKey

        // Metadata
        public string FriendlyName;
        public string FriendlyDescription;
        public long Size;

        public eBinaryResourceContentType ContentType;
        public eBinaryResourceLocationType LocationType;

        /// <summary>Json-encoded Content descriptor (a class that derives from <see cref="ContentDescriptor"/>)</summary>
        public string ContentDescriptorJson;
        /// <summary>Json-encoded Location descriptor (a class that derives from <see cref="LocationDescriptor"/>)</summary>
        public string LocationDescriptorJson;

        [JsonIgnore]
        private ContentDescriptor _ContentDescriptor;
        [JsonIgnore]
        private LocationDescriptor _LocationDescriptor;

        [DoNotStoreInDB]
        public object ContentDescriptor
        {
            get
            {
                if (_ContentDescriptor == null)
                {
                    try
                    {
                        object convertedDescriptor;
                        var type = BinaryResourceHelper.EnumToContentType.TryGetOrDefault(ContentType);
                        if (type != null && !string.IsNullOrEmpty(ContentDescriptorJson))
                        {
                            convertedDescriptor = JsonConvert.DeserializeObject(ContentDescriptorJson.HtmlDecode(), type);
                            if (convertedDescriptor is ContentDescriptor)
                                _ContentDescriptor = convertedDescriptor as ContentDescriptor;
                        }
                    }
                    catch { }
                }
                return _ContentDescriptor;
            }
            set { _ContentDescriptor = null; ContentDescriptorJson = value.ToJSON(); }
        }

        [DoNotStoreInDB]
        public object LocationDescriptor
        {
            get
            {
                if (_LocationDescriptor == null)
                {
                    try
                    {
                        object convertedDescriptor;
                        var type = BinaryResourceHelper.EnumToLocationType.TryGetOrDefault(LocationType);
                        if (type != null && !string.IsNullOrEmpty(LocationDescriptorJson))
                        {
                            convertedDescriptor = JsonConvert.DeserializeObject(LocationDescriptorJson.HtmlDecode(), type);
                            if (convertedDescriptor is LocationDescriptor)
                                _LocationDescriptor = convertedDescriptor as LocationDescriptor;
                        }
                    }
                    catch { }
                }
                return _LocationDescriptor;
            }
            set { _LocationDescriptor = null; LocationDescriptorJson = value.ToJSON(); }
        }


        #region Constructors

        public BinaryResourceDescriptor() { }

        public BinaryResourceDescriptor(string name,
                                        eBinaryResourceContentType contentType,
                                        eBinaryResourceLocationType locationType,
                                        ContentDescriptor contentDescriptor,
                                        LocationDescriptor locationDescriptor,
                                        int size = 0,
                                        string description = "")
        {
            this.FriendlyName = name;
            this.FriendlyDescription = description;
            this.Size = size;
            this.ContentType = contentType;
            this.LocationType = locationType;
            this.ContentDescriptorJson = contentDescriptor.ToJSON();
            this.LocationDescriptorJson = locationDescriptor.ToJSON();
        }

        #endregion

        #region Functions
        /// <summary>
        /// Return true if this binary resource is image, elsewhere return false
        /// </summary>
        /// <returns></returns>
        public bool IsImage() { return ContentType == eBinaryResourceContentType.Image; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brd"></param>
        public void Update(BinaryResourceDescriptor brd) { }

        #endregion
    }


    #endregion

    #region Binary resource location descriptors

    /// <summary>
    /// Descriptor of a binary resource location.
    /// </summary>
    public class LocationDescriptor
    {
    }

    /// <summary>
    /// Type of a REST service
    /// </summary>
    public enum RestServiceType : byte
    {
        Undefined = 0,
        Dropbox = 1,
        Pastebin = 2,
        GoogleDrive = 3,
        Yodiwo = 4,
    }

    /// <summary>
    /// Descriptor of HTTP Location
    /// </summary>
    public class HttpLocationDescriptor : LocationDescriptor
    {
        public string Uri;
        public RestServiceType RestServiceType;
    }


    /// <summary>
    /// Descriptor of RedidDB Location
    /// </summary>
    public class RedisDBLocationDescriptor : LocationDescriptor
    {
        public string ConnectionAddress;
        public string DatabaseName;
    }

    #endregion

    #region Binary resource content descriptors

    /// <summary>
    /// 
    /// </summary>
    public class ContentDescriptor
    {
        public object Data;
    }

    public class DataContentDescriptor : ContentDescriptor { }
    public class TextContentDescriptor : ContentDescriptor { }

    public enum ImageFileFormat : byte
    {
        Undefined = 0,
        PNG = 1,
        TIFF = 2,
        GIF = 3,
        BMP = 4,
        SVG = 5,
    }

    public enum ImageType : byte
    {
        Raster = 0,
        Vector = 1,
    }


    public class ImageContentDescriptor : ContentDescriptor
    {
        public ImageType Type;
        public ImageFileFormat Format;

        public int PixelSizeX;
        public int PixelSizeY;
        public int ColorDepth;

        public bool IsRaster() { return !(Format == ImageFileFormat.SVG); }
        public bool IsSVG() { return !IsRaster(); }

        /// <summary>
        /// Return image file format from URI
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        public static ImageFileFormat ImageFileFormatFromURI(string Uri)
        {
            // TODO: find extension
            return ImageFileFormat.Undefined;
        }

        /// <summary>
        /// Return image type from URI
        /// </summary>
        /// <param name="Uri"></param>
        /// <returns></returns>
        public static ImageType ImageTypeFromURI(string Uri)
        {
            if (ImageFileFormatFromURI(Uri) == ImageFileFormat.SVG)
                return ImageType.Vector;
            else
                return ImageType.Raster;
        }
    }

    public class AudioContentDescriptor : ContentDescriptor { }

    public class VideoContentDescriptor : ContentDescriptor { }

    #endregion

    public static class BinaryResourceHelper
    {
        internal static Dictionary<eBinaryResourceContentType, Type> EnumToContentType = new Dictionary<eBinaryResourceContentType, Type>()
        {
            { eBinaryResourceContentType.Undefined, null },
            { eBinaryResourceContentType.Image, typeof(ImageContentDescriptor) },
            { eBinaryResourceContentType.Audio, typeof(AudioContentDescriptor) },
            { eBinaryResourceContentType.Video, typeof(VideoContentDescriptor) },
            { eBinaryResourceContentType.Text, typeof(TextContentDescriptor) },
            { eBinaryResourceContentType.Data, typeof(DataContentDescriptor) },
        };

        internal static Dictionary<eBinaryResourceLocationType, Type> EnumToLocationType = new Dictionary<eBinaryResourceLocationType, Type>()
        {
            { eBinaryResourceLocationType.Undefined, null },
            { eBinaryResourceLocationType.Http, typeof(HttpLocationDescriptor) },
            { eBinaryResourceLocationType.RedisDB, typeof(RedisDBLocationDescriptor) },
        };
    }
}
