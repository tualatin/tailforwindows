using System;
using System.Windows.Data;
using System.Windows.Markup;
using Org.Vs.TailForWin.Ui.Utils.Converters.MultiConverters;


namespace Org.Vs.TailForWin.Ui.Utils.Extensions
{
  /// <summary>
  /// Converter bindable binding
  /// </summary>
  public class ConverterBindableBinding : MarkupExtension
  {
    /// <summary>
    /// Binding
    /// </summary>
    public Binding Binding
    {
      get;
      set;
    }

    /// <summary>
    /// Converter
    /// </summary>
    public IValueConverter Converter
    {
      get;
      set;
    }

    /// <summary>
    /// Bindable converter parameter
    /// </summary>
    public Binding ConverterParameterBinding
    {
      get;
      set;
    }

    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The object value to set on the property where the extension is applied. </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      var multiBinding = new MultiBinding();

      multiBinding.Bindings.Add(Binding);
      multiBinding.Bindings.Add(ConverterParameterBinding);

      var adapter = new MultiValueConverterAdapter
      {
        Converter = Converter
      };
      multiBinding.Converter = adapter;

      return multiBinding.ProvideValue(serviceProvider);
    }
  }
}
