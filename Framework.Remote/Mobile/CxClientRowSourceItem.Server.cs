using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Remote.Mobile
{
    public partial class CxClientRowSourceItem
    {
        public CxClientRowSourceItem(string text, object value, string imageId)
        {
            Text = text;
            Value = value;
            ImageId = imageId;
        }
    }
}
