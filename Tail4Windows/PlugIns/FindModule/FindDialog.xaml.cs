using System.Windows;


namespace Org.Vs.TailForWin.PlugIns.FindModule
{
  /// <summary>
  /// Interaction logic for FindDialog.xaml
  /// </summary>
  public partial class FindDialog
  {
    #region Properties

    private string _dialogTitle;

    /// <summary>
    /// Current dialog title
    /// </summary>
    public string DialogTitle
    {
      get => _dialogTitle;
      set
      {
        string title = Application.Current.TryFindResource("FindDialogWindowTitle").ToString();
        string noFile = Application.Current.TryFindResource("NoFile").ToString();
        _dialogTitle = value;

        if ( string.IsNullOrWhiteSpace(_dialogTitle) || Equals(_dialogTitle, noFile) )
        {
          Title = title;
          return;
        }

        Title = $"{title} in {DialogTitle}";
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindDialog() => InitializeComponent();
  }
}
