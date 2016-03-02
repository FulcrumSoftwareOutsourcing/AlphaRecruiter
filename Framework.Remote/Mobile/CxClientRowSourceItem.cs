using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientRowSourceItem", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientRowSourceItem
    {
        [DataMember]
        public string Text { get; set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public object Value { get; set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string ImageId { get; set; }
        
        //----------------------------------------------------------------------------
        public CxClientRowSourceItem()
        {

        }

    }
}
