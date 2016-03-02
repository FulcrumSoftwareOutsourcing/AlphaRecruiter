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
using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
  public class CxWinTabMetadata : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    public const string DEFAULT_PANEL_ID = "pnDefault";
    public const string DEFAULT_PANEL_TEXT = "";
    //-------------------------------------------------------------------------
    protected CxWinFormMetadata m_Form = null;
    private List<CxWinPanelMetadata> m_Panels = new List<CxWinPanelMetadata>();
    private List<CxWinTabMetadata> m_ChildTabs = new List<CxWinTabMetadata>();
    private Dictionary<string, CxWinPanelMetadata> m_PanelsMap = new Dictionary<string, CxWinPanelMetadata>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="form">form metadata</param>
    /// <param name="element">XML element to load data from</param>
    public CxWinTabMetadata(
      CxMetadataHolder holder, 
      CxWinFormMetadata form,
      XmlElement element) : base(holder, element)
		{
      m_Form = form;

      XmlNodeList panelNodes = element.SelectNodes("panel");
      if (panelNodes == null)
        throw new ExNullReferenceException("panelNodes");

      foreach (XmlElement panelElement in panelNodes)
      {
        CxWinPanelMetadata panel = new CxWinPanelMetadata(Holder, this, panelElement);
        Add(panel);
      }

      XmlNodeList childTabNodes = element.SelectNodes("tab");
      if (childTabNodes == null)
        throw new ExNullReferenceException("childTabNodes");

      foreach (XmlElement childTabElement in childTabNodes)
      {
        CxWinTabMetadata tab = new CxWinTabMetadata(Holder, m_Form, childTabElement);
        AddChildTab(tab);
      }

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor. Creates tab element with the overridden ID.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="form">form metadata</param>
    /// <param name="element">XML element to load data from</param>
    /// <param name="id">overridden tab ID</param>
    public CxWinTabMetadata(
      CxMetadataHolder holder,
      CxWinFormMetadata form,
      XmlElement element,
      string id) : this(holder, form, element)
    {
      if (CxUtils.NotEmpty(id))
      {
        Id = id;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor. Creates tab element with the given ID containing default panel.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="form">form metadata</param>
    /// <param name="id">tab ID</param>
    public CxWinTabMetadata(
      CxMetadataHolder holder,
      CxWinFormMetadata form,
      string id,
      bool createDefaultPanel) : base(holder)
    {
      m_Form = form;
      Id = id;
      if (createDefaultPanel)
      {
        CxWinPanelMetadata panel = new CxWinPanelMetadata(Holder, this, DEFAULT_PANEL_ID);
        Add(panel);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Seeks for the win tab by its id.
    /// </summary>
    /// <param name="tabs">collection of tabs to seek in</param>
    /// <param name="id">the string id to seek by</param>
    /// <returns>win-tab metadata if any found, otherwise null</returns>
    public static CxWinTabMetadata FindTabById(IList<CxWinTabMetadata> tabs, string id)
    {
      foreach (CxWinTabMetadata tab in tabs)
      {
        if (string.Equals(tab.Id, id, StringComparison.OrdinalIgnoreCase))
          return tab;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds panel to the collection.
    /// </summary>
    public void Add(CxWinPanelMetadata panel)
    {
      Panels.Add(panel);
      PanelsMap.Add(panel.Id, panel);
      m_Form.AddPanel(panel);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds panel to the collection.
    /// </summary>
    public void AddChildTab(CxWinTabMetadata tab)
    {
      ChildTabs.Add(tab);
      //m_Form.AddTab(tab);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);
      LoadChildTabsOverride(element);
      LoadPanelsOverride(element);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads panels override.
    /// </summary>
    /// <param name="element">XML element to load overridden panels from</param>
    public void LoadPanelsOverride(XmlElement element)
    {
      XmlNodeList panelNodes = element.SelectNodes("panel");
      if (panelNodes == null)
        throw new ExNullReferenceException("panelNodes");

      foreach (XmlElement panelElement in panelNodes)
      {
        CxWinPanelMetadata panel = m_Form.FindPanel(CxXml.GetAttr(panelElement, "id"));
        if (panel != null)
        {
          panel.LoadOverride(panelElement);
        }
        else
        {
          panel = new CxWinPanelMetadata(Holder, this, panelElement);
          Add(panel);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads panels override.
    /// </summary>
    /// <param name="element">XML element to load overridden panels from</param>
    public void LoadChildTabsOverride(XmlElement element)
    {
      XmlNodeList childTabNodes = element.SelectNodes("tab");
      if (childTabNodes == null)
        throw new ExNullReferenceException("childTabNodes");

      foreach (XmlElement childTabElement in childTabNodes)
      {
        CxWinTabMetadata childTab = m_Form.FindTab(CxXml.GetAttr(childTabElement, "id"));
        if (childTab != null)
        {
          childTab.LoadOverride(childTabElement);
        }
        else
        {
          childTab = new CxWinTabMetadata(Holder, Form, childTabElement);
          AddChildTab(childTab);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns tab caption translated in the context of the entity usage.
    /// </summary>
    /// <param name="entityMetadata">entity or entity usage context</param>
    /// <returns>tab caption translated in the context of the entity usage</returns>
    public string GetCaption(CxEntityMetadata entityMetadata)
    {
      return GetCaption(entityMetadata, null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns tab caption translated in the context of the entity usage.
    /// </summary>
    /// <param name="entityMetadata">entity or entity usage context</param>
    /// <param name="languageCd">language code to be used, null if default</param>
    /// <returns>tab caption translated in the context of the entity usage</returns>
    public string GetCaption(CxEntityMetadata entityMetadata, string languageCd)
    {
      string localizedCaption = null;
      string nonLocalizedPropertyValue = GetNonLocalizedPropertyValue("text");
      if (Holder.Multilanguage != null)
      {
        string languageCode = languageCd ?? Holder.Multilanguage.LanguageCode;
        CxEntityUsageMetadata entityUsage = entityMetadata as CxEntityUsageMetadata;
        while (localizedCaption == null && entityUsage != null)
        {
          localizedCaption = Holder.Multilanguage.GetLocalizedValue(
            languageCode,
            LocalizationObjectTypeCode,
            "text",
            entityUsage.Id + "." + Form.Id + "." + Id,
            nonLocalizedPropertyValue);
          entityUsage = entityUsage.InheritedEntityUsage;
        }
      }
      if (localizedCaption != null)
      {
        return localizedCaption;
      }
      else
      {
        if (CxText.Equals(languageCd, Holder.LanguageCode))
          return DisplayText;
        else
          return nonLocalizedPropertyValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text of the tab.
    /// </summary>
    public string DisplayText
    { get { return Id != Text ? Text : ""; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Display order of the tab.
    /// </summary>
    public int DisplayOrder
    { get { return CxInt.Parse(this["display_order"], int.MaxValue); } }
      //-------------------------------------------------------------------------
    /// <summary>
    /// Returns form metadata.
    /// </summary>
    public CxWinFormMetadata Form
    { get { return m_Form; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all tab panels.
    /// </summary>
    public IList<CxWinPanelMetadata> Panels
    { get { return m_Panels; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all child tabs.
    /// </summary>
    public IList<CxWinTabMetadata> ChildTabs
    {
      get { return m_ChildTabs; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent metadata object.
    /// </summary>
    public override CxMetadataObject ParentObject
    {
      get
      {
        foreach (CxWinTabMetadata tabMetadata in Form.Tabs)
        {
          if (tabMetadata == this)
            return Form;
          foreach (CxWinTabMetadata childTabMetadata in tabMetadata.ChildTabs)
          {
            if (childTabMetadata == this)
              return tabMetadata;
          }
        }
        return Form;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WinTab";
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
        return Form != null ? Form.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //-------------------------------------------------------------------------
    public Dictionary<string, CxWinPanelMetadata> PanelsMap
    {
      get { return m_PanelsMap; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns panel metadata by the given panel ID.
    /// </summary>
    /// <param name="id">panel ID</param>
    public CxWinPanelMetadata FindPanel(string id)
    {
      string upperId = CxText.ToUpper(id);
      if (m_PanelsMap.ContainsKey(upperId))
        return m_PanelsMap[upperId];
      else 
        return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Renders the metadata object to its XML representation.
    /// </summary>
    /// <param name="document">the document to render into</param>
    /// <param name="custom">indicates whether just customization should be rendered</param>
    /// <param name="tagNameToUse">the tag name to be used while rendering</param>
    /// <returns>the rendered object</returns>
    public override CxXmlRenderedObject RenderToXml(XmlDocument document, bool custom, string tagNameToUse)
    {
      CxXmlRenderedObject result = base.RenderToXml(document, custom, tagNameToUse);

      foreach (CxWinPanelMetadata panelMetadata in Panels)
      {
        CxXmlRenderedObject renderedAttribute = panelMetadata.RenderToXml(document, custom, "panel");
        if (!renderedAttribute.IsEmpty)
        {
          result.Element.AppendChild(renderedAttribute.Element);
          result.IsEmpty = false;
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata object from the given XML element.
    /// </summary>
    /// <param name="element">the element to load from</param>
    public override void LoadCustomMetadata(XmlElement element)
    {
      base.LoadCustomMetadata(element);

      if (element != null)
      {
        foreach (XmlElement panelElement in element.SelectNodes("panel"))
        {
          string panelId = CxXml.GetAttr(panelElement, "id");
          if (m_PanelsMap.ContainsKey(panelId))
          {
            CxWinPanelMetadata panel = m_PanelsMap[panelId];
            panel.LoadCustomMetadata(panelElement);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}