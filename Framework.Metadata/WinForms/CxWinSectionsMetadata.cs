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
  public class CxWinSectionsMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    public const string PROPERTY_WIN_SECTION_VISIBLE_ORDER = "win_section_visible_order";
    public const string PROPERTY_KNOWN_WIN_SECTIONS = "known_win_sections";
    //----------------------------------------------------------------------------
    protected List<CxWinSectionMetadata> m_ItemList = new List<CxWinSectionMetadata>(); // Sections list
    protected Dictionary<string, CxWinSectionMetadata> m_ItemMap = new Dictionary<string, CxWinSectionMetadata>(); // Sections dictionary
    protected CxWinSectionMetadata m_Default;
    private CxWinSectionOrder m_WinSectionOrder;
    protected UniqueList<string> m_NewSectionNames;
    private Dictionary<string, string> m_CustomProperties = new Dictionary<string, string>();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">document to read sections metadata</param>
    public CxWinSectionsMetadata(CxMetadataHolder holder, XmlDocument doc)
      : base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxWinSectionsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reinitialize some sections property.
    /// </summary>
    public void ReInit()
    {
      m_NewSectionNames = null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("section"))
      {
        CxWinSectionMetadata section = new CxWinSectionMetadata(Holder, element);
        m_ItemList.Add(section);
        m_ItemMap.Add(section.Id, section);
      }
      LoadOverrides(doc, "section_override", m_ItemMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after metadata loaded.
    /// </summary>
    override protected void DoAfterLoad()
    {
      base.DoAfterLoad();
      // Determine default section.
      foreach (CxWinSectionMetadata section in m_ItemList)
      {
        if (section.IsDefault)
        {
          m_Default = section;
          break;
        }
      }
      if (m_Default == null && m_ItemList.Count > 0)
      {
        m_Default = m_ItemList[0];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds the section by id.
    /// </summary>
    /// <param name="id">section id</param>
    /// <returns>section metadata object or null</returns>
    public CxWinSectionMetadata Find(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        string sectionId = id.ToUpper();
        if (m_ItemMap.ContainsKey(sectionId))
        {
          CxWinSectionMetadata section = m_ItemMap[sectionId];
          return section;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Refreshes tree items created by tree item provider classes.
    /// </summary>
    public void RefreshDynamicTreeItems(
      Type itemProviderType,
      CxEntityUsageMetadata entityUsage)
    {
      foreach (CxWinSectionMetadata section in m_ItemList)
      {
        section.Items.RefreshDynamicItems(itemProviderType, entityUsage);
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
    /// Section with the given ID.
    /// </summary>
    public CxWinSectionMetadata this[string id]
    {
      get
      {
        CxWinSectionMetadata section = Find(id);
        if (section != null)
          return section;
        else
          throw new ExMetadataException(string.Format("Section with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of available sections (allowed by security settings).
    /// </summary>
    public IList<CxWinSectionMetadata> Items
    {
      get 
      {
        List<CxWinSectionMetadata> sections = new List<CxWinSectionMetadata>();
        foreach (CxWinSectionMetadata section in m_ItemList)
        {
          if (section.Visible)
          {
            sections.Add(section);
          }
        }
        return sections; 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of available sections (allowed by security settings).
    /// </summary>
    public IList<CxWinSectionMetadata> AllItemsListWithoutHiddenForUser
    {
      get
      {
        List<CxWinSectionMetadata> sections = new List<CxWinSectionMetadata>();
        foreach (CxWinSectionMetadata section in m_ItemList)
        {
          if (!section.IsHiddenForUser)
          {
            sections.Add(section);
          }
        }
        return sections;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of available sections (allowed by security settings).
    /// </summary>
    public IList<CxWinSectionMetadata> AllItemsListJustHiddenForUser
    {
      get
      {
        List<CxWinSectionMetadata> sections = new List<CxWinSectionMetadata>();
        foreach (CxWinSectionMetadata section in m_ItemList)
        {
          if (section.IsHiddenForUser)
          {
            sections.Add(section);
          }
        }
        return sections;
      }
    }
    //----------------------------------------------------------------------------
    protected string GetCustomProperty(string propName)
    {
      string propValue;
      m_CustomProperties.TryGetValue(propName, out propValue);

      return propValue;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of all registered sections independent from security settings.
    /// </summary>
    public IList<CxWinSectionMetadata> AllItemsList
    { get { return m_ItemList; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Map of all registered sections independent from security settings.
    /// </summary>
    public Dictionary<string, CxWinSectionMetadata> AllItems
    { get { return m_ItemMap; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default section.
    /// </summary>
    public CxWinSectionMetadata Default
    { 
      get 
      {
        IList<CxWinSectionMetadata> items = WinSectionOrder.OrderPlusNewSections;
        foreach (CxWinSectionMetadata section in items)
        {
          if (section.IsDefault)
          {
            return section;
          }
        }
        if (items.Count > 0)
        {
          return items[0];
        }
        return null;
      } 
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "WinSections.xml"; } }
    //----------------------------------------------------------------------------
    public CxWinSectionOrder WinSectionOrder
    {
      get
      {
        if(m_WinSectionOrder == null)
        {
          m_WinSectionOrder = new CxWinSectionOrder(this);
          string sectionVisibleOrderProperty = GetCustomProperty(PROPERTY_WIN_SECTION_VISIBLE_ORDER);
          if (!string.IsNullOrEmpty(sectionVisibleOrderProperty))
          {
            if (!CxList.CompareOrdered(m_WinSectionOrder.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(sectionVisibleOrderProperty)))
              m_WinSectionOrder.SetCustomOrder(sectionVisibleOrderProperty);
          }
        }
        return m_WinSectionOrder;
      }
      protected set { m_WinSectionOrder = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of section ids built on the ground of the given sections.
    /// </summary>
    /// <param name="sections">sections to be used to build the result</param>
    /// <returns>list of section ids</returns>
    public IList<string> GetIdsFromSection(IEnumerable<CxWinSectionMetadata> sections)
    {
      UniqueList<string> result = new UniqueList<string>();
      foreach (CxWinSectionMetadata section in sections)
      {
        result.Add(section.Id);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of sections built on the ground of the given sections ids.
    /// </summary>
    /// <param name="sectionIds">section ids to be used to build the result</param>
    /// <returns>list of sections</returns>
    public IList<CxWinSectionMetadata> GetSectionsFromIds(IEnumerable<string> sectionIds)
    {
      // Validate
      if (sectionIds == null)
        throw new ExNullArgumentException("sectionIds");

      UniqueList<CxWinSectionMetadata> result = new UniqueList<CxWinSectionMetadata>();
      foreach (string sectionId in sectionIds)
      {
        if (AllItems.ContainsKey(sectionId))
        {
          CxWinSectionMetadata section = this[sectionId];
          result.Add(section);
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns section names for new added sections that are not
    /// included into the section order.
    /// </summary>
    protected internal UniqueList<string> NewSectionNames
    {
      get
      {
        if (m_NewSectionNames == null)
        {
          m_NewSectionNames = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
          string customProperty = GetCustomProperty(PROPERTY_KNOWN_WIN_SECTIONS);
          if (CxUtils.NotEmpty(customProperty))
          {
            IList<string> list = CxText.DecomposeWithSeparator(CxUtils.ToString(customProperty), ",");
            UniqueList<string> oldSectionList = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
            oldSectionList.AddRange(CxList.ToList<string>(list));
            foreach (CxWinSectionMetadata section in Items)
            {
              if (!section.IsHiddenForUser && !oldSectionList.Contains(section.Id))
              {
                m_NewSectionNames.Add(section.Id);
              }
            }
          }
        }
        return m_NewSectionNames;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata objects from the given XML documents sorted by object IDs.
    /// </summary>
    public void LoadCustomMetadata(IDictionary<string, XmlDocument> documents)
    {
      XmlDocument value;
      if(documents.TryGetValue("singleton", out value))
      {
        if (value.DocumentElement != null && value.DocumentElement.Name == "win_sections_custom")
        {
          foreach (XmlAttribute attr in value.DocumentElement.FirstChild.Attributes)
          {
            m_CustomProperties[attr.Name] = attr.Value;
          }
          LoadCustomMetadata(value.DocumentElement);
        }
      }

      foreach (KeyValuePair<string, XmlDocument> pair in documents)
      {
        CxWinSectionMetadata sectionMetadata = Find(pair.Key);
        if (sectionMetadata != null)
        {
          foreach (XmlElement element in pair.Value.DocumentElement.SelectNodes("win_section_custom"))
          {
            sectionMetadata.LoadCustomMetadata(element);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata object from the given XML element.
    /// </summary>
    /// <param name="element">the element to load from</param>
    public void LoadCustomMetadata(XmlElement element)
    {
      
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Renders the metadata object to its XML representation.
    /// </summary>
    /// <param name="document">the document to render into</param>
    /// <param name="custom">indicates whether just customization should be rendered</param>
    /// <param name="tagName">the tag name to be used while rendering</param>
    /// <returns>the rendered object</returns>
    public CxXmlRenderedObject RenderToXml(XmlDocument document, bool custom, object tagName)
    {
      CxXmlRenderedObject result = new CxXmlRenderedObject();
      result.IsEmpty = true;
      result.Element = document.CreateElement("win_sections", string.Empty);

      Dictionary<string, string> propertiesToRender = new Dictionary<string, string>();
      string propertiesToRemove = "";

      bool isCustomWinSectionOrderSaved = false;
      string customProperty = GetCustomProperty(PROPERTY_WIN_SECTION_VISIBLE_ORDER);
      if (WinSectionOrder.IsCustom)
      {
        propertiesToRender[PROPERTY_WIN_SECTION_VISIBLE_ORDER] = CxText.ComposeCommaSeparatedString(WinSectionOrder.OrderIds);
        isCustomWinSectionOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(customProperty))
      {
        propertiesToRemove = PROPERTY_WIN_SECTION_VISIBLE_ORDER;
      }

      if (isCustomWinSectionOrderSaved)
      {
        propertiesToRender[PROPERTY_KNOWN_WIN_SECTIONS] =
          CxText.ComposeCommaSeparatedString(CxMetadataObject.ExtractIds(AllItemsListWithoutHiddenForUser));
      }

      if (!string.IsNullOrEmpty(propertiesToRemove))
      {
        XmlAttribute attribute = result.Element.Attributes[propertiesToRemove];
        if (attribute != null)
          result.Element.Attributes.Remove(attribute);
      }

      foreach (KeyValuePair<string, string> pair in propertiesToRender)
      {
        XmlAttribute attribute = document.CreateAttribute(pair.Key);
        attribute.Value = pair.Value;
        result.Element.Attributes.Append(attribute);
        result.IsEmpty = false;
      }

      return result;
    }
    //-------------------------------------------------------------------------
  }
}