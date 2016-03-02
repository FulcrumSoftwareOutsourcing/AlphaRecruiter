using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientAssemblyMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientAssemblyMetadata
    {
        //----------------------------------------------------------------------------
        [DataMember]
        public readonly string Id;
        //----------------------------------------------------------------------------
        [DataMember]
        public readonly string Namespace;
        //----------------------------------------------------------------------------
        [DataMember]
        public readonly string AssemblyName;
        //----------------------------------------------------------------------------
        [DataMember]
        public readonly string SlPluginPath;
        //----------------------------------------------------------------------------
        [DataMember]
        public readonly string FileName;
        //----------------------------------------------------------------------------
    }
}
