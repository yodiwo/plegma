package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 17/8/2015 3:43:41 &#956;&#956;.
 */

/**
 * Collection of instructions ("hints") for how to present this thing in the Cyan UI
 */
public class ThingUIHints {
    /**
     * URI of icon to show in Cyan for this thing
     */
    public String IconURI;
    /**
     * Description of Thing to show in Cyan (tooltip, etc)
     */
    public String Description;

    public ThingUIHints() {
    }

    public ThingUIHints(String IconURI, String Description) {
        this.IconURI = IconURI;
        this.Description = Description;

    }

}
