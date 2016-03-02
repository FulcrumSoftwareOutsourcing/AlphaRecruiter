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

using System.Collections.Generic;

using Framework.Db;

namespace Framework.Metadata
{
  using System.Xml;
  using Utils;

  public class CxLookupCustomizer : CxCustomizerBase
  {
    //-------------------------------------------------------------------------
    private CxRowSourceMetadata m_Metadata;

    private CxLookupCustomizerData m_InitialData;
    private CxLookupCustomizerData m_CurrentData;
    private CxLookupCustomizerLocalization m_InitialLocalization;
    private CxLookupCustomizerLocalization m_CurrentLocalization;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Metadata object the customizer belongs to.
    /// </summary>
    public CxRowSourceMetadata Metadata
    {
      get { return m_Metadata; }
      set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customization data in its initial state.
    /// </summary>
    public CxLookupCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customization data in its current state.
    /// </summary>
    public CxLookupCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Id of the customizer.
    /// </summary>
    public override string Id
    {
      get { return Metadata != null ? Metadata.Id : string.Empty; }
    }
    //-------------------------------------------------------------------------
    public CxLookupCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxLookupCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxLookupCustomizer(
      CxCustomizationManager manager, CxRowSourceMetadata rowSource)
      : base(manager)
    {
      Metadata = rowSource;
      Context = manager.Context;

      Initialize();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies the customizer state to the metadata it belongs to.
    /// </summary>
    public override bool ApplyToMetadata()
    {
      bool isChanged = base.ApplyToMetadata();
      if (Metadata.WinIsLookupVisible != CurrentData.IsUsed)
      {
        Metadata.WinIsLookupVisible = CurrentData.IsUsed;
        isChanged |= true;
      }
      if (Metadata.DisplayColor != CurrentData.DisplayColor)
      {
        Metadata.DisplayColor = CurrentData.DisplayColor;
        isChanged |= true;
      }
      if (CurrentLocalization.LanguagePluralCaptionMap.ContainsKey(Metadata.Holder.LanguageCode))
      {
        CxMultilanguage multilanguage = Context.Holder.Multilanguage;
        string localizationObjectName = Metadata.EntityUsage.Id;
        multilanguage.SetLocalizedValueInMemory(
          multilanguage.LanguageCode,
          Metadata.EntityUsage.LocalizationObjectTypeCode,
          "plural_caption",
          localizationObjectName,
          CurrentLocalization.LanguagePluralCaptionMap[multilanguage.LanguageCode]);
      }

      return isChanged;
    }
    //-------------------------------------------------------------------------
    public void Initialize()
    {
      CurrentData = new CxLookupCustomizerData(this);
      CurrentData.InitializeFromMetadata();
      InitialData = CurrentData.Clone();

      CurrentLocalization = new CxLookupCustomizerLocalization(this);
      InitialLocalization = CurrentLocalization.Clone();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      if (Metadata != null)
        return Metadata.Id;
      else
        return base.ToString();
    }
    //-------------------------------------------------------------------------
    public bool GetIsModifiedLocalization()
    {
      return !CurrentLocalization.Compare(InitialLocalization);
    }
    //-------------------------------------------------------------------------
    public bool GetIsModifiedData()
    {
      return !CurrentData.Compare(InitialData);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves customization data to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Save(CxDbConnection connection)
    {
      SaveData(connection);
      SaveLanguageCaptions(connection);
    }
    //-------------------------------------------------------------------------
    private void SaveData(CxDbConnection connection)
    {
      if (GetIsModifiedData())
      {
        DbInsertOrUpdate(connection);
      }
    }
    //-------------------------------------------------------------------------
    private void SaveLanguageCaptions(CxDbConnection connection)
    {
      if (GetIsModifiedLocalization())
      {
        CxMultilanguage multilanguage = Metadata.Holder.Multilanguage;
        if (multilanguage != null)
        {
          CxEntityUsageMetadata entityUsage = Metadata.EntityUsage;
          foreach (KeyValuePair<string, string> langCaption in CurrentLocalization.LanguagePluralCaptionMap)
          {
            string localizationObjectName = entityUsage.Id;
            multilanguage.SetLocalizedValue(
              connection,
              langCaption.Key,
              entityUsage.LocalizationObjectTypeCode,
              "plural_caption",
              localizationObjectName,
              CurrentLocalization.NonLocalizedPluralCaption,
              langCaption.Value);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the customizer to the metadata object default values.
    /// </summary>
    public override void ResetToDefault()
    {
      base.ResetToDefault();

      CurrentData.IsUsed = CxBool.Parse(Metadata.GetInitialProperty("win_is_lookup_visible", false), false);
      bool isRgbColorAttributeIdSet = CxUtils.NotEmpty(Metadata.GetInitialProperty("rgb_color_attr_id"));
      CurrentData.DisplayColor = CxBool.Parse(Metadata.GetInitialProperty("display_color", false), isRgbColorAttributeIdSet);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value provider for a data operation.
    /// </summary>
    /// <returns></returns>
    protected override IxValueProvider GetValueProvider()
    {
      CxHashtable provider = new CxHashtable();

      provider["ApplicationCd"] = Metadata.Holder.ApplicationCode;
      provider["MetadataObjectId"] = Metadata.Id;
      provider["MetadataObjectTypeCd"] = "row_source";

      CxXmlRenderedObject renderedObject = Metadata.RenderToXml(new XmlDocument(), true, null);
      provider["MetadataContent"] = renderedObject.ToString();
      return provider;
    }
    //-------------------------------------------------------------------------
  }
}
