package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 11-May-16.
 */

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;

/**
 * Share Action Response class. Each <see cref="ShareThingsRsp"/> message contains the same number of these as in
 * the <see cref="ShareThingsReq"/> message it is responding to, and each <see cref="ShareActionRsp.Id"/> corresponds to a <see cref="ShareActionReq.Id"/>
 */
public class ShareActionRsp {
    /**
     * Id of request that this response refers to
     */
    public int Id;

    /**
     *  Marks whether the referenced (by <see cref="Id"/> and <see cref="ThingKeysToOthers"/> request was successful of not. A value of <see langword="false"/>
     *  means the request got handled but failed due to the reason provided in <see cref="Message"/>
     */
    public boolean IsSuccessful;

    /**
     * If true, the respective request was not completed because of an action pending on the receiver's part. In such a case <see cref="PendingToken"/>
     * will be set to a value that will be provided as reference in a future <see cref="ShareNotifyMsg"/>
     */
    public boolean IsPending;

    /**
     *   A token that can be used to match a currently pending operation to a future <see cref="ShareNotifyMsg"/>
     */
    public String PendingToken;

    /**
     * Contains Things shared to other users by the requesting user. Used only when <see cref="ShareActionReq.Type"/> is <see cref="eShareActionType.AskAll"/>
     */
    public HashMap<String, ArrayList<String>> ThingKeysToOthers;

    /**
     * Contains Things shared to the requesting user by other users. Used only when <see cref="ShareActionReq.Type"/> is <see cref="eShareActionType.AskAll"/>
     */
    public HashMap<String, ArrayList<String>> ThingKeysByOthers;

    /**
     *  (refers to <see cref="eShareActionType.Ask"/> requests only) Array of Users that referenced ThingKey is currently shared with. May be empty.
     *  <para>Is left empty for <see cref="eShareActionType.Share"/> and <see cref="eShareActionType.Unshare"/> requests where the request outcome
     *  is derived from the <see cref="IsSuccessful"/> and <see cref="Message"/> fields</para>
     */
    public String[] Users;

    /**
     * (Optional) text message offering feedback on what went wrong
     */
    public String Message;

    public ShareActionRsp(){}

    public ShareActionRsp(int Id, boolean IsSuccessful, boolean IsPending, String PendingToken, HashMap<String, ArrayList<String>> ThingKeysToOthers, HashMap<String, ArrayList<String>> ThingKeysByOthers, String[] Users){
        this.Id = Id;
        this.IsSuccessful = IsSuccessful;
        this.IsPending = IsPending;
        this.PendingToken = PendingToken;
        this.ThingKeysByOthers = ThingKeysByOthers;
        this.ThingKeysToOthers = ThingKeysToOthers;
        this.Users = Users;
    }

    public ShareActionRsp(int Id, boolean IsSuccessful,  boolean IsPending, String PendingToken, HashMap<String, ArrayList<String>> ThingKeysToOthers, HashMap<String, ArrayList<String>> ThingKeysByOthers, String[] Users, String Message){
        this.Id = Id;
        this.IsSuccessful = IsSuccessful;
        this.IsPending = IsPending;
        this.PendingToken = PendingToken;
        this.ThingKeysByOthers = ThingKeysByOthers;
        this.ThingKeysToOthers = ThingKeysToOthers;
        this.Users = Users;
        this.Message = Message;
    }

    @Override
    public String toString()
    {
        return "ID: " + Id +
                " / Success: " + IsSuccessful +
                " / Users: " + Arrays.toString(Users);
    }
}
