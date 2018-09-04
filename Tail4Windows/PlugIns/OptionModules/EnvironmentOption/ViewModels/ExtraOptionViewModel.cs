using System.Windows;
using System.Windows.Input;
using Org.Vs.TailForWin.Controllers.Commands;
using Org.Vs.TailForWin.Controllers.PlugIns.OptionModules.EnvironmentOption.Interfaces;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.EnvironmentOption.ViewModels
{
  /// <summary>
  /// Extra option view model
  /// </summary>
  public class ExtraOptionViewModel : NotifyMaster, IExtraOptionViewModel
  {
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
      string editorPath;

      if ( !InteractionService.OpenFileDialog(out editorPath, Application.Current.TryFindResource("ExtraOpenExecutableDialog").ToString(), CoreEnvironment.ApplicationTitle) )
        return;


    }

    #endregion
  }
}
