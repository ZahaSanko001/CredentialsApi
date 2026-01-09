using System.Security.Cryptography;
using System.Text;

namespace CredentialsApi.Services
{
 //   public interface IEncryptionService
 //   {
 //       string Encrypt(string plainText);
 //       string Decrypt(string cipherText);
 //   }

    public class EncryptionService
    {
        private readonly byte[] _key;

        public EncryptionService(IConfiguration config)
        {
            var keyBase64 = Environment.GetEnvironmentVariable("APP_ENC_KEY");
            if (string.IsNullOrEmpty(keyBase64)) throw new Exception("APP_ENC_KEY env var is missing");
            _key = Convert.FromBase64String(keyBase64);
        }

        public string Encrypt(string plainText)
        {
            using var aes = new AesGcm(_key);

            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherText = new byte[plaintextBytes.Length];
            byte[] tag = new byte[16];

            aes.Encrypt(nonce, plaintextBytes, cipherText, tag);

            byte[] result = new byte[nonce.Length + tag.Length + cipherText.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encoded)
        {
            byte[] fullCipher = Convert.FromBase64String(encoded);
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] cyphertext = new byte[fullCipher.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(fullCipher, nonce.Length + tag.Length, cyphertext, 0, cyphertext.Length);

            using var aes = new AesGcm(_key);

            byte[] plainTextBytes = new byte[cyphertext.Length];
            aes.Decrypt(nonce, cyphertext, tag, plainTextBytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }
    }
}
