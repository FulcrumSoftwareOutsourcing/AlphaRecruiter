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
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Metadata for portal page.
	/// </summary>
	public class CxPageMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected CxTabsCollection m_Tabs = null;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load page from</param>
		public CxPageMetadata(
      CxMetadataHolder holder, 
      XmlElement element) : base(holder, element)
		{
      int tabCount = element.SelectNodes("tab").Count;
      int webPartCount = element.SelectNodes("web_part").Count;
      m_Tabs = new CxTabsCollection(this);
      if (webPartCount > 0)
      {
        CxTabMetadata defaultTab = new CxTabMetadata(Holder, this);
        defaultTab.LoadWebPartsFrom(element);
        m_Tabs.Add(defaultTab);
      }
      if (tabCount > 0)
      {
        m_Tabs.AddFrom(element);
      }
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);

      if (m_Tabs != null)
      {
        int tabCount = element.SelectNodes("tab").Count;
        int webPartCount = element.SelectNodes("web_part").Count;
        if (webPartCount > 0 && m_Tabs.Count > 0)
        {
          m_Tabs[0].LoadWebPartsOverrideFrom(element);
        }
        if (tabCount > 0)
        {
          foreach (XmlElement tabElement in element.SelectNodes("tab"))
          {
            CxTabMetadata tab = m_Tabs.Find(CxXml.GetAttr(tabElement, "id"));
            if (tab != null)
            {
              tab.LoadOverride(tabElement);
            }
            else
            {
              tab = new CxTabMetadata(Holder, tabElement, this);
              m_Tabs.Add(tab);
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Collection of page tabs.
    /// </summary>
    public CxTabsCollection Tabs
    { get {return m_Tabs;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default tab of the page.
    /// </summary>
    public CxTabMetadata DefaultTab
    {
      get
      {
        return m_Tabs.Count > 0 ? m_Tabs[0] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if page state 'back' stack should be cleared 
    /// before page displaying.
    /// </summary>
    public bool IsPageStackShouldBeCleared
    { get {return this["clear_history"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if page state 'back' stack should be cleared 
    /// before page displaying.
    /// </summary>
    public bool IsStoredInPageStack
    { get {return this["save_history"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if the page is a default application startup page.
    /// </summary>
    public bool IsDefault
    { get {return this["is_default"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if page is secured, i.e. SSL is required.
    /// </summary>
    public bool IsSecured
    { get {return this["is_secured"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if page focus should be set on load.
    /// </summary>
    public bool IsFocusSetOnLoad
    { get {return this["set_focus_on_load"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the portal template that should be used on the page.
    /// Is used to override default portal template for some pages.
    /// </summary>
    public string PortalTemplateControl
    { get {return this["portal_template_control"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if page is scrollable.
    /// </summary>
    public bool IsScrollable
    { get {return this["is_scrollable"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WebPage";
      }
    }
    //----------------------------------------------------------------------------
  }
}