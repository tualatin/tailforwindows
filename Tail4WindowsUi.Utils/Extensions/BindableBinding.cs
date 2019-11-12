using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;


namespace Org.Vs.TailForWin.Ui.Utils.Extensions
{
  /// <summary>
  /// Bindable binding
  /// </summary>
  public class BindableBinding : MarkupExtension
  {
    private readonly Binding _binding = new Binding();

    /// <summary>
    /// Standard constructor
    /// </summary>
    public BindableBinding()
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="path"><see cref="PropertyPath"/></param>
    public BindableBinding(PropertyPath path) => _binding.Path = path;

    /// <summary>
    /// Property path
    /// </summary>
    public PropertyPath Path
    {
      get => _binding.Path;
      set => _binding.Path = value;
    }

    /// <summary>
    /// Source
    /// </summary>
    public object Source
    {
      get => _binding.Source;
      set => _binding.Source = value;
    }

    /// <summary>
    /// Relative source
    /// </summary>
    public RelativeSource RelativeSource
    {
      get => _binding.RelativeSource;
      set => _binding.RelativeSource = value;
    }

    /// <summary>
    /// Element name
    /// </summary>
    public string ElementName
    {
      get => _binding.ElementName;
      set => _binding.ElementName = value;
    }

    /// <summary>
    /// XPath
    /// </summary>
    public string XPath
    {
      get => _binding.XPath;
      set => _binding.XPath = value;
    }

    /// <summary>
    /// Path
    /// </summary>
    public BindingMode Mode
    {
      get => _binding.Mode;
      set => _binding.Mode = value;
    }

    /// <summary>
    /// Converter culture
    /// </summary>
    public CultureInfo ConverterCulture
    {
      get => _binding.ConverterCulture;
      set => _binding.ConverterCulture = value;
    }

    /// <summary>
    /// Converter parameter
    /// </summary>
    public object ConverterParameter
    {
      get;
      set;
    }

    /// <summary>
    /// Converter parameter binding
    /// </summary>
    public Binding ConverterParameterBinding
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
    /// Converter binding
    /// </summary>
    public Binding ConverterBinding
    {
      get;
      set;
    }

    /// <summary>
    /// String format
    /// </summary>
    public string StringFormat
    {
      get;
      set;
    }

    /// <summary>
    /// String format binding
    /// </summary>
    public Binding StringFormatBinding
    {
      get; set;
    }

    /// <summary>
    /// When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <returns>The object value to set on the property where the extension is applied. </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      var multiBinding = new MultiBinding
      {
        Mode = _binding.Mode,
        ConverterCulture = _binding.ConverterCulture,
        Converter = new InternalConverter(this)
      };
      multiBinding.Bindings.Add(_binding);

      if ( ConverterParameterBinding != null )
        multiBinding.Bindings.Add(ConverterParameterBinding);
      else
        multiBinding.ConverterParameter = ConverterParameter;

      if ( ConverterBinding != null )
        multiBinding.Bindings.Add(ConverterBinding);

      if ( StringFormatBinding != null )
        multiBinding.Bindings.Add(StringFormatBinding);

      return multiBinding.ProvideValue(serviceProvider);
    }

    private class InternalConverter : IMultiValueConverter
    {
      private readonly BindableBinding _binding;
      private IValueConverter _lastConverter;
      private object _lastConverterParameter;

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="binding"><see cref="BindableBinding"/></param>
      public InternalConverter(BindableBinding binding) => _binding = binding;

      object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
      {
        var valueIndex = 1;
        object converterParameter = parameter;

        if ( _binding.ConverterParameterBinding != null )
          converterParameter = values[valueIndex++];

        _lastConverterParameter = converterParameter;

        IValueConverter converter = _binding.Converter;

        if ( _binding.ConverterBinding != null )
          converter = values[valueIndex++] as IValueConverter;

        _lastConverter = converter;

        string stringFormat = _binding.StringFormat;

        if ( _binding.StringFormatBinding != null )
          stringFormat = values[valueIndex++] as string;

        object value = values[0];

        if ( converter != null )
          value = converter.Convert(value, targetType, converterParameter, culture);

        if ( stringFormat != null )
          value = string.Format(stringFormat, value);

        return value;
      }

      object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => _lastConverter != null ? new[] { _lastConverter.ConvertBack(value, targetTypes[0], _lastConverterParameter, culture) } : new[] { value };
    }
  }
}
