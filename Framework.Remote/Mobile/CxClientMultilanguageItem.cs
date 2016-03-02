using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientMultilanguageItem", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientMultilanguageItem
    {
        public CxClientMultilanguageItem()
        {
        }

        [DataMember]
        public string LocalizedValue { get;  set; }

        [DataMember]
        public string DefaultValue { get;  set; }

        [DataMember]
        public string ObjectType { get;  set; }

        [DataMember]
        public string ObjectNamespace { get;  set; }

        [DataMember]
        public string ObjectName { get;  set; }

        [DataMember]
        public string LocalizedPropertyName { get;  set; }

        [DataMember]
        public string ObjectParent { get;  set; }

        [DataMember]
        public string ObjectOwner { get;  set; }
    }
}
