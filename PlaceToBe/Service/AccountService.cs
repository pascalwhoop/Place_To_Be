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

        public string fromAddress;
        public string mailPassword;

        /// <summary>
        /// Registers a new User.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async Task<Guid> Register(string email, string password,string activationcode)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt(saltSize);
            byte[] passwordSalt = GenerateSaltedHash(plainText, salt);

            User user = new User(email, passwordSalt, salt);
            user.status = false;
            user.activationcode = activationcode;
            return await userRepo.InsertAsync(user);
        }


        public async Task<FormsAuthenticationTicket> Login(string email, string password)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            User user = await GetEmail(email);
            byte[] salt = user.salt;
            byte[] passwordSalt = GenerateSaltedHash(plainText, salt);
           
            bool compare = CompareByteArrays(passwordSalt,user.passwordSalt);
            if (compare==true) 
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(email, false, 5);
                return ticket;
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

        /* Mail id password from where mail will be sent; At the moment from gmail 
        / with "placetobecologne@gmail.com" asusername and "placetobe123" as password.
        */

        public async Task ConfirmEmail(string activationcode)
        {
            
            fromAddress = "placetobecologne@gmail.com";
            mailPassword = "placetobe123";

            string messageBody = "Mail confirmed.";
            messageBody += "<br /><br />Thank you for the Registration";

            //Create smtp connection.
            SmtpClient client = new SmtpClient();
            client.Port = 587; //outgoing port for the mail-server.
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword);

            // Fill the mail form.
            var send_mail = new MailMessage();

            send_mail.IsBodyHtml = true;
            //address from where mail will be sent.
            send_mail.From = new MailAddress("placetobecologne@gmail.com");
            //address to which mail will be sent.           
            send_mail.To.Add(new MailAddress("madys1955@rhyta.com"));
            //subject of the mail.
            send_mail.Subject = "Confirmation: PlaceToBe";
            send_mail.Body = messageBody;

            User user = await userRepo.GetByActivationCode(activationcode);
            user.status = true;
            await userRepo.UpdateAsync(user);
            
            //Send the mail 
            client.Send(send_mail);
            
        }

        /* SendActivationEmail: Send an email to user, register with "inactive" status.
         * Mail id password from where mail will be sent; At the moment from gmail 
        / with "placetobecologne@gmail.com" asusername and "placetobe123" as password.
        */
        public async Task SendActivationEmail(string email, string passwort)
        {
            fromAddress = "placetobecologne@gmail.com";
            mailPassword = "placetobe123";
            
            string activationCode = Guid.NewGuid().ToString();

            string messageBody = "Confirm the mail:";
            messageBody += "<br /><br />Please click the following link to activate your account";
            messageBody += "<br /><a href = ' http://localhost:18172/api/user?confirm=" + activationCode + "'>Click here to activate your account.</a>";
            messageBody += "<br /><br />Thanks"+activationCode;

            //Create smtp connection.
            SmtpClient client = new SmtpClient();
            client.Port = 587; //outgoing port for the mail-server.
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword);

            // Fill the mail form.
            var send_mail = new MailMessage();

            send_mail.IsBodyHtml = true;
            //address from where mail will be sent.
            send_mail.From = new MailAddress("placetobecologne@gmail.com");
            //address to which mail will be sent.           
            send_mail.To.Add(new MailAddress("Madys1955@rhyta.com"));
            //subject of the mail.
            send_mail.Subject = "Registration: PlaceToBe";

            send_mail.Body = messageBody;
            //Register the user with inactive status (status==false)
            await Register(email, passwort, activationCode);

            
            //Send the mail 
            client.Send(send_mail);
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