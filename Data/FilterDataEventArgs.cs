using System;


namespace Org.Vs.TailForWin.Data
{
  public class FilterDataEventArgs : EventArgs, IDisposable
  {
    private FilterData filterData;


    public void Dispose()
    {
      if (filterData == null)
        return;

      filterData.Dispose();
      filterData = null;
    }

    public FilterDataEventArgs(FilterData filterData)
    {
      this.filterData = filterData;
    }

    /// <summary>
    /// Get FilterData data
    /// </summary>
    /// <returns>FilterData object</returns>
    public FilterData GetData()
    {
      return (filterData);
    }
  }
}
