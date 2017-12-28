using System.Windows;
using System.Windows.Input;
using log4net;
using Org.Vs.TailForWin.Core.Data.Base;
using Org.Vs.TailForWin.Core.Utils;
using Org.Vs.TailForWin.UI.Commands;


namespace Org.Vs.TailForWin.UI.ViewModels
{
  /// <inheritdoc />
  /// <summary>
  /// T4Window view model
  /// </summary>
  public class T4WindowViewModel : NotifyMaster
  {
    // ReSharper disable once InconsistentNaming
    private static readonly ILog LOG = LogManager.GetLogger(typeof(T4WindowViewModel));

    /// <summary>
    /// Window title
    /// </summary>
    public string WindowTitle => EnvironmentContainer.ApplicationTitle;

    /// <summary>
    /// Width of main window
    /// </summary>
    public double Width
    {
      get;
      set;
    }

    /// <summary>
    /// Height of main window
    /// </summary>
    public double Height
    {
      get;
      set;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public T4WindowViewModel()
    {
      LOG.Trace("Start view model");
    }

    #region Commands

    private ICommand _goToLineCommand;

    /// <summary>
    /// Go to line xxx command
    /// </summary>
    public ICommand GoToLineCommand => _goToLineCommand ?? (_goToLineCommand = new RelayCommand(p => ExecuteGoToLineCommand()));

    #endregion

    #region Command functions

    private void ExecuteGoToLineCommand()
    {
      MessageBox.Show("Test", "Hint", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    #endregion
  }
}
