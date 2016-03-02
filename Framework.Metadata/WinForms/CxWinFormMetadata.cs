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
using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinFormMetadata : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    public const string DEFAULT_TAB_ID = "tpDefault";
    public const string DEFAULT_TAB_TEXT = "General";
    //-------------------------------------------------------------------------
    protected List<CxWinTabMetadata> m_Tabs = new List<CxWinTabMetadata>();
    protected Dictionary<string, CxWinTabMetadata> m_TabsMap = 
      new Dictionary<string, CxWinTabMetadata>();
    protected List<CxWinPanelMetadata> m_Panels = new List<CxWinPanelMetadata>();
    protected Dictionary<string, CxWinPanelMetadata> m_PanelsMap = 
      new Dictionary<string, CxWinPanelMetadata>();
    private CxWinTabOrderManager m_TabOrderManager;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load data from</param>
    /// <param name="parentCollection">the collection of forms owning the current form</param>
    public CxWinFormMetadata(
      CxMetadataHolder holder, 
      XmlElement element,
      CxWinFormsMetadata parentCollection)
      : base(holder, element)
		{
      XmlNodeList panelList = element.SelectNodes("panel");
      if (panelList == null)
        throw new ExNullReferenceException("panelList");

      if (panelList.Count > 0)
      {
        CxWinTabMetadata tab = new CxWinTabMetadata(Holder, this, element, DEFAULT_TAB_ID);
        Add(tab);
      }
      
      XmlNodeList tabNodeList = element.SelectNodes("tab");
      if (tabNodeList == null)
        throw new ExNullReferenceException("tabNodeList");

      foreach (XmlElement tabElement in tabNodeList)
      {
        CxWinTabMetadata tab = new CxWinTabMetadata(Holder, this, tabElement);
        Add(tab);
      }

      CxWinFormMetadata inheritedFrom = parentCollection.Find(InheritedFromId);
      InheritTabsFrom(inheritedFrom);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inherits the tabs from the given form metadata to the current form.
    /// </summary>
    /// <param name="formMetadata">the form metadata to inherit from</param>
    private void InheritTabsFrom(CxWinFormMetadata formMetadata)
    {
      // Here we "inherit" from the other form if defined.
      if (formMetadata != null)
      {
        InheritPropertiesFrom(new List<CxMetadataObject>(new CxMetadataObject[] { formMetadata }));
        foreach (CxWinTabMetadata tabMetadata in formMetadata.Tabs)
        {
          CxWinTabMetadata existingTabMetadata = FindTab(tabMetadata.Id);
          if (existingTabMetadata != null)
          {
            existingTabMetadata.InheritPropertiesFrom(tabMetadata);
            InheritPanelsFrom(tabMetadata, existingTabMetadata);
          }
          else
          {
            CxWinTabMetadata tabMetadataCopy = new CxWinTabMetadata(Holder, this, tabMetadata.Id, false);
            tabMetadataCopy.InheritPropertiesFrom(new CxMetadataObject[] { tabMetadata });
            InheritPanelsFrom(tabMetadata, tabMetadataCopy);
            Add(tabMetadataCopy);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inherits the panels from the given tab to the given tab.
    /// </summary>
    /// <param name="tabMetadataFrom">the tab to inherit from</param>
    /// <param name="tabMetadataTo">the tab to inherit to</param>
    private void InheritPanelsFrom(CxWinTabMetadata tabMetadataFrom, CxWinTabMetadata tabMetadataTo)
    {
      foreach (CxWinPanelMetadata panelMetadata in tabMetadataFrom.Panels)
      {
        CxWinPanelMetadata existingPanelMetadata = FindPanel(panelMetadata.Id);
        if (existingPanelMetadata != null)
        {
          existingPanelMetadata.InheritPropertiesFrom(panelMetadata);
        }
        else
        {
          CxWinPanelMetadata panelMetadataCopy = new CxWinPanelMetadata(Holder, tabMetadataTo, panelMetadata.Id);
          panelMetadataCopy.InheritPropertiesFrom(panelMetadata);
          tabMetadataTo.Add(panelMetadataCopy);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor. Creates win form metadata with default settings.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxWinFormMetadata(CxMetadataHolder holder) : base(holder)
    {
      CxWinTabMetadata tab = new CxWinTabMetadata(Holder, this, DEFAULT_TAB_ID, true);
      Add(tab);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the tab order class instance capable of managing the order of 
    /// the tabs inside the form.
    /// </summary>
    public CxWinTabOrderManager GetTabOrderManager()
    {
      if (TabOrderManager == null)
        TabOrderManager = new CxWinTabOrderManager(this);
      return TabOrderManager;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds tab metadata to the collection.
    /// </summary>
    protected void Add(CxWinTabMetadata tab)
    {
      m_Tabs.Add(tab);
      m_TabsMap.Add(tab.Id, tab);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds panel metadata to the collection.
    /// </summary>
    internal void AddPanel(CxWinPanelMetadata panel)
    {
      m_Panels.Add(panel);
      if (m_PanelsMap.ContainsKey(panel.Id))
        throw new ExException(string.Format("Form <{0}> already contains the panel <{1}>", Id, panel.Id));
      m_PanelsMap.Add(panel.Id, panel);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns tab metadata by the given tab ID.
    /// </summary>
    /// <param name="id">tab ID</param>
    public CxWinTabMetadata FindTab(string id)
    {
      string upperId = CxText.ToUpper(id);
      return m_TabsMap.ContainsKey(upperId) ? m_TabsMap[upperId] : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns panel metadata by the given panel ID.
    /// </summary>
    /// <param name="id">panel ID</param>
    public CxWinPanelMetadata FindPanel(string id)
    {
      string upperId = CxText.ToUpper(id);
      return m_PanelsMap.ContainsKey(upperId) ? m_PanelsMap[upperId] : null;
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);

      XmlNodeList panelList = element.SelectNodes("panel");
      
      if (panelList == null)
        throw new ExNullReferenceException("panelList");
      
      if (panelList.Count > 0 && m_Tabs.Count > 0)
      {
        (m_Tabs[0]).LoadPanelsOverride(element);
      }

      XmlNodeList tabNodeList = element.SelectNodes("tab");
      if (tabNodeList == null)
        throw new ExNullReferenceException("tabNodeList");

      foreach (XmlElement tabElement in tabNodeList)
      {
        CxWinTabMetadata tab = FindTab(CxXml.GetAttr(tabElement, "id"));
        if (tab != null)
        {
          tab.LoadOverride(tabElement);
        }
        else
        {
          tab = new CxWinTabMetadata(Holder, this, tabElement);
          Add(tab);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all tabs.
    /// </summary>
    public IList<CxWinTabMetadata> Tabs
    { get { return m_Tabs; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all form panels.
    /// </summary>
    public IList<CxWinPanelMetadata> Panels
    { get { return m_Panels; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if text captions should be aligned in bounds of each group
    /// separately. Otherwise, text is aligned in bounds of the whole form.
    /// </summary>
    public bool TextAlignInGroups
    { get { return this["text_align_in_groups"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if child grid panel has a splitter.
    /// </summary>
    public bool ChildGridSplitter
    { get { return this["child_grid_splitter"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the win-form metadata is customizable.
    /// </summary>
    public bool Customizable
    {
      get { return CxBool.Parse(this["customizable"], true); }
      set { this["customizable"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Points out the id of the form the current form is inherited from.
    /// </summary>
    public string InheritedFromId
    {
      get { return this["inherited_from_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Points out the form the current form is inherited from.
    /// </summary>
    public CxWinFormMetadata InheritedFrom
    {
      get
      {
        if (Holder.WinForms != null)
          return Holder.WinForms.Find(InheritedFromId);
        else
          return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Tab order string, comma-separeted list.
    /// </summary>
    public string CustomTabOrder
    {
      get { return this["tab_order"]; }
      set { this["tab_order"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Tab order, an instance of a class that manages the order of the tabs inside
    /// the form.
    /// </summary>
    protected CxWinTabOrderManager TabOrderManager
    {
      get { return m_TabOrderManager; }
      set { m_TabOrderManager = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the XML tag name applicable for the current metadata object.
    /// </summary>
    public override string GetTagName()
    {
      return "form";
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
      foreach (CxWinTabMetadata tabMetadata in Tabs)
      {
        CxXmlRenderedObject renderedAttribute = tabMetadata.RenderToXml(document, custom, "tab");
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
        foreach (XmlElement tabElement in element.SelectNodes("tab"))
        {
          string tabId = CxXml.GetAttr(tabElement, "id");
          if (m_TabsMap.ContainsKey(tabId))
          {
            CxWinTabMetadata tab = m_TabsMap[tabId];
            tab.LoadCustomMetadata(tabElement);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}