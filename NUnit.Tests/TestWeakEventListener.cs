using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using NUnit.Framework;
using NUnit.Framework.Legacy;
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
      ObservableCollection<string> testCollection = new ObservableCollection<string>();
      var testChangedListener = new WeakEventListener<NotifyCollectionChangedEventArgs>(HandleChanging);
      CollectionChangedEventManager.AddListener(testCollection, testChangedListener);

      testCollection.Add("Test");
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
      ClassicAssert.IsInstanceOf<ObservableCollection<string>>(sender);
      ClassicAssert.IsInstanceOf<NotifyCollectionChangedEventArgs>(e);

      ClassicAssert.AreEqual(1, ((ObservableCollection<string>) sender).Count);
    }

    private void HandlePropertyChanging(object sender, PropertyChangedEventArgs e)
    {
      ClassicAssert.IsInstanceOf<MyTestClass>(sender);
      ClassicAssert.AreEqual("IsFocused", e.PropertyName);
      ClassicAssert.IsTrue(((MyTestClass) sender).IsFocused);
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
          OnPropertyChanged();
        }
      }
    }
  }
}
