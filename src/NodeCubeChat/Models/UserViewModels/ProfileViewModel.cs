using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.UserViewModels
{
    public class ProfileViewModel
    {
        public string EMail { get; set; }
        public string DisplayName { get; set; }
        public string City { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Interests { get; set; }
        public bool Self { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public int Rating { get; set; }

        public bool DidCurrentUserRateThisUser { get; set; }

        public bool IsCurrentUserAdmin { get; set; }
        public bool IsUserAdmin { get; set; }
        public List<Image> Images { get; set; }
    }
}
