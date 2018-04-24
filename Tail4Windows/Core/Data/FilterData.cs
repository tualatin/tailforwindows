using System;
using System.ComponentModel;
using System.Drawing;
using Org.Vs.TailForWin.Core.Data.Settings;
using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;

using FontStyle = System.Drawing.FontStyle;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Filter data object
  /// </summary>
  public class FilterData : StateManager, IDisposable, IDataErrorInfo, ICloneable
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FilterData()
    {
      Id = Guid.NewGuid();
      FilterFontType = new Font("Segoe UI", 11f, FontStyle.Regular);
      FilterColorHex = DefaultEnvironmentSettings.FilterFontColor;
    }

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
        if ( Equals(value, _description) )
          return;

        string currentValue = _description;
        ChangeState(new Command(() => _description = value, () => _description = currentValue, nameof(Description), Notification));
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
        if ( Equals(value, _filter) )
          return;

        string currentValue = _filter;
        ChangeState(new Command(() => _filter = value, () => _filter = currentValue, nameof(Filter), Notification));
      }
    }

    private string _filterColorHex;

    /// <summary>
    /// Font foreground color
    /// </summary>
    public string FilterColorHex
    {
      get => _filterColorHex;
      set
      {
        if ( Equals(value, _filterColorHex) )
          return;

        string currentValue = _filterColorHex;
        ChangeState(new Command(() => _filterColorHex = value, () => _filterColorHex = currentValue, nameof(FilterColorHex), Notification));
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
        var currentValue = _filterFontType;
        ChangeState(new Command(() => _filterFontType = value, () => _filterFontType = currentValue, nameof(FilterFontType), Notification));
      }
    }

    private void Notification(string propertyName) => OnPropertyChanged(propertyName);

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
    /// Creates a shallow copy of the current Object.
    /// </summary>
    /// <returns>A shallow copy of the current Object.</returns>
    public object Clone() => MemberwiseClone();
  }
}
