using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Remote.Mobile
{
    public partial class CxSkin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Framework.Remote.CxSkin"/> class.
        /// </summary>
        internal CxSkin(string id, string name, byte[] skinData, bool isSelected)
        {
            Id = id;
            Name = name;
            SkinData = skinData;
            IsSelected = isSelected;
        }
    }
}
