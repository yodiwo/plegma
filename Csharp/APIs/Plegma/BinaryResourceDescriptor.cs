using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Plegma
{
    #region Top-level classes and types

    /// <summary>
    /// 
    /// </summary>
    public enum BinaryResourceContentType : byte
    {
        Undefined = 0,
        Data = 1,
        Text = 2,
        Image = 3,
        Audio = 4,
        Video = 5,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BinaryResourceLocationType : byte
    {
        Undefined = 0,
        Http = 1,
        RedisDB = 2,
    }

    /// <summary>
    /// 
    /// </summary>
    public class BinaryResourceDescriptor
    {
        // Metadata
        public string FriendlyName;
        public string FriendlyDescription;
        public int Size;
        public BinaryResourceContentType ContentType;
        public BinaryResourceLocationType LocationType;

        // Descriptors
        public object ContentDescriptor;
        public object LocationDescriptor;


        #region Constructors

        public BinaryResourceDescriptor() { }

        public BinaryResourceDescriptor(string name, 
                                        BinaryResourceContentType contentType, 
                                        BinaryResourceLocationType locationType,
                                        object contentDescriptor,
                                        object locationDescriptor,
                                        int size = 0,
                                        string description = "")
        {
            this.FriendlyName = name;
            this.FriendlyDescription = description;
            this.Size = size;
            this.ContentType = contentType;
            this.LocationType = locationType;
            this.ContentDescriptor = contentDescriptor;
            this.LocationDescriptor = locationDescriptor;
        }

        #endregion
    }

    #endregion

    #region Binary resource location descriptors

    public enum RestServiceType : byte
    {
        Undefined = 0,
        Dropbox = 1,
        Pastebin = 2,
        GoogleDrive = 3,
    }

    public class HttpLocationDescriptor
    {
        public string Uri;
        public RestServiceType RestServiceType;
    }

    public class RedisDBLocationDescriptor
    {
        public string ConnectionAddress;
        public string DatabaseName;
    }

    #endregion

    #region Binary resource content descriptors
    
    public class DataContentDescriptor
    {

    }

    public class TextContentDescriptor : DataContentDescriptor
    {

    }

    public enum ImageFileFormat : byte
    {
        Undefined = 0,
        PNG = 1,
        TIFF = 2,
        GIF = 3,
        BMP = 4,
        SVG = 5
    }

    public enum ImageType : byte
    {
        Raster = 0,
        Vector = 1,
    }

    public class ImageContentDescriptor : DataContentDescriptor
    {
        public ImageType Type;
        public ImageFileFormat Format;

        public int PixelSizeX;
        public int PixelSizeY;
        public int ColorDepth;
    }

    public class AudioContentDescriptor : DataContentDescriptor
    {

    }

    public class VideoContentDescriptor : DataContentDescriptor
    {

    }

    #endregion
}
