using System.Configuration;
using Framework.Utils;

namespace Framework.Common
{
  public static class CxConfigurationHelper
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// The display name of the application.
    /// </summary>
    public static string ApplicationName { get { return ConfigurationManager.AppSettings["ApplicationName"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The inner code of the application used for uniquity purposes. Should not contain anything but letters. No spaces allowed.
    /// </summary>
    public static string ApplicationCode { get { return ConfigurationManager.AppSettings["ApplicationCode"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The inner code of the frontend layer of the application used to distinguish different presentations for the same database (means, even if the application code is the same).
    /// </summary>
    public static string FrontendCode { get { return ConfigurationManager.AppSettings["FrontendCode"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Application UI language code. Presence of this parameter in the config file enables multilanguage subsystem.    
    /// </summary>
    public static string LocalizationLanguageCode { get { return ConfigurationManager.AppSettings["localizationLanguageCode"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether to show the splash screen during the application start.
    /// </summary>
    public static bool IsShowSplashEnabled { get { return CxBool.Parse(ConfigurationManager.AppSettings["showSplash"], false); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If false, then no Outlook Integration related UI is available in the application, and the plugin is not being automatically installed on the startup.
    /// </summary>
    public static bool IsOutlookIntegrationEnabled { get { return CxBool.Parse(ConfigurationManager.AppSettings["IsOutlookIntegrationEnabled"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Storage for the forms, controls and grids layout: Database or Registry.
    /// </summary>
    public static string LayoutStorage { get { return ConfigurationManager.AppSettings["layoutStorage"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets the wait time (in sec) before terminating the attempt to execute a command and generating an error. Usage: IDBCommand
    /// </summary>
    public static int DefaultCommandTimeout { get { return CxInt.Parse(ConfigurationManager.AppSettings["DefaultCommandTimeout"], 0); } }
    //-------------------------------------------------------------------------
    public static string AssemblyFolder { get { return ConfigurationManager.AppSettings["AssemblyFolder"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default record count limit used to fetch large datasets.
    /// </summary>
    public static int DefaultRecordCountLimit { get { return CxInt.Parse(ConfigurationManager.AppSettings["DefaultRecordCountLimit"], -1); } }
    //-------------------------------------------------------------------------
    public static bool IsPagingAllowed { get { return CxBool.Parse(ConfigurationManager.AppSettings["pagingAllowed"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// False to disable calculating amount of child entities.
    /// </summary>
    public static bool IsCalcAmountOfItemsForChildEntitiesEnabled { get { return CxBool.Parse(ConfigurationManager.AppSettings["calcAmountOfItemsForChildEntitiesEnabled"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to enable metadata customization.
    /// </summary>
    public static bool IsCustomizationEnabled { get { return CxBool.Parse(ConfigurationManager.AppSettings["customizationEnabled"], false); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to enable lookups cache for performance improving.
    /// </summary>
    public static bool IsLookupCacheEnabled { get { return CxBool.Parse(ConfigurationManager.AppSettings["lookupCacheEnabled"], false); } }
    //-------------------------------------------------------------------------
    public static string DefaultPageId { get { return ConfigurationManager.AppSettings["DefaultPageId"]; } }
    //-------------------------------------------------------------------------

    // Error logging stuff
    //-------------------------------------------------------------------------
    /// <summary>
    /// Comma-separated list of exception types to ignore (do not write to log file).
    /// </summary>
    public static string ErrorLogIgnoreExceptions { get { return ConfigurationManager.AppSettings["errorLogIgnoreExceptions"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Value should be 'On' or 'Off'. If errorLogging='On', all exceptions are logged in text file.
    /// </summary>
    public static bool IsErrorLoggingEnabled { get { return CxBool.Parse(ConfigurationManager.AppSettings["errorLogging"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Days to keep error log files.
    /// </summary>
    public static int ErrorLogDaysToKeep { get { return CxInt.Parse(ConfigurationManager.AppSettings["errorLogDays"], 7); } }
    //-------------------------------------------------------------------------

    // Email-related stuff
    //-------------------------------------------------------------------------
    public static string DebugEmailRedirectAddress { get { return ConfigurationManager.AppSettings["debugEmailRedirectAddress"]; } }
    //-------------------------------------------------------------------------
    public static string DefaultEmailFromAddress { get { return ConfigurationManager.AppSettings["defaultEmailFromAddress"]; } }
    //-------------------------------------------------------------------------
    public static string SmtpMailServer { get { return ConfigurationManager.AppSettings["SmtpMailServer"]; } }
    //-------------------------------------------------------------------------
  }
}
