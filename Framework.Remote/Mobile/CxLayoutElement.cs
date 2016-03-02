using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Remote.Mobile
{
    [DataContract(Name = "CxLayoutElement", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxLayoutElement
    {
        //----------------------------------------------------------------------------
        [DataMember]
        public  string Type;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string Id;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string Text = string.Empty;
        //----------------------------------------------------------------------------
        [DataMember]
        public  int Row = 0;
        //----------------------------------------------------------------------------
        [DataMember]
        public  int Column = 0;
        //----------------------------------------------------------------------------
        [DataMember]
        public  int RowSpan = 1;
        //----------------------------------------------------------------------------
        [DataMember]
        public  int ColumnSpan = 1;
        //----------------------------------------------------------------------------
        [DataMember]
        public  int RowsCount = 0;
        //----------------------------------------------------------------------------
        [DataMember]
        public  int ColumnsCount = 0;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string ColumnsWidth = "*";
        //----------------------------------------------------------------------------
        [DataMember]
        public  string RowsHeight = "*";
        //----------------------------------------------------------------------------
        [DataMember]
        public List<CxLayoutElement> Children ;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string ControlClassId;
        //----------------------------------------------------------------------------
        [DataMember]
        public  bool IsBorderVisible;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string FrameClassId;
        //----------------------------------------------------------------------------
        [DataMember]
        public  string EntityUsageId;
        //----------------------------------------------------------------------------
        [DataMember]
        public  bool ShowBorder = true;
        //---------------------------------------------------------------------------
        [DataMember]
        public string SlAutoLayoutFrameId { get; set; }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Default ctor.
        /// </summary>
        public CxLayoutElement()
        {
        }
       
    }
}
