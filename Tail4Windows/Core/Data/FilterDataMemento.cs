using System;
using System.Drawing;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Filter data object
  /// </summary>
  public partial class FilterData
  {
    /// <summary>
    /// Save data to memenento
    /// </summary>
    /// <returns>Copy of FilterData</returns>
    public MementoFilterData SaveToMemento()
    {
      return new MementoFilterData(this);
    }

    /// <summary>
    /// Roll object back to the state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    public void RestoreFromMemento(MementoFilterData memento)
    {
      MementoFilterData mementoFilterData = memento;
      Arg.NotNull(mementoFilterData, nameof(mementoFilterData));

      Id = memento.Id;
      Filter = memento.Filter;
      Description = memento.Description;
      FilterFontType = memento.FilterFontType;
      FilterColor = memento.FilterColor;
    }

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class MementoFilterData
    {
      internal MementoFilterData(FilterData obj)
      {
        Id = obj.Id;
        Filter = obj.Filter;
        Description = obj.Description;
        FilterColor = obj.FilterColor;
        FilterFontType = obj.FilterFontType;
      }

      /// <summary>
      /// ID filter
      /// </summary>
      public Guid Id
      {
        get;
      }

      /// <summary>
      /// Filter name
      /// </summary>
      public string Filter
      {
        get;
      }

      /// <summary>
      /// Description
      /// </summary>
      public string Description
      {
        get;
      }

      /// <summary>
      /// Filter color
      /// </summary>
      public Color FilterColor
      {
        get;
      }

      /// <summary>
      /// Filter font type
      /// </summary>
      public Font FilterFontType
      {
        get;
      }
    }
  }
}
