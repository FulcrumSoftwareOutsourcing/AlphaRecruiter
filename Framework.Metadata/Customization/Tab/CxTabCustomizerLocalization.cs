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

  public class CxTabCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxTabCustomizer m_Customizer;
    private Dictionary<CxEntityUsageMetadata, Dictionary<string, string>> m_LanguageCaptionMap =
      new Dictionary<CxEntityUsageMetadata, Dictionary<string, string>>();
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxTabCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The map containing the caption of the metadata object per language.
    /// </summary>
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
        bool isDefaultTab = string.Equals(Customizer.Id, CxWinFormMetadata.DEFAULT_TAB_ID,
                                            StringComparison.OrdinalIgnoreCase);

        string customTabCaption = null;
        if (LanguageCaptionMap.ContainsKey(Customizer.Context.CurrentEntityUsage))
        {
          Dictionary<string, string> captions = LanguageCaptionMap[Customizer.Context.CurrentEntityUsage];
          string localized;
          if (captions.TryGetValue(Customizer.Context.CurrentLanguageCd, out localized))
          {
            customTabCaption = localized;
          }
        }
        if (customTabCaption == null)
          customTabCaption = Customizer.Metadata.GetCaption(Customizer.Context.CurrentEntityUsage, Customizer.Context.CurrentLanguageCd);

        if (CxUtils.IsEmpty(customTabCaption) && isDefaultTab)
          return CxWinFormMetadata.DEFAULT_TAB_TEXT;
        else
          return customTabCaption;
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
    public CxTabCustomizerLocalization(CxTabCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxTabCustomizerLocalization otherData)
    {
      bool result = CxCustomizationUtils.CompareLanguageDictionaries(
        LanguageCaptionMap, otherData.LanguageCaptionMap);
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxTabCustomizerLocalization Clone()
    {
      CxTabCustomizerLocalization clone = new CxTabCustomizerLocalization(Customizer);
      clone.LanguageCaptionMap = new Dictionary<CxEntityUsageMetadata, Dictionary<string, string>>(LanguageCaptionMap);
      return clone;
    }
  }
}
