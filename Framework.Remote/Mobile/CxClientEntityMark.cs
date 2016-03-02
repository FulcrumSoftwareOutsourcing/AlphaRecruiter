using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientEntityMark", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientEntityMark
    {
        [DataMember]
        public string EntityUsageId { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string PrimaryKeyText { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string Name { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string ImageId { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string OpenMode { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string UniqueId { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<object> PrimaryKeyValues { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string MarkType { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string ApplicationCd { get;  set; }

       
    }
}
