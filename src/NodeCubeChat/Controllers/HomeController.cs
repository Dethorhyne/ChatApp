using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using NodeCubeChat.Models;
using NodeCubeChat.Models.HomeViewModels;
using NodeCubeChat.Data.Extensions;

namespace NodeCubeChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private ChatContext _context;

        public HomeController(
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ChatContext context)
        {
            _signInManager = signInManager;
            _UserManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string Id = "Welcome")
        {
            ValidateRooms();

            if (_signInManager.IsSignedIn(User))
            {
                if(Id!="Welcome")
                {
                    if (!_context.ChatRooms.Any(x => x.RoomName == Id))
                        Id = "Welcome";
                }

                var currentUser = await _UserManager.GetUserAsync(User);

                List<ChatMessageTemplate> messages = new List<ChatMessageTemplate>();
                foreach (GlobalMessage message in _context.GlobalMessages.Where(o => o.RoomName == Id).OrderByDescending(o => o.TimeStamp).Take(10).ToList())
                {
                    messages.Add(new ChatMessageTemplate()
                    {
                        Message = message.Message,
                        Timestamp = message.TimeStamp,
                        DisplayName = (from item in _UserManager.Users
                                       where item.NumericId == message.SenderId
                                       select item.DisplayName).FirstOrDefault(),
                        ProfilePicture = (from item in _UserManager.Users
                                          where item.NumericId == message.SenderId
                                          select item.ProfilePicture).FirstOrDefault(),
                    });
                }

                var model = new ChatIndexViewModel
                {
                    RoomName = Id,
                    ProfilePicture = currentUser.ProfilePicture,
                    DisplayName = currentUser.DisplayName,
                    Rooms = (from item in _context.ChatRooms select item.RoomName).ToList(),
                    IsCurrentUserAdmin = currentUser.IsAdmin,
                    LastMessages = messages.OrderBy(x => x.Timestamp).ToList()
                };
                return View(model);
            }
            else
                return RedirectToAction("Login", "Account");
        }

        private void ValidateRooms()
        {
            if(_context.ChatRooms.Count() == 0)
            {
                _context.ChatRooms.Add(new ChatRoom { RoomName = "Welcome" });
                _context.SaveChanges();
                _context.ChatRooms.Add(new ChatRoom { RoomName = "Sport" });
                _context.SaveChanges();
                _context.ChatRooms.Add(new ChatRoom { RoomName = "Cooking" });
                _context.SaveChanges();
                _context.ChatRooms.Add(new ChatRoom { RoomName = "Gaming" });
                _context.SaveChanges();
                _context.ChatRooms.Add(new ChatRoom { RoomName = "Random" });
                _context.SaveChanges();
                _context.ChatRooms.Add(new ChatRoom { RoomName = "NSFW" });
                _context.SaveChanges();
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public IActionResult History( string Id = "Welcome")
        {
            if (Id != "Welcome")
            {
                if (!_context.ChatRooms.Any(x => x.RoomName == Id))
                    Id = "Welcome";
            }
            List<ChatMessageTemplate> messages = new List<ChatMessageTemplate>();
            foreach(GlobalMessage message in _context.GlobalMessages.Where(o => o.RoomName == Id).OrderBy(o => o.TimeStamp).ToList())
            {
                messages.Add(new ChatMessageTemplate() {
                    Message = message.Message,
                    Timestamp = message.TimeStamp,
                    DisplayName = (from item in _UserManager.Users
                                   where item.NumericId == message.SenderId
                                   select item.DisplayName).FirstOrDefault(),
                    ProfilePicture = (from item in _UserManager.Users
                             where item.NumericId == message.SenderId
                             select item.ProfilePicture).FirstOrDefault(),
                });
            }
            var model = new HistoryViewModel
            {
                RoomName = Id,
                Messages = messages
            };
            return View(model);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Report()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _UserManager.GetUserAsync(HttpContext.User);
        }

        [HttpPost]
        public async Task<JsonResult> AddMessage(string Message, string Room)
        {
            object result = new { errors = String.Empty };
            if (Message != null && _signInManager.IsSignedIn(User))
            {
                var currentUser = await _UserManager.GetUserAsync(User);

                var _Message = new GlobalMessage()
                {
                    Message = Message,
                    SenderId = currentUser.NumericId,
                    TimeStamp = DateTime.Now,
                    RoomName = Room
                };

                _context.GlobalMessages.Add(_Message);

                _context.SaveChanges();

                result = new { errors = "No errors" };
                return new JsonResult(result);

            }
            result = new { errors = "errors found" };
            return new JsonResult(result);
        }
    }
}
