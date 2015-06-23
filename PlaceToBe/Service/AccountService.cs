using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace placeToBe.Services
{
    public class AccountService
    {
        UserRepository userRepo = new UserRepository();
        int saltSize = 20;
        /// <summary>
        /// Registers a new User.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async Task<Guid> Register(string email, string password)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt(saltSize);
            byte[] passwordSalt = GenerateSaltedHash(plainText, salt);

            User user = new User(email, passwordSalt, salt);
            user.userId = new Guid();
            //SendActivationEmail(email, password);
            return await userRepo.InsertAsync(user);
        }

        public async Task<User> Login(string email, string password)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            User user = await GetEmail(email);
            byte[] salt = GenerateSalt(saltSize);
            byte[] passwordSalt = GenerateSaltedHash(plainText, salt);

            if (passwordSalt == user.passwordSalt)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(email, false, 5);
                return await userRepo.GetByEmailAsync(email);
            }
            else
            {
                return null;
            }
        }

        //Log out the user and redirect to login-page.
        public void Logout()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        //Facebook-Login
        public void ExternalLogin()
        {

        }

        public void ConfirmEmail(string email)
        {
            MailMessage oMail = new System.Net.Mail.MailMessage();
            SmtpClient smtp = new System.Net.Mail.SmtpClient();
            oMail.From = new System.Net.Mail.MailAddress("placetobe@gmail.com");
            //oMail.To.Add(TextBox1.Text.Trim());
            oMail.Subject = "EMail Confirmation";
            oMail.Body = "Body*";
            oMail.IsBodyHtml = true;
            smtp.Host = "smtp.sendgrid.net";
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential("myusername", "mypassword");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = cred;
            smtp.Send(oMail);
        }

        private void SendActivationEmail(string email, string password)
        {
            string activationCode = Guid.NewGuid().ToString();
            using (MailMessage mm = new MailMessage("sender@gmail.com", email))
            {
                mm.Subject = "Account Activation";
                string body = "Confirm the mail:";
                body += "<br /><br />Please click the following link to activate your account";
                body += "<br /><a href = ' ActivationCode=" + activationCode + "'>Click here to activate your account.</a>";
                body += "<br /><br />Thanks";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("sender@gmail.com", password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        public void ForgetPassword(string email)
        {
            var test = userRepo.GetByEmailAsync(email);
        }

        public void ResetPassword()
        {

        }

        #region Helper Methods
        /// <summary>
        /// Get salt from db
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <returns>return the user saved in the db</returns>
        public async Task<User> GetEmail(String email)
        {
            return await userRepo.GetByEmailAsync(email);
        }


        /// <summary>
        /// Generate Salt
        /// </summary>
        /// <returns>Salt</returns>
        private byte[] GenerateSalt(int saltSize)
        {
            byte[] saltCrypt = new byte[saltSize];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltCrypt);
            return saltCrypt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText">password text</param>
        /// <param name="salt">generated Salt</param>
        /// <returns>Value with plaintext+salt</returns>
        private byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        public static bool CompareByteArrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }

}