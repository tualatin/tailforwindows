using System;
using System.ComponentModel;
using System.Drawing;
using Org.Vs.TailForWin.Core.Data.Base;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Filter data object
  /// </summary>
  public partial class FilterData : NotifyMaster, IDisposable, IDataErrorInfo, ICloneable
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public FilterData()
    {
      Id = Guid.NewGuid();
      FilterFontType = new Font("Segoe UI", 11f, FontStyle.Regular);
      FilterColor = System.Windows.Media.Brushes.Crimson;
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

    private System.Windows.Media.Brush _filterColor;

    /// <summary>
    /// Color of filter match
    /// </summary>
    public System.Windows.Media.Brush FilterColor
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
    /// Creates a shallow copy of the current Object.
    /// </summary>
    /// <returns>A shallow copy of the current Object.</returns>
    public object Clone() => MemberwiseClone();
  }
}
