using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.AdminViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Company> Companies { get; set; }
        public List<List<string>> CompanyAdPackages { get; set; }
        public List<AdPackage> AdPackages { get; set; }
        public List<Survey> Surveys { get; set; }
        public List<ChatRoom> ChatRooms { get; set; }
    }
}
