using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Org.Vs.TailForWin.BaseView.ViewModels;

namespace Org.Vs.NUnit.Tests.Options
{
  [TestFixture]
  public class TestTreeNodeViewModel
  {
    [SetUp]
    protected void SetUp()
    {
    }

    [Test]
    public void TestMyTreeNodeViewModel()
    {
      var general = new GeneralOptions();

      var optionPage1 = new TreeNodeViewModel(general.PageTitle, new[]
      {
        new TreeNodeViewModel(general),
        new TreeNodeViewModel(new GeneralProxyOptions()),
        new TreeNodeViewModel(new GeneralResetOptions())
      });

      var extras = new ExtraOptions();

      var optionPage2 = new TreeNodeViewModel(extras.PageTitle, new[]
      {
        new TreeNodeViewModel(extras),
        new TreeNodeViewModel(new ExtraSmartWatch())

      });

      var aboutOptions = new AboutOptions();

      var optionPage3 = new TreeNodeViewModel(aboutOptions.PageTitle, new[]
      {
        new TreeNodeViewModel(new AboutSystemInformations()),
        new TreeNodeViewModel(new AboutUpdate())
      });

      Assert.AreEqual(3, optionPage1.Children.Count());
      Assert.AreEqual(2, optionPage2.Children.Count());
      Assert.AreEqual(2, optionPage3.Children.Count());

      Assert.AreEqual("Reset current settings", optionPage1.Children.Last().Name);

      var root = new ObservableCollection<TreeNodeViewModel>
      {
        optionPage1,
        optionPage2,
        optionPage3
      };

      Assert.AreEqual(3, root.Count);
      Assert.IsTrue(optionPage3.Children.SingleOrDefault(p => p.Name.Equals("System information"))?.IsLeaf);
      Assert.IsFalse(optionPage1.IsLeaf);
    }
  }
}
