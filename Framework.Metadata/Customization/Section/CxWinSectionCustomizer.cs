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
using Framework.Utils;
using System.Xml;

namespace Framework.Metadata
{
  public class CxWinSectionCustomizer : CxCustomizerBase, IxStorableInIdOrder
  {
    //-------------------------------------------------------------------------
    private CxWinSectionMetadata m_Metadata;
    private CxWinSectionsCustomizer m_ParentCustomizer;

    private CxWinSectionCustomizerData m_InitialData;
    private CxWinSectionCustomizerData m_CurrentData;
    private CxWinSectionCustomizerLocalization m_CurrentLocalization;
    private CxWinSectionCustomizerLocalization m_InitialLocalization;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxWinSectionMetadata Metadata
    {
      get { return m_Metadata; }
      set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    public CxWinSectionCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    public CxWinSectionCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parent customizer.
    /// </summary>
    public CxWinSectionsCustomizer ParentCustomizer
    {
      get { return m_ParentCustomizer; }
      set { m_ParentCustomizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Identifier of the customizer.
    /// </summary>
    public override string Id
    {
      get { return Metadata != null ? Metadata.Id : string.Empty; }
    }
    //-------------------------------------------------------------------------
    public CxWinSectionCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxWinSectionCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="parentCustomizer"></param>
    /// <param name="sectionId"></param>
    public CxWinSectionCustomizer(
      CxWinSectionsCustomizer parentCustomizer, CxWinSectionMetadata metadata)
      : base(parentCustomizer.Manager)
    {
      ParentCustomizer = parentCustomizer;
      Context = parentCustomizer.Context;

      Metadata = metadata;

      CurrentData = new CxWinSectionCustomizerData(this);
      CurrentData.InitializeFromMetadata();
      InitialData = CurrentData.Clone();

      CurrentLocalization = new CxWinSectionCustomizerLocalization(this);
      InitialLocalization = new CxWinSectionCustomizerLocalization(this);
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
      return Metadata != null ? Metadata.ToString() : base.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModifiedLocalization()
    {
      return !CurrentLocalization.Compare(InitialLocalization);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies latest version of the customization to the current metadata.
    /// </summary>
    public override bool ApplyToMetadata()
    {
      base.ApplyToMetadata();

      if (Context.Holder.IsDevelopmentMode)
        Metadata.IsHiddenForUser = !CurrentData.VisibleToAdministrator;

      if (CurrentLocalization.LanguageCaptionMap.ContainsKey(ParentCustomizer.Metadata.Holder.LanguageCode))
        ParentCustomizer.Metadata.AllItems[Id].Text = CurrentLocalization.LanguageCaptionMap[ParentCustomizer.Metadata.Holder.LanguageCode];

      return true;
    }
    //-------------------------------------------------------------------------
    public void SaveData(CxDbConnection connection)
    {
      if (GetIsModifiedData())
      {
        DbInsertOrUpdate(connection);
        
        InitialData = CurrentData.Clone();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value provider for a data operation.
    /// </summary>
    protected override IxValueProvider GetValueProvider()
    {
      CxHashtable provider = new CxHashtable();

      provider["ApplicationCd"] = Metadata.Holder.ApplicationCode;
      provider["MetadataObjectId"] = Metadata.Id;
      provider["MetadataObjectTypeCd"] = "win_section";

      CxXmlRenderedObject renderedObject = Metadata.RenderToXml(new XmlDocument(), true, null);
      provider["MetadataContent"] = renderedObject.ToString();
      return provider;
    }
    //-------------------------------------------------------------------------
    public void SaveLocalization(CxDbConnection connection)
    {
      if (GetIsModifiedLocalization())
      {
        CxMultilanguage multilanguage = Context.Holder.Multilanguage;
        if (multilanguage != null)
        {
          foreach (KeyValuePair<string, string> langCaption in CurrentLocalization.LanguageCaptionMap)
          {
            multilanguage.SetLocalizedValue(
              connection,
              langCaption.Key,
              ParentCustomizer.Metadata.AllItems[Id].LocalizationObjectTypeCode,
              "text",
              Id,
              CurrentLocalization.NonLocalizedCaption,
              langCaption.Value);
          }
        }

        InitialLocalization = CurrentLocalization.Clone();
      }
    }
    //-------------------------------------------------------------------------
    public bool GetIsModifiedData()
    {
      return !CurrentData.Compare(InitialData);
    }
    //-------------------------------------------------------------------------
  }
}
