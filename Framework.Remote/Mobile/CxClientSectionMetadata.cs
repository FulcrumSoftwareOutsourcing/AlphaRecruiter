using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{

    [DataContract(Name = "CxClientSectionMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
    public partial class CxClientSectionMetadata
    {

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public  string ImageId { get; set; }

        [DataMember]
        public  bool IsDefault { get; set; }

        [DataMember]
        public string UiProviderClassId { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public int DisplayOrder { get; set; }

        [DataMember]
        public List<CxClientTreeItemMetadata> TreeItems { get; set; }

        [DataMember]
        public string SectionFont { get; set; }

        [DataMember]
        public int SectionFontSize { get; set; }

        [DataMember]
        public string TreeItemsFont { get; set; }

        [DataMember]
        public int TreeItemsFontSize { get; set; }

        public string AppLogoImageId { get; set; }
        

        public string AppLogoText { get; set; }
        


    }
}
