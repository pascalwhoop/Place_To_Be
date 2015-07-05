using Newtonsoft.Json;
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
        FbUserRepository fbUserRepo = new FbUserRepository();
        int saltSize = 20;
        /// <summary>
        /// Get the email data from the ConfigurationManager.
        /// </summary>
        private string fromAddress = System.Configuration.ConfigurationManager.AppSettings["placeToBeEmail"];
        private string mailPassword = System.Configuration.ConfigurationManager.AppSettings["placeToBePasswordFromMail"];
        /// <summary>
        /// SaveFBData to out database.
        /// </summary>
        /// <param name="fbuser"></param>
        /// <returns></returns>
        public async Task SaveFBData(FbUser fbuser)
        {
            await fbUserRepo.InsertAsync(fbuser);
        }

        /// <summary>
        /// Changes the password from user (with given email) from old password to new password.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ChangePasswort(string userEmail, string oldPassword, string newPassword)
        {
            try
            {
                byte[] oldPasswordBytes = Encoding.UTF8.GetBytes(oldPassword);
                User user = await GetUserByEmail(userEmail);
                if (user != null)
                {
                    /*find out the old password from user and compare it.*/
                    byte[] oldSalt = user.salt;
                    byte[] oldPasswordSalt = GenerateSaltedHash(oldPasswordBytes, oldSalt);
                    bool comparePasswords = CompareByteArrays(oldPasswordSalt, user.passwordSalt);

                    /*statement: when users password is correct and status is activated  -> true */
                    if (comparePasswords == true && user.status == true)
                    {
                        /*set the new password now and insert into DB*/
                        byte[] newPasswordBytes = Encoding.UTF8.GetBytes(newPassword);
                        byte[] newSalt = GenerateSalt(saltSize);
                        byte[] newPasswordSalt = GenerateSaltedHash(newPasswordBytes, newSalt);
                        user.passwordSalt = newPasswordSalt;
                        user.salt = newSalt;
                        await userRepo.UpdateAsync(user);
                        return HttpStatusCode.OK;
                    }
                    else
                    {
                        //Change password is not possible because password is false, status is not activated.
                        return HttpStatusCode.Unauthorized;
                    }
                }
                else
                {
                    //User is null - User with email do not exists in database.
                    return HttpStatusCode.InternalServerError;
                }
            }
            catch (TimeoutException e)
            {
                //Database Error
                Console.WriteLine("{0} Exception caught: Database-Timeout. ", e);
                return HttpStatusCode.GatewayTimeout;
            }
            catch (Exception e)
            {
                //Database Error
                Console.WriteLine("{0} Exception caught. ", e);
                return HttpStatusCode.InternalServerError;
            }

        }

        /// <summary>
        /// Send a confirmation-email to the users email and activate the account, by setting (user.status==true).
        /// </summary>
        /// <param name="activationcode"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ConfirmEmail(string activationcode)
        {
            try
            {
                string messageBody = "Mail confirmed.";
                messageBody += "<br /><br />Thank you for the Registration";

                //Create smtp connection.
                SmtpClient client = new SmtpClient
                {

                    Port = 587, //outgoing port for the mail-server.
                    Host = "smtp.gmail.com", //smtp host from gmail.
                    EnableSsl = true, //EnabledSsl, because Email-Server need it.
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword)
                };

                // Fill the mail form.
                var sendMail = new MailMessage();
                sendMail.IsBodyHtml = true;
                //address from where mail will be sent.
                sendMail.From = new MailAddress(fromAddress);
                //address to which mail will be sent.           
                User user = await GetUserbyActivationCode(activationcode);
                sendMail.To.Add(new MailAddress(user.email));
                //subject of the mail.
                sendMail.Subject = "Confirmation: PlaceToBe";
                sendMail.Body = messageBody;
                //Change user status to true, because user is now activated
                await ChangeUserStatus(activationcode);

                try
                {
                    //Send the mail 
                    client.Send(sendMail);
                    return HttpStatusCode.OK;
                }
                catch (System.Net.Mail.SmtpException cantsend)
                {
                    //Email cannot be sent.
                    Console.WriteLine("{0} Exception caught. Email cannot be sent.", cantsend);
                    return HttpStatusCode.ServiceUnavailable;
                }
                catch (System.ArgumentNullException messagenull)
                {
                    //Email-message is null
                    Console.WriteLine("{0} Exception caught. Email is null.", messagenull);
                    return HttpStatusCode.ServiceUnavailable;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught. Confirmmail can not be done.", e);
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Send an email to user, register user with inactive status (means user.status==false).
        /// Moreover it will check, wether the user exists in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userPassword"></param>
        /// <returns>HttpStatusCode.OK if the user can be written in the database. Otherwise it returns refusal HttpStatusCode</returns>
        public async Task<HttpStatusCode> SendActivationEmail(string userEmail, string userPassword)
        {
            try
            {
                //check if user already exists. if the user is null, the user does not exists- so we can send a activationmail
                if (await CheckIfUserExists(userEmail))
                {
                    //activationcode to identify the user in the email.
                    string activationCode = Guid.NewGuid().ToString();

                    string messageBody = "Confirm the mail:";
                    messageBody += "<br /><br />Please click the following link to activate your account";
                    messageBody += "<br /><a href = ' http://localhost:18172/api/user?activationcode=" + activationCode + "'>Click here to activate your account.</a>";
                    messageBody += "<br /><br />Thanks";
                    messageBody += "<br /><br />Notice: If your browser does not allow linking in your emails go and type this:  http://localhost:18172/api/user?activationcode=" +activationCode + "in your adress bar.";

                    //Create smtp connection.
                    SmtpClient client = new SmtpClient();
                    //outgoing port for the mail-server.
                    client.Port = 587;
                    client.Host = "smtp.gmail.com";
                    client.EnableSsl = true;
                    client.Timeout = 10000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(fromAddress, mailPassword);

                    // Fill the mail form.
                    var sendMail = new MailMessage();
                    sendMail.IsBodyHtml = true;
                    //address from where mail will be sent.
                    sendMail.From = new MailAddress(fromAddress);
                    //address to which mail will be sent.           
                    sendMail.To.Add(new MailAddress(userEmail));
                    //subject of the mail.
                    sendMail.Subject = "Registration: PlaceToBe";
                    sendMail.Body = messageBody;

                    //Register the user with inactive status (status==false)
                    byte[] plainText = Encoding.UTF8.GetBytes(userPassword);
                    byte[] salt = GenerateSalt(saltSize);
                    byte[] passwordSalt = GenerateSaltedHash(plainText, salt);

                    User user = new User(userEmail, passwordSalt, salt);
                    user.status = false;
                    user.activationcode = activationCode;
                    await InsertUserToDB(user);

                    //Send the mail 
                    try
                    {
                        client.Send(sendMail);
                        return HttpStatusCode.OK;
                    }
                    catch (System.Net.Mail.SmtpException cantsend)
                    {
                        //email cannot be sent.
                        Console.WriteLine("{0} Exception caught. Email cannot be sent.", cantsend);
                        return HttpStatusCode.ServiceUnavailable;
                    }
                    catch (System.ArgumentNullException messagenull)
                    {
                        //Email-message is null
                        Console.WriteLine("{0} Exception caught. Email is null.", messagenull);
                        return HttpStatusCode.ServiceUnavailable;
                    }

                }
                else
                {
                    //User already exists
                    return HttpStatusCode.Conflict;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught. ", e);
                return HttpStatusCode.InternalServerError;
            }
        }


        /// <summary>
        /// Reset the password from the users mail and then send a mail to user with a new password. 
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ForgetPasswordReset(string userEmail)
        {
            try
            {
                User forgetUserPassword = await userRepo.GetByEmailAsync(userEmail);
                byte[] newPassword = Encoding.UTF8.GetBytes(CreateRandomString(8));
                byte[] salt = GenerateSalt(saltSize);
                byte[] passwordSalt = GenerateSaltedHash(newPassword, salt);
                forgetUserPassword.passwordSalt = passwordSalt;
                forgetUserPassword.salt = salt;
                await userRepo.UpdateAsync(forgetUserPassword);
                SendForgetPassword(newPassword, userEmail);
                return HttpStatusCode.OK;
            }
            catch (NullReferenceException usernull)
            {
                Console.WriteLine("{0} ForgetPassword: User is null", usernull);
                return HttpStatusCode.ServiceUnavailable;
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("{0} ForgetPassword: Cant update database ", e);
                return HttpStatusCode.GatewayTimeout;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught. ", e);
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Send a mail with a new password to the users email
        /// </summary>
        /// <param name="bytePassword"></param>
        /// <param name="userEmail"></param>
        public void SendForgetPassword(byte[] bytePassword, string userEmail)
        {
            string passwordString = Encoding.UTF8.GetString(bytePassword);

            string messageBody = "Your new password is: " + passwordString;
            messageBody += "<br /><a href = ' http://localhost:18172/api/login/ '>Click here to login with the new password.</a>";
            messageBody += "<br /><br />Have Fun with placeToBe.";

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
            var sendMail = new MailMessage();
            sendMail.IsBodyHtml = true;
            //address from where mail will be sent.
            sendMail.From = new MailAddress(fromAddress);
            //address to which mail will be sent.           
            sendMail.To.Add(new MailAddress(userEmail));
            //subject of the mail.
            sendMail.Subject = "PlaceToBe: New password";
            sendMail.Body = messageBody;
            //Send the mail 
            try
            {
                client.Send(sendMail);
            }
            catch (System.Net.Mail.SmtpException cantsend)
            {
                //Email cannot be sent.
                Console.WriteLine("{0} Exception caught. Email cannot be sent.", cantsend);
            }
            catch (System.ArgumentNullException messagenull)
            {
                //Email-message is null
                Console.WriteLine("{0} Exception caught. Email is null.", messagenull);
            }
        }

        ////// Cookie-Logic (unused now)
        ///// <summary>
        ///// Login the user with given email and correct password.
        ///// </summary>
        ///// <param name="usersEmail"></param>
        ///// <param name="userPassword"></param>
        ///// <returns></returns>
        //public async Task<Cookie> Login(string userEmail)
        //{
        //    try
        //    {
        //        User loginuser = await GetUserByEmail(userEmail);
        //        if (loginuser.ticket == null)
        //        {
        //            //Set a ticket for five minutes to stay logged in. 
        //            Cookie ticket = new Cookie(userEmail, userEmail);
        //            ticket.Name = userEmail;
        //            //for testing : in seconds, but as a rule 5 minutes.
        //            //set the expire-date
        //            ticket.Expires = DateTime.Now.AddSeconds(20);
        //            loginuser.ticket = ticket; ;
        //            await userRepo.UpdateAsync(loginuser);
        //            return loginuser.ticket;
        //        }
        //        else
        //        {
        //            if (loginuser.ticket.Expired)
        //            {
        //                await Logout(userEmail);
        //                return null;
        //            }
        //            else
        //            {
        //                return loginuser.ticket;
        //            }
        //        }

        //    }
        //    catch (CookieException)
        //    {
        //        //ToDo: UI-Fehlermeldung : "Cant create Cookie"
        //        return null;
        //    }
        //}

        ////Log out the user and redirect to login-page.
        //public async Task<HttpStatusCode> Logout(string userEmail)
        //{
        //    User userToLogOut = await userRepo.GetByEmailAsync(userEmail);
        //    userToLogOut.ticket = null;
        //    await userRepo.UpdateAsync(userToLogOut);
        //    return HttpStatusCode.Unauthorized;
        //}


        #region Helper Methods

        /// <summary>
        /// Create a random string from a specific content
        /// </summary>
        /// <param name="length"></param>
        /// <returns>string</returns>
        public string CreateRandomString(int length)
        {
            StringBuilder stringBuilder = new System.Text.StringBuilder();
            string content = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
                stringBuilder.Append(content[rnd.Next(content.Length)]);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Change User status from false to true to activate the account.
        /// </summary>
        /// <param name="activationcode"></param>
        /// <returns></returns>

        public async Task ChangeUserStatus(string activationcode)
        {

            User user = await GetUserbyActivationCode(activationcode);
            user.status = true;
            await userRepo.UpdateAsync(user);

        }

        ///<summary>Get user from DB by the given activationcode</summary>
        /// <param name="activationcode" </ param>
        /// <returns>User</returns>
        public async Task<User> GetUserbyActivationCode(string activationcode)
        {

            return await userRepo.GetByActivationCode(activationcode);
        }

        /// <summary>
        /// Get the user by the email of the user.
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <returns>User</returns>
        public async Task<User> GetUserByEmail(string email)
        {
            return await userRepo.GetByEmailAsync(email);
        }

        /// <summary>
        /// Insert a user into DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<Guid> InsertUserToDB(User user)
        {
            return await userRepo.InsertAsync(user);
        }

        /// <summary>
        /// Check if the user already exists in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns>true, if user exists, else false.</returns>
        public async Task<Boolean> CheckIfUserExists(string userEmail)
        {
            if (await GetUserByEmail(userEmail) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Generate Salt
        /// </summary>
        /// <returns>Salt</returns>
        public byte[] GenerateSalt(int saltSize)
        {
            byte[] saltCrypt = new byte[saltSize];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltCrypt);
            return saltCrypt;
        }

        /// <summary>
        /// Generate a "salted" Hashcode from a given plaintext and a salt.
        /// </summary>
        /// <param name="plainText">password text</param>
        /// <param name="salt">generated Salt</param>
        /// <returns>Value with plaintext+salt</returns>
        public byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
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

        /// <summary>
        /// Compare two byte-arrays.
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public bool CompareByteArrays(byte[] array1, byte[] array2)
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