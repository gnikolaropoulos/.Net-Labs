using System;
using System.SoftBytes.Encryption;
using Xunit;

namespace System.SoftBytes.Tests.Encryption
{
    public sealed class CryptoTest
    {
        private readonly String m_password;

        public CryptoTest()
        {
            m_password = "Password";
        }

        [Fact]
        public void EncryptedValueIsDifferentFromOriginal()
        {
            String encrypted = Crypto.YokeEncrypt(m_password);
            Assert.NotSame(encrypted, m_password);
        }

        [Fact]
        public void DecryptedValueIsEqualToOriginal()
        {
            String encrypted = Crypto.YokeEncrypt(m_password);
            String decrypted = Crypto.YokeDecrypt(encrypted);
            Assert.Equal<String>(m_password, decrypted);
        }

        [Fact]
        public void Md5HashIsDifferentFromOriginal()
        {
            String md5hash = Crypto.GetMD5Hash(m_password);
            Assert.NotSame(md5hash, m_password);
        }

        [Fact]
        public void Md5HashOfTwoGivenValues()
        {
            String md5HashFromUser = Crypto.GetMD5Hash("Password");
            String md5HashFromData = Crypto.GetMD5Hash(m_password);
            Assert.Equal<String>(md5HashFromUser, md5HashFromData);
        }
    }
}