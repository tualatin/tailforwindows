using System;


namespace TailForWin.Data
{
  public class FilterDataEventArgs : EventArgs
  {
    private FilterData filterData;


    public FilterDataEventArgs (FilterData filterData)
    {
      this.filterData = filterData;
    }

    /// <summary>
    /// Get FilterData data
    /// </summary>
    /// <returns>FilterData object</returns>
    public FilterData GetData ()
    {
      return (filterData);
    }
  }
}
