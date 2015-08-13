package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 3/8/2015 10:26:08 &#956;&#956;.
 */

/**
 * Globally unique identifier of a User
 */
public class UserKey {

    public String UserID;

    public Boolean IsValid;

    public Boolean IsInvalid;

    public UserKey() {
    }

    public UserKey(String UserID, Boolean IsValid, Boolean IsInvalid) {
        this.UserID = UserID;
        this.IsValid = IsValid;
        this.IsInvalid = IsInvalid;

    }

}
