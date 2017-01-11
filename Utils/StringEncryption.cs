using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Simple string encryption class to encrypt and decrypt passwords
  /// </summary>
  public static class StringEncryption
  {
    /// <summary>
    /// This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
    /// This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
    /// 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
    /// </summary>
    private const string InitVector = "98Tgyxjgh4FbmnQp";

    /// <summary>
    /// This constant is used to determine the keysize of the encryption algorithm.
    /// </summary>
    private const int Keysize = 256;


    /// <summary>
    /// Encrypt a plain string
    /// </summary>
    /// <param name="plainText">Plaintext</param>
    /// <param name="passPhrase">Passphrase</param>
    /// <returns>The encrypt string</returns>
    public static string Encrypt (string plainText, string passPhrase)
    {
      byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
      byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
      byte[] keyBytes = password.GetBytes(Keysize / 8);
      RijndaelManaged symmetricKey = new RijndaelManaged
      {
        Mode = CipherMode.CBC
      };
      ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
      MemoryStream memoryStream = new MemoryStream();
      CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
      cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
      cryptoStream.FlushFinalBlock();
      byte[] cipherTextBytes = memoryStream.ToArray();

      memoryStream.Close();
      cryptoStream.Close();

      return (Convert.ToBase64String(cipherTextBytes));
    }

    /// <summary>
    /// Decrypt a cipher string
    /// </summary>
    /// <param name="cipherText">Cipher text</param>
    /// <param name="passPhrase">Passphrase</param>
    /// <returns>The plain string</returns>
    public static string Decrypt (string cipherText, string passPhrase)
    {
      byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
      byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
      PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
      byte[] keyBytes = password.GetBytes(Keysize / 8);
      RijndaelManaged symmetricKey = new RijndaelManaged
      {
        Mode = CipherMode.CBC
      };
      ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
      MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
      CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
      byte[] plainTextBytes = new byte[cipherTextBytes.Length];
      int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
      memoryStream.Close();
      cryptoStream.Close();

      return (Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount));
    }
  }
}
