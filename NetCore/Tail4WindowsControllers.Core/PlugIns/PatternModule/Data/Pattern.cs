using Org.Vs.Tail4Win.Core.Data.Base;

namespace Org.Vs.Tail4Win.Controllers.PlugIns.PatternModule.Data
{
  /// <summary>
  /// Pattern container
  /// </summary>
  public class PatternData : NotifyMaster
  {
    private bool _isRegex;

    /// <summary>
    /// Is regex pattern
    /// </summary>
    public bool IsRegex
    {
      get => _isRegex;
      set
      {
        _isRegex = value;
        OnPropertyChanged(nameof(IsRegex));
      }
    }

    private string _patternString;

    /// <summary>
    /// Pattern
    /// </summary>
    public string PatternString
    {
      get => _patternString;
      set
      {
        _patternString = value;
        OnPropertyChanged(nameof(PatternString));
      }
    }
  }
}
