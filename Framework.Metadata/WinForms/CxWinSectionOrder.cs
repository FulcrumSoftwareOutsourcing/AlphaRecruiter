using System;
using System.Collections.Generic;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinSectionOrder
  {
    //-------------------------------------------------------------------------
    private UniqueList<string> m_XmlExplicitOrderIds;
    private List<string> m_CustomOrderIds;
    private IList<string> m_XmlImplicitOrderIdsCache;
    private CxWinSectionsMetadata m_XmlExplicitSource;
    private readonly CxWinSectionsMetadata m_Metadata;
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
    protected CxWinSectionsMetadata XmlExplicitSource
    {
      get { return m_XmlExplicitSource; }
      set { m_XmlExplicitSource = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the sections in the actual order to be used.
    /// </summary>
    public IList<CxWinSectionMetadata> OrderSections
    {
      get
      {
        return m_Metadata.GetSectionsFromIds(OrderIds);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the sectins in the actual order to be used +
    /// all the "new" sections as well.
    /// </summary>
    public IList<CxWinSectionMetadata> OrderPlusNewSections
    {
      get
      {
        UniqueList<CxWinSectionMetadata> result = new UniqueList<CxWinSectionMetadata>();
        result.AddRange(OrderSections);
        if (IsCustom)
        {
          foreach (string name in m_Metadata.NewSectionNames)
          {
            CxWinSectionMetadata section = m_Metadata[name];
            result.Add(section);
          }
        }
        return result;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the section ids in the actual order to be used.
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
    /// Returns the section order implicitly defined in the XML.
    /// </summary>
    protected IList<string> XmlImplicitOrderIds
    {
      get
      {
        if (m_XmlImplicitOrderIdsCache == null)
        {
          m_XmlImplicitOrderIdsCache = new List<string>();
          IList<string> sectionIds = GetAllIdsInNaturalOrder();
          foreach (string id in sectionIds)
          {
            CxWinSectionMetadata item = m_Metadata.AllItems[id];
            if (CxBool.Parse(item["visible"], true))
            {
              m_XmlImplicitOrderIdsCache.Add(id);
            }
          }
        }
        return m_XmlImplicitOrderIdsCache;
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Xml Explicit orders
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ids of the sections in order of their explicit declaration.
    /// </summary>
    protected IList<string> XmlExplicitOrderIds
    {
      get
      {
        if (XmlExplicitSource != null)
        {
          return XmlExplicitSource.WinSectionOrder.XmlExplicitOrderIds;
        }
        return m_XmlExplicitOrderIds;
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Ctors
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">section metadata</param>
    public CxWinSectionOrder(CxWinSectionsMetadata metadata)
    {
      m_Metadata = metadata;
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
    /// Returns ids of all the section in their natural order.
    /// </summary>
    /// <returns></returns>
    protected IList<string> GetAllIdsInNaturalOrder()
    {
      return m_Metadata.GetIdsFromSection(m_Metadata.AllItemsListWithoutHiddenForUser);
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
      SetXmlDefSource(m_Metadata);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets source for the XML defined section order.
    /// </summary>
    /// <param name="source">source for the section order</param>
    protected void SetXmlDefSource(CxWinSectionsMetadata source)
    {
      if (source != m_Metadata)
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
