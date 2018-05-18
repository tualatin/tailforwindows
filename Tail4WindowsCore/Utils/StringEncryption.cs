using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using log4net;


namespace Org.Vs.TailForWin.Core.Utils
{
  /// <summary>
  /// Simple string encryption class to encrypt and decrypt passwords
  /// </summary>
  public static class StringEncryption
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(StringEncryption));

    private const string Password = "7#S&,fD0EcJt+%sf~9/Q";
    private static readonly byte[] Salt = { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };


    /// <summary>
    /// Encrypt a plain string
    /// </summary>
    /// <param name="plainText">Plaintext</param>
    /// <returns>The encrypt string</returns>
    public static async Task<string> EncryptAsync(string plainText)
    {
      try
      {
        byte[] bytesBuffer = Encoding.Unicode.GetBytes(plainText);

        using ( var aes = Aes.Create() )
        {
          Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(Password, Salt);

          if ( aes != null )
          {
            aes.Key = k1.GetBytes(32);
            aes.IV = k1.GetBytes(16);

            using ( MemoryStream encryptionStream = new MemoryStream() )
            {
              using ( CryptoStream encrypt = new CryptoStream(encryptionStream, aes.CreateEncryptor(), CryptoStreamMode.Write) )
              {
                await encrypt.WriteAsync(bytesBuffer, 0, bytesBuffer.Length).ConfigureAwait(false);
                encrypt.Close();
              }

              return Convert.ToBase64String(encryptionStream.ToArray());
            }
          }
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
      return null;
    }

    /// <summary>
    /// Decrypt a cipher string
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <returns>The plain string</returns>
    public static async Task<string> DecryptAsync(string cipherText)
    {
      try
      {
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

        using ( var aes = Aes.Create() )
        {
          Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(Password, Salt);

          if ( aes != null )
          {
            aes.Key = k1.GetBytes(32);
            aes.IV = k1.GetBytes(16);

            using ( MemoryStream encryptionStream = new MemoryStream() )
            {
              using ( CryptoStream decrypt = new CryptoStream(encryptionStream, aes.CreateDecryptor(), CryptoStreamMode.Write) )
              {
                await decrypt.WriteAsync(cipherTextBytes, 0, cipherTextBytes.Length).ConfigureAwait(false);
                decrypt.Close();
              }

              return Encoding.Unicode.GetString(encryptionStream.ToArray());
            }
          }
        }
      }
      catch ( Exception ex )
      {
        LOG.Error(ex, "{0} caused a(n) {1}", ex.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
      }
      return null;
    }
  }
}
