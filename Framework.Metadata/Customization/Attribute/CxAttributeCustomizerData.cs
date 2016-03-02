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
  public class CxAttributeCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxAttributeCustomizer m_Customizer;
    private string m_WinControlPlacement = string.Empty;
    private string m_WinControl = string.Empty;
    private bool m_IsShownOnAdvancedFilterPanel = false;
    private string m_RowSourceId;
    private bool m_IsRequired;
    private bool m_IsNewLine;
    private string m_DefaultValue;
    private bool m_IsCustomizable;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxAttributeCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Represents the panel the attribute should be placed on.
    /// </summary>
    public string WinControlPlacement
    {
      get { return m_WinControlPlacement; }
      set { m_WinControlPlacement = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Represents the win-control belonging to the current attribute.
    /// </summary>
    public string WinControl
    {
      get { return m_WinControl; }
      set { m_WinControl = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the attribute is shown on the advanced filter panel.
    /// </summary>
    public bool IsShownOnAdvancedFilterPanel
    {
      get { return m_IsShownOnAdvancedFilterPanel; }
      set { m_IsShownOnAdvancedFilterPanel = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row source id.
    /// </summary>
    public string RowSourceId
    {
      get { return m_RowSourceId; }
      set 
      { 
        if (m_RowSourceId == value)
          return;
        m_RowSourceId = value;
        UpdateWinControlForRowSource();
      }
    }
    //-------------------------------------------------------------------------
    public bool IsNewLine
    {
      get { return m_IsNewLine; }
      set { m_IsNewLine = value; }
    }
    //-------------------------------------------------------------------------
    public bool IsRequired
    {
      get { return m_IsRequired; }
      set { m_IsRequired = value; }
    }
    //-------------------------------------------------------------------------
    public string DefaultValue
    {
      get { return m_DefaultValue; }
      set { m_DefaultValue = value; }
    }
    //-------------------------------------------------------------------------
    public bool IsCustomizable
    {
      get { return m_IsCustomizable; }
      set { m_IsCustomizable = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">customizer object the data belongs to</param>
    public CxAttributeCustomizerData(CxAttributeCustomizer customizer)
    {
      Customizer = customizer;
      
      CxAttributeMetadata metadata = Customizer.ValueMetadata;

      IsShownOnAdvancedFilterPanel = metadata.FilterAdvanced;
      WinControlPlacement = metadata.WinControlPlacement;
      WinControl = metadata.WinControl;
      RowSourceId = metadata.RowSourceId;
      IsRequired = !metadata.Nullable;
      IsNewLine = metadata.IsPlacedOnNewLine;
      DefaultValue = metadata.Default;
      IsCustomizable = metadata.IsCustomizable;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxAttributeCustomizerData otherData)
    {
      bool result =
        IsShownOnAdvancedFilterPanel == otherData.IsShownOnAdvancedFilterPanel &&
        IsRequired == otherData.IsRequired &&
        IsNewLine == otherData.IsNewLine &&
        IsCustomizable == otherData.IsCustomizable &&
        CxText.Equals(WinControlPlacement, otherData.WinControlPlacement) &&
        CxText.Equals(WinControl, otherData.WinControl) &&
        CxText.Equals(DefaultValue, otherData.DefaultValue) &&
        CxText.Equals(RowSourceId, otherData.RowSourceId);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxAttributeCustomizerData Clone()
    {
      CxAttributeCustomizerData clone = new CxAttributeCustomizerData(Customizer);
      clone.WinControlPlacement = WinControlPlacement;
      clone.WinControl = WinControl;
      clone.IsShownOnAdvancedFilterPanel = IsShownOnAdvancedFilterPanel;
      clone.RowSourceId = RowSourceId;
      clone.IsRequired = IsRequired;
      clone.IsNewLine = IsNewLine;
      clone.IsCustomizable = IsCustomizable;
      clone.DefaultValue = DefaultValue;
      return clone;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates the WinControl of the attribute accordingly to the row-source set.
    /// Is needed just for some lookup fields which win-control should be changable on the fly.
    /// </summary>
    public void UpdateWinControlForRowSource()
    {
      if (!Customizer.Context.IsInitializing)
      {
        if (!string.IsNullOrEmpty(RowSourceId) && (string.IsNullOrEmpty(WinControl) || WinControl == CxWinControlNames.WIN_CONTROL_DROPDOWN || WinControl == CxWinControlNames.WIN_CONTROL_DROPDOWNIMAGE) )
        {
          CxRowSourceMetadata rowSource = Customizer.Context.Holder.RowSources[RowSourceId];
          if (Customizer.Manager.LookupCustomizerMap.ContainsKey(rowSource))
          {
            CxLookupCustomizer lookupCustomizer = Customizer.Manager.LookupCustomizerMap[rowSource];
            WinControl =
              lookupCustomizer.CurrentData.DisplayColor
                ? CxWinControlNames.WIN_CONTROL_DROPDOWNIMAGE
                : CxWinControlNames.WIN_CONTROL_DROPDOWN;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}
