package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:12 &#956;&#956;.
 */

/**
 * Direction of Port
 */
public enum ioPortDirection {
    /**
     * undefined, should not be used!
     */
    Undefined,
    /**
     * both Input and Output, Port will be used in both Graph Input and Output Things
     */
    InputOutput,
    /**
     * Output only; Port will be used only in Graph Input Things (node->cloud)
     */
    Output,
    /**
     * Input only; Port will be used only in Graph Output Things (cloud->node)
     */
    Input,
}
