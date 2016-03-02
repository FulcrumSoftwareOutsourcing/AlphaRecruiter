/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
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
using System.Runtime.Serialization;

using Framework.Db;
using Framework.Metadata;

namespace Framework.Remote
{

  [DataContract]
  public sealed class CxClientEntityMetadata : IxErrorContainer
  {
    [DataMember]
    public readonly string EntityId;

    [DataMember]
    public readonly string Id;

    [DataMember]
    public readonly string SingleCaption;

    [DataMember]
    public readonly string PluralCaption;

    [DataMember]
    public readonly string FrameClassId;

    [DataMember]
    public readonly string ClientClassId;

    [DataMember]
    public readonly bool SlFilterOnStart;

    [DataMember]
    public readonly bool IsAlwaysSaveOnEdit;

    [DataMember]
    public readonly Dictionary<string, CxClientAttributeMetadata> Attributes = new Dictionary<string, CxClientAttributeMetadata>();

    [DataMember]
    public readonly List<string> EditableAttributes = new List<string>();

    [DataMember]
    public readonly List<string> GridOrderedAttributes = new List<string>();

    [DataMember]
    public readonly CxClientEntityMetadata[] ChildEntities;

    [DataMember]
    public readonly CxClientCommandMetadata[] Commands;

    [DataMember]
    public readonly string EditControllerClassId;

    [DataMember]
    public readonly string EditFrameId;

    [DataMember]
    public List<string> JoinParamsNames { get; private set; }

    [DataMember]
    public List<string> WhereParamsNames { get; private set; }

    [DataMember]
    public List<string> FilterableIds { get; private set; }

    [DataMember]
    public CxExceptionDetails Error { get; internal set; }

    [DataMember]
    public bool ApplyDefaultFilter { get; internal set; }

    /// <summary>
    /// It is used to switch off find feature on the entity list. (true by default)
    /// </summary>
    [DataMember]
    public bool IsFilterEnabled { get; internal set; }

    [DataMember]
    public Dictionary<string, object> ApplicationValues = new Dictionary<string, object>();

    [DataMember]
    public string ImageId { get; set; }

    [DataMember]
    public bool SaveAndStayCommand { get; set; }

    [DataMember]
    public bool IsPagingEnabled { get; set; }

    [DataMember]
    public bool RefreshParentAfterSave { get; set; }

    [DataMember]
    public new List<CxClientParentEntity> ParentEntities { get; set; }

    [DataMember]
    public bool MultipleGridEdit { get; set; }

    //----------------------------------------------------------------------------
    public CxClientEntityMetadata()
    {

    }

    //----------------------------------------------------------------------------
    internal CxClientEntityMetadata(
      CxSlMetadataHolder holder, CxEntityUsageMetadata entityUsage)
    {
      if (holder == null)
        throw new ArgumentNullException("holder");
      if (entityUsage == null)
        throw new ArgumentNullException("entityUsage");

      EntityId = entityUsage.EntityId;
      Id = entityUsage.Id;
      SingleCaption = entityUsage.SingleCaption;
      PluralCaption = entityUsage.PluralCaption;
      FrameClassId = entityUsage.FrameClassId;
      SlFilterOnStart = entityUsage.SlFilterOnStart;
      IsAlwaysSaveOnEdit = entityUsage.IsAlwaysSaveOnEdit;
      bool saveAndStayCommand;
      SaveAndStayCommand = bool.TryParse(entityUsage["sl_save_and_stay"], out saveAndStayCommand);


      foreach (CxAttributeMetadata attrMetadata in entityUsage.Attributes)
      {
        CxClientAttributeMetadata clientAttributeMetadata = new CxClientAttributeMetadata(attrMetadata, entityUsage);
        Attributes.Add(clientAttributeMetadata.Id, clientAttributeMetadata);
        
      }

      foreach (CxAttributeMetadata editableAttrMetadata in entityUsage.GetAttributeOrder(NxAttributeContext.Edit).OrderAttributes)
      {
          if (Attributes.ContainsKey(editableAttrMetadata.Id))
            EditableAttributes.Add(editableAttrMetadata.Id);
      }
      foreach (CxAttributeMetadata gridOrderedMetadata in entityUsage.GetAttributeOrder(NxAttributeContext.GridVisible).OrderAttributes)
      {
          if (Attributes.ContainsKey(gridOrderedMetadata.Id))
            GridOrderedAttributes.Add(gridOrderedMetadata.Id);
      }

      ChildEntities = entityUsage.ChildEntityUsages.Values.ToArray(childEntityUsageMetadata => new CxClientEntityMetadata(holder, childEntityUsageMetadata.EntityUsage));
      Commands = entityUsage.Commands.ToArray(commandMetadata => new CxClientCommandMetadata(commandMetadata, entityUsage));
      ClientClassId = entityUsage["sl_client_class_id"];
      EditControllerClassId = entityUsage["sl_edit_controller_class_id"];
      EditFrameId = entityUsage.SlEditFrameId;

      JoinParamsNames = new List<string>();
      JoinParamsNames.AddRange(CxDbParamParser.GetList(entityUsage.JoinCondition, true));
      WhereParamsNames = new List<string>();
      WhereParamsNames.AddRange(CxDbParamParser.GetList(entityUsage.WhereClause, true));

      FilterableIds = new List<string>();
      IsPagingEnabled = entityUsage.IsPagingEnabled;
      foreach (CxAttributeMetadata filterable in entityUsage.FilterableAttributes)
      {
          if (Attributes.ContainsKey(filterable.Id))
            FilterableIds.Add(filterable.Id);
      }

      ApplyDefaultFilter = string.IsNullOrEmpty(entityUsage["sl_apply_default_filter"])
                            ? false
                            : Convert.ToBoolean(entityUsage["sl_apply_default_filter"]);

      IsFilterEnabled = entityUsage.IsFilterEnabled;
      ImageId = entityUsage.ImageId;

      bool refreshParent;
      bool.TryParse(entityUsage["sl_refresh_parent_after_save"], out refreshParent);
      RefreshParentAfterSave = refreshParent;

      ParentEntities = new List<CxClientParentEntity>();
      foreach (CxParentEntityMetadata parentEntity in entityUsage.Entity.ParentEntities)
      {
        ParentEntities.Add(
          new CxClientParentEntity(
            parentEntity.Entity.Id,
            parentEntity.EntityUsageId,
            CxDbParamParser.GetList(parentEntity.WhereClause, true)));
      }

      bool multipleGridEdit;
      MultipleGridEdit = bool.TryParse(entityUsage["sl_multiple_grid_edit"], out multipleGridEdit); 
    }

  }
}
