/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;
using Framework.Common;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to get user permission provider.
  /// </summary>
  public delegate IxUserPermissionProvider DxGetUserPermissionProvider();
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to get value provider that returns predefined application 
  /// parameters.
  /// </summary>
  public delegate IxValueProvider DxGetApplicationValueProvider();
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to get entity rule cache.
  /// </summary>
  public delegate CxEntityRuleCache DxGetEntityRuleCache();
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to get user-defined metadata.
  /// </summary>
  public delegate CxUserMetadataCacheElement DxGetUserMetadataCache(
    CxMetadataHolder sender, 
    Type metadataObjectType,
    CxMetadataObject parentObject,
    IList predefinedItems);
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to get cached entity data object.
  /// </summary>
  public delegate object DxGetEntityDataCacheObject(string key);
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to set cached entity data object.
  /// </summary>
  public delegate void DxSetEntityDataCacheObject(string key, object cachedObject);
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method to clear cached object with the given entity ID.
  /// </summary>
  public delegate void DxClearEntityIdCache(string entityId);
  //------------------------------------------------------------------------------
  /// <summary>
  /// Delegate method invoked before metadata object is constructed from the XML element.
  /// </summary>
  public delegate void DxMetadataObjectLoading(
    CxMetadataHolder sender,
    CxMetadataObject metadataObject,
    XmlElement element);
  //------------------------------------------------------------------------------
  /// <summary>
  /// Event arguments for row source cached row changed event.
  /// </summary>
  public class CxRowSourceCachedRowChangedEventArgs
  {
    //----------------------------------------------------------------------------
    protected CxRowSourceMetadata m_RowSource;
    //----------------------------------------------------------------------------
    public CxRowSourceCachedRowChangedEventArgs(CxRowSourceMetadata rowSource)
    {
      m_RowSource = rowSource;
    }
    //----------------------------------------------------------------------------
    public CxRowSourceMetadata RowSource
    { get { return m_RowSource; } }
    //----------------------------------------------------------------------------
  }
  //------------------------------------------------------------------------------
  /// <summary>
  /// Row source cached row changed event delegate.
  /// </summary>
  public delegate void DxRowSourceCachedRowChanged(CxRowSourceCachedRowChangedEventArgs e);
  //------------------------------------------------------------------------------
  public enum NxMetadataHolderStartMode { Full, MultilanguageOnly }
  //------------------------------------------------------------------------------

  //------------------------------------------------------------------------------
  /// <summary>
	/// Class that holds all application metadata. Singleton.
	/// </summary>
	public class CxMetadataHolder
	{
    //----------------------------------------------------------------------------
    protected CxConfigMetadata m_Config; // Configuration metadata
    protected CxAssembliesMetadata m_Assemblies; // Metadata for assemblies
    protected CxClassesMetadata m_Classes; // Metadata for classes
    protected CxImagesMetadata m_Images; // Metadata for images
    protected CxMainMenuMetadata m_MainMenu; // Metadata for main menu
    protected CxEntitiesMetadata m_Entities; // Metadata for entities
    protected CxEntityUsagesMetadata m_EntityUsages; // Metadata for entity usages
    protected CxAttributesMetadata m_Attributes; // Metadata for entity attributes
    protected CxAttributeUsagesMetadata m_AttributeUsages; // Metadata for attribute usages
    protected CxRowSourcesMetadata m_RowSources; // Metadata for row sources
    protected CxReportsMetadata m_Reports; // Metadata for reports
    protected CxConstraintsMetadata m_Constraints; // Metadata for constraints

    protected CxPortalsMetadata m_Portals; // Metadata for Web portals
    protected CxPagesMetadata m_Pages; // Metadata for web pages
    protected CxWebPartsMetadata m_WebParts; // Metadata for web parts
    protected CxPortalSkinsMetadata m_PortalSkins; // Metadata for portal skins
    protected CxCommandsMetadata m_Commands; // Metadata for commands
    protected CxEntityCommandsMetadata m_EntityCommands; // Metadata for commands

    protected CxWinSectionsMetadata m_WinSections; // Metadata for windows app navigation sections
    protected CxWinFormsMetadata m_WinForms; // Metadata for windows auto-forms description

    private CxSlSkinsMetadata m_SlSkins;
    private CxSlSectionsMetadata m_SlSections; // Metadata for silverlight app navigation sections
    private CxSlFramesMetadata m_SlFrames; // Metadata for silverlight app frames.

    private CxSlDashboardsMetadata m_SlDashboards;// Metadata for silverlight dashboards.


    protected CxSecurityMetadata m_Security; // Metadata for security settings.
    private CxMultilanguage m_Multilanguage; // Multilanguage management object.
    protected IxCustomMetadataProvider m_CustomMetadataProvider; // Custom metadata provider.
    private CxMetadataPlaceholderManager m_PlaceholderManager;

    private CxAttributeTypePresenter m_AttributeTypePresenter;

    // Event handler for DB connection create.
    protected DxCreateDbConnection m_OnCreateDbConnection;

    // Default entity usage ID for file library.
    protected string m_DefaultFileLibraryEntityUsageId;
    // Default thumbnail sizes for DB image displaying.
    protected int m_ImageSmallThumbnailSize = 32;
    protected int m_ImageLargeThumbnailSize = 64;
    // Default record count limit during fetching records from DB.
    protected int m_DefaultRecordCountLimit = -1;
    // True if entity data cache is enabled.
    protected bool m_IsEntityDataCacheEnabled;
    // True if multilanguage is enabled.
    protected bool m_IsMultilanguageEnabled;
    // True if metadata customization is enabled.
    protected bool? m_IsCustomizationEnabled_Cache;
    // True if lookup cache is enabled.
    protected bool? m_IsLookupCacheEnabled_Cache;
    // True if paging is allowed.
    protected bool? m_IsPagingAllowed_Cache;
    // True if calculating amount of child entities is enabled.
    private bool? m_IsCalcAmountOfItemsForChildEntitiesEnabled_Cache;
    //----------------------------------------------------------------------------
    protected const string APP_PARAMS_TABLE_NAME = "Framework_ApplicationParameters";
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxMetadataHolder(NxMetadataHolderStartMode startMode)
		{
      switch (startMode)
      {
        case NxMetadataHolderStartMode.Full:
          Initialize();
          break;
        case NxMetadataHolderStartMode.MultilanguageOnly:
          break;
        default:
          throw new ArgumentOutOfRangeException("startMode");
      }
		}
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxMetadataHolder(DxMetadataObjectLoading eventMetadataObjectLoading)
    {
      OnMetadataObjectLoading += eventMetadataObjectLoading;
      Initialize();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxMetadataHolder(
      IxCustomMetadataProvider customMetadataProvider,
      DxMetadataObjectLoading eventMetadataObjectLoading)
    {
      m_CustomMetadataProvider = customMetadataProvider;
      OnMetadataObjectLoading += eventMetadataObjectLoading;
      Initialize();

      // Here we synchronize the application code to the centralized place as soon as possible.
      if (CxUtils.IsEmpty(CxAppInfo.ApplicationCode) && CxUtils.NotEmpty(ApplicationCode))
        CxAppInfo.ApplicationCode = ApplicationCode;
      
      if (IsCustomizationEnabled)
        LoadCustomMetadata();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxMetadataHolder()
    {
      Initialize();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads metadata from XML files.
    /// </summary>
    virtual protected void Initialize()
    {
      m_DefaultRecordCountLimit = CxConfigurationHelper.DefaultRecordCountLimit;
      PlaceholderManager = new CxMetadataPlaceholderManager(this);
      AttributeTypePresenter = CreateAttributeTypePresenter();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Factory method which creates an instance of attribute type presenter.
    /// </summary>
    /// <returns>the instance created</returns>
    protected virtual CxAttributeTypePresenter CreateAttributeTypePresenter()
    {
      return new CxAttributeTypePresenter();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata document from a specified file.
    /// Handles an exception.
    /// </summary>
    /// <param name="fileName">file name to load metadata from</param>
    /// <returns>loaded XML document</returns>
    public XmlDocument LoadMetadata(string fileName)
    {
      try
      {
        XmlDocument doc = GetMetadataDocument(fileName);
        if (doc == null)
        {
          throw new ExException(
            string.Format("Could not find metadata file: <{0}>", fileName));
        }
        return doc;
      }
      catch (Exception e)
      {
        throw new ExMetadataException(string.Format("Error reading metadata from {0}", fileName), e);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads all the metadata files resolving all their includes.
    /// </summary>
    /// <param name="fileName">a name of the root metadata file to be loaded</param>
    /// <returns>an array of XML documents</returns>
    public IEnumerable<XmlDocument> LoadMetadataWithIncludes(string fileName)
    {
      XmlDocument doc = LoadMetadata(fileName);
      List<XmlDocument> docs = new List<XmlDocument>();
      docs.Add(doc);
      if (doc.DocumentElement != null)
      {
        XmlNodeList includes = doc.DocumentElement.SelectNodes("include");
        if (includes == null)
          throw new ExNullReferenceException("includes");

        foreach (XmlElement element in includes)
        {
          string includeFileName = CxXml.GetAttr(element, "file");
          docs.AddRange(LoadMetadataWithIncludes(includeFileName));
        }
      }
      return docs;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata objects or updates the existing ones with custom values.
    /// </summary>
    public void LoadCustomMetadata()
    {
      IDictionary<string, IDictionary<string, XmlDocument>> documents = CustomMetadataProvider.GetCustomMetadata(new CxHashtable("APPLICATION$APPLICATIONCODE", ApplicationCode));
      if (documents.ContainsKey("entity_usage"))
      {
        m_EntityUsages.LoadCustomMetadata(documents["entity_usage"]);
        m_AttributeUsages.LoadCustomMetadata(documents["entity_usage"]);
      }
      if (documents.ContainsKey("form"))
      {
        m_WinForms.LoadCustomMetadata(documents["form"]);
      }
      if (documents.ContainsKey("row_source"))
      {
        m_RowSources.LoadCustomMetadata(documents["row_source"]);
      }
      if (documents.ContainsKey("win_section"))
      {
        m_WinSections.LoadCustomMetadata(documents["win_section"]);
      }
      if (documents.ContainsKey("win_sections"))
      {
        m_WinSections.LoadCustomMetadata(documents["win_sections"]);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns XML document with a specified file name to load metadata from.
    /// </summary>
    /// <param name="fileName">file name to load metadata from</param>
    virtual protected XmlDocument GetMetadataDocument(string fileName)
    {
      return LoadConfigFile(fileName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads config file and parses it as XML document.
    /// </summary>
    /// <param name="fileName">name of the XML file</param>
    /// <returns>XML document parsed from file</returns>
    static public XmlDocument LoadConfigFile(string fileName)
    {
      string path1 = Path.Combine(CxPath.GetApplicationBinaryFolder(), @"Config");
      string path2 = Path.Combine(CxPath.GetApplicationBinaryFolder(), @"..\..\..\Config");
      string fullFileName = Path.Combine(path1, fileName);
      if ( ! File.Exists(fullFileName) )
      {
        fullFileName = Path.Combine(path2, fileName);
      }
      if (File.Exists(fullFileName))
      {
        return CxXml.LoadDocument(fullFileName);
      }
      else
      {
        throw new ExMetadataException(string.Format("Required metadata file \"{0}\" not found.", fileName),
                                      new IOException(string.Format("File \"{0}\" not found.", fullFileName))); 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads assembly resource file and parses it as XML document.
    /// </summary>
    /// <param name="assembly">assembly to load resource from</param>
    /// <param name="nameSpace">name space</param>
    /// <param name="fileName">name of the XML file</param>
    /// <returns>XML document parsed from file</returns>
    public XmlDocument LoadResourceFile(
      Assembly assembly, 
      string nameSpace,
      string fileName)
    {
      string fileNameSpace =
        CxUtils.Nvl(Path.GetDirectoryName(fileName)).Replace('\\', '.').Replace('/', '.').Trim('.');
      string file = CxUtils.Nvl(Path.GetFileName(fileName)).Trim('\\', '/');
      string fullNameSpace = nameSpace + (CxUtils.NotEmpty(fileNameSpace) ? "." + fileNameSpace : "");
      return CxXml.LoadXmlFromResource(assembly, file, fullNameSpace);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads assembly resource file and parses it as XML document.
    /// </summary>
    /// <param name="fileName">name of the XML file</param>
    /// <returns>XML document parsed from file</returns>
    public XmlDocument LoadResourceFile(string fileName)
    {
      return LoadResourceFile(GetType().Assembly, GetType().Namespace, fileName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Resets the cache of all the row sources registered in the holder.
    /// </summary>
    public void ResetRowSourceCache()
    {
      foreach (CxRowSourceMetadata rowSource in RowSources.RowSources.Values)
      {
        if (rowSource.IsCached)
        {
          rowSource.ResetCache();
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets user-defined metadata cache.
    /// </summary>
    protected internal CxUserMetadataCacheElement GetUserMetadataCache(
      Type metadataObjectType,
      CxMetadataObject parentObject,
      IList predefinedItems)
    {
      if (OnGetUserMetadataCache != null)
      {
        return OnGetUserMetadataCache(
          this, 
          metadataObjectType, 
          parentObject, 
          predefinedItems);
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata configuration.
    /// </summary>
    public CxConfigMetadata Config
    {
      get { return m_Config; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for assemblies.
    /// </summary>
    public CxAssembliesMetadata Assemblies
    {
      get { return m_Assemblies; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for classes.
    /// </summary>
    public CxClassesMetadata Classes 
    {
      get { return m_Classes; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for images.
    /// </summary>
    public CxImagesMetadata Images
    {
      get { return m_Images; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for main menu.
    /// </summary>
    public CxMainMenuMetadata MainMenu
    {
      get { return m_MainMenu; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for entities.
    /// </summary>
    public CxEntitiesMetadata Entities
    {
      get { return m_Entities; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for entity usages.
    /// </summary>
    public CxEntityUsagesMetadata EntityUsages
    {
      get { return m_EntityUsages; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for attributes.
    /// </summary>
    public CxAttributesMetadata Attributes
    {
      get { return m_Attributes; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for attribute usages.
    /// </summary>
    public CxAttributeUsagesMetadata AttributeUsages
    {
      get { return m_AttributeUsages; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for row sources.
    /// </summary>
    public CxRowSourcesMetadata RowSources
    {
      get { return m_RowSources; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for reports.
    /// </summary>
    public CxReportsMetadata Reports
    {
      get { return m_Reports; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for constraints.
    /// </summary>
    public CxConstraintsMetadata Constraints
    {
      get { return m_Constraints; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for portals.
    /// </summary>
    public CxPortalsMetadata Portals
    {
      get { return m_Portals; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for web pages.
    /// </summary>
    public CxPagesMetadata Pages
    {
      get { return m_Pages; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for web pages.
    /// </summary>
    public CxWebPartsMetadata WebParts
    {
      get { return m_WebParts; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for portal skins.
    /// </summary>
    public CxPortalSkinsMetadata PortalSkins
    {
      get { return m_PortalSkins; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for commands.
    /// </summary>
    public CxCommandsMetadata Commands
    {
      get { return m_Commands; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for entity commands.
    /// </summary>
    public CxEntityCommandsMetadata EntityCommands
    {
      get { return m_EntityCommands; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for security settings.
    /// </summary>
    public CxSecurityMetadata Security
    {
      get { return m_Security; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for windows auto-forms description.
    /// </summary>
    public CxWinFormsMetadata WinForms
    {
      get { return m_WinForms; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for windows application navigation tree sections.
    /// </summary>
    public CxWinSectionsMetadata WinSections
    {
      get { return m_WinSections; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for silverlight application navigation tree sections.
    /// </summary>
    public CxSlSectionsMetadata SlSections
    {
      get { return m_SlSections; }
      protected set { m_SlSections = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for silverlight application skins collection.
    /// </summary>
    public CxSlSkinsMetadata SlSkins
    {
      get { return m_SlSkins; }
      protected set { m_SlSkins = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for silverlight application frames.
    /// </summary>
    public CxSlFramesMetadata SlFrames
    {
      get { return m_SlFrames; }
      protected set { m_SlFrames = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Metadata for silverlight Dashboards.
    /// </summary>
    public CxSlDashboardsMetadata SlDashboards
    {
      get { return m_SlDashboards; }
      protected set { m_SlDashboards = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns user permission provider.
    /// </summary>
    public IxUserPermissionProvider UserPermissionProvider
    {
      get
      {
        if (OnGetUserPermissionProvider != null)
        {
          return OnGetUserPermissionProvider();
        }
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the paging feature is allowed to work.
    /// </summary>
    public bool IsPagingAllowed
    {
      get
      {
        if (m_IsPagingAllowed_Cache == null)
        {
          m_IsPagingAllowed_Cache = CxConfigurationHelper.IsPagingAllowed;
        }
        return m_IsPagingAllowed_Cache.Value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the paging feature is allowed to work.
    /// </summary>
    public bool IsCalcAmountOfItemsForChildEntitiesEnabled
    {
      get
      {
        if (m_IsCalcAmountOfItemsForChildEntitiesEnabled_Cache == null)
        {
          m_IsCalcAmountOfItemsForChildEntitiesEnabled_Cache = CxConfigurationHelper.IsCalcAmountOfItemsForChildEntitiesEnabled;
        }
        return m_IsCalcAmountOfItemsForChildEntitiesEnabled_Cache.Value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates new user permission provider instance.
    /// </summary>
    /// <returns>created instance</returns>
    public IxUserPermissionProvider CreateUserPermissionProvider()
    {
      if (OnCreateUserPermissionProvider != null)
      {
        return OnCreateUserPermissionProvider();
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns global application value provider.
    /// </summary>
    public IxValueProvider ApplicationValueProvider
    {
      get
      {
        if (OnGetApplicationValueProvider != null)
        {
          return OnGetApplicationValueProvider();
        }
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity rule cache.
    /// </summary>
    public CxEntityRuleCache EntityRuleCache
    {
      get
      {
        if (OnGetEntityRuleCache != null)
        {
          return OnGetEntityRuleCache();
        }
        return null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of entity usage metadata that is used as a default file/image library.
    /// </summary>
    public string DefaultFileLibraryEntityUsageId
    {
      get { return m_DefaultFileLibraryEntityUsageId; }
      set { m_DefaultFileLibraryEntityUsageId = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default image thumbnail size.
    /// </summary>
    public int ImageSmallThumbnailSize
    { get {return m_ImageSmallThumbnailSize;} set {m_ImageSmallThumbnailSize = value;} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default image thumbnail size.
    /// </summary>
    public int ImageLargeThumbnailSize
    { get {return m_ImageLargeThumbnailSize;} set {m_ImageLargeThumbnailSize = value;} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if application is in development mode (security is not applied).
    /// </summary>
    public bool IsDevelopmentMode
    { 
      get 
      {
        return Security != null && Security.IsDevelopmentMode;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique application code to identify application in DB.
    /// </summary>
    public string ApplicationCode
    {
      get
      {
        if (CxUtils.NotEmpty(CxAppInfo.ApplicationCode))
        {
          return CxAppInfo.ApplicationCode;
        }
        else if (m_Config != null)
        {
          return m_Config.ApplicationCode;
        }
        else if (m_Assemblies != null)
        {
          return m_Assemblies.ApplicationCode;
        }
        return null;
      }
      set
      {
        if (CxUtils.NotEmpty(value))
        {
          CxAppInfo.ApplicationCode = value;
          if (m_Config != null)
          {
            m_Config.ApplicationCode = value;
          }
          else if (m_Assemblies != null)
          {
            m_Assemblies.ApplicationCode = value;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads from database and returns collection of application parameters.
    /// </summary>
    /// <param name="connection">database connection</param>
    public NameValueCollection ReadApplicationParameters(CxDbConnection connection)
    {
      NameValueCollection result = new NameValueCollection();
      string sql = 
        @"SELECT * 
            FROM " + APP_PARAMS_TABLE_NAME + @" 
           WHERE ApplicationCd = :ApplicationCd";
      DataTable dt = new DataTable();
      connection.GetQueryResult(dt, sql, ApplicationCode);
      foreach (DataRow row in dt.Rows)
      {
        result[row["Code"].ToString()] = row["Value"].ToString();
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates DB connection.
    /// </summary>
    public CxDbConnection CreateDbConnection()
    {
      if (m_OnCreateDbConnection != null)
      {
        return m_OnCreateDbConnection();
      }
      else
      {
        throw new ExException("Could not create DB connection. OnCreateDbConnection event handler of metadata holder object is not assigned.");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets record count limit during fetching records from DB.
    /// </summary>
    public int DefaultRecordCountLimit
    {
      get {return m_DefaultRecordCountLimit;}
      set {m_DefaultRecordCountLimit = value;}
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Enables or disables entity data cache.
    /// </summary>
    public bool IsEntityDataCacheEnabled
    {
      get
      {
        return m_IsEntityDataCacheEnabled &&
               OnGetEntityDataCacheObject != null &&
               OnSetEntityDataCacheObject != null;
      }
      set
      {
        m_IsEntityDataCacheEnabled = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity data cache object.
    /// </summary>
    public object GetEntityDataCacheObject(string key)
    {
      if (OnGetEntityDataCacheObject != null)
      {
        return OnGetEntityDataCacheObject(key);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets entity data cache object.
    /// </summary>
    public void SetEntityDataCacheObject(string key, object cachedObject)
    {
      if (OnSetEntityDataCacheObject != null)
      {
        OnSetEntityDataCacheObject(key, cachedObject);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears cache for the given entity ID.
    /// </summary>
    public void ClearEntityIdCache(string entityId)
    {
      if (OnClearEntityIdCache != null)
      {
        OnClearEntityIdCache(entityId);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Prepares value provider before passing to command.
    /// </summary>
    /// <param name="provider">base value provider</param>
    /// <returns>value provider to pass to SQL command</returns>
    public IxValueProvider PrepareValueProvider(IxValueProvider provider)
    {
      CxValueProviderCollection result = new CxValueProviderCollection();
      result.AddIfNotEmpty(provider);
      result.AddIfNotEmpty(ApplicationValueProvider);
      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The method is invoked before metadata object is constructed from the XML element.
    /// </summary>
    /// <param name="metadataObject"></param>
    /// <param name="element"></param>
    internal void DoOnMetadataObjectLoading(
      CxMetadataObject metadataObject, 
      XmlElement element)
    {
      if (OnMetadataObjectLoading != null)
      {
        OnMetadataObjectLoading(this, metadataObject, element);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity group names sorted by name.
    /// </summary>
    public IList<string> GetEntityGroupNameList()
    {
      List<CxEntityMetadata> entityList = new List<CxEntityMetadata>();
      entityList.AddRange(Entities.Items);
      entityList.AddRange(EntityUsages.Items.ConvertAll<CxEntityMetadata>(delegate(CxEntityUsageMetadata entityUsage)
                                                                            { return entityUsage; }));
      Dictionary<string, string> groupNameTable = new Dictionary<string, string>();
      foreach (CxEntityMetadata entity in entityList)
      {
        if (CxUtils.NotEmpty(entity.GroupName) && !groupNameTable.ContainsKey(entity.GroupName))
        {
          groupNameTable[entity.GroupName] = entity.GroupName;
        }
      }

      List<string> groupNameList = new List<string>(groupNameTable.Keys);
      groupNameList.Sort();
      return groupNameList;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Initializes command unique ids.
    /// </summary>
    /// <param name="connection">connection to be used</param>
    public void InitCommandUniqueIds(
      CxDbConnection connection)
    {
      DataTable commandsTable = new DataTable();
      connection.GetQueryResult(commandsTable, "select * from Framework_Commands where ApplicationCd = :APPLICATION$APPLICATIONCODE", new CxHashtable("APPLICATION$APPLICATIONCODE", ApplicationCode));

      foreach (CxCommandMetadata commandMetadata in Commands.Items)
      {
        if (commandMetadata.UniqueID == 0)
        {
          DataRow[] rows = commandsTable.Select(string.Format("CommandId = '{0}'", commandMetadata.Id));
          if (rows.Length == 1)
          {
            commandMetadata.UniqueID = Convert.ToInt32(rows[0]["Unique_Id"]);
          }
          else
          {
            CxDbParameter uniqueIdParameter = connection.CreateParameter("@UniqueId", 0, ParameterDirection.Output,
                                                                         DbType.Int32);
            connection.ExecuteCommandSP(
              "p_GetFrameworkCommandUniqueId",
              connection.CreateParameter("@ApplicationCd", ApplicationCode),
              connection.CreateParameter("@CommandId", commandMetadata.Id),
              uniqueIdParameter);
            commandMetadata.UniqueID = uniqueIdParameter.ValueAsInt;
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: 
    /// initializes multilanguage management object, 
    /// loads multilanguage data from the database.
    /// </summary>
    public void InitMultilanguage(
      CxDbConnection connection,
      string languageCode)
    {
      Multilanguage = new CxMultilanguage(this);
      Multilanguage.LanguageCode = languageCode;
      Multilanguage.ReadFromDatabase(connection, CxAppInfo.LocalizationApplicationCode);
      IsMultilanguageEnabled = true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: 
    /// initializes multilanguage management object, 
    /// loads multilanguage data from the database.
    /// </summary>
    public void InitMultilanguage(CxDbConnection connection)
    {
      string languageCode = ConfigLanguageCode;
      if (CxUtils.NotEmpty(languageCode))
      {
        InitMultilanguage(connection, languageCode);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: 
    /// initializes multilanguage management object, 
    /// loads multilanguage data from the cache.
    /// </summary>
    public void InitMultilanguage(string languageCode, CxHashtable objectProperties, CxHashtable dictionaryValues, CxHashtable localizedValues)
    {
      Multilanguage = new CxMultilanguage(this);
      Multilanguage.ObjectProperties = objectProperties;
      Multilanguage.DictionaryValues = dictionaryValues;
      Multilanguage.LocalizedValues = localizedValues;
      Multilanguage.LanguageCode = languageCode;
      IsMultilanguageEnabled = true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns applic  ation configuration language code.
    /// </summary>
    static public string ConfigLanguageCode
    {
      get
      {
        return CxAppInfo.LanguageCode;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated text.
    /// </summary>
    /// <param name="text">text to translate</param>
    /// <param name="code">code of text (equals to text if empty)</param>
    /// <returns>translated text</returns>
    public string GetTxt(string text, string code)
    {
      return GetTxt(text, code, LanguageCode);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated text.
    /// </summary>
    /// <param name="text">text to translate</param>
    /// <param name="code">code of text (equals to text if empty)</param>
    /// <param name="languageCode">language code</param>
    /// <returns>translated text</returns>
    public string GetTxt(string text, string code, string languageCode)
    {
      if (IsMultilanguageEnabled)
      {
        if (CxUtils.IsEmpty(code))
        {
          code = text;
        }
        return Multilanguage.GetValue(languageCode, CxMultilanguage.OTC_TEXT, CxMultilanguage.PC_TEXT, code, text);
      }
      return text;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated text.
    /// </summary>
    /// <param name="text">text to translate</param>
    /// <param name="code">code if text (equals to text if empty)</param>
    /// <param name="parameters">parameters to substitute into {0}, {1}, ... placeholders</param>
    /// <returns>translated text</returns>
    public string GetTxt(string text, string code, object[] parameters)
    {
      string localizedText = GetTxt(text, code);
      return CxText.Format(localizedText, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated text.
    /// </summary>
    /// <param name="text">text to translate</param>
    /// <param name="parameters">parameters to substitute into {0}, {1}, ... placeholders</param>
    /// <returns>translated text</returns>
    public string GetTxt(string text, object[] parameters)
    {
      return GetTxt(text, null, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated text.
    /// </summary>
    /// <param name="text">text to translate</param>
    /// <returns>translated text</returns>
    public string GetTxt(string text)
    {
      return GetTxt(text, "");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated error message.
    /// </summary>
    /// <param name="text">error message to translate</param>
    /// <param name="code">code if message (equals to message if empty)</param>
    /// <returns>translated message</returns>
    public string GetErr(string text, string code)
    {
      if (IsMultilanguageEnabled)
      {
        if (CxUtils.IsEmpty(code))
        {
          code = text;
        }
        return Multilanguage.GetValue(CxMultilanguage.OTC_ERROR, CxMultilanguage.PC_TEXT, code, text);
      }
      return text;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated error message.
    /// </summary>
    /// <param name="text">error message to translate</param>
    /// <param name="code">code of message (equals to message if empty)</param>
    /// <param name="parameters">parameters to substitute into {0}, {1}, ... placeholders</param>
    /// <returns>translated message</returns>
    public string GetErr(string text, string code, object[] parameters)
    {
      string localizedText = GetErr(text, code);
      return CxText.Format(localizedText, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated error message.
    /// </summary>
    /// <param name="text">error message to translate</param>
    /// <param name="parameters">parameters to substitute into {0}, {1}, ... placeholders</param>
    /// <returns>translated message</returns>
    public string GetErr(string text, object[] parameters)
    {
      return GetErr(text, null, parameters);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Multilanguage: returns translated error message.
    /// </summary>
    /// <param name="text">error message to translate</param>
    /// <returns>translated message</returns>
    public string GetErr(string text)
    {
      return GetErr(text, "");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute presenter instance object.
    /// </summary>
    public CxAttributeTypePresenter AttributeTypePresenter
    {
      get { return m_AttributeTypePresenter; }
      protected set { m_AttributeTypePresenter = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if multilanguage is enabled.
    /// </summary>
    public bool IsMultilanguageEnabled
    {
      get
      {
        return m_IsMultilanguageEnabled && Multilanguage != null;
      }
      set
      {
        m_IsMultilanguageEnabled = value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns multilanguage management object.
    /// </summary>
    public CxMultilanguage Multilanguage
    {
      get { return m_Multilanguage; }
      protected set { m_Multilanguage = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets current language code.
    /// </summary>
    public string LanguageCode
    {
      get
      {
        return Multilanguage != null ? Multilanguage.LanguageCode : CxMultilanguage.DEFAULT_LANGUAGE;
      }
      set
      {
        if (Multilanguage != null)
        {
          Multilanguage.LanguageCode = value;
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns code of application used for localization.
    /// </summary>
    public string LocalizationApplicationCode
    {
      get
      {
        return Multilanguage != null ? Multilanguage.LocalizationApplicationCode : ApplicationCode;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns custom metadata provider (provider to get customized metadata values).
    /// </summary>
    public IxCustomMetadataProvider CustomMetadataProvider
    { get { return m_CustomMetadataProvider; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if metadata customization is enabled.
    /// </summary>
    public bool IsCustomizationEnabled
    {
      get
      {
        if (m_IsCustomizationEnabled_Cache == null)
        {
          m_IsCustomizationEnabled_Cache = CxConfigurationHelper.IsCustomizationEnabled;
        }
        return m_IsCustomizationEnabled_Cache == true && CustomMetadataProvider != null;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if metadata XML element is in the current application scope.
    /// It means that application_scope attribute of the element should be empty
    /// or equal to the application_scope defined in the Config.xml metadata.
    /// </summary>
    /// <param name="element">element to check</param>
    public bool GetIsElementInScope(XmlElement element)
    {
      if (m_Config != null && m_Config.ApplicationScope != NxApplicationScope.All)
      {
        string scopeText = CxXml.GetAttr(element, "application_scope");
        if (CxUtils.NotEmpty(scopeText))
        {
          NxApplicationScope scope = CxEnum.Parse(scopeText, NxApplicationScope.All);
          return scope == NxApplicationScope.All || scope == m_Config.ApplicationScope;
        }
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given application scope is current scope.
    /// </summary>
    /// <param name="scope">scope to check</param>
    public bool IsCurrentScope(NxApplicationScope scope)
    {
      if (m_Config != null && 
          m_Config.ApplicationScope != NxApplicationScope.All &&
          scope != NxApplicationScope.All)
      {
        return m_Config.ApplicationScope == scope;
      }
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Refreshes tree items created by tree item provider classes.
    /// </summary>
    public void RefreshDynamicTreeItems(
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      if (WinSections != null)
      {
        WinSections.RefreshDynamicTreeItems(itemProviderType, entityUsage);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Refreshes tree items created by tree item provider classes.
    /// </summary>
    public void RefreshDynamicTreeItems()
    {
      RefreshDynamicTreeItems(null, null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if lookup cache should be enabled.
    /// </summary>
    public bool IsLookupCacheEnabled
    {
      get
      {
        if (m_IsLookupCacheEnabled_Cache == null)
        {
          m_IsLookupCacheEnabled_Cache = CxConfigurationHelper.IsLookupCacheEnabled;
        }
        return m_IsLookupCacheEnabled_Cache == true;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Component for managing placeholders in the context of the metadata holder.
    /// </summary>
    public CxMetadataPlaceholderManager PlaceholderManager
    {
      get { return m_PlaceholderManager; }
      set { m_PlaceholderManager = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Does actions on row source cached row changed.
    /// </summary>
    /// <param name="rowSource">row source whose cache was changed</param>
    internal void DoOnRowSourceCachedRowChanged(CxRowSourceMetadata rowSource)
    {
      if (OnRowSourceCachedRowChanged != null && rowSource != null)
      {
        OnRowSourceCachedRowChanged(new CxRowSourceCachedRowChangedEventArgs(rowSource));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler that returns user permission provider.
    /// </summary>
    public event DxGetUserPermissionProvider OnGetUserPermissionProvider;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler that creates new user permission provider instance.
    /// </summary>
    public event DxGetUserPermissionProvider OnCreateUserPermissionProvider;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler that returns global application value provider.
    /// </summary>
    public event DxGetApplicationValueProvider OnGetApplicationValueProvider;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler that returns entity rule cache.
    /// </summary>
    public event DxGetEntityRuleCache OnGetEntityRuleCache;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler to return user-defined metadata.
    /// </summary>
    public event DxGetUserMetadataCache OnGetUserMetadataCache;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler to create database connection.
    /// </summary>
    public event DxCreateDbConnection OnCreateDbConnection
    {
      add
      {
        m_OnCreateDbConnection = null;
        m_OnCreateDbConnection += value;
      }
      remove
      {
        m_OnCreateDbConnection -= value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler to get entity data cache object.
    /// </summary>
    public event DxGetEntityDataCacheObject OnGetEntityDataCacheObject;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Event handler to set entity data cache object.
    /// </summary>
    public event DxSetEntityDataCacheObject OnSetEntityDataCacheObject;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Invoked before metadata object is constructed from the XML element.
    /// </summary>
    public event DxMetadataObjectLoading OnMetadataObjectLoading;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Row source cached row changed event.
    /// </summary>
    public event DxRowSourceCachedRowChanged OnRowSourceCachedRowChanged;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Row source cached row changed event.
    /// </summary>
    public event DxClearEntityIdCache OnClearEntityIdCache;
    //----------------------------------------------------------------------------
	}
}