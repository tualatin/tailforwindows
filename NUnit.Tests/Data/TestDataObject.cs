using System;
using System.Collections.Generic;


namespace Org.Vs.NUnit.Tests.Data
{
  public class TestDataObject
  {
    public TestDataObject()
    {
      TestList = new List<string>();
    }

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

    public List<string> TestList
    {
      get;
    }
  }
}
