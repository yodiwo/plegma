package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 09-Jun-16.
 */


/**
 * Add friend to user. Will reply with <see cref="GenericRsp"/>
  */
public class AddFriendReq extends ApiMsg {
    /**
     *  Key can be valid email or valid user token.
     *  If the target user has Auto-Accept set then the request is immediately successful. Otherwise the request "fails"
     *  and the response message is "Pending". The target user will receive a notification to act on the new request
     *
     *  An empty key will result in a failed request
     */
    public String Key;

    public AddFriendReq(){}

    public AddFriendReq(int SeqNo, String Key){
        this.SeqNo = SeqNo;
        this.Key = Key;
    }
}