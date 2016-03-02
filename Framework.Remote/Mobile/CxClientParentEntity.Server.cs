using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Remote.Mobile
{
  public partial class CxClientParentEntity
  {
    public CxClientParentEntity(string entityId, string entityUsageId, string[] whereParamNames)
    {
      EntityId = entityId;
      EntityUsageId = entityUsageId;
      WhereParamNames = whereParamNames;
    }
  }
}
