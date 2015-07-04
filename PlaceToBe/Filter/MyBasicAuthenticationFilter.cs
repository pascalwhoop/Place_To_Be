using placeToBe.Controllers;
using placeToBe.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace placeToBe.Services
{
    public class MyBasicAuthenticationFilter : BasicAuthenticationFilter
    {

        UserController test;
        public MyBasicAuthenticationFilter()
        { }

        public MyBasicAuthenticationFilter(bool active)
            : base(active)
        { active = true; }


        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)){
                return false;
        }else{
                return true;
            }
        }
    }
}