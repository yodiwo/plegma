package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:47 &#956;&#956;.
 */

/**
 * Base class that describes a group of Thing Models Yodiwo.API.Plegma.NodeModelType
 */
public class NodeThingType {
    /**
     * The unique Type Name which identifies this group (must neither be null, nor empty)
     */
    public String Type;
    /**
     * Specifies whether model(s) of this group can automatically be discovered
     */
    public Boolean Searchable;
    /**
     * Human readable description for this group
     */
    public String Description;
    /**
     * Describes the model(s) of this groupYodiwo.API.Plegma.NodeModelType
     */
    public NodeModelType[] Model;

    public NodeThingType() {
    }

    public NodeThingType(String Type, Boolean Searchable, String Description, NodeModelType[] Model) {
        this.Type = Type;
        this.Searchable = Searchable;
        this.Description = Description;
        this.Model = Model;

    }

}
