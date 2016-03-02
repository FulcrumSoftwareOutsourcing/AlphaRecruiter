using System.Collections.Generic;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxWinSectionCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxWinSectionCustomizer m_Customizer;
    private Dictionary<string, string> m_LanguageCaptionMap = new Dictionary<string, string>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    public CxWinSectionCustomizerLocalization(CxWinSectionCustomizer customizer)
    {
      m_Customizer = customizer;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxWinSectionCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    public Dictionary<string, string> LanguageCaptionMap
    {
      get { return m_LanguageCaptionMap; }
      set { m_LanguageCaptionMap = value; }
    }
    //-------------------------------------------------------------------------
    public string NonLocalizedCaption
    {
      get { return Customizer.ParentCustomizer.Metadata.AllItems[Customizer.Id].GetInitialProperty("text"); }
    }
    //-------------------------------------------------------------------------
    public string CustomCaption
    {
      get
      {
        if (!LanguageCaptionMap.ContainsKey(Customizer.Context.CurrentLanguageCd))
          InitializeForLanguage(Customizer.Context.CurrentLanguageCd);
        return LanguageCaptionMap[Customizer.Context.CurrentLanguageCd];
      }
      set { LanguageCaptionMap[Customizer.Context.CurrentLanguageCd] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxWinSectionCustomizerLocalization otherData)
    {
      bool result =
         CxCustomizationUtils.CompareLanguageDictionaries(
           LanguageCaptionMap, otherData.LanguageCaptionMap);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clones the data object.
    /// </summary>
    /// <returns>the clone created</returns>
    public CxWinSectionCustomizerLocalization Clone()
    {
      CxWinSectionCustomizerLocalization clone = new CxWinSectionCustomizerLocalization(Customizer);
      clone.LanguageCaptionMap = CxDictionary.CreateDictionary(new List<string>(LanguageCaptionMap.Keys), new List<string>(LanguageCaptionMap.Values));

      return clone;
    }
    //-------------------------------------------------------------------------
    public bool GetIsInitializedForLanguage(string languageCd)
    {
      return (LanguageCaptionMap.ContainsKey(languageCd));
    }
    //-------------------------------------------------------------------------
    public void InitializeForLanguage(string languageCd)
    {
      if (string.IsNullOrEmpty(languageCd) || GetIsInitializedForLanguage(languageCd))
        return;

      string originalCaption = Customizer.ParentCustomizer.Metadata.AllItems[Customizer.Id].GetNonLocalizedPropertyValue("text");
      LanguageCaptionMap[languageCd] = Customizer.ParentCustomizer.Metadata.Holder.Multilanguage.GetLocalizedValue(
        languageCd,
        Customizer.ParentCustomizer.Metadata.AllItems[Customizer.Id].LocalizationObjectTypeCode,
        "text",
        Customizer.Id,
        originalCaption) ?? originalCaption;
    }
    //-------------------------------------------------------------------------
  }
}
