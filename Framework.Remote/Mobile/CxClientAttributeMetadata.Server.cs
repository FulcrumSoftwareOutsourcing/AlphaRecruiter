using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;
using Framework.Metadata;

namespace Framework.Remote.Mobile
{
    public partial class CxClientAttributeMetadata
    {
        //----------------------------------------------------------------------------
        public CxClientAttributeMetadata()
        {
        }

        //----------------------------------------------------------------------------
        public CxClientAttributeMetadata(CxAttributeMetadata attributeMetadata, CxEntityUsageMetadata entityUsage)
        {
            if (attributeMetadata == null)
                throw new ArgumentNullException();

            Id = attributeMetadata.Id;
            Type = attributeMetadata.Type;
            Caption = attributeMetadata.Caption;
            FormCaption = attributeMetadata.FormCaption;
            RowSourceId = attributeMetadata.RowSourceId.ToUpper();
            PrimaryKey = attributeMetadata.PrimaryKey;
            Visible = attributeMetadata.Visible;

            ControlModifiers = attributeMetadata.ControlModifiers;
            IsPlacedOnNewLine = attributeMetadata.IsPlacedOnNewLine;
            IsPlacedOnNewLineInFilter = attributeMetadata.IsPlacedOnNewLineInFilter;
            GridWidth = attributeMetadata.GridWidth;
            Default = attributeMetadata.Default;
            ControlWidth = !string.IsNullOrEmpty(attributeMetadata["sl_control_width"])
                                  ? Convert.ToInt32(attributeMetadata["sl_control_width"])
                                  : attributeMetadata.ControlWidth;

            ControlHeight = attributeMetadata.ControlHeight;
            MaxLength = attributeMetadata.MaxLength;
            MinValue = attributeMetadata.MinValue;
            MaxValue = attributeMetadata.MaxValue;
            Nullable = attributeMetadata.Nullable;
            Storable = attributeMetadata.Storable;

            CxAttributeOrder orderEdit = attributeMetadata.EntityMetadata.GetAttributeOrder(NxAttributeContext.Edit);
            Editable = orderEdit.OrderAttributes.Contains(attributeMetadata);

            ReadOnly = attributeMetadata.ReadOnly;

            CxAttributeOrder orderFilter = attributeMetadata.EntityMetadata.GetAttributeOrder(NxAttributeContext.Filter);
            Filterable = orderFilter.OrderAttributes.Contains(attributeMetadata);

            FilterDefaultOperation = attributeMetadata.FilterOperation;

            NxFilterOperation[] filterOperations = CxFilterElement.GetAttributeFilterOperations(attributeMetadata);

            FilterOperations = new List<string>();
            if (filterOperations != null)
            {
                foreach (NxFilterOperation filterOperation in filterOperations)
                {
                    FilterOperations.Add(Enum.GetName(typeof(NxFilterOperation), filterOperation));
                }
            }

            FilterDefault1 = attributeMetadata.FilterDefault1;
            FilterDefault2 = attributeMetadata.FilterDefault2;
            FilterAdvanced = attributeMetadata.FilterAdvanced;
            FilterMandatory = attributeMetadata.FilterMandatory;

            SlControl = attributeMetadata["sl_control"];
            ControlPlacement = attributeMetadata["sl_control_placement"];
            FormCaptionPart = string.IsNullOrEmpty(attributeMetadata["sl_form_caption_part"])
                                  ? false
                                  : Convert.ToBoolean(attributeMetadata["sl_form_caption_part"]);


            IList<CxAttributeMetadata> dependAttrs = entityUsage.GetDependentAttributes(attributeMetadata);
            DependentAttributesIds = (from attr in dependAttrs
                                      select attr.Id).ToList();

            IList<CxAttributeMetadata> dependMandatoryAttrs = entityUsage.GetDependentMandatoryAttributes(attributeMetadata);
            DependentMandatoryAttributesIds = (from attr in dependMandatoryAttrs
                                               select attr.Id).ToList();

            IList<CxAttributeMetadata> dependStateAttrs = entityUsage.GetDependentStateAttributes(attributeMetadata);
            DependentStateIds = (from attr in dependStateAttrs
                                 select attr.Id).ToList();

            HasRowSourceFilter = !string.IsNullOrEmpty(attributeMetadata.RowSourceFilter);

            BlobFileNameAttributeId = attributeMetadata.BlobFileNameAttributeId;
            BlobFileSizeAttributeId = attributeMetadata.BlobFileSizeAttributeId;


            HyperlinkCommandId = attributeMetadata.HyperLinkCommandId;
            HyperlinkEntityUsageAttrId = attributeMetadata.HyperLinkEntityUsageAttrId;
            HyperlinkEntityUsageId = attributeMetadata.HyperLinkEntityUsageId;

            IsDisplayName = attributeMetadata.IsDisplayName;

            if (string.IsNullOrWhiteSpace(attributeMetadata.JsControlCssClass) == false)
                JsControlCssClass = attributeMetadata.JsControlCssClass;
            else
                JsControlCssClass = "";

            if (!string.IsNullOrEmpty(attributeMetadata["sl_sorting_in_grid"]))
            {
                bool.TryParse(attributeMetadata["sl_sorting_in_grid"], out SortingInGrid);
            }

            Autofilter = attributeMetadata.Autofilter;
            ReadOnlyForUpdate = attributeMetadata.ReadOnlyForUpdate;

            ThumbnailWidth = attributeMetadata.ThumbnailWidth;
            ThumbnailHeight = attributeMetadata.ThumbnailHeight;
            ShowTotal = attributeMetadata.ShowTotal;
            CalcTotalJs = attributeMetadata.CalcTotalJs;
            TotalText = attributeMetadata.TotalText;
        }
}
}
