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
  //---------------------------------------------------------------------------
  /// <summary>
  /// Represents a shortened description of the tab metadata.
  /// </summary>
  public class CxTabCustomizer: CxCustomizerBase, IxStorableInIdOrder
  {
    //-------------------------------------------------------------------------
    private CxFormCustomizer m_ParentCustomizer;
    private CxTabCustomizer m_ParentTabCustomizer;
    private CxPanelCustomizerList m_PanelCustomizers;
    private CxTabCustomizerList m_SubTabCustomizers;

    private CxWinTabMetadata m_Metadata;
    private CxTabCustomizerData m_CurrentData;
    private CxTabCustomizerData m_InitialData;
    private CxTabCustomizerLocalization m_CurrentLocalization;
    private CxTabCustomizerLocalization m_InitialLocalization;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxWinTabMetadata Metadata
    {
      get { return m_Metadata; }
      set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Current data snapshot, non-saved data.
    /// </summary>
    public CxTabCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initial data snapshot, the data which is actually applied to the application.
    /// </summary>
    public CxTabCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Logical parent customizer object.
    /// </summary>
    public CxFormCustomizer ParentCustomizer
    {
      get { return m_ParentCustomizer; }
      set { m_ParentCustomizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Logical parent customizer object.
    /// </summary>
    public CxTabCustomizer ParentTabCustomizer
    {
      get { return m_ParentTabCustomizer; }
      set { m_ParentTabCustomizer = value; }
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
    public CxPanelCustomizerList PanelCustomizers
    {
      get { return m_PanelCustomizers; }
      set { m_PanelCustomizers = value; }
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizerList SubTabCustomizers
    {
      get { return m_SubTabCustomizers; }
      set { m_SubTabCustomizers = value; }
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="manager">customization manager</param>
    /// <param name="parentCustomizer">the parent customizer</param>
    /// <param name="tabMetadata">tab metadata</param>
    public CxTabCustomizer(
      CxCustomizationManager manager, CxFormCustomizer parentCustomizer, CxWinTabMetadata tabMetadata)
      : base(manager)
    {
      Metadata = tabMetadata;
      ParentCustomizer = parentCustomizer;
      Context = manager.Context;

      PanelCustomizers = new CxPanelCustomizerList();
      SubTabCustomizers = new CxTabCustomizerList();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="manager">customization manager</param>
    /// <param name="parentCustomizer">the parent customizer</param>
    /// <param name="tabMetadata">tab metadata</param>
    public CxTabCustomizer(
      CxCustomizationManager manager, CxFormCustomizer parentCustomizer, 
      CxTabCustomizer parentTabCustomizer, CxWinTabMetadata tabMetadata)
      : this(manager, parentCustomizer, tabMetadata)
    {
      ParentTabCustomizer = parentTabCustomizer;
    }
    //-------------------------------------------------------------------------
    public void Initialize()
    {
      InitializePanelCustomizers();
      InitializeSubTabCustomizers();
      
      CurrentData = new CxTabCustomizerData(this);
      InitialData = CurrentData.Clone();

      CurrentLocalization = new CxTabCustomizerLocalization(this);
      InitialLocalization = CurrentLocalization.Clone();
    }
    //-------------------------------------------------------------------------
    protected void InitializePanelCustomizers()
    {
      PanelCustomizers.Clear();
      foreach (CxWinPanelMetadata panel in Metadata.Panels)
      {
        CxPanelCustomizer panelCustomizer =
          new CxPanelCustomizer(Manager, this, panel);
        panelCustomizer.Initialize();
        PanelCustomizers.Add(panelCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    protected void InitializeSubTabCustomizers()
    {
      SubTabCustomizers.Clear();
      foreach (CxWinTabMetadata tab in Metadata.ChildTabs)
      {
        CxTabCustomizer tabCustomizer =
          new CxTabCustomizer(Manager, ParentCustomizer, this, tab);
        tabCustomizer.Initialize();
        SubTabCustomizers.Add(tabCustomizer);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies the customizer state to the metadata it belongs to.
    /// </summary>
    public override bool ApplyToMetadata()
    {
      bool isChanged = base.ApplyToMetadata();
      foreach (CxPanelCustomizer panelCustomizer in PanelCustomizers)
      {
        isChanged |= panelCustomizer.ApplyToMetadata();
      }
      return isChanged;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModifiedData()
    {
      bool result = !CurrentData.Compare(InitialData);
      if (!result)
      {
        foreach (CxPanelCustomizer panelCustomizer in PanelCustomizers)
        {
          result |= panelCustomizer.GetIsModifiedData();
          if (result)
            break;
        }
      }

      return result;
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
      foreach (CxPanelCustomizer panelCustomizer in PanelCustomizers)
      {
        panelCustomizer.SaveData(connection);
      }
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
    /// Saves the language captions into the multilanguage subsystem.
    /// </summary>
    /// <param name="connection">connection to be used</param>
    public void SaveLocalization(CxDbConnection connection)
    {
      if (GetIsModifiedLocalization())
      {
        CxMultilanguage multilanguage = Metadata.Holder.Multilanguage;
        if (multilanguage != null)
        {
          foreach (KeyValuePair<CxEntityUsageMetadata, Dictionary<string, string>> pair in CurrentLocalization.LanguageCaptionMap)
          {
            foreach (KeyValuePair<string, string> langCaption in pair.Value)
            {
              string localizationObjectName =
                Context.CurrentEntityUsage.Id + "." +
                ParentCustomizer.Metadata.Id + "." +
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
          InitialLocalization = CurrentLocalization.Clone();
        }
      }

      foreach (CxPanelCustomizer panelCustomizer in PanelCustomizers)
      {
        panelCustomizer.SaveLocalization(connection);
      }
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
      return !string.IsNullOrEmpty(Id) ? Id : base.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the customizer to the metadata object default values.
    /// </summary>
    public override void ResetToDefault()
    {
      base.ResetToDefault();
      foreach (CxPanelCustomizer panelCustomizer in PanelCustomizers)
      {
        panelCustomizer.ResetToDefault();
      }
    }
    //-------------------------------------------------------------------------
  }
}
