package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 11-May-16.
 */
public class eShareActionType {
    /**
     * Explicit share request, must include specific ThingKey
     * and specific User Id to share to
     */
    public static final int Share = 0;
    /**
     * Explicit unshare request, must include specific ThingKey and either a
     * specific User Id to unshare from, or a broadcast UserKey in order
     * to unshare from all
     */
    public static final int Unshare = 1;
    /**
     *  Get com.yodiwo.nebit.sharing info for specified ThingKey. Specific ThingKey must be provided
     *  (no broadcast keys allowed). If specific TargetUser is provided
     *  the response will inform whether Thing is shared with specific User
     *  If broadcast UserKey is provided and the requesting User is the
     *  Thing's Owner, then the response will provide an array of users the Thing
     *  is currently shared to.
     *  A broadcast UserKey cannot be used to request the list of users for a Thing
     *  that doesn't belong to the requester.
     */
    public static final int Ask = 2;
    /**
     * Used to receive two Things lists:
     * - Things shared to the requesting user by other users
     * - Things shared to other users by the requesting user
     */
    public static final int AskAll = 3;
}
