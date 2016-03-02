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

using Framework.Utils;

namespace Framework.Metadata
{
  public class CxFormCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxFormCustomizer m_Customizer;
    private CxStorableInIdOrderList m_TabOrder;
    //-------------------------------------------------------------------------
    public CxFormCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Tab order.
    /// </summary>
    public CxStorableInIdOrderList TabOrder
    {
      get { return m_TabOrder; }
      set { m_TabOrder = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizerData(CxFormCustomizer customizer)
    {
      Customizer = customizer;
      
      TabOrder = new CxStorableInIdOrderList();
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizer FindTabInTabOrderById(string id)
    {
      foreach (CxTabCustomizer tabCustomizer in TabOrder)
      {
        if (tabCustomizer.Id == id)
          return tabCustomizer;
        foreach (CxTabCustomizer subtabCustomizer in tabCustomizer.SubTabCustomizers)
        {
          if (subtabCustomizer.Id == id)
            return subtabCustomizer;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the customizer.
    /// </summary>
    public void InitializeFromMetadata()
    {
      InitOrder_Tabs();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initilizes the tab order for the customizer.
    /// </summary>
    protected void InitOrder_Tabs()
    {
      TabOrder.Clear();
      CxTabCustomizerList tabIds = Customizer.TabCustomizers.GetSublistBy(Customizer.Metadata.GetTabOrderManager().Ids);
      CxList.AddRange(TabOrder, tabIds);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clones the data object.
    /// </summary>
    /// <returns>the clone created</returns>
    public CxFormCustomizerData Clone()
    {
      CxFormCustomizerData clone = new CxFormCustomizerData(Customizer);

      CxList.AddRange(clone.TabOrder, TabOrder);

      return clone;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxFormCustomizerData otherData)
    {
      bool result = CxList.CompareOrdered(otherData.TabOrder, TabOrder);
      return result;
    }
    //-------------------------------------------------------------------------
  }
}
