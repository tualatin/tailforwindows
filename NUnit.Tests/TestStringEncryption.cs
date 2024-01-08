using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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

      ClassicAssert.IsInstanceOf<string>(encryptedData);
      ClassicAssert.IsNotNull(encryptedData);
      ClassicAssert.AreNotEqual(message, encryptedData);

      string decryptedData = await StringEncryption.DecryptAsync(encryptedData).ConfigureAwait(false);

      ClassicAssert.IsInstanceOf<string>(decryptedData);
      ClassicAssert.AreEqual(message, decryptedData);
    }
  }
}
