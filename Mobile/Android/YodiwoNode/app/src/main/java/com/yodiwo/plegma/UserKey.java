package com.yodiwo.plegma;

/**
 * Created by ApiGenerator Tool (Java) on 28/08/2015 18:34:39.
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
