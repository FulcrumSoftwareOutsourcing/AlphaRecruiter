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

namespace Framework.Metadata
{
  public class CxPanelCustomizer: CxCustomizerBase
  {
    //-------------------------------------------------------------------------
    private CxTabCustomizer m_ParentCustomizer;
    private CxWinPanelMetadata m_Metadata;
    private CxPanelCustomizerData m_CurrentData;
    private CxPanelCustomizerData m_InitialData;
    private CxPanelCustomizerLocalization m_CurrentLocalization;
    private CxPanelCustomizerLocalization m_InitialLocalization;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parent customizer.
    /// </summary>
    public CxTabCustomizer ParentCustomizer
    {
      get { return m_ParentCustomizer; }
      set { m_ParentCustomizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxWinPanelMetadata Metadata
    {
      get { return m_Metadata; }
      set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The current (customized) data snapshot.
    /// </summary>
    public CxPanelCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The initial (non-customized) data snapshot.
    /// </summary>
    public CxPanelCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Identifier of the customizer.
    /// </summary>
    public override string Id
    {
      get { return Metadata.Id; }
    }
    //-------------------------------------------------------------------------
    public CxPanelCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxPanelCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="manager">customization manager</param>
    /// <param name="parentCustomizer">parent customizer</param>
    /// <param name="metadata">the metadata object current customizer belongs to</param>
    public CxPanelCustomizer(
      CxCustomizationManager manager,
      CxTabCustomizer parentCustomizer, CxWinPanelMetadata metadata)
      : base(manager)
    {
      ParentCustomizer = parentCustomizer;
      Metadata = metadata;
      Context = manager.Context;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the customizer.
    /// </summary>
    public void Initialize()
    {
      CurrentData = new CxPanelCustomizerData(this);
      CurrentData.InitializeFromMetadata();
      
      InitialData = CurrentData.Clone();

      CurrentLocalization = new CxPanelCustomizerLocalization(this);
      InitialLocalization = CurrentLocalization.Clone();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies the customizer state to the metadata it belongs to.
    /// </summary>
    public override bool ApplyToMetadata()
    {
      bool isChanged = base.ApplyToMetadata();
      if (Metadata.IsShownAsSeparator != CurrentData.IsShownAsSeparator)
      {
        Metadata.IsShownAsSeparator = CurrentData.IsShownAsSeparator;
        isChanged |= true;
      }
      if (Metadata.IsBorderVisible != CurrentData.IsBorderVisible)
      {
        Metadata.IsBorderVisible = CurrentData.IsBorderVisible;
        isChanged |= true;
      }
      if (Metadata.IsCaptionVisible != CurrentData.IsCaptionVisible)
      {
        Metadata.IsCaptionVisible = CurrentData.IsCaptionVisible;
        isChanged |= true;
      }
      if (Metadata.Visible != CurrentData.Visible)
      {
        Metadata.Visible = CurrentData.Visible;
        isChanged |= true;
      }
      return isChanged;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the customizer to the metadata object default values.
    /// </summary>
    public override void ResetToDefault()
    {
      base.ResetToDefault();
      CurrentData.IsShownAsSeparator = CxBool.Parse(Metadata.GetInitialProperty("is_shown_as_separator", false), false);
      CurrentData.IsBorderVisible = CxBool.Parse(Metadata.GetInitialProperty("border", false), true);
      CurrentData.IsCaptionVisible = CxBool.Parse(Metadata.GetInitialProperty("is_caption_visible", false), false);
      CurrentData.Visible = CxBool.Parse(Metadata.GetInitialProperty("visible", false), true);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModifiedData()
    {
      return !CurrentData.Compare(InitialData);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves the changes done to the customizer into the database store.
    /// </summary>
    /// <param name="connection">connection to be used</param>
    public void SaveData(CxDbConnection connection)
    {
      if (GetIsModifiedData())
      {
        InitialData = CurrentData.Clone();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves the language captions into the multilanguage subsystem.
    /// </summary>
    /// <param name="connection">connection to be used</param>
    public void SaveLocalization(CxDbConnection connection)
    {
      CxMultilanguage multilanguage = Metadata.Holder.Multilanguage;
      if (multilanguage != null)
      {
        foreach (KeyValuePair<CxEntityUsageMetadata, Dictionary<string,string>> pair in CurrentLocalization.LanguageCaptionMap)
        {
          foreach (KeyValuePair<string, string> langCaption in pair.Value)
          {
            string localizationObjectName =
              Context.CurrentEntityUsage.Id + "." +
              ParentCustomizer.ParentCustomizer.Metadata.Id + "." +
              Metadata.Id;
            multilanguage.SetLocalizedValue(
              connection,
              langCaption.Key,
              Metadata.LocalizationObjectTypeCode,
              "text",
              localizationObjectName,
              CurrentLocalization.NonLocalizedCaption,
              langCaption.Value);
          }
        }
      }
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
  }
}
