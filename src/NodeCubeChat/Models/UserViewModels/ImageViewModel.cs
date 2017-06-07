using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NodeCubeChat.Models.UserViewModels
{
    public class ImageViewModel
    {
        public ApplicationUser ImageOwner { get; set; }
        public double Rating { get; set; }
        public Image Image { get; set; }
        public bool Owner { get; set; }
    }
}
