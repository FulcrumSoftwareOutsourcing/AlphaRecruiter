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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// The class to hold information about entity attribute.
  /// </summary>
  public class CxAttributeMetadata : CxMetadataObject
  {
    #region Consts
    //----------------------------------------------------------------------------
    // Attribute types
    public const string TYPE_STRING = "string";
    public const string TYPE_LONGSTRING = "longstring";
    public const string TYPE_DATETIME = "datetime";
    public const string TYPE_DATE = "date";
    public const string TYPE_TIME = "time";
    public const string TYPE_INT = "int";
    public const string TYPE_FLOAT = "float";
    public const string TYPE_BOOLEAN = "boolean";
    public const string TYPE_FILE = "file";
    public const string TYPE_IMAGE = "image";
    public const string TYPE_ICON = "icon";
    public const string TYPE_LINK = "hyperlink";

    // Web control types
    public const string WEB_CONTROL_TEXT = "text";
    public const string WEB_CONTROL_CHECKBOX = "checkbox";
    public const string WEB_CONTROL_INT = "int";
    public const string WEB_CONTROL_NUMBER = "number";
    public const string WEB_CONTROL_DATE = "date";
    public const string WEB_CONTROL_DATETIME = "datetime";
    public const string WEB_CONTROL_TIME = "time";
    public const string WEB_CONTROL_MONTH = "month";
    public const string WEB_CONTROL_FUTURE_MONTH = "future_month";
    public const string WEB_CONTROL_DD_DATE = "date_dropdown";
    public const string WEB_CONTROL_MEMO = "memo";
    public const string WEB_CONTROL_HTML = "html";
    public const string WEB_CONTROL_DROPDOWN = "dropdown";
    public const string WEB_CONTROL_LOOKUP = "lookup";
    public const string WEB_CONTROL_PASSWORD = "password";
    public const string WEB_CONTROL_FILE = "file";
    public const string WEB_CONTROL_IMAGE = "image";
    public const string WEB_CONTROL_ICON = "icon";
    public const string WEB_CONTROL_ICON_TEXT = "icon_text";
    public const string WEB_CONTROL_LINK = "hyperlink";
    public const string WEB_CONTROL_SCRAMBLED_TEXT = "scrambled_text";

    internal const string OBJECT_TYPE_ATTRIBUTE = "Metadata.Attribute";
    #endregion

    #region Fields
    //----------------------------------------------------------------------------
    protected CxEntityMetadata m_EntityMetadata; // Entity this attribute belongs to
    protected string m_OriginalId = ""; // Attribute ID as it was set in metadata
    protected Hashtable m_AttributesToGetFromLookup; // Map with attributes to get from lookup datasource and put to teh current grid/edit form
    private CxRowSourceMetadata m_RowSource_Cache;
    //----------------------------------------------------------------------------
    private readonly Dictionary<CxEntityMetadata, CxAttributeMetadata>
      m_TextAttributes = new Dictionary<CxEntityMetadata, CxAttributeMetadata>();
    private readonly Dictionary<CxEntityMetadata, CxAttributeMetadata>
      m_ValueAttributes = new Dictionary<CxEntityMetadata, CxAttributeMetadata>();
    private readonly Dictionary<CxEntityMetadata, CxAttributeMetadata>
      m_TextDefinedAttributes = new Dictionary<CxEntityMetadata, CxAttributeMetadata>();
    private readonly Dictionary<CxEntityMetadata, CxAttributeMetadata>
      m_ValueDefinedAttributes = new Dictionary<CxEntityMetadata, CxAttributeMetadata>();
    //----------------------------------------------------------------------------
    private bool m_IsReferenceEntityUsagesCached;
    private CxEntityUsageMetadata[] m_ReferenceEntityUsages;
    //----------------------------------------------------------------------------
    #endregion

    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="entity">entity this attribute belongs to</param>
    public CxAttributeMetadata(XmlElement element, CxEntityMetadata entity)
      : base(entity.Holder, element)
    {
      m_OriginalId = CxXml.GetAttr(element, "id");
      EntityMetadata = entity;

      AddNodeToProperties(element, "filter_condition");

      if (element.SelectSingleNode("visibility_condition") != null)
      {
        AddNodeToProperties(element, "visibility_condition");
      }
      if (element.SelectSingleNode("read_only_condition") != null)
      {
        AddNodeToProperties(element, "read_only_condition");
      }
      if (element.SelectSingleNode("mandatory_condition") != null)
      {
        AddNodeToProperties(element, "mandatory_condition");
      }


            if (element.SelectSingleNode("calc_total_js") != null)
            {
                AddNodeToProperties(element, "calc_total_js");
            }
        }
    //----------------------------------------------------------------------------  
    #endregion

    #region Caption related stuff
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute caption.
    /// </summary>
    public string Caption
    {
      get
      {
        string caption = this["caption"];
        return CxUtils.NotEmpty(caption) ? caption : Id;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute caption to be shown in the grid.
    /// </summary>
    public string CaptionGrid
    {
      get { return this["caption_grid"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute caption related to the edit form.
    /// </summary>
    public string CaptionEdit
    {
      get { return this["caption_edit"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute caption related to the filter form.
    /// </summary>
    public string CaptionFilter
    {
      get { return this["caption_filter"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute caption related to the Query interface.
    /// </summary>
    public string CaptionQuery
    {
      get { return this["caption_query"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute caption to display on a Edit/View form.
    /// </summary>
    public string FormCaption
    {
      get { return Contains("form_caption") ? this["form_caption"] : Caption; }
    }
    //----------------------------------------------------------------------------
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute caption translated in the context of the entity usage.
    /// </summary>
    /// <param name="entityMetadata">entity or entity usage context</param>
    /// <returns>attribute caption translated in the context of the entity usage</returns>
    public string GetCaption(CxEntityMetadata entityMetadata)
    {
      string localizedCaption = null;
      string originalCaption = GetNonLocalizedPropertyValue("caption");
      CxEntityUsageMetadata entityUsage = entityMetadata as CxEntityUsageMetadata;
      if (Holder.Multilanguage != null)
      {
        while (localizedCaption == null && entityUsage != null)
        {
          localizedCaption = Holder.Multilanguage.GetLocalizedValue(
            CxAttributeUsageMetadata.OBJECT_TYPE_ATTRIBUTE_USAGE,
            "caption",
            entityUsage.Id + "." + Id,
            originalCaption);
          entityUsage = entityUsage.InheritedEntityUsage;
        }
      }

      if (localizedCaption != null)
      {
        localizedCaption = Holder.PlaceholderManager.ReplacePlaceholders(localizedCaption);
        localizedCaption = CxGlobalPlaceholderManager.Instance.ReplacePlaceholders(localizedCaption);
        return localizedCaption;
      }
      else
      {
        originalCaption = Holder.PlaceholderManager.ReplacePlaceholders(originalCaption);
        originalCaption = CxGlobalPlaceholderManager.Instance.ReplacePlaceholders(originalCaption);
        return originalCaption;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute caption translated in the context of the entity usage.
    /// </summary>
    /// <param name="entityMetadata">entity or entity usage context</param>
    /// <param name="context">attribute context</param>
    /// <returns>attribute caption translated in the context of the entity usage</returns>
    public string GetCaption(CxEntityMetadata entityMetadata, NxAttributeContext context)
    {
      string localizedCaption = null;
      string propertyName;
      switch (context)
      {
        case NxAttributeContext.GridVisible:
          propertyName = "caption_grid";
          break;
        case NxAttributeContext.Edit:
          propertyName = "caption_edit";
          break;
        case NxAttributeContext.Filter:
          propertyName = "caption_filter";
          break;
        case NxAttributeContext.Queryable:
          propertyName = "caption_query";
          break;
        default:
          throw new ExException(string.Format("Unknown attribute context encountered: {0}", context));
      }
      string originalCaption = GetNonLocalizedPropertyValue(propertyName);
      if (!string.IsNullOrEmpty(originalCaption))
      {
        CxEntityUsageMetadata entityUsage = entityMetadata as CxEntityUsageMetadata;
        if (Holder.Multilanguage != null)
        {
          while (localizedCaption == null && entityUsage != null)
          {
            localizedCaption = Holder.Multilanguage.GetLocalizedValue(
              CxAttributeUsageMetadata.OBJECT_TYPE_ATTRIBUTE_USAGE,
              propertyName,
              entityUsage.Id + "." + Id,
              originalCaption);
            entityUsage = entityUsage.InheritedEntityUsage;
          }
        }
      }
      if (localizedCaption != null)
      {
        localizedCaption = CxGlobalPlaceholderManager.Instance.ReplacePlaceholders(localizedCaption);
        return localizedCaption;
      }
      else if (!string.IsNullOrEmpty(originalCaption))
      {
        originalCaption = CxGlobalPlaceholderManager.Instance.ReplacePlaceholders(originalCaption);
        return originalCaption;
      }
      else
      {
        return GetCaption(entityMetadata);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns form caption in context of the given entity metadata.
    /// </summary>
    /// <param name="entityMetadata">entity metadata context</param>
    /// <returns>form caption</returns>
    public string GetFormCaption(CxEntityMetadata entityMetadata)
    {
      if (Contains("form_caption"))
      {
        return this["form_caption"];
      }
      else
      {
        return GetCaption(entityMetadata);
      }
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Hyperlink related stuff
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the attribute is a hyperlink.
    /// </summary>
    public bool IsHyperLink
    {
      get
      {
        return Type == TYPE_LINK &&
               (CxUtils.NotEmpty(HyperLinkEntityUsageId) || CxUtils.NotEmpty(HyperLinkEntityUsageAttrId));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Command ID to perform when hyperlink control is clicked (only for hyperlink type).
    /// </summary>
    public string HyperLinkCommandId
    {
      get
      {
        if (Holder.Config != null)
        {
          if (Holder.Config.ApplicationScope == NxApplicationScope.Windows &&
              CxUtils.NotEmpty(this["hyperlink_windows_command_id"]))
          {
            return this["hyperlink_windows_command_id"];
          }
          if (Holder.Config.ApplicationScope == NxApplicationScope.Web &&
              CxUtils.NotEmpty(this["hyperlink_web_command_id"]))
          {
            return this["hyperlink_web_command_id"];
          }
        }
        return this["hyperlink_command_id"];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage ID for the hyperlink command.
    /// </summary>
    public string HyperLinkEntityUsageId
    { get { return this["hyperlink_entity_usage_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage ID for the hyperlink command.
    /// </summary>
    public string HyperLinkEntityUsageAttrId
    { get { return this["hyperlink_entity_usage_attr_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage for the hyperlink command.
    /// </summary>
    public CxEntityUsageMetadata HyperLinkEntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(HyperLinkEntityUsageId) ? Holder.EntityUsages[HyperLinkEntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates is hyperlink displayed when attribute value is null.
    /// </summary>
    public bool HyperLinkIgnoreNull
    {
      get
      {
        if (CxUtils.IsEmpty(this["hyperlink_ignore_null"]) &&
            (CxUtils.NotEmpty(HyperLinkEntityUsageId) || CxUtils.NotEmpty(HyperLinkEntityUsageAttrId)))
        {
          return true;
        }
        return CxBool.Parse(this["hyperlink_ignore_null"], false);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if this record should be refreshed when hyperlink referenced record is changed.
    /// </summary>
    public bool HyperLinkAutoRefresh
    {
      get { return CxBool.Parse(this["hyperlink_auto_refresh"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if the content of the field should be automatically replaced with an Xml structure having
    /// all the necessary info for the hyperlink functioning.
    /// Should be applied just to "text" attributes. ('cause the value attributes do not 
    /// support non-int values, as a rule).
    /// </summary>
    public bool HyperLinkComposeXml
    {
      get { return CxBool.Parse(this["hyperlink_compose_xml"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if hyperlink should contain small icon URL redirecting by GET
    /// request to the command target page opening in a separate popup window
    /// to avoid extra postback. Is applicable to web applications only.
    /// </summary>
    public bool HyperLinkShowInPopup
    {
      get { return CxBool.Parse(this["hyperlink_show_in_popup"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if hyperlink should contain small icon URL redirecting by GET
    /// request to the command target page to avoid extra postback. 
    /// Is applicable to web applications only.
    /// </summary>
    public bool HyperLinkShowGetUrl
    {
      get { return CxBool.Parse(this["hyperlink_show_get_url"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if hyperlink URL redirects by GET request to the command target 
    /// page to avoid extra postback. Is applicable to web applications only.
    /// </summary>
    public bool HyperLinkShowGetUrlOnly
    {
      get { return CxBool.Parse(this["hyperlink_show_get_url_only"], false); }
    }

    //-------------------------------------------------------------------------
    #endregion

    #region Lookup related stuff
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of row source for combobox and lookup controls.
    /// </summary>
    public string RowSourceId
    {
      get { return this["row_source_id"]; }
      set
      {
        this["row_source_id"] = value;
        m_RowSource_Cache = null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Volatile filter part for the lookup control row source (can depend on parameters).
    /// </summary>
    public string RowSourceVolatileFilter
    {
      get { return this["row_source_filter"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Volatile filter part for the lookup control row source to join by OR operator.
    /// </summary>
    public string RowSourceVolatileOrFilter
    {
      get { return this["row_source_or_filter"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constant part (if present) of the filter for the lookup control row source.
    /// </summary>
    public string RowSourceConstantFilter
    {
      get { return this["row_source_constant_filter"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constant filter part for the lookup control row source to join by OR operator.
    /// </summary>
    public string RowSourceConstantOrFilter
    {
      get { return this["row_source_constant_or_filter"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns complete row source filter combined from the 
    /// RowSourceVolatileFilter and RowSourceConstantFilter properties.
    /// </summary>
    public string RowSourceFilter
    {
      get
      {
        return CxUtils.Nvl(CxData.ComposeWhereClause(
          NxBooleanBinaryOperator.Or,
          CxData.ComposeWhereClause(RowSourceVolatileFilter, RowSourceConstantFilter),
          CxData.ComposeWhereClause(RowSourceVolatileOrFilter, RowSourceConstantOrFilter)));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If value provider is specified, returns complete row source filter.
    /// If value provider is not specified, returns constant part of filter only.
    /// </summary>
    /// <param name="valueProvider">value provider to check presence of</param>
    public string GetRowSourceFilter(IxValueProvider valueProvider)
    {
      return valueProvider != null ?
        RowSourceFilter :
        CxUtils.Nvl(CxData.ComposeWhereClause(
                      NxBooleanBinaryOperator.Or, RowSourceConstantFilter, RowSourceConstantOrFilter));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row source for combobox and lookup controls.
    /// </summary>
    public CxRowSourceMetadata RowSource
    {
      get
      {
        if (m_RowSource_Cache != null)
          return m_RowSource_Cache;

        string rowSourceId = RowSourceId;
        return m_RowSource_Cache = (CxUtils.NotEmpty(rowSourceId) ? Holder.RowSources[rowSourceId] : null);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this attribute has row source taken from database.
    /// </summary>
    public bool IsDynamicLookup
    { get { return RowSource != null && !RowSource.HardCoded; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this attribute has row source taken from database
    /// and has row source constant or volatile filter.
    /// </summary>
    public bool IsFilteredLookup
    { get { return IsDynamicLookup && CxUtils.NotEmpty(RowSourceFilter); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this attribute has row source taken from database
    /// and has row source volatile filter (dependent from other attribute values).
    /// </summary>
    public bool IsParamFilteredLookup
    {
      get
      {
        return IsDynamicLookup &&
               (CxUtils.NotEmpty(RowSourceVolatileFilter) || CxUtils.NotEmpty(RowSourceVolatileOrFilter));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this attribute is multiple value lookup.
    /// </summary>
    public bool IsMultiValueLookup
    {
      get { return RowSource != null && WinControl == CxWinControlNames.WIN_CONTROL_LOOKUP_MULTI; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this attribute is hierarchical lookup.
    /// </summary>
    public bool IsHierarchicalLookup
    {
      get
      {
        return IsDynamicLookup &&
               RowSource.IsHierarchical &&
               RowSource.EntityUsage.IsSelfReferencing;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this attribute has huge row source that cannot be 
    /// selected from the database without filtering (due to performance).
    /// </summary>
    public bool IsHugeLookup
    { get { return IsDynamicLookup && RowSource.IsHuge; } }

    //-------------------------------------------------------------------------
    #endregion

    #region Reference related stuff
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ids of the entity usages this attribute references to.
    /// </summary>
    public string[] ReferenceEntityUsageIds
    {
      get
      {
        string idsText = this["reference_entity_usage_ids"];
        IList<string> ids = CxText.DecomposeWithSeparator(idsText, ",");
        string hyperlinkEntityUsageId = HyperLinkEntityUsageId;
        if (!ids.Contains(hyperlinkEntityUsageId))
          ids.Add(hyperlinkEntityUsageId);
        List<string> result = new List<string>();
        for (int i = 0; i < ids.Count; i++)
        {
          string id = ids[i];
          if (!string.IsNullOrEmpty(id))
            result.Add(id);
        }
        return result.ToArray();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity usages metadata this attribute references to.
    /// </summary>
    public CxEntityUsageMetadata[] ReferenceEntityUsages
    {
      get
      {
        if (!m_IsReferenceEntityUsagesCached)
        {
          List<CxEntityUsageMetadata> entityUsages = new List<CxEntityUsageMetadata>();
          foreach (string referenceEntityUsageId in ReferenceEntityUsageIds)
          {
            CxEntityUsageMetadata entityUsage = Holder.EntityUsages[referenceEntityUsageId];
            entityUsages.Add(entityUsage);
          }
          m_ReferenceEntityUsages = entityUsages.ToArray();
          m_IsReferenceEntityUsagesCached = true;
        }
        return m_ReferenceEntityUsages;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an attribute id this attribute references to.
    /// </summary>
    public string ReferenceAttributeId
    {
      get { return this["reference_attribute_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an id of the value attribute to get referenced entity by.
    /// </summary>
    public string ReferenceValueAttributeId
    {
      get { return this["reference_value_attribute_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a value attribute to get referenced entity by.
    /// </summary>
    public CxAttributeMetadata ReferenceValueAttribute
    {
      get
      {
        string referenceValueAttributeId = ReferenceValueAttributeId;
        if (!string.IsNullOrEmpty(referenceValueAttributeId))
          return EntityMetadata.GetAttribute(referenceValueAttributeId);
        else
          return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the attribute metadata referenced to by the current attribute,
    /// in the context of the given entity usage.
    /// </summary>
    /// <param name="referenceEntityUsage">a context of referenced entity usage</param>
    /// <returns></returns>
    public CxAttributeMetadata GetReferenceAttribute(CxEntityUsageMetadata referenceEntityUsage)
    {
      if (referenceEntityUsage != null)
      {
        string referenceAttributeId = ReferenceAttributeId;
        if (!string.IsNullOrEmpty(referenceAttributeId))
          return referenceEntityUsage.GetAttribute(referenceAttributeId);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Specifies if the entity attribute should be refreshed when some referenced entity
    /// has been changed.
    /// </summary>
    public bool ReferenceAutoRefresh
    {
      get { return CxBool.Parse(this["reference_auto_refresh"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of entity usages which are referenced by attribute properties.
    /// </summary>
    /// <returns>list of CxEntityMetadata objects or null</returns>
    override public IList<CxEntityMetadata> GetReferencedEntities()
    {
      UniqueList<CxEntityMetadata> result = new UniqueList<CxEntityMetadata>();

      if (RowSource != null && !RowSource.HardCoded)
        result.Add(RowSource.EntityUsage);

      result.Add(GetParentExpressionEntity(Default));
      result.Add(GetParentExpressionEntity(FilterDefault1));
      result.Add(GetParentExpressionEntity(FilterDefault2));
      result.Add(FileLibraryEntityUsage);

      return result;
    }

    //----------------------------------------------------------------------------
    #endregion

    #region Filtering related stuff
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if grid column may be filtered.
    /// </summary>
    public bool GridFiltered
    {
      get { return CxBool.Parse(this["grid_filtered"], true); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute should present on a find form.
    /// </summary>
    public bool Filterable
    {
      get
      {
        if (Holder.IsCurrentScope(NxApplicationScope.Web) && (Type == TYPE_ICON || IsDbFile))
        {
          return false;
        }
        string strValue = this["filterable"].ToLower();
        if (strValue == "true")
        {
          return true;
        }
        return Visible && strValue != "false";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute should allow local filter.
    /// </summary>
    public bool WinFilterableLocal
    {
      get { return (this["win_filterable_local"].ToLower() != "false"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of control used on the windows filter form.
    /// </summary>
    public string WinFilterControl
    {
      get
      {
        string result = this["win_filter_control"].ToLower();
        if (CxUtils.IsEmpty(result))
        {
          if (Type == TYPE_DATETIME)
          {
            return CxWinControlNames.WIN_CONTROL_DATE;
          }
          result = WinControl;
        }
        return result;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of control used on the WEB find form.
    /// </summary>
    public string WebFilterControl
    {
      get
      {
        string result = this["web_filter_control"].ToLower();
        if (CxUtils.IsEmpty(result))
        {
          if (Type == TYPE_DATETIME)
          {
            return WEB_CONTROL_DATE;
          }
          result = WebControl;
        }
        return result;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the parent control to put windows filter control on.
    /// </summary>
    public string WinFilterControlPlacement
    { get { return this["win_filter_control_placement"]; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the parent control to put web filter control on.
    /// </summary>
    public string WebFilterControlPlacement
    { get { return this["web_filter_control_placement"]; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns first default value of filter
    /// </summary>
    public string FilterDefault1
    {
      get
      {
        string result = this["filter_default_1"];
        if (CxUtils.IsEmpty(result))
        {
          result = this["filter_default"];
        }
        return result;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns second default value of filter
    /// </summary>
    public string FilterDefault2
    {
      get
      {
        return this["filter_default_2"];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default filter operation that will be selected by default.
    /// </summary>
    public string FilterOperation
    {
      get
      {
        return this["filter_operation"];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns custom filter condition defined by the developer.
    /// </summary>
    public string FilterCondition
    {
      get
      {
        return this["filter_condition"];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns string with the list of all enabled filter operation names.
    /// </summary>
    public string EnabledFilterOperations
    {
      get
      {
        return this["enabled_filter_operations"];
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True, if attribute should be mandatory on the filter form.
    /// </summary>
    public bool FilterMandatory
    { get { return this["filter_mandatory"].ToLower() == "true"; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True, if attribute should be placed on the advanced filter panel.
    /// </summary>
    public bool FilterAdvanced
    {
      get { return CxBool.Parse(this["filter_advanced"], false); }
      set { this["filter_advanced"] = value.ToString(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True, if 'Myself' filter operation should be applicable.
    /// Can be applied to filter by user ID. 
    /// 'Myself' means filtering by the current user ID.
    /// </summary>
    public bool FilterMyself
    { get { return this["filter_myself"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if control should be placed on the new line of the filter form.
    /// </summary>
    public bool IsPlacedOnNewLineInFilter
    { get { return this["new_line_filter"].ToLower() == "true"; } }
    //----------------------------------------------------------------------------
    #endregion

    #region File related stuff
    //----------------------------------------------------------------------------
    /// <summary>
    /// Id of the attribute responsible for storing the name of the blob-file uploaded
    /// into the current attribute.
    /// </summary>
    public string BlobFileNameAttributeId
    {
      get { return this["blob_file_name_attribute_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute responsible for storing the name of the blob-file uploaded
    /// into the current attribute.
    /// </summary>
    public CxAttributeMetadata BlobFileNameAttribute
    {
      get { return EntityMetadata.GetAttribute(BlobFileNameAttributeId); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Id of the attribute responsible for storing the size of the blob-file uploaded
    /// into the current attribute.
    /// </summary>
    public string BlobFileSizeAttributeId
    {
      get { return this["blob_file_size_attribute_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute responsible for storing the size of the blob-file uploaded
    /// into the current attribute.
    /// </summary>
    public CxAttributeMetadata BlobFileSizeAttribute
    {
      get { return EntityMetadata.GetAttribute(BlobFileSizeAttributeId); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of entity usage that is used as file/image library.
    /// </summary>
    public string FileLibraryEntityUsageId
    {
      get
      {
        string id = this["file_lib_entity_usage_id"];
        return CxUtils.NotEmpty(id) ? id : Holder.DefaultFileLibraryEntityUsageId;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage that is used as file/image library.
    /// </summary>
    public CxEntityUsageMetadata FileLibraryEntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(FileLibraryEntityUsageId) ?
          Holder.EntityUsages[FileLibraryEntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of attribute that contains file/image binary data.
    /// </summary>
    public string FileContentAttributeId
    {
      get
      {
        return Contains("file_content_attribute_id") ? this["file_content_attribute_id"] : Id;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute that contains file/image binary data.
    /// </summary>
    public CxAttributeMetadata GetFileContentAttribute(
      CxEntityUsageMetadata entityUsage)
    {
      return CxUtils.NotEmpty(FileContentAttributeId) ?
        entityUsage.GetAttribute(FileContentAttributeId) : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image. Group ID to open default file library group (category).
    /// </summary>
    public string FileLibraryCategoryCode
    { get { return this["file_lib_category_code"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image. ID of attribute that contains 
    /// group ID to open default file library group (category).
    /// </summary>
    public string FileLibraryCategoryCodeAttributeId
    { get { return this["file_lib_category_code_attribute_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For file/image. If true, then do not show 'Upload File' section
    /// </summary>
    public bool FileLibraryHideUploadFileSection
    { get { return (this["file_lib_hide_upload_file_section"].ToLower() == "true"); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if attribute represents file stored in database table BLOB field.
    /// </summary>
    public bool IsDbFile
    {
      get
      {
        string type = Type;
        return type == TYPE_FILE || type == TYPE_IMAGE;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of attribute that contains a foreign key reference to file/image library.
    /// </summary>
    public string FileLibraryReferenceAttributeId
    { get { return this["file_lib_reference_attribute_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute that contains a foreign key reference to file/image library.
    /// </summary>
    public CxAttributeMetadata GetFileLibraryReferenceAttribute(
      CxEntityUsageMetadata entityUsage)
    {
      return CxUtils.NotEmpty(FileLibraryReferenceAttributeId) ?
        entityUsage.GetAttribute(FileLibraryReferenceAttributeId) : null;
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Text-value stuff
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached text attribute for the current value attribute
    /// </summary>
    /// <param name="metadata">entity metadata</param>
    /// <returns>cached text attribute</returns>
    internal protected CxAttributeMetadata GetCachedTextAttribute(CxEntityMetadata metadata)
    {
      CxAttributeMetadata attr;
      if (metadata != null && m_TextAttributes.TryGetValue(metadata, out attr))
      {
        return attr;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets cached text attribute.
    /// </summary>
    /// <param name="metadata">entity metadata context</param>
    /// <param name="textAttr">text attribute to set cache for</param>
    internal protected void SetCachedTextAttribute(
      CxEntityMetadata metadata, CxAttributeMetadata textAttr)
    {
      if (metadata != null)
      {
        m_TextAttributes[metadata] = textAttr;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached value attribute for the current text attribute
    /// </summary>
    /// <param name="metadata">entity metadata</param>
    /// <returns>cached value attribute</returns>
    internal protected CxAttributeMetadata GetCachedValueAttribute(CxEntityMetadata metadata)
    {
      CxAttributeMetadata attr;
      if (metadata != null && m_ValueAttributes.TryGetValue(metadata, out attr))
      {
        return attr;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets cached value attribute.
    /// </summary>
    /// <param name="metadata">entity metadata context</param>
    /// <param name="valueAttr">value attribute to set cache for</param>
    internal protected void SetCachedValueAttribute(
      CxEntityMetadata metadata, CxAttributeMetadata valueAttr)
    {
      if (metadata != null)
      {
        m_ValueAttributes[metadata] = valueAttr;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached text defined attribute for the current value attribute
    /// </summary>
    /// <param name="metadata">entity metadata</param>
    /// <returns>cached text attribute</returns>
    internal protected CxAttributeMetadata GetCachedTextDefinedAttribute(CxEntityMetadata metadata)
    {
      CxAttributeMetadata attr;
      if (metadata != null && m_TextDefinedAttributes.TryGetValue(metadata, out attr))
      {
        return attr;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets cached text defined attribute.
    /// </summary>
    /// <param name="metadata">entity metadata context</param>
    /// <param name="textDefinedAttr">text attribute</param>
    internal protected void SetCachedTextDefinedAttribute(
      CxEntityMetadata metadata, CxAttributeMetadata textDefinedAttr)
    {
      if (metadata != null)
      {
        m_TextDefinedAttributes[metadata] = textDefinedAttr;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached value defined attribute for the current text attribute
    /// </summary>
    /// <param name="metadata">entity metadata</param>
    /// <returns>cached value attribute</returns>
    internal protected CxAttributeMetadata GetCachedValueDefinedAttribute(
      CxEntityMetadata metadata)
    {
      CxAttributeMetadata attr;
      if (metadata != null && m_ValueDefinedAttributes.TryGetValue(metadata, out attr))
      {
        return attr;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets cached value defined attribute.
    /// </summary>
    /// <param name="metadata">entity metadata context</param>
    /// <param name="valueDefinedAttr">value attribute</param>
    internal protected void SetCachedValueDefinedAttribute(
      CxEntityMetadata metadata, CxAttributeMetadata valueDefinedAttr)
    {
      if (metadata != null)
      {
        m_ValueDefinedAttributes[metadata] = valueDefinedAttr;
      }
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Dependency related stuff
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of dependency parameter names used in the attribute expressions.
    /// </summary>
    public IList<string> GetDependencyParamNames(ICollection expressionList)
    {
      UniqueList<string> list = new UniqueList<string>();
      foreach (string expression in expressionList)
      {
        list.AddRange(CxDbParamParser.GetList(expression, true));
      }
      return list;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of dependency parameter names used in the attribute expressions.
    /// </summary>
    public IList<string> DependencyParamNames
    {
      get
      {
        return GetDependencyParamNames(new string[] { RowSourceFilter, LocalExpression });
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of dependency parameter names used in the 
    /// visibility or readonly attribute expressions.
    /// </summary>
    public IList<string> DependencyStateParamNames
    {
      get
      {
        return GetDependencyParamNames(new string[] { VisibilityCondition, ReadOnlyCondition });
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of dependency parameter names used in the mandatory expressions.
    /// </summary>
    public IList<string> DependencyMandatoryParamNames
    {
      get
      {
        return GetDependencyParamNames(new string[] { MandatoryCondition });
      }
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Other stuff
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute type.
    /// </summary>
    public string Type
    {
      get { return this["type"].ToLower(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute default expression.
    /// </summary>
    public string Default
    {
      get { return this["default"]; }
      set { this["default"] = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute paste expression.
    /// </summary>
    public string PasteDefault
    {
      get { return this["paste_default"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute control.
    /// </summary>
    public string WinControl
    {
      get
      {
        string control = this["win_control"].ToLower();
        if (CxUtils.IsEmpty(control))
        {
          control = GetDefaultWinControl();
        }
        return control;
      }
      set { this["win_control"] = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of control used on the WEB edit form.
    /// </summary>
    public string WebControl
    {
      get
      {
        string result = this["web_control"].ToLower();
        if (CxUtils.IsEmpty(result))
        {
          result = GetDefaultWebControl();
        }
        return result;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns horizontal text alignment.
    /// </summary>
    public NxTextHorzAlignment Alignment
    {
      get
      {
        string strAlign = this["alignment"];
        if (CxUtils.NotEmpty(strAlign))
        {
          return (NxTextHorzAlignment) Enum.Parse(typeof(NxTextHorzAlignment), strAlign, true);
        }
        else if ((Type == TYPE_INT || Type == TYPE_FLOAT) && string.IsNullOrEmpty(TextAttributeId) && string.IsNullOrEmpty(RowSourceId))
        {
          return NxTextHorzAlignment.Right;
        }
        else
        {
          return NxTextHorzAlignment.None;
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Width (in columns) of an attribute control.
    /// </summary>
    public int ControlWidth
    {
      get { return CxInt.Parse(this["control_width"], 0); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Height (in lines) of an attribute control (for memo controls).
    /// </summary>
    public int ControlHeight
    {
      get { return CxInt.Parse(this["control_height"], 0); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The fixed column style in the grid.
    /// </summary>
    public NxColumnFixed GridColumnFixedByDefault
    {
      get
      {
        string value = this["grid_column_fixed_by_default"];
        if (string.Equals(value, "left", StringComparison.OrdinalIgnoreCase))
          return NxColumnFixed.Left;
        else if (string.Equals(value, "right", StringComparison.OrdinalIgnoreCase))
          return NxColumnFixed.Right;
        else
          return NxColumnFixed.None;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute control modifiers.
    /// </summary>
    public string ControlModifiers
    {
      get { return this["control_modifiers"].ToUpper(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Host control the attribute control should be placed on.
    /// </summary>
    public string WinControlPlacement
    {
      get { return this["win_control_placement"]; }
      set { this["win_control_placement"] = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the parent control to put web control on.
    /// </summary>
    public string WebControlPlacement
    { get { return this["web_control_placement"]; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the parent control to put silverlight control on.
    /// </summary>
    public string SlControlPlacement
    { get { return this["sl_control_placement"]; } }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute value should be initialized with 
    /// default row source value on entity creation if attribute is mandatory.
    /// </summary>
    public bool InitWithRowSourceDefaultValue
    {
      get
      {
        return CxBool.Parse(this["init_with_row_source_default_value"], true);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute should present on the entity customization form.
    /// The customization form allows hiding/showing attributes in entities from
    /// user interface by application user.
    /// </summary>
    public bool IsCustomizable
    {
      get
      {
        return this["customizable"].ToLower() != "false";
      }
      set { this["customizable"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if seconds should be displayed for date-time field.
    /// </summary>
    public bool AreSecondsDisplayed
    {
      get
      {
        return CxBool.Parse(this["display_seconds"], true);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the attribute to sort dataset when sorting is applied to the current attribute.
    /// </summary>
    public string SortByAttributeId
    {
      get { return this["sort_by_attr_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of the attribute to sort dataset when sorting is applied to the current attribute.
    /// </summary>
    public string GroupByAttributeId
    {
      get { return this["group_by_attr_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Class to perform text scrambling for the scrambled_text web controls.
    /// </summary>
    public string ScramblerClassCd
    {
      get { return this["scrambler_class_cd"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns scrambler class metadata.
    /// </summary>
    public CxClassMetadata ScramblerClass
    {
      get
      {
        return !String.IsNullOrEmpty(ScramblerClassCd) ? Holder.Classes[ScramblerClassCd] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Method from Scrambled class to perform text scrambling for the scrambled_text web controls.
    /// </summary>
    public string ScramblerMethod
    {
      get { return this["scrambler_method"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Client-side processing of automatic numeration.
    /// If true, fills field of the new created entity with incremented max value.
    /// Max value is calculated among currently present in memory child entities,
    /// then max value incremented and assigned to the entity field.
    /// Works for Windows only. Is used for an entity dialog child grids.
    /// </summary>
    public bool IncrementOnCreate
    {
      get { return (this["increment_on_create"].ToLower() == "true"); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns ID of select form class (for windows lookup select forms).
    /// </summary>
    public string SelectFormClassId
    { get { return this["select_form_class_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns metadata of select form class (for windows lookup select forms).
    /// </summary>
    public CxClassMetadata SelectFormClass
    {
      get
      {
        return CxUtils.NotEmpty(SelectFormClassId) ? Holder.Classes[SelectFormClassId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Lookup popup form width (for Windows)
    /// </summary>
    public int LookupWidth
    { get { return CxInt.Parse(this["lookup_width"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if button edit text should be editable.
    /// </summary>
    public bool IsButtonEditTextEditable
    { get { return this["button_edit_text_editable"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the image to display on the additional editor button (Windows).
    /// </summary>
    public string ButtonImageId
    { get { return this["button_image_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// False to hide additional editor button (Windows).
    /// </summary>
    public bool IsButtonVisible
    { get { return this["button_visible"].ToLower() != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True to hide additional editor button in edit (not new) mode (Windows).
    /// I.e., button is available for new records only.
    /// </summary>
    public bool IsButtonHiddenForUpdate
    { get { return this["button_hidden_for_update"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command button should be ellipsis instead of glyph.
    /// </summary>
    public bool IsButtonEllipsis
    { get { return this["button_ellipsis"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns index of the DB object to store this attribute in.
    /// </summary>
    public int DbObjectIndex
    { get { return CxInt.Parse(this["db_object_index"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the command to execute on additional editor button click (Windows).
    /// </summary>
    public string ButtonCommandId
    { get { return this["button_command_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the entity usage to get command on additional editor button click (Windows).
    /// </summary>
    public string ButtonCommandEntityUsageId
    { get { return this["button_command_entity_usage_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Id of the image to be displayed in the column header.
    /// </summary>
    public string ColumnHeaderImageId
    { get { return this["column_header_image_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The image to be displayed in the column header.
    /// </summary>
    public CxImageMetadata ColumnHeaderImage
    {
      get
      {
        return CxUtils.NotEmpty(ColumnHeaderImageId) ? Holder.Images[ColumnHeaderImageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage metadata to get command on additional editor button click (Windows).
    /// </summary>
    public CxEntityUsageMetadata ButtonCommandEntityUsage
    {
      get
      {
        return CxUtils.NotEmpty(ButtonCommandEntityUsageId) ? Holder.EntityUsages[ButtonCommandEntityUsageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field name to compare with filter value.
    /// </summary>
    public string FilterSearchField
    { get { return this["filter_search_field"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns database object to search while filtering by this attribute.
    /// </summary>
    public string FilterSearchObject
    { get { return this["filter_search_object"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns condition to join filter search object.
    /// </summary>
    public string FilterSearchObjectJoin
    { get { return this["filter_search_object_join"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if table should be filtered by custom external DB object.
    /// </summary>
    public bool IsFilteredByCustomObject
    {
      get
      {
        return CxUtils.NotEmpty(FilterSearchObject) &&
               CxUtils.NotEmpty(FilterSearchObjectJoin);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, column content will be wrapped in the grid cell.
    /// </summary>
    public bool GridWordWrap
    { get { return this["grid_word_wrap"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if grid_word_wrap property is specified.
    /// </summary>
    public bool IsGridWordWrapSpecified
    { get { return CxUtils.NotEmpty(this["grid_word_wrap"]); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// If true, control value on the edit form will be always calculated
    /// by the LocalExpression.
    /// </summary>
    public bool IsCalculated
    { get { return this["is_calculated"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns minimum relative year value for date control.
    /// </summary>
    public int MinYearValue
    {
      get
      {
        return CxInt.Parse(this["min_year_value"], -10);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns maximum relative year value for date control.
    /// </summary>
    public int MaxYearValue
    {
      get
      {
        return CxInt.Parse(this["max_year_value"], 10);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the current attribute should be present in the list
    /// of fields available for lookup settings customizing.
    /// </summary>
    public bool CustomizableLookup
    {
      get { return CxBool.Parse(this["customizable_lookup"], false); }
      set { this["customizable_lookup"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines mandatory condition of the attribute.
    /// </summary>
    public string MandatoryCondition
    { get { return this["mandatory_condition"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns order of calculation default value within entity fields.
    /// </summary>
    public int DefaultValueCalculationOrder
    {
      get
      {
        return CxInt.Parse(this["default_value_calculation_order"], -1);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Retruns ID of attribute containing text description for lookup columns.
    /// Must be used with non-empty row_source_id when row source is taken from
    /// database (from entity usage).
    /// </summary>
    public string TextAttributeId
    { get { return this["text_attr_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Map with attributes to get from lookup datasource and put to teh current grid/edit form.
    /// </summary>
    public Hashtable AttributesToGetFromLookup
    {
      get
      {
        if (m_AttributesToGetFromLookup == null)
        {
          m_AttributesToGetFromLookup = new Hashtable();
          string s = this["attributes_to_get_from_lookup"];
          IList<string> pairs = CxText.DecomposeWithSeparator(s, ",");
          foreach (string pair in pairs)
          {
            string target = CxText.TrimSpace(CxText.SubstringBeforeSeparator(pair, "="));
            string source = CxText.TrimSpace(CxText.SubstringAfterSeparator(pair, "="));
            m_AttributesToGetFromLookup.Add(target, source);
          }
        }
        return m_AttributesToGetFromLookup;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// ID of the attribute that should contain value of combobox selected item.
    /// </summary>
    public string ValueAttributeId
    {
      get { return this["value_attribute_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// true if row source should be refreshed on lookup popup.
    /// </summary>
    public bool RefreshOnPopup
    {
      get { return (this["refresh_on_popup"].ToLower() == "true"); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the attribute can be involved into querying thru Query Builder UI.
    /// </summary>
    public bool IsQueryable
    { get { return this["is_queryable"] != "false"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default attribute sorting direction.
    /// </summary>
    public NxSortingDirection Sorting
    {
      get
      {
        return (this["sorting"].ToLower() == "asc" ? NxSortingDirection.Asc :
          this["sorting"].ToLower() == "desc" ? NxSortingDirection.Desc :
          NxSortingDirection.None);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sort order determines order of multiple sort keys in a final sort expression.
    /// </summary>
    public int SortOrder
    { get { return CxInt.Parse(this["sort_order"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute maximal length.
    /// </summary>
    public int MaxLength
    {
      get { return CxInt.Parse(this["max_length"], 0); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute scale (number of characters after decimal point).
    /// </summary>
    public int Scale
    {
      get { return CxInt.Parse(this["scale"], 0); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute default with in grid.
    /// </summary>
    public int GridWidth
    {
      get { return CxInt.Parse(this["grid_width"], -1); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute minimal value.
    /// </summary>
    public Decimal MinValue
    {
      get
      {
        return CxFloat.ParseDecimal(this["min_value"], Decimal.MinValue);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute maximal value.
    /// </summary>
    public Decimal MaxValue
    {
      get
      {
        return CxFloat.ParseDecimal(this["max_value"], Decimal.MaxValue);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if attribute is part of primary key.
    /// </summary>
    public bool PrimaryKey
    {
      get { return (this["primary_key"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if attribute is part of alternative unique key.
    /// </summary>
    public bool AlternativeKey
    {
      get { return (this["alternative_key"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Index of the alternative key (in a case of several alternative keys)
    /// </summary>
    public int AlternativeKeyIndex
    {
      get { return CxInt.Parse(this["alternative_key_index"], 0); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is nullable.
    /// </summary>
    public bool Nullable
    {
      get { return CxBool.Parse(this["nullable"], true); }
      set { this["nullable"] = value.ToString(); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the row-source list should contain an empty row for the null key value.
    /// </summary>
    public bool RowSourceHasEmptyRow
    {
      get { return CxBool.Parse(this["row_source_has_empty_row"], true); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is storable.
    /// </summary>
    public bool Storable
    {
      get { return (this["storable"].ToLower() != "false"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the grid column for this attribute should have the cell merging
    /// feature enabled.
    /// </summary>
    public bool GridCellMerging
    {
      get { return CxBool.Parse(this["grid_cell_merging"], false); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is editable.
    /// </summary>
    public bool Editable
    {
      get
      {
        string editable = this["editable"].ToLower();
        if (editable == "true")
        {
          return true;
        }
        return Visible && editable != "false";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is enabled.
    /// </summary>
    public bool Enabled
    {
      get { return (this["enabled"].ToLower() != "false"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is enabled.
    /// </summary>
    public bool ReadOnly
    {
      get
      {
        if (Type == TYPE_ICON || Type == TYPE_LINK)
        {
          return true;
        }
        return this["read_only"].ToLower() == "true";
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if attribute is read only in record update mode.
    /// </summary>
    public bool ReadOnlyForUpdate
    {
      get { return (this["read_only_for_update"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// True if attribute is read only in the editable grid.
    /// </summary>
    public bool ReadOnlyInGrid
    {
      get { return (this["read_only_in_grid"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is custom one.
    /// </summary>
    public bool Custom
    {
      get { return (this["custom"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if control for the attribute defined in the design time.
    /// </summary>
    public bool Manual
    {
      get { return (this["manual"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if only one row in the grid may has true in this boolean attribute.
    /// </summary>
    public bool OnlyOneSelected
    {
      get { return (this["only_one_selected"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if grid column width should be fixed.
    /// </summary>
    public bool GridWidthFixed
    {
      get { return (this["grid_width_fixed"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Definition of grid summary item for this attribute.
    /// </summary>
    public string GridSummary
    {
      get { return this["grid_summary"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Expression used to calculate value of this attribute when some 
    /// other attribute values change.
    /// </summary>
    public string LocalExpression
    {
      get { return this["local_expression"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Expression calculated before saving entity to the database.
    /// </summary>
    public string OnSaveExpression
    {
      get { return this["on_save_expression"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the attribute that contains definition for button edits.
    /// </summary>
    public string DefinitionAttributeId
    {
      get { return this["definition_attribute_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the definition class for button edits.
    /// </summary>
    public string DefinitionClassId
    {
      get { return this["definition_class_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the definition dialog class for button edits.
    /// </summary>
    public string DefinitionDialogClassId
    {
      get { return this["definition_dialog_class_id"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if this attribute shoudl not participate in entity comparisons.
    /// </summary>
    public bool Incomparable
    {
      get { return (this["incomparable"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Name of the color to show attribute grid column with.
    /// </summary>
    public string GridColor
    {
      get { return this["grid_color"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given attribute is a display name of the entity.
    /// </summary>
    public bool IsDisplayName
    { get { return this["display_name"].ToLower() == "true"; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For image type. Size of image thumbnail.
    /// </summary>
    public int ImageSmallThumbnailSize
    { get { return CxInt.Parse(this["image_small_thumbnail_size"], Holder.ImageSmallThumbnailSize); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// For image type. Size of image thumbnail.
    /// </summary>
    public int ImageLargeThumbnailSize
    { get { return CxInt.Parse(this["image_large_thumbnail_size"], Holder.ImageLargeThumbnailSize); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if control should be placed on the new line of the edit form.
    /// </summary>
    public bool IsPlacedOnNewLine
    {
      get { return CxBool.Parse(this["new_line"], false); }
      set { this["new_line"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines visibility condition of the attribute.
    /// </summary>
    public string VisibilityCondition
    { get { return this["visibility_condition"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines read-only state condition of the attribute.
    /// </summary>
    public string ReadOnlyCondition
    { get { return this["read_only_condition"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns expression that determines read-only state condition of the attribute.
    /// Is checked just if all the previous checks resulted in "readonly true". 
    /// Performed after the entity's "post-check" condition.
    /// </summary>
    public string ReadOnlyConditionPostCheck
    { get { return this["read_only_condition_post_check"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the image to be displayed instead of the default checkmark image for the
    /// TRUE boolean value in the field.
    /// </summary>
    public string BooleanTrueImageId
    {
      get { return this["boolean_true_image_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the image to be displayed instead of the default checkmark image for the
    /// FALSE boolean value in the field.
    /// </summary>
    public string BooleanFalseImageId
    {
      get { return this["boolean_false_image_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the image to be displayed instead of the default checkmark image for the
    /// NULL (not true and not false) boolean value in the field.
    /// </summary>
    public string BooleanNullImageId
    {
      get { return this["boolean_null_image_id"]; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata of the image to be displayed instead of the default checkmark image for the
    /// TRUE boolean value in the field.
    /// </summary>
    public CxImageMetadata BooleanTrueImage
    {
      get
      {
        if (!string.IsNullOrEmpty(BooleanTrueImageId))
          return Holder.Images.Find(BooleanTrueImageId);
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata of the image to be displayed instead of the default checkmark image for the
    /// FALSE boolean value in the field.
    /// </summary>
    public CxImageMetadata BooleanFalseImage
    {
      get
      {
        if (!string.IsNullOrEmpty(BooleanFalseImageId))
          return Holder.Images.Find(BooleanFalseImageId);
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata of the image to be displayed instead of the default checkmark image for the
    /// NULL boolean value in the field.
    /// </summary>
    public CxImageMetadata BooleanNullImage
    {
      get
      {
        if (!string.IsNullOrEmpty(BooleanNullImageId))
          return Holder.Images.Find(BooleanNullImageId);
        return null;
      }
    }
    //----------------------------------------------------------------------------
    #endregion

    //----------------------------------------------------------------------------
    /// <summary>
    /// Attribute ID as it was set in metadata.
    /// </summary>
    public string OriginalId
    {
      get { return m_OriginalId; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Entity this attribute belongs to.
    /// </summary>
    public CxEntityMetadata EntityMetadata
    {
      get { return m_EntityMetadata; }
      protected set { m_EntityMetadata = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default web control by the attribute properties.
    /// </summary>
    /// <returns>the name of web control type</returns>
    public string GetDefaultWebControl()
    {
      if (CxUtils.NotEmpty(RowSourceId))
      {
        return WEB_CONTROL_DROPDOWN;
      }
      switch (Type)
      {
        case TYPE_STRING: return WEB_CONTROL_TEXT;
        case TYPE_LONGSTRING: return WEB_CONTROL_TEXT;
        case TYPE_DATETIME: return WEB_CONTROL_DATETIME;
        case TYPE_DATE: return WEB_CONTROL_DATE;
        case TYPE_TIME: return WEB_CONTROL_TIME;
        case TYPE_INT: return WEB_CONTROL_INT;
        case TYPE_FLOAT: return WEB_CONTROL_NUMBER;
        case TYPE_BOOLEAN: return WEB_CONTROL_CHECKBOX;
        case TYPE_FILE: return WEB_CONTROL_FILE;
        case TYPE_IMAGE: return WEB_CONTROL_IMAGE;
        case TYPE_ICON: return WEB_CONTROL_ICON;
        case TYPE_LINK: return WEB_CONTROL_LINK;
      }
      return "";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default windows control by the attribute properties.
    /// </summary>
    /// <returns>the name of windows control type</returns>
    public string GetDefaultWinControl()
    {
      if (RowSource != null)
      {
        return RowSource.HardCoded ? CxWinControlNames.WIN_CONTROL_DROPDOWN : CxWinControlNames.WIN_CONTROL_LOOKUP;
      }
      switch (Type)
      {
        case TYPE_STRING: return CxWinControlNames.WIN_CONTROL_TEXT;
        case TYPE_LONGSTRING: return CxWinControlNames.WIN_CONTROL_TEXT;
        case TYPE_DATETIME: return CxWinControlNames.WIN_CONTROL_TEXT;
        case TYPE_DATE: return CxWinControlNames.WIN_CONTROL_DATE;
        case TYPE_TIME: return CxWinControlNames.WIN_CONTROL_TIME;
        case TYPE_INT: return CxWinControlNames.WIN_CONTROL_SPIN;
        case TYPE_FLOAT: return CxWinControlNames.WIN_CONTROL_CALC;
        case TYPE_BOOLEAN: return CxWinControlNames.WIN_CONTROL_CHECKBOX;
        case TYPE_FILE: return CxWinControlNames.WIN_CONTROL_FILE;
        case TYPE_IMAGE: return CxWinControlNames.WIN_CONTROL_IMAGE;
        case TYPE_ICON: return CxWinControlNames.WIN_CONTROL_DROPDOWNIMAGE;
        case TYPE_LINK: return CxWinControlNames.WIN_CONTROL_HYPERLINK;
      }
      return "";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the parent control depending 
    /// on the current application scope to put a control on.
    /// </summary>
    public string CurrentScopeControlPlacement
    {
      get
      {
        switch (Holder.Config.ApplicationScope)
        {
          case NxApplicationScope.Web:
            return WebControlPlacement;
          case NxApplicationScope.Windows:
            return WinControlPlacement;
          default:
            throw new ExException(
              "Cannot determine the current application scope " +
              "to get an appropriate control placement value");
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the control placement that should be used if
    /// no control placement set explicitly.
    /// </summary>
    public string DefaultControlPlacement
    {
      get
      {
        switch (Holder.Config.ApplicationScope)
        {
          case NxApplicationScope.Web:
            return CxTabMetadata.DEFAULT_TAB_ID;
          case NxApplicationScope.Windows:
            return CxWinTabMetadata.DEFAULT_PANEL_ID;
          default:
            throw new ExException(
              "Cannot determine the current application scope " +
              "to get an appropriate default control placement value");
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if attribute is entity user ID.
    /// </summary>
    public bool UserId
    {
      get { return IsDisplayName; /*(this["user_id"].ToLower() == "true");*/ }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Converts constant expression to the object of the corresponding type.
    /// </summary>
    /// <param name="constant">constant expression to convert</param>
    /// <returns>object of the corresponding type converted from the constant expression</returns>
    public object ConvertConstantToObject(string constant)
    {
      string t = Type.ToLower();
      if (t == TYPE_STRING || t == TYPE_LONGSTRING || t == TYPE_ICON || t == TYPE_LINK)
        return constant;
      else if (t == TYPE_DATETIME)
        return DateTime.ParseExact(constant, "yyyy-MM-dd HH:mm:ss", null);
      else if (t == TYPE_DATE)
        return DateTime.ParseExact(constant, "yyyy-MM-dd", null);
      else if (t == TYPE_TIME)
        return DateTime.ParseExact(constant, "HH:mm:ss", null);
      else if (t == TYPE_INT)
        return Convert.ToInt32(constant);
      else if (t == TYPE_FLOAT)
        return CxFloat.ParseFloatConst(constant);
      else if (t == TYPE_BOOLEAN)
        return CxBool.Parse(constant);
      else
        throw new ExMetadataException(string.Format("Invalid attribute type <{0}> " +
          "in attribute with ID=<{1}> " +
          "for entity with ID=<{2}>",
          Type, Id, EntityMetadata.Id));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns C# type of the property attribute correcponds to.
    /// </summary>
    /// <returns>C# type of the property attribute correcponds to</returns>
    public Type GetPropertyType()
    {
      string t = Type;
      return
        (t == TYPE_STRING ||
         t == TYPE_LONGSTRING ? typeof(string) :
         t == TYPE_DATETIME ||
         t == TYPE_DATE ||
         t == TYPE_TIME ? typeof(DateTime) :
         t == TYPE_INT ? typeof(int) :
         t == TYPE_FLOAT ? typeof(double) :
         t == TYPE_BOOLEAN ? typeof(bool) :
         t == TYPE_ICON ? typeof(string) :
         t == TYPE_LINK ? typeof(string) :
         typeof(object));
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// User-friendly metadata object caption.
    /// </summary>
    override public string Text
    { get { return Caption; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Checks attribute permission for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">parent entity usage</param>
    /// <param name="connection">Db connection to check security rule</param>
    /// <param name="entityValueProvider">entity instance value provider</param>
    /// <param name="permissionGroupId">ID of permission group to check</param>
    protected bool GetIsAllowed(
      CxEntityUsageMetadata entityUsage,
      CxDbConnection connection,
      IxValueProvider entityValueProvider,
      string permissionGroupId)
    {
      if (Holder != null && Holder.Security != null)
      {
        IxValueProvider provider = entityValueProvider != null ?
          entityUsage.PrepareValueProvider(entityValueProvider) : null;
        return Holder.Security.GetRight(
          this, entityUsage, permissionGroupId, connection, provider);
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is visible for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">parent entity usage</param>
    /// <param name="connection">Db connection to check security rule</param>
    /// <param name="entityValueProvider">entity instance value provider</param>
    public bool GetIsVisible(
      CxEntityUsageMetadata entityUsage,
      CxDbConnection connection,
      IxValueProvider entityValueProvider)
    {
      return GetIsAllowed(entityUsage, connection, entityValueProvider, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is visible for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">parent entity usage</param>
    public bool GetIsVisible(CxEntityUsageMetadata entityUsage)
    {
      return GetIsVisible(entityUsage, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is enabled for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">parent entity usage</param>
    /// <param name="connection">Db connection to check security rule</param>
    /// <param name="entityValueProvider">entity instance value provider</param>
    public bool GetIsEnabled(
      CxEntityUsageMetadata entityUsage,
      CxDbConnection connection,
      IxValueProvider entityValueProvider)
    {
      return GetIsAllowed(
        entityUsage, connection, entityValueProvider, CxSecurityMetadata.PERM_GROUP_EDIT);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if attribute is enabled for the given entity usage
    /// depending on security settings.
    /// </summary>
    /// <param name="entityUsage">parent entity usage</param>
    public bool GetIsEnabled(CxEntityUsageMetadata entityUsage)
    {
      return GetIsEnabled(entityUsage, null, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if additional editor button should be visible. (Windows)
    /// </summary>
    /// <param name="isNewEntityMode">new entity mode flag</param>
    public bool GetIsButtonVisible(bool isNewEntityMode)
    {
      return IsButtonVisible && CxUtils.NotEmpty(ButtonCommandId) &&
             (!IsButtonHiddenForUpdate || isNewEntityMode);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Metadata of the image to display on the additional editor button (Windows).
    /// </summary>
    public CxImageMetadata ButtonImage
    {
      get
      {
        return CxUtils.NotEmpty(ButtonImageId) ? Holder.Images[ButtonImageId] : null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if the attribute has memo (double-width) control.
    /// </summary>
    public bool IsMemoControl
    {
      get
      {
        return (Holder.IsCurrentScope(NxApplicationScope.Windows) &&
                (WinControl == CxWinControlNames.WIN_CONTROL_MEMO ||
                 WinControl == CxWinControlNames.WIN_CONTROL_HTML ||
                 WinControl == CxWinControlNames.WIN_CONTROL_IMAGE ||
                 WinControl == CxWinControlNames.WIN_CONTROL_IMAGE_HYBRID ||
                 WinControl == CxWinControlNames.WIN_CONTROL_RICHTEXT ||
                 WinControl == CxWinControlNames.WIN_CONTROL_MEMO_BUTTON_LABEL)) ||
               (Holder.IsCurrentScope(NxApplicationScope.Web) &&
                (WebControl == WEB_CONTROL_MEMO || WebControl == WEB_CONTROL_HTML));
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
        return OBJECT_TYPE_ATTRIBUTE;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return EntityMetadata != null ? EntityMetadata.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity referenced by the expression.
    /// Expression should be in format =PARENT.[EntityId | EntityUsageId].[AttributeId]
    /// </summary>
    public CxEntityMetadata GetParentExpressionEntity(string expression)
    {
      CxEntityMetadata entity = null;
      string entityId = null;
      expression = CxText.ToUpper(expression);
      if (expression.StartsWith("=PARENT."))
      {
        expression = expression.Substring("=PARENT.".Length);
        int dotIndex = expression.IndexOf(".");
        if (dotIndex > 0)
        {
          entityId = expression.Substring(0, dotIndex);
        }
      }
      if (CxUtils.NotEmpty(entityId))
      {
        entity = Holder.EntityUsages.Find(entityId) ?? Holder.Entities.Find(entityId);
      }
      return entity;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns parent entity.
    /// </summary>
    override public CxMetadataObject ParentObject
    {
      get { return EntityMetadata; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Defines the attribute in the destination entity usage 
    /// if it isn't defined yet and applies attribute's 
    /// initial and current values.
    /// </summary>
    /// <param name="entityUsage">a destination entity usage</param>
    public CxAttributeUsageMetadata ApplyToEntityUsage(CxEntityUsageMetadata entityUsage)
    {
      CxAttributeUsageMetadata attributeUsage;
      IDictionary<string, string> targetInitialValues;
      IDictionary<string, string> sourceInitialValues = GetInitialPropertyValues();
      if (entityUsage.IsAttributeDefined(Id))
      {
        attributeUsage =
          (CxAttributeUsageMetadata) entityUsage.GetAttribute(Id);
        targetInitialValues = attributeUsage.GetInitialPropertyValues();
      }
      else
      {
        targetInitialValues = GetInitialPropertyValues();
        attributeUsage =
          (CxAttributeUsageMetadata) entityUsage.DefineEntityAttribute(Id, targetInitialValues);
      }

      List<string> propertiesToOverride = new List<string>();
      foreach (string property in PropertyNames)
      {
        if (property != idAttribute
          && (!sourceInitialValues.ContainsKey(property) || sourceInitialValues[property] != this[property])
          && (!targetInitialValues.ContainsKey(property)
          || (!targetInitialValues.ContainsKey(property) && !sourceInitialValues.ContainsKey(property))
          || (sourceInitialValues.ContainsKey(property) && targetInitialValues.ContainsKey(property) && targetInitialValues[property] == sourceInitialValues[property])))
        {
          propertiesToOverride.Add(property);
        }
      }
      foreach (string property in propertiesToOverride)
      {
        attributeUsage[property] = GetNonLocalizedPropertyValue(property);
      }
      return attributeUsage;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if attribute value should be scrambled.
    /// </summary>
    public bool IsScrambled
    {
      get { return WebControl == WEB_CONTROL_SCRAMBLED_TEXT; }
    }
        //-------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        
        public string JsControlCssClass
        {
            get
            {
                return this["js_control_css_class"];
            }
        }

        public bool Autofilter
        {
            get
            {
                return CxBool.Parse( this["autofilter"], false);
            }
        }


        public int ThumbnailWidth
        {
            get
            {
                return CxInt.Parse(this["thumbnail_width"], 50);
            }
        }

        public int ThumbnailHeight
        {
            get
            {
                return CxInt.Parse(this["thumbnail_height"], 50);
            }
        }

        public bool ShowTotal
        {
            get { return CxBool.Parse(this["show_total"], false); }
        }

        public string CalcTotalJs
        {
            get { return this["calc_total_js"]; }
        }

        public string TotalText
        {
            get { return this["total_text"]; }
        }

    }
}