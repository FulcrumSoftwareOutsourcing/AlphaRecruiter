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
  public class CxAssemblyContainer : IxErrorContainer
  {
    /// <summary>
    /// Assembly file as binary array.
    /// </summary>
    [DataMember]
    public byte[] Assembly { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the CxExceptionDetails that contains information about occured Exception.
    /// </summary>
    [DataMember]
    public CxExceptionDetails Error { get; internal set; }
  }
}
