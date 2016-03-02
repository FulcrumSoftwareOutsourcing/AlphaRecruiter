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
using System.Xml;

using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Option type enumeration.
  /// </summary>
  public enum NxOptionType { WindowsOption, WebOption, CommonOption, LocalOption }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Option scope enumeration.
  /// </summary>
  public enum NxOptionScope { CurrentUser, AllUsers }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Class incapsulating application options management.
  /// </summary>
  public class CxOptions
  {
    //-------------------------------------------------------------------------
    protected const string OPTIONS_ROOT = "Options";
    //-------------------------------------------------------------------------
    protected const string ATTR_OPTION_TYPE = "OPTION_TYPE";
    protected const string ATTR_OPTION_SCOPE = "OPTION_SCOPE";
    //-------------------------------------------------------------------------
    protected CxSettingsStorage m_CommonAllUsersStorage = null;
    protected CxSettingsStorage m_CommonCurrentUserStorage = null;
    protected CxSettingsStorage m_WindowsAllUsersStorage = null;
    protected CxSettingsStorage m_WindowsCurrentUserStorage = null;
    protected CxSettingsStorage m_WebAllUsersStorage = null;
    protected CxSettingsStorage m_WebCurrentUserStorage = null;
    protected CxSettingsStorage m_LocalAllUsersStorage;
    protected CxSettingsStorage m_LocalCurrentUserStorage;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxOptions()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes options instance before use.
    /// </summary>
    /// <param name="commonAllUsersStorage"></param>
    /// <param name="commonCurrentUserStorage"></param>
    /// <param name="windowsAllUsersStorage"></param>
    /// <param name="windowsCurrentUserStorage"></param>
    /// <param name="webAllUsersStorage"></param>
    /// <param name="webCurrentUserStorage"></param>
    /// <param name="localAllUsersStorage"></param>
    /// <param name="localCurrentUserStorage"></param>
    public void Initialize(
      CxSettingsStorage commonAllUsersStorage,
      CxSettingsStorage commonCurrentUserStorage,
      CxSettingsStorage windowsAllUsersStorage,
      CxSettingsStorage windowsCurrentUserStorage,
      CxSettingsStorage webAllUsersStorage,
      CxSettingsStorage webCurrentUserStorage,
      CxSettingsStorage localAllUsersStorage,
      CxSettingsStorage localCurrentUserStorage)
    {
      m_CommonAllUsersStorage = commonAllUsersStorage;
      m_CommonCurrentUserStorage = commonCurrentUserStorage;
      m_WindowsAllUsersStorage = windowsAllUsersStorage;
      m_WindowsCurrentUserStorage = windowsCurrentUserStorage;
      m_WebAllUsersStorage = webAllUsersStorage;
      m_WebCurrentUserStorage = webCurrentUserStorage;
      m_LocalAllUsersStorage = localAllUsersStorage;
      m_LocalCurrentUserStorage = localCurrentUserStorage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns storage for the option type and scope.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="optionScope">option scope</param>
    /// <returns>settings storage</returns>
    public CxSettingsStorage GetStorage(
      NxOptionType optionType, NxOptionScope optionScope)
    {
      if (optionType == NxOptionType.CommonOption && optionScope == NxOptionScope.AllUsers)
      {
        return m_CommonAllUsersStorage;
      }
      else if (optionType == NxOptionType.CommonOption && optionScope == NxOptionScope.CurrentUser)
      {
        return m_CommonCurrentUserStorage;
      }
      else if (optionType == NxOptionType.WindowsOption && optionScope == NxOptionScope.AllUsers)
      {
        return m_WindowsAllUsersStorage;
      }
      else if (optionType == NxOptionType.WindowsOption && optionScope == NxOptionScope.CurrentUser)
      {
        return m_WindowsCurrentUserStorage;
      }
      else if (optionType == NxOptionType.WebOption && optionScope == NxOptionScope.AllUsers)
      {
        return m_WebAllUsersStorage;
      }
      else if (optionType == NxOptionType.WebOption && optionScope == NxOptionScope.CurrentUser)
      {
        return m_WebCurrentUserStorage;
      }
      else if (optionType == NxOptionType.LocalOption && optionScope == NxOptionScope.AllUsers)
      {
        return m_LocalAllUsersStorage;
      }
      else if (optionType == NxOptionType.LocalOption && optionScope == NxOptionScope.CurrentUser)
      {
        return m_LocalCurrentUserStorage;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns option value by the given key.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="optionScope">option scope</param>
    /// <param name="keyName">key to get value for</param>
    /// <param name="defValue">default value to return if value not found</param>
    /// <returns>found value or default value</returns>
    public string GetValue(
      NxOptionType optionType, NxOptionScope optionScope, string keyName, string defValue)
    {
      CxSettingsStorage storage = GetStorage(optionType, optionScope);
      if (storage != null)
      {
        return storage.Read(OPTIONS_ROOT, keyName, defValue);
      }
      return defValue;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets option value.
    /// </summary>
    /// <param name="optionType">option type</param>
    /// <param name="optionScope">option scope</param>
    /// <param name="keyName">key name to set value for</param>
    /// <param name="value">value to set</param>
    public void SetValue(
      NxOptionType optionType, NxOptionScope optionScope, string keyName, string value)
    {
      CxSettingsStorage storage = GetStorage(optionType, optionScope);
      if (storage != null)
      {
        storage.Write(OPTIONS_ROOT, keyName, value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Copies options values to the given entity.
    /// </summary>
    /// <param name="entity">entity to copy to</param>
    public void CopyToEntity(CxBaseEntity entity)
    {
      foreach (CxAttributeMetadata attr in entity.Metadata.Attributes)
      {
        NxOptionType optionType;
        NxOptionScope optionScope;
        if (CxEnum.Parse(attr[ATTR_OPTION_TYPE], out optionType) &&
            CxEnum.Parse(attr[ATTR_OPTION_SCOPE], out optionScope))
        {
          string defValue = CxCommon.ObjectToString(CxUtils.Nvl(entity[attr.Id], ""));
          string value = GetValue(optionType, optionScope, attr.Id, defValue);
          try
          {
            entity[attr.Id] = CxCommon.StringToObject(value, attr.GetPropertyType());
          }
          catch 
          {
            // Suppress string to object convertation exception.
            // If option value is invalid just ignore it.
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Copies values from the given entity to the options.
    /// </summary>
    /// <param name="entity">entity to copy from</param>
    public void CopyFromEntity(CxBaseEntity entity)
    {
      foreach (CxAttributeMetadata attr in entity.Metadata.Attributes)
      {
        if (attr.Storable)
        {
          NxOptionType optionType;
          NxOptionScope optionScope;
          if (CxEnum.Parse(attr[ATTR_OPTION_TYPE], out optionType) &&
              CxEnum.Parse(attr[ATTR_OPTION_SCOPE], out optionScope))
          {
            string value = CxCommon.ObjectToString(CxUtils.Nvl(entity[attr.Id], ""));
            SetValue(optionType, optionScope, attr.Id, value);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves option section of the storage to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="storage">storage to save</param>
    protected void SaveToDatabase(CxDbConnection connection, CxSettingsStorage storage)
    {
      if (storage is CxDbSettingsStorage)
      {
        ((CxDbSettingsStorage)storage).SaveSection(connection, OPTIONS_ROOT);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves options to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void SaveToDatabase(CxDbConnection connection)
    {
      bool ownsTransaction = !connection.InTransaction;
      if (ownsTransaction)
      {
        connection.BeginTransaction();
      }
      try
      {
        SaveToDatabase(connection, m_CommonAllUsersStorage);
        SaveToDatabase(connection, m_CommonCurrentUserStorage);
        SaveToDatabase(connection, m_WindowsAllUsersStorage);
        SaveToDatabase(connection, m_WindowsCurrentUserStorage);
        SaveToDatabase(connection, m_WebAllUsersStorage);
        SaveToDatabase(connection, m_WebCurrentUserStorage);
        if (ownsTransaction)
        {
          connection.Commit();
        }
      }
      catch (Exception e)
      {
        if (ownsTransaction)
        {
          connection.Commit();
        }
        throw new ExException(e.Message, e);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns option groups XML description.
    /// </summary>
    /// <returns></returns>
    virtual public XmlDocument GetOptionGroupsXml()
    {
      return CxXml.LoadXmlFromResource(GetType().Assembly, "OptionGroups.xml");
    }
    //-------------------------------------------------------------------------

    #region Properties to Get Option Values
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if auto-update is enabled.
    /// </summary>
    public bool AutoUpdateSchedule
    {
      get 
      { 
        return 
          CxBool.Parse(GetValue(
            NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.AutoUpdateSchedule, "Yes")) &&
          AutoUpdateScheduleInterval > 0; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns auto-update interval in minutes.
    /// </summary>
    public int AutoUpdateScheduleInterval
    {
      get
      {
        return CxInt.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.AutoUpdateScheduleInterval, "120"), 0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if database version check is enabled.
    /// </summary>
    public bool AutoUpdateDbVersionCheck
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.AutoUpdateDbVersionCheck, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minimal interval for database version check.
    /// </summary>
    public int AutoUpdateDbVersionCheckMinInterval
    {
      get
      {
        return CxInt.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.AutoUpdateDbVersionCheckMinInterval, "0"), 0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default value for keyboard navigation timer delay.
    /// </summary>
    public int KeyboardNavigationDelay
    {
      get
      {
        return CxInt.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.KeyboardNavigationDelay, "250"), 0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity icons should be displayed in hyperlink controls in grid.
    /// </summary>
    public bool DisplayEntityIconsInHyperlinksInGrid
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.DisplayEntityIconsInHyperlinksInGrid, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if entity icons should be displayed in hyperlink controls in form.
    /// </summary>
    public bool DisplayEntityIconsInHyperlinksInForm
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.DisplayEntityIconsInHyperlinksInForm, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if navigation tree lines are visible.
    /// </summary>
    public bool NavigationTreeLines
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.NavigationTreeLines, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns amount of Recent Items visible.
    /// </summary>
    public int AmountOfRecentItemsVisible
    {
      get
      {
        return CxInt.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.AmountOfRecentItemsVisible, "10"), 10);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the amount of items for each tab is shown.
    /// </summary>
    public bool ShowTheAmountOfItemsForEachTab
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.ShowTheAmountOfItemsForEachTab, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns Current Language.
    /// </summary>
    public string CurrentLanguage
    {
      get
      {
        return GetValue(NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.CurrentLanguage, "EN");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if grid horizontal lines are visible.
    /// </summary>
    public bool GridHorizontalLines
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridHorizontalLines, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if grid vertical lines are visible.
    /// </summary>
    public bool GridVerticalLines
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridVerticalLines, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridEnableGrouping
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridEnableGrouping, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridEnableLocalFilter
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridEnableLocalFilter, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowGroupPanelMaster
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowGroupPanelMaster, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowGroupPanelDetail
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowGroupPanelDetail, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowLocalFilterRowMaster
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowLocalFilterRowMaster, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowLocalFilterRowDetail
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowLocalFilterRowDetail, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowSummaryRow
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowSummaryRow, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowGroupSummaryRow
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowGroupSummaryRow, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowNavigator
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowNavigator, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridShowIndicator
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridShowIndicator, "Yes"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool GridDisplayAlternateRows
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.GridDisplayAlternateRows, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public bool UsePaging
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.UsePaging, "No"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public int PagingLowerBound
    {
      get
      {
        return int.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.PagingLowerBound, "50"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default option for grid.
    /// </summary>
    public int PagingUpperBound
    {
      get
      {
        return int.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.PagingUpperBound, "100"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the Enterprise Customization should have 
    /// the Default entity caption shown along with the other captions.
    /// </summary>
    public bool ShowDefaultEntityCaption
    {
      get
      {
        return CxBool.Parse(GetValue(
                              NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.ShowDefaultEntityCaption,
                              "true"));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// </summary>
    public NxAutomaticallyApplyChangesToDatabaseWhenMovingOffEntity AutomaticallyApplyChangesToDatabaseWhenMovingOffEntity
    {
      get
      {
        return (NxAutomaticallyApplyChangesToDatabaseWhenMovingOffEntity) CxInt.Parse(GetValue(
                              NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.AutomaticallyApplyChangesToDatabaseWhenMovingOffEntity,
                              "0"), 0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// An option for Enterprise Security section.
    /// </summary>
    public bool StrongPassword
    {
      get
      {
        return CxBool.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.AllUsers, CxOptionCodes.StrongPassword, "No"), false);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// An option for the User Communication section.
    /// </summary>
    public int OpenEmailAndWebHyperlink
    {
      get
      {
        return CxInt.Parse(GetValue(
          NxOptionType.WindowsOption, NxOptionScope.CurrentUser, CxOptionCodes.OpenEmailAndWebHyperlink, "0"), 0);
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Singleton implementation
    //-------------------------------------------------------------------------
    private static IxOptionStore m_Store;
    private static object m_LockObject = new object();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets a new instance of the application options if no instance was set before.
    /// </summary>
    /// <param name="optionsInstance">the instance to be set</param>
    /// <param name="store">the store to be used</param>
    /// <returns>the options instance</returns>
    public static CxOptions SetInstanceIfNull(CxOptions optionsInstance, IxOptionStore store)
    {
      lock (m_LockObject)
      {
        if (m_Store == null)
          m_Store = store;
        if (m_Store.Read() == null)
          m_Store.Save(optionsInstance);
        return m_Store.Read();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The application options object instance.
    /// </summary>
    public static CxOptions Instance
    {
      get { return m_Store != null ? m_Store.Read() : null; }
    }
    //-------------------------------------------------------------------------
    #endregion
  }
}