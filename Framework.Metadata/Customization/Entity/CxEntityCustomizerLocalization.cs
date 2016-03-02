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
  using System.Collections.Generic;
  using Utils;

  public class CxEntityCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxEntityCustomizer m_Customizer;
    private Dictionary<string, string> m_LanguageSingleCaptionMap = new Dictionary<string, string>();
    private Dictionary<string, string> m_LanguagePluralCaptionMap = new Dictionary<string, string>();
    //-------------------------------------------------------------------------
    public Dictionary<string, string> LanguageSingleCaptionMap
    {
      get { return m_LanguageSingleCaptionMap; }
      set { m_LanguageSingleCaptionMap = value; }
    }
    //-------------------------------------------------------------------------
    public Dictionary<string, string> LanguagePluralCaptionMap
    {
      get { return m_LanguagePluralCaptionMap; }
      set { m_LanguagePluralCaptionMap = value; }
    }
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
    public string NonLocalizedPluralCaption
    {
      get { return Customizer.Metadata.GetInitialProperty("plural_caption", false); }
    }
    //-------------------------------------------------------------------------
    public string CustomPluralCaption
    {
      get
      {
        if (!LanguagePluralCaptionMap.ContainsKey(Customizer.Context.CurrentLanguageCd))
          Customizer.InitializeForLanguage(Customizer.Context.CurrentLanguageCd);
        return LanguagePluralCaptionMap[Customizer.Context.CurrentLanguageCd];
      }
      set { LanguagePluralCaptionMap[Customizer.Context.CurrentLanguageCd] = value; }
    }
    //-------------------------------------------------------------------------
    public string NonLocalizedSingleCaption
    {
      get { return Customizer.Metadata.GetInitialProperty("single_caption", false); }
    }
    //-------------------------------------------------------------------------
    public string CustomSingleCaption
    {
      get
      {
        if (!LanguageSingleCaptionMap.ContainsKey(Customizer.Context.CurrentLanguageCd))
          Customizer.InitializeForLanguage(Customizer.Context.CurrentLanguageCd);
        return LanguageSingleCaptionMap[Customizer.Context.CurrentLanguageCd];
      }
      set { LanguageSingleCaptionMap[Customizer.Context.CurrentLanguageCd] = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">the customizer object the data belongs to</param>
    public CxEntityCustomizerLocalization(CxEntityCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxEntityCustomizerLocalization otherData)
    {
      bool result =
         CxCustomizationUtils.CompareLanguageDictionaries(
           LanguageSingleCaptionMap, otherData.LanguageSingleCaptionMap) &&
         CxCustomizationUtils.CompareLanguageDictionaries(
           LanguagePluralCaptionMap, otherData.LanguagePluralCaptionMap);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clones the data object.
    /// </summary>
    /// <returns>the clone created</returns>
    public CxEntityCustomizerLocalization Clone()
    {
      CxEntityCustomizerLocalization clone = new CxEntityCustomizerLocalization(Customizer);
      clone.LanguageSingleCaptionMap = CxDictionary.CreateDictionary(new List<string>(LanguageSingleCaptionMap.Keys), new List<string>(LanguageSingleCaptionMap.Values));
      clone.LanguagePluralCaptionMap = CxDictionary.CreateDictionary(new List<string>(LanguagePluralCaptionMap.Keys), new List<string>(LanguagePluralCaptionMap.Values));

      return clone;
    }
    //-------------------------------------------------------------------------
    public bool GetIsInitializedForLanguage(string languageCd)
    {
      return (LanguagePluralCaptionMap.ContainsKey(languageCd));
    }
    //-------------------------------------------------------------------------
    public void InitializeForLanguage(string languageCd)
    {
      if (string.IsNullOrEmpty(languageCd) || GetIsInitializedForLanguage(languageCd))
        return;

      LanguageSingleCaptionMap[languageCd] = Customizer.Metadata.Holder.Multilanguage.GetLocalizedValue(
        languageCd,
        CxEntityUsageMetadata.LOCALIZATION_OBJECT_TYPE_CODE,
        "single_caption",
        Customizer.Metadata.Id,
        NonLocalizedSingleCaption) ?? NonLocalizedSingleCaption;

      LanguagePluralCaptionMap[languageCd] =
        Customizer.Metadata.Holder.Multilanguage.GetLocalizedValue(
            languageCd,
            CxEntityUsageMetadata.LOCALIZATION_OBJECT_TYPE_CODE,
            "plural_caption",
            Customizer.Metadata.Id,
            NonLocalizedPluralCaption) ?? NonLocalizedPluralCaption;
    }
    //-------------------------------------------------------------------------
  }
}
