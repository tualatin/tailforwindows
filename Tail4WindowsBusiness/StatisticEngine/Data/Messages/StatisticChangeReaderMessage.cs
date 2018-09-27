﻿using System;


namespace Org.Vs.TailForWin.Business.StatisticEngine.Data.Messages
{
  /// <summary>
  /// Informs the statistic, that file has changed
  /// </summary>
  public class StatisticChangeReaderMessage
  {
    /// <summary>
    /// LogReader Id
    /// </summary>
    public Guid LogReaderId
    {
      get;
    }

    /// <summary>
    /// Index
    /// </summary>
    public int Index
    {
      get;
    }

    /// <summary>
    /// Name of file with path
    /// </summary>
    public string FileName
    {
      get;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="logReaderId">LogReader Id</param>
    /// <param name="index">Current index</param>
    /// <param name="fileName">Name of file with path</param>
    public StatisticChangeReaderMessage(Guid logReaderId, int index, string fileName)
    {
      LogReaderId = logReaderId;
      Index = index;
      FileName = fileName;
    }
  }
}
