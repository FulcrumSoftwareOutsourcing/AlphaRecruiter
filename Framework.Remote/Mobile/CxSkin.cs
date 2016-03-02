using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxSkin", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxSkin 
    {
        [DataMember]
        public string Id { get;  set; }

        //---------------------------------------------------------------------------
        [DataMember]
        public string Name { get;  set; }

        //---------------------------------------------------------------------------
        [DataMember]
        public byte[] SkinData { get; set; }

        //---------------------------------------------------------------------------
        [DataMember]
        public bool IsSelected { get;  set; }

        //---------------------------------------------------------------------------
        [DataMember]
        public bool IsDefault { get; set; }

        //---------------------------------------------------------------------------
        [DataMember]
        public CxExceptionDetails Error { get; set; }
    }
}
