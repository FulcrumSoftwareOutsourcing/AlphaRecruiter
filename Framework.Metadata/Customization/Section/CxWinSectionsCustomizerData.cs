using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinSectionsCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxWinSectionsCustomizer m_Customizer;
    private CxStorableInIdOrderList m_VisibleOrder;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    public CxWinSectionsCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Visible section order.
    /// </summary>
    public CxStorableInIdOrderList VisibleOrder
    {
      get { return m_VisibleOrder; }
      set { m_VisibleOrder = value; }
    }
    //-------------------------------------------------------------------------
    public CxWinSectionsCustomizerData(CxWinSectionsCustomizer customizer)
    {
      Customizer = customizer;
      VisibleOrder = new CxStorableInIdOrderList();
    }
    //-------------------------------------------------------------------------
    public CxWinSectionsCustomizerData Clone()
    {
      CxWinSectionsCustomizerData clone = new CxWinSectionsCustomizerData(Customizer);
      CxList.AddRange(clone.VisibleOrder, VisibleOrder);

      return clone;
    }
    //-------------------------------------------------------------------------
    public void InitializeFromMetadata()
    {
      InitOrder_Visible();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the section visability order from metadata.
    /// </summary>
    private void InitOrder_Visible()
    {
      VisibleOrder.Clear();
      CxWinSectionOrder sectionOrder = Customizer.Metadata.WinSectionOrder;
      foreach (string sectionId in sectionOrder.OrderIds)
      {
        VisibleOrder.Add(Customizer.SectionCustomizers[sectionId]);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds attribute with the specified ID to the list box.
    /// </summary>
    public void AddSectionToVisibleOrder(
      CxWinSectionCustomizer sectionCustomizer)
    {
      if (Customizer.CurrentData.VisibleOrder.Contains(sectionCustomizer))
        return;

      int insertIndex = Customizer.CurrentData.VisibleOrder.Count;

      Customizer.CurrentData.VisibleOrder.Insert(insertIndex, sectionCustomizer);
    }
    //-------------------------------------------------------------------------
    public void RemoveSectionFromVisibleOrder(
      CxWinSectionCustomizer sectionCustomizer)
    {
      if (VisibleOrder.Contains(sectionCustomizer))
        VisibleOrder.Remove(sectionCustomizer);
    }
    //-------------------------------------------------------------------------
    public bool Compare(CxWinSectionsCustomizerData otherData)
    {
      bool result =
        CxList.CompareOrdered(otherData.VisibleOrder, VisibleOrder);

      return result;
    }
    //-------------------------------------------------------------------------
  }
}
