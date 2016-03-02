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

using System.Collections.Generic;

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinTabOrderManager
  {
    //-------------------------------------------------------------------------
    private CxWinFormMetadata m_FormMetadata;
    private IList<string> m_OrderIds_Cache;
    private IList<CxWinTabMetadata> m_OrderTabs_Cache;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Win form metadata the manager belongs to.
    /// </summary>
    public CxWinFormMetadata FormMetadata
    {
      get { return m_FormMetadata; }
      protected set { m_FormMetadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// A list of tabs ordered.
    /// </summary>
    public IList<CxWinTabMetadata> Tabs { get { return GetTabs(); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// A list of tabs ordered in the non-custom (factory) order.
    /// </summary>
    public IList<CxWinTabMetadata> NonCustomTabs { get { return GetNonCustomTabs(); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// A list of tab ids orderer in the non-custom (factory) order.
    /// </summary>
    public IList<string> NonCustomIds { get { return GetNonCustomIds(); } }
      //-------------------------------------------------------------------------
    /// <summary>
    /// A list of tab ids ordered.
    /// </summary>
    public IList<string> Ids { get { return GetIds(); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="formMetadata">win-form metadata the order belongs to</param>
    public CxWinTabOrderManager(CxWinFormMetadata formMetadata)
    {
      if (formMetadata == null)
        throw new ExNullArgumentException("formMetadata");
      FormMetadata = formMetadata;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of tab ids ordered.
    /// </summary>
    protected IList<string> GetIds()
    {
      if (m_OrderIds_Cache == null)
      {
        string customTabOrderString = FormMetadata.CustomTabOrder;

        // if the string is null then we have no customized tab order
        // however if the string is just empty (or contains a space), it tells us
        // about the complete tabs invisibility.
        if (string.IsNullOrEmpty(customTabOrderString))
        {
          m_OrderIds_Cache = new List<string>();
          List<CxWinTabMetadata> tabs = new List<CxWinTabMetadata>(FormMetadata.Tabs);
          
          // Sort the tabs
          SortTabs(tabs);

          foreach (CxWinTabMetadata tabMetadata in tabs)
          {
            if (tabMetadata.Visible)
              m_OrderIds_Cache.Add(tabMetadata.Id);
          }
        }
        else
        {
          m_OrderIds_Cache = new List<string>();

          IList<string> orderIds = new List<string>(CxText.DecomposeWithWhiteSpaceAndComma(customTabOrderString));

          // Then we fill the original list with the tabs in the right order.
          foreach (string orderId in orderIds)
          {
            if (orderId.Trim() == string.Empty)
              continue;

            m_OrderIds_Cache.Add(orderId);
          }
        }
      }
      return m_OrderIds_Cache;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sorts the tabs according to their display order defined.
    /// </summary>
    /// <param name="tabs">the tabs to sort</param>
    private void SortTabs(List<CxWinTabMetadata> tabs)
    {
      List<CxWinTabMetadata> tabsToSort = new List<CxWinTabMetadata>();
      for (int i = tabs.Count - 1; i >= 0; i--)
      {
        CxWinTabMetadata tab = tabs[i];
        if (tab.DisplayOrder != int.MaxValue)
        {
          tabsToSort.Add(tab);
          tabs.RemoveAt(i);
        }
      }
      tabsToSort.Sort(delegate(CxWinTabMetadata x, CxWinTabMetadata y) { return x.DisplayOrder - y.DisplayOrder; });
      tabs.InsertRange(0, tabsToSort);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a collection of tabs ordered according to the metadata
    /// and customization settings.
    /// </summary>
    protected IList<CxWinTabMetadata> GetTabs()
    {
      if (m_OrderTabs_Cache == null)
      {
        IList<string> orderIds = GetIds();

        if (CxList.IsEmpty2(orderIds))
        {
          m_OrderTabs_Cache = new List<CxWinTabMetadata>(FormMetadata.Tabs);
        }
        else
        {
          m_OrderTabs_Cache = new List<CxWinTabMetadata>();

          // We initialize the complete list of the tabs in the original order.
          IList<CxWinTabMetadata> sourceTabs = new List<CxWinTabMetadata>(FormMetadata.Tabs);

          // Then we fill the original list with the tabs in the right order.
          foreach (string orderId in orderIds)
          {
            CxWinTabMetadata tab = CxWinTabMetadata.FindTabById(sourceTabs, orderId);
            if (tab == null)
              throw new ExNullReferenceException("tab");

            m_OrderTabs_Cache.Add(tab);
          }
        }
      }
      return m_OrderTabs_Cache;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of tab ids in the non-custom (factory) order.
    /// </summary>
    protected IList<string> GetNonCustomIds()
    {
      List<string> result = new List<string>();
      foreach (CxWinTabMetadata tab in GetNonCustomTabs())
      {
        result.Add(tab.Id);
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a list of tabs in the non-custom (factory) order.
    /// </summary>
    protected IList<CxWinTabMetadata> GetNonCustomTabs()
    {
      List<CxWinTabMetadata> result = new List<CxWinTabMetadata>(FormMetadata.Tabs);
      SortTabs(result);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the tab order to its default state (non-custom).
    /// </summary>
    public void ResetToDefaults()
    {
      FormMetadata.CustomTabOrder = null;
      m_OrderIds_Cache = null;
      m_OrderTabs_Cache = null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets the custom order of the tabs according to the given id list.
    /// </summary>
    public void SetCustomOrder(IList<string> ids)
    {
      SetCustomOrder(CxText.ComposeWithSeparator(ids, ","));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets the custom order of the tabs according to the given comma-separated string list.
    /// </summary>
    /// <param name="idString"></param>
    public void SetCustomOrder(string idString)
    {
      FormMetadata.CustomTabOrder = idString;
      m_OrderIds_Cache = null;
      m_OrderTabs_Cache = null;
    }
    //-------------------------------------------------------------------------
  }
}
