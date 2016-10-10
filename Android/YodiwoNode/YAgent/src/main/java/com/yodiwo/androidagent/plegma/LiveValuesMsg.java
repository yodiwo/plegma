package com.yodiwo.androidagent.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:35:00.
 */

public class LiveValuesMsg extends ApiMsg {

    public LiveValue[] LiveValues;

    public LiveValuesMsg() {
    }

    public LiveValuesMsg(int SeqNo, LiveValue[] liveValues) {
        this.SeqNo = SeqNo;
        this.LiveValues = liveValues;
    }
}
