using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yodiwo.API.Warlock
{
    public class UserOptions
    {
        public eSharingOpts SharingOpts;
        public eFriendAddOpts FriendOpts;

        public enum eSharingOpts
        {
            Invalid = 0,
            AutoAcceptFromAll = 1,
            AutoAcceptFromFriends = 2,
            AlwaysAsk = 3,
            AlwaysDeny = 4,
        }

        public enum eFriendAddOpts
        {
            Invalid = 0,
            AutoAcceptFromAll = 1,
            AlwaysAsk = 2,
            AlwaysDeny = 3,
        }
    }
    public class UserInfoResponse
    {
        public string Username;
        public string Email;
        public string PublicToken;
        public string PrivateToken;
        public string Avatar;
        public bool LoggedIn;
        public int AccountLevel;
    }
}
