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
using System.Runtime.Serialization;
using Framework.Entity;

namespace Framework.Remote
{
  [DataContract]
  public class CxClientEntityMark
  {
    [DataMember]
    public string EntityUsageId { get; private set;}
    //----------------------------------------------------------------------------
    [DataMember]
    public string PrimaryKeyText { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string Name { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string ImageId { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string OpenMode { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string UniqueId { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<object> PrimaryKeyValues { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string MarkType { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public string ApplicationCd { get; private set; }

    //----------------------------------------------------------------------------
    public CxClientEntityMark(CxEntityMark mark)
    {
      EntityUsageId = mark.EntityUsage.Id;
      PrimaryKeyText = mark.PrimaryKeyText;
      Name =  string.Concat(mark.OpenMode, " ", mark.Name);
      ImageId = mark.EntityUsage.ImageId;
      OpenMode = mark.OpenMode;
      UniqueId = mark.UniqueId;
      PrimaryKeyValues = new List<object>();
      PrimaryKeyValues.AddRange(mark.PrimaryKeyValues);
      MarkType = Enum.GetName(typeof (NxEntityMarkType), mark.MarkType)[0].ToString();
      ApplicationCd = mark.ApplicationCd;
    }
  }
}

