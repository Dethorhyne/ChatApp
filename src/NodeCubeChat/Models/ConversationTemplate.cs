using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models
{
    public class ConversationTemplate
    {
        public int ConversationId { get; set; }
        public string PartnerDisplayName { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public DateTime InitiationDate { get; set; }
        public string LastMessageText { get; set; }
        public string PartnerProfilePicture { get; set; }
        public bool Approved { get; set; }

    }
}
