using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NodeCubeChat.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using NodeCubeChat.Models.UserViewModels;

namespace NodeCubeChat.Controllers
{
    public class UserController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private ChatContext _context;
        private IHostingEnvironment _environment;

        public UserController(
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ChatContext context, IHostingEnvironment environment)
        {
            _signInManager = signInManager;
            _UserManager = userManager;
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Profile(string Id = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var currentUser = await _UserManager.GetUserAsync(User);
                ApplicationUser user = null;
                if (Id == null)
                {
                    user = currentUser;
                }
                else
                {
                    if (_UserManager.Users.Any(item => item.DisplayName == Id))
                    {
                        user = (from x in _UserManager.Users where x.DisplayName == Id select x).FirstOrDefault();

                    }
                }

                int rating = 0;

                rating += _context.UserRatings.Where(item => item.RatedUser == user.UserName && item.Score == true).Count();
                rating -= _context.UserRatings.Where(item => item.RatedUser == user.UserName && item.Score == false).Count();



                var model = new ProfileViewModel
                {
                    IsUserAdmin = user.IsAdmin,
                    IsCurrentUserAdmin = currentUser.IsAdmin,
                    DisplayName = user.DisplayName,
                    EMail = user.Email,
                    City = (user.City != null) ? user.City : "Not set",
                    DateOfBirth = user.DateOfBirth,
                    FirstName = (user.FirstName != null) ? user.FirstName : "Not",
                    LastName = (user.LastName != null) ? user.LastName : "set",
                    Interests = (user.Interests != null) ? user.Interests.Split(',').ToList() : new List<string>(),
                    ProfilePicture = user.ProfilePicture,
                    Gender = user.Gender == (short)Gender.NotSet ? "Not set"
                           : user.Gender == (short)Gender.Male ? "Male"
                           : user.Gender == (short)Gender.Female ? "Female"
                           : "Undefined",
                    Rating = rating,
                    Images = (from item in _context.Images
                              where item.OwnerUsername == user.UserName
                              select item).ToList(),
                    Self = (user.UserName == User.Identity.Name),
                    DidCurrentUserRateThisUser = _context.UserRatings.Any(x => x.RatingUser == User.Identity.Name && x.RatedUser == user.UserName)
                };
                return View(model);

            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<JsonResult> UpvoteUser(string DisplayName = null)
        {
            var ratinguser = await _UserManager.GetUserAsync(User);
            var rateduser = _UserManager.Users.Where(user => user.DisplayName == DisplayName).SingleOrDefault();

            object result = new { errors = String.Empty };
            
            if (_context.UserRatings.Any(x => x.RatingUser == ratinguser.UserName && x.RatedUser == rateduser.UserName))
            {
                result = new { errors = "User already rated" };
                return new JsonResult(result);
            }

            var rating = new UserRating
            {
                RatedUser = rateduser.UserName,
                RatingUser = ratinguser.UserName,
                Score = true
            };

            _context.UserRatings.Add(rating);

            _context.SaveChanges();

            result = new { errors = "No errors" };
            return new JsonResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> DownvoteUser(string DisplayName = null)
        {
            var ratinguser = await _UserManager.GetUserAsync(User);
            var rateduser = _UserManager.Users.Where(user => user.DisplayName == DisplayName).SingleOrDefault();

            object result = new { errors = String.Empty };

            if (_context.UserRatings.Any(x => x.RatingUser == ratinguser.UserName && x.RatedUser == rateduser.UserName))
            {
                result = new { errors = "User already rated" };
                return new JsonResult(result);
            }

            var rating = new UserRating
            {
                RatedUser = rateduser.UserName,
                RatingUser = ratinguser.UserName,
                Score = false
            };

            _context.UserRatings.Add(rating);

            _context.SaveChanges();

            result = new { errors = "No errors" };
            return new JsonResult(result);
        }

        public async Task<IActionResult> Gallery(string Id = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = null;
                if (Id == null)
                {
                    user = await _UserManager.GetUserAsync(User);
                }
                else
                {
                    if (_UserManager.Users.Any(item => item.DisplayName == Id))
                    {
                        user = (from x in _UserManager.Users where x.DisplayName == Id select x).FirstOrDefault();

                    }
                }
                

                var model = new GalleryViewModel
                {
                    DisplayName = user.DisplayName,
                    EMail = user.Email,
                    Self = (user.UserName == User.Identity.Name)
                };
                return View(model);

            }
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Image(string Id = null)
        {
            if(Id == null)
            {
                return View("Error");
            }
            if (_signInManager.IsSignedIn(User))
            {
                Image image = _context.Images.Where(item => item.FileName == Id).SingleOrDefault();

                double rating = -1;

                if(_context.ImageRatings.Any(item => item.ImageId == image.ImageId))
                {
                    rating = _context.ImageRatings.Where(item => item.ImageId == image.ImageId).Select(item => item.Score).Sum() / _context.ImageRatings.Where(item => item.ImageId == image.ImageId).Count();
                }

                var model = new ImageViewModel
                {
                    Image = image,
                    Owner = (image.OwnerUsername == User.Identity.Name) ? true : false,
                    Rating = rating,
                    ImageOwner = _UserManager.Users.Where(item => item.UserName == image.OwnerUsername).SingleOrDefault()
                };
                return View(model);

            }
            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> ImageRate(int Rating)
        {

            string ImageId = Request.Headers["Referer"].ToString().Split('/').Last();
            var ratinguser = await _UserManager.GetUserAsync(User);
            var Image = _context.Images.Where(item => item.FileName == ImageId).SingleOrDefault();

            var rating = new ImageRating
            {
                ImageId = Image.ImageId,
                RatingUser = ratinguser.UserName,
                Score = Rating
            };

            _context.ImageRatings.Add(rating);

            _context.SaveChanges();

            return RedirectToAction("Image", "User", new { Id = ImageId });
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ICollection<IFormFile> files)
        {
            var user = await _UserManager.GetUserAsync(User);

            var uploads = Path.Combine(_environment.WebRootPath, "images");
            uploads = Path.Combine(uploads, "Gallery");



            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string extension = file.FileName.Split('.').ToArray().Last();

                    string filename = HelperMethods.Md5Hash(user.UserName + file.FileName + file.Length.ToString() + DateTime.Now.ToString());




                    var image = new Image
                    {
                        FileName = filename,
                        OwnerUsername = user.UserName,
                        Extension = extension,
                        ServerPath = "/images/Gallery/" + filename + "." + extension
                    };


                    using (var fileStream = new FileStream(Path.Combine(uploads, image.FileName+"."+image.Extension), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    _context.Images.Add(image);

                    await _context.SaveChangesAsync();
                }
            }
            return View();
        }


        public enum Gender
        {
            NotSet = 0,
            Male = 1,
            Female = 2,
            Undefined = 9
        }
    }
}
