using System;
using System.Windows;
using System.Net;
using TailForWin.Utils;
using System.IO;
using System.Text;


namespace TailForWin.Template.UpdateController
{
  /// <summary>
  /// Simple class to a show message box with error message
  /// </summary>
  public static class UserErrorException
  {

    public static void HandleUserException (Exception ex)
    {
      if (ex.GetType ( ) == typeof (ArgumentNullException) || ex.GetType ( ) == typeof (NullReferenceException))
      {
        ShowMessageBox (ex.Message);
        return;
      }

      if (ex.GetType ( ) == typeof (WebException))
      {
        var webException = ex as WebException;

        if (webException == null)
          return;

        HttpWebResponse response = (HttpWebResponse) webException.Response;

        if (response != null)
        {
          switch (response.StatusCode)
          {
          case HttpStatusCode.ProxyAuthenticationRequired:

            ShowMessageBox (ex.Message);
            break;

          case HttpStatusCode.NotFound:

            ShowMessageBox (string.Format ("{0}\n{1}", ex.Message, response.ResponseUri));
            break;

          case HttpStatusCode.Forbidden:

            ShowMessageBox (string.Format ("{0}\n{1}", ex.Message, response.StatusDescription));
            break;

          case HttpStatusCode.Conflict:

            ShowMessageBox (string.Format ("{0}\n{1}", ex.Message, response.StatusDescription));
            break;

          case HttpStatusCode.Unauthorized:

            ShowMessageBox (string.Format ("{0}\n{1}", ex.Message, response.StatusDescription));
            break;

          default:

            try
            {
              string result;
              using (StreamReader reader = new StreamReader (response.GetResponseStream ( ), (string.IsNullOrEmpty (response.ContentEncoding) ? Encoding.UTF8 : Encoding.GetEncoding (response.ContentEncoding))))
              {
                result = reader.ReadToEnd ( );
              }

              ShowMessageBox (result);
            }
            catch (Exception e)
            {
              ErrorLog.WriteLog (ErrorFlags.Error, "UserErrorException", string.Format ("{1}, exception: {0}", e, System.Reflection.MethodBase.GetCurrentMethod (  ).Name));
            }
            break;
          }
        }
        else
          ShowMessageBox (ex.Message);

        return;
      }

      if (ex.GetType ( ) == typeof (UriFormatException))
      {
        ShowMessageBox (ex.Message);
        return;
      }

      ShowMessageBox (ex.Message);
    }

    private static void ShowMessageBox (string msg)
    {
      MessageBox.Show (msg, Application.Current.FindResource ("Error").ToString ( ), MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
