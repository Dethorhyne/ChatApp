using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NodeCubeChat.Models;
using NodeCubeChat.Models.AdminViewModels;
using NodeCubeChat.Models.SurveyViewModels;

namespace NodeCubeChat.Controllers
{
    public class SurveyController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private ChatContext _context;

        public SurveyController(
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ChatContext context)
        {
            _signInManager = signInManager;
            _UserManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Survey> EligableSurvey = new List<Survey>();

            var currentUser = await _UserManager.GetUserAsync(User);
            int UserAge = DateTime.Now.Year - currentUser.DateOfBirth.Year;

            int rating = 0;

            rating += _context.UserRatings.Where(item => item.RatedUser == currentUser.UserName && item.Score == true).Count();
            rating -= _context.UserRatings.Where(item => item.RatedUser == currentUser.UserName && item.Score == false).Count();

            bool UserPositiveProfile = (rating >= 0) ? true : false;

            var FilteredSurvey = (from item in _context.Surveys where (item.RequiredMinAge <= UserAge) && (item.RequiredMaxAge >= UserAge) && (!item.RequiredPositiveUserRating || UserPositiveProfile) select item).ToList();
            
            foreach(Survey survey in FilteredSurvey)
            {
                switch(survey.RequiredGender)
                {
                    case 0:
                        EligableSurvey.Add(survey);
                        break;
                    case 1:
                        if(currentUser.Gender == 1)
                            EligableSurvey.Add(survey);
                        break;
                    case 2:
                        if (currentUser.Gender == 2)
                            EligableSurvey.Add(survey);
                        break;

                        
                }
            }

            //Filter out completed and on-cooldown surveys
            var InvalidSurveys = (from item in _context.SurveyStatistics
                                      where (item.ParticipantUserName == User.Identity.Name && item.FinishedSurvey == true) || (item.ParticipantUserName == User.Identity.Name && DateTime.Now < item.CoolDownPeriod)
                                      select item.SurveyId).ToList<int>();

            int surveysfiltered = 0;

            for(int i=0; i<EligableSurvey.Count;i++)
            {
                if(InvalidSurveys.Any(x => x == EligableSurvey[i-surveysfiltered].SurveyId))
                {
                    EligableSurvey.Remove(EligableSurvey[i-surveysfiltered]);
                    surveysfiltered++;
                }
            }
            

            return View(EligableSurvey);
        }

        [HttpGet]
        public IActionResult Execute(int Id=0)
        {
            Survey SelectedSurvey = (from item in _context.Surveys where item.SurveyId == Id select item).SingleOrDefault();

            if (SelectedSurvey == null)
                return RedirectToAction("Home", "Index");

            var stat = new SurveyStatistic
            {
                SurveyId = Id,
                TimeStamp = DateTime.Now,
                CoolDownPeriod = DateTime.Now.AddDays(7),
                FinishedSurvey = false,
                ParticipantUserName = User.Identity.Name,
            };

            _context.SurveyStatistics.Add(stat);

            _context.SaveChanges();

            List<SurveyQuestionModel> surveyquestions = new List<SurveyQuestionModel>();
            List<SurveyOptionModel> questionoptions = new List<SurveyOptionModel>();


            List<SurveyQuestion> SelectedSurveyQuestions = (from item in _context.SurveyQuestions where item.SurveyId == SelectedSurvey.SurveyId select item).ToList();

            foreach(SurveyQuestion question in SelectedSurveyQuestions)
            {
                questionoptions = new List<SurveyOptionModel>();

                if (question.Type=="Text")
                {
                    var text = new SurveyOptionModel
                    {
                        OptionId = null,
                        Text = "",
                        Title = "",
                        Checked = false
                    };

                    questionoptions.Add(text);

                    var textqmodel = new SurveyQuestionModel
                    {
                        Title = question.Title,
                        Description = question.Description,
                        QuestionId = question.SurveyQuestionId,
                        Options = questionoptions,
                        Type = question.Type
                    };

                    surveyquestions.Add(textqmodel);
                    continue;
                }

                List<SurveyOption> QuestionOptions = (from item in _context.SurveyOptions where item.SurveyQuestionId == question.SurveyQuestionId select item).ToList();

                foreach(SurveyOption option in QuestionOptions)
                {

                    var multipleoption = new SurveyOptionModel
                    {
                        OptionId = option.SurveyOptionId,
                        Text = null,
                        Title = option.Title,
                        Checked = false
                    };
                    questionoptions.Add(multipleoption);
                }

                var multipleqmodel = new SurveyQuestionModel
                {
                    Title = question.Title,
                    Description = question.Description,
                    QuestionId = question.SurveyQuestionId,
                    Options = questionoptions,
                    Type = question.Type
                };

                surveyquestions.Add(multipleqmodel);
            }

            var model = new SurveyModel
            {
                Title = SelectedSurvey.Title,
                Description = SelectedSurvey.Description,
                SurveyId = SelectedSurvey.SurveyId,
                Questions = surveyquestions
            };

            return View(model);
        }
        

        [HttpPost]
        public async Task<IActionResult> Execute(SurveyModel model)
        {
            var user = await _UserManager.GetUserAsync(User);

            var stat = (from item in _context.SurveyStatistics
                        where item.SurveyId == model.SurveyId && item.ParticipantUserName == User.Identity.Name
                        select item).SingleOrDefault();



            return View("ThankYou");
        }
        public async Task<ActionResult> GetUserInfo()
        {
            var user = await _UserManager.GetUserAsync(User);
            return Json(user);
        }

        public async Task<Survey> GetAnEligableSurvey()
        {
            List<Survey> EligableSurveys = new List<Survey>();

            var currentUser = await _UserManager.GetUserAsync(User);
            int UserAge = DateTime.Now.Year - currentUser.DateOfBirth.Year;

            int rating = 0;

            rating += _context.UserRatings.Where(item => item.RatedUser == currentUser.UserName && item.Score == true).Count();
            rating -= _context.UserRatings.Where(item => item.RatedUser == currentUser.UserName && item.Score == false).Count();

            bool UserPositiveProfile = (rating >= 0) ? true : false;

            var FilteredSurvey = (from item in _context.Surveys where (item.RequiredMinAge <= UserAge) && (item.RequiredMaxAge >= UserAge) && (!item.RequiredPositiveUserRating || UserPositiveProfile) select item).ToList();

            foreach (Survey survey in FilteredSurvey)
            {
                switch (survey.RequiredGender)
                {
                    case 0:
                        EligableSurveys.Add(survey);
                        break;
                    case 1:
                        if (currentUser.Gender == 1)
                        EligableSurveys.Add(survey);
                        break;
                    case 2:
                        if (currentUser.Gender == 2)
                        EligableSurveys.Add(survey);
                        break;


                }
            }

            if(EligableSurveys.Count == 0)
            {
                return null;
            }

            return EligableSurveys[new Random().Next(0, EligableSurveys.Count - 1)];
        }

    }
}