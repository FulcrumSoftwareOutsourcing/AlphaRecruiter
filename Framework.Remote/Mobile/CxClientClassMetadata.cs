using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientClassMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientClassMetadata
    {
        //----------------------------------------------------------------------------
        [DataMember]
        public  string Id;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string AssemblyId;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string Name;
        //----------------------------------------------------------------------------
        public string ClientSideFolder;
      
     
    }
}
