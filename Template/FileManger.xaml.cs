using System.Windows;
using TailForWin.Data;
using TailForWin.Utils;
using System.Windows.Input;


namespace TailForWin.Template
{
  /// <summary>
  /// Interaction logic for FileManger.xaml
  /// </summary>
  public partial class FileManger: Window
  {
    public FileManger ()
    {
      InitializeComponent ( );

      PreviewKeyDown += HandleEsc;

      FileManagerStructure fmDoc = new FileManagerStructure ( );

      if (fmDoc.Categories.Count < 1)
        comboBoxCategory.IsEnabled = false;
      else
      {
        comboBoxCategory.ItemsSource = fmDoc.Categories;
        comboBoxCategory.SelectedIndex = 0;
      }
    }

    private void comboBoxCategory_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }

    private void HandleEsc (object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        OnExit ( );
    }

    private void OnExit ()
    {
      Close ( );
    }
  }
}
