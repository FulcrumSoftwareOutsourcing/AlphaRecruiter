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
using System.Xml;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Enumeration describing command web request (GET or POST).
  /// </summary>
  public enum NxWebRequest {Get, Post}
  //---------------------------------------------------------------------------
  /// <summary>
  /// Command type (for security purposes).
  /// </summary>
  public enum NxCommandType {Edit, View}
  //---------------------------------------------------------------------------
  /// <summary>
  /// Entity save mode. Entity save is performed by command caller.
  /// </summary>
  public enum NxEntitySave { None, Before, BeforeSilent, After }
  //---------------------------------------------------------------------------
  /// <summary>
  /// Windows toolbar display mode.
  /// </summary>
  public enum NxToolbarDisplay { Default, ImageAndText, Text }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
	/// <summary>
	/// Metadata of entity command.
	/// </summary>
	public class CxCommandMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected List<CxErrorConditionMetadata> m_DisableConditions = new List<CxErrorConditionMetadata>();
    // Entity or entity usage metadata command belongs to
    protected CxEntityMetadata m_EntityMetadata = null;
    // List of child commands 
    // (commands reference this command as parent_command_id)
    protected List<CxCommandMetadata> m_ChildCommands = new List<CxCommandMetadata>();
    // List of entity usages command should be splitted to
    protected List<CxEntityUsageMetadata> m_SplitEntityUsages = null;
    protected IxCommandStateHandler m_StateHandlerClassInstance;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load properties from</param>
    /// <param name="entity">parent entity or entity usage object</param>
    public CxCommandMetadata(
      CxMetadataHolder holder, 
      XmlElement element,
      CxEntityMetadata entity) : base(holder, element)
    {
      m_EntityMetadata = entity;

      AddNodeToProperties(element, "sql_command");

      CxErrorConditionMetadata.LoadListFromNode(
        holder, 
        element.SelectSingleNode("disable_conditions"),
        m_DisableConditions,
        this);

      InheritPropertiesFrom(InheritanceList);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor. Creates new command from the existing command.
    /// </summary>
    /// <param name="command">command to copy properties from</param>
    /// <param name="entity">parent entity or entity usage object</param>
    public CxCommandMetadata(
      CxCommandMetadata command,
      CxEntityMetadata entity) : base(command.Holder)
    {
      Id = command.Id;
      m_EntityMetadata = entity;
      CopyPropertiesFrom(command);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element to load properties from</param>
    public CxCommandMetadata(
      CxMetadataHolder holder, 
      XmlElement element) : this(holder, element, null)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxCommandMetadata(CxMetadataHolder holder) : base(holder)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Method is called after properties copying.
    /// </summary>
    /// <param name="sourceObj">object properties were taken from</param>
    override protected void DoAfterCopyProperties(CxMetadataObject sourceObj)
    {
      base.DoAfterCopyProperties(sourceObj);
      if (sourceObj is CxCommandMetadata)
      {
        CxErrorConditionMetadata.CombineLists(
          m_DisableConditions, 
          ((CxCommandMetadata)sourceObj).DisableConditions,
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
    /// <summary>
    /// Executes SQL command specified in the command metadata.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="valueProvider"></param>
    public void ExecuteSqlCommand(
      CxDbConnection connection,
      IxValueProvider valueProvider)
    {
      if (CxUtils.NotEmpty(SqlCommandText))
      {
        connection.BeginTransaction();
        try
        {
          connection.ExecuteCommand(SqlCommandText, valueProvider);
          connection.Commit();
        }
        catch (Exception e)
        {
          connection.Rollback();
          throw new ExException(e.Message, e);
        }
      }
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns operation code for command.
    /// </summary>
    public string OperationCode
    { get {return this["operation_code"].ToUpper();} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the parent command.
    /// </summary>
    public string ParentCommandId
    { get {return this["parent_command_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets the caption of the command.
    /// </summary>
    /// <param name="entityUsage">the entity usage context</param>
    /// <returns>the caption</returns>
    public string GetCaption(CxEntityUsageMetadata entityUsage)
    {
      string text = Text;
      if (!string.IsNullOrEmpty(text))
      {
        if (entityUsage != null)
          text = entityUsage.ReplacePlaceholders(base.Text);
      }
      return text;
    }
	  //-------------------------------------------------------------------------
    /// <summary>
    /// Returns target portal for command.
    /// If portal is not null, the command redirects to the specified portal.
    /// </summary>
    public CxPortalMetadata TargetPortal
    {
      get
      {
        string id = this["target_portal_id"];
        return CxUtils.NotEmpty(id) ? Holder.Portals[id] : null;
      }
      set
      {
        this["target_portal_id"] = value != null ? value.Id : "";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns target navigation tree item for command.
    /// If tree item is not null, the command redirects to the specified tree item.
    /// </summary>
    public CxTreeItemMetadata TargetTreeItem
    {
      get
      {
        string id = this["target_tree_item_id"];
        return TargetPortal != null && CxUtils.NotEmpty(id) ? TargetPortal.Find(id) : null;
      }
      set
      {
        this["target_tree_item_id"] = value != null ? value.Id : "";
        if (TargetPortal != null && TargetPortal != value.Portal)
        {
          TargetPortal = value.Portal;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns target page for command.
    /// If page is not null, the command redirects to the specified page.
    /// </summary>
    public CxPageMetadata TargetPage
    {
      get
      {
        string id = this["target_page_id"];
        return CxUtils.NotEmpty(id) ? (Holder.Pages != null ? Holder.Pages[id]: null) : null;
      }
      set
      {
        this["target_page_id"] = value != null ? value.Id : "";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns target page tab for command.
    /// If page tab is not null, the command redirects to the specified page tab.
    /// </summary>
    public CxTabMetadata TargetTab
    {
      get
      {
        string id = this["target_tab_id"];
        return TargetPage != null && CxUtils.NotEmpty(id) ? TargetPage.Tabs.Find(id) : null;
      }
      set
      {
        this["target_tab_id"] = value != null ? value.Id : "";
        TargetPage = value.Page;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the image used for the command.
    /// </summary>
    public string ImageId
    { get {return this["image_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns image metadata for image used for display command.
    /// </summary>
    public CxImageMetadata Image
    { get {return CxUtils.NotEmpty(ImageId) ? Holder.Images[ImageId] : null; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command WEB request type - GET or POST.
    /// </summary>
    public NxWebRequest WebRequest
    { 
      get
      {
        if (this["web_request"].ToUpper() == "GET")
        {
          return NxWebRequest.Get;
        }
        else
        {
          return NxWebRequest.Post;
        }
      }
      set
      {
        this["web_request"] = value == NxWebRequest.Get ? "GET" : "POST";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns web display mode (is target URL should be opened in popup window).
    /// The property value should be empty or should contain "Popup" string.
    /// </summary>
    public string WebDisplayMode
    { get {return this["web_display_mode"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true of command requires related entity instance.
    /// </summary>
    public bool IsEntityInstanceRequired
    { get {return this["is_entity_instance_required"] == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage ID related to the command.
    /// </summary>
    public string EntityUsageId
    { get {return this["entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage object related to the command.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get
      {
        if (CxUtils.NotEmpty(EntityUsageId))
        {
          return Holder.EntityUsages[EntityUsageId];
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent entity usage ID related to the command.
    /// </summary>
    public string ParentEntityUsageId
    { get {return this["parent_entity_usage_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent entity usage object related to the command.
    /// </summary>
    public CxEntityUsageMetadata ParentEntityUsage
    {
      get
      {
        if (CxUtils.NotEmpty(ParentEntityUsageId))
        {
          return Holder.EntityUsages[ParentEntityUsageId];
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usage metadata to use with the command.
    /// </summary>
    /// <param name="entityUsage">entity usage to which the command belongs</param>
    /// <param name="command">command to get entity usage for</param>
    /// <returns>entity usage to use with the command</returns>
    static public CxEntityUsageMetadata GetEntityUsage(
      CxEntityUsageMetadata entityUsage,
      CxCommandMetadata command)
    {
      if (command != null && command.EntityUsage != null)
      {
        return command.EntityUsage;
      }
      if (entityUsage != null && entityUsage.CommandEntityUsage != null)
      {
        return entityUsage.CommandEntityUsage;
      }
      return entityUsage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command confirmation text, if specified.
    /// </summary>
    public string ConfirmationText
    { get {return this["confirmation_text"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent command from the list of entity usage commands.
    /// </summary>
    /// <param name="entityUsage">entity usage to get parent command from</param>
    public CxCommandMetadata GetParentCommand(CxEntityUsageMetadata entityUsage)
    {
      if (entityUsage != null && CxUtils.NotEmpty(ParentCommandId))
      {
        return entityUsage.GetCommand(ParentCommandId);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command type (for security purposes).
    /// </summary>
    public NxCommandType CommandType
    {
      get
      {
        string commandType = this["command_type"];
        if (CxUtils.NotEmpty(commandType))
        {
          return (NxCommandType) Enum.Parse(typeof(NxCommandType), commandType, true);
        }
        return NxCommandType.Edit;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Group name for the metadata object (if applicable).
    /// </summary>
    override public string GroupName
    { get {return CommandType.ToString();} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the string representation of the permission group id depending 
    /// on the type of the command.
    /// </summary>
    protected string PermissionGroupIdByCommandType
    {
      get { return CommandType == NxCommandType.View ? "VIEW" : "EDIT"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command is enabled for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">command entity usage</param>
    /// <param name="connection">Db connection to check security rule</param>
    /// <param name="entityValueProvider">entity instance value provider</param>
    public bool GetIsEnabled(
      CxEntityUsageMetadata entityUsage,
      CxDbConnection connection,
      IxValueProvider entityValueProvider)
    {
      if (Holder != null && Holder.Security != null)
      {
        IxValueProvider provider = entityValueProvider != null ? 
          entityUsage.PrepareValueProvider(entityValueProvider) : null;
        return Holder.Security.GetRight(
          this, entityUsage, 
          PermissionGroupIdByCommandType, 
          connection, provider);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command is enabled for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">command entity usage</param>
    public bool GetIsEnabled(CxEntityUsageMetadata entityUsage)
    {
      return GetIsEnabled(entityUsage, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if the command is a default command for the entity usage.
    /// </summary>
    public bool IsDefault
    { get {return this["is_default"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns target command ID to redirect to.
    /// It is used in a combination with command entity override.
    /// </summary>
    public string TargetCommandId
    { get {return this["target_command_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns SQL command text specified in metadata.
    /// </summary>
    public string SqlCommandText
    { get {return this["sql_command"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the method in the entity instance to execute by the command.
    /// </summary>
    public string InstanceMethodName
    { get {return this["instance_method"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of class containing static method to execute by the command.
    /// </summary>
    public string StaticClassId
    { get {return this["static_class_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of class containing static method to execute by the command.
    /// </summary>
    public CxClassMetadata StaticClassMetadata
    { get {return CxUtils.NotEmpty(StaticClassId) ? Holder.Classes[StaticClassId] : null;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of static method to execute by the command.
    /// </summary>
    public string StaticMethodName
    { get {return this["static_method"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if page should be refreshed after command execution.
    /// </summary>
    public bool IsPageToBeRefreshed
    { get {return this["refresh_page"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if web part should be refreshed after command execution.
    /// </summary>
    public bool IsPartToBeRefreshed
    { get {return this["refresh_part"].ToLower() == "true";} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of command disable conditions.
    /// List contains CxErrorConditionMetadata objects.
    /// </summary>
    public IList<CxErrorConditionMetadata> DisableConditions
    { get {return m_DisableConditions;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the attribute containing entity usage ID for 
    /// command execution. Attribute value is taken from the current entity
    /// related to the command.
    /// </summary>
    public string DynamicEntityUsageAttrId
    { get {return this["dynamic_entity_usage_attr_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the attribute containing command ID for 
    /// command execution. Attribute value is taken from the current entity
    /// related to the command.
    /// </summary>
    public string DynamicCommandAttrId
    { get {return this["dynamic_command_attr_id"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the attribute containing entity usage ID for 
    /// command execution. Attribute value is taken from the current entity
    /// related to the command. Current (this) entity is converted to the given
    /// entity usage.
    /// </summary>
    public string ConvertEntityUsageAttrId
    { get { return this["convert_entity_usage_attr_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command can be applied to multiple selected entities.
    /// </summary>
    public bool IsMultiple
    { 
      get 
      {
        if (this["multiple"].ToLower() == "true")
        {
          return true;
        }
        else if (this["multiple"].ToLower() == "false")
        {
          return false;
        }
        else
        {
          // By default, SQL commands are multiple.
          return TargetPortal == null &&
                 TargetTreeItem == null &&
                 TargetPage == null &&
                 TargetTab == null &&
                 CxUtils.NotEmpty(SqlCommandText);
        }
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the windows command hander class
    /// </summary>
    public string WindowsHandlerClassId
    { get { return this["windows_handler_class_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns class metadata of the windows command hander class
    /// </summary>
    public CxClassMetadata WindowsHandlerClass
    { 
      get 
      { 
        return CxUtils.NotEmpty(WindowsHandlerClassId) ? Holder.Classes[WindowsHandlerClassId] : null; 
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an ID of the handler that returns the current command state.
    /// </summary>
    public string StateHandlerClassId
    { get { return this["state_handler_class_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns class metadata of the command state handler class.
    /// </summary>
    public CxClassMetadata StateHandlerClass
    { 
      get 
      { 
        return CxUtils.NotEmpty(StateHandlerClassId) ? Holder.Classes[StateHandlerClassId] : null; 
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an instance of the state handler class.
    /// </summary>
    public IxCommandStateHandler StateHandlerClassInstance
    {
      get
      {
        if (m_StateHandlerClassInstance == null)
        {
          CxClassMetadata classMetadata = StateHandlerClass;
          if (classMetadata != null)
          {
            m_StateHandlerClassInstance = CxType.CreateInstance(classMetadata.Class) as IxCommandStateHandler;
          }
        }
        return m_StateHandlerClassInstance;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command windows handler is batch handler.
    /// </summary>
    public bool IsWindowsHandlerBatch
    { get { return this["windows_handler_batch"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets unique integer identifier of the command (for windows menu).
    /// </summary>
    public int UniqueID
    {
      get
      {
        int result = CxInt.Parse(this["unique_id"], 0);
        if (result == 0 && EntityMetadata != null && Holder.Commands != null)
        {
          CxCommandMetadata originalCommand = Holder.Commands.Find(Id);
          if (originalCommand != null)
          {
            result = UniqueID = originalCommand.UniqueID;
          }
        }
        return result;
      }
      set { this["unique_id"] = Convert.ToString(value); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command category name (for windows menu).
    /// </summary>
    public string MenuCategory
    { get { return this["menu_category"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command display order (in windows menu, within category)
    /// </summary>
    public int DisplayOrder
    { get { return CxInt.Parse(this["display_order"], Int32.MaxValue); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command visibility in the main menu (in windows menu)
    /// </summary>
    public bool IsVisibleInMainMenu
    { get { return this["visible_in_main_menu"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command visibility in the main toolbar (in windows application)
    /// </summary>
    public bool IsVisibleInToolbar
    { get { return this["visible_in_toolbar"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command visibility in the main toolbar is specified
    /// </summary>
    public bool IsVisibleInToolbarSpecified
    { get { return CxUtils.NotEmpty(this["visible_in_toolbar"]); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns command visibility in the popup menu (in windows menu)
    /// </summary>
    public bool IsVisibleInPopupMenu
    { get { return this["visible_in_popup_menu"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the command is a batch one in the Silverlight scope.
    /// </summary>
    public bool IsSlHandlerBatch
    { get { return CxBool.Parse(this["sl_handler_batch"], false); } }
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Windows toolbar display mode.
    /// </summary>
    public NxToolbarDisplay ToolbarDisplay
    { 
      get 
      {
        return CxEnum.Parse(this["toolbar_display"], NxToolbarDisplay.Default);
      } 
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if command begins group (in windows menu)
    /// </summary>
    public bool IsGroupBeginner
    { get { return this["begin_group"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Command shortcut (for windows menu). 
    /// Should be valid to convert into the Shortcut enumeration.
    /// </summary>
    public string Shortcut
    { get { return this["shortcut"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if command should be splitted by entity usages.
    /// </summary>
    public bool IsSplittedByEntityUsages
    { get { return this["split_by_entity_usages"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if menu item should be always published in the main application menu.
    /// Is used for the non-instance commands in a combination with a particular
    /// entity usage to put New Something commands to the main application menu.
    /// Windows menu only.
    /// </summary>
    public bool IsPublishedGlobally
    { get { return this["publish_global"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if menu item should be always published in the view/edit form menu.
    /// Is used for the non-instance commands in a combination with a particular
    /// entity usage to put New Something commands to the view/edit form menu.
    /// Windows menu only.
    /// </summary>
    public bool IsPublishedLocally
    { get { return this["publish_local"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if command should be hidden if not available. (windows menu)
    /// </summary>
    public bool IsHiddenWhenDisabled
    { get { return this["hidden_when_disabled"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if command should be disabled if not available. (windows menu)
    /// </summary>
    public bool IsDisabledWhenHidden
    { get { return CxBool.Parse(this["disabled_when_hidden"], false); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if current entity image should be assigned to the command. (windows menu)
    /// </summary>
    public bool IsEntityImageCopied
    { get { return this["copy_entity_image"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if command text should be followed with ellipsis (...). (windows menu)
    /// </summary>
    public bool IsElliplisMarked
    { get { return this["ellipsis_mark"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity save mode. Entity save must be performed by the command caller.
    /// </summary>
    public NxEntitySave EntitySave
    {
      get
      {
        string value = this["entity_save"];
        if (CxUtils.IsEmpty(value) && CxUtils.NotEmpty(SqlCommandText))
        {
          return NxEntitySave.Before;
        }
        return CxEnum.Parse(value, NxEntitySave.None);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity usage metadata to take image from.
    /// </summary>
    public string ImageEntityUsageId
    { get { return this["image_entity_usage_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage metadata to take image from.
    /// </summary>
    public CxEntityUsageMetadata ImageEntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(ImageEntityUsageId) ? Holder.EntityUsages[ImageEntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages command should be splitted to.
    /// </summary>
    public IList<CxEntityUsageMetadata> SplitEntityUsages
    {
      get
      {
        string csvText = this["split_entity_usages"];
        if (m_SplitEntityUsages == null && CxUtils.NotEmpty(csvText))
        {
          m_SplitEntityUsages = new List<CxEntityUsageMetadata>();
          IList<string> list = CxText.DecomposeWithSeparator(csvText, ",");
          if (list != null)
          {
            foreach (string entityUsageId in list)
            {
              CxEntityUsageMetadata entityUsage = Holder.EntityUsages[entityUsageId];
              m_SplitEntityUsages.Add(entityUsage);
            }
          }
        }
        return m_SplitEntityUsages;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity metadata command was created within.
    /// </summary>
    internal CxEntityMetadata EntityMetadata
    { get { return m_EntityMetadata; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of child commands.
    /// Listed commands reference this command via parent_command_id in 
    /// the Commands.xml file.
    /// </summary>
    public IList<CxCommandMetadata> ChildCommands
    { get { return m_ChildCommands; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.Command";
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
        if (m_EntityMetadata != null)
        {
          string prefix = m_EntityMetadata is CxEntityUsageMetadata ? "EntityUsage." : "Entity.";
          return prefix + m_EntityMetadata.Id + "." + Id;
        }
        return base.LocalizationObjectName;
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
        List<CxMetadataObject> list = new List<CxMetadataObject>();
        if (m_EntityMetadata != null)
        {
          if (m_EntityMetadata is CxEntityUsageMetadata)
          {
            CxCommandMetadata parentCommand;
            CxEntityUsageMetadata entityUsage = (CxEntityUsageMetadata) m_EntityMetadata;
            if (entityUsage.InheritedEntityUsage != null)
            {
              parentCommand = entityUsage.InheritedEntityUsage.GetCommand(Id);
              if (parentCommand != null)
              {
                list.Add(parentCommand);
              }
            }
            if (entityUsage.Entity != null)
            {
              parentCommand = entityUsage.Entity.GetCommand(Id);
              if (parentCommand != null)
              {
                list.Add(parentCommand);
              }
            }
          }
          list.Add(Holder.Commands[Id]);
        }
        return list;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages which are referenced by attribute properties.
    /// </summary>
    /// <returns>list of CxEntityMetadata objects or null</returns>
    override public IList<CxEntityMetadata> GetReferencedEntities()
    {
      UniqueList<CxEntityMetadata> result = new UniqueList<CxEntityMetadata>();
      
      result.Add(EntityUsage);
      result.Add(ParentEntityUsage);
      
      foreach (CxErrorConditionMetadata condition in DisableConditions)
      {
        result.Add(condition.Entity);
        result.Add(condition.EntityUsage);
      }
      return result;
    }

        //-------------------------------------------------------------------------
        /// <summary>
        /// Returns is command available on Edir/View forms in JS Application.
        /// </summary>
        public bool AvailableOnEditform
        { get { return CxBool.Parse( this["available_on_editform"], false); } }
    }
}