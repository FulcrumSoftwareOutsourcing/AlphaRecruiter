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
using System.Xml;
using System.Collections;
using Framework.Db;
using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Enumeration to determine web part position on the portal page (left or right).
  /// </summary>
  public enum NxWebPartPosition {Left, Right, Top, Bottom}
  //---------------------------------------------------------------------------
  /// <summary>
  /// For grid web parts determines automatic row selection mode.
  /// </summary>
  public enum NxGridRowSelection {None, OnLoad, Always}
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Web part content types.
  /// </summary>
  public enum NxWebPartContentType
  {
    // Custom web part content. 
    // ContentControl web part property should be specified
    Custom, 
    // New entity web part. 
    // ContentControl is taken from the WebEditControl property of the entity usage.
    New,    
    // Edit entity web part. 
    // ContentControl is taken from the WebEditControl property of the entity usage.
    Edit,
    // View entity web part. 
    // ContentControl is taken from the WebEditControl property of the entity usage.
    View,
    // Find and display entity list (grid) web part. May contain find dialog or not.
    // ContentControl is taken from the WebFindControl property of the entity usage.
    Find
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
	/// <summary>
	/// Metadata object for WebPart element.
	/// </summary>
	public class CxWebPartMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected CxTabMetadata m_Tab = null;
    protected CxEntityUsageMetadata m_EntityUsage = null;
    protected NxWebPartPosition m_Position = NxWebPartPosition.Top;
    protected Type m_ContentClass = null;
    protected Hashtable m_HiddenCommands = null;
    protected IList<CxErrorConditionMetadata> m_DisableConditions = new List<CxErrorConditionMetadata>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load data from</param>
    public CxWebPartMetadata(CxMetadataHolder holder, XmlElement element) : 
      base(holder, element)
		{
      if (CxUtils.NotEmpty(EntityUsageId))
      {
        m_EntityUsage = Holder.EntityUsages[EntityUsageId];
      }

      if (CxUtils.NotEmpty(this["position"]))
      {
        m_Position = (NxWebPartPosition) Enum.Parse(typeof(NxWebPartPosition), this["position"], true);
      }

      CxErrorConditionMetadata.LoadListFromNode(
        holder, 
        element.SelectSingleNode("disable_conditions"),
        m_DisableConditions,
        this);

      if (element.SelectSingleNode("visibility_condition") != null)
      {
        AddNodeToProperties(element, "visibility_condition");
      }

      if (element.SelectSingleNode("hint_expression") != null)
      {
        AddNodeToProperties(element, "hint_expression");
      }

      if (element.SelectSingleNode("hint") != null)
      {
        AddNodeToProperties(element, "hint");
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load data from</param>
    /// <param name="tab">tab container of web part</param>
    public CxWebPartMetadata(
      CxMetadataHolder holder, 
      XmlElement element, 
      CxTabMetadata tab) : this(holder, element)
    {
      m_Tab = tab;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Method is called after properties copying.
    /// </summary>
    /// <param name="sourceObj">object properties were taken from</param>
    override protected void DoAfterCopyProperties(CxMetadataObject sourceObj)
    {
      base.DoAfterCopyProperties(sourceObj);
      if (CxUtils.NotEmpty(EntityUsageId))
      {
        m_EntityUsage = Holder.EntityUsages[EntityUsageId];
      }
      if (sourceObj is CxWebPartMetadata)
      {
        CxErrorConditionMetadata.CombineLists(
          m_DisableConditions, 
          ((CxWebPartMetadata)sourceObj).DisableConditions,
          this);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata object override.
    /// </summary>
    /// <param name="element">XML element to load overridden properties from</param>
    override public void LoadOverride(XmlElement element)
    {
      base.LoadOverride(element);

      List<CxErrorConditionMetadata> disableConditions = new List<CxErrorConditionMetadata>();

      CxErrorConditionMetadata.LoadListFromNode(
        Holder,
        element.SelectSingleNode("disable_conditions"),
        disableConditions,
        this);

      CxErrorConditionMetadata.CombineLists(
        m_DisableConditions,
        disableConditions,
        this);
    }
    //----------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage ID to be used by the web part.
    /// </summary>
    public string EntityUsageId
    { get {return this["entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage metadata object to be used by the web part.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    { get {return m_EntityUsage;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns reference to the parent tab object metadata.
    /// </summary>
    public CxTabMetadata Tab
    { get {return m_Tab;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the relative position of the web part on the page.
    /// </summary>
    public NxWebPartPosition Position
    { get {return m_Position;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user control name (ASCX) for web part content.
    /// </summary>
    public string ContentControl
    { get {return this["content_control"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns C# class ID for web part content rendering.
    /// </summary>
    public string ContentClassId
    { get {return this["content_class_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns C# class for web part content rendering.
    /// </summary>
    public Type ContentClass
    { 
      get 
      {
        if (CxUtils.NotEmpty(ContentClassId))
        {
          if (m_ContentClass == null)
          {
            m_ContentClass = Holder.Classes[ContentClassId].Class;
          }
          return m_ContentClass;
        }
        return null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns user control name (ASCX) for web part template.
    /// </summary>
    public string TemplateControl
    { get {return this["template_control"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minimal width constraint.
    /// </summary>
    public int MinWidth
    { get {return CxInt.Parse(this["min_width"], -1);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minimal height constraint.
    /// </summary>
    public int MinHeight
    { get {return CxInt.Parse(this["min_height"], -1);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns fixed width of the web part.
    /// </summary>
    public int FixedWidth
    { get {return CxInt.Parse(this["fixed_width"], -1);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns fixed height of the web part.
    /// </summary>
    public int FixedHeight
    { get {return CxInt.Parse(this["fixed_height"], -1);} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if web part content should be fitted to available width.
    /// </summary>
    public bool IsFitToWidth
    { get {return this["fit_to_width"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if web part content should be fitted to available height.
    /// </summary>
    public bool IsFitToHeight
    { get {return this["fit_to_height"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given command should be hidden on the web part.
    /// </summary>
    /// <param name="command">command to check</param>
    public bool GetIsCommandHidden(CxCommandMetadata command)
    {
      if (m_HiddenCommands == null)
      {
        m_HiddenCommands = new Hashtable();
        string hiddenCommands = this["hidden_commands"];
        if (CxUtils.NotEmpty(hiddenCommands))
        {
          IList<string> commandList = CxText.DecomposeWithSeparator(hiddenCommands, ",");
          if (commandList != null)
          {
            foreach (string commandId in commandList)
            {
              m_HiddenCommands[commandId.ToUpper()] = true;
            }
          }
        }
      }
      return m_HiddenCommands.ContainsKey(command.Id.ToUpper());
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Web part content type.
    /// </summary>
    public NxWebPartContentType ContentType
    {
      get
      {
        string contentType = this["content_type"];
        if (CxUtils.NotEmpty(contentType))
        {
          return (NxWebPartContentType) Enum.Parse(typeof(NxWebPartContentType), contentType, true);
        }
        return NxWebPartContentType.Custom;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if web part title should be visible.
    /// </summary>
    public bool IsTitleVisible
    { get {return this["title_visible"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if web part menu should be visible.
    /// </summary>
    public bool IsMenuVisible
    { get {return this["menu_visible"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// EntityList Web Part property.
    /// Indicates is filtering enabled (if false, grid is displayed without filter).
    /// </summary>
    public bool IsFilterEnabled
    { get {return this["filter_enabled"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of web part on the portal page tab.
    /// </summary>
    public int TabIndex
    {
      get
      {
        if (Tab != null)
        {
          return Tab.WebParts.IndexOf(this);
        }
        return -1;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web part hint displayed at the top of the web part.
    /// </summary>
    public string Hint
    { get {return this["hint"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Expression that returns hint displayed at the top of the web part.
    /// </summary>
    public string HintExpression
    { get {return this["hint_expression"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage used as a value provider to calculate hint expression.
    /// </summary>
    public string HintExpressionEntityUsageId
    { get {return this["hint_expression_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage used as a value provider to calculate hint expression.
    /// </summary>
    public CxEntityUsageMetadata HintExpressionEntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(HintExpressionEntityUsageId) ?
          Holder.EntityUsages[HintExpressionEntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if 'hint' property of the web part is specified in metadata.
    /// </summary>
    public bool IsHintSpecified
    { get {return PropertyValues.ContainsKey("hint") || PropertyValues.ContainsKey("hint_expression");} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For entity edit web parts. 
    /// If true, redirect to previous page is performed after entity insert.
    /// True is a default setting.
    /// </summary>
    public bool IsRedirectedAfterInsert
    { get {return this["redirect_after_insert"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For entity edit web parts. 
    /// If true, redirect to previous page is performed after entity update.
    /// True is a default setting.
    /// </summary>
    public bool IsRedirectedAfterUpdate
    { get {return this["redirect_after_update"].ToLower() != "false";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks web part access permission for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">display entity usage</param>
    /// <param name="connection">Db connection to check security rule</param>
    /// <param name="entityValueProvider">entity instance value provider</param>
    public bool GetIsAllowed(
      CxEntityUsageMetadata entityUsage,
      CxDbConnection connection,
      IxValueProvider entityValueProvider)
    {
      if (Holder != null && Holder.Security != null)
      {
        IxValueProvider provider = 
          entityUsage != null && entityValueProvider != null ? 
          entityUsage.PrepareValueProvider(entityValueProvider) : null;
        return Holder.Security.GetRight(this, entityUsage, null, connection, provider);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of web part disable conditions.
    /// List contains CxErrorConditionMetadata objects.
    /// </summary>
    public IList<CxErrorConditionMetadata> DisableConditions
    { get {return m_DisableConditions;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines visibility condition of the web part.
    /// </summary>
    public string VisibilityCondition
    { get {return this["visibility_condition"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage ID to check visibility condition for.
    /// </summary>
    public string VisibilityConditionEntityUsageId
    { get {return this["visibility_condition_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage to check visibility condition for.
    /// </summary>
    public CxEntityUsageMetadata VisibilityConditionEntityUsage
    { 
      get 
      {
        return CxUtils.NotEmpty(VisibilityConditionEntityUsageId) ? 
          Holder.EntityUsages[VisibilityConditionEntityUsageId] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: page ID to redirect after OK button clicked.
    /// </summary>
    public string OkButtonRedirectPageId
    { get {return this["ok_button_redirect_page_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: page metadata to redirect after OK button clicked.
    /// </summary>
    public CxPageMetadata OkButtonRedirectPage
    { 
      get 
      {
        return CxUtils.NotEmpty(OkButtonRedirectPageId) ? Holder.Pages[OkButtonRedirectPageId] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: tab ID to redirect after OK button clicked.
    /// </summary>
    public string OkButtonRedirectTabId
    { get {return this["ok_button_redirect_tab_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: tab metadata to redirect after OK button clicked.
    /// </summary>
    public CxTabMetadata OkButtonRedirectTab
    { 
      get 
      {
        if (CxUtils.NotEmpty(OkButtonRedirectTabId) && OkButtonRedirectPage != null)
        {
          return OkButtonRedirectPage.Tabs.Find(OkButtonRedirectTabId);
        }
        return null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: used only with 'ok_button_redirect_page_id' property.
    /// Defines context entity usage ID to pass to redirection page after OK button clicked.
    /// </summary>
    public string OkButtonRedirectEntityUsageId
    { get {return this["ok_button_redirect_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: used only with 'ok_button_redirect_page_id' property.
    /// Defines context entity usage to pass to redirection page after OK button clicked.
    /// </summary>
    public CxEntityUsageMetadata OkButtonRedirectEntityUsage
    { 
      get 
      {
        return CxUtils.NotEmpty(OkButtonRedirectEntityUsageId) ? 
          Holder.EntityUsages[OkButtonRedirectEntityUsageId] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For edit form: caption to display on OK button.
    /// </summary>
    public string OkButtonText
    { get {return this["ok_button_text"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For grid web parts determines automatic row selection mode.
    /// </summary>
    public NxGridRowSelection GridRowSelection
    {
      get
      {
        return CxEnum.Parse(this["grid_row_selection"], NxGridRowSelection.None);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For grid web parts with automatic row selection mode specifies 
    /// entity usage ID to select row.
    /// </summary>
    public string GridRowSelectionEntityUsageId
    { get {return this["grid_row_selection_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For grid web parts with automatic row selection mode specifies 
    /// that navigator parent entity will be used as current selected entity in the grid.
    /// </summary>
    public bool GridRowSelectionByParent
    { get {return this["grid_row_selection_by_parent"] == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For grid web parts with automatic row selection mode specifies 
    /// entity usage to select row.
    /// </summary>
    public CxEntityUsageMetadata GridRowSelectionEntityUsage
    { 
      get 
      {
        return CxUtils.NotEmpty(GridRowSelectionEntityUsageId) ? 
          Holder.EntityUsages[GridRowSelectionEntityUsageId] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For view / edit form: control column count on the form.
    /// </summary>
    public string FormColumnCount
    { get {return this["form_column_count"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For view / edit form: standard control width on the form.
    /// </summary>
    public string FormControlWidth
    { get {return this["form_control_width"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For view / edit form: standard label width on the form.
    /// </summary>
    public string FormLabelWidth
    { get {return this["form_label_width"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity usage required as a parameter value provider for 
    /// select statement used for grid (Default_List web part).
    /// </summary>
    public string QueryValueProviderEntityUsageId
    { get {return this["query_value_provider_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage required as a parameter value provider for 
    /// select statement used for grid (Default_List web part).
    /// </summary>
    public CxEntityUsageMetadata QueryValueProviderEntityUsage
    { 
      get 
      {
        return CxUtils.NotEmpty(QueryValueProviderEntityUsageId) ? 
          Holder.EntityUsages[QueryValueProviderEntityUsageId] : null;
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.WebPart";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return Tab != null ? Tab.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of metadata objects properties was inherited from.
    /// </summary>
    override public IList<CxMetadataObject> InheritanceList
    {
      get
      {
        if (Tab != null)
        {
          List<CxMetadataObject> list = new List<CxMetadataObject>();
          list.Add(Holder.WebParts[Id]);
          return list;
        }
        return null;
      }
    }
    //----------------------------------------------------------------------------
  }
}