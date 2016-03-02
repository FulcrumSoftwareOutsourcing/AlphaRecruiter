using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Framework.Remote.Mobile
{
  public partial class CxSettingsContainer : IExtensibleDataObject
  {
     public virtual ExtensionDataObject ExtensionData { get; set; }
  }
}
