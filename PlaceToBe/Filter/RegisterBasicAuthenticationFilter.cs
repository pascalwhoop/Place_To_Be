using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace placeToBe.Filter
{
    public class RegisterBasicAuthenticationFilter : BasicAuthenticationFilter
    {
        public RegisterBasicAuthenticationFilter()
        { }

        public RegisterBasicAuthenticationFilter(bool active)
            : base(active)
        { active = true; }


        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}