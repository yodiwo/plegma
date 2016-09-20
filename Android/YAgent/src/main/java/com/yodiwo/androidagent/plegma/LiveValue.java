package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 27-Jul-16.
 */
public class LiveValue {
    public Object Value;
    public int Index;
    public boolean IsConnected;
    public boolean IsDirty;
    public boolean IsInput;
    public boolean IsOutput;
    public boolean IsTouched;
    public boolean IsPort;

    public String IoName;
    public String BlockKey;
    public String PortKey;
    public String Extra;
    public int RevNum;
    public long Timestamp;

    public LiveValue(){}

    public LiveValue(Object value,
                         int index,
                         boolean isConnected, boolean isDirty, boolean isInput,
                         boolean isOutput, boolean isTouched, boolean isPort,
                         String ioName, String blockKey, String portKey, String extra,
                         int revNum,
                         long timestamp) {
        this.BlockKey = blockKey;
        this.Extra = extra;
        this.Index = index;
        this.Value = value;
        this.IsConnected = isConnected;
        this.IsDirty = isDirty;
        this.IoName = ioName;
        this.IsInput = isInput;
        this.IsOutput = isOutput;
        this.IsTouched = isTouched;
        this.IsPort = isPort;
        this.RevNum = revNum;
        this.Timestamp = timestamp;
    }
}
