using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.PlugIns.FontChooserModule
{
  /// <summary>
  /// Interaction logic for FontChooseDialog.xaml
  /// </summary>
  public partial class FontChooseDialog
  {

    private FontInfo _selectedFont;

    /// <summary>
    /// SelectedFont <see cref="FontInfo"/>
    /// </summary>
    public FontInfo SelectedFont
    {
      get => _selectedFont;
      set
      {
        if ( Equals(value, _selectedFont) )
          return;

        _selectedFont = value;
      }
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public FontChooseDialog()
    {
      InitializeComponent();

      DataContext = this;
    }

    #region Commands

    private ICommand _loadedCommand;

    /// <summary>
    /// Loaded command
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(p => ExecuteLoadedCommand()));

    private ICommand _openCommand;

    /// <summary>
    /// Open command
    /// </summary>
    public ICommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(p => ExecuteOpenCommand((Window) p)));

    private ICommand _cancelCommand;

    /// <summary>
    /// Cancel command
    /// </summary>
    public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(p => ExecuteCancelCommand((Window) p)));

    #endregion

    #region command functions

    private void ExecuteLoadedCommand()
    {
      SyncFontName();
      SyncFontSize();
      SyncFontTypeface();
    }

    private void ExecuteOpenCommand(Window window)
    {
      SelectedFont = FontChooser.SelectedFont;
      DialogResult = true;
      window.Close();
    }

    private void ExecuteCancelCommand(Window window)
    {
      SelectedFont = null;
      DialogResult = false;

      window.Close();
    }

    #endregion

    private void SyncFontName()
    {
      string fontFamilyName = SelectedFont.FontType.FontFamily.Source;
      int idx = 0;

      foreach ( var item in FontChooser.LstFamily.Items )
      {
        string itemName = item.ToString();

        if ( fontFamilyName == itemName )
          break;

        idx++;
      }

      FontChooser.LstFamily.SelectedIndex = idx;
      FontChooser.LstFamily.ScrollIntoView(FontChooser.LstFamily.Items[idx]);
    }

    private void SyncFontSize() => FontChooser.FontSizeSlider.Value = SelectedFont.FontType.FontSize;

    private void SyncFontTypeface()
    {
      string fontTypeFaceSb = FontInfo.TypefaceToString(SelectedFont.Typeface);
      int idx = 0;

      foreach ( var item in FontChooser.LstTypefaces.Items )
      {
        FamilyTypeface face = item as FamilyTypeface;

        if ( fontTypeFaceSb == FontInfo.TypefaceToString(face) )
          break;

        idx++;
      }

      FontChooser.LstTypefaces.SelectedIndex = idx;
    }
  }
}
