package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 17-May-16.
 */
public class ShareNotification {
        /**
         * Type of action that is being notified; see <see cref="eShareActionType"/>
         */
        public int Type;

        /**
         * If not empty, it holds a token that can be used to match a currently pending operation to a future <see cref="ShareNotifyMsg"/>
         */
        public String PendingToken;

        /**
         * If <see cref="PendingToken"/> not empty, this holds the outcome of previously-pending share action
         */
        public boolean IsSuccessful;

        /**
         * UserToken of User that requested the action this is referring to
         */
        public String RequestingUserToken;

        /**
         * UserToken of Target User the action is referring to
         */
        public String TargetUserToken;

        /**
         * Specific ThingKey of Thing that this action refers to
         */
        public String ThingKey;
}
