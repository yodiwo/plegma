using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class UserFacebookDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string Jid;
        public string Email;
        public string Name;
        public bool PendingInvitation;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserFacebookDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserFacebookDescriptor(string Jid, string Email, string Name)
        {
            this.Jid = Jid;
            this.Email = Email;
            this.Name = Name;
        }

        #endregion
    }

    public class UserHangoutsDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string Jid;
        public string Email;
        public string Name;
        public bool PendingInvitation;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserHangoutsDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserHangoutsDescriptor(string Jid, string Email, string Name)
        {
            this.Jid = Jid;
            this.Email = Email;
            this.Name = Name;
        }

        #endregion
    }

    public class UserSipDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string SipUsername;
        public int AssignedListenerBot;
        public int AssignedOutgoingBot;
        public bool IOPLync;
        public int AssignedB2BLyncBot;
        public bool PendingInvitation;
        public string Pin;
        public bool IsActivated;
        public string Secret;
        public string Host;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserSipDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserSipDescriptor(string SipUsername, int AssignedListenerBot, int AssignedOutgoingBot, bool IOPLync, int AssignedB2BLyncBot)
        {
            this.SipUsername = SipUsername;
            this.AssignedListenerBot = AssignedListenerBot;
            this.AssignedOutgoingBot = AssignedOutgoingBot;
            this.IOPLync = IOPLync;
            this.AssignedB2BLyncBot = AssignedB2BLyncBot;
        }

        #endregion
    }

    public class UserSkypeDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string MsMail;
        public int AssignedIMBot;
        public int AssignedAVBot;
        public int AssignedSipBot;
        public int AssignedB2BLyncBot;
        public bool PendingInvitation;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserSkypeDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserSkypeDescriptor(string MsMail)
        {
            this.MsMail = MsMail;
        }

        #endregion
    }

    public class UserLyncDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string LyncUsername;
        public int AssignedListenerBot;
        public int AssignedOutgoingBot;
        public int AssignedIMBot;
        public bool IOPSip;
        public int AssignedSipBot;
        public int AssignedB2BLyncBot;
        public bool PendingInvitation;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserLyncDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserLyncDescriptor(string LyncUsername, int AssignedListenerBot, int AssignedOutgoingBot, bool IOPSip, int AssignedSipBot, int AssignedB2BLyncBot)
        {
            this.LyncUsername = LyncUsername;
            this.AssignedListenerBot = AssignedListenerBot;
            this.AssignedOutgoingBot = AssignedOutgoingBot;
            this.IOPSip = IOPSip;
            this.AssignedSipBot = AssignedSipBot;
            this.AssignedB2BLyncBot = AssignedB2BLyncBot;
        }

        #endregion

    }

    public class UserGmailDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string email;
        public TokenResponse tokenresponse;

        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserGmailDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserGmailDescriptor(string email, TokenResponse tokenresp)
        {
            this.email = email;
            this.tokenresponse = tokenresp;
        }

        #endregion
    }

    public class TokenResponse
    {
        public string access_token;
        public string token_type;
        public string refresh_token;
        public int expires_in;
    }

    public class UserIrcDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public string Nickname;
        public List<string> SubscribedChannels;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserIrcDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserIrcDescriptor(string Nickname, List<string> SubscribedChannels)
        {
            this.SubscribedChannels = SubscribedChannels;
            this.Nickname = Nickname;
        }

        #endregion
    }

    public class UserSocialAccountsDescriptor
    {

        #region Variables
        //-------------------------------------------------------------------------------------------------------------------------
        public List<UserFacebookDescriptor> Facebook;
        public List<UserHangoutsDescriptor> Hangout;
        public List<UserIrcDescriptor> Irc;
        public List<UserSipDescriptor> Sip;
        public List<UserLyncDescriptor> Lync;
        public List<UserSkypeDescriptor> Skype;
        public List<UserGmailDescriptor> Gmail;
        public List<RestServiceBlockDescriptor> RestService;
        //-------------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Constructors
        //-------------------------------------------------------------------------------------------------------------------------
        public UserSocialAccountsDescriptor() { }
        //-------------------------------------------------------------------------------------------------------------------------
        public UserSocialAccountsDescriptor(List<UserFacebookDescriptor> Facebook, List<UserHangoutsDescriptor> Hangout, List<UserIrcDescriptor> Irc, List<UserSipDescriptor> Sip, List<UserLyncDescriptor> Lync, List<UserSkypeDescriptor> Skype, List<RestServiceBlockDescriptor> RestService, List<UserGmailDescriptor> Gmail)
        {
            this.Facebook = Facebook;
            this.Hangout = Hangout;
            this.Irc = Irc;
            this.Sip = Sip;
            this.Lync = Lync;
            this.Skype = Skype;
            this.Gmail = Gmail;
            this.RestService = RestService;
        }

        #endregion
    }
}
