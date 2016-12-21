package com.yodiwo.plegma;

public class BinaryResourceDescriptor {

    // Metadata
    public String FriendlyName;
    public String FriendlyDescription;
    public int Size;
    public int ContentType;
    public int LocationType;

    // Descriptors
    public Object ContentDescriptor;
    public Object LocationDescriptor;

    public BinaryResourceDescriptor(String friendlyName,
                                    String friendlyDescription,
                                    int size,
                                    int contentType,
                                    int locationType,
                                    Object contentDescriptor,
                                    Object locationDescriptor) {

        this.FriendlyName = friendlyName;
        this.FriendlyDescription = friendlyDescription;
        this.Size = size;
        this.ContentType = contentType;
        this.LocationType = locationType;
        this.ContentDescriptor = contentDescriptor;
        this.LocationDescriptor = locationDescriptor;
    }
}
