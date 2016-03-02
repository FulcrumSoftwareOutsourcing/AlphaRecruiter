using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientPortalMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientPortalMetadata 
    {

        [DataMember]
        public readonly List<CxClientSectionMetadata> Sections = new List<CxClientSectionMetadata>();

        [DataMember]
        public readonly List<CxClientRowSource> StaticRowsources = new List<CxClientRowSource>();

        [DataMember]
       public readonly List<CxClientAssemblyMetadata> Assemblies = new List<CxClientAssemblyMetadata>();

        [DataMember]
        public readonly List<CxClientClassMetadata> Classes = new List<CxClientClassMetadata>();

        [DataMember]
        public readonly CxClientImageMetadata[] Images;

        [DataMember]
        public readonly List<CxLayoutElement> Frames = new List<CxLayoutElement>();

        [DataMember]
        public CxExceptionDetails Error { get;  set; }

        [DataMember]
        public Dictionary<string, object> ApplicationValues { get; set; }

        [DataMember]
        public Dictionary<string, byte[]> AssembliesData = new Dictionary<string, byte[]>();

        [DataMember]
        public CxClientEntityMarks ClientEntityMarks { get;  set; }

        [DataMember]
        public List<CxClientMultilanguageItem> MultilanguageItems = new List<CxClientMultilanguageItem>();

        [DataMember]
        public List<CxLanguage> Languages = new List<CxLanguage>();

        [DataMember]
        public List<CxSkin> Skins;

        [DataMember]
        public Dictionary<string, string> Constraints = new Dictionary<string, string>();

        public CxClientPortalMetadata()
        {
          ApplicationValues = new Dictionary<string, object>();
        }

        
        public Dictionary<string, object> JsAppProperties { get; set; }
    }
}
