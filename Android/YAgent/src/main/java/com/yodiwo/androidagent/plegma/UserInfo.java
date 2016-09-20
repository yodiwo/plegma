package com.yodiwo.androidagent.plegma;

/**
 * Created by vaskanas on 26-May-16.
 */
public class UserInfo {

    public String UserToken;

    public String Username;

    public String Name;

    public String Email;

    public String Avatar;

    public Boolean LoggedIn;

    public UserInfo(){}

    public UserInfo(String usertoken, String username, String name, String email, String avatar, Boolean loggedin){
        this.Username = username;
        this.UserToken = usertoken;
        this.Name = name;
        this.Email = email;
        this.Avatar = avatar;
        this.LoggedIn = loggedin;
    }
    @Override
    public String toString(){
        return UserToken + ": " + Name + ", " + Email;
    }
}
