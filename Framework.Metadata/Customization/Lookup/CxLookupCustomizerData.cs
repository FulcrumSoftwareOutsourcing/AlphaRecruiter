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
  public class CxLookupCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxLookupCustomizer m_Customizer;
    
    private bool m_IsUsed;
    private bool m_DisplayColor;
    //-------------------------------------------------------------------------
    public CxLookupCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the lookup is used wherever.
    /// </summary>
    public bool IsUsed
    {
      get { return m_IsUsed; }
      set
      {
        if (m_IsUsed != value)
        {
          m_IsUsed = value;
          UpdateWinControlOfReferingFields();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the lookup should display a color associated with each lookup item.
    /// </summary>
    public bool DisplayColor
    {
      get { return m_DisplayColor; }
      set 
      {
        if (m_DisplayColor != value)
        {
          m_DisplayColor = value;
          UpdateWinControlOfReferingFields();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">the customizer current data object belongs to</param>
    public CxLookupCustomizerData(CxLookupCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    public CxLookupCustomizerData Clone()
    {
      CxLookupCustomizerData clone = new CxLookupCustomizerData(Customizer);
      clone.DisplayColor = DisplayColor;
      clone.IsUsed = IsUsed;
      return clone;
    }
    //-------------------------------------------------------------------------
    public void InitializeFromMetadata()
    {
      IsUsed = Customizer.Metadata.WinIsLookupVisible;
      DisplayColor = Customizer.Metadata.DisplayColor;
    }
    //-------------------------------------------------------------------------
    public bool Compare(CxLookupCustomizerData otherData)
    {
      bool result = true;

      if (result)
      {
        result = IsUsed == otherData.IsUsed &&
                 DisplayColor == otherData.DisplayColor;
      }

      return result;
    }
    //-------------------------------------------------------------------------
    private void UpdateWinControlOfReferingFields()
    {
      if (!Customizer.Context.IsInitializing)
      {
        // Here we look thru all the attributes and change their control type according to
        // the value of the DisplayColor property.
        foreach (CxEntityCustomizer entityCustomizer in Customizer.Manager.EntityCustomizerMap.Values)
        {
          foreach (CxAttributeCustomizer attributeCustomizer in entityCustomizer.AttributeCustomizers)
          {
            if (CxText.Equals(attributeCustomizer.CurrentData.RowSourceId, Customizer.Id))
            {
              attributeCustomizer.CurrentData.UpdateWinControlForRowSource();
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}
