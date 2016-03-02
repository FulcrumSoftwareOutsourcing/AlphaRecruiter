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
  public class CxSkin : IxErrorContainer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Framework.Remote.CxSkin"/> class.
    /// </summary>
    public CxSkin(string id, string name, byte[] skinData, bool isSelected )
    {
      Id = id;
      Name = name;
      SkinData = skinData;
      IsSelected = isSelected;
    }

    [DataMember]
    public string Id { get; private set; }

    //---------------------------------------------------------------------------
    [DataMember]
    public string Name { get; private set; }

    //---------------------------------------------------------------------------
    [DataMember]
    public byte[] SkinData { get; set; }

    //---------------------------------------------------------------------------
    [DataMember]
    public bool IsSelected { get; private set; }

    //---------------------------------------------------------------------------
    [DataMember]
    public bool IsDefault { get; set; }

    //---------------------------------------------------------------------------
    [DataMember]
    public CxExceptionDetails Error{get; set;}
  }
}
