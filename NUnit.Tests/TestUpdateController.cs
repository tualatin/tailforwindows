using System;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Interfaces;


namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestUpdateController
  {
    private IUpdater _updateController;

    [SetUp]
    protected void SetUp()
    {
      var webController = new WebDataController();
      _updateController = new UpdateController(webController);

      if ( Application.Current == null )
        Application.LoadComponent(new Uri("/T4W;component/app.xaml", UriKind.Relative));
    }

    [Test]
    public async Task TestGetUpdateStringFromGitAsync()
    {
      var shouldUpdate = await _updateController.UpdateNecessaryAsync().ConfigureAwait(false);

      Assert.IsInstanceOf<bool>(shouldUpdate);
    }
  }
}
