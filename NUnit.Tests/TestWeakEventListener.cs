using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using NUnit.Framework;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestWeakEventListener
  {
    [Test]
    public void TestWeakEventObservableCollection()
    {
      ObservableCollection<string> testColleciton = new ObservableCollection<string>();
      var testChangedListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(HandleChanging);
      CollectionChangedEventManager.AddListener(testColleciton, testChangedListener);

      testColleciton.Add("Test");
    }

    [Test]
    public void TestWeakPropertyChangedEventManager()
    {
      var myTest = new MyTestClass
      {
        IsFocused = false
      };
      PropertyChangedEventManager.AddHandler(myTest, HandlePropertyChanging, "IsFocused");
      myTest.IsFocused = true;
    }

    private void HandleChanging(object sender, EventArgs e)
    {
      Assert.IsInstanceOf<ObservableCollection<string>>(sender);
      Assert.IsInstanceOf<NotifyCollectionChangedEventArgs>(e);

      Assert.AreEqual(1, ((ObservableCollection<string>) sender).Count);
    }

    private void HandlePropertyChanging(object sender, PropertyChangedEventArgs e)
    {
      Assert.IsInstanceOf<MyTestClass>(sender);
      Assert.AreEqual("IsFocused", e.PropertyName);
      Assert.IsTrue(((MyTestClass) sender).IsFocused);
    }

    private class MyTestClass : NotifyMaster
    {
      private bool _isFocused;

      public bool IsFocused
      {
        get => _isFocused;
        set
        {
          _isFocused = value;
          OnPropertyChanged(nameof(IsFocused));
        }
      }
    }
  }
}
