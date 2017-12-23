using System;


namespace Org.Vs.NUnit.Tests.Data
{
  public class TestDataObject : ICloneable
  {
    public Guid Id
    {
      get;
      set;
    }

    public string TestString
    {
      get;
      set;
    }

    public int TestInt
    {
      get;
      set;
    }

    public double TestDouble
    {
      get;
      set;
    }

    public object Clone()
    {
      return MemberwiseClone();
    }
  }
}
