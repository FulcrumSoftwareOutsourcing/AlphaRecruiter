using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote.Mobile
{
  [DataContract(Name = "CxClientCommandMetadata", Namespace = "http://schemas.datacontract.org/2004/07/FulcrumWeb")]
  public partial class CxClientCommandMetadata
  {
    [DataMember]
    public  string Id;

    [DataMember]
    public  string Text;

    [DataMember]
    public  bool IsEntityInstanceRequired;

    [DataMember]
    public  string SlHandlerClassId;

    [DataMember]
    public  string ImageId;

    [DataMember]
    public  bool IsMultiple;

    [DataMember]
    public  bool IsHandlerBatch;

    [DataMember]
    public  bool IsEnabled;

    [DataMember]
    public  string ConfirmationText;

    [DataMember]
    public  string CommandType;

    [DataMember]
    public bool Visible { get;  set; }

    [DataMember]
    public  bool HasServerHandler;

    [DataMember]
    public  string EntityUsageId;

    [DataMember]
    public  string PostCreateCommandId;

    [DataMember]
    public bool IsDbCommand { get;  set; }

    [DataMember]
    public bool RefreshPage { get;  set; }

    [DataMember]
    public  string TargetCommandId;

    [DataMember]
    public  string DisableConditionErrorText = string.Empty;

    [DataMember]
    public  string DynamicEntityUsageAttrId = string.Empty;

    [DataMember]
    public  string DynamicCommandAttrId = string.Empty;

    [DataMember]
    public string ReportCodeParameter;

        public bool HiddenWhenDisabled = false;

        public bool AvailableOnEditform = false;
  }
}
