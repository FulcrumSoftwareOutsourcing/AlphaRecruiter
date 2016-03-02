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
  public class CxEntityCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxEntityCustomizer m_Customizer;
    private CxStorableInIdOrderList m_GridVisibleOrder;
    private CxStorableInIdOrderList m_QueryOrder;
    private CxStorableInIdOrderList m_EditOrder;
    private CxStorableInIdOrderList m_FilterOrder;

    private bool m_Visible;
    private bool m_VisibleToAdministrator;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxEntityCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates if the entity usage is visible.
    /// </summary>
    public bool Visible
    { get { return m_Visible; } set { m_Visible = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates if the entity usage is visible to administrator.
    /// </summary>
    public bool VisibleToAdministrator
    { get { return m_VisibleToAdministrator; } set { m_VisibleToAdministrator = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Grid visible attributes order.
    /// </summary>
    public CxStorableInIdOrderList GridVisibleOrder
    { 
      get { return m_GridVisibleOrder; }
      set { m_GridVisibleOrder = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Query attributes order.
    /// </summary>
    public CxStorableInIdOrderList QueryOrder
    {
      get { return m_QueryOrder; }
      set { m_QueryOrder = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Editable attributes order.
    /// </summary>
    public CxStorableInIdOrderList EditOrder
    { 
      get { return m_EditOrder; }
      set { m_EditOrder = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Filterable attributes order.
    /// </summary>
    public CxStorableInIdOrderList FilterOrder
    { 
      get { return m_FilterOrder; }
      set { m_FilterOrder = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The dictionary of order for each type of secondary tab orders.
    /// </summary>
    public Dictionary<NxChildEntityUsageOrderType, CxStorableInIdOrderList> SecondaryTabsOrder { get; protected set; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">the customizer object the data belongs to</param>
    public CxEntityCustomizerData(CxEntityCustomizer customizer)
    {
      Customizer = customizer;

      GridVisibleOrder = new CxStorableInIdOrderList();
      EditOrder = new CxStorableInIdOrderList();
      FilterOrder = new CxStorableInIdOrderList();
      QueryOrder = new CxStorableInIdOrderList();

      SecondaryTabsOrder = new Dictionary<NxChildEntityUsageOrderType, CxStorableInIdOrderList>();
      SecondaryTabsOrder[NxChildEntityUsageOrderType.InList] = new CxStorableInIdOrderList();
      SecondaryTabsOrder[NxChildEntityUsageOrderType.InView] = new CxStorableInIdOrderList();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the Secondary tabs order of the given order type.
    /// </summary>
    public CxStorableInIdOrderList GetSecondaryTabsOrder(NxChildEntityUsageOrderType orderType)
    {
      CxStorableInIdOrderList result;
      if (!SecondaryTabsOrder.TryGetValue(orderType, out result))
        throw new ExException(string.Format("The tab order type {0} is not recognized", orderType));
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the customizer.
    /// </summary>
    public void InitializeFromMetadata()
    {
      InitOrder_Grid();
      InitOrder_Edit();
      InitOrder_Filter();
      InitOrder_Query();
      InitOrder_SecondaryTabs(NxChildEntityUsageOrderType.InList);
      InitOrder_SecondaryTabs(NxChildEntityUsageOrderType.InView);
      Visible = Customizer.Metadata.Visible;
      VisibleToAdministrator = !Customizer.Metadata.IsHiddenForUser;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxEntityCustomizerData otherData)
    {
      bool result =
        CxList.CompareOrdered(otherData.GridVisibleOrder, GridVisibleOrder) &&
        CxList.CompareOrdered(otherData.QueryOrder, QueryOrder) &&
        CxList.CompareOrdered(otherData.EditOrder, EditOrder) &&
        CxList.CompareOrdered(otherData.FilterOrder, FilterOrder) &&
        CxList.CompareOrdered(otherData.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InList), GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InList)) &&
        CxList.CompareOrdered(otherData.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InView), GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InView)) &&
        otherData.Visible == Visible &&
        otherData.VisibleToAdministrator == VisibleToAdministrator;
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clones the data object.
    /// </summary>
    /// <returns>the clone created</returns>
    public CxEntityCustomizerData Clone()
    {
      CxEntityCustomizerData clone = new CxEntityCustomizerData(Customizer);
      CxList.AddRange(clone.GridVisibleOrder, GridVisibleOrder);
      CxList.AddRange(clone.EditOrder, EditOrder);
      CxList.AddRange(clone.FilterOrder, FilterOrder);
      CxList.AddRange(clone.QueryOrder, QueryOrder);
      CxList.AddRange(clone.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InList), GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InList));
      CxList.AddRange(clone.GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InView), GetSecondaryTabsOrder(NxChildEntityUsageOrderType.InView));
      clone.Visible = Visible;
      clone.VisibleToAdministrator = VisibleToAdministrator;

      return clone;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the grid attribute order from metadata.
    /// </summary>
    private void InitOrder_Grid()
    {
      GridVisibleOrder.Clear();
      CxAttributeOrder attributeOrder = Customizer.Metadata.GetAttributeOrder(NxAttributeContext.GridVisible);
      foreach (string attributeId in attributeOrder.OrderIds)
      {
        CxAttributeCustomizer attributeCustomizer = Customizer.AttributeCustomizers.FindByValueOrTextId(attributeId);
        if (attributeCustomizer == null)
          continue;
        GridVisibleOrder.Add(attributeCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the edit attribute order from metadata.
    /// </summary>
    private void InitOrder_Edit()
    {
      EditOrder.Clear();
      CxAttributeOrder attributeOrder = Customizer.Metadata.GetAttributeOrder(NxAttributeContext.Edit);
      foreach (string attributeId in attributeOrder.OrderIds)
      {
        CxAttributeCustomizer attributeCustomizer = Customizer.AttributeCustomizers.FindByValueOrTextId(attributeId);
        if (attributeCustomizer == null)
          continue;
        EditOrder.Add(attributeCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the filter attribute order from metadata.
    /// </summary>
    private void InitOrder_Filter()
    {
      FilterOrder.Clear();
      CxAttributeOrder attributeOrder = Customizer.Metadata.GetAttributeOrder(NxAttributeContext.Filter);
      foreach (string attributeId in attributeOrder.OrderIds)
      {
        CxAttributeCustomizer attributeCustomizer = Customizer.AttributeCustomizers.FindByValueOrTextId(attributeId);
        if (attributeCustomizer == null)
          continue;
        FilterOrder.Add(attributeCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the query attribute order from metadata.
    /// </summary>
    private void InitOrder_Query()
    {
      QueryOrder.Clear();
      CxAttributeOrder attributeOrder = Customizer.Metadata.GetAttributeOrder(NxAttributeContext.Queryable);
      foreach (string attributeId in attributeOrder.OrderIds)
      {
        CxAttributeCustomizer attributeCustomizer = Customizer.AttributeCustomizers.FindByValueOrTextId(attributeId);
        if (attributeCustomizer == null)
          continue;
        QueryOrder.Add(attributeCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the secondary tabs order from metadata.
    /// </summary>
    public void InitOrder_SecondaryTabs(NxChildEntityUsageOrderType orderType)
    {
      CxStorableInIdOrderList tabOrder = GetSecondaryTabsOrder(orderType);
      tabOrder.Clear();
      CxChildEntityUsageOrder entityUsageOrder = Customizer.Metadata.GetChildEntityUsageOrder(orderType);
      foreach (string id in entityUsageOrder.OrderIds)
      {
        CxChildEntityCustomizer customizer = Customizer.ChildEntityCustomizers.Find(delegate(CxChildEntityCustomizer x) { return x.ChildMetadata.Id == id; });
        if (customizer == null)
          CxLogger.SafeWrite(string.Format("Child entity usage customizer <{0}> has not been found", id));
        else
          tabOrder.Add(customizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute with the specified ID to the list box.
    /// </summary>
    public void AddAttributeToGridOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (GridVisibleOrder.Contains(attributeCustomizer))
        return;

      int insertIndex = GridVisibleOrder.Count;

      GridVisibleOrder.Insert(insertIndex, attributeCustomizer);
      EnforceCustomCaptionSetting(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    public void RemoveAttributeFromGridOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (GridVisibleOrder.Contains(attributeCustomizer))
        GridVisibleOrder.Remove(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute with the specified ID to the list box.
    /// </summary>
    public void AddAttributeToEditOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (EditOrder.Contains(attributeCustomizer))
        return;

      string winControlPlacement = attributeCustomizer.CurrentData.WinControlPlacement;
      
      CxPanelCustomizer panelCustomizer = null;
      if (!string.IsNullOrEmpty(winControlPlacement))
      {
        panelCustomizer = Customizer.FormCustomizer.FindPanelById(winControlPlacement);
      }
      if (panelCustomizer == null || !panelCustomizer.CurrentData.Visible)
      {
        if (Customizer.FormCustomizer.CurrentData.TabOrder.Count == 0)
          throw new ExException("The form contains no tabs");
        CxTabCustomizer tabCustomizer = Customizer.FormCustomizer.TabCustomizers.FindById(Customizer.FormCustomizer.CurrentData.TabOrder[0].Id);
        if (tabCustomizer.PanelCustomizers.Count == 0)
          throw new ExException("The tab contains no panels");
        panelCustomizer = tabCustomizer.PanelCustomizers[0];
      }
      if (!CxText.Equals(attributeCustomizer.CurrentData.WinControlPlacement, panelCustomizer.Id))
        attributeCustomizer.CurrentData.WinControlPlacement = panelCustomizer.Id;

      // Here it would be better to obtain the precise index to insert the item to
      // but temporarily it will work this way.
      int insertIndex = EditOrder.Count;

      EditOrder.Insert(insertIndex, attributeCustomizer);
      EnforceCustomCaptionSetting(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    public void RemoveAttributeFromEditOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (EditOrder.Contains(attributeCustomizer))
        EditOrder.Remove(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute with the specified ID to the list box.
    /// </summary>
    public void AddAttributeToFilterOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (FilterOrder.Contains(attributeCustomizer))
        return;

      int insertIndex = FilterOrder.Count;

      FilterOrder.Insert(insertIndex, attributeCustomizer);
      EnforceCustomCaptionSetting(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    public void RemoveAttributeFromFilterOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (FilterOrder.Contains(attributeCustomizer))
        FilterOrder.Remove(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute with the specified ID to the list box.
    /// </summary>
    public void AddAttributeToQueryOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (QueryOrder.Contains(attributeCustomizer))
        return;

      int insertIndex = QueryOrder.Count;

      QueryOrder.Insert(insertIndex, attributeCustomizer);
      EnforceCustomCaptionSetting(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    public void RemoveAttributeFromQueryOrder(
      CxAttributeCustomizer attributeCustomizer)
    {
      if (QueryOrder.Contains(attributeCustomizer))
        QueryOrder.Remove(attributeCustomizer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute with the specified ID to the list box.
    /// </summary>
    public void AddTabToSecondaryTabsOrder(
      NxChildEntityUsageOrderType orderType,
      CxChildEntityCustomizer childEntityCustomizer)
    {
      if (childEntityCustomizer == null)
        throw new ArgumentNullException("childEntityCustomizer");
      CxStorableInIdOrderList orderList = GetSecondaryTabsOrder(orderType);
      if (orderList.Contains(childEntityCustomizer))
        return;

      int insertIndex = orderList.Count;
      orderList.Insert(insertIndex, childEntityCustomizer);
    }
    //-------------------------------------------------------------------------
    public void RemoveTabFromSecondaryTabsOrder(
      NxChildEntityUsageOrderType orderType,
      CxChildEntityCustomizer childEntityCustomizer)
    {
      CxStorableInIdOrderList orderList = GetSecondaryTabsOrder(orderType);
      if (orderList.Contains(childEntityCustomizer))
        orderList.Remove(childEntityCustomizer);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Just sets the Custom Caption to the 
    /// </summary>
    private void EnforceCustomCaptionSetting(CxAttributeCustomizer attributeCustomizer)
    {
      attributeCustomizer.CurrentLocalization.CustomCaption = CxUtils.Nvl(attributeCustomizer.CurrentLocalization.CustomCaption, attributeCustomizer.Id);
    }
    //-------------------------------------------------------------------------
  }
}
