using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;


namespace Org.Vs.TailForWin.UI.Behaviors
{
  /// <summary>
  /// <see cref="TextBox"/> mask behavior
  /// </summary>
  public class TextBoxMaskBehavior : Behavior<TextBox>
  {
    #region DependencyProperties

    /// <summary>
    /// Input mask property
    /// </summary>
    public static readonly DependencyProperty InputMaskProperty = DependencyProperty.Register(nameof(InputMask), typeof(string), typeof(TextBoxMaskBehavior), null);

    /// <summary>
    /// Input mask
    /// </summary>
    public string InputMask
    {
      get => (string) GetValue(InputMaskProperty);
      set => SetValue(InputMaskProperty, value);
    }

    /// <summary>
    /// Prompt character property
    /// </summary>
    public static readonly DependencyProperty PromptCharProperty = DependencyProperty.Register(nameof(PromptChar), typeof(char), typeof(TextBoxMaskBehavior), new PropertyMetadata('_'));

    /// <summary>
    /// Prompt character
    /// </summary>
    public char PromptChar
    {
      get => (char) GetValue(PromptCharProperty);
      set => SetValue(PromptCharProperty, value);
    }

    #endregion

    /// <summary>
    /// Provider
    /// </summary>
    public MaskedTextProvider Provider
    {
      get;
      private set;
    }

    /// <summary>
    /// Called after the behavior is attached to an AssociatedObject.
    /// </summary>
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.Loaded += AssociatedObjectLoaded;
      AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
      AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

      DataObject.AddPastingHandler(AssociatedObject, Pasting);
    }

    /// <summary>
    /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
    /// </summary>
    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.Loaded -= AssociatedObjectLoaded;
      AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
      AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

      DataObject.RemovePastingHandler(AssociatedObject, Pasting);
    }

    /*
    Mask Character  Accepts  Required?  
    0  Digit (0-9)  Required  
    9  Digit (0-9) or space  Optional  
    #  Digit (0-9) or space  Required  
    L  Letter (a-z, A-Z)  Required  
    ?  Letter (a-z, A-Z)  Optional  
    &  Any character  Required  
    C  Any character  Optional  
    A  Alphanumeric (0-9, a-z, A-Z)  Required  
    a  Alphanumeric (0-9, a-z, A-Z)  Optional  
       Space separator  Required 
    .  Decimal separator  Required  
    ,  Group (thousands) separator  Required  
    :  Time separator  Required  
    /  Date separator  Required  
    $  Currency symbol  Required  

    In addition, the following characters have special meaning:

    Mask Character  Meaning  
    <  All subsequent characters are converted to lower case  
    >  All subsequent characters are converted to upper case  
    |  Terminates a previous < or >  
    \  Escape: treat the next character in the mask as literal text rather than a mask symbol
    */
    void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
    {
      Provider = new MaskedTextProvider(InputMask, CultureInfo.CurrentCulture);
      Provider.Set(AssociatedObject.Text);
      Provider.PromptChar = PromptChar;
      AssociatedObject.Text = Provider.ToDisplayString();

      //seems the only way that the text is formatted correct, when source is updated
      var textProp = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));
      textProp?.AddValueChanged(AssociatedObject, (s, args) => UpdateText());
    }

    void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      TreatSelectedText();

      var position = GetNextCharacterPosition(AssociatedObject.SelectionStart);

      if ( Keyboard.IsKeyToggled(Key.Insert) )
      {
        if ( Provider.Replace(e.Text, position) )
          position++;
      }
      else
      {
        if ( Provider.InsertAt(e.Text, position) )
          position++;
      }

      position = GetNextCharacterPosition(position);

      RefreshText(position);

      e.Handled = true;
    }

    void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if ( e.Key == Key.Space )//handle the space
      {
        TreatSelectedText();

        var position = GetNextCharacterPosition(AssociatedObject.SelectionStart);

        if ( Provider.InsertAt(" ", position) )
          RefreshText(position);

        e.Handled = true;
      }

      if ( e.Key == Key.Back )//handle the back space
      {
        if ( TreatSelectedText() )
        {
          RefreshText(AssociatedObject.SelectionStart);
        }
        else
        {
          if ( AssociatedObject.SelectionStart != 0 )
          {
            if ( Provider.RemoveAt(AssociatedObject.SelectionStart - 1) )
              RefreshText(AssociatedObject.SelectionStart - 1);
          }
        }

        e.Handled = true;
      }

      if ( e.Key == Key.Delete )//handle the delete key
      {
        //treat selected text
        if ( TreatSelectedText() )
        {
          RefreshText(AssociatedObject.SelectionStart);
        }
        else
        {
          if ( Provider.RemoveAt(AssociatedObject.SelectionStart) )
            RefreshText(AssociatedObject.SelectionStart);
        }

        e.Handled = true;
      }
    }

    private void Pasting(object sender, DataObjectPastingEventArgs e)
    {
      if ( e.DataObject.GetDataPresent(typeof(string)) )
      {
        var pastedText = (string) e.DataObject.GetData(typeof(string));

        TreatSelectedText();

        var position = GetNextCharacterPosition(AssociatedObject.SelectionStart);

        if ( pastedText != null && Provider.InsertAt(pastedText, position) )
          RefreshText(position);
      }

      e.CancelCommand();
    }

    private void UpdateText()
    {
      //check Provider.Text + TextBox.Text
      if ( Provider.ToDisplayString().Equals(AssociatedObject.Text) )
        return;

      //use provider to format
      var success = Provider.Set(AssociatedObject.Text);

      //ui and mvvm/codebehind should be in sync
      SetText(success ? Provider.ToDisplayString() : AssociatedObject.Text);
    }

    private bool TreatSelectedText() =>
      AssociatedObject.SelectionLength > 0 && Provider.RemoveAt(AssociatedObject.SelectionStart, AssociatedObject.SelectionStart + AssociatedObject.SelectionLength - 1);

    private void RefreshText(int position)
    {
      SetText(Provider.ToDisplayString());
      AssociatedObject.SelectionStart = position;
    }

    private void SetText(string text) => AssociatedObject.Text = string.IsNullOrWhiteSpace(text) ? string.Empty : text;

    private int GetNextCharacterPosition(int startPosition)
    {
      var position = Provider.FindEditPositionFrom(startPosition, true);
      return position == -1 ? startPosition : position;
    }
  }
}
