package com.yodiwo.androidagent.plegma;

public class BinaryResourceDescriptor {

    public BinaryResourceDescriptorKey BinaryResourceDescriptorKey;

    // Metadata
    public String FriendlyName;
    public String FriendlyDescription;
    public int Size;

    public int ContentType;
    public int LocationType;

    public String ContentDescriptorJson;
    public String LocationDescriptorJson;

    public BinaryResourceDescriptor(BinaryResourceDescriptorKey BinaryResourceDescriptorKey,
                                    String friendlyName,
                                    String friendlyDescription,
                                    int size,
                                    int contentType,
                                    int locationType,
                                    String contentDescriptorJson,
                                    String locationDescriptorJson) {

        this.FriendlyName = friendlyName;
        this.FriendlyDescription = friendlyDescription;
        this.Size = size;
        this.ContentType = contentType;
        this.LocationType = locationType;
        this.ContentDescriptorJson = contentDescriptorJson;
        this.LocationDescriptorJson  = locationDescriptorJson;
    }
}
