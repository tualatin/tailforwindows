using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Org.Vs.TailForWin.PlugIns.OptionModules.Interfaces;


namespace Org.Vs.TailForWin.PlugIns.OptionModules.SmartWatchOption
{
  /// <summary>
  /// Interaction logic for SmartWatchOptionPage.xaml
  /// </summary>
  public partial class SmartWatchOptionPage : IOptionPage
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SmartWatchOptionPage() => InitializeComponent();

    /// <summary>
    /// Current page title
    /// </summary>
    public string PageTitle => Application.Current.TryFindResource("SmartWatchPageTitle").ToString();
  }
}
