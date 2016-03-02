using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxClientEntityMarks", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientEntityMarks
    {
        [DataMember]
        public List<CxClientEntityMark> AllRecentItems { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxClientEntityMark> RemovedRecentItems { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxClientEntityMark> AddedRecentItems { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxClientEntityMark> AllBookmarkItems { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxClientEntityMark> RemovedBookmarkItems { get;  set; }
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxClientEntityMark> AddedBookmarkItems { get;  set; }

        //----------------------------------------------------------------------------
        public CxClientEntityMarks()
        {
            AllRecentItems = new List<CxClientEntityMark>();
            RemovedRecentItems = new List<CxClientEntityMark>();
            AddedRecentItems = new List<CxClientEntityMark>();
            AllBookmarkItems = new List<CxClientEntityMark>();
            RemovedBookmarkItems = new List<CxClientEntityMark>();
            AddedBookmarkItems = new List<CxClientEntityMark>();
        }

      
    }
}
