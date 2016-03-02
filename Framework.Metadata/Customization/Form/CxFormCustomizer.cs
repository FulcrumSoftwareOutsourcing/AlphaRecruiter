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

using System.Xml;

using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxFormCustomizer: CxCustomizerBase
  {
    //-------------------------------------------------------------------------
    private CxWinFormMetadata m_Metadata;
    private CxTabCustomizerList m_TabCustomizers;

    private CxFormCustomizerData m_CurrentData;
    private CxFormCustomizerData m_InitialData;

    private CxFormCustomizerLocalization m_CurrentLocalization;
    private CxFormCustomizerLocalization m_InitialLocalization;
    //-------------------------------------------------------------------------
    public override string Id
    {
      get { return Metadata != null ? Metadata.Id : string.Empty; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxWinFormMetadata Metadata
    {
      get { return m_Metadata; }
      set { m_Metadata = value; }
    }
    //-------------------------------------------------------------------------
    public CxTabCustomizerList TabCustomizers
    {
      get { return m_TabCustomizers; }
      set { m_TabCustomizers = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="manager">customization manager object</param>
    /// <param name="metadata">the metadata object customizer belongs to</param>
    public CxFormCustomizer(CxCustomizationManager manager, CxWinFormMetadata metadata)
      : base(manager)
    {
      Metadata = metadata;
      Context = manager.Context;

      TabCustomizers = new CxTabCustomizerList();

      Initialize();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the customizer object.
    /// </summary>
    public void Initialize()
    {
      InitializeTabCustomizers();

      // Initializing the Current Data
      CurrentData = new CxFormCustomizerData(this);
      CurrentData.InitializeFromMetadata();
      // Setting the current data as the initial one.
      InitialData = CurrentData.Clone();

      CurrentLocalization = new CxFormCustomizerLocalization(this);
      InitialLocalization = CurrentLocalization.Clone();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes the tab customizers.
    /// </summary>
    protected void InitializeTabCustomizers()
    {
      TabCustomizers.Clear();
      if (Metadata != null)
      {
        foreach (CxWinTabMetadata tab in Metadata.Tabs)
        {
          CxTabCustomizer tabCustomizer =
            new CxTabCustomizer(Manager, this, tab);
          tabCustomizer.Initialize();
          TabCustomizers.Add(tabCustomizer);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModified()
    {
      bool result = !CurrentData.Compare(InitialData);
      if (!result)
      {
        foreach (CxTabCustomizer tabCustomizer in TabCustomizers)
        {
          result |= tabCustomizer.GetIsModifiedData();
          if (result) break;
        }
      }
      return result;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies the changes done to the customizer to the corresponding
    /// metadata object.
    /// </summary>
    /// <returns>true if changed, otherwise false</returns>
    public override bool ApplyToMetadata()
    {
      bool isChanged = base.ApplyToMetadata();
      if (!CxList.CompareOrdered(
            CurrentData.TabOrder.ToStringList(),
            Metadata.GetTabOrderManager().Ids))
      {
        Metadata.GetTabOrderManager().SetCustomOrder(CurrentData.TabOrder.ToStringList());
        isChanged |= true;
      }

      foreach (CxTabCustomizer tabCustomizer in TabCustomizers)
      {
        isChanged |= tabCustomizer.ApplyToMetadata();
      }
      return isChanged;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves customization data to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    private void SaveData(CxDbConnection connection)
    {
      if (GetIsModified())
      {
        // Commented because this algorythm doesn't take into account inner modifications in the tabs.

        // First we should check if the tab order is different from the initial one.
        //if (!CxList.CompareOrdered(CurrentData.TabOrder, InitialData.TabOrder))
        //{
        //  // Win forms management.
        //  CxWinFormMetadata winForm = Metadata;
        //  CxWinTabOrderManager tabManager = winForm.GetTabOrderManager();

        //  bool isWinFormNonCustomState =
        //    CxList.CompareOrdered(CurrentData.TabOrder.ToStringList(), tabManager.NonCustomIds);

        //  if (!isWinFormNonCustomState)
        //    DbInsertOrUpdateWinForm(connection);
        //}

        DbInsertOrUpdate(connection);
      }
      foreach (CxTabCustomizer tabCustomizer in TabCustomizers)
      {
        tabCustomizer.SaveData(connection);
      }

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves customization data to the database.
    /// </summary>
    /// <param name="connection">database connection</param>
    public void Save(CxDbConnection connection)
    {
      SaveData(connection);
      SaveLocalization(connection);
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
    private void SaveLocalization(CxDbConnection connection)
    {
      if (GetIsModifiedLocalization())
      {
        CxMultilanguage multilanguage = Context.Holder.Multilanguage;
      }

      foreach (CxTabCustomizer tabCustomizer in TabCustomizers)
      {
        tabCustomizer.SaveLocalization(connection);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the customizer to the metadata object default values.
    /// </summary>
    public override void ResetToDefault()
    {
      base.ResetToDefault();
      // Tab Order
      CxWinTabOrderManager orderManager = Metadata.GetTabOrderManager();
      orderManager.ResetToDefaults();
      CurrentData.TabOrder.Clear();
      CxList.AddRange(
        CurrentData.TabOrder, TabCustomizers.GetSublistBy(orderManager.Ids));

      foreach (CxTabCustomizer tabCustomizer in TabCustomizers)
      {
        tabCustomizer.ResetToDefault();
      }
    }
    //-------------------------------------------------------------------------
    public CxPanelCustomizer FindPanelById(string panelId)
    {
      foreach (CxTabCustomizer tabCustomizer in TabCustomizers)
      {
        foreach (CxPanelCustomizer panelCustomizer in tabCustomizer.PanelCustomizers)
          if (CxText.Equals(panelCustomizer.Id, panelId))
            return panelCustomizer;
        foreach (CxTabCustomizer subtabCustomizer in tabCustomizer.SubTabCustomizers)
          foreach (CxPanelCustomizer panelCustomizer in subtabCustomizer.PanelCustomizers)
            if (CxText.Equals(panelCustomizer.Id, panelId))
            return panelCustomizer;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    #region Database related
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
      provider["MetadataObjectTypeCd"] = "form";

      CxXmlRenderedObject renderedObject = Metadata.RenderToXml(new XmlDocument(), true, null);
      provider["MetadataContent"] = renderedObject.ToString();
      return provider;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Deletes customization record from the DB table.
    /// </summary>
    protected void DbDeleteWinForm(CxDbConnection connection)
    {
      string sql =
        @"delete from Framework_WinForms
                where WinFormId          = :WinFormId
                  and ApplicationCd      = :ApplicationCd";
      IxValueProvider provider = GetValueProvider();
      connection.ExecuteCommand(sql, provider);
    }
    //-------------------------------------------------------------------------
    #endregion
    //-------------------------------------------------------------------------
    /// <summary>
    ///                     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///                     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      return Metadata != null ? Metadata.ToString() : base.ToString();
    }
    //-------------------------------------------------------------------------
  }
}
