using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestStringEncryption
  {
    [Test]
    public async Task TestEncryptDecryptStringAsync()
    {
      const string message = "blablabla";
      string encryptedData = await StringEncryption.EncryptAsync(message).ConfigureAwait(false);

      Assert.IsInstanceOf<string>(encryptedData);
      Assert.IsNotNull(encryptedData);
      Assert.AreNotEqual(message, encryptedData);

      string decryptedData = await StringEncryption.DecryptAsync(encryptedData).ConfigureAwait(false);

      Assert.IsInstanceOf<string>(decryptedData);
      Assert.AreEqual(message, decryptedData);
    }
  }
}
