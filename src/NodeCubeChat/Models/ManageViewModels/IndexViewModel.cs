using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NodeCubeChat.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        
        public string EMail { get; set; }

        public string DisplayName { get; set; }

        public IList<UserLoginInfo> Logins { get; set; }

        public bool BrowserRemembered { get; set; }
        public string ProfilePicture { get; set; }
    }
}
