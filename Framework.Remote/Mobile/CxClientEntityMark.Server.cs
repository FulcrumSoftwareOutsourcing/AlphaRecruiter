using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;

namespace Framework.Remote.Mobile
{
    public partial class CxClientEntityMark
    {
        public CxClientEntityMark(CxEntityMark mark)
        {
            EntityUsageId = mark.EntityUsage.Id;
            PrimaryKeyText = mark.PrimaryKeyText;
            Name = string.Concat(mark.OpenMode, " ", mark.Name);
            ImageId = mark.EntityUsage.ImageId;
            OpenMode = mark.OpenMode;
            UniqueId = mark.UniqueId;
            PrimaryKeyValues = new List<object>();
            PrimaryKeyValues.AddRange(mark.PrimaryKeyValues);
            MarkType = Enum.GetName(typeof(NxEntityMarkType), mark.MarkType)[0].ToString();
            ApplicationCd = mark.ApplicationCd;
        }
    }
}
