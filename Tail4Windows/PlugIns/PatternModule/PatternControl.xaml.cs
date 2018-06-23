using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Org.Vs.TailForWin.Core.Data;
using Org.Vs.TailForWin.PlugIns.PatternModule.Interfaces;
using Org.Vs.TailForWin.PlugIns.PatternModule.ViewModels;


namespace Org.Vs.TailForWin.PlugIns.PatternModule
{
  /// <summary>
  /// Interaction logic for PatternControl.xaml
  /// </summary>
  public partial class PatternControl
  {
    private readonly IPatternControlViewModel _patternControlViewModel;

    #region Properties

    /// <summary>
    /// Current <see cref="TailData"/>
    /// </summary>
    public TailData CurrentTailData
    {
      get => _patternControlViewModel?.CurrenTailData;
      set
      {
        if ( _patternControlViewModel == null )
          return;

        _patternControlViewModel.CurrenTailData = value;
      }
    }

    #endregion

    /// <summary>
    /// Standard constructor
    /// </summary>
    public PatternControl()
    {
      InitializeComponent();
      _patternControlViewModel = (PatternControlViewModel) DataContext;

      PreviewKeyDown += HandleEsc;
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
      if ( e.Key != Key.Escape )
        return;

      e.Handled = true;
      Close();
    }

    private void PatternTextBoxOnSelectionChanged(object sender, RoutedEventArgs e)
    {
      if ( !(sender is TextBox tb) )
        return;

      _patternControlViewModel.CaretIndex = tb.CaretIndex;
    }
  }
}
