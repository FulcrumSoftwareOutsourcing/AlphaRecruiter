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
using Framework.Utils;

namespace Framework.Metadata
{
  // Let's review the attribute orders architecture.
  // One instance of the CxAttributeOrder class belongs to one entity/entity_usage, and
  // describes just one order, one of these types: GridVisible, Edit, Filter, Queryable.
  // At the same time, the "sorts" of the order are different too,
  // for instance, the order can be defined by direct XML assignment,
  // can be not defined (therefore we use the "natural" order of the attributes),
  // or be defined customly.
  // 
  // Let's provide some terminology for that all.
  // "XmlImplicit" order is the "natural" order of the attributes defined as appliable 
  // to the current type.
  // "XmlExplicit" order is the order defined explicitly using the XML *_order element.
  // "Custom" order is the order that's customized by the user. Stored in database so far.

  //---------------------------------------------------------------------------
  /// <summary>
  /// Class to define operations with ordered attribute list.
  /// </summary>
  public class CxAttributeOrder
  {
    //-------------------------------------------------------------------------
    protected NxAttributeContext m_Type;
    protected CxEntityMetadata m_Metadata = null;
    protected UniqueList<string> m_XmlExplicitOrderIds = null;
    private List<string> m_CustomOrderIds = null;
    private CxEntityMetadata m_XmlExplicitSource = null;
    private CxEntityMetadata m_CustomOrderSource = null;
    //-------------------------------------------------------------------------
    private IList<string> m_XmlImplicitFilterableOrderIds_Cache;
    private IList<string> m_XmlImplicitVisibleOrderIds_Cache;
    private IList<string> m_XmlImplicitEditableOrderIds_Cache;
    private IList<string> m_XmlImplicitQueryableOrderIds_Cache;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    { get { return m_Metadata as CxEntityUsageMetadata; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if custom order is used.
    /// </summary>
    public bool IsCustom
    { get { return CustomOrderIds != null; } }
    //-------------------------------------------------------------------------

    #region Xml Implicit orders
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the Editable order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitEditableOrderIds
    {
      get
      {
        if (m_XmlImplicitEditableOrderIds_Cache == null)
        {
          IList<string> attributeIds = GetAllIdsInNaturalOrder();
          IList<string> result = new UniqueList<string>();
          foreach (string attributeId in attributeIds)
          {
            CxAttributeMetadata attribute = m_Metadata.GetAttribute(attributeId);
            if (attribute == null)
              throw new ExNullReferenceException("attribute");
            attribute = m_Metadata.GetValueAttribute(attribute) ?? attribute;
            if (attribute.Editable)
            {
              result.Add(attribute.Id);
            }
          }
          m_XmlImplicitEditableOrderIds_Cache = RebuildEditAttributesTakingIntoAccountControlPlacement(result);
        }
        return m_XmlImplicitEditableOrderIds_Cache;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the Filterable order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitFilterableOrderIds
    {
      get
      {
        if (m_XmlImplicitFilterableOrderIds_Cache == null)
        {
          IList<string> attributeIds = GetAllIdsInNaturalOrder();
          IList<string> result = new UniqueList<string>();
          foreach (string attributeId in attributeIds)
          {
            CxAttributeMetadata attribute = m_Metadata.GetAttribute(attributeId);
            attribute = m_Metadata.GetValueAttribute(attribute) ?? attribute;
            if (attribute.Filterable)
              result.Add(attribute.Id);
          }
          m_XmlImplicitFilterableOrderIds_Cache = result;
        }
        return m_XmlImplicitFilterableOrderIds_Cache;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the Queryable order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitQueryableOrderIds
    {
      get
      {
        if (m_XmlImplicitQueryableOrderIds_Cache == null)
        {
          IList<CxAttributeMetadata> attributes = GetAllAttributesInNaturalOrder();
          IList<string> result = new UniqueList<string>();
          foreach (CxAttributeMetadata attribute in attributes)
          {
            if (attribute.IsQueryable)
              result.Add(attribute.Id);
          }
          m_XmlImplicitQueryableOrderIds_Cache = result;
        }
        return m_XmlImplicitQueryableOrderIds_Cache;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the Visible order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitVisibleOrderIds
    {
      get
      {
        if (m_XmlImplicitVisibleOrderIds_Cache == null)
        {
          IList<string> attributeIds = GetAllIdsInNaturalOrder();
          IList<string> result = new UniqueList<string>();
          foreach (string attributeId in attributeIds)
          {
            CxAttributeMetadata attribute = m_Metadata.GetAttribute(attributeId);
            if (attribute.Visible)
              result.Add(attribute.Id);
          }
          m_XmlImplicitVisibleOrderIds_Cache = result;
        }
        return m_XmlImplicitVisibleOrderIds_Cache;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the current-type order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitOrderIds
    {
      get
      {
        switch (m_Type)
        {
          case NxAttributeContext.Edit:
            return XmlImplicitEditableOrderIds;
          case NxAttributeContext.Filter:
            return XmlImplicitFilterableOrderIds;
          case NxAttributeContext.GridVisible:
            return XmlImplicitVisibleOrderIds;
          case NxAttributeContext.Queryable:
            return XmlImplicitQueryableOrderIds;
          default:
            throw new ExException(
              string.Format(
                "Unknown NxAttributeOrderType encountered: <{0}>", m_Type));
        }
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Xml Explicit orders
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the XML order has been overridden in the current entity.
    /// </summary>
    public bool HasOverriddenXmlExplicitOrder
    {
      get { return !CxList.IsEmpty2(m_XmlExplicitOrderIds); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ids of the attributes in order of their explicit declaration.
    /// </summary>
    protected IList<string> XmlExplicitOrderIds
    {
      get
      {
        if (XmlExplicitSource != null)
        {
          return XmlExplicitSource.GetAttributeOrder(m_Type).XmlExplicitOrderIds;
        }
        else
        {
          return m_XmlExplicitOrderIds;
        }
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    /// <summary>
    /// Xml defined order.
    /// </summary>
    public IList<string> XmlOrderIds
    {
      get
      {
        IList<string> order = XmlExplicitOrderIds;
        if (!CxList.IsEmpty2(order))
        {
          return order;
        }
        else
        {
          return XmlImplicitOrderIds;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attribute ids in the actual order to be used.
    /// </summary>
    public IList<string> OrderIds
    {
      get
      {
        IList<string> orderIds = GetOrderIds();
        if (orderIds == null)
          throw new ExNullReferenceException("orderIds");
        return orderIds;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attributes in the actual order to be used.
    /// </summary>
    public IList<CxAttributeMetadata> OrderAttributes
    {
      get
      {
        return m_Metadata.GetAttributesFromIds(OrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attributes in the actual order to be used + 
    /// all the other attributes as well.
    /// </summary>
    public IList<CxAttributeMetadata> OrderPlusAllAttributes
    {
      get
      {
        List<CxAttributeMetadata> result = new List<CxAttributeMetadata>();
        
        result.AddRange(OrderAttributes);
        foreach (CxAttributeMetadata attribute in m_Metadata.Attributes)
        {
          string id = attribute.Id;
          if (result.Find(delegate(CxAttributeMetadata item)
                            { return item.Id == id; }) == null)
          result.Add(attribute);
        }
        return result;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attributes in the actual order to be used +
    /// all the "new" attributes as well.
    /// </summary>
    public IList<CxAttributeMetadata> OrderPlusNewAttributes
    {
      get
      {
        UniqueList<CxAttributeMetadata> result = new UniqueList<CxAttributeMetadata>();
        result.AddRange(OrderAttributes);
        if (IsCustom)
        {
          foreach (string name in m_Metadata.NewAttributeNames)
          {
            CxAttributeMetadata attr = m_Metadata.GetAttribute(name);
            if ((m_Type == NxAttributeContext.Edit && attr.Editable) ||
                (m_Type == NxAttributeContext.Filter && attr.Filterable))
            {
              result.Add(attr);
            }
          }
        }
        return result;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the custom ordered ids list.
    /// </summary>
    protected List<string> CustomOrderIds
    {
      get
      {
        if (CustomOrderSource != null)
          return CustomOrderSource.GetAttributeOrder(m_Type).CustomOrderIds;
        else
          return m_CustomOrderIds;
      }
      set
      {
        if (value != null)
        {
          m_CustomOrderIds = new List<string>(value);
          CustomOrderSource = null;
        }
        else
          m_CustomOrderIds = null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The source of the custom order.
    /// </summary>
    protected CxEntityMetadata CustomOrderSource
    {
      get { return m_CustomOrderSource; }
      set { m_CustomOrderSource = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The source of the explicit XML order.
    /// </summary>
    protected CxEntityMetadata XmlExplicitSource
    {
      get { return m_XmlExplicitSource; }
      set { m_XmlExplicitSource = value; }
    }
    //-------------------------------------------------------------------------

    #region Ctors
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">entity metadata</param>
    /// <param name="type">order type</param>
    public CxAttributeOrder(CxEntityMetadata metadata, NxAttributeContext type)
    {
      m_Metadata = metadata;
      m_Type = type;
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Methods
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the ordered ids list.
    /// </summary>
    protected IList<string> GetOrderIds()
    {
      if (CustomOrderIds != null)
      {
        return CustomOrderIds;
      }

      return XmlOrderIds;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all the attributes in their natural order.
    /// </summary>
    /// <returns></returns>
    protected IList<CxAttributeMetadata> GetAllAttributesInNaturalOrder()
    {
      return m_Metadata.Attributes;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ids of all the attributes in their natural order.
    /// </summary>
    /// <returns></returns>
    protected IList<string> GetAllIdsInNaturalOrder()
    {
      return m_Metadata.GetIdsFromAttributes(m_Metadata.Attributes);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Rebuilds the given set of attribute ids so that the new order will be taking
    /// into account attribute placement on the form.
    /// </summary>
    /// <param name="attributeIds">ids to rebuild</param>
    /// <returns>rebuilt ids</returns>
    protected IList<string> RebuildEditAttributesTakingIntoAccountControlPlacement(
      IEnumerable<string> attributeIds)
    {
      if (m_Metadata.Holder.Config.ApplicationScope == NxApplicationScope.Web)
        return new List<string>(attributeIds);

      if (m_Metadata.Holder.Config.ApplicationScope == NxApplicationScope.Silverlight)
        return new List<string>(attributeIds);

      if (m_Metadata.Holder.Config.ApplicationScope == NxApplicationScope.Windows)
      {
        // For now just windows edit order is taken into account.

        // Input attributes.
        IList<CxAttributeMetadata> attributes = m_Metadata.GetAttributesFromIds(attributeIds);

        // Each panel holds its own list of attributes.
        Dictionary<string, List<CxAttributeMetadata>> panelAttributes =
          new Dictionary<string, List<CxAttributeMetadata>>();

        foreach (CxAttributeMetadata attribute in attributes)
        {
          string placement = attribute.CurrentScopeControlPlacement.ToUpper();
          if (string.IsNullOrEmpty(placement))
            placement = attribute.DefaultControlPlacement.ToUpper();
          List<CxAttributeMetadata> list;
          if (panelAttributes.ContainsKey(placement))
            list = panelAttributes[placement];
          else
          {
            list = new List<CxAttributeMetadata>();
            panelAttributes[placement] = list;
          }
          list.Add(attribute);
        }

        UniqueList<CxAttributeMetadata> result = new UniqueList<CxAttributeMetadata>();
        if (m_Metadata.WinForm != null)
        {
          // If there're some panels defined on the form:

          foreach (CxWinTabMetadata tab in m_Metadata.WinForm.GetTabOrderManager().Tabs)
          {
            foreach (CxWinPanelMetadata panel in tab.Panels)
            {
              if (panelAttributes.ContainsKey(panel.Id))
                result.AddRange(panelAttributes[panel.Id]);
            }
          }
        }
        else
        {
          // If no tabs/panels are defined for the entity:

          List<List<CxAttributeMetadata>> list = CxDictionary.ExtractDictionaryValueList(panelAttributes);
          if (list.Count > 1)
            throw new ExException("The only list is expected in the dictionary");

          if (list.Count == 1)
            result.AddRange(CxDictionary.ExtractDictionaryValueList(panelAttributes)[0]);
        }
        return m_Metadata.GetIdsFromAttributes(result);
      }
      
      throw new ExException("Unknown application scope encountered");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets XML defined order from the given comma-separated text.
    /// </summary>
    /// <param name="orderText">comma-separated text</param>
    public void SetXmlDefOrder(string orderText)
    {
      if (CxUtils.IsEmpty(CxText.TrimSpace(orderText))) return;

      IList<string> resultList = 
        CxText.RemoveEmptyStrings(CxText.DecomposeWithWhiteSpaceAndComma(orderText.ToUpper()));
     
      m_XmlExplicitOrderIds = new UniqueList<string>(StringComparer.OrdinalIgnoreCase);
      m_XmlExplicitOrderIds.AddRange(resultList);
      SetXmlDefSource(m_Metadata);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets source for the XML defined attribute order.
    /// </summary>
    /// <param name="source">source for the attribute order</param>
    protected void SetXmlDefSource(CxEntityMetadata source)
    {
      if (source != m_Metadata)
      {
        if (m_XmlExplicitOrderIds != null)
        {
          XmlExplicitSource = null;
          return;
        }
        XmlExplicitSource = source;
      }
      else
      {
        XmlExplicitSource = null;
      }
      foreach (CxEntityUsageMetadata entityUsage in m_Metadata.Holder.EntityUsages.Items)
      {
        if (entityUsage.InheritedEntityUsage == m_Metadata || entityUsage.Entity == m_Metadata)
        {
          CxAttributeOrder attrOrder = entityUsage.GetAttributeOrder(m_Type);
          attrOrder.SetXmlDefSource(m_Metadata);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets custom order from the given comma-separated text.
    /// </summary>
    /// <param name="orderText">comma-separated text</param>
    public void SetCustomOrder(string orderText)
    {
      if (orderText == null)
      {
        CustomOrderIds = null;
        SetCustomOrderSource(null);
      }
      else
      {
        IList<string> resultList =
          CxText.RemoveEmptyStrings(CxText.DecomposeWithWhiteSpaceAndComma(orderText.ToUpper()));

        CustomOrderIds = new List<string>();
        CustomOrderIds.AddRange(resultList);
        SetCustomOrderSource(m_Metadata);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets source for the XML defined attribute order.
    /// </summary>
    /// <param name="source">source for the attribute order</param>
    protected void SetCustomOrderSource(CxEntityMetadata source)
    {
      if (source != m_Metadata)
      {
        if (CustomOrderIds != null)
        {
          CustomOrderSource = null;
          return;
        }
        CustomOrderSource = source;
      }
      else
      {
        CustomOrderSource = null;
      }
      // Here we should set the current entity as the custom source for the inherited entities which do 
      // not have their own XML explicit or custom order.
      foreach (CxEntityUsageMetadata entityUsage in m_Metadata.Holder.EntityUsages.Items)
      {
        if (entityUsage.InheritedEntityUsage == m_Metadata || entityUsage.Entity == m_Metadata)
        {
          CxAttributeOrder attrOrder = entityUsage.GetAttributeOrder(m_Type);
          if (!attrOrder.HasOverriddenXmlExplicitOrder && CxList.IsEmpty2(attrOrder.CustomOrderIds))
            attrOrder.SetCustomOrderSource(m_Metadata);
        }
      }
    }    
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets order to XML defined order.
    /// </summary>
    public void ResetToDefault()
    {
      CustomOrderIds = null;
    }
    //-------------------------------------------------------------------------
    #endregion
  }
}