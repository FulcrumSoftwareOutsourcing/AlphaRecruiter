using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Remote.Mobile
{
    public partial class CxClientMultilanguageItem
    {
        public CxClientMultilanguageItem(
           string localizedValue,
           string defaultValue,
           string objectType,
           string objectNamespace,
           string objectName,
           string localizedPropertyName,
           string objectParent)
        {
            LocalizedValue = localizedValue;
            DefaultValue = defaultValue;
            ObjectType = objectType;
            ObjectNamespace = objectNamespace;
            ObjectName = objectName;
            LocalizedPropertyName = localizedPropertyName;
            ObjectParent = objectParent;
        }
    }
}
