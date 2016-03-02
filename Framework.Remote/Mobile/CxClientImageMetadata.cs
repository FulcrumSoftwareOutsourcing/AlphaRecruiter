using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientImageMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientImageMetadata
    {
        [DataMember]
        public readonly string Id;

        [DataMember]
        public readonly string ProviderClassId;

        [DataMember]
        public readonly int ImageIndex;

        public readonly string FileName;

        public readonly string Folder;
    }
}
