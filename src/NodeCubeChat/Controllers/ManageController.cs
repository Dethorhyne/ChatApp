using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NodeCubeChat.Models;
using NodeCubeChat.Models.ManageViewModels;
using NodeCubeChat.Services;
using NodeCubeChat.Data.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace NodeCubeChat.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _environment;

        public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        IHostingEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _environment = environment;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.ChangeEmailSuccess ? "Your E-mail has been changed."
                : message == ManageMessageId.ChangeDisplayNameSuccess ? "Your Display name has been changed."
                : message == ManageMessageId.ChangeAvatarSuccess ? "Your Avatar has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var model = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                EMail = await _userManager.GetEmailAsync(user),
                DisplayName = user.DisplayName,
                Logins = await _userManager.GetLoginsAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                ProfilePicture = user.ProfilePicture
            };
            return View(model);
        }

        ////
        //// POST: /Manage/RemoveLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel account)
        //{
        //    ManageMessageId? message = ManageMessageId.Error;
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        var result = await _userManager.RemoveLoginAsync(user, account.LoginProvider, account.ProviderKey);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            message = ManageMessageId.RemoveLoginSuccess;
        //        }
        //    }
        //    return RedirectToAction(nameof(ManageLogins), new { Message = message });
        //}

        //
        // GET: /Manage/AddPhoneNumber
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        ////
        //// POST: /Manage/AddPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    // Generate the token and send it
        //    var user = await GetCurrentUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
        //    await _smsSender.SendSmsAsync(model.PhoneNumber, "Your security code is: " + code);
        //    return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        //}

        ////
        //// POST: /Manage/EnableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EnableTwoFactorAuthentication()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        await _userManager.SetTwoFactorEnabledAsync(user, true);
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        _logger.LogInformation(1, "User enabled two-factor authentication.");
        //    }
        //    return RedirectToAction(nameof(Index), "Manage");
        //}

        ////
        //// POST: /Manage/DisableTwoFactorAuthentication
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DisableTwoFactorAuthentication()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        await _userManager.SetTwoFactorEnabledAsync(user, false);
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        _logger.LogInformation(2, "User disabled two-factor authentication.");
        //    }
        //    return RedirectToAction(nameof(Index), "Manage");
        //}

        ////
        //// GET: /Manage/VerifyPhoneNumber
        //[HttpGet]
        //public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        //    // Send an SMS to verify the phone number
        //    return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        //}

        ////
        //// POST: /Manage/VerifyPhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
        //        }
        //    }
        //    // If we got this far, something failed, redisplay the form
        //    ModelState.AddModelError(string.Empty, "Failed to verify phone number");
        //    return View(model);
        //}

        ////
        //// POST: /Manage/RemovePhoneNumber
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RemovePhoneNumber()
        //{
        //    var user = await GetCurrentUserAsync();
        //    if (user != null)
        //    {
        //        var result = await _userManager.SetPhoneNumberAsync(user, null);
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.RemovePhoneSuccess });
        //        }
        //    }
        //    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        //}

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // GET: /Manage/ChangeEmail
        [HttpGet]
        public async Task<IActionResult> ChangeEmail()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var model = new ChangeEmailViewModel
            {
                CurrentEmail = await _userManager.GetEmailAsync(user)
            };
            return View(model);
        }
        //
        // GET: /Manage/ChangeEmail
        [HttpGet]
        public async Task<IActionResult> ChangeDisplayName()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var model = new ChangeDisplayNameViewModel
            {
                CurrentDisplayName = user.DisplayName
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeAvatar()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var model = new ChangeAvatarViewModel
            {
                CurrentAvatar = user.ProfilePicture
            };
            return View(model);
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Manage/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                var result = await _userManager.SetEmailAsync(user, model.NewEmail);

                if (result.Succeeded)
                {
                    await _userManager.UpdateNormalizedEmailAsync(user);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their E-mail successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangeEmailSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }
        //
        // POST: /Manage/ChangeEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeDisplayName(ChangeDisplayNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();

            if (user != null)
            {
                user.DisplayName = model.NewDisplayName;

                await _userManager.UpdateAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(3, "User changed their Display name successfully.");
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangeDisplayNameSuccess });
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);

            var uploads = Path.Combine(_environment.WebRootPath, "images");
            uploads = Path.Combine(uploads, "Avatar");
            
            if (file != null && file.Length > 0)
            {
                string extension = file.FileName.Split('.').ToArray().Last();

                string filename = HelperMethods.Md5Hash(user.UserName + file.FileName + file.Length.ToString() + DateTime.Now.ToString());
                

                var image = new Image
                {
                    FileName = filename,
                    OwnerUsername = user.UserName,
                    Extension = extension,
                    ServerPath = "/images/Avatar/" + filename + "." + extension
                };


                using (var fileStream = new FileStream(Path.Combine(uploads, image.FileName + "." + image.Extension), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                user.HasProfilePicture = true;
                user.ProfilePicture = image.ServerPath;

                await _userManager.UpdateAsync(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(3, "User changed their Avatar successfully.");
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangeAvatarSuccess });
            }

            var model = new ChangeAvatarViewModel
            {
                CurrentAvatar = user.ProfilePicture
            };
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }



        ////GET: /Manage/ManageLogins
        //[HttpGet]
        //public async Task<IActionResult> ManageLogins(ManageMessageId? message = null)
        //{
        //    ViewData["StatusMessage"] =
        //        message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.AddLoginSuccess ? "The external login was added."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    var user = await GetCurrentUserAsync();
        //    if (user == null)
        //    {
        //        return View("Error");
        //    }
        //    var userLogins = await _userManager.GetLoginsAsync(user);
        //    var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
        //    ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;
        //    return View(new ManageLoginsViewModel
        //    {
        //        CurrentLogins = userLogins,
        //        OtherLogins = otherLogins
        //    });
        //}

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return Challenge(properties, provider);
        }

        //
        // GET: /Manage/LinkLoginCallback
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(user, info);
            var message = result.Succeeded ? ManageMessageId.AddLoginSuccess : ManageMessageId.Error;
            return RedirectToAction(nameof(Index), new { Message = message });
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            ChangeDisplayNameSuccess,
            ChangeEmailSuccess,
            ChangeAvatarSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
