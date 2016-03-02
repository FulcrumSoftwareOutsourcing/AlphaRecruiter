using System;
using System.Web;
using Framework.Db;
using Framework.Metadata;

namespace Framework.Remote
{
  /// <summary>
  /// Class for check some are most possible errors.  
  /// </summary>
  public class OnAppStartErrorsChecker
  {
    private const string MetadataExceptionAppKey = "MdEx";
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks occured or possible errors;
    /// </summary>
    public void CheckErrors()
    {
      if (HttpContext.Current.Application[MetadataExceptionAppKey] != null)
      {
        string exceptionKey =
          SaveException((Exception) HttpContext.Current.Application[MetadataExceptionAppKey]);
        HttpContext.Current.Server.Transfer("~/ErrorPages/MetadataException.aspx?exKey=" + exceptionKey);
        HttpContext.Current.Server.ClearError();
      }

      try
      {
        CheckConnection();
      }
      catch(Exception ex)
      {
        string exceptionKey = SaveException(ex);
        HttpContext.Current.Server.Transfer("~/ErrorPages/DbConnectionError.aspx?exKey=" + exceptionKey);
        HttpContext.Current.Server.ClearError();
      }

    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks database connection. 
    /// </summary>
    private void CheckConnection()
    {
      using (CxDbConnection conn = CxDbConnections.CreateEntityConnection())
      {
        conn.ExecuteScalar("select top 1 [name] from DbUpdateLog");
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Handle Exception related on errors in metadata.
    /// </summary>
    /// <param name="ex">Occured Exception.</param>
    public void HandleMetatadataException(Exception ex)
    {
        HttpContext.Current.Application[MetadataExceptionAppKey] = ex;
        HttpContext.Current.Server.ClearError();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Removes saved in HttpAplicationState metadata exception.
    /// </summary>
    public void ClearMetadataException()
    {
        HttpContext.Current.Application.Remove(MetadataExceptionAppKey);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Saves given Exception to HttpApplicationState and returns key.
    /// </summary>
    /// <param name="ex">Exception to save</param>
    /// <returns>Saved Exception key in HttpApplicationState dictionary. </returns>
    private string SaveException(Exception ex)
    {
      string exceptionKey = Guid.NewGuid().ToString();
      HttpContext.Current.Application[exceptionKey] = ex;
      return exceptionKey;
    }
  }
}
