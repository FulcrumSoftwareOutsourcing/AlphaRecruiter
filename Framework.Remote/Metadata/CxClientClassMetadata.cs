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

using System.Runtime.Serialization;

using Framework.Metadata;

namespace Framework.Remote
{
  [DataContract]
  public class CxClientClassMetadata
  {
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Id;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string AssemblyId;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Name;
    //----------------------------------------------------------------------------
    public CxClientClassMetadata()
    {
    }
    //----------------------------------------------------------------------------
    internal CxClientClassMetadata(CxClassMetadata classMetadata)
    {
      Id = classMetadata.Id;
      AssemblyId = classMetadata.AssemblyId;
      Name = classMetadata.Name;
    }
    //----------------------------------------------------------------------------
  }


}
