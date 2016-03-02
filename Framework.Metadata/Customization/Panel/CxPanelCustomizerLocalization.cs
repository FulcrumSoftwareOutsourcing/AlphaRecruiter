using System;
using System.Collections.Generic;

namespace Framework.Metadata
{
  using Utils;

  public class CxPanelCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxPanelCustomizer m_Customizer;
    private Dictionary<CxEntityUsageMetadata, Dictionary<string, string>> m_LanguageCaptionMap = new Dictionary<CxEntityUsageMetadata, Dictionary<string, string>>();
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
    public Dictionary<CxEntityUsageMetadata, Dictionary<string, string>> LanguageCaptionMap
    {
      get { return m_LanguageCaptionMap; }
      set { m_LanguageCaptionMap = value; }
    }
    //-------------------------------------------------------------------------
    public string NonLocalizedCaption
    {
      get { return Customizer.Metadata.GetNonLocalizedPropertyValue("text"); }
    }
    //-------------------------------------------------------------------------
    public string CustomCaption
    {
      get
      {
        bool isDefaultPanel = string.Equals(Customizer.Id, CxWinTabMetadata.DEFAULT_PANEL_ID,
                                            StringComparison.OrdinalIgnoreCase);

        string customPanelCaption = null;
        if (LanguageCaptionMap.ContainsKey(Customizer.Context.CurrentEntityUsage))
        {
          Dictionary<string, string> captions = LanguageCaptionMap[Customizer.Context.CurrentEntityUsage];
          string localized;
          if (captions.TryGetValue(Customizer.Context.CurrentLanguageCd, out localized))
          {
            customPanelCaption = localized;
          }
        }
        if (customPanelCaption == null)
          customPanelCaption = Customizer.Metadata.GetCaption(Customizer.Context.CurrentEntityUsage, Customizer.Context.CurrentLanguageCd);

        if (CxUtils.IsEmpty(customPanelCaption) && isDefaultPanel)
          return CxWinTabMetadata.DEFAULT_PANEL_TEXT;
        else
          return customPanelCaption;
      }
      set
      {
        Dictionary<string, string> captions;
        CxEntityUsageMetadata currentEntityUsage = Customizer.Context.CurrentEntityUsage;
        if (LanguageCaptionMap.ContainsKey(currentEntityUsage))
          captions = LanguageCaptionMap[currentEntityUsage];
        else
          LanguageCaptionMap[currentEntityUsage] = captions = new Dictionary<string, string>();
        captions[Customizer.Context.CurrentLanguageCd] = value;
      }
    }
    //-------------------------------------------------------------------------
    public CxPanelCustomizerLocalization(CxPanelCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxPanelCustomizerLocalization otherData)
    {
      bool result = CxCustomizationUtils.CompareLanguageDictionaries(
        LanguageCaptionMap, otherData.LanguageCaptionMap);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxPanelCustomizerLocalization Clone()
    {
      CxPanelCustomizerLocalization clone = new CxPanelCustomizerLocalization(Customizer);
      clone.LanguageCaptionMap = new Dictionary<CxEntityUsageMetadata, Dictionary<string, string>>(LanguageCaptionMap);
      return clone;
    }
    //-------------------------------------------------------------------------

  }
}
