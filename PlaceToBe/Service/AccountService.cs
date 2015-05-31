using placeToBe.Model;
using placeToBe.Model.Entities;
using placeToBe.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace placeToBe.Services
{
    public class AccountService
    {
        MongoDbRepository<User> repo = new MongoDbRepository<User>();
        AccountService aService = new AccountService();
        int saltLength = 20;

        /// <summary>
        /// Registers a new User.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void Register(String email, String password)
        {
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            byte[] salt = aService.GenerateSalt();
            byte[] passwordSalt= GenerateSaltedHash(plainText, salt);

            User user = new User();
            user.email = email;
            user.passwordSalt = passwordSalt;
            repo.InsertAsync(user);

        }

        /// <summary>
        /// Generate Salt
        /// </summary>
        /// <returns>Salt</returns>
        private byte[] GenerateSalt(){
            byte[] saltCrypt = new byte[saltLength];
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
        public void Login()
        {

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

        public void Logoff()
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
    }
}