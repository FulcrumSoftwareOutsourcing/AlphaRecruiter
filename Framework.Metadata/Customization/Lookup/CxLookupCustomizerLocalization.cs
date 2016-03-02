using System;
using System.Collections.Generic;

namespace Framework.Metadata
{
  public class CxLookupCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxLookupCustomizer m_Customizer;
    private Dictionary<string, string> m_LanguagePluralCaptionMap = new Dictionary<string, string>();
    //-------------------------------------------------------------------------
    public CxLookupCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    public Dictionary<string, string> LanguagePluralCaptionMap
    {
      get { return m_LanguagePluralCaptionMap; }
      set { m_LanguagePluralCaptionMap = value; }
    }
    //-------------------------------------------------------------------------
    public string CurrentPluralCaption
    {
      get
      {
        string currentPluralCaption;
        if (LanguagePluralCaptionMap.ContainsKey(Customizer.Context.CurrentLanguageCd))
          currentPluralCaption = LanguagePluralCaptionMap[Customizer.Context.CurrentLanguageCd];
        else
        {
          currentPluralCaption = Customizer.Metadata.Holder.Multilanguage.GetLocalizedValue(
            Customizer.Context.CurrentLanguageCd,
            CxEntityUsageMetadata.LOCALIZATION_OBJECT_TYPE_CODE,
            "plural_caption",
            Customizer.Metadata.EntityUsage.Id,
            NonLocalizedPluralCaption);
          if (currentPluralCaption == null)
            currentPluralCaption = NonLocalizedPluralCaption;
        }
        return LanguagePluralCaptionMap[Customizer.Context.CurrentLanguageCd] = currentPluralCaption;
      }
      set { LanguagePluralCaptionMap[Customizer.Context.CurrentLanguageCd] = value; }
    }
    //-------------------------------------------------------------------------
    public string NonLocalizedPluralCaption
    {
      get { return Customizer.Metadata.EntityUsage.GetInitialProperty("plural_caption", false); }
    }
    //-------------------------------------------------------------------------
    public CxLookupCustomizerLocalization(CxLookupCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    public CxLookupCustomizerLocalization Clone()
    {
      CxLookupCustomizerLocalization clone = new CxLookupCustomizerLocalization(Customizer);
      clone.LanguagePluralCaptionMap = new Dictionary<string, string>(LanguagePluralCaptionMap);
      return clone;
    }
    //-------------------------------------------------------------------------
    public bool Compare(CxLookupCustomizerLocalization otherData)
    {
      bool result = CxCustomizationUtils.CompareLanguageDictionaries(
        LanguagePluralCaptionMap, otherData.LanguagePluralCaptionMap);
      return result;
    }
    //-------------------------------------------------------------------------
  }
}
