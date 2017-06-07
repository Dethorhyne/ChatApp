using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.PrivateViewModels
{
    public class PrivateViewModel
    {
        public string DisplayName { get; set; }
        public string ProfilePicture { get; set; }
        public int ConversationId { get; set; }
        public List<ConversationTemplate> Conversation { get; set; }
        public List<ChatMessageTemplate> Messages { get; set; }

        public bool ConversationApproved { get; set; }
        public bool IsCurrentUserInitiator { get; set; }
        public string PartnerDisplayName { get; set; }
        public bool ConversationActive { get; set; }
    }
}
