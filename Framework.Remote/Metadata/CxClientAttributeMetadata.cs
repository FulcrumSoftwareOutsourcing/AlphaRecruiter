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
using System.Linq;
using System.Runtime.Serialization;

using Framework.Entity;
using Framework.Metadata;

namespace Framework.Remote
{
  [DataContract]
  public sealed class CxClientAttributeMetadata
  {
    [DataMember]
    public readonly string Id;

    [DataMember]
    public readonly string Type;

    [DataMember]
    public readonly string Caption;

    [DataMember]
    public readonly string FormCaption;

    [DataMember]
    public readonly string RowSourceId;

    [DataMember]
    public readonly bool HasRowSourceFilter;

    [DataMember]
    public readonly bool PrimaryKey;

    [DataMember]
    public bool Visible;

    [DataMember]
    public string ControlModifiers;

    [DataMember]
    public readonly object Default;

    [DataMember]
    public readonly int ControlWidth;

    [DataMember]
    public readonly int ControlHeight;

    [DataMember]
    public readonly int MaxLength;

    [DataMember]
    public readonly decimal MinValue;

    [DataMember]
    public readonly decimal MaxValue;

    [DataMember]
    public readonly bool Nullable;

    [DataMember]
    public readonly bool Editable;

    [DataMember]
    public readonly bool IsPlacedOnNewLine;
    
    [DataMember]
    public readonly bool IsPlacedOnNewLineInFilter;

    [DataMember]
    public bool ReadOnly;

    [DataMember]
    public readonly string SlControl;

    [DataMember]
    public readonly string ControlPlacement;

    [DataMember]
    public readonly bool FormCaptionPart;

    [DataMember]
    public readonly int GridWidth;

    [DataMember]
    public readonly bool Filterable;

    [DataMember]
    public readonly string FilterDefaultOperation;

    [DataMember]
    public readonly List<string> FilterOperations;

    [DataMember]
    public readonly string FilterDefault1;

    [DataMember]
    public readonly string FilterDefault2;

    [DataMember]
    public readonly bool FilterAdvanced;

    [DataMember]
    public readonly bool FilterMandatory;

    [DataMember]
    public List<string> DependentAttributesIds { get; private set; }

    [DataMember]
    public List<string> DependentMandatoryAttributesIds { get; private set; }

    [DataMember]
    public List<string> DependentStateIds { get; private set; }

    [DataMember]
    public readonly string BlobFileNameAttributeId;

    [DataMember]
    public readonly string BlobFileSizeAttributeId;

    [DataMember]
    public readonly bool Storable;

    [DataMember]
    public readonly string HyperlinkCommandId;

    [DataMember] 
    public readonly string HyperlinkEntityUsageId;

    [DataMember]
    public readonly string HyperlinkEntityUsageAttrId;

    [DataMember]
    public readonly bool IsDisplayName;

    [DataMember]
    public readonly bool SortingInGrid = true;

    //----------------------------------------------------------------------------
    public CxClientAttributeMetadata()
    {
    }

    //----------------------------------------------------------------------------
    internal CxClientAttributeMetadata(CxAttributeMetadata attributeMetadata, CxEntityUsageMetadata entityUsage)
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
          FilterOperations.Add(Enum.GetName(typeof (NxFilterOperation), filterOperation));
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

      if (!string.IsNullOrEmpty(attributeMetadata["sl_sorting_in_grid"]))
      {
        bool.TryParse(attributeMetadata["sl_sorting_in_grid"], out SortingInGrid);   
      }
    }


  }
}
