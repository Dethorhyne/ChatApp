using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NodeCubeChat.Models;
using NodeCubeChat.Models.AdminViewModels;

namespace NodeCubeChat.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private ChatContext _context;
        private Dictionary<int, AdPackage> AdPackages = new Dictionary<int, AdPackage>();

        public AdminController(
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ChatContext context)
        {
            _signInManager = signInManager;
            _UserManager = userManager;
            _context = context;


            foreach (AdPackage package in _context.AdPackages)
            {
                AdPackages.Add(package.AdPackageId, package);
            }
        }

        public async Task<IActionResult> PromoteToAdmin(string Id = null)
        {
            if (Id != null)
            {
                var user = _UserManager.Users.Where(x => x.DisplayName == Id).Select(x => x).SingleOrDefault();
                if (user != null)
                {
                    user.IsAdmin = true;

                    await _UserManager.UpdateAsync(user);
                }
            }

            return RedirectToAction("Profile", "User", new { Id = Id });
        }

        public IActionResult Index()
        {
            List<List<string>> PackagesForCompanies = new List<List<string>>();
            List<string> PackagesForCompany = new List<string>();


            foreach (Company package in _context.Companies)
            {
                if (package.AdPackageIds != String.Empty)
                {
                    List<string> uniquepackages = package.AdPackageIds.Split(',').ToList();

                    foreach (string Id in uniquepackages)
                    {
                        PackagesForCompany.Add(AdPackages[Convert.ToInt32(Id)].AdPackageName);
                    }
                }
                PackagesForCompanies.Add(PackagesForCompany);
                PackagesForCompany = new List<string>();
            }

            var model = new AdminDashboardViewModel
            {
                Companies = (from item in _context.Companies
                             select item).ToList(),
                CompanyAdPackages = PackagesForCompanies,
                AdPackages = (from item in _context.AdPackages
                              select item).ToList(),
                Surveys = (from item in _context.Surveys
                           select item).ToList(),
                ChatRooms = (from item in _context.ChatRooms
                             select item).ToList()
            };

            return View(model);
        }

        #region Adding surveys
        [HttpGet]
        public IActionResult AddSurvey()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSurvey(Survey model)
        {
            var survey = new Survey
            {
                DateCreated = DateTime.Now,
                Title = model.Title,
                Description = model.Description,
                RequiredGender = model.RequiredGender,
                RequiredMaxAge = model.RequiredMaxAge,
                RequiredMinAge = model.RequiredMinAge,
                RequiredPositiveUserRating = model.RequiredPositiveUserRating,
                Active = false
            };

            _context.Surveys.Add(survey);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        [HttpGet]
        public IActionResult SurveyInformation(int Id = 0)
        {

            var survey = (from item in _context.Surveys where item.SurveyId == Id select item).SingleOrDefault();

            if (survey != null)
            {
                var questions = (from item in _context.SurveyQuestions where item.SurveyId == survey.SurveyId select item).ToList();

                var model = new SurveyInformationViewModel
                {
                    Survey = survey,
                    Questions = questions
                };

                return View(model);
            }

            return RedirectToAction("Index", "Admin");
        }

        #region Survey editing
        [HttpGet]
        public IActionResult EditSurvey(int Id = 0)
        {
            var survey = (from item in _context.Surveys where item.SurveyId == Id select item).SingleOrDefault();



            return View(survey);
        }

        [HttpPost]
        public IActionResult EditSurvey(Survey model)
        {
            var survey = (from item in _context.Surveys where item.SurveyId == model.SurveyId select item).SingleOrDefault();

            survey.Title = model.Title;
            survey.Description = model.Description;
            survey.RequiredGender = model.RequiredGender;
            survey.RequiredMaxAge = model.RequiredMaxAge;
            survey.RequiredMinAge = model.RequiredMinAge;
            survey.RequiredPositiveUserRating = model.RequiredPositiveUserRating;

            _context.SaveChanges();

            return RedirectToAction("SurveyInformation", "Admin", new { Id = model.SurveyId });
        }

        #endregion


        #region Adding survey questions
        [HttpGet]
        public IActionResult AddSurveyQuestion(int Id = 0)
        {
            var model = new SurveyQuestion
            {
                SurveyId = Id
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddSurveyQuestion(SurveyQuestion q)
        {
            var question = new SurveyQuestion
            {
                Title = q.Title,
                Description = q.Description,
                SurveyId = q.SurveyId,
                Type = q.Type
            };

            _context.SurveyQuestions.Add(question);

            _context.SaveChanges();

            return RedirectToAction("SurveyInformation", "Admin", new { Id = question.SurveyId });
        }
        #endregion



        [HttpGet]
        public IActionResult SurveyQuestionInformation(int Id = 0)
        {

            var question = (from item in _context.SurveyQuestions where item.SurveyQuestionId == Id select item).SingleOrDefault();

            if (question != null)
            {
                List<SurveyOption> options = new List<SurveyOption>();
                if (question.Type != "Text")
                    options = (from item in _context.SurveyOptions where item.SurveyQuestionId == question.SurveyQuestionId select item).ToList();

                var model = new SurveyQuestionViewModel
                {
                    Question = question,
                    Options = options
                };

                return View(model);
            }

            return RedirectToAction("Index", "Admin");
        }


        #region Edit Survey Question
        [HttpGet]
        public IActionResult EditSurveyQuestion(int Id = 0)
        {
            var question = (from item in _context.SurveyQuestions where item.SurveyQuestionId == Id select item).SingleOrDefault();

            return View(question);
        }
        [HttpPost]
        public IActionResult EditSurveyQuestion(SurveyQuestion q)
        {
            var question = (from item in _context.SurveyQuestions where item.SurveyQuestionId == q.SurveyQuestionId select item).SingleOrDefault();

            question.Title = q.Title;
            question.Description = q.Description;
            question.Type = q.Type;

            _context.SaveChanges();

            return RedirectToAction("SurveyInformation", "Admin", new { Id = question.SurveyId });
        }
        #endregion


        #region Adding Options for questions
        [HttpGet]
        public IActionResult AddSurveyOption(int Id = 0)
        {
            var model = new SurveyOption
            {
                SurveyQuestionId = Id
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult AddSurveyOption(SurveyOption model)
        {
            var option = new SurveyOption
            {
                SurveyQuestionId = model.SurveyQuestionId,
                Title = model.Title,
                AttachedTextbox = model.AttachedTextbox
            };

            _context.SurveyOptions.Add(option);

            _context.SaveChanges();

            return RedirectToAction("SurveyQuestionInformation", "Admin", new { Id = option.SurveyQuestionId });
        }
        #endregion


        #region Editing Options for questions
        [HttpGet]
        public IActionResult EditSurveyOption(int Id = 0)
        {
            var option = (from item in _context.SurveyOptions where item.SurveyOptionId == Id select item).SingleOrDefault();

            return View(option);
        }
        [HttpPost]
        public IActionResult EditSurveyOption(SurveyOption model)
        {
            var option = (from item in _context.SurveyOptions where item.SurveyOptionId == model.SurveyOptionId select item).SingleOrDefault();

            option.Title = model.Title;
            option.AttachedTextbox = model.AttachedTextbox;

            _context.SaveChanges();

            return View(model);
        }
        #endregion

        #region Company section

        [HttpGet]
        public IActionResult AddCompany()
        {
            List<CheckBoxModel> CompanyPackages = new List<CheckBoxModel>();


            foreach (AdPackage package in AdPackages.Values)
            {
                CompanyPackages.Add(new CheckBoxModel() { Checked = false, Id = package.AdPackageId, Name = package.AdPackageName + " - €" + package.AdPackagePrice });
            }

            var model = new EditCompanyViewModel
            {
                AdPackages = CompanyPackages
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddCompany(EditCompanyViewModel model)
        {
            string AdPackageIds = "";
            if (model.AdPackages != null)
            {
                foreach (CheckBoxModel package in model.AdPackages)
                {
                    if (package.Checked)
                        AdPackageIds += package.Id.ToString() + ",";
                }
            }
            AdPackageIds = AdPackageIds.TrimEnd(',');

            var company = new Company
            {
                Name = model.Name,
                AdPackageIds = AdPackageIds,
                Displays = 0,
                Paid = true
            };

            _context.Companies.Add(company);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditCompany(int Id)
        {
            Company company = (from item in _context.Companies
                               where item.CompanyId == Id
                               select item).FirstOrDefault();
            List<CheckBoxModel> CompanyPackages = new List<CheckBoxModel>();

            List<string> CompanyOwnedPackageIds = company.AdPackageIds.Split(',').ToList();

            foreach (AdPackage package in AdPackages.Values)
            {
                if (CompanyOwnedPackageIds.Contains(package.AdPackageId.ToString()))
                    CompanyPackages.Add(new CheckBoxModel() { Checked = true, Id = package.AdPackageId, Name = package.AdPackageName + " - €" + package.AdPackagePrice });
                else
                    CompanyPackages.Add(new CheckBoxModel() { Checked = false, Id = package.AdPackageId, Name = package.AdPackageName + " - €" + package.AdPackagePrice });
            }

            var model = new EditCompanyViewModel
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Displays = company.Displays,
                paidField = company.Paid,
                AdPackages = CompanyPackages
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditCompany(EditCompanyViewModel Model)
        {
            if (ModelState.IsValid)
            {
                Company company = (from item in _context.Companies
                                   where item.CompanyId == Model.CompanyId
                                   select item).FirstOrDefault();
                string AdPackageIds = "";
                if (Model.AdPackages != null)
                {
                    foreach (CheckBoxModel package in Model.AdPackages)
                    {
                        if (package.Checked)
                            AdPackageIds += package.Id.ToString() + ",";
                    }
                }
                AdPackageIds = AdPackageIds.TrimEnd(',');

                company.Name = Model.Name;
                company.AdPackageIds = AdPackageIds;

                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(Model);
        }

        [HttpGet]
        public IActionResult AddAdPackage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAdPackage(AdPackage model)
        {
            var package = new AdPackage
            {
                AdPackageName = model.AdPackageName,
                AdPackagePrice = model.AdPackagePrice
            };

            _context.AdPackages.Add(package);
            _context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        [HttpGet]
        public IActionResult EditAdPackage(int Id = 0)
        {
            AdPackage package = (from item in _context.AdPackages where item.AdPackageId == Id select item).SingleOrDefault();
            if (package != null)
            {
                var model = new AdPackage
                {
                    AdPackageId = package.AdPackageId,
                    AdPackageName = package.AdPackageName,
                    AdPackagePrice = package.AdPackagePrice
                };
                return View(model);
            }
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public IActionResult EditAdPackage(AdPackage model)
        {
            var package = (from item in _context.AdPackages where item.AdPackageId == model.AdPackageId select item).SingleOrDefault();

            package.AdPackageName = model.AdPackageName;
            package.AdPackagePrice = model.AdPackagePrice;

            _context.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }


        #endregion


        #region room section


        #region Adding Rooms
        [HttpGet]
        public IActionResult AddChatRoom()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddChatRoom(ChatRoom model)
        {
            var chatroom = new ChatRoom
            {
                RoomName = model.RoomName
            };

            _context.ChatRooms.Add(chatroom);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        #region Renaming Rooms
        [HttpGet]
        public IActionResult RenameChatRoom(int Id = 0)
        {
            var chatroom = _context.ChatRooms.Where(x => x.ChatRoomId == Id).SingleOrDefault();

            if(chatroom != null)
                return View(chatroom);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RenameChatRoom(ChatRoom model)
        {
            var chatroom = _context.ChatRooms.Where(x => x.ChatRoomId == model.ChatRoomId).SingleOrDefault();

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        #region Deleting Rooms
        [HttpGet]
        public IActionResult DeleteChatRoom(int Id = 0)
        {
            var chatroom = _context.ChatRooms.Where(x => x.ChatRoomId == Id).SingleOrDefault();

            if (chatroom != null)
                _context.ChatRooms.Remove(chatroom);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        #endregion
    }
}