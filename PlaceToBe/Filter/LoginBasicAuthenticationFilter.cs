using placeToBe.Controllers;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;

namespace placeToBe.Services
{
    public class LoginBasicAuthenticationFilter : BasicAuthenticationFilter
    {
        UserRepository userRepo = new UserRepository();
        AccountService acc = new AccountService();
        public LoginBasicAuthenticationFilter()
        { }

        public LoginBasicAuthenticationFilter(bool active)
            : base(active)
        { active = true; }


        protected override bool OnAuthorizeUser(string userEmail, string userPassword, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword))
            {
                return false;
            }
            else
            {

                // try nr 1
                // Task.Run(async () => { await acc.CheckPassword(userEmail,userPassword); }).Wait();

                // try nr 2

                //var task = acc.CheckPassword(userEmail, userPassword);
                //var result = task.Result;

                // try nr 3
                //bool check = await CheckPassword(userEmail, userPassword);
                //if (check == true)
                //{
                //    return return ;
                //}
                //else
                //{
                //    return false;
                //}

                // try nr 4

                Task<bool> task = Task.Run(() => CheckPassword(userEmail, userPassword));
                bool result = task.Result;

                if (result == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        private async Task<bool> CheckPassword(string userEmail, string userPassword)
        {
            try
            {
                byte[] userPasswordInBytes = Encoding.UTF8.GetBytes(userPassword);
                User user = await userRepo.GetByEmailAsync(userEmail);
                byte[] salt = user.salt;
                byte[] passwordSalt = acc.GenerateSaltedHash(userPasswordInBytes, salt);
                bool comparePasswords = acc.CompareByteArrays(passwordSalt, user.passwordSalt);

                //statement: if users password is correct and status is activated          
                if (comparePasswords == true && user.status == true)
                {
                    return true;
                }
                else if (comparePasswords == true && user.status == false)
                {
                    //Please activate your acc.
                    return false;
                }
                else
                {
                    //Authentification failed.
                    return false;
                }
            }
            catch (Exception e)
            {
                //Something go wrong.
                return false;
            }
        }

    }
}