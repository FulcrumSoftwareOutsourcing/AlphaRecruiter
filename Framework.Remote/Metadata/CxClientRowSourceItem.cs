/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote
{
  [DataContract]
  public class CxClientRowSourceItem
  {
    [DataMember]
    public string Text { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public object Value { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string ImageId { get; set; }
    //----------------------------------------------------------------------------
    public CxClientRowSourceItem(string text, object value, string imageId)
    {
      Text = text;
      Value = value;
      ImageId = imageId;
    }
    //----------------------------------------------------------------------------
    public CxClientRowSourceItem()
    {

    }

  }
}
