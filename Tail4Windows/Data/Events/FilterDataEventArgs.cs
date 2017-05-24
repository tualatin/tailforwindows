using System;


namespace Org.Vs.TailForWin.Data.Events
{
  /// <summary>
  /// FilterDataEventArgs object
  /// </summary>
  public class FilterDataEventArgs : EventArgs, IDisposable
  {
    private FilterData filterData;


    /// <summary>
    /// Releases all resources used by the FilterDataEventArgs.
    /// </summary>
    public void Dispose()
    {
      if(filterData == null)
        return;

      filterData.Dispose();
      filterData = null;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="filterData">FilterData object</param>
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
      return filterData;
    }
  }
}
