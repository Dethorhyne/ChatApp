using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.AdminViewModels
{
    public class SurveyInformationViewModel
    {
        public Survey Survey { get; set; }
        public List<SurveyQuestion> Questions { get; set; }
    }
}
