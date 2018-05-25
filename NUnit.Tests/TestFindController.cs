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
      _textWorker = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.";
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
      bool result = await _findController.MatchTextAsync(settings, _textWorker, "lorem*", _cts.Token).ConfigureAwait(false);
      Assert.IsFalse(result);

      settings.CaseSensitive = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, "*ip??m*", _cts.Token).ConfigureAwait(false);
      Assert.IsTrue(result);

      settings.WholeWord = true;
      settings.UseWildcard = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, "sed", _cts.Token).ConfigureAwait(false);
      Assert.IsTrue(result);

      settings.CaseSensitive = true;
      result = await _findController.MatchTextAsync(settings, _textWorker, "Diam", _cts.Token).ConfigureAwait(false);
      Assert.IsFalse(result);

      settings.CaseSensitive = false;
      settings.UseWildcard = false;
      settings.UseRegex = true;
      settings.WholeWord = false;
      result = await _findController.MatchTextAsync(settings, _textWorker, @"\w*[ip]", _cts.Token).ConfigureAwait(false);
      Assert.IsTrue(result);

      settings.WholeWord = true;
      settings.UseRegex = true;
      result = await _findController.MatchTextAsync(settings, _textWorker, "sed|invidunt", _cts.Token).ConfigureAwait(false);
      Assert.IsTrue(result);
    }

    private void InitTokenSource()
    {
      _cts?.Dispose();
      _cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
    }
  }
}
