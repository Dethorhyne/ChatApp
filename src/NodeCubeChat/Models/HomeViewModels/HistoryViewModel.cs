using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.HomeViewModels
{
    public class HistoryViewModel
    {
        public string RoomName { get; set; }
        public List<ChatMessageTemplate> Messages { get; set; }
    }
}
