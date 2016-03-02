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
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Metadata for particular tab of a portal page
	/// </summary>
	public class CxTabMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    public const string DEFAULT_TAB_ID = "DEFAULT";
    public const string DEFAULT_TAB_TEXT = "General";
    //-------------------------------------------------------------------------
    protected CxPageMetadata m_Page = null;
    protected CxWebPartsCollection m_WebParts = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder</param>
    /// <param name="page">parent page for tab</param>
    /// <param name="element">XML element to load data from</param>
		public CxTabMetadata(
      CxMetadataHolder holder, 
      XmlElement element,
      CxPageMetadata page) : base(holder, element)
		{
      m_Page = page;
      if (element.SelectSingleNode("visibility_condition") != null)
      {
        AddNodeToProperties(element, "visibility_condition");
      }
      LoadWebPartsFrom(element);
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder</param>
    /// <param name="page">parent page for tab</param>
    public CxTabMetadata(
      CxMetadataHolder holder, 
      CxPageMetadata page) : base(holder)
    {
      m_Page = page;
      m_Id = DEFAULT_TAB_ID;
      this["text"] = DEFAULT_TAB_TEXT;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads web part collection from the given XML element.
    /// </summary>
    /// <param name="element">XML element to load web parts</param>
    public void LoadWebPartsFrom(XmlElement element)
    {
      m_WebParts = new CxWebPartsCollection(this, element);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);
      LoadWebPartsOverrideFrom(element);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads web part collection override from the given XML element.
    /// </summary>
    /// <param name="element">XML element to load web parts</param>
    public void LoadWebPartsOverrideFrom(XmlElement element)
    {
      foreach (XmlElement wpElement in element.SelectNodes("web_part"))
      {
        CxWebPartMetadata webPart = m_WebParts.FindById(CxXml.GetAttr(wpElement, "id"));
        if (webPart != null)
        {
          webPart.LoadOverride(wpElement);
        }
        else
        {
          webPart = m_WebParts.CreateWebPart(this, wpElement);
          m_WebParts.Add(webPart);
        }
      }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Collection of tab web parts.
    /// </summary>
    public CxWebPartsCollection WebParts
    { get {return m_WebParts;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parent tab page.
    /// </summary>
    public CxPageMetadata Page
    { get {return m_Page;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata to take security settings from.
    /// </summary>
    public CxEntityUsageMetadata TabSecurityEntityUsage
    {
      get
      {
        return Holder != null && Holder.Security != null ? 
          Holder.Security.GetSecurityEntityUsage(this) : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks tab access permission depending on security settings.
    /// </summary>
    /// <param name="connection">Db connection to check security rule</param>
    /// <param name="entityValueProvider">entity instance value provider</param>
    public bool GetIsVisible(
      CxDbConnection connection,
      IxValueProvider entityValueProvider)
    {
      if (Holder != null && Holder.Security != null)
      {
        CxEntityUsageMetadata entityUsage = TabSecurityEntityUsage;
        IxValueProvider provider = 
          entityUsage != null && entityValueProvider != null ? 
          entityUsage.PrepareValueProvider(entityValueProvider) : null;
        return Holder.Security.GetRight(this, entityUsage, null, connection, provider);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if page focus should be set on load.
    /// </summary>
    public bool IsFocusSetOnLoad
    { get {return this["set_focus_on_load"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines visibility condition of the tab.
    /// </summary>
    public string VisibilityCondition
    { get {return this["visibility_condition"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage ID to check visibility condition for.
    /// </summary>
    public string VisibilityConditionEntityUsageId
    { get {return this["visibility_condition_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage to check visibility condition for.
    /// </summary>
    public CxEntityUsageMetadata VisibilityConditionEntityUsage
    { 
      get 
      {
        return CxUtils.NotEmpty(VisibilityConditionEntityUsageId) ? 
          Holder.EntityUsages[VisibilityConditionEntityUsageId] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if tab is scrollable.
    /// </summary>
    public bool IsScrollable
    { get {return this["is_scrollable"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns localized value of the property 
    /// or null if localized value not found (or localization is disabled)
    /// </summary>
    /// <param name="propertyName">name of the property</param>
    /// <param name="propertyValue">value of the property</param>
    /// <param name="languageCode">language code</param>
    override public string GetLocalizedPropertyValue(
      string propertyName, 
      string propertyValue,
      string languageCode)
    {
      if (CxText.Equals(propertyName, "Text") &&
          GetNonLocalizedPropertyValue(propertyName) == DEFAULT_TAB_TEXT &&
          base.IsPropertyLocalizable(propertyName))
      {
        return Holder.GetTxt("General", "Metadata.DefaultWebTabText");
      }
      return base.GetLocalizedPropertyValue(propertyName, propertyValue, languageCode);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if property with the given name should be localizable.
    /// </summary>
    override public bool IsPropertyLocalizable(string propertyName)
    {
      bool isLocalizable = base.IsPropertyLocalizable(propertyName);
      if (isLocalizable &&
          CxText.Equals(propertyName, "TEXT") &&
          GetNonLocalizedPropertyValue(propertyName) == DEFAULT_TAB_TEXT)
      {
        isLocalizable = false;
      }
      return isLocalizable;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WebTab";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return Page != null ? Page.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //----------------------------------------------------------------------------
  }
}