using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote.Mobile
{
    public partial class CxClientEntityMetadata
    {
        //----------------------------------------------------------------------------
        public CxClientEntityMetadata()
        {

        }

        //----------------------------------------------------------------------------
        public CxClientEntityMetadata(
          CxSlMetadataHolder holder, CxEntityUsageMetadata entityUsage, Dictionary<string, object> filterDefaults1, Dictionary<string, object> filterDefaults2)
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
                if (attrMetadata.PrimaryKey)
                    PrimaryKeysIds.Add(attrMetadata.Id);

                if (filterDefaults1 != null)
                {
                    if (filterDefaults1.ContainsKey(attrMetadata.Id))
                        clientAttributeMetadata.FilterDefault1 = Convert.ToString(filterDefaults1[attrMetadata.Id]);
                }
                if (filterDefaults2 != null)
                {
                    if (filterDefaults2.ContainsKey(attrMetadata.Id))
                        clientAttributeMetadata.FilterDefault2 = Convert.ToString(filterDefaults2[attrMetadata.Id]);
                }
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

            ChildEntities = entityUsage.ChildEntityUsages.Values.ToArray(childEntityUsageMetadata => new CxClientEntityMetadata(holder, childEntityUsageMetadata.EntityUsage, null, null));
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

            MultipleGridEdit = CxBool.Parse(entityUsage["sl_multiple_grid_edit"], false);

            //MultipleGridSelection = CxBool.Parse(entityUsage["multiple_grid_selection"], true);

            //foreach (var attr in AttributesList)
            //{
            //    if(string.IsNullOrEmpty(!string.IsNullOrEmpty(attr.RowSourceFilter)) )
            //}
            PostCreateCommandId = entityUsage.PostCreateCommandId;

            IsRecordCountLimited = entityUsage.IsRecordCountLimited;

            RecordCountLimit = entityUsage.RecordCountLimit;

            MultilselectionAllowded = entityUsage.MultilselectionAllowded;

            DisplayAllRecordsWithoutFooter = entityUsage.DisplayAllRecordsWithoutFooter;

            WordwrapRowdata = entityUsage.WordwrapRowdata;

            GridHint = holder.GetTxt( entityUsage.GridHint);
    }
    }
}
