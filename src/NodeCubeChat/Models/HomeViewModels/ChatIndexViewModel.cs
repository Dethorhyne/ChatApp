using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.HomeViewModels
{
    public class ChatIndexViewModel
    {
        public string RoomName { get; set; }
        public string ProfilePicture { get; set; }
        public string DisplayName { get; set; }
        public List<string> Rooms { get; set; }
        public bool IsCurrentUserAdmin { get; set; }

        public List<ChatMessageTemplate> LastMessages { get; set; }
    }
}
