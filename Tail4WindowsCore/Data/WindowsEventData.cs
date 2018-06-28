using Org.Vs.TailForWin.Core.Utils.UndoRedoManager;


namespace Org.Vs.TailForWin.Core.Data
{
  /// <summary>
  /// Windows event data
  /// </summary>
  public class WindowsEventData : StateManager
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public WindowsEventData() => Machine = ".";

    private string _category;

    /// <summary>
    /// Windows event category
    /// </summary>
    public string Category
    {
      get => _category;
      set
      {
        if ( Equals(value, _category) )
          return;

        string currentValue = _category;
        ChangeState(new Command(() => _category = value, () => _category = currentValue, nameof(Category), Notification));
      }
    }

    private string _name;

    /// <summary>
    /// Name
    /// </summary>
    public string Name
    {
      get => _name;
      set
      {
        if (Equals(value, _name))
          return;

        _name = value;
        OnPropertyChanged();
      }
    }

    private string _machine;

    /// <summary>
    /// Machine
    /// </summary>
    public string Machine
    {
      get => _machine;
      set
      {
        if ( Equals(value, _machine) )
          return;

        string currentValue = _machine;
        ChangeState(new Command(() => _machine = value, () => _machine = currentValue, nameof(Machine), Notification));
      }
    }

    private string _userName;

    /// <summary>
    /// Username of the remote machine
    /// </summary>
    public string UserName
    {
      get => _userName;
      set
      {
        if ( Equals(value, _userName) )
          return;

        string currentValue = _userName;
        ChangeState(new Command(() => _userName = value, () => _userName = currentValue, nameof(UserName), Notification));
      }
    }
  }
}
