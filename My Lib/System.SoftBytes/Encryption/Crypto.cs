//-------------------------------------------------------------------------------------------------
// Code from Data & Object Factory http://www.dofactory.com/Framework/Framework.aspx
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace System.SoftBytes.Encryption
{
    /// <summary>
    /// Cryptographic class for encryption and decryption of string values.
    /// </summary>
    public static class Crypto
    {
        // Arbitrary key and iv vector.
        private const String c_key = "EA81AA1D5FC1EC53E84F30AA746139EEBAFF8A9B76638895";
        private const String c_iv  = "87AF7EA221F3FFF5";

        private static TripleDESCryptoServiceProvider s_des3 = InitializeTripleDES();

        /// <summary>
        /// Initializes static members of the Crypto class.
        /// </summary>
        private static TripleDESCryptoServiceProvider InitializeTripleDES()
        {
            s_des3      = new TripleDESCryptoServiceProvider();
            s_des3.Mode = CipherMode.CBC;
            return s_des3;
        }

        /// <summary>
        /// Generates a 24 byte Hex key.
        /// </summary>
        /// <returns>Generated Hex key.</returns>
        public static String GenerateKey()
        {
            // Length is 24.
            s_des3.GenerateKey();
            return BytesToHex(s_des3.Key);
        }

        /// <summary>
        /// Generates an 8 byte Hex IV (Initialization Vector).
        /// </summary>
        /// <returns>Initialization vector.</returns>
        public static String GenerateIV()
        {
            // Length = 8.
            s_des3.GenerateIV();
            return BytesToHex(s_des3.IV);
        }

        /// <summary>
        /// Converts a hex string to a byte array.
        /// </summary>
        /// <param name="hex">Hex string.</param>
        /// <returns>Byte array.</returns>
        private static Byte[] HexToBytes(String hex)
        {
            Byte[] bytes = new Byte[hex.Length / 2];
            for (Int32 i = 0; i < hex.Length / 2; i++)
            {
                String code = hex.Substring(i * 2, 2);
                bytes[i] = Byte.Parse(
                    code, 
                    NumberStyles.HexNumber, 
                    CultureInfo.InvariantCulture);
            }

            return bytes;
        }

        /// <summary>
        /// Converts bytes to hex.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        private static String BytesToHex(Byte[] bytes)
        {
            StringBuilder hex = new StringBuilder();
            for (Int32 i = 0; i < bytes.Length; i++)
            {
                hex.AppendFormat("{0:X2}", bytes[i]);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Encrypts a memory string (i.e. variable).
        /// </summary>
        /// <param name="data">String to be encrypted.</param>
        /// <param name="key">Encryption key.</param>
        /// <param name="iv">Encryption initialization vector.</param>
        /// <returns>Encrypted string.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = " ")]
        public static String Encrypt(String data, String key, String iv)
        {
            Byte[] bdata = Encoding.ASCII.GetBytes(data);
            Byte[] bkey = HexToBytes(key);
            Byte[] biv = HexToBytes(iv);

            MemoryStream stream = new MemoryStream();
            CryptoStream encStream = new CryptoStream(stream,
                s_des3.CreateEncryptor(bkey, biv), CryptoStreamMode.Write);

            encStream.Write(bdata, 0, bdata.Length);
            encStream.FlushFinalBlock();
            encStream.Close();

            return BytesToHex(stream.ToArray());
        }

        /// <summary>
        /// Decrypts a memory string (i.e. variable).
        /// </summary>
        /// <param name="data">String to be decrypted.</param>
        /// <param name="key">Original encryption key.</param>
        /// <param name="iv">Original initialization vector.</param>
        /// <returns>Decrypted string.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = " ")]
        public static String Decrypt(String data, String key, String iv)
        {
            Byte[] bdata = HexToBytes(data);
            Byte[] bkey = HexToBytes(key);
            Byte[] biv = HexToBytes(iv);

            MemoryStream stream = new MemoryStream();
            CryptoStream encStream = new CryptoStream(stream,
                s_des3.CreateDecryptor(bkey, biv), CryptoStreamMode.Write);

            encStream.Write(bdata, 0, bdata.Length);
            encStream.FlushFinalBlock();
            encStream.Close();

            return Encoding.ASCII.GetString(stream.ToArray());
        }

        /// <summary>
        /// Standard encrypt method for Patterns in Action.
        /// Uses the predefined key and iv.
        /// </summary>
        /// <param name="data">String to be encrypted.</param>
        /// <returns>Encrypted string.</returns>
        public static String YokeEncrypt(String data)
        {
            return Encrypt(data, c_key, c_iv);
        }

        /// <summary>
        /// Standard decrypt method for Patterns in Action.
        /// Uses the predefined key and iv.
        /// </summary>
        /// <param name="data">String to be decrypted.</param>
        /// <returns>Decrypted string.</returns>
        public static String YokeDecrypt(String data)
        {
            return Decrypt(data, c_key, c_iv);
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static String GetMD5Hash(String input)
        {
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sb = new StringBuilder();

            // Create a new instance of the MD5CryptoServiceProvider object.
            using (MD5 md5Hasher = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                Byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));             

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (Int32 i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
                }
            }

            // Return the hexadecimal string.
            return sb.ToString();
        }

        // Verify a hash against a string.
        public static Boolean VerifyMD5Hash(String input, String hash)
        {
            // Hash the input.
            String hashOfInput = GetMD5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (comparer.Compare(hashOfInput, hash) == 0)
            {
                return true;
            }
            
            return false;
        }
    }
}
