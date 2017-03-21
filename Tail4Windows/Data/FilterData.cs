using System;
using System.ComponentModel;
using System.Drawing;
using Org.Vs.TailForWin.Data.Base;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// FilterData object
  /// </summary>
  public class FilterData : INotifyMaster, IDisposable, IDataErrorInfo, ICloneable
  {
    /// <summary>
    /// Releases all resources used by the FilterData.
    /// </summary>
    public void Dispose()
    {
      if(filterFontType == null)
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

    #region IDataErrorInfo interface

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    public string Error => throw new NotImplementedException();

    /// <summary>
    /// Gets the error message for the property with the given name.
    /// </summary>
    /// <param name="columnName">Name of column</param>
    /// <returns>Current error result</returns>
    public string this[string columnName]
    {
      get
      {
        string result = null;

        if(columnName == "Description")
        {
          if(string.IsNullOrEmpty(Description))
            result = "Please enter a Description";
        }
        if(columnName == "Filter")
        {
          if(string.IsNullOrEmpty(Filter))
            result = "Please enter a Filterpattern";
        }
        return (result);
      }
    }

    #endregion

    /// <summary>
    /// Save data to memenento
    /// </summary>
    /// <returns>Copy of FilterData</returns>
    public MementoFilterData SaveToMemento()
    {
      return (new MementoFilterData(this));
    }

    /// <summary>
    /// Equals two objects
    /// </summary>
    /// <param name="obj">Reference of FilterData</param>
    /// <returns>If equal true otherwise false</returns>
    public bool EqualsProperties(object obj)
    {
      MementoFilterData other = obj as MementoFilterData;

      if(other == null)
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

      if(mementoFilterData == null)
        throw new ArgumentException(string.Format("Argument is not a MementoFilterData {0}!", memento.GetType().Name), "memento");

      Id = memento.Id;
      Filter = memento.Filter;
      Description = memento.Description;
      FilterFontType = memento.FilterFontType;
      FilterColor = memento.FilterColor;

    }

    /// <summary>
    /// Creates a shallow copy of the current Object.
    /// </summary>
    /// <returns>A shallow copy of the current Object.</returns>
    public object Clone()
    {
      return (MemberwiseClone());
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
