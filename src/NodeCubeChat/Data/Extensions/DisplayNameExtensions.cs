using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NodeCubeChat.Data.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            
            var DisplayName = principal.Claims.FirstOrDefault(c => c.Type == "DisplayName");
            return DisplayName?.Value;
        }
    }
}
