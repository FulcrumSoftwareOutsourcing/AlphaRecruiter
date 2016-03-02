using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Remote
{
  /// <summary>
  /// Base page for error pages.
  /// </summary>
  public class CxErrorPage : System.Web.UI.Page
  {

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Page.LoadComplete"/> event at the end of the page load stage.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.
    ///                 </param>
    protected override void OnLoadComplete(EventArgs e)
    {
      Message = string.Empty;
      StackTrace = string.Empty;
      string exceptionKey = Request.QueryString["exKey"];
      if (string.IsNullOrEmpty(exceptionKey))
        return;

      Exception exception = (Exception)Application[exceptionKey];
      if (exception == null)
        return;

      Message = exception.Message;
      StackTrace = exception.ToString();

      Application.Remove(exceptionKey);

      base.OnLoad(e);
    }

    //----------------------------------------------------------------------------
    public string Message { get; private set; }
    //----------------------------------------------------------------------------
    public string StackTrace { get; private set; }
  }
}
