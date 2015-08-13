package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:17 &#956;&#956;.
 */

public class StateDescription {

    public double Minimum;

    public double Maximum;

    public double Step;

    public String Pattern;

    public Boolean ReadOnly;

    public StateDescription() {
    }

    public StateDescription(double Minimum, double Maximum, double Step, String Pattern, Boolean ReadOnly) {
        this.Minimum = Minimum;
        this.Maximum = Maximum;
        this.Step = Step;
        this.Pattern = Pattern;
        this.ReadOnly = ReadOnly;

    }

}
