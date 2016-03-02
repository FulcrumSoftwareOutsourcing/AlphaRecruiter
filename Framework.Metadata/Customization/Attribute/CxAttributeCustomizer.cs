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
  //---------------------------------------------------------------------------
  /// <summary>
  /// Represents a shortened description of the attribute metadata.
  /// </summary>
  public class CxAttributeCustomizer : CxCustomizerBase, IxStorableInIdOrder
  {
    //-------------------------------------------------------------------------
    private CxEntityCustomizer m_ParentCustomizer;
    private CxAttributeCustomizerData m_CurrentData;
    private CxAttributeCustomizerData m_InitialData;
    private CxAttributeCustomizerLocalization m_CurrentLocalization;
    private CxAttributeCustomizerLocalization m_InitialLocalization;
    private string m_ValueId;
    private string m_TextId;
    private CxAttributeMetadata m_TextMetadata;
    private CxAttributeMetadata m_ValueMetadata;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata object the customizer belongs to.
    /// </summary>
    public CxAttributeMetadata TextMetadata
    {
      get { return m_TextMetadata ?? ParentCustomizer.Metadata.GetTextDefinedAttribute(ParentCustomizer.Metadata.GetAttribute(Id)); }
      set { m_TextMetadata = value; }
    }
    //-------------------------------------------------------------------------
    public CxAttributeMetadata ValueMetadata
    {
      get
      {
        return m_ValueMetadata ?? ParentCustomizer.Metadata.GetAttribute(Id);
      }
      set { m_ValueMetadata = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Current data snapshot, non-saved data.
    /// </summary>
    public CxAttributeCustomizerData CurrentData
    {
      get { return m_CurrentData; }
      set { m_CurrentData = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initial data snapshot, the data which is actually applied to the application.
    /// </summary>
    public CxAttributeCustomizerData InitialData
    {
      get { return m_InitialData; }
      set { m_InitialData = value; }
    }
    //-------------------------------------------------------------------------
    public bool IsCustomizable
    {
      get { return CurrentData.IsCustomizable; }
    }
    //-------------------------------------------------------------------------
    public bool IsCustomizableLookup
    {
      get { return ValueMetadata.CustomizableLookup; }
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizerLocalization CurrentLocalization
    {
      get { return m_CurrentLocalization; }
      set { m_CurrentLocalization = value; }
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizerLocalization InitialLocalization
    {
      get { return m_InitialLocalization; }
      set { m_InitialLocalization = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Logical parent customizer object.
    /// </summary>
    public CxEntityCustomizer ParentCustomizer
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
      get { return m_ValueId; }
    }
    //-------------------------------------------------------------------------
    public string TextId
    {
      get { return m_TextId = m_TextId ?? (TextMetadata != null ? TextMetadata.Id : null); }
    }
    //-------------------------------------------------------------------------
    public CxAttributeCustomizer(
      CxEntityCustomizer parentCustomizer, string attributeId)
      : base(parentCustomizer.Manager)
    {
      m_ValueId = attributeId;
      ParentCustomizer = parentCustomizer;
      Context = ParentCustomizer.Context;
      Manager = ParentCustomizer.Manager;

      CurrentData = new CxAttributeCustomizerData(this);
      InitialData = new CxAttributeCustomizerData(this);

      CurrentLocalization = new CxAttributeCustomizerLocalization(this);
      InitialLocalization = new CxAttributeCustomizerLocalization(this);
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
    /// Indicates whether the customizer has been modified somehow.
    /// </summary>
    /// <returns>true if modified, otherwise false</returns>
    public bool GetIsModifiedLocalization()
    {
      return !CurrentLocalization.Compare(InitialLocalization);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies the customizer state to the metadata it belongs to.
    /// </summary>
    /// <returns>true if changes took place</returns>
    public override bool ApplyToMetadata()
    {
      bool isChanged = base.ApplyToMetadata();
      CxAttributeMetadata metadata = ValueMetadata;

      bool isChanged_WinControlPlacement = !CxText.Equals(metadata.WinControlPlacement, CurrentData.WinControlPlacement);
      bool isChanged_WinControl = !CxText.Equals(metadata.WinControl, CurrentData.WinControl);
      bool isChanged_FilterAdvanced = metadata.FilterAdvanced != CurrentData.IsShownOnAdvancedFilterPanel;
      bool isChanged_IsRequired = metadata.Nullable == CurrentData.IsRequired;
      bool isChanged_IsNewLine = metadata.IsPlacedOnNewLine != CurrentData.IsNewLine;
      bool isChanged_Default = !CxText.Equals(metadata.Default, CurrentData.DefaultValue);
      bool isChanged_RowSourceId = !CxText.Equals(metadata.RowSourceId, CurrentData.RowSourceId);
      bool isChanged_IsCustomizable = metadata.IsCustomizable != CurrentData.IsCustomizable;

      if (isChanged_WinControlPlacement ||
          isChanged_WinControl ||
          isChanged_FilterAdvanced ||
          isChanged_IsRequired ||
          isChanged_IsNewLine ||
          isChanged_Default ||
          isChanged_RowSourceId ||
          isChanged_IsCustomizable)
      {
        if (!ParentCustomizer.Metadata.IsAttributeDefined(metadata.Id))
        {
          metadata = ValueMetadata = metadata.ApplyToEntityUsage(ParentCustomizer.Metadata);
        }
        if (isChanged_WinControlPlacement)
          metadata.WinControlPlacement = CurrentData.WinControlPlacement;

        if (isChanged_WinControl)
          metadata.WinControl = CurrentData.WinControl;

        if (isChanged_FilterAdvanced)
          metadata.FilterAdvanced = CurrentData.IsShownOnAdvancedFilterPanel;

        if (isChanged_IsRequired)
          metadata.Nullable = !CurrentData.IsRequired;

        if (isChanged_IsNewLine)
          metadata.IsPlacedOnNewLine = CurrentData.IsNewLine;

        if (isChanged_IsCustomizable)
          metadata.IsCustomizable = CurrentData.IsCustomizable;

        if (isChanged_Default)
          metadata.Default = CurrentData.DefaultValue;

        if (isChanged_RowSourceId)
          metadata.RowSourceId = CurrentData.RowSourceId;

        isChanged |= true;
      }
      return isChanged;
    }
    //-------------------------------------------------------------------------
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
      if (GetIsModifiedLocalization())
      {
        CxMultilanguage multilanguage = ValueMetadata.Holder.Multilanguage;
        if (multilanguage != null)
        {
          foreach (KeyValuePair<string, string> langCaption in CurrentLocalization.LanguageCaptionMap)
          {
            string localizationObjectName = ParentCustomizer.Metadata.Id + "." + ValueMetadata.Id;
            multilanguage.SetLocalizedValue(
              connection,
              langCaption.Key,
              CxAttributeUsageMetadata.OBJECT_TYPE_ATTRIBUTE_USAGE,
              "caption",
              localizationObjectName,
              CurrentLocalization.NonLocalizedCaption,
              langCaption.Value);

            if (TextMetadata != null)
            {
              string localizationObjectName2 = ParentCustomizer.Metadata.Id + "." + TextMetadata.Id;
              multilanguage.SetLocalizedValue(
                connection,
                langCaption.Key,
                CxAttributeUsageMetadata.OBJECT_TYPE_ATTRIBUTE_USAGE,
                "caption",
                localizationObjectName2,
                CurrentLocalization.NonLocalizedCaption,
                langCaption.Value);
            }
          }
        }

        InitialLocalization = CurrentLocalization.Clone();
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

      CxAttributeMetadata metadata = ValueMetadata;

      CurrentData.WinControlPlacement = metadata.GetInitialProperty("win_control_placement", false);
      CurrentData.WinControl = metadata.GetInitialProperty("win_control", false);
      CurrentData.IsShownOnAdvancedFilterPanel = CxBool.Parse(metadata.GetInitialProperty("filter_advanced", false), false);
      CurrentData.IsRequired = !CxBool.Parse(metadata.GetInitialProperty("nullable", false), true);
      CurrentData.IsNewLine = CxBool.Parse(metadata.GetInitialProperty("new_line", false), false);
      CurrentData.DefaultValue = metadata.GetInitialProperty("default", false);
      CurrentData.RowSourceId = metadata.GetInitialProperty("row_source_id", false);
      CurrentData.IsCustomizable = CxBool.Parse(metadata.GetInitialProperty("customizable", false), true);
    }
    //-------------------------------------------------------------------------
  }
}
