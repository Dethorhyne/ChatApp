using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.ManageViewModels
{
    public class ChangeEmailViewModel
    {
        [EmailAddress]
    
        public string CurrentEmail { get; set; }
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        [EmailAddress]
        [Compare("NewEmail", ErrorMessage = "The new E-mail and confirmation E-mail do not match.")]
        public string ConfirmNewEmail { get; set; }
    }
}
