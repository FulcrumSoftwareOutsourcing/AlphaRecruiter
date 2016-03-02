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

namespace Framework.Metadata
{
  public class CxPanelCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxPanelCustomizer m_Customizer;
    private bool m_IsShownAsSeparator;
    private bool m_Visible;
    private bool m_IsCaptionVisible;
    private bool m_IsBorderVisible;

    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxPanelCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the panel should be shown in UI as a separator.
    /// </summary>
    public bool IsShownAsSeparator
    {
      get { return m_IsShownAsSeparator; }
      set { m_IsShownAsSeparator = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Panel identifier.
    /// </summary>
    public string PanelId
    {
      get { return Customizer.Id; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the panel is visible.
    /// </summary>
    public bool Visible
    {
      get { return m_Visible; }
      set { m_Visible = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the border of the panel is visible.
    /// </summary>
    public bool IsBorderVisible
    {
      get { return m_IsBorderVisible; }
      set { m_IsBorderVisible = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the panel caption is visible.
    /// </summary>
    public bool IsCaptionVisible
    {
      get { return m_IsCaptionVisible; }
      set { m_IsCaptionVisible = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">the customizer current data belongs to</param>
    public CxPanelCustomizerData(CxPanelCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxPanelCustomizerData otherData)
    {
      bool result = true;
      if (result)
        result = 
          otherData.IsShownAsSeparator == IsShownAsSeparator &&
          otherData.Visible == Visible &&
          otherData.IsCaptionVisible == IsCaptionVisible &&
          otherData.IsBorderVisible == IsBorderVisible;

      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxPanelCustomizerData Clone()
    {
      CxPanelCustomizerData clone = new CxPanelCustomizerData(Customizer);
      clone.IsShownAsSeparator = IsShownAsSeparator;
      clone.Visible = Visible;
      clone.IsBorderVisible = IsBorderVisible;
      clone.IsCaptionVisible = IsCaptionVisible;
      return clone;
    }
    //-------------------------------------------------------------------------
    public void InitializeFromMetadata()
    {
      IsShownAsSeparator = Customizer.Metadata.IsShownAsSeparator;
      Visible = Customizer.Metadata.Visible;
      IsCaptionVisible = Customizer.Metadata.IsCaptionVisible;
      IsBorderVisible = Customizer.Metadata.IsBorderVisible;
    }
    //-------------------------------------------------------------------------
  }
}
