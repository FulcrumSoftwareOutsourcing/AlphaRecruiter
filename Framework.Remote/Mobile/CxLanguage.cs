using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxLanguage", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxLanguage
    {
        [DataMember]
        public string LanguageCd { get;  set; }

        [DataMember]
        public string Name { get;  set; }

        [DataMember]
        public bool IsSelected { get; set; }
    }
}
