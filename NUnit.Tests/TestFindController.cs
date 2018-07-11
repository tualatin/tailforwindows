using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.Vs.TailForWin.Business.SearchEngine.Controllers;
using Org.Vs.TailForWin.Business.SearchEngine.Interfaces;
using Org.Vs.TailForWin.Core.Data;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestFindController
  {
    private IFindController _findController;
    private string _textWorker;
    private CancellationTokenSource _cts;

    [SetUp]
    protected void SetUp()
    {
      _findController = new FindController();
      _textWorker = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, Sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. Sad";
    }

    [Test]
    public async Task TestSearchPatternAsync()
    {
      InitTokenSource();

      var settings = new FindData
      {
        CaseSensitive = true,
        UseWildcard = true
      };
      var result = await _findController.MatchTextAsync(settings, _textWorker, "lorem*").ConfigureAwait(false);
      Assert.IsFalse(result != null);

      settings.CaseSensitive = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, "ip??m").ConfigureAwait(false);
      Assert.IsTrue(result.Count > 0);
      Assert.AreEqual("ipsum", string.Join("|", result));

      settings.WholeWord = true;
      settings.UseWildcard = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, "sed").ConfigureAwait(false);
      Assert.IsTrue(result.Count > 0);
      Assert.AreEqual("Sed|sed", string.Join("|", result));

      settings.CaseSensitive = true;
      result = await _findController.MatchTextAsync(settings, _textWorker, "Diam").ConfigureAwait(false);
      Assert.AreEqual(0, result.Count);

      settings.CaseSensitive = false;
      settings.UseWildcard = false;
      settings.UseRegex = true;
      settings.WholeWord = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, @"\w*[ip]").ConfigureAwait(false);
      Assert.IsTrue(result.Count > 0);

      settings.WholeWord = true;
      settings.UseRegex = true;
      result = await _findController.MatchTextAsync(settings, _textWorker, "sed|invidunt").ConfigureAwait(false);
      Assert.IsTrue(result.Count > 0);
      Assert.IsTrue(result.Count == 3);
      Assert.IsTrue(result.Contains("sed"));
      Assert.IsFalse(result.Contains("lorem"));

      _textWorker = "TiCon4.exe Information: 0 : Bootstrapper sequence completed";

      settings.WholeWord = true;
      settings.CaseSensitive = false;
      settings.UseRegex = false;
      settings.UseWildcard = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, "Info").ConfigureAwait(false);

      Assert.AreEqual(0, result.Count);

      _textWorker = "ShellWindow with a shell and a shellnut";

      settings.WholeWord = false;
      settings.CaseSensitive = false;
      settings.UseRegex = false;
      settings.UseWildcard = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, "shell").ConfigureAwait(false);

      Assert.AreEqual(3, result.Count);
      Assert.IsTrue(result.Contains("ShellWindow"));
      Assert.IsFalse(result.Contains("with"));
    }

    private void InitTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
