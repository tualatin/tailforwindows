using System;
using System.ComponentModel;
using System.Drawing;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Filter data object
  /// </summary>
  public class FilterData : NotifyMaster, IDisposable, IDataErrorInfo, ICloneable
  {
    /// <summary>
    /// Releases all resources used by the FilterData.
    /// </summary>
    public void Dispose()
    {
      if ( _filterFontType == null )
        return;

      _filterFontType.Dispose();
      _filterFontType = null;
    }

    private Guid _id;

    /// <summary>
    /// ID filter
    /// </summary>
    public Guid Id
    {
      get => _id;
      set
      {
        _id = value;
        OnPropertyChanged(nameof(Id));
      }
    }

    private string _description;

    /// <summary>
    /// Filter description
    /// </summary>
    public string Description
    {
      get => _description;
      set
      {
        _description = value;
        OnPropertyChanged(nameof(Description));
      }
    }

    private string _filter;

    /// <summary>
    /// Filter
    /// </summary>
    public string Filter
    {
      get => _filter;
      set
      {
        _filter = value;
        OnPropertyChanged(nameof(Filter));
      }
    }

    private Color _filterColor;

    /// <summary>
    /// Color of filter match
    /// </summary>
    public Color FilterColor
    {
      get => _filterColor;
      set
      {
        _filterColor = value;
        OnPropertyChanged(nameof(FilterColor));
      }
    }

    private Font _filterFontType;

    /// <summary>
    /// Font type
    /// </summary>
    public Font FilterFontType
    {
      get => _filterFontType;
      set
      {
        _filterFontType = value;
        OnPropertyChanged(nameof(FilterFontType));
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

        switch ( columnName )
        {
        case nameof(Description):

          if ( string.IsNullOrEmpty(Description) )
            result = "Please enter a Description";
          break;

        case nameof(Filter):

          if ( string.IsNullOrEmpty(Filter) )
            result = "Please enter a Filterpattern";
          break;
        }
        return result;
      }
    }

    #endregion

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

      if ( mementoFilterData == null )
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
      return MemberwiseClone();
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

      #endregion
    }
  }
}