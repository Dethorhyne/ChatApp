using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.AdminViewModels
{
    public class SurveyQuestionViewModel
    {
        public SurveyQuestion Question { get; set; }
        public List<SurveyOption> Options { get; set; }
    }
}
