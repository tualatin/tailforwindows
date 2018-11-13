using System;
using NUnit.Framework;
using Org.Vs.TailForWin.Business.StatisticEngine.Data;
using Org.Vs.TailForWin.Business.StatisticEngine.DbScheme;
using Org.Vs.TailForWin.Business.StatisticEngine.Interfaces;

namespace Org.Vs.NUnit.Tests
{
  [TestFixture]
  public class TestStatisticAnalysisCollection
  {
    private IStatisticAnalysisCollection<StatisticAnalysisData> _statisticAnalysisCollection;
    private int _index;

    [SetUp]
    protected void SetUp() => _statisticAnalysisCollection = new StatisticAnalysisCollection();

    [Test]
    public void TestClearCollection()
    {
      GenerateList();

      Assert.DoesNotThrow(() => _statisticAnalysisCollection.Clear());
      Assert.AreEqual(0, _statisticAnalysisCollection.Count);
    }

    [Test]
    public void TestCurrentInCollection()
    {
      GenerateList();
      Assert.IsTrue(_statisticAnalysisCollection.MoveNext());

      var current = _statisticAnalysisCollection.Current;
      Assert.IsNotNull(current);
      Assert.IsInstanceOf<StatisticAnalysisData>(current);
      Assert.AreEqual(0, ((StatisticAnalysisData) current).SessionEntity.SessionId);

      Assert.IsTrue(_statisticAnalysisCollection.MoveNext());

      current = _statisticAnalysisCollection.Current;
      Assert.IsNotNull(current);
      Assert.IsInstanceOf<StatisticAnalysisData>(current);
      Assert.AreEqual(1, ((StatisticAnalysisData) current).SessionEntity.SessionId);
    }

    private void GenerateList()
    {
      _statisticAnalysisCollection.Add(new StatisticAnalysisData
      {
        SessionEntity = new SessionEntity
        {
          SessionId = _index,
          Date = DateTime.Now,
          Session = Guid.NewGuid()
        }
      });

      _index++;

      _statisticAnalysisCollection.Add(new StatisticAnalysisData
      {
        SessionEntity = new SessionEntity
        {
          SessionId = _index,
          Date = DateTime.Now,
          Session = Guid.NewGuid()
        }
      });

      _index++;

      _statisticAnalysisCollection.Add(new StatisticAnalysisData
      {
        SessionEntity = new SessionEntity
        {
          SessionId = _index,
          Date = DateTime.Now,
          Session = Guid.NewGuid()
        }
      });

      _index++;

      _statisticAnalysisCollection.Add(new StatisticAnalysisData
      {
        SessionEntity = new SessionEntity
        {
          SessionId = _index,
          Date = DateTime.Now,
          Session = Guid.NewGuid()
        }
      });

      _index++;

      _statisticAnalysisCollection.Add(new StatisticAnalysisData
      {
        SessionEntity = new SessionEntity
        {
          SessionId = _index,
          Date = DateTime.Now,
          Session = Guid.NewGuid()
        }
      });

      _index++;

      _statisticAnalysisCollection.Add(new StatisticAnalysisData
      {
        SessionEntity = new SessionEntity
        {
          SessionId = _index,
          Date = DateTime.Now,
          Session = Guid.NewGuid()
        }
      });

      _index++;

      Assert.AreEqual(_index, _statisticAnalysisCollection.Count);
    }
  }
}
