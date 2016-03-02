using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientEntityMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientEntityMetadata
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

        public readonly List<CxClientAttributeMetadata> AttributesList = new List<CxClientAttributeMetadata>();

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
        public List<string> JoinParamsNames { get; set; }

        [DataMember]
        public List<string> WhereParamsNames { get; set; }

        [DataMember]
        public List<string> FilterableIds { get; set; }

        [DataMember]
        public CxExceptionDetails Error { get; set; }

        [DataMember]
        public bool ApplyDefaultFilter { get; set; }

        /// <summary>
        /// It is used to switch off find feature on the entity list. (true by default)
        /// </summary>
        [DataMember]
        public bool IsFilterEnabled { get; set; }

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
        public List<CxClientParentEntity> ParentEntities { get; set; }

        [DataMember]
        public bool MultipleGridEdit { get; set; }


        public bool MultipleGridSelection { get; set; }

        public Dictionary<string, string> AttrsIdsWithDependencies = new Dictionary<string, string>();

        public List<string> PrimaryKeysIds = new List<string>();


        public string PostCreateCommandId;

        public bool IsRecordCountLimited { get; set; }

        public int RecordCountLimit { get; set; }

        public bool MultilselectionAllowded { get; set; }


        public bool DisplayAllRecordsWithoutFooter { get; set; }
        

        public bool WordwrapRowdata { get; set; }

        public string GridHint { get; set; }
    }
}
