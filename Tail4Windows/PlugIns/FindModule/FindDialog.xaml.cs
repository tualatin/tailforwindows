using System;
using System.Windows;
using Org.Vs.TailForWin.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.FindModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.FindModule
{
  /// <summary>
  /// Interaction logic for FindDialog.xaml
  /// </summary>
  public partial class FindDialog
  {
    private readonly IFindDialogViewModel _findDialogViewModel;

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

    /// <summary>
    /// Search text
    /// </summary>
    public string SearchText
    {
      get => _findDialogViewModel == null ? string.Empty : _findDialogViewModel.SearchText;
      set
      {
        if ( _findDialogViewModel == null )
          return;

        _findDialogViewModel.SearchText = value;
      }
    }

    /// <summary>
    /// Which window call the find dialog
    /// </summary>
    public Guid WindowGuid
    {
      set
      {
        if ( _findDialogViewModel == null )
          return;

        _findDialogViewModel.WindowGuid = value;
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FindDialog()
    {
      InitializeComponent();

      _findDialogViewModel = (FindDialogViewModel) DataContext;
    }
  }
}
