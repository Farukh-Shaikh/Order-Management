using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace OrderManagement.Services
{
    public class CreditCardHandler
    {
        private static readonly byte[] MasterKey = Convert.FromBase64String("YOUR_MASTER_KEY_BASE64"); // Static master key for demonstration purposes

        private static readonly byte[] EncryptedKey = GetStoredEncryptedKey(); // Securely stored encrypted key
        private static readonly byte[] EncryptedIV = GetStoredEncryptedIV(); // Securely stored encrypted IV

        private static byte[] GetStoredEncryptedKey()
        {
            // Retrieve the encrypted key securely from a key management service
            return Convert.FromBase64String("YOUR_STORED_ENCRYPTED_KEY_BASE64");
        }

        private static byte[] GetStoredEncryptedIV()
        {
            // Retrieve the encrypted IV securely from a key management service
            return Convert.FromBase64String("YOUR_STORED_ENCRYPTED_IV_BASE64");
        }

        private static byte[] DecryptKey(byte[] encryptedKey)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = MasterKey;
                aes.IV = new byte[16]; // Initialization vector (IV) can be hardcoded or derived
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(encryptedKey))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return Convert.FromBase64String(sr.ReadToEnd());
                        }
                    }
                }
            }
        }

        private static byte[] Key => DecryptKey(EncryptedKey);
        private static byte[] IV => DecryptKey(EncryptedIV);

        public static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }


    public class TwoFactorAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TwoFactorAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Implement your 2FA logic here
            // For demo purposes, we will assume 2FA is always successful
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "user"), new Claim("MFA", "true"), new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }


}
