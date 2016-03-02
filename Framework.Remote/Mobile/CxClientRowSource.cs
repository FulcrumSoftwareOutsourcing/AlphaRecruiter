using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientRowSource", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientRowSource 
    {
        [DataMember]
        public string RowSourceId { get; set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxClientRowSourceItem> RowSourceData { get; set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public Dictionary<string, object> OwnerEntityPks { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public string OwnerAttributeId { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public bool IsFilteredRowSource { get;  set; }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets the CxExceptionDetails that contains information about occured Exception.
        /// </summary>
        [DataMember]
        public CxExceptionDetails Error { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public Dictionary<string, object> ApplicationValues = new Dictionary<string, object>();
        //----------------------------------------------------------------------------
    }
}
