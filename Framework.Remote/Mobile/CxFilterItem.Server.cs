using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;

namespace Framework.Remote.Mobile
{
  public partial class CxFilterItem : IxFilterElement
  {
    #region Implementation of IxFilterElement for server side

    public NxFilterOperation Operation { get; set; }

    public void SetValue(int index, object value)
    {
      Values[index] = value;
    }

    #endregion
  }
}
