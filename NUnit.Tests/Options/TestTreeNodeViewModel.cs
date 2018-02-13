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

      var optionPage1 = new TreeNodeOptionViewModel(general, new[]
      {
        new TreeNodeOptionViewModel(general),
        new TreeNodeOptionViewModel(new GeneralProxyOptions()),
        new TreeNodeOptionViewModel(new GeneralResetOptions())
      });

      var extras = new ExtraOptions();

      var optionPage2 = new TreeNodeOptionViewModel(extras, new[]
      {
        new TreeNodeOptionViewModel(extras),
        new TreeNodeOptionViewModel(new ExtraSmartWatch())

      });

      var aboutOptions = new AboutOptions();

      var optionPage3 = new TreeNodeOptionViewModel(aboutOptions, new[]
      {
        new TreeNodeOptionViewModel(new AboutSystemInformations()),
        new TreeNodeOptionViewModel(new AboutUpdate())
      });

      Assert.AreEqual(3, optionPage1.Children.Count());
      Assert.AreEqual(2, optionPage2.Children.Count());
      Assert.AreEqual(2, optionPage3.Children.Count());

      Assert.AreEqual("Reset current settings", optionPage1.Children.Last().Name);

      var root = new ObservableCollection<TreeNodeOptionViewModel>
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
