using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async void Register(string email, string password)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt(saltSize);
            byte[] passwordSalt= GenerateSaltedHash(plainText, salt);

            User user = new User(email, passwordSalt, salt);
            await userRepo.InsertAsync(user);
        }

        public async Task<bool> Login(string email, string password)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            User user =await GetUser(email);
            byte[] salt = user.salt;
            byte[] passwordSalt = GenerateSaltedHash(plainText, salt);

            if (passwordSalt == user.passwordSalt)
            {
                FormsAuthentication.SetAuthCookie(email, false);
                FormsAuthentication.RedirectFromLoginPage(email, false);
                return true;
                
            }
            else
            {
                return false;
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

        public void ConfirmEmail()
        {

        }

        public void ForgetPassword()
        {

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
        public async Task<User> GetUser(String email)
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