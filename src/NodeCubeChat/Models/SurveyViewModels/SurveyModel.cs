using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.SurveyViewModels
{
    public class SurveyModel
    {
        public int SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SurveyQuestionModel> Questions { get; set; }

    }

    public class SurveyQuestionModel
    {

        public int QuestionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int? SelectAnswer { get; set; }
        public List<SurveyOptionModel> Options { get; set; }
    }
    public class SurveyOptionModel
    {

        public int? OptionId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
    }
}
