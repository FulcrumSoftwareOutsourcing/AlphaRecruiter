using System;
using System.Collections.Generic;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxChildEntityUsageOrder
  {
    //-------------------------------------------------------------------------
    private UniqueList<string> m_XmlExplicitOrderIds;
    private List<string> m_CustomOrderIds;
    private IList<string> m_XmlImplicitOrderIdsCache;
    private CxEntityUsageMetadata m_XmlExplicitSource;
    private CxEntityUsageMetadata m_Metadata;
    private NxChildEntityUsageOrderType m_OrderType;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the custom ordered ids list.
    /// </summary>
    protected List<string> CustomOrderIds
    {
      get { return m_CustomOrderIds; }
      set
      {
        if (value != null)
          m_CustomOrderIds = new List<string>(value);
        else
          m_CustomOrderIds = null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if custom order is used.
    /// </summary>
    public bool IsCustom
    { get { return CustomOrderIds != null; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The source of the explicit XML order.
    /// </summary>
    protected CxEntityUsageMetadata XmlExplicitSource
    {
      get { return m_XmlExplicitSource; }
      set { m_XmlExplicitSource = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attributes in the actual order to be used.
    /// </summary>
    public IList<CxChildEntityUsageMetadata> OrderChildEntityUsages
    {
      get
      {
        return Metadata.GetChildEntityUsagesFromIds(OrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attributes in the actual order to be used +
    /// all the "new" attributes as well.
    /// </summary>
    public IList<CxChildEntityUsageMetadata> OrderPlusNewChildEntityUsages
    {
      get
      {
        UniqueList<CxChildEntityUsageMetadata> result = new UniqueList<CxChildEntityUsageMetadata>();
        result.AddRange(OrderChildEntityUsages);
        if (IsCustom)
        {
          foreach (string name in Metadata.NewChildEntityUsageNames)
          {
            CxChildEntityUsageMetadata childEntityUsage = Metadata.GetChildEntityUsage(name);
            result.Add(childEntityUsage);
          }
        }
        return result;
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
        return XmlImplicitOrderIds;
      }
    }
    //-------------------------------------------------------------------------

    #region Xml Implicit orders
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the child entity usage order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitOrderIds
    {
      get
      {
        if (m_XmlImplicitOrderIdsCache == null)
        {
          m_XmlImplicitOrderIdsCache = GetAllIdsInNaturalOrder();
        }
        return m_XmlImplicitOrderIdsCache;
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Xml Explicit orders
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
          return XmlExplicitSource.GetChildEntityUsageOrder(OrderType).XmlExplicitOrderIds;
        }
        return m_XmlExplicitOrderIds;
      }
    }
    //-------------------------------------------------------------------------
    public NxChildEntityUsageOrderType OrderType
    {
      get { return m_OrderType; }
      protected set { m_OrderType = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityUsageMetadata Metadata
    {
      get { return m_Metadata; }
      protected set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Ctors
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">entity usage metadata</param>
    /// <param name="orderType">the type of the order</param>
    public CxChildEntityUsageOrder(CxEntityUsageMetadata metadata, NxChildEntityUsageOrderType orderType)
    {
      Metadata = metadata;
      OrderType = orderType;
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
    /// Returns ids of all the child in their natural order.
    /// </summary>
    /// <returns></returns>
    protected IList<string> GetAllIdsInNaturalOrder()
    {
      List<CxChildEntityUsageMetadata> childEntityUsages = new List<CxChildEntityUsageMetadata>();
      foreach (CxChildEntityUsageMetadata childMetadata in Metadata.ChildEntityUsagesList)
      {
        if (OrderType == NxChildEntityUsageOrderType.InList && childMetadata.IsVisibleInList)
          childEntityUsages.Add(childMetadata);
        if (OrderType == NxChildEntityUsageOrderType.InView && childMetadata.IsVisibleInView)
          childEntityUsages.Add(childMetadata);
      }
      return Metadata.GetIdsFromChildEntityUsage(childEntityUsages);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets custom order from the given comma-separated text.
    /// </summary>
    /// <param name="orderText">comma-separated text</param>
    public void SetCustomOrder(string orderText)
    {
      if (CxUtils.IsEmpty(CxText.TrimSpace(orderText)))
      {
        CustomOrderIds = null;
      }
      else
      {
        IList<string> resultList =
          CxText.RemoveEmptyStrings(CxText.DecomposeWithWhiteSpaceAndComma(orderText.ToUpper()));

        CustomOrderIds = new List<string>();
        CustomOrderIds.AddRange(resultList);
      }
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
      SetXmlDefSource(Metadata);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets source for the XML defined attribute order.
    /// </summary>
    /// <param name="source">source for the attribute order</param>
    protected void SetXmlDefSource(CxEntityUsageMetadata source)
    {
      if (source != Metadata)
      {
        if (m_XmlExplicitOrderIds != null)
        {
          m_XmlExplicitSource = null;
          return;
        }
        m_XmlExplicitSource = source;
      }
      else
      {
        m_XmlExplicitSource = null;
      }
      foreach (CxEntityUsageMetadata entityUsage in Metadata.Holder.EntityUsages.Items)
      {
        if (entityUsage.InheritedEntityUsage == Metadata || entityUsage.Entity == Metadata)
        {
          CxChildEntityUsageOrder childEntityUsageOrder = entityUsage.GetChildEntityUsageOrder(OrderType);
          childEntityUsageOrder.SetXmlDefSource(Metadata);
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
