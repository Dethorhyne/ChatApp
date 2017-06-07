using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.AdminViewModels
{
    public class EditCompanyViewModel
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public int Displays { get; set; }
        public List<CheckBoxModel> AdPackages { get; set; }
        public bool paidField;
        public string Paid
        {
            get
            {
                if(paidField)
                    return "Company has paid for all ads";
                return "Company did not pay for all ads";
            }
        }
    }
}
