using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models
{
    public class ChatMessageTemplate
    {
        public string Message { get; set; }
        public string ProfilePicture{ get; set; }
        public DateTime Timestamp { get; set; }
        public string DisplayName { get; set; }

    }
}
