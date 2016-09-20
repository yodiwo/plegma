package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 11-May-16.
 */

/**
 * Share Action sub-request
 */
public class ShareActionReq {
    /**
     * Id of specific sub-request. Can be ignored if Req consists of single action or requester
     * doesn't require to match requests to responses
     */
    public int Id;

    /**
     * Type of action requested; see <see cref="eShareActionType"/>
     */
    public int Type;

    /**
     * User Token of Target User the action is referring to, as retrieved
     * by <see cref="GetUserTokensReq"/>. A wildcard key may be used to
     * request info on all users for a single ThingKey
     */
    public String TargetUserToken;

    /**
     * Specific ThingKey of Thing that this action refers to.
     * Mandatory for <see cref="eShareActionType.Share"/>, <see cref="eShareActionType.Unshare"/>
     * and <see cref="eShareActionType.Ask"/>.
     * Ignored for <see cref="eShareActionType.AskAll"/>.
     */
    public String ThingKey;

    /**
     *  If the request is used to answer a previously pending request,
     *  this token should be used to reference the initial request
     */
    public String PendingToken;

    public ShareActionReq() {
    }

    public ShareActionReq(int Id, int Type, String TargetUserToken) {
        this.Id = Id;
        this.Type = Type;
        this.TargetUserToken = TargetUserToken;
        this.ThingKey = null;
        this.PendingToken = null;
    }

    public ShareActionReq(int Id, int Type, String TargetUserToken, String ThingKey) {
        this.Id = Id;
        this.Type = Type;
        this.TargetUserToken = TargetUserToken;
        this.ThingKey = ThingKey;
        this.PendingToken = null;
    }

    public ShareActionReq(int Id, int Type, String TargetUserToken, String ThingKey, String PendingToken) {
        this.Id = Id;
        this.Type = Type;
        this.TargetUserToken = TargetUserToken;
        this.ThingKey = ThingKey;
        this.PendingToken = PendingToken;
    }

    @Override
    public String toString() {
        return "ID:" + Id +
                " / Type:" + Type +
                " / ThingKey:" + ThingKey +
                " / Target User:" + TargetUserToken;
    }
}
