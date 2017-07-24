using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using log4net;


namespace Org.Vs.TailForWin.Controller.WebServices
{
  /// <summary>
  /// Simple class to a show message box with error message
  /// </summary>
  public static class UserErrorException
  {
    private static readonly ILog LOG = LogManager.GetLogger(typeof(UserErrorException));

    /// <summary>
    /// Handle user exception
    /// </summary>
    /// <param name="ex">Exception</param>
    public static void HandleUserException(Exception ex)
    {
      if ( ex.GetType() == typeof(ArgumentNullException) || ex.GetType() == typeof(NullReferenceException) )
      {
        ShowMessageBox(ex.Message);
        return;
      }

      if ( ex.GetType() == typeof(WebException) )
      {
        var webException = ex as WebException;

        if ( webException == null )
          return;

        HttpWebResponse response = (HttpWebResponse) webException.Response;

        if ( response != null )
        {
          switch ( response.StatusCode )
          {
          case HttpStatusCode.ProxyAuthenticationRequired:

            ShowMessageBox(ex.Message);
            break;

          case HttpStatusCode.NotFound:

            ShowMessageBox($"{ex.Message}\n{response.ResponseUri}");
            break;

          case HttpStatusCode.Forbidden:

            ShowMessageBox($"{ex.Message}\n{response.StatusDescription}");
            break;

          case HttpStatusCode.Conflict:

            ShowMessageBox($"{ex.Message}\n{response.StatusDescription}");
            break;

          case HttpStatusCode.Unauthorized:

            ShowMessageBox($"{ex.Message}\n{response.StatusDescription}");
            break;

          default:

            try
            {
              string result;

              using ( StreamReader reader = new StreamReader(response.GetResponseStream(), string.IsNullOrEmpty(response.ContentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding(response.ContentEncoding)) )
              {
                result = reader.ReadToEnd();
              }

              ShowMessageBox(result);
            }
            catch ( Exception e )
            {
              LOG.Error(e, "{0} caused a(n) {1}", System.Reflection.MethodBase.GetCurrentMethod().Name, e.GetType().Name);
            }
            break;
          }
        }
        else
        {
          ShowMessageBox(ex.Message);
        }

        return;
      }

      if ( ex.GetType() == typeof(UriFormatException) )
      {
        ShowMessageBox(ex.Message);
        return;
      }

      ShowMessageBox(ex.Message);
    }

    private static void ShowMessageBox(string msg)
    {
      MessageBox.Show(msg, Application.Current.FindResource("Error")?.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
