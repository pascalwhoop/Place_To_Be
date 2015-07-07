using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using placeToBe.Filter;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;

namespace placeToBe.Services {
    public class AccountService {
        private readonly FbUserRepository fbUserRepo = new FbUserRepository();
        private readonly FacebookUserVerification facebookVerifier = new FacebookUserVerification();

        /// <summary>
        ///     Get the email data from the ConfigurationManager.
        /// </summary>
        private readonly string fromAddress = ConfigurationManager.AppSettings["placeToBeEmail"];

        private readonly string mailPassword = ConfigurationManager.AppSettings["placeToBePasswordFromMail"];
        private readonly int saltSize = 20;
        private readonly UserRepository userRepo = new UserRepository();

        /// <summary>
        ///     SaveFBData to out database.
        /// </summary>
        /// <param name="fbuser"></param>
        /// <returns></returns>
        public async Task<FbUser> SaveFBData(FbUser fbuser) {
            if(await facebookVerifier.authorizeRequest(fbuser.shortAccessToken, fbuser.fbId))
            await fbUserRepo.InsertAsync(fbuser);
            return fbuser;
        }

        /// <summary>
        ///     Login the user with given email and correct password.
        /// </summary>
        /// <param name="usersEmail"></param>
        /// <param name="userPassword"></param>
        /// <returns></returns>
        /*public async Task<Cookie> Login(string userEmail) {
            try {
                var loginuser = await GetUserByEmail(userEmail);
                if (loginuser.ticket == null) {
                    //Set a ticket for five minutes to stay logged in. 
                    var ticket = new Cookie(userEmail, userEmail);
                    ticket.Name = userEmail;
                    //for testing : in seconds, but as a rule 5 minutes.
                    //set the expire-date
                    ticket.Expires = DateTime.Now.AddSeconds(20);
                    loginuser.ticket = ticket;
                    ;
                    await userRepo.UpdateAsync(loginuser);
                    return loginuser.ticket;
                }
                if (loginuser.ticket.Expired) {
                    await Logout(userEmail);
                    return null;
                }
                return loginuser.ticket;
            }
            catch (CookieException) {
                //ToDo: UI-Fehlermeldung : "Cant create Cookie"
                return null;
            }
        }*/

        //Log out the user and redirect to login-page.
       /* public async Task<HttpStatusCode> Logout(string userEmail) {
            var user1 = await userRepo.GetByEmailAsync(userEmail);
            user1.ticket = null;
            await userRepo.UpdateAsync(user1);
            return HttpStatusCode.Unauthorized;
        }*/

        /// <summary>
        ///     Changes the password from user (with given email) from old password to new password.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ChangePasswort(string userEmail, string oldPassword, string newPassword) {
            try {
                var oldPasswordBytes = Encoding.UTF8.GetBytes(oldPassword);
                var user = await GetUserByEmail(userEmail);
                if (user != null) {
                    /*find out the old password from user and compare it.*/
                    var oldSalt = user.salt;
                    var oldPasswordSalt = GenerateSaltedHash(oldPasswordBytes, oldSalt);
                    var comparePasswords = CompareByteArrays(oldPasswordSalt, user.passwordSalt);

                    /*statement: when users password is correct and status is activated  -> true */
                    if (comparePasswords && user.status) {
                        /*set the new password now and insert into DB*/
                        var newPasswordBytes = Encoding.UTF8.GetBytes(newPassword);
                        var newSalt = GenerateSalt(saltSize);
                        var newPasswordSalt = GenerateSaltedHash(newPasswordBytes, newSalt);
                        user.passwordSalt = newPasswordSalt;
                        user.salt = newSalt;
                        await userRepo.UpdateAsync(user);
                        return HttpStatusCode.OK;
                    }
                    //Change password is not possible because password is false, status is not activated.
                    return HttpStatusCode.BadRequest;
                }
                //User is null - User with email do not exists in database.
                return HttpStatusCode.NotFound;
            }
            catch (TimeoutException e) {
                //Database Error
                Console.WriteLine("{0} Exception caught: Database-Timeout. ", e);
                return HttpStatusCode.Conflict;
            }
            catch (Exception e) {
                //Database Error
                Console.WriteLine("{0} Exception caught. ", e);
                return HttpStatusCode.Conflict;
            }
        }

        /// <summary>
        ///     Send a confirmation-email to the users email and activate the account, by setting (user.status==true).
        /// </summary>
        /// <param name="activationcode"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmEmail(string activationcode) {
            //user already activated --> return
            if (await isUserActivated(activationcode)) return true;
            
            try {
                var messageBody = "Mail confirmed.";
                messageBody += "<br /><br />Thank you for the Registration";

                //Create smtp connection.
                var client = new SmtpClient {
                    Port = 587, //outgoing port for the mail-server.
                    Host = "smtp.gmail.com", //smtp host from gmail.
                    EnableSsl = true, //EnabledSsl, because Email-Server need it.
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress, mailPassword)
                };

                // Fill the mail form.
                var sendMail = new MailMessage();
                sendMail.IsBodyHtml = true;
                //address from where mail will be sent.
                sendMail.From = new MailAddress(fromAddress);
                //address to which mail will be sent.           
                var user = await GetUserbyActivationCode(activationcode);
                sendMail.To.Add(new MailAddress(user.email));
                //subject of the mail.
                sendMail.Subject = "Confirmation: PlaceToBe";
                sendMail.Body = messageBody;

                try {
                    //activate user
                    var u = await GetUserbyActivationCode(activationcode);
                    u.status = true;
                    await userRepo.UpdateAsync(u);
                    //Send the mail 
                    client.Send(sendMail);
                    return true;
                }
                catch (SmtpException cantsend) {
                    //Email cannot be sent.
                    Console.WriteLine("{0} Exception caught. Email cannot be sent.", cantsend);
                    return false;
                }
                catch (ArgumentNullException messagenull) {
                    //Email-message is null
                    Console.WriteLine("{0} Exception caught. Email is null.", messagenull);
                    return false;
                }
            }
            catch (Exception e) {
                Console.WriteLine("{0} Exception caught. Confirmmail can not be done.", e);
                return false;
            }
        }

        public async Task<User> createUser(User usr) {
            //check if user already exists. if the user is null, the user does not exists- so we can send a activationmail
            if (userRepo.GetByEmailAsync(usr.email) != null) throw new DuplicateNameException();
            //Register the user with inactive status (status==false)
            var plainText = Encoding.UTF8.GetBytes(usr.password);
            var salt = GenerateSalt(saltSize);
            var passwordSalt = GenerateSaltedHash(plainText, salt);

            var activationCode = await SendActivationEmail(usr.email, usr.password);
            var user = new User(usr.email, passwordSalt, salt) {status = false, activationcode = activationCode};
            await InsertUserToDB(user);
            return user;
        }

        /// <summary>
        ///     Send an email to user, register user with inactive status (means user.status==false).
        ///     Moreover it will check, wether the user exists in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="userPassword"></param>
        /// <returns>HttpStatusCode.OK if the user can be written in the database. Otherwise it returns refusal HttpStatusCode</returns>
        public async Task<String> SendActivationEmail(string userEmail, string userPassword) {
            try {
                //activationcode to identify the user in the email.
                var activationCode = Guid.NewGuid().ToString();

                var messageBody = "Confirm the mail:";
                messageBody += "<br /><br />Please click the following link to activate your account";
                messageBody += "<br /><a href = ' http://localhost:18172/api/user?activationcode=" + activationCode +
                               "'>Click here to activate your account.</a>";
                messageBody += "<br /><br />Thanks";
                messageBody +=
                    "<br /><br />Notice: If your browser does not allow linking in your emails go and type this:  http://localhost:18172/api/user?activationcode=" +
                    activationCode + "in your adress bar.";

                //Create smtp connection.
                var client = new SmtpClient();
                //outgoing port for the mail-server.
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(fromAddress, mailPassword);

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

                //Send the mail 
                try {
                    client.Send(sendMail);
                    return activationCode;
                }
                catch (SmtpException cantsend) {
                    //email cannot be sent.
                    Console.WriteLine("{0} Exception caught. Email cannot be sent.", cantsend);
                    throw;
                }
                catch (ArgumentNullException messagenull) {
                    //Email-message is null
                    Console.WriteLine("{0} Exception caught. Email is null.", messagenull);
                    throw;
                }
            }

            catch (Exception e) {
                Console.WriteLine("{0} Exception caught. ", e);
                throw;
            }
        }

        /// <summary>
        ///     Reset the password from the users mail and then send a mail to user with a new password.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task ForgetPasswordReset(string userEmail) {
            try {
                var forgetUserPassword = userRepo.GetByEmailAsync(userEmail);
                var newPassword = Encoding.UTF8.GetBytes(CreateRandomString(8));
                var salt = GenerateSalt(saltSize);
                var passwordSalt = GenerateSaltedHash(newPassword, salt);
                forgetUserPassword.passwordSalt = passwordSalt;
                forgetUserPassword.salt = salt;
                await userRepo.UpdateAsync(forgetUserPassword);
                SendForgetPassword(newPassword, userEmail);
            }
            catch (NullReferenceException usernull) {
                Console.WriteLine("{0} ForgetPassword: User is null", usernull);
            }
            catch (TimeoutException e) {
                Console.WriteLine("{0} ForgetPassword: Cant update database ", e);
            }
            catch (Exception e) {
                Console.WriteLine("{0} Exception caught. ", e);
            }
        }

        /// <summary>
        ///     Send a mail with a new password to the users email
        /// </summary>
        /// <param name="bytePassword"></param>
        /// <param name="userEmail"></param>
        public void SendForgetPassword(byte[] bytePassword, string userEmail) {
            var passwordString = Encoding.UTF8.GetString(bytePassword);

            var messageBody = "Your new password is: " + passwordString;
            messageBody +=
                "<br /><a href = ' http://localhost:18172/api/login/ '>Click here to login with the new password.</a>";
            messageBody += "<br /><br />Have Fun with placeToBe.";

            //Create smtp connection.
            var client = new SmtpClient();
            client.Port = 587; //outgoing port for the mail-server.
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(fromAddress, mailPassword);

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
            try {
                client.Send(sendMail);
            }
            catch (SmtpException cantsend) {
                //Email cannot be sent.
                Console.WriteLine("{0} Exception caught. Email cannot be sent.", cantsend);
            }
            catch (ArgumentNullException messagenull) {
                //Email-message is null
                Console.WriteLine("{0} Exception caught. Email is null.", messagenull);
            }
        }

        #region Helper Methods

        /// <summary>
        ///     Create a random string from a specific content
        /// </summary>
        /// <param name="length"></param>
        /// <returns>string</returns>
        public string CreateRandomString(int length) {
            var stringBuilder = new StringBuilder();
            var content = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var rnd = new Random();
            for (var i = 0; i < length; i++)
                stringBuilder.Append(content[rnd.Next(content.Length)]);
            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Change User status from false to true to activate the account.
        /// </summary>
        /// <param name="activationcode"></param>
        /// <returns></returns>
        public async Task<bool> isUserActivated(string activationcode) {
            var user = await GetUserbyActivationCode(activationcode);
            return user.status;
            user.status = true;
            await userRepo.UpdateAsync(user);
        }

        /// <summary>Get user from DB by the given activationcode</summary>
        /// <param name="activationcode"
        /// </param>
        /// <returns>User</returns>
        public async Task<User> GetUserbyActivationCode(string activationcode) {
            return await userRepo.GetByActivationCode(activationcode);
        }

        /// <summary>
        ///     Get the user by the email of the user.
        /// </summary>
        /// <param name="email">email of the user</param>
        /// <returns>User</returns>
        public async Task<User> GetUserByEmail(string email) {
            return userRepo.GetByEmailAsync(email);
        }

        /// <summary>
        ///     Insert a user into DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<Guid> InsertUserToDB(User user) {
            return await userRepo.InsertAsync(user);
        }

        /// <summary>
        ///     Check if the user already exists in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns>true, if user exists, else false.</returns>
        public async Task<Boolean> CheckIfUserExists(string userEmail) {
            if (await GetUserByEmail(userEmail) == null) return false;
            return true;
        }

        /// <summary>
        ///     Generate Salt
        /// </summary>
        /// <returns>Salt</returns>
        public byte[] GenerateSalt(int saltSize) {
            var saltCrypt = new byte[saltSize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltCrypt);
            return saltCrypt;
        }

        /// <summary>
        ///     Generate a "salted" Hashcode from a given plaintext and a salt.
        /// </summary>
        /// <param name="plainText">password text</param>
        /// <param name="salt">generated Salt</param>
        /// <returns>Value with plaintext+salt</returns>
        public byte[] GenerateSaltedHash(byte[] plainText, byte[] salt) {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes =
                new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++) plainTextWithSaltBytes[i] = plainText[i];
            for (var i = 0; i < salt.Length; i++) plainTextWithSaltBytes[plainText.Length + i] = salt[i];

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        ///     Compare two byte-arrays.
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public bool CompareByteArrays(byte[] array1, byte[] array2) {
            if (array1.Length != array2.Length) return false;

            for (var i = 0; i < array1.Length; i++) if (array1[i] != array2[i]) return false;

            return true;
        }

        #endregion
    }
}