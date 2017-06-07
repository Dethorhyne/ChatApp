using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodeCubeChat.Models.PrivateViewModels;
using Microsoft.AspNetCore.Identity;
using NodeCubeChat.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NodeCubeChat.Controllers
{
    public class PrivateController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private ChatContext _context;
        private List<ConversationTemplate> Conversations = new List<ConversationTemplate>();

        public PrivateController(
        SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ChatContext context)
        {
            _signInManager = signInManager;
            _UserManager = userManager;
            _context = context;
            
        }

        async private Task<bool> FetchConversations()
        {
            var currentUser = await _UserManager.GetUserAsync(User);

            string partnername = String.Empty;
            string partnerprofilepicture = String.Empty;
            string lasttext = String.Empty;
            DateTime? lasttime = null;

            foreach (Conversation convo in from item in _context.Conversations where item.InitiatorId == currentUser.NumericId || item.PartnerId == currentUser.NumericId select item)
            {
                if (convo.InitiatorId == currentUser.NumericId)
                {
                    partnername = (from user in _UserManager.Users where user.NumericId == convo.PartnerId select user.DisplayName).FirstOrDefault();
                    partnerprofilepicture = (from user in _UserManager.Users where user.NumericId == convo.PartnerId select user.ProfilePicture).FirstOrDefault();
                }
                else
                {
                    partnername = (from user in _UserManager.Users where user.NumericId == convo.InitiatorId select user.DisplayName).FirstOrDefault();
                    partnerprofilepicture = (from user in _UserManager.Users where user.NumericId == convo.InitiatorId select user.ProfilePicture).FirstOrDefault();
                }

                if (_context.PrivateMessages.Any(message => message.ConversationId == convo.ConversationId))
                {
                    PrivateMessage lastMessage = _context.PrivateMessages.LastOrDefault(im => im.ConversationId == convo.ConversationId);
                    lasttext = lastMessage.Message;
                    lasttime = lastMessage.TimeStamp;
                }

                Conversations.Add(new ConversationTemplate() { ConversationId = convo.ConversationId, PartnerDisplayName = partnername, PartnerProfilePicture = partnerprofilepicture,  LastMessageText = lasttext, LastMessageTime = lasttime, InitiationDate = (DateTime)convo.DateInitiated, Approved = convo.Accepted });
            }
            return true;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Chat");
        }

        public async Task<IActionResult> Chat(string Id = null)
        {
            if(_signInManager.IsSignedIn(User))
            {
                PrivateViewModel model = null;
                string PartnerDisplayName = "";
                await FetchConversations();
                List<ChatMessageTemplate> Messages = new List<ChatMessageTemplate>();
                var currentUser = await _UserManager.GetUserAsync(User);
                

                if (Id!=null)
                {
                    var partner = (from item in _UserManager.Users where item.DisplayName == Id select item).SingleOrDefault();

                    if(partner == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    PartnerDisplayName = partner.DisplayName;



                    bool convoApproved = Conversations.Where(x => x.PartnerDisplayName == PartnerDisplayName).Select(item => item.Approved).SingleOrDefault();
                    bool CurrentUserInitiated = false;

                    int ConversationId = Conversations.Where(item => item.PartnerDisplayName == partner.DisplayName).Select(item => item.ConversationId).SingleOrDefault();

                    if(ConversationId == 0)
                    {
                        return RedirectToAction("Profile", "User", new { Id = Id });
                    }

                    if (_context.Conversations.Any(item => item.ConversationId == ConversationId && item.InitiatorId == currentUser.NumericId))
                    {
                        CurrentUserInitiated = true;
                    }
                    foreach (PrivateMessage message in _context.PrivateMessages.Where(item => item.ConversationId == ConversationId))
                    {
                        if (message.SenderId == currentUser.NumericId)
                            Messages.Add(new ChatMessageTemplate() { Message = message.Message, Timestamp = message.TimeStamp, DisplayName = currentUser.DisplayName, ProfilePicture = currentUser.ProfilePicture });

                        if (message.SenderId == partner.NumericId)
                            Messages.Add(new ChatMessageTemplate() { Message = message.Message, Timestamp = message.TimeStamp, DisplayName = partner.DisplayName, ProfilePicture = currentUser.ProfilePicture });
                    }

                    model = new PrivateViewModel
                    {
                        ProfilePicture = currentUser.ProfilePicture,
                        DisplayName = currentUser.DisplayName,
                        ConversationId = ConversationId,
                        Conversation = Conversations,
                        Messages = Messages,
                        ConversationActive = true,
                        ConversationApproved = convoApproved,
                        IsCurrentUserInitiator = CurrentUserInitiated,
                        PartnerDisplayName = PartnerDisplayName
                    };

                    return View(model);
                }

                if(Conversations.Count>0)
                {
                    if(Conversations.Any(x=> x.LastMessageTime!=null))
                    {
                        var convoredirect = Conversations.OrderBy(x => x.LastMessageTime).LastOrDefault();

                        return RedirectToAction("Chat", "Private", new { Id = convoredirect.PartnerDisplayName });
                    }
                    else
                    {
                        var convoredirect = Conversations.OrderBy(x => x.InitiationDate).LastOrDefault();

                        return RedirectToAction("Chat", "Private", new { Id = convoredirect.PartnerDisplayName });
                    }
                }

                model = new PrivateViewModel
                {
                    ConversationId = 0,
                    ConversationActive = false,
                    Conversation = Conversations,
                };

                return View(model);

            }
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<JsonResult> ApproveConversation(int ConversationId)
        {
            object result = new { errors = String.Empty };
            if (ConversationId > 0 && _signInManager.IsSignedIn(User))
            {
                var currentUser = await _UserManager.GetUserAsync(User);

                var convo = _context.Conversations.Where(x => x.ConversationId == ConversationId).Select(x => x).SingleOrDefault();

                convo.Accepted = true;
                convo.DateAccepted = DateTime.Now;

                _context.SaveChanges();

                result = new { errors = "No errors" };
                return new JsonResult(result);

            }
            result = new { errors = "errors found" };
            return new JsonResult(result);
        }


        [HttpPost]
        public async Task<JsonResult> AddPrivateMessage(string Message, int ConversationId)
        {
            object result = new { errors = String.Empty };
            if (Message != null && _signInManager.IsSignedIn(User))
            {
                var currentUser = await _UserManager.GetUserAsync(User);

                await FetchConversations();

                if(Conversations.Any(x => x.ConversationId == ConversationId))
                {
                    var _Message = new PrivateMessage()
                    {
                        Message = Message,
                        SenderId = currentUser.NumericId,
                        TimeStamp = DateTime.Now,
                        ConversationId = ConversationId
                    };

                    _context.PrivateMessages.Add(_Message);

                    _context.SaveChanges();

                    result = new { errors = "No errors" };
                    return new JsonResult(result);
                }

                result = new { errors = currentUser.DisplayName+" is not part of this conversation" };
                return new JsonResult(result);


            }
            result = new { errors = "errors found" };
            return new JsonResult(result);
        }

        public async Task<IActionResult> ChatRequest(string Id = null)
        {
            if (_signInManager.IsSignedIn(User))
            {

                await FetchConversations();

                var Partner = _UserManager.Users.Where(x => x.DisplayName == Id).SingleOrDefault();

                if(Partner == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var currentUser = await _UserManager.GetUserAsync(User);

                int ConversationId = Conversations.Where(item => item.PartnerDisplayName == Partner.DisplayName).Select(item => item.ConversationId).SingleOrDefault();

                if (ConversationId == 0)
                {
                    var convo = new Conversation()
                    {
                        Accepted = false,
                        DateAccepted = null,
                        DateInitiated = DateTime.Now,
                        InitiatorId = currentUser.NumericId,
                        PartnerId = Partner.NumericId,
                        MessagesSent = 0
                    };

                    _context.Conversations.Add(convo);

                    await _context.SaveChangesAsync();

                    return RedirectToAction("Chat", "Private", new { Id = Partner.DisplayName });
                }

                return RedirectToAction("Chat", "Private", new { Id = Partner.DisplayName });
            }
            return RedirectToAction("Login", "Account");
        }
        
    }
}
