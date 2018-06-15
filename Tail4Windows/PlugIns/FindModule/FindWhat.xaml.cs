using System;
using System.Windows;
using System.Windows.Interop;
using log4net;
using Org.Vs.TailForWin.Core.Native;
using Org.Vs.TailForWin.PlugIns.FindModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.FindModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.FindModule
{
  /// <summary>
  /// Interaction logic for FindDialog.xaml
  /// </summary>
  public partial class FindWhat
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(FindWhat));

    private readonly IFindWhatViewModel _findDialogViewModel;

    /// <summary>
    /// Hotkey F3
    /// </summary>
    private const int HotkeyId = 7000;

    private HwndSource _source;

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
    public FindWhat()
    {
      InitializeComponent();

      Loaded += FindWhatOnLoaded;
      Closing += FindWhatOnClosing;

      _findDialogViewModel = (FindWhatViewModel) DataContext;
    }

    private void FindWhatOnLoaded(object sender, RoutedEventArgs e)
    {
      var helper = new WindowInteropHelper(this);
      _source = HwndSource.FromHwnd(helper.Handle);

      _source?.AddHook(HwndHook);
      RegisterHotKey();
    }

    private void FindWhatOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ( _source == null )
        return;

      _source.RemoveHook(HwndHook);
      _source = null;

      UnregisterHotKey();
    }

    #region HelperFunctions

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      const int wmHotkey = 0x0312;

      if ( msg != wmHotkey )
        return IntPtr.Zero;

      if ( wParam.ToInt32() != HotkeyId )
        return IntPtr.Zero;

      if ( _findDialogViewModel == null || !_findDialogViewModel.CanExecuteFindCommand() )
        return IntPtr.Zero;

      _findDialogViewModel.FindNextCommand.ExecuteAsync(null);
      handled = true;

      return IntPtr.Zero;
    }

    private void RegisterHotKey()
    {
      var helper = new WindowInteropHelper(this);
      const uint vkF3 = 0x72;

      if ( !NativeMethods.RegisterHotKey(helper.Handle, HotkeyId, 0, vkF3) )
        LOG.Error("{0} can not register hotkey", System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    private void UnregisterHotKey()
    {
      var helper = new WindowInteropHelper(this);
      NativeMethods.UnregisterHotKey(helper.Handle, HotkeyId);
    }

    #endregion
  }
}
