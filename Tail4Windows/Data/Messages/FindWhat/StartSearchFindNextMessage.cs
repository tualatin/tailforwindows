using System;
using Org.Vs.TailForWin.Core.Data;


namespace Org.Vs.TailForWin.Data.Messages.FindWhat
{
  /// <summary>
  /// Starts searching find next
  /// </summary>
  public class StartSearchFindNextMessage
  {
    /// <summary>
    /// Which window calls the FindWhat dialog
    /// </summary>
    public Guid WindowGuid
    {
      get;
    }

    /// <summary>
    /// Current <see cref="FindData"/>
    /// </summary>
    public FindData FindData
    {
      get;
    }

    /// <summary>
    /// Search text
    /// </summary>
    public string SearchText
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowGuid">Which window calls the FindWhat dialog</param>
    /// <param name="findData"><see cref="FindData"/></param>
    /// <param name="searchText">Search text</param>
    public StartSearchFindNextMessage(Guid windowGuid, FindData findData, string searchText)
    {
      WindowGuid = windowGuid;
      FindData = findData;
      SearchText = searchText;
    }
  }
}
