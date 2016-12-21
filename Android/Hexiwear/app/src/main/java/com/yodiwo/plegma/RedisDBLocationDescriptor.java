package com.yodiwo.plegma;

/**
 * Created by r00tb00t on 12/17/15.
 */
public class RedisDBLocationDescriptor
{
    public String ConnectionAddress;
    public String DatabaseName;

    public RedisDBLocationDescriptor(String connectionAddress, String databaseName) {
        this.ConnectionAddress = connectionAddress;
        this.DatabaseName = databaseName;
    }
}
