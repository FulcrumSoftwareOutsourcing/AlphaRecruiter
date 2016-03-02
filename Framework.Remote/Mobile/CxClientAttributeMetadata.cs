using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxClientAttributeMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxClientAttributeMetadata
  {
    [DataMember]
    public  string Id;

    [DataMember]
    public  string Type;

    [DataMember]
    public  string Caption;

    [DataMember]
    public  string FormCaption;

    [DataMember]
    public  string RowSourceId;

    [DataMember]
    public  bool HasRowSourceFilter;

    [DataMember]
    public  bool PrimaryKey;

    [DataMember]
    public bool Visible;

    [DataMember]
    public string ControlModifiers;

    [DataMember]
    public  object Default;

    [DataMember]
    public  int ControlWidth;

    [DataMember]
    public  int ControlHeight;

    [DataMember]
    public  int MaxLength;

    [DataMember]
    public  decimal MinValue;

    [DataMember]
    public  decimal MaxValue;

    [DataMember]
    public  bool Nullable;

    [DataMember]
    public  bool Editable;

    [DataMember]
    public  bool IsPlacedOnNewLine;

    [DataMember]
    public  bool IsPlacedOnNewLineInFilter;

    [DataMember]
    public bool ReadOnly;

    [DataMember]
    public  string SlControl;

    [DataMember]
    public  string ControlPlacement;

    [DataMember]
    public  bool FormCaptionPart;

    [DataMember]
    public  int GridWidth;

    [DataMember]
    public  bool Filterable;

    [DataMember]
    public  string FilterDefaultOperation;

    [DataMember]
    public  List<string> FilterOperations;

    [DataMember]
    public  string FilterDefault1;

    [DataMember]
    public  string FilterDefault2;

    [DataMember]
    public  bool FilterAdvanced;

    [DataMember]
    public  bool FilterMandatory;

    [DataMember]
    public List<string> DependentAttributesIds { get;  set; }

    [DataMember]
    public List<string> DependentMandatoryAttributesIds { get;  set; }

    [DataMember]
    public List<string> DependentStateIds { get;  set; }

    [DataMember]
    public  string BlobFileNameAttributeId;

    [DataMember]
    public  string BlobFileSizeAttributeId;

    [DataMember]
    public  bool Storable;

    [DataMember]
    public  string HyperlinkCommandId;

    [DataMember]
    public  string HyperlinkEntityUsageId;

    [DataMember]
    public  string HyperlinkEntityUsageAttrId;

    [DataMember]
    public  bool IsDisplayName;

    [DataMember]
    public  bool SortingInGrid = true;

        public string JsControlCssClass = "";

        public bool Autofilter = false;

        public bool ReadOnlyForUpdate = false;

        public int ThumbnailWidth;

        public int ThumbnailHeight;

        public bool ShowTotal;
        public string CalcTotalJs;
        public string TotalText;
    }
}
