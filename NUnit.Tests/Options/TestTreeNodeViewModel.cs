﻿using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
        new TreeNodeOptionViewModel(general, null),
        new TreeNodeOptionViewModel(new GeneralProxyOptions(), null),
        new TreeNodeOptionViewModel(new GeneralResetOptions(), null)
      }, null);

      var extras = new ExtraOptions();

      var optionPage2 = new TreeNodeOptionViewModel(extras, new[]
      {
        new TreeNodeOptionViewModel(extras, null),
        new TreeNodeOptionViewModel(new ExtraSmartWatch(), null)

      }, null);

      var aboutOptions = new AboutOptions();

      var optionPage3 = new TreeNodeOptionViewModel(aboutOptions, new[]
      {
        new TreeNodeOptionViewModel(new AboutSystemInformations(), null),
        new TreeNodeOptionViewModel(new AboutUpdate(), null)
      }, null);

      ClassicAssert.AreEqual(3, optionPage1.Children.Count());
      ClassicAssert.AreEqual(2, optionPage2.Children.Count());
      ClassicAssert.AreEqual(2, optionPage3.Children.Count());

      ClassicAssert.AreEqual("Reset current settings", optionPage1.Children.Last().Name);

      var root = new ObservableCollection<TreeNodeOptionViewModel>
      {
        optionPage1,
        optionPage2,
        optionPage3
      };

      ClassicAssert.AreEqual(3, root.Count);
      ClassicAssert.IsTrue(optionPage3.Children.SingleOrDefault(p => p.Name.Equals("System information"))?.IsLeaf);
      ClassicAssert.IsFalse(optionPage1.IsLeaf);
    }
  }
}
