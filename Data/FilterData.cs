using System;
using System.Drawing;


namespace TailForWin.Data
{
  public class FilterData : INotifyMaster, IDisposable
  {
    public void Dispose()
    {
      if (filterFontType == null)
        return;

      filterFontType.Dispose();
      filterFontType = null;
    }

    /// <summary>
    /// ID filter
    /// </summary>
    public int Id
    {
      get;
      set;
    }

    private string description;

    /// <summary>
    /// Filter description
    /// </summary>
    public string Description
    {
      get
      {
        return (description);
      }
      set
      {
        description = value;
        OnPropertyChanged("Description");
      }
    }

    private string filter;

    /// <summary>
    /// Filter
    /// </summary>
    public string Filter
    {
      get
      {
        return (filter);
      }
      set
      {
        filter = value;
        OnPropertyChanged("Filter");
      }
    }

    private Color filterColor;

    /// <summary>
    /// Color of filter match
    /// </summary>
    public Color FilterColor
    {
      get
      {
        return (filterColor);
      }
      set
      {
        filterColor = value;
        OnPropertyChanged("FilterColor");
      }
    }

    private Font filterFontType;

    /// <summary>
    /// Font type
    /// </summary>
    public Font FilterFontType
    {
      get
      {
        return (filterFontType);
      }
      set
      {
        filterFontType = value;
        OnPropertyChanged("FilterFontType");
      }
    }

    /// <summary>
    /// Save data to memenento
    /// </summary>
    /// <returns>Copy of FilterData</returns>
    public MementoFilterData SaveToMemento()
    {
      return (new MementoFilterData(this));
    }

    /// <summary>
    /// Create copy of object
    /// </summary>
    /// <returns>A clone of object</returns>
    public FilterData Clone()
    {
      return (MemberwiseClone() as FilterData);
    }

    /// <summary>
    /// Equals two objects
    /// </summary>
    /// <param name="obj">Reference of FilterData</param>
    /// <returns>If equal true otherwise false</returns>
    public bool EqualsProperties(object obj)
    {
      MementoFilterData other = obj as MementoFilterData;

      if (other == null)
        return (false);

      bool equal = true;

      equal &= Equals(other.Id, Id);
      equal &= Equals(other.Filter, Filter);
      equal &= Equals(other.Description, Description);
      equal &= Equals(other.FilterColor, FilterColor);
      equal &= Equals(other.FilterFontType, FilterFontType);

      return (equal);
    }

    /// <summary>
    /// Roll object back to the state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    public void RestoreFromMemento(MementoFilterData memento)
    {
      MementoFilterData mementoFilterData = memento;

      if (mementoFilterData == null)
        throw new ArgumentException(string.Format("Argument is not a MementoFilterData {0}!", memento.GetType().Name), "memento");

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

      #region Memento properties

      /// <summary>
      /// ID filter
      /// </summary>
      public int Id
      {
        get;
        private set;
      }

      /// <summary>
      /// Filter name
      /// </summary>
      public string Filter
      {
        get;
        private set;
      }

      /// <summary>
      /// Description
      /// </summary>
      public string Description
      {
        get;
        private set;
      }

      /// <summary>
      /// Filter color
      /// </summary>
      public Color FilterColor
      {
        get;
        private set;
      }

      /// <summary>
      /// Filter font type
      /// </summary>
      public Font FilterFontType
      {
        get;
        private set;
      }

      #endregion
    }
  }
}
