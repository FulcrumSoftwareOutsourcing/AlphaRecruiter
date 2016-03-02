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

namespace Framework.Metadata
{
  using Utils;

  public class CxAttributeCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxAttributeCustomizer m_Customizer;
    private Dictionary<string, string> m_LanguageCaptionMap = new Dictionary<string, string>();
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
    /// The map containing the caption of the metadata object per language.
    /// </summary>
    public Dictionary<string, string> LanguageCaptionMap
    {
      get { return m_LanguageCaptionMap; }
      set { m_LanguageCaptionMap = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the caption of the metadata object being customized in the scope
    /// of the current language.
    /// </summary>
    public string CustomCaption
    {
      get
      {
        string localized;
        if (LanguageCaptionMap.TryGetValue(Customizer.Context.CurrentLanguageCd, out localized))
        {
          return localized;
        }
        else
        {
          CxAttributeMetadata metadata = Customizer.ValueMetadata;
          string nonLocalized = metadata.GetNonLocalizedPropertyValue("caption");
          localized = Customizer.Context.Holder.Multilanguage.GetLocalizedValue(
            Customizer.Context.CurrentLanguageCd,
            CxAttributeUsageMetadata.OBJECT_TYPE_ATTRIBUTE_USAGE,
            "caption",
            Customizer.ParentCustomizer.Id + "." + Customizer.Id,
            nonLocalized);
          return CxUtils.Nvl(localized, nonLocalized);
        }
      }
      set { LanguageCaptionMap[Customizer.Context.CurrentLanguageCd] = value; }
    }
    //-------------------------------------------------------------------------
    public string NonLocalizedCaption
    {
      get { return CxUtils.Nvl(Customizer.ValueMetadata.GetInitialProperty("caption"), Customizer.Id); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">customizer object the data belongs to</param>
    public CxAttributeCustomizerLocalization(CxAttributeCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxAttributeCustomizerLocalization otherData)
    {
      bool result = CxCustomizationUtils.CompareLanguageDictionaries(
        LanguageCaptionMap, otherData.LanguageCaptionMap);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxAttributeCustomizerLocalization Clone()
    {
      CxAttributeCustomizerLocalization clone = new CxAttributeCustomizerLocalization(Customizer);
      clone.LanguageCaptionMap = new Dictionary<string, string>(LanguageCaptionMap);
      return clone;
    }
    //-------------------------------------------------------------------------
  }
}
