namespace TailForWin.Template.TaskBarNotifier
{
  public class NotifyObject
  {
    public NotifyObject (string message, string title)
    {
      Message = message;
      Title = title;
    }

    private string title;

    public string Title
    {
      get 
      { 
        return (title); 
      }
      set 
      { 
        title = value; 
      }
    }

    private string message;
    
    public string Message
    {
      get 
      { 
        return (message); 
      }
      set 
      { 
        message = value; 
      }
    }
  }
}
