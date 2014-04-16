using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Forms;


namespace TailForWin.Template.TaskBarNotifier
{
  public class TaskBarNotifier : Window, INotifyPropertyChanged
  {
    /// <summary>
    /// Internal states
    /// </summary>
    private enum DisplayStates
    {
      Opening,
      Opened,
      Hiding,
      Hidden
    }

    private DispatcherTimer stayOpenTimer;
    private Storyboard storyboard;
    private DoubleAnimation animation;

    private double hiddenTop;
    private double openedTop;
    private EventHandler arrivedHidden;
    private EventHandler arrivedOpened;


    public TaskBarNotifier ()
    {
      Loaded += TaskNotifier_Loaded;
    }

    private void TaskNotifier_Loaded (object sender, RoutedEventArgs e)
    {
      SetInitialLocations (false);

      DisplayState = DisplayStates.Hidden;

      stayOpenTimer = new DispatcherTimer
      {
        Interval = TimeSpan.FromMilliseconds (StayOpenMilliseconds)
      };
      stayOpenTimer.Tick += stayOpenTimer_Elapsed;

      animation = new DoubleAnimation ( );
      Storyboard.SetTargetProperty (animation, new PropertyPath (Window.TopProperty));
      storyboard = new Storyboard
      {
        FillBehavior = FillBehavior.Stop,
      };
      storyboard.Children.Add (animation);

      arrivedHidden = Storyboard_ArrivedHidden;
      arrivedOpened = Storyboard_ArrivedOpened;
    }

    protected override void OnInitialized (EventArgs e)
    {
      // No title bar or resize border.
      WindowStyle = WindowStyle.None;
      ResizeMode = ResizeMode.NoResize;

      // Don't show in taskbar.
      ShowInTaskbar = false;

      base.OnInitialized (e);
    }

    private void SetInitialLocations (bool showOpened)
    {
      // Determine screen working area.
      System.Drawing.Rectangle workingArea = new System.Drawing.Rectangle ((int) this.Left, (int) this.Top, (int) this.ActualWidth, (int) this.ActualHeight);
      workingArea = Screen.GetWorkingArea (workingArea);

      // Initialize the window location to the bottom right corner.
      Left = workingArea.Right - ActualWidth - LeftOffset;

      // Set the opened and hidden locations.
      hiddenTop = workingArea.Bottom;
      openedTop = workingArea.Bottom - ActualHeight;

      // Set Top based on whether opened or hidden is desired
      if (showOpened)
        Top = openedTop;
      else
        Top = hiddenTop;
    }

    private void BringToTop ()
    {
      // Bring this window to the top without making it active.
      Topmost = true;
      Topmost = false;
    }

    private void OnDisplayStateChanged ()
    {
      // The display state has changed.

      // Unless the stortboard as already been created, nothing can be done yet.
      if (storyboard == null)
        return;

      // Stop the current animation.
      storyboard.Stop (this);

      // Since the storyboard is reused for opening and closing, both possible
      // completed event handlers need to be removed.  It is not a problem if
      // either of them was not previously set.
      storyboard.Completed -= arrivedHidden;
      storyboard.Completed -= arrivedOpened;

      if (DisplayState != DisplayStates.Hidden)
        // Unless the window has just arrived at the hidden state, it must be
        // moving, and should be shown.
        BringToTop ( );

      if (DisplayState == DisplayStates.Opened)
      {
        // The window has just arrived at the opened state.

        // Because the inital settings of this TaskNotifier depend on the screen's working area,
        // it is best to reset these occasionally in case the screen size has been adjusted.
        SetInitialLocations (true);

        if (!IsMouseOver)
        {
          // The mouse is not within the window, so start the countdown to hide it.
          stayOpenTimer.Stop ( );
          stayOpenTimer.Start ( );
        }
      }
      else if (DisplayState == DisplayStates.Opening)
      {
        // The window should start opening.
        // Make the window visible.
        Visibility = Visibility.Visible;
        BringToTop ( );

        // Because the window may already be partially open, the rate at which
        // it opens may be a fraction of the normal rate.
        // This must be calculated.
        int milliseconds = CalculateMillseconds (OpeningMilliseconds, this.openedTop);

        if (milliseconds < 1)
        {
          // This window must already be open.
          this.DisplayState = DisplayStates.Opened;
          return;
        }

        // Reconfigure the animation.
        animation.To = openedTop;
        animation.Duration = new Duration (new TimeSpan (0, 0, 0, 0, milliseconds));

        // Set the specific completed event handler.
        storyboard.Completed += arrivedOpened;

        // Start the animation.
        storyboard.Begin (this, true);
      }
      else if (DisplayState == DisplayStates.Hiding)
      {
        // The window should start hiding.
        // Because the window may already be partially hidden, the rate at which
        // it hides may be a fraction of the normal rate.
        // This must be calculated.
        int milliseconds = CalculateMillseconds (HidingMilliseconds, hiddenTop);

        if (milliseconds < 1)
        {
          // This window must already be hidden.
          DisplayState = DisplayStates.Hidden;
          return;
        }

        // Reconfigure the animation.
        animation.To = hiddenTop;
        animation.Duration = new Duration (new TimeSpan (0, 0, 0, 0, milliseconds));

        // Set the specific completed event handler.
        storyboard.Completed += arrivedHidden;

        // Start the animation.
        storyboard.Begin (this, true);
      }
      else if (DisplayState == DisplayStates.Hidden)
      {
        // Ensure the window is in the hidden position.
        SetInitialLocations (false);

        // Hide the window.
        Visibility = Visibility.Hidden;
      }
    }

    private int CalculateMillseconds (int totalMillsecondsNormally, double destination)
    {
      if (Top == destination)
        // The window is already at its destination. Nothing to do.
        return (0);

      double distanceRemaining = Math.Abs (Top - destination);
      double percentDone = distanceRemaining / ActualHeight;

      // Determine the percentage of normal milliseconds that are actually required.
      return ((int)(totalMillsecondsNormally * percentDone));
    }

    protected virtual void Storyboard_ArrivedHidden (object sender, EventArgs e)
    {
      // Setting the display state will result in any needed actions.
      DisplayState = DisplayStates.Hidden;
    }

    protected virtual void Storyboard_ArrivedOpened (object sender, EventArgs e)
    {
      // Setting the display state will result in any needed actions.
      DisplayState = DisplayStates.Opened;
    }

    private void stayOpenTimer_Elapsed (Object sender, EventArgs args)
    {
      // Stop the timer because this should not be an ongoing event.
      stayOpenTimer.Stop ( );

      if (!IsMouseOver)
        // Only start closing the window if the mouse is not over it.
        DisplayState = DisplayStates.Hiding;
    }

    #region Public functions

    public void Notify ()
    {
      if (DisplayState == DisplayStates.Opened)
      {
        stayOpenTimer.Stop ( );
        stayOpenTimer.Start ( );
      }
      else
        DisplayState = DisplayStates.Opening;
    }

    public void ForceHidden ()
    {
      DisplayState = DisplayStates.Hidden;
    }

    #endregion

    #region MouseEvents

    protected override void OnMouseEnter (System.Windows.Input.MouseEventArgs e)
    {
      if (DisplayState == DisplayStates.Opened)
        stayOpenTimer.Stop ( );
      else if ((DisplayState == DisplayStates.Hidden) || (DisplayState == DisplayStates.Hiding))
        DisplayState = DisplayStates.Opening;

      base.OnMouseEnter (e);
    }

    protected override void OnMouseLeave (System.Windows.Input.MouseEventArgs e)
    {
      if (DisplayState == DisplayStates.Opened)
      {
        stayOpenTimer.Stop ( );
        stayOpenTimer.Start ( );
      }

      base.OnMouseEnter (e);
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged (string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;

      if (handler != null)
        handler (this, new PropertyChangedEventArgs (name));
    }

    protected override void OnPropertyChanged (DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged (e);

      if (String.Compare (e.Property.Name, "Top", false) == 0)
      {
        if (((double) e.NewValue != (double) e.OldValue) && ((double) e.OldValue != hiddenTop))
          BringToTop ( );
      }
    }

    #endregion

    #region Properties

    private DisplayStates displayState;

    /// <summary>
    /// Current display state
    /// </summary>
    private DisplayStates DisplayState
    {
      get
      {
        return (displayState);
      }
      set
      {
        if (value == displayState)
          return;

        displayState = value;
        OnDisplayStateChanged ( );
      }
    }

    private int leftOffset;

    /// <summary>
    /// The space, if any, between the left side of the TaskNotifer window and the right side of the screen.
    /// </summary>
    public int LeftOffset
    {
      get 
      { 
        return (leftOffset); 
      }
      set
      {
        leftOffset = value;
        OnPropertyChanged ("LeftOffset");
      }
    }

    #endregion

    #region Control Properties

    public static readonly DependencyProperty StayOpenMillisecondsProperty = DependencyProperty.Register ("StayOpenMilliseconds", typeof (int), typeof (TaskBarNotifier),
      new PropertyMetadata (1000));

    /// <summary>
    /// The time the TaskbarNotifier window should stay open in milliseconds.
    /// </summary>
    [Category ("Popup Window Settings")]
    public int StayOpenMilliseconds
    {
      get 
      { 
        return ((int) GetValue (StayOpenMillisecondsProperty)); 
      }
      set
      {
        SetValue (StayOpenMillisecondsProperty, value);
      }
    }

    public static readonly DependencyProperty HidingMillisecondsProperty = DependencyProperty.Register ("HidingMilliseconds", typeof (int), typeof (TaskBarNotifier),
      new PropertyMetadata (1000));

    /// <summary>
    /// The time the TaskbarNotifier window should take to hide in milliseconds.
    /// </summary>
    [Category ("Popup Window Settings")]
    public int HidingMilliseconds
    {
      get 
      { 
        return ((int) GetValue (HidingMillisecondsProperty)); 
      }
      set
      {
        SetValue (HidingMillisecondsProperty, value);
      }
    }

    public static readonly DependencyProperty OpeningMillisecondsProperty = DependencyProperty.Register ("OpeningMilliseconds", typeof (int), typeof (TaskBarNotifier),
      new PropertyMetadata (1000));

    /// <summary>
    /// The time the TaskbarNotifier window should take to open in milliseconds.
    /// </summary>
    [Category ("Popup Window Settings")]
    public int OpeningMilliseconds
    {
      get 
      { 
        return ((int) GetValue (OpeningMillisecondsProperty)); 
      }
      set
      {
        SetValue (OpeningMillisecondsProperty, value);
      }
    }

    #endregion
  }
}
