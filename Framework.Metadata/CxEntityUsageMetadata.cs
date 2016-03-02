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
using System.ComponentModel;
using System.Security.Cryptography;
using System.Xml;
using System.Data;
using System.Text;

using Framework.Utils;
using Framework.Db;

namespace Framework.Metadata
{
  //------------------------------------------------------------------------------
  /// <summary>
  /// Class to hold information about application entity usage.
  /// </summary>
  public class CxEntityUsageMetadata : CxEntityMetadata
  {
    //----------------------------------------------------------------------------
    public const string LOCALIZATION_OBJECT_TYPE_CODE = "Metadata.EntityUsage";
    public const string PROPERTY_GRID_ORDER = "grid_visible_order";
    public const string PROPERTY_EDIT_ORDER = "edit_order";
    public const string PROPERTY_FILTER_ORDER = "filter_order";
    public const string PROPERTY_QUERY_ORDER = "query_order";
    public const string PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST = "child_entity_usage_order_in_list";
    public const string PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW = "child_entity_usage_order_in_view";
    //----------------------------------------------------------------------------
    private CxEntityUsagesMetadata m_EntityUsages; // Global collection of entity usages
    protected Dictionary<string, CxChildEntityUsageMetadata> m_ChildEntityUsages = new Dictionary<string, CxChildEntityUsageMetadata>(); // Dictionary of child entity usages
    protected IList<CxChildEntityUsageMetadata> m_ChildEntityUsagesList = new List<CxChildEntityUsageMetadata>(); // Ordered list child entity usages
    protected IList<string> m_OrderedAttributeNames; // List of attribute names with overriden order
    protected IList<CxEntityUsageToEditMetadata> m_EntityUsagesToEdit = new List<CxEntityUsageToEditMetadata>(); // List of entity usages to edit instead of this one
    protected IList<string> m_OrderedCommandNames; // Ordered list of command names.
    protected bool m_IsOrderedCommandNamesDefined;

    protected List<CxCommandMetadata> m_OrderedCommandList; // Ordered list of commands.
    protected CxGenericDataTable m_EmptyDataTable; // Empty data table.
    // Lock objects to get cached entity data.
    protected Hashtable m_EntityDataCacheLockObjectMap = new Hashtable();
    // Descendant entity usage cache.
    protected IList<CxEntityUsageMetadata> m_DirectDescendantEntityUsages;
    protected List<CxEntityUsageMetadata> m_DescendantEntityUsages;
    // Empty entity instance, for internal purposes
    protected object m_EntityInstance;
    // Cached list of attributes
    protected List<CxAttributeMetadata> m_Attributes;
    // Cached flag
    private readonly object m_AttributeLock = new object();
    private CxEntityUsageMetadata m_InheritedEntityUsage;
    private bool m_IsInheritedEntityUsageCached;
    private CxEntityMetadata m_Entity;
    private bool m_IsEntityCached;
    private readonly Dictionary<NxChildEntityUsageOrderType, CxChildEntityUsageOrder> m_WinChildEntityUsageOrder
      = new Dictionary<NxChildEntityUsageOrderType, CxChildEntityUsageOrder>();
    protected UniqueList<string> m_NewChildEntityUsageNames;
    //----------------------------------------------------------------------------
    /// <summary>
    /// A list of property names that are not inheritable.
    /// </summary>
    public override List<string> NonInheritableProperties
    {
      get
      {
        if (m_NonInheritableProperties == null)
        {
          m_NonInheritableProperties = new List<string>(base.NonInheritableProperties);
          m_NonInheritableProperties.Add("is_queryable");
        }
        return m_NonInheritableProperties;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether an explicit command order is defined.
    /// </summary>
    public bool IsOrderedCommandNamesDefined
    {
      get { return m_IsOrderedCommandNamesDefined; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the child entity usage order of the requested type.
    /// </summary>
    /// <param name="orderType">the type of the order</param>
    public CxChildEntityUsageOrder GetChildEntityUsageOrder(NxChildEntityUsageOrderType orderType)
    {
      CxChildEntityUsageOrder result;
      if (m_WinChildEntityUsageOrder.ContainsKey(orderType))
        result = m_WinChildEntityUsageOrder[orderType];
      else
      {
        result = m_WinChildEntityUsageOrder[orderType] = new CxChildEntityUsageOrder(this, orderType);
      }
      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">a metadata holder the usage belongs to</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="entityUsages">a collection of entity usages the usage should be added to</param>
    public CxEntityUsageMetadata(
      CxMetadataHolder holder,
      XmlElement element,
      CxEntityUsagesMetadata entityUsages)
      : base(holder, element)
    {
      m_EntityUsages = entityUsages;

      AddNodeToProperties(element, "where_clause");
      AddNodeToProperties(element, "join_condition");

      // Load child entity usages
      XmlElement childrenEntitiesElement = (XmlElement) element.SelectSingleNode("child_entity_usages");
      if (childrenEntitiesElement != null)
      {
        LoadChildEntityUsages(childrenEntitiesElement, false);
      }

      // Load description of entity usages to edit 
      XmlElement entitiesToEditElement = (XmlElement) element.SelectSingleNode("entity_usages_to_edit");
      if (entitiesToEditElement != null)
      {
        string importFromId = CxXml.GetAttr(entitiesToEditElement, "import_from");
        if (CxUtils.NotEmpty(importFromId))
        {
          CxEntityUsageMetadata importFromEntityUsage = Holder.EntityUsages[importFromId];
          foreach (CxEntityUsageToEditMetadata edit in importFromEntityUsage.EntityUsagesToEdit)
          {
            m_EntityUsagesToEdit.Add(edit);
          }
        }
        else
        {
          XmlNodeList nodes = entitiesToEditElement.SelectNodes("entity_usage_to_edit");
          if (nodes != null)
          {
            foreach (XmlElement editElement in nodes)
            {
              CxEntityUsageToEditMetadata edit = new CxEntityUsageToEditMetadata(Holder, editElement);
              m_EntityUsagesToEdit.Add(edit);
            }
          }
        }
      }
      CreateChildEntityUsageOrder();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxEntityUsageMetadata(
      CxMetadataHolder holder,
      CxEntityUsageMetadata baseEntityUsage)
      : base(holder)
    {
      CopyPropertiesFrom(baseEntityUsage);
      CreateChildEntityUsageOrder();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads children entity usages from the given XML element.
    /// </summary>
    /// <param name="childrenEntitiesElement"></param>
    protected void LoadChildEntityUsages(XmlElement childrenEntitiesElement, bool doLoadOverrides)
    {
      string importFromId = CxXml.GetAttr(childrenEntitiesElement, "import_from");
      if (CxUtils.NotEmpty(importFromId))
      {
        CxEntityUsageMetadata importFromEntityUsage = m_EntityUsages[importFromId];
        foreach (CxChildEntityUsageMetadata child in importFromEntityUsage.ChildEntityUsagesList)
        {
          if (!ChildEntityUsages.ContainsKey(child.Id))
          {
            AddChild(child);
          }
        }
      }
      else
      {
        XmlNodeList nodes = childrenEntitiesElement.SelectNodes("child_entity_usage");
        if (nodes != null)
        {
          foreach (XmlElement childElement in nodes)
          {
            string childId = CxXml.GetAttr(childElement, "id").ToUpper();
            if (!ChildEntityUsages.ContainsKey(childId))
            {
              CxChildEntityUsageMetadata child = new CxChildEntityUsageMetadata(Holder, childElement, this);
              AddChild(child);
            }
            else if (doLoadOverrides)
            {
              CxChildEntityUsageMetadata child = ChildEntityUsages[childId];
              child.LoadOverride(childElement);
            }
          }
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates attribute order objects.
    /// </summary>
    protected void CreateChildEntityUsageOrder()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the XML tag name applicable for the current metadata object.
    /// </summary>
    public override string GetTagName()
    {
      return "entity_usage";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Seeks for entity usages that contain the given entity usage in their "child_entity_usages" reference.
    /// </summary>
    /// <returns>a list of entity usages found</returns>
    public IList<CxEntityUsageMetadata> FindParentEntityUsages()
    {
      List<CxEntityUsageMetadata> parentEntityUsages = new List<CxEntityUsageMetadata>();
      foreach (CxEntityUsageMetadata entityUsage in Entity.Holder.EntityUsages.Items)
      {
        foreach (CxChildEntityUsageMetadata childEntityUsageMetadata in entityUsage.ChildEntityUsagesList)
        {
          if (childEntityUsageMetadata.EntityUsage == this && !parentEntityUsages.Contains(entityUsage))
            parentEntityUsages.Add(entityUsage);
        }
      }
      return parentEntityUsages;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Renders the metadata object to its XML representation.
    /// </summary>
    /// <param name="document">the document to render into</param>
    /// <param name="custom">indicates whether just customization should be rendered</param>
    /// <param name="tagName">the tag name to be used while rendering</param>
    /// <returns>the rendered object</returns>
    public override CxXmlRenderedObject RenderToXml(XmlDocument document, bool custom, string tagName)
    {
      Dictionary<string, string> propertiesToRender = new Dictionary<string, string>();
      List<string> propertiesToRemove = new List<string>();

      bool isOneOrderSaved = false;
      CxAttributeOrder orderGrid = GetAttributeOrder(NxAttributeContext.GridVisible);
      if (orderGrid.IsCustom)
      {
        propertiesToRender[PROPERTY_GRID_ORDER] = CxUtils.Nvl(CxText.ComposeCommaSeparatedString(orderGrid.OrderIds), " ");
        isOneOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(this[PROPERTY_GRID_ORDER]))
      {
        propertiesToRemove.Add(PROPERTY_GRID_ORDER);
      }

      CxAttributeOrder orderEdit = GetAttributeOrder(NxAttributeContext.Edit);
      if (orderEdit.IsCustom)
      {
        propertiesToRender[PROPERTY_EDIT_ORDER] = CxUtils.Nvl(CxText.ComposeCommaSeparatedString(orderEdit.OrderIds), " ");
        isOneOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(this[PROPERTY_EDIT_ORDER]))
      {
        propertiesToRemove.Add(PROPERTY_EDIT_ORDER);
      }

      CxAttributeOrder orderFilter = GetAttributeOrder(NxAttributeContext.Filter);
      if (orderFilter.IsCustom)
      {
        propertiesToRender[PROPERTY_FILTER_ORDER] = CxUtils.Nvl(CxText.ComposeCommaSeparatedString(orderFilter.OrderIds), " ");
        isOneOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(this[PROPERTY_FILTER_ORDER]))
      {
        propertiesToRemove.Add(PROPERTY_FILTER_ORDER);
      }

      CxAttributeOrder orderQuery = GetAttributeOrder(NxAttributeContext.Queryable);
      if (orderQuery.IsCustom)
      {
        propertiesToRender[PROPERTY_QUERY_ORDER] = CxUtils.Nvl(CxText.ComposeCommaSeparatedString(orderQuery.OrderIds), " ");
        isOneOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(this[PROPERTY_QUERY_ORDER]))
      {
        propertiesToRemove.Add(PROPERTY_QUERY_ORDER);
      }

      if (isOneOrderSaved)
      {
        // This property should be set BEFORE the object properties are rendered.
        KnownAttributesString = CxText.ComposeCommaSeparatedString(ExtractIds(Attributes));
        //propertiesToRender["known_attributes"] = CxText.ComposeCommaSeparatedString(ExtractIds(Attributes));
      }

      bool isCustomChildEntityUsageOrderSaved = false;

      // 1. Main form
      CxChildEntityUsageOrder childEntityUsageOrder_InList = GetChildEntityUsageOrder(NxChildEntityUsageOrderType.InList);
      if (childEntityUsageOrder_InList.IsCustom)
      {
        propertiesToRender[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST] = CxText.ComposeCommaSeparatedString(childEntityUsageOrder_InList.OrderIds);
        isCustomChildEntityUsageOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(this[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST]))
      {
        propertiesToRemove.Add(PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST);
      }
      // 2. Edit form
      CxChildEntityUsageOrder childEntityUsageOrder_InView = GetChildEntityUsageOrder(NxChildEntityUsageOrderType.InView);
      if (childEntityUsageOrder_InView.IsCustom)
      {
        propertiesToRender[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW] = CxText.ComposeCommaSeparatedString(childEntityUsageOrder_InView.OrderIds);
        isCustomChildEntityUsageOrderSaved = true;
      }
      else if (!string.IsNullOrEmpty(this[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW]))
      {
        propertiesToRemove.Add(PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW);
      }

      if (isCustomChildEntityUsageOrderSaved)
      {
        propertiesToRender["known_child_entity_usages"] = CxText.ComposeCommaSeparatedString(ExtractIds(ChildEntityUsagesList));
      }

      // Here we call the base method writing down all the current object properties
      // so they should be in the proper state at this moment in order to write them down.
      CxXmlRenderedObject result = base.RenderToXml(document, custom, tagName);

      foreach (string propertyName in propertiesToRemove)
      {
        XmlAttribute attribute = result.Element.Attributes[propertyName];
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

      XmlElement attributeUsagesElement = document.CreateElement("attribute_usages");
      bool isAttributeUsagesNonEmpty = false;
      foreach (string attributeId in ExtractIds(Attributes))
      {
        CxAttributeMetadata attributeMetadata = GetAttribute(attributeId);

        CxXmlRenderedObject renderedAttribute = attributeMetadata.RenderToXml(document, custom, "attribute_usage");
        if (!renderedAttribute.IsEmpty)
        {
          attributeUsagesElement.AppendChild(renderedAttribute.Element);
          isAttributeUsagesNonEmpty = true;
        }
      }
      if (isAttributeUsagesNonEmpty)
      {
        result.Element.AppendChild(attributeUsagesElement);
        result.IsEmpty = false;
      }

      return result;
    }
    //-------------------------------------------------------------------------
    public string GridOrderCustom
    {
      get { return this["grid_visible_order_custom"]; }
      set { this["grid_visible_order_custom"] = value; }
    }
    //-------------------------------------------------------------------------
    public string EditOrderCustom
    {
      get { return this["edit_order_custom"]; }
      set { this["edit_order_custom"] = value; }
    }
    //-------------------------------------------------------------------------
    public string FilterOrderCustom
    {
      get { return this["filter_order_custom"]; }
      set { this["filter_order_custom"] = value; }
    }
    //-------------------------------------------------------------------------
    public string QueryOrderCustom
    {
      get { return this["query_order_custom"]; }
      set { this["query_order_custom"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads custom metadata object from the given XML element.
    /// </summary>
    /// <param name="element">the element to load from</param>
    public override void LoadCustomMetadata(XmlElement element)
    {
      base.LoadCustomMetadata(element);

      string gridOrder = this["grid_visible_order"];
      if (!string.IsNullOrEmpty(gridOrder))
      {
        CxAttributeOrder orderGrid = GetAttributeOrder(NxAttributeContext.GridVisible);
        if (!CxList.CompareOrdered(orderGrid.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(gridOrder)))
          orderGrid.SetCustomOrder(gridOrder);
        this["grid_visible_order"] = GetInitialProperty("grid_visible_order");
      }

      string editOrder = this["edit_order"];
      if (!string.IsNullOrEmpty(editOrder))
      {
        CxAttributeOrder orderEdit = GetAttributeOrder(NxAttributeContext.Edit);
        if (!CxList.CompareOrdered(orderEdit.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(editOrder)))
          orderEdit.SetCustomOrder(editOrder);
        this["edit_order"] = GetInitialProperty("edit_order");
      }

      string filterOrder = this["filter_order"];
      if (!string.IsNullOrEmpty(filterOrder))
      {
        CxAttributeOrder orderFilter = GetAttributeOrder(NxAttributeContext.Filter);
        if (!CxList.CompareOrdered(orderFilter.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(filterOrder)))
          orderFilter.SetCustomOrder(filterOrder);
        this["filter_order"] = GetInitialProperty("filter_order");
      }

      string queryOrder = this["query_order"];
      if (!string.IsNullOrEmpty(queryOrder))
      {
        CxAttributeOrder orderQuery = GetAttributeOrder(NxAttributeContext.Queryable);
        if (!CxList.CompareOrdered(orderQuery.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(queryOrder)))
          orderQuery.SetCustomOrder(queryOrder);
        this["query_order"] = GetInitialProperty("query_order");
      }

      // 1. Main form
      string childEntityUsageOrder_InList = this[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST];
      if (!string.IsNullOrEmpty(childEntityUsageOrder_InList))
      {
        CxChildEntityUsageOrder orderChildEntityUsage = GetChildEntityUsageOrder(NxChildEntityUsageOrderType.InList);
        if (!CxList.CompareOrdered(orderChildEntityUsage.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(childEntityUsageOrder_InList)))
          orderChildEntityUsage.SetCustomOrder(childEntityUsageOrder_InList);
        this[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST] = GetInitialProperty(PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_LIST);
      }
      // 2. Edit form
      string childEntityUsageOrder_InView = this[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW];
      if (!string.IsNullOrEmpty(childEntityUsageOrder_InView))
      {
        CxChildEntityUsageOrder orderChildEntityUsage = GetChildEntityUsageOrder(NxChildEntityUsageOrderType.InView);
        if (!CxList.CompareOrdered(orderChildEntityUsage.OrderIds, CxText.DecomposeWithWhiteSpaceAndComma(childEntityUsageOrder_InView)))
          orderChildEntityUsage.SetCustomOrder(childEntityUsageOrder_InView);
        this[PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW] = GetInitialProperty(PROPERTY_CHILD_ENTITY_USAGE_ORDER_IN_VIEW);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);

      XmlElement childrenEntitiesElement = (XmlElement) element.SelectSingleNode("child_entity_usages");
      if (childrenEntitiesElement != null)
      {
        LoadChildEntityUsages(childrenEntitiesElement, true);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds command to the list.
    /// </summary>
    /// <param name="command">command to add</param>
    override public void AddCommand(CxCommandMetadata command)
    {
      base.AddCommand(command);

      // Copy command properties from base entity or inherited entity usage.
      CxCommandMetadata parentCommand;
      if (InheritedEntityUsage != null)
      {
        parentCommand = InheritedEntityUsage.GetCommand(command.Id);
        if (parentCommand != null)
        {
          command.CopyPropertiesFrom(parentCommand);
        }
      }
      parentCommand = Entity.GetCommand(command.Id);
      if (parentCommand != null)
      {
        command.CopyPropertiesFrom(parentCommand);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// "Registers" child entity usage.
    /// </summary>
    /// <param name="child">child entity usage to add</param>
    protected void AddChild(CxChildEntityUsageMetadata child)
    {
      ChildEntityUsages.Add(child.Id, child);
      m_ChildEntityUsagesList.Add(child);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Dictionary of child entity usages.
    /// </summary>
    public Dictionary<string, CxChildEntityUsageMetadata> ChildEntityUsages
    {
      get { return m_ChildEntityUsages; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ordered list child entity usages.
    /// </summary>
    public IList<CxChildEntityUsageMetadata> ChildEntityUsagesList
    {
      get { return m_ChildEntityUsagesList; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns child entity usage by ID. If not found raises exception.
    /// </summary>
    /// <param name="childId">ID of child entity usage</param>
    /// <returns>child entity usage by ID</returns>
    public CxChildEntityUsageMetadata GetChildEntityUsage(string childId)
    {
      string upperChildId = CxText.ToUpper(childId);
      CxChildEntityUsageMetadata child = null;
      if (ChildEntityUsages.ContainsKey(upperChildId))
        child = ChildEntityUsages[upperChildId];

      if (child != null)
      {
        return child;
      }
      else
      {
        throw new ExMetadataException(string.Format(
           "Entity usage <{0}> does not have child entity usage <{1}>", Id, childId));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of entity usages to edit instead of this one.
    /// </summary>
    public IList<CxEntityUsageToEditMetadata> EntityUsagesToEdit
    {
      get { return m_EntityUsagesToEdit; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of entity this usage based on.
    /// </summary>
    public string EntityId
    {
      get { return this["entity_id"].ToUpper(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity this usage based on.
    /// </summary>
    public CxEntityMetadata Entity
    {
      get
      {
        if (!m_IsEntityCached)
        {
          m_Entity = Holder.Entities[EntityId];
          m_IsEntityCached = true;
        }
        return m_Entity;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of entity usage this usage based on.
    /// </summary>
    public string InheritedEntityUsageId
    {
      get
      {
        return this["inherited_entity_usage_id"].ToUpper();
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity usage this usage based on.
    /// </summary>
    public CxEntityUsageMetadata InheritedEntityUsage
    {
      get
      {
        if (!m_IsInheritedEntityUsageCached)
        {
          if (CxUtils.NotEmpty(InheritedEntityUsageId) && InheritedEntityUsageId != Id)
          {
            CxEntityUsageMetadata entityUsage = EntityUsages.Find(InheritedEntityUsageId);
            if (entityUsage == null)
            {
              throw new ExMetadataException(
                String.Format(
                  "Inherited entity usage with ID=\"{0}\" could not be found. " +
                  "Probably it is not defined or defined after the dependent entity usage with ID=\"{1}\".",
                  InheritedEntityUsageId, Id));
            }
            m_InheritedEntityUsage = entityUsage;
          }
          m_IsInheritedEntityUsageCached = true;
        }
        return m_InheritedEntityUsage;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of menu item to use security rights from.
    /// </summary>
    public string SecurityMenuItemId
    {
      get { return this["security_menu_item_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of base entity usage for this one.
    /// </summary>
    public string BaseEntityUsageId
    {
      get { return this["base_entity_usage_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Base entity usage for this one.
    /// </summary>
    public CxEntityUsageMetadata BaseEntityUsage
    {
      get
      {
        return (CxUtils.NotEmpty(BaseEntityUsageId) ?
                Holder.EntityUsages[BaseEntityUsageId] :
                null);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity should be used in Bookmarks and Recent Items lists on behalf of another entity metadata.
    /// Notice, the reference cannot go deeper than just one hop. No complex circular references possible.
    /// </summary>
    public string BookmarksAndRecentItemsEntityMetadataId
    {
      get { return this["bookmarks_and_recent_items_entity_metadata_id"]; }
    }
    //-------------------------------------------------------------------------
    public CxEntityUsageMetadata BookmarksAndRecentItemsEntityMetadata
    {
      get
      {
        if (!string.IsNullOrEmpty(BookmarksAndRecentItemsEntityMetadataId))
        {
          var entityMetadata = Holder.EntityUsages.Find(BookmarksAndRecentItemsEntityMetadataId);
          if (entityMetadata != null)
            return entityMetadata;
        }
        return this;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of entity usage this one should use main menu properties of.
    /// </summary>
    public string MainMenuEntityUsageId
    {
      get { return this["main_menu_entity_usage_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if this entity usage may be added to the hot item list.
    /// </summary>
    public bool Hottable
    {
      get
      {
        string s = this["hottable"].ToLower();
        return (CxUtils.NotEmpty(s) ? (s == "true") :
                BaseEntityUsage != null ? BaseEntityUsage.Hottable :
                                          false);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Join condition to link entity usage to parent.
    /// </summary>
    public string JoinCondition
    {
      get { return this["join_condition"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Additional where clause.
    /// </summary>
    public string WhereClause
    {
      get { return this["where_clause"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if grid should has grouping by default.
    /// </summary>
    public bool GridGrouping
    {
      get { return (this["grid_grouping"].ToLower() != "false"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if grid sorting should be enabled.
    /// </summary>
    public bool GridSorting
    {
      get { return (this["grid_sorting"].ToLower() != "false"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if master entity should be refreshed on detail change.
    /// </summary>
    public bool RefreshMaster
    {
      get { return (this["refresh_master"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if detail entities should be refreshed on detail change.
    /// </summary>
    public bool RefreshDetail
    {
      get { return (this["refresh_detail"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if all current list of entities should be refreshed on 
    /// record change (insert, update or delete).
    /// </summary>
    public bool RefreshList
    {
      get { return (this["refresh_list"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if the whole list of master entities should be refreshed on 
    /// record change (insert, update or delete).
    /// </summary>
    public bool RefreshMasterList
    {
      get { return (this["refresh_master_list"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if grid shoudl be refreshed after delete.
    /// </summary>
    public bool RefreshAfterDelete
    {
      get { return (this["refresh_after_delete"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if SQL select clause already contains parent-child join condition,
    /// and join_condition attribute is not needed for parent-child grid relation.
    /// </summary>
    public bool IsJoinConditionInSelectClause
    {
      get { return (this["join_condition_in_select"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute with the given name.
    /// </summary>
    /// <param name="name">attribute name</param>
    /// <returns>attribute with the given name</returns>
    override public CxAttributeMetadata GetAttribute(string name)
    {
      // Try to get attribute defined directly in this entity usage.
      CxAttributeMetadata attribute = base.GetAttribute(name);
      // Try to get attribute defined in the inherited entity usage.
      if (attribute == null && InheritedEntityUsage != null)
      {
        attribute = InheritedEntityUsage.GetAttribute(name);
      }
      // Try to get attribute defined in the base entity.
      if (attribute == null)
      {
        attribute = Entity.GetAttribute(name);
      }
      return attribute;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ordered list of attributes.
    /// </summary>
    override public IList<CxAttributeMetadata> Attributes
    {
      get
      {
        lock (m_AttributeLock)
        {
          if (m_Attributes == null)
          {
            m_Attributes = new List<CxAttributeMetadata>();
            if (CxList.IsEmpty2(m_OrderedAttributeNames))
            {
              if (InheritedEntityUsage != null)
              {
                m_Attributes.AddRange(InheritedEntityUsage.Attributes);
              }
              else
              {
                m_Attributes.AddRange(Entity.Attributes);
              }

              foreach (CxAttributeMetadata attributeUsage in base.Attributes)
              {
                CxAttributeMetadata entityAttribute;
                if (InheritedEntityUsage != null)
                {
                  entityAttribute = InheritedEntityUsage.GetAttribute(attributeUsage.Id);
                }
                else
                {
                  entityAttribute = Entity.GetAttribute(attributeUsage.Id);
                }

                if (entityAttribute != null)
                {
                  int index = ExtractIds(Attributes).IndexOf(entityAttribute.Id);
                  if (index < 0)
                    throw new ExException("index is expected to be greater then -1");
                  m_Attributes[index] = attributeUsage;
                }
                else
                {
                  m_Attributes.Add(attributeUsage);
                }
              }
            }
            else
            {
              foreach (string name in m_OrderedAttributeNames)
              {
                CxAttributeMetadata attribute = GetAttribute(name);
                if (attribute != null) m_Attributes.Add(attribute);
              }
            }
          }
          return new List<CxAttributeMetadata>(m_Attributes);
        }
      }
    }
    //----------------------------------------------------------------------------
    private IList<CxAttributeMetadata> m_HyperlinkComposeXmlAttributes;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the list of hyperlink "xml compose" attributes, cached.
    /// </summary>
    public IList<CxAttributeMetadata> GetHyperlinkComposeXmlAttributes()
    {
      if (m_HyperlinkComposeXmlAttributes == null)
      {
        m_HyperlinkComposeXmlAttributes = new List<CxAttributeMetadata>();
        foreach (CxAttributeMetadata attributeMetadata in Attributes)
        {
          if (attributeMetadata.HyperLinkComposeXml)
          {
            m_HyperlinkComposeXmlAttributes.Add(attributeMetadata);
          }
        }
      }
      return m_HyperlinkComposeXmlAttributes;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns command with the given name.
    /// </summary>
    /// <param name="commandId">command name</param>
    /// <returns>command with the given name</returns>
    override public CxCommandMetadata GetCommand(string commandId)
    {
      CxCommandMetadata command = base.GetCommand(commandId);
      if (command == null && InheritedEntityUsage != null)
      {
        command = InheritedEntityUsage.GetCommand(commandId);
      }
      if (command == null)
      {
        command = Entity.GetCommand(commandId);
      }
      return command;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ordered list of commands.
    /// </summary>
    override public IList<CxCommandMetadata> Commands
    {
      get
      {
        if (m_OrderedCommandList == null)
        {
          m_OrderedCommandList = new List<CxCommandMetadata>();
          if (CxList.IsEmpty2(m_OrderedCommandNames))
          {
            if (InheritedEntityUsage != null)
            {
              m_OrderedCommandList.AddRange(InheritedEntityUsage.Commands);
            }
            else
            {
              m_OrderedCommandList.AddRange(Entity.Commands);
            }

            foreach (CxCommandMetadata command in base.Commands)
            {
              CxCommandMetadata entityCommand;
              if (InheritedEntityUsage != null)
              {
                entityCommand = InheritedEntityUsage.GetCommand(command.Id);
              }
              else
              {
                entityCommand = Entity.GetCommand(command.Id);
              }

              if (entityCommand != null)
              {
                int index = m_OrderedCommandList.IndexOf(entityCommand);
                m_OrderedCommandList[index] = command;
              }
              else
              {
                m_OrderedCommandList.Add(command);
              }
            }
          }
          else
          {
            foreach (string name in m_OrderedCommandNames)
            {
              CxCommandMetadata command = GetCommand(name);
              if (command != null)
              {
                m_OrderedCommandList.Add(command);
              }
            }
          }
        }
        return m_OrderedCommandList;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets overriden order of attributes.
    /// </summary>
    /// <param name="orderedAttributes">comma-separated list of attribute names</param>
    /// <param name="overwrite">determines if the ordered attributes 
    /// should be overwritten even if it already exists</param>
    public void SetAttributeOrder(string orderedAttributes, bool overwrite)
    {
      if (overwrite || CxList.IsEmpty2(m_OrderedAttributeNames))
      {
        m_OrderedAttributeNames = CxText.DecomposeWithWhiteSpaceAndComma(orderedAttributes.ToUpper());
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets overriden order of commands.
    /// </summary>
    /// <param name="orderedCommands">comma-separated list of command names</param>
    /// <param name="overwrite">determines if the command order
    /// should be overwritten even if it already exists</param>
    public void SetCommandOrder(string orderedCommands, bool overwrite)
    {
      if (overwrite)
      {
        m_OrderedCommandNames = CxText.DecomposeWithWhiteSpaceAndComma(orderedCommands.ToUpper());
        m_IsOrderedCommandNamesDefined = true;
      }
      else if (CxList.IsEmpty2(m_OrderedCommandNames))
      {
        m_OrderedCommandNames = new string[0];
        m_IsOrderedCommandNamesDefined = false;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the command should be visible from the current entity usage's point of view.
    /// </summary>
    /// <param name="commandMetadata">a command to evaluate</param>
    /// <returns>true if visible</returns>
    public bool GetIsCommandVisible(CxCommandMetadata commandMetadata)
    {
      return GetIsCommandVisible(commandMetadata, null);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the command should be visible from the current entity usage's point of view.
    /// </summary>
    /// <param name="commandMetadata">a command to evaluate</param>
    /// <returns>true if visible</returns>
    public bool GetIsCommandVisible(CxCommandMetadata commandMetadata, IxEntity parentEntity)
    {
      if (!Commands.Contains(commandMetadata))
        return false;
      if (IsOrderedCommandNamesDefined && !m_OrderedCommandNames.Contains(commandMetadata.Id))
        return false;
      if (!commandMetadata.Visible)
        return false;
      if (!EntityInstance.GetIsCommandVisible(commandMetadata, parentEntity))
        return false;
      return true;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns full SQL SELECT statement with WHERE condition.
    /// </summary>
    public string GetSelectStatement()
    {
      return CxDbUtils.AddToWhere(SqlSelect, WhereClause);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Compose SQL statement for ReadData method.
    /// </summary>
    /// <param name="where">additional where clause</param>
    public string ComposeReadDataSql(string where = "")
    {
      if (CxUtils.IsEmpty(SqlSelect))
      {
        throw new ExMetadataException("SQL SELECT statement not defined for " + Id + " entity usage");
      }
      string sql = GetSelectStatement();
      sql = CxDbUtils.AddToWhere(sql, where);
      sql = CxDbUtils.AddToWhere(sql, GetCustomWhereClause());
      sql = CxDbUtils.AddToWhere(sql, GetAccessWhereClause());
      return sql;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Prepares value provider before passing to command.
    /// </summary>
    /// <param name="provider">base value provider</param>
    /// <returns>value provider to pass to SQL command</returns>
    public IxValueProvider PrepareValueProvider(IxValueProvider provider)
    {

            IxValueProvider res;
      if (Holder != null)
      {
        res = Holder.PrepareValueProvider(provider);
      }
      else
      {
        
                res = new CxValueProviderCollection();
                ((CxValueProviderCollection)res).AddIfNotEmpty(provider);

            }


            foreach (var atr in Attributes)
            {
                res.ValueTypes.Add(atr.Id.ToUpper(), atr.Type);
            }
            return res;
    }


     

        //----------------------------------------------------------------------------
        /// <summary>
        /// Reads entity data from the database.
        /// </summary>
        /// <param name="connection">database connection to use</param>
        /// <param name="dt">data table with list of entities</param>
        /// <param name="where">additional where clause</param>
        /// <param name="provider">parameter values provider</param>
        /// <param name="cacheMode">entity cache mode</param>
        /// <param name="customPreProcessHandler">a custom pre-processing handler to be applied to the
        /// data obtained</param>
        public void ReadData(
      CxDbConnection connection,
      DataTable dt,
      string where = "",
      IxValueProvider provider = null,
      NxEntityDataCache cacheMode = NxEntityDataCache.Default,
      DxCustomPreProcessDataSource customPreProcessHandler = null)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }

      string sql = ComposeReadDataSql(where);
      sql = ComposeRecordLimitSqlText(connection, sql);
      GetQueryResult(connection, dt, sql, provider, cacheMode, customPreProcessHandler);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="orderClause">order by clause in format {field1} asc | desc, {field2} asc | desc</param>
    /// <param name="recordCountLimit">record count limit</param>
    public void ReadData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      string orderClause,
      int recordCountLimit)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }

      string sql = ComposeReadDataSql(where);
      sql = ComposeRecordLimitSqlText(connection, sql, recordCountLimit);
      if (CxUtils.NotEmpty(orderClause))
      {
        sql += "\r\nORDER BY " + orderClause;
      }
      GetQueryResult(connection, dt, sql, provider, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="orderClause">order by clause in format {field1} asc | desc, {field2} asc | desc</param>
    public void ReadData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      string orderClause)
    {
      ReadData(connection, dt, where, provider, orderClause, -1, -1);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Performs the query to be performed before any SELECT statement.
    /// </summary>
    /// <param name="connection">database connection</param>
    protected void DoSqlSelectRunBefore(
      CxDbConnection connection)
    {
      string statement = SqlSelectRunBefore;
      if (!string.IsNullOrEmpty(statement))
      {
        IxValueProvider provider = Holder.ApplicationValueProvider;
        connection.ExecuteCommand(statement, provider);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="orderClause">order by clause in format {field1} asc | desc, {field2} asc | desc</param>
    /// <param name="startRowIndex">start record index to get data</param>
    /// <param name="rowCount">amount of records to get</param>
    public void ReadData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      string orderClause,
      int startRowIndex,
      int rowCount)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }

      string sql = ComposeReadDataSql(where);
      if (startRowIndex == -1)
      {
        if (rowCount == -1)
          sql = ComposeRecordLimitSqlText(connection, sql);
        else
          sql = ComposeRecordLimitSqlText(connection, sql, rowCount);
      }
      else
      {
        if (rowCount == -1)
          throw new ExArgumentException("recordsAmount", rowCount.ToString());
        sql = ComposePagedSqlText(
          connection, sql, startRowIndex, rowCount, orderClause);

      }
      if (!String.IsNullOrEmpty(orderClause))
      {
        sql += "\r\nORDER BY " + orderClause;
      }
      GetQueryResult(connection, dt, sql, provider, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the overall amount of data rows (not taking into account the record
    /// frame probably used).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="where">where clause</param>
    /// <param name="valueProvider">value provider for parameters</param>
    /// <returns>amount of datarows available</returns>
    public int ReadDataRowAmount(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider)
    {
      if (!GetIsAccessGranted())
      {
        return -1;
      }
      string sql = ComposeReadDataSql(where);
      sql = connection.ScriptGenerator.GetCountSqlScriptForSelect(sql);
      object resultObj = connection.ExecuteScalar(sql, valueProvider);
      if (!CxUtils.IsNull(resultObj))
        return Convert.ToInt32(resultObj);
      else
        throw new ExException("Unexpected query result: NULL result for the <select count> query");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the overall amount of data rows taking into account
    /// the existing parent-child relationship
    /// (not taking into account the record frame probably used).
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="where">where clause</param>
    /// <param name="valueProvider">value provider for parameters</param>
    /// <returns>amount of datarows available</returns>
    public int ReadChildDataRowAmount(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider)
    {
      if (!GetIsAccessGranted())
      {
        return -1;
      }

      string sql = GetChildDataQuery(connection, where);
      sql = connection.ScriptGenerator.GetCountSqlScriptForSelect(sql);
      object resultObj = connection.ExecuteScalar(sql, valueProvider);
      if (!CxUtils.IsNull(resultObj))
        return Convert.ToInt32(resultObj);
      else
        throw new ExException("Unexpected query result: NULL result for the <select count> query");
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an array of entities of the given entity usage.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values</param>
    /// <param name="startRowIndex">start index offset</param>
    /// <param name="rowCount">amount of records to get</param>
    /// <param name="sortings">sort descriptors collection</param>
    /// <returns>an array of entities</returns>
    public IxEntity[] ReadEntities(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider,
      int startRowIndex = -1,
      int rowCount = -1,
      CxSortDescriptorList sortings = null)
    {
      return EntityInstance.ReadEntities(
        connection, where, valueProvider, startRowIndex, rowCount, sortings);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads an entity of the given entity usage and with the given entity PK.
    /// </summary>
    /// <param name="connection">a database connection</param>
    /// <param name="where">where clause (filter condition)</param>
    /// <param name="valueProvider">a provider for parameter values, including PK</param>
    /// <returns>the entity read</returns>
    public IxEntity ReadEntity(
      CxDbConnection connection,
      string where,
      IxValueProvider valueProvider)
    {
      return EntityInstance.ReadEntity(
        connection,
        where,
        valueProvider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="paramValues">values of parameters in where clause (if any)</param>
    public void ReadData(CxDbConnection connection,
                         DataTable dt,
                         string where,
                         object[] paramValues)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }

      string sql = ComposeReadDataSql(where);
      IxValueProvider provider = CxDbUtils.GetValueProvider(sql, paramValues);
      ReadData(connection, dt, where, provider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="customPreProcessHandler">a custom pre-processing handler to be applied to the
    /// data obtained</param>
    public void ReadData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      DxCustomPreProcessDataSource customPreProcessHandler)
    {
      ReadData(connection, dt, where, provider, NxEntityDataCache.Default, customPreProcessHandler);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Compose SQL statement for ReadDataRow method.
    /// </summary>
    /// <param name="where">additional where clause</param>
    protected string ComposeReadDataRowSql(string where)
    {
      string sql = SqlSelectSingleRow;
      if (CxUtils.IsEmpty(sql))
      {
        throw new ExMetadataException("SQL SELECT statement not defined for " + Id + " entity usage");
      }
      sql = CxDbUtils.AddToWhere(sql, where);
      sql = CxDbUtils.AddToWhere(sql, GetAccessWhereClause());
      return sql;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="where">primary key condition</param>
    /// <param name="primaryKeyValues">values of primary key fields</param>
    public DataRow ReadDataRow(
      CxDbConnection connection,
      string where,
      object[] primaryKeyValues)
    {
      if (!GetIsAccessGranted())
      {
        return null;
      }
      IxValueProvider valueProvider = PrepareValueProvider(
        CreatePrimaryKeyValueProvider(primaryKeyValues));

      return ReadDataRow(connection, where, valueProvider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates a value provider containing primary key values
    /// taken from the given collection.
    /// </summary>
    /// <param name="primaryKeyValues">the ordered collection of primary key values</param>
    /// <returns>value provider created</returns>
    public IxValueProvider CreatePrimaryKeyValueProvider(
      object[] primaryKeyValues)
    {
      CxHashtable result = new CxHashtable();
      CxAttributeMetadata[] primaryKeyAttributes = PrimaryKeyAttributes;
      for (int i = 0; i < primaryKeyAttributes.Length; i++)
      {
        CxAttributeMetadata attributeMetadata = PrimaryKeyAttributes[i];
        result[attributeMetadata.Id] = primaryKeyValues[i];
      }
      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="where">primary key condition</param>
    /// <param name="provider">parameter values provider to get primary key values</param>
    /// <param name="cacheMode">entity cache mode</param>
    public DataRow ReadDataRow(
      CxDbConnection connection,
      string where,
      IxValueProvider provider,
      NxEntityDataCache cacheMode)
    {
      if (!GetIsAccessGranted())
      {
        return null;
      }

      string sql = ComposeReadDataRowSql(where);
      CxGenericDataTable dt = new CxGenericDataTable();
      GetQueryResult(connection, dt, sql, provider, cacheMode);
      if (dt.Rows.Count == 1)
      {
        return dt.Rows[0];
      }
      else if (dt.Rows.Count == 0)
      {
        return null;
      }
      else
      {
        string[] pkNames = CxDbParamParser.GetList(where, false);
        object[] pkValues = new object[pkNames.Length];
        for (int i = 0; i < pkNames.Length; i++)
        {
          pkValues[i] = provider[pkNames[i]];
        }
        throw new ExTooManyRowsException(PluralCaption, pkNames, pkValues, dt.Rows.Count);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="where">primary key condition</param>
    /// <param name="provider">parameter values provider to get primary key values</param>
    public DataRow ReadDataRow(
      CxDbConnection connection,
      string where,
      IxValueProvider provider)
    {
      return ReadDataRow(connection, where, provider, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads root-level self-referencing hierarchical entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="orderByClause">order by clause</param>
    /// <param name="cacheMode">entity cache mode</param>
    public void ReadRootLevelData(CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      string orderByClause,
      NxEntityDataCache cacheMode)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }

      if (CxUtils.IsEmpty(SqlSelect))
      {
        throw new ExMetadataException("SQL SELECT statement not defined for " + Id + " entity usage");
      }
      string sql = GetSelectStatement();
      sql = CxDbUtils.AddToWhere(sql, RootCondition);
      sql = CxDbUtils.AddToWhere(sql, where);
      sql = CxDbUtils.AddToWhere(sql, GetCustomWhereClause());
      sql = CxDbUtils.AddToWhere(sql, GetAccessWhereClause());
      sql = ComposeRecordLimitSqlText(connection, sql);
      if (!String.IsNullOrEmpty(orderByClause))
      {
        sql += "\r\nORDER BY " + orderByClause;
      }
      GetQueryResult(connection, dt, sql, provider, cacheMode);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads root-level self-referencing hierarchical entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="cacheMode">entity cache mode</param>
    public void ReadRootLevelData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      NxEntityDataCache cacheMode)
    {
      ReadRootLevelData(connection, dt, where, provider, null, cacheMode);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads root-level self-referencing hierarchical entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    public void ReadRootLevelData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider)
    {
      ReadRootLevelData(connection, dt, where, provider, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads root-level self-referencing hierarchical entity data from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="orderByClause">order by clause</param>
    public void ReadRootLevelData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider provider,
      string orderByClause)
    {
      ReadRootLevelData(connection, dt, where, provider, orderByClause, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads next level of self-reference hierarchilcal entity data.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="provider">parameter values provider</param>
    /// <param name="cacheMode">entity cache mode</param>
    public void ReadNextLevelData(
      CxDbConnection connection,
      DataTable dt,
      IxValueProvider provider,
      NxEntityDataCache cacheMode)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }
      if (CxUtils.IsEmpty(SqlSelect))
      {
        throw new ExMetadataException("SQL SELECT statement not defined for " + Id + " entity usage");
      }
      if (CxUtils.IsEmpty(LevelCondition))
      {
        throw new ExMetadataException("'level_condition' statement not defined for " + Id + " entity usage");
      }

      string sql = GetSelectStatement();
      sql = CxDbUtils.AddToWhere(sql, LevelCondition);
      sql = CxDbUtils.AddToWhere(sql, GetCustomWhereClause());
      sql = CxDbUtils.AddToWhere(sql, GetAccessWhereClause());
      sql = ComposeRecordLimitSqlText(connection, sql);
      GetQueryResult(connection, dt, sql, provider, cacheMode);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads next level of self-reference hierarchilcal entity data.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of entities</param>
    /// <param name="provider">parameter values provider</param>
    public void ReadNextLevelData(
      CxDbConnection connection,
      DataTable dt,
      IxValueProvider provider)
    {
      ReadNextLevelData(connection, dt, provider, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a query to obtain childish data.
    /// </summary>
    /// <param name="connection">a connection to be used</param>
    /// <param name="where">where clause to be used</param>
    /// <returns>a query</returns>
    public string GetChildDataQuery(
      CxDbConnection connection,
      string where)
    {
      string sql = GetSelectStatement();
      sql = CxDbUtils.AddToWhere(sql, where);
      sql = CxDbUtils.AddToWhere(sql, JoinCondition);
      sql = CxDbUtils.AddToWhere(sql, GetCustomWhereClause());
      sql = CxDbUtils.AddToWhere(sql, GetAccessWhereClause());
      sql = ComposeRecordLimitSqlText(connection, sql);
      return sql;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets data provider for the query to obtain childish data.
    /// </summary>
    /// <param name="paramProvider">parameter provider</param>
    /// <param name="where">where clause</param>
    /// <param name="paramValues"></param>
    /// <returns>a value provider</returns>
    public CxValueProviderCollection GetChildDataProvider(
      IxValueProvider paramProvider,
      string where,
      params object[] paramValues)
    {
      CxValueProviderCollection provider = new CxValueProviderCollection();
      provider.AddIfNotEmpty(paramProvider);
      provider.AddIfNotEmpty(CxDbUtils.GetValueProvider(where, paramValues));
      return provider;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data as child one from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of child entities</param>
    /// <param name="paramProvider">provider of the parent/child parameter values</param>
    /// <param name="where">additional where clause</param>
    /// <param name="paramValues">values of parameters in where clause (if any)</param>
    /// <param name="cacheMode">entity cache mode</param>
    public void ReadChildData(
      CxDbConnection connection,
      DataTable dt,
      IxValueProvider paramProvider,
      string where,
      NxEntityDataCache cacheMode,
      params object[] paramValues)
    {
      ReadChildData(connection, dt, paramProvider, where, cacheMode, -1, -1, paramValues);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data as child one from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of child entities</param>
    /// <param name="paramProvider">provider of the parent/child parameter values</param>
    /// <param name="where">additional where clause</param>
    /// <param name="rowCount">amount of rows to get</param>
    /// <param name="paramValues">values of parameters in where clause (if any)</param>
    /// <param name="cacheMode">entity cache mode</param>
    /// <param name="startRowIndex">start index offset to get rows from</param>
    public void ReadChildData(
      CxDbConnection connection,
      DataTable dt,
      IxValueProvider paramProvider,
      string where,
      NxEntityDataCache cacheMode,
      int startRowIndex,
      int rowCount,
      params object[] paramValues)
    {
      if (!GetIsAccessGranted())
      {
        GetEmptyDataTable(dt);
        return;
      }
      if (CxUtils.IsEmpty(SqlSelect))
      {
        throw new ExMetadataException("SQL SELECT statement not defined for " + Id + " entity usage");
      }
      if (CxUtils.IsEmpty(JoinCondition) && !IsJoinConditionInSelectClause)
      {
        throw new ExMetadataException("Parent/child join condition not defined for " + Id + " entity usage");
      }

      CxValueProviderCollection provider =
        GetChildDataProvider(paramProvider, where, paramValues);
      string sql = GetChildDataQuery(connection, where);
      if (startRowIndex >= 0 && rowCount >= 0)
        sql = ComposePagedSqlText(connection, sql, startRowIndex, rowCount, OrderByClause);

      GetQueryResult(connection, dt, sql, provider, cacheMode);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data as child one from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of child entities</param>
    /// <param name="where">additional where clause</param>
    /// <param name="paramProvider">provider of all values</param>
    public void ReadChildData(
      CxDbConnection connection,
      DataTable dt,
      string where,
      IxValueProvider paramProvider)
    {
      ReadChildData(connection, dt, paramProvider, where, NxEntityDataCache.Default);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Reads entity data as child one from the database.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="dt">data table with list of child entities</param>
    /// <param name="paramProvider">provider of the parent/child parameter values</param>
    public void ReadChildData(CxDbConnection connection, DataTable dt, IxValueProvider paramProvider)
    {
      ReadChildData(connection, dt, "", paramProvider);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata for the child with the specified ID.
    /// </summary>
    /// <param name="childId">ID of the child entity usage</param>
    /// <returns>entity usage metadata for the child with the specified ID</returns>
    public CxEntityUsageMetadata GetChildMetadata(string childId)
    {
      CxEntityUsageMetadata childEntityUsage = null;
      if (CxUtils.NotEmpty(childId))
      {
        childEntityUsage = ChildEntityUsages[childId.ToUpper()].EntityUsage;
      }
      if (childEntityUsage != null)
        return childEntityUsage;
      else
        throw new ExMetadataException(string.Format("Child entity usage with ID=<{0}> not defined for entity usage with ID=<{1}>", childId, Id));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns child entity usage by the id of the target entity usage.
    /// </summary>
    /// <param name="entityUsageId">string id</param>
    /// <returns>child entity usage metadata object</returns>
    public CxChildEntityUsageMetadata GetChildEntityUsageOrInheritedByEntityUsageId(string entityUsageId)
    {
      CxEntityUsageMetadata entityUsage = Holder.EntityUsages[entityUsageId];
      foreach (CxChildEntityUsageMetadata childEntityUsageMetadata in ChildEntityUsages.Values)
      {
        if (childEntityUsageMetadata.EntityUsage == entityUsage ||
            childEntityUsageMetadata.EntityUsage.IsInheritedFrom(entityUsage))
          return childEntityUsageMetadata;
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Generates attributes for this entity usage using information from database schema.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <param name="attributeUsage">true to generate attribute usage instead of attribute</param>
    /// <returns>generated attributes text</returns>
    public string GenerateAttributes(CxDbConnection connection, bool attributeUsage)
    {
      string sql = "select distinct c.colid, o.name as object_name, c.name as column_name,\r\n" +
        "       t.name as type_name, c.prec as length, c.scale, c.isnullable as nullable,\r\n" +
        "       (select 1\r\n" +
        "          from sysobjects pk,\r\n" +
        "               sysindexes i,\r\n" +
        "               sysindexkeys k\r\n" +
        "         where pk.parent_obj = c.id\r\n" +
        "           and pk.xtype = 'PK'\r\n" +
        "           and i.name = pk.name\r\n" +
        "           and k.id = i.id\r\n" +
        "           and k.indid = i.indid\r\n" +
        "           and k.colid = c.colid\r\n" +
        "       ) as primary_key\r\n" +
        "  from syscolumns c,\r\n" +
        "       sysobjects o,\r\n" +
        "       systypes   t\r\n" +
        " where c.id = o.id\r\n" +
        "   and c.xtype = t.xusertype\r\n" +
        "   and t.name not in ('sysname', 'Category')\r\n" +
        "   and o.name = '" + DbObject + "'\r\n" +
        " order by c.colid";
      DataTable dt = connection.GetQueryResult(sql);

      StringBuilder sb = new StringBuilder();
      sb.Append(attributeUsage ?
                "  <entity_usage id=\"" + Id + "\">\r\n" :
                "  <entity id=\"" + Entity.Id + "\" id=\"" + Id + "\">\r\n");
      sb.Append(attributeUsage ? "    <attribute_usages>\r\n" : "    <attributes>\r\n");
      string padding = attributeUsage ? "                       " : "                 ";
      foreach (DataRow dr in dt.Rows)
      {
        string name = (string) dr["column_name"];
        sb.Append("      <" + (attributeUsage ? "attribute_usage" : "attribute") + " id=\"" + name + "\"\r\n");
        string type = ((string) dr["type_name"]).ToLower();
        type =
          (type == "varchar" || type == "nvarchar" ||
          type == "char" || type == "nchar" ||
          type == "udt_longstring" ||
          type == "udt_text" || type == "udt_string" ? CxAttributeMetadata.TYPE_STRING :
          type == "text" || type == "ntext" ? CxAttributeMetadata.TYPE_LONGSTRING :
          type == "datetime" || type == "smalldatetime" ? CxAttributeMetadata.TYPE_DATE :
          type == "bigint" || type == "int" || type == "udt_objid" ||
          type == "smallint" || type == "tinyint" ? CxAttributeMetadata.TYPE_INT :
          type == "decimal" || type == "numeric" || type == "udt_currency" || type == "udt_curr_rate" ||
          type == "money" || type == "smallmoney" ||
          type == "float" || type == "real" ? CxAttributeMetadata.TYPE_FLOAT :
          type == "bit" ? CxAttributeMetadata.TYPE_BOOLEAN :
          "??? " + type + " ???");
        bool visible = true;
        if (!CxUtils.IsNull(dr["primary_key"]) && Convert.ToInt32(dr["primary_key"]) == 1)
        {
          sb.Append(padding + "primary_key=\"true\"\r\n");
          if (type == CxAttributeMetadata.TYPE_INT)
          {
            sb.Append(padding + "visible=\"false\"\r\n");
            sb.Append(padding + "editable=\"false\"\r\n");
            sb.Append(padding + "default=\"=@sequence\"\r\n");
            visible = false;
          }
        }
        if (visible)
        {
          sb.Append(padding + "caption=\"" + name + "\"\r\n");
        }
        sb.Append(padding + "type=\"" + type + "\"\r\n");
        if (dr["length"] != DBNull.Value)
        {
          int length = Convert.ToInt32(dr["length"]);
          sb.Append(padding + "max_length=\"" + length + "\"\r\n");
        }
        if (!CxUtils.IsNull(dr["scale"]) && Convert.ToInt32(dr["scale"]) != 0)
        {
          sb.Append(padding + "scale=\"" + dr["scale"] + "\"\r\n");
        }
        if ((int) dr["nullable"] == 0)
        {
          sb.Append(padding + "nullable=\"false\"\r\n");
        }
        /*string control = 
         (type == CxAttributeMetadata.TYPE_BOOLEAN ? CxAttributeMetadata.WIN_CONTROL_CHECKBOX :
          type == CxAttributeMetadata.TYPE_DATE    ? CxAttributeMetadata.WIN_CONTROL_DATE :
          type == CxAttributeMetadata.TYPE_INT  ||   
          type == CxAttributeMetadata.TYPE_FLOAT   ? CxAttributeMetadata.WIN_CONTROL_SPIN :
                                                     CxAttributeMetadata.WIN_CONTROL_TEXT);
        sb.Append("                 control=\"" + control + "\"\r\n");*/
        /*int width = Math.Min(Math.Max(length * 10, 30), 200);
        sb.Append("                 grid_width=\"" + width + "\"\r\n");*/
        sb.Append("      />\r\n");
      }
      sb.Append(attributeUsage ? "    </attribute_usages>\r\n" : "    </attributes>\r\n");
      sb.Append(attributeUsage ? "  </entity_usage>\r\n" : "  </entity>\r\n");
      sb.Append("  <!-- -*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*- -->\r\n");
      return sb.ToString();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Generates attributes for this entity usage using information from database schema.
    /// </summary>
    /// <param name="connection">database connection to use</param>
    /// <returns>generated attributes text</returns>
    public string GenerateAttributes(CxDbConnection connection)
    {
      return GenerateAttributes(connection, false);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of custom attributes.
    /// </summary>
    public IList<CxAttributeMetadata> CustomAttributes
    {
      get
      {
        List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
        foreach (CxAttributeMetadata attribute in Attributes)
        {
          if (attribute.Custom)
          {
            list.Add(attribute);
          }
        }
        return list;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of attributes with controls defined in design-time.
    /// </summary>
    public IList<CxAttributeMetadata> ManualAttributes
    {
      get
      {
        List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
        foreach (CxAttributeMetadata attribute in Attributes)
        {
          if (attribute.Manual)
          {
            list.Add(attribute);
          }
        }
        return list;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object that desctibes what to edit depending on the column name.
    /// </summary>
    /// <param name="columnName">name of the column to find description for</param>
    /// <param name="exact">if true return only explicitly specified definition</param>
    /// <returns>object that desctibes what to edit depending on the column name</returns>
    public CxEntityUsageToEditMetadata GetEntityUsageToEdit(string columnName, bool exact)
    {
      foreach (CxEntityUsageToEditMetadata edit in m_EntityUsagesToEdit)
      {
        if (edit.Complies(columnName, exact))
        {
          return edit;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the attribute to store the states of each sub-tab into.
    /// For WinForms only.
    /// </summary>
    public string WinSubTabStateAttributeId
    {
      get { return this["win_sub_tab_state_attribute_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The attribute to store the state of each sub-tab into.
    /// For WinForms only.
    /// </summary>
    public CxAttributeMetadata WinSubTabStateAttribute
    {
      get { return GetAttribute(WinSubTabStateAttributeId); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this entity usage is default for the entity.
    /// </summary>
    public bool IsDefault
    { get { return CxBool.Parse(this["is_default"], false); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the entity usage can be involved into querying thru Query Builder UI.
    /// </summary>
    public bool IsQueryable
    { get { return CxBool.Parse(this["is_queryable"], false); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default entity usage metadata object for the given entity.
    /// </summary>
    override public CxEntityUsageMetadata DefaultEntityUsage
    {
      get
      {
        // Returns null since the property is applicable for entity metadata only.
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Replaces text placeholders with actual property values.
    /// Placeholders are in format: %property_name%.
    /// </summary>
    /// <param name="text">text to replace</param>
    /// <returns>text with placeholders replaced to values</returns>
    public string ReplacePlaceholders(string text)
    {
      if (CxUtils.NotEmpty(text))
      {
        CxEntityUsagePlaceholderManager placeholderManager = new CxEntityUsagePlaceholderManager(this);
        text = placeholderManager.ReplacePlaceholders(text);
        text = CxText.RemovePlaceholders(text, '%');
      }
      return text;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage ID to use for entity commands.
    /// </summary>
    public string CommandEntityUsageId
    { get { return this["command_entity_usage_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage object to use for entity commands.
    /// </summary>
    public CxEntityUsageMetadata CommandEntityUsage
    {
      get
      {
        if (CxUtils.NotEmpty(CommandEntityUsageId))
        {
          return Holder.EntityUsages[CommandEntityUsageId];
        }
        return this;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an id of the entity usage the visibility of the current
    /// entity usage depends on.
    /// </summary>
    public string VisibilityReferenceEntityUsageId
    {
      get { return this["visibility_ref_entity_usage_id"]; }
      set { this["visibility_ref_entity_usage_id"] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an entity usage the visibility of the current
    /// entity usage depends on.
    /// </summary>
    public CxEntityUsageMetadata VisibilityReferenceEntityUsage
    {
      get { return Holder.EntityUsages.Find(VisibilityReferenceEntityUsageId); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the entity usage is visible.
    /// </summary>
    protected override bool GetIsVisible()
    {
      CxEntityUsageMetadata entityUsage = VisibilityReferenceEntityUsage;
      if (entityUsage != null && entityUsage != this)
      {
        return entityUsage.Visible;
      }
      return base.GetIsVisible();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns empty data table with columns filled by attribute metadata.
    /// </summary>
    public CxGenericDataTable GetEmptyDataTable()
    {
      if (m_EmptyDataTable == null)
      {
        CxGenericDataTable dt = new CxGenericDataTable();
        EmptyDataSource(dt);
        m_EmptyDataTable = dt;
      }
      return m_EmptyDataTable;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Fills data table with columns by attribute metadata.
    /// </summary>
    public void GetEmptyDataTable(DataTable dt)
    {
      dt.Clear();
      dt.Columns.Clear();
      dt.Rows.Clear();
      dt.Columns.AddRange(GetDataColumnsFromAttributes());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Fills the given data source with columns by attribute metadata.
    /// </summary>
    public void EmptyDataSource(IxGenericDataSource dataSource)
    {
      dataSource.ClearDataAndSchema();
      dataSource.PopulateColumns(GetDataColumnsFromAttributes());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an array of data columns created by the entity's attributes.
    /// </summary>
    /// <returns>an array of data columns</returns>
    public DataColumn[] GetDataColumnsFromAttributes()
    {
      DataColumn[] columns = new DataColumn[Attributes.Count];
      for (int i = 0; i < Attributes.Count; i++)
      {
        CxAttributeMetadata attr = Attributes[i];

        DataColumn dc = new DataColumn(attr.Id, attr.GetPropertyType());

        if (dc.DataType == typeof(string) && attr.Type == CxAttributeMetadata.TYPE_LONGSTRING)
          dc.MaxLength = int.MaxValue;

        columns[i] = dc;
      }
      return columns;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if access to the entity usage is granted.
    /// </summary>
    public bool GetIsAccessGranted()
    {
      if (Holder != null && Holder.Security != null)
      {
        return Holder.Security.GetRight(this);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns access rule WHERE clause (horizontal security).
    /// </summary>
    protected string GetAccessWhereClause()
    {
      string whereClause = "";
      if (Holder != null)
      {
        // Apply horizontal security rule defined in the object permissions.
        if (Holder.Security != null)
        {
          whereClause =
            CxDbUtils.ComposeWhereClause(whereClause, Holder.Security.GetWhereClause(this));
        }
        // Apply workspace condition.
        string workspaceClause = GetWorkspaceWhereClause();
        if (CxUtils.NotEmpty(workspaceClause))
        {
          whereClause = CxDbUtils.ComposeWhereClause(whereClause, workspaceClause);
        }
      }
      return whereClause;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns workspace filter WHERE clause or null if clause is not needed.
    /// </summary>
    /// <returns>workspace filter WHERE clause or null if clause is not needed</returns>
    protected string GetWorkspaceWhereClause()
    {
      string clause = null;
      if (Holder.UserPermissionProvider != null && IsWorkspaceDependent)
      {
        if (IsFilteredByCurrentWorkspace)
        {
          // Filter by current selected workspace.
          clause = "WorkspaceId in (0, :Application$CurrentWorkspaceId)";
        }
        else if (IsFilteredByAvailableWorkspaces)
        {
          // Filter by workspaces available for the current user.
          if (Holder.ApplicationValueProvider != null)
          {
            string valueListStr = "0";
            DataTable dt =
              (DataTable) Holder.ApplicationValueProvider["Application$WorkspaceAvailableForUserTable"];
            if (dt != null)
            {
              foreach (DataRow dr in dt.Rows)
              {
                if (CxUtils.NotEmpty(dr["WorkspaceId"]))
                {
                  valueListStr += "," + dr["WorkspaceId"];
                }
              }
            }
            clause = "WorkspaceId in (" + valueListStr + ")";
          }
        }
        string customClause = GetCustomWorkspaceWhereClause(clause);
        if (CxUtils.NotEmpty(customClause))
        {
          clause = customClause;
        }
      }
      return clause;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom WHERE clause (can be provided by the entity).
    /// </summary>
    protected string GetCustomWhereClause()
    {
      if (EntityInstance != null)
      {
        return EntityInstance.GetCustomWhereClause();
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns custom workspace filter WHERE clause 
    /// or null if custom workspace clause is not needed.
    /// </summary>
    /// <param name="defaultWorkspaceClause">default workspace WHERE clause</param>
    /// <returns>custom clause or null if no custom clause should be applied</returns>
    protected string GetCustomWorkspaceWhereClause(string defaultWorkspaceClause)
    {
      if (EntityInstance != null)
      {
        return EntityInstance.GetCustomWorkspaceWhereClause(defaultWorkspaceClause);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of DB file attributes.
    /// </summary>
    public IList<CxAttributeMetadata> DbFileAttributes
    {
      get
      {
        List<CxAttributeMetadata> list = new List<CxAttributeMetadata>();
        foreach (CxAttributeMetadata attr in Attributes)
        {
          if (attr.IsDbFile)
          {
            list.Add(attr);
          }
        }
        return list;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns first found DB file attribute.
    /// </summary>
    public CxAttributeMetadata GetFirstDbFileAttribute()
    {
      foreach (CxAttributeMetadata attr in Attributes)
      {
        if (attr.IsDbFile)
        {
          return attr;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds child entity usages to the given list.
    /// </summary>
    /// <param name="list">list to add child entity usages to</param>
    protected void AddChildEntityUsagesToList(IList<CxEntityUsageMetadata> list)
    {
      foreach (CxChildEntityUsageMetadata childMetadata in ChildEntityUsagesList)
      {
        if (childMetadata.EntityUsage != null &&
            list.IndexOf(childMetadata.EntityUsage) < 0)
        {
          list.Add(childMetadata.EntityUsage);
          childMetadata.EntityUsage.AddChildEntityUsagesToList(list);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns complete list of all child entity usages (iterates throw all levels).
    /// </summary>
    /// <returns>list of CxEntityUsage objects</returns>
    public IList<CxEntityUsageMetadata> GetChildEntityUsagesRecurrent()
    {
      List<CxEntityUsageMetadata> list = new List<CxEntityUsageMetadata>();
      AddChildEntityUsagesToList(list);
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default command for the entity usage.
    /// </summary>
    public CxCommandMetadata GetDefaultCommand()
    {
      foreach (CxCommandMetadata command in Commands)
      {
        if (command.IsDefault)
        {
          return command;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds record limitation clause to the original SQL text.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="sqlText"></param>
    /// <param name="recordCountLimit"></param>
    /// <returns></returns>
    protected string ComposeRecordLimitSqlText(
      CxDbConnection connection,
      string sqlText,
      int recordCountLimit)
    {
      return recordCountLimit > 0 ? connection.ScriptGenerator.GetTopRecordsSqlText(sqlText, recordCountLimit) : sqlText;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds record limitation clause to the original SQL text.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="sqlText"></param>
    /// <returns></returns>
    protected string ComposeRecordLimitSqlText(
      CxDbConnection connection,
      string sqlText)
    {
      return IsRecordCountLimited ?
        ComposeRecordLimitSqlText(connection, sqlText, RecordCountLimit) : sqlText;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes an SQL statement basing on the given statement and the 
    /// given record frame to get.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="sqlText">SQL text to use</param>
    /// <param name="startRowIndex">starting index of records to get by the statement</param>
    /// <param name="rowsAmount">amount of records to get by the statement</param>
    /// <param name="orderByClause">sorting conditions</param>
    /// <returns>composed statement</returns>
    protected string ComposePagedSqlText(
      CxDbConnection connection,
      string sqlText,
      int startRowIndex,
      int rowsAmount,
      string orderByClause)
    {
      return connection.ScriptGenerator.GetPagedScriptForSelect(
        sqlText, startRowIndex, rowsAmount, ComposeOrderClauseForPagedSql(connection, orderByClause));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes the complete order by clause for paged queries.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="orderByClause">original order by clause</param>
    /// <returns>composed clause</returns>
    protected string ComposeOrderClauseForPagedSql(
      CxDbConnection connection,
      string orderByClause)
    {
      // Validation
      if (connection == null)
        throw new ExNullArgumentException("connection");

      List<string> statements = new List<string>();
      CxSortDescriptorList sortDescriptors = new CxSortDescriptorList();
      if (!string.IsNullOrEmpty(orderByClause))
        statements.Add(orderByClause);
      foreach (CxAttributeMetadata attributeMetadata in PrimaryKeyAttributes)
      {
        sortDescriptors.Add(new CxSortDescriptor(attributeMetadata.Id, ListSortDirection.Ascending));
      }
      statements.Add(connection.ScriptGenerator.GetOrderByClause(sortDescriptors));
      return string.Join(",", statements.ToArray());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cache key used to cache data returned by SQL statements.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    protected string GetSqlSelectCacheKey(
      string sql,
      IxValueProvider provider)
    {
      string fullKey = CxUtils.Nvl(sql, "NO_SQL") + "\r\nPARAMS:\r\n";
      IList<string> paramNames = CxDbParamParser.GetList(sql, true);
      foreach (string paramName in paramNames)
      {
        string paramValue = provider != null ? CxUtils.ToString(provider[paramName]) : "";
        fullKey += paramName + "=" + paramValue + "\r\n";
      }
      string hashKey = Convert.ToBase64String(
        new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(fullKey)));
      hashKey = EntityId + "." + hashKey;
      return hashKey;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes select statement and returns query result.
    /// </summary>
    /// <param name="connection">a connection context to get query result through</param>
    /// <param name="dt">a data table to load data to</param>
    /// <param name="sql">a SQL statement to be used to get data</param>
    /// <param name="valueProvider">a value provider to be used to provide the SQL query with values</param>
    /// <param name="cacheMode">entity cache mode</param>
    public void GetQueryResult(
      CxDbConnection connection,
      DataTable dt,
      string sql,
      IxValueProvider valueProvider,
      NxEntityDataCache cacheMode)
    {
      GetQueryResult(connection, dt, sql, valueProvider, cacheMode, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Executes select statement and returns query result.
    /// </summary>
    /// <param name="connection">a connection context to get query result through</param>
    /// <param name="dt">a data table to load data to</param>
    /// <param name="sql">a SQL statement to be used to get data</param>
    /// <param name="valueProvider">a value provider to be used to provide the SQL query with values</param>
    /// <param name="cacheMode">entity cache mode</param>
    /// <param name="customPreProcessingHandler">a custom pre-processing handler to be applied to
    /// the data obtained</param>
    public void GetQueryResult(
      CxDbConnection connection,
      DataTable dt,
      string sql,
      IxValueProvider valueProvider,
      NxEntityDataCache cacheMode,
      DxCustomPreProcessDataSource customPreProcessingHandler)
    {

      IxValueProvider preparedProvider = PrepareValueProvider(valueProvider);

      bool isCacheEnabled = cacheMode != NxEntityDataCache.NoCache &&
                            IsCached &&
                            Holder.IsEntityDataCacheEnabled;

      if (isCacheEnabled)
      {
        string cacheKey = GetSqlSelectCacheKey(sql, preparedProvider);
        object lockObject;
        lock (this)
        {
          lockObject = m_EntityDataCacheLockObjectMap[cacheKey];
          if (lockObject == null)
          {
            lockObject = new object();
            m_EntityDataCacheLockObjectMap[cacheKey] = lockObject;
          }
        }
        DataTable cachedTable;
        lock (lockObject)
        {
          cachedTable = (DataTable) Holder.GetEntityDataCacheObject(cacheKey);
          if (cachedTable == null)
          {
            cachedTable = new DataTable();
            DoSqlSelectRunBefore(connection);
            connection.GetQueryResult(cachedTable, sql, preparedProvider);
            Holder.SetEntityDataCacheObject(cacheKey, cachedTable);
          }
        }
        CxData.CopyDataTable(cachedTable, dt);
      }
      else
      {
        DoSqlSelectRunBefore(connection);
        connection.GetQueryResult(dt, sql, preparedProvider);
      }

      PreProcessDataSourceEntirely(dt as IxGenericDataSource, customPreProcessingHandler);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Invoked immediately after data table is read by the entity usage metadata.
    /// </summary>
    /// <param name="dataSource">data source containing entity or entity list</param>
    /// <param name="customPreProcessingHandler">a custom pre-processing handler to be applied to
    /// the data obtained</param>
    public void PreProcessDataSourceEntirely(
      IxGenericDataSource dataSource, DxCustomPreProcessDataSource customPreProcessingHandler)
    {
      if (dataSource != null && EntityInstance != null)
      {
        // Data source
        EntityInstance.PreProcessDataSource(dataSource, customPreProcessingHandler);
        // Data columns
        for (int i = 0; i < dataSource.Columns.Count; i++)
        {
          PreProcessDataColumn(dataSource.Columns[i]);
        }
        // Data rows
        for (int i = 0; i < dataSource.Count; i++)
        {
          EntityInstance.PreProcessDataRow(dataSource[i]);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Preprocesses the given data-column accordingly to the entity's attributes.
    /// </summary>
    /// <param name="column">a data column to be pre-processed</param>
    public void PreProcessDataColumn(DataColumn column)
    {
      if (column.DataType == typeof(string))
      {
        CxAttributeMetadata attribute = GetAttribute(column.ColumnName);
        if (attribute != null)
        {
          if (column.DataType == typeof(string) && attribute.Type == CxAttributeMetadata.TYPE_LONGSTRING)
            column.MaxLength = int.MaxValue;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if hierarchical entity usage should be always expanded in the grid.
    /// </summary>
    public bool IsAlwaysExpanded
    { get { return this["always_expanded"] == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Retruns list of entity usages which use this entity usage as inherited entity usage.
    /// </summary>
    public IList<CxEntityUsageMetadata> DirectDescendantEntityUsages
    {
      get
      {
        if (m_DirectDescendantEntityUsages == null)
        {
          m_DirectDescendantEntityUsages = GetDirectlyInheritedEntityUsages();
        }
        return m_DirectDescendantEntityUsages;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all descendant entity usages.
    /// </summary>
    protected void GetDescendantEntityUsagesList(
      CxEntityUsageMetadata entityUsage,
      IList<CxEntityUsageMetadata> list,
      Hashtable checkedMap)
    {
      foreach (CxEntityUsageMetadata descendant in entityUsage.DirectDescendantEntityUsages)
      {
        if (!checkedMap.ContainsKey(descendant))
        {
          list.Add(descendant);
          checkedMap[descendant] = true;
          GetDescendantEntityUsagesList(descendant, list, checkedMap);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages which use this entity usage as inherited entity usage.
    /// </summary>
    public IList<CxEntityUsageMetadata> DescendantEntityUsages
    {
      get
      {
        if (m_DescendantEntityUsages == null)
        {
          m_DescendantEntityUsages = new List<CxEntityUsageMetadata>();
          Hashtable checkedMap = new Hashtable();
          GetDescendantEntityUsagesList(this, m_DescendantEntityUsages, checkedMap);
        }
        return m_DescendantEntityUsages;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns global collection of entity usages.
    /// </summary>
    public CxEntityUsagesMetadata EntityUsages
    {
      get
      {
        return Holder != null && Holder.EntityUsages != null ? Holder.EntityUsages : m_EntityUsages;
      }
      set { m_EntityUsages = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return LOCALIZATION_OBJECT_TYPE_CODE;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of metadata objects properties was inherited from.
    /// </summary>
    override public IList<CxMetadataObject> InheritanceList
    {
      get
      {
        List<CxMetadataObject> list = new List<CxMetadataObject>();
        if (InheritedEntityUsage != null)
        {
          list.Add(InheritedEntityUsage);
        }
        list.Add(Entity);
        return list;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this entity usage is inherited from the given entity usage.
    /// </summary>
    public bool IsInheritedFrom(CxEntityUsageMetadata entityUsage)
    {
      CxEntityUsageMetadata ancestor = this;
      while (ancestor != null)
      {
        if (ancestor == entityUsage)
        {
          return true;
        }
        ancestor = ancestor.InheritedEntityUsage;
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this entity usage is inherited from the given entity usage.
    /// </summary>
    /// <param name="entityUsageId">entity usage id to be evaluated</param>
    /// <returns>true if this entity usage is inherited from the given entity usage</returns>
    public bool IsInheritedFrom(string entityUsageId)
    {
      CxEntityUsageMetadata entityUsage = Holder.EntityUsages[entityUsageId];
      return IsInheritedFrom(entityUsage);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given entity usage metadata is inherited from this 
    /// entity usage or vise versa.
    /// </summary>
    public bool IsAncestorOrDescendant(CxEntityUsageMetadata entityUsage)
    {
      if (entityUsage != null)
      {
        return IsInheritedFrom(entityUsage) || entityUsage.IsInheritedFrom(this);
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given entity usage contains all this entity usage attributes.
    /// </summary>
    public bool IsCompatibleByAttributes(CxEntityUsageMetadata entityUsage)
    {
      if (entityUsage != null && entityUsage.EntityId == EntityId)
      {
        if (entityUsage.Id == Id)
        {
          return true;
        }
        foreach (CxAttributeMetadata attr in Attributes)
        {
          if (entityUsage.GetAttribute(attr.Id) == null)
          {
            return false;
          }
        }
        return true;
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages inherited from this entity.
    /// </summary>
    override public IList<CxEntityUsageMetadata> GetInheritedEntityUsages()
    {
      List<CxEntityUsageMetadata> list = new List<CxEntityUsageMetadata>();
      foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
      {
        if (entityUsage.InheritedEntityUsage == this)
        {
          list.Add(entityUsage);
          list.AddRange(entityUsage.GetInheritedEntityUsages());
        }
      }
      return list;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages which are referenced by attribute properties.
    /// </summary>
    /// <returns>list of CxEntityMetadata objects or null</returns>
    override public IList<CxEntityMetadata> GetReferencedEntities()
    {
      UniqueList<CxEntityMetadata> result = new UniqueList<CxEntityMetadata>();

      result.Add(Entity);

      if (FileLibraryCategoryEntityUsage != null && FileLibraryCategoryEntityUsage != this)
        result.Add(FileLibraryCategoryEntityUsage);

      foreach (CxAttributeMetadata attribute in Attributes)
        result.AddRange(attribute.GetReferencedEntities());

      foreach (CxCommandMetadata command in Commands)
        result.AddRange(command.GetReferencedEntities());

      foreach (CxEntityUsageMetadata entityUsage in GetDirectlyInheritedEntityUsages())
      {
        result.Add(entityUsage);
        result.AddRange(entityUsage.GetReferencedEntities());
      }

      return result;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates instance of the entity class (defined as entity_class_id).
    /// </summary>
    /// <returns>created instance</returns>
    public object CreateEntityInstance()
    {
      return CxType.CreateInstance(
        EntityClass,
        new Type[] { typeof(CxEntityUsageMetadata) },
        new object[] { this });
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Empty entity instance for internal purposes.
    /// </summary>
    protected IxEntity EntityInstance
    {
      get
      {
        if (m_EntityInstance == null)
        {
          m_EntityInstance = CreateEntityInstance();
        }
        return m_EntityInstance as IxEntity;
      }
    }
    //-------------------------------------------------------------------------
    public override CxAttributeMetadata[] GetPrimaryKeyAttributes(int dbObjectIndex)
    {
      CxAttributeMetadata[] result = EntityInstance.GetPrimaryKeyAttributes(dbObjectIndex);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares value providers by primary key values.
    /// </summary>
    /// <param name="p1">provider #1</param>
    /// <param name="p2">provider #2</param>
    /// <returns>true if equals</returns>
    public bool CompareByPK(IxValueProvider p1, IxValueProvider p2)
    {
      if (p1 != null && p2 != null)
      {
        CxAttributeMetadata[] pkAttrs = PrimaryKeyAttributes;
        if (pkAttrs != null && pkAttrs.Length > 0)
        {
          foreach (CxAttributeMetadata pkAttr in pkAttrs)
          {
            if (!CxUtils.Compare(p1[pkAttr.Id], p2[pkAttr.Id]))
            {
              return false;
            }
          }
          return true;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of attributes that may contain the entity usage id 
    /// the entity instance depends on.
    /// </summary>
    /// <returns></returns>
    public CxAttributeMetadata[] GetEntityInstanceDependentAttributesToCalculateAutoRefreshBy()
    {
      List<CxAttributeMetadata> result = new List<CxAttributeMetadata>();

      foreach (CxAttributeMetadata attributeMetadata in Attributes)
      {
        if (attributeMetadata.Type == CxAttributeMetadata.TYPE_LINK &&
            attributeMetadata.HyperLinkAutoRefresh)
        {
          if (GetValueDefinedAttribute(attributeMetadata) != null)
          {
            if (CxUtils.NotEmpty(attributeMetadata.HyperLinkEntityUsageAttrId))
              result.Add(GetAttribute(attributeMetadata.HyperLinkEntityUsageAttrId));
          }
        }
      }
      return result.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if entity usage has at least one referencing column with
    /// auto refresh flag set to true.
    /// </summary>
    public bool HasAutoRefreshableAttributes(
      CxEntityUsageMetadata changedEntityUsageMetadata)
    {
      bool result = false;

      foreach (CxAttributeMetadata attributeMetadata in Attributes)
      {
        if (attributeMetadata.Type == CxAttributeMetadata.TYPE_LINK &&
            attributeMetadata.HyperLinkAutoRefresh)
        {
          if (GetValueDefinedAttribute(attributeMetadata) != null)
          {
            if (attributeMetadata.HyperLinkEntityUsageId == changedEntityUsageMetadata.Id)
            {
              result = true;
            }
          }
        }

        if (!result)
        {
          result =
            attributeMetadata.ReferenceAutoRefresh &&
            ((IList) attributeMetadata.ReferenceEntityUsages).Contains(changedEntityUsageMetadata);
        }
        if (result)
          break;
      }

      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if refresh of the current entity depends on the given
    /// entity usage. I.e. current entity should be refreshed when the given
    /// entity usage is changed.
    /// </summary>
    /// <param name="entityUsage">entity usage to check dependency on</param>
    /// <returns>true if refresh of the current entity depends on the given entity usage</returns>
    public bool IsRefreshDependentOn(CxEntityUsageMetadata entityUsage)
    {
      if (entityUsage != null)
      {
        if (IsAncestorOrDescendant(entityUsage))
        {
          return true;
        }
        IList<string> entityUsageIds = CxText.DecomposeWithSeparator(RefreshDependsOnEntityUsages, ",");
        foreach (string entityUsageId in entityUsageIds)
        {
          CxEntityUsageMetadata dependency = Holder.EntityUsages.Find(entityUsageId);
          if (dependency != null && entityUsage.IsInheritedFrom(dependency))
          {
            return true;
          }
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines an attribute with the given id and initial values on the level of
    /// the current entity metadata. Overriden.
    /// </summary>
    /// <param name="attributeId">id of the attribute to be added</param>
    /// <param name="initialValues">initial values of the attribute to be added</param>
    /// <returns>defined attribute metadata</returns>
    public override CxAttributeMetadata DefineEntityAttribute(
      string attributeId, IDictionary<string, string> initialValues)
    {
      return DefineEntityAttribute(attributeId, initialValues, "attribute_usage");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of direct descendants of the current entity usage.
    /// </summary>
    /// <returns>a list of direct descendants</returns>
    public override IList<CxEntityUsageMetadata> GetDirectlyInheritedEntityUsages()
    {
      List<CxEntityUsageMetadata> list = new List<CxEntityUsageMetadata>();
      foreach (CxEntityUsageMetadata entityUsage in Holder.EntityUsages.Items)
      {
        if (entityUsage.InheritedEntityUsage == this)
          list.Add(entityUsage);
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of child entity usage ids built on the ground of the given childs.
    /// </summary>
    /// <param name="childs">childs to be used to build the result</param>
    /// <returns>list of child entity usage ids</returns>
    public IList<string> GetIdsFromChildEntityUsage(IEnumerable<CxChildEntityUsageMetadata> childs)
    {
      UniqueList<string> result = new UniqueList<string>();
      foreach (CxChildEntityUsageMetadata child in childs)
      {
        result.Add(child.Id);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of child entity usages built on the ground of the given attribute ids.
    /// </summary>
    /// <param name="childEntityUsageIds">child entity usage ids to be used to build the result</param>
    /// <returns>list of attributes</returns>
    public IList<CxChildEntityUsageMetadata> GetChildEntityUsagesFromIds(IEnumerable<string> childEntityUsageIds)
    {
      // Validate
      if (childEntityUsageIds == null)
        throw new ExNullArgumentException("childEntityUsageIds");

      UniqueList<CxChildEntityUsageMetadata> result = new UniqueList<CxChildEntityUsageMetadata>();
      foreach (string childEntityUsageId in childEntityUsageIds)
      {
        CxChildEntityUsageMetadata childEntityUsage = GetChildEntityUsage(childEntityUsageId);
#if DEBUG
        //if (attribute == null)
        //  throw new ExException(
        //    string.Format("Cannot find attribute by its id: <{0}.{1}>", Id, attributeId), new ExNullReferenceException("attribute"));
#endif
        result.Add(childEntityUsage);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns child entity usage names for new added child entity usages that are not
    /// included into the child entity usage order.
    /// </summary>
    protected internal UniqueList<string> NewChildEntityUsageNames
    {
      get
      {
        if (m_NewChildEntityUsageNames == null)
        {
          m_NewChildEntityUsageNames = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
          if (CxUtils.NotEmpty(this["known_child_entity_usages"]))
          {
            IList<string> list = CxText.DecomposeWithWhiteSpaceAndComma(this["known_child_entity_usages"]);
            UniqueList<string> oldChildEntityUsageList = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
            oldChildEntityUsageList.AddRange(CxList.ToList<string>(list));
            foreach (CxChildEntityUsageMetadata child in ChildEntityUsagesList)
            {
              if (!oldChildEntityUsageList.Contains(child.Id))
              {
                m_NewChildEntityUsageNames.Add(child.Id);
              }
            }
          }
        }
        return m_NewChildEntityUsageNames;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if Entity should be saved
    /// </summary>
    public bool IsAlwaysSaveOnEdit
    { get { return this["always_save_on_edit"] == "true"; } }
    //-------------------------------------------------------------------------
  }
}