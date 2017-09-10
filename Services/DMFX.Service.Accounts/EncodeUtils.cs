using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;

namespace DMFX.Service.Accounts
{
    public class EncodeUtils
    {
        public static string CreateAccountKey()
        {
            string result = Guid.NewGuid().ToString().Replace("-", "").Replace("{", "").Replace("}", "");

            result = Crypto.SHA1(result);

            return result;
        }

        public static string CreateSessionID()
        {
            string result = Guid.NewGuid().ToString();

            return result;
        }

        public static string GetPasswordHash(string pwd)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] pwdBytes = GetBytes(pwd);

            byte[] plainText =
              new byte[pwdBytes.Length];

            for (int i = 0; i < pwdBytes.Length; i++)
            {
                plainText[i] = pwdBytes[i];
            }

            string hashedPassword = Convert.ToBase64String(algorithm.ComputeHash(plainText));

            return hashedPassword;
        }

        public static bool IsValidPassword(string pwdHash, string pwd)
        {
            bool result = pwdHash.Equals(GetPasswordHash(pwd));

            return result;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}