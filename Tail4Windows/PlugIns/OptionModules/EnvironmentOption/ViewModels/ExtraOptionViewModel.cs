using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Org.Vs.TailForWin.Business.Utils;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.Core.Controllers;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Extra option view model
  /// </summary>
  public class ExtraOptionViewModel : NotifyMaster, IExtraOptionViewModel
  {
    #region Properties

    private ImageSource _iconSource;

    /// <summary>
    /// ImageSource
    /// </summary>
    public ImageSource IconSource
    {
      get => _iconSource;
      set
      {
        _iconSource = value;
        OnPropertyChanged();
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public ExtraOptionViewModel()
    {
      if ( !File.Exists(SettingsHelperController.CurrentSettings.EditorPath) && !string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.EditorPath) )
        SettingsHelperController.CurrentSettings.EditorPath = string.Empty;

      IconSource = string.IsNullOrWhiteSpace(SettingsHelperController.CurrentSettings.EditorPath) ?
        BusinessHelper.CreateBitmapIcon("/T4W;component/Resources/notepad.ico") :
        BusinessHelper.GetAssemblyIcon(SettingsHelperController.CurrentSettings.EditorPath);
    }

    #region Commands

    private ICommand _selectEditorCommand;

    /// <summary>
    /// Selected editor command
    /// </summary>
    public ICommand SelectEditorCommand => _selectEditorCommand ?? (_selectEditorCommand = new RelayCommand(p => ExecuteSelectEditorCommand()));

    #endregion

    #region Command functions

    private void ExecuteSelectEditorCommand()
    {
      if ( !InteractionService.OpenFileDialog(out string editorPath, Application.Current.TryFindResource("ExtrasOpenExecutableDialog").ToString(), CoreEnvironment.ApplicationTitle) )
        return;

      SettingsHelperController.CurrentSettings.EditorPath = editorPath;
      IconSource = BusinessHelper.GetAssemblyIcon(editorPath);
    }

    #endregion
  }
}
