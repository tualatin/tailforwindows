using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Org.Vs.TailForWin.PlugIns.LogWindowModule
{
  /// <summary>
  /// Interaction logic for LogWindow.xaml
  /// </summary>
  public partial class LogWindow : INotifyPropertyChanged
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public LogWindow()
    {
      InitializeComponent();
    }

    private string _tabItemBackgroundColorHex;

    /// <summary>
    /// TabItem background color as string
    /// </summary>
    public string TabItemBackgroundColorHex
    {
      get => _tabItemBackgroundColorHex;
      set
      {
        if ( Equals(value, _tabItemBackgroundColorHex) )
          return;

        _tabItemBackgroundColorHex = value;
        OnPropertyChanged();
      }
    }

    /// <summary>
    /// Declare the event
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;


    /// <summary>
    /// OnPropertyChanged
    /// </summary>
    /// <param name="name">Name of property</param>
    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
  }
}
