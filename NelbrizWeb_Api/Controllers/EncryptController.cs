using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptController : ControllerBase
    {

        //private readonly ICryptoTransform _cryptoTransform;
        private readonly ILogger<EncryptController> _logger;
        public EncryptController(ILogger<EncryptController> logger)
        {
              _logger = logger;
        }




        [HttpGet("Encrypt")]
        public async Task<IActionResult> Encrypt(string plaintext)
        {
            //Below is the key. 
            string keyString = "ThisIsASecretKey1234567890";  
            byte[] key = GenerateKey(keyString);

            //Below is the iv. Gotten from an IV generator enpoint below.
            string base64String = "E6bq398ehjA=";
            byte[] iv = Convert.FromBase64String(base64String); 

            byte[] ciphertext = Encrypt(plaintext, key, iv);
            _logger.LogInformation("This is the Encrypted value:  " + Convert.ToBase64String(ciphertext));
           
            return Ok(ciphertext);
        }




        [HttpGet("IV")]
        public async Task<IActionResult> GenerateIV()
        {
            byte[] iv = new byte[8]; // 8 bytes for TripleDES --IV
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            return Ok(iv);
        }




        [HttpPost("Decrypt")]
        public async Task<IActionResult> Decrypt([FromBody] byte[] ciphertext)
        {

            string keyString = "ThisIsASecretKey1234567890";
            byte[] key = GenerateKey(keyString);

            string base64String = "E6bq398ehjA=";
            byte[] iv = Convert.FromBase64String(base64String);

            string decrypted = Decrypt(ciphertext, key, iv);
            _logger.LogInformation("Decrypted: " + decrypted);

            return Ok(decrypted);
        }
























        #region Encryption methods

        public static byte[] Encrypt(string plaintext, byte[] key, byte[] iv)
        {
            using (TripleDESCryptoServiceProvider tripleDes = new())
            {
                tripleDes.Key = key;
                tripleDes.IV = iv;

                ICryptoTransform encryptor = tripleDes.CreateEncryptor(tripleDes.Key, tripleDes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plaintext);
                        }
                    }
                    return msEncrypt.ToArray();
                }
            }
        }





        public static string Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using (TripleDESCryptoServiceProvider tripleDes = new())
            {
                tripleDes.Key = key;
                tripleDes.IV = iv;

                ICryptoTransform decryptor = tripleDes.CreateDecryptor(tripleDes.Key, tripleDes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }




        private byte[] GenerateKey(string keyString)
        {
            // Convert the key string to a byte array
            byte[] keyBytes = Encoding.UTF8.GetBytes(keyString);

            // Trim or expand the key to match the required size (24 bytes for TripleDES)
            byte[] validKeyBytes = new byte[24];
            Array.Copy(keyBytes, validKeyBytes, Math.Min(keyBytes.Length, validKeyBytes.Length));

            return validKeyBytes;
        }

        #endregion



    }
}
