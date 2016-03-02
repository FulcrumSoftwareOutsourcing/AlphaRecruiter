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
using Framework.Entity;
using Framework.Utils;

namespace Framework.Remote
{
  [DataContract]
  public class CxExpressionResult : IxErrorContainer
  {
    [DataMember]
    public Dictionary<CxClientAttributeMetadata, object> Entity { get; set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public Dictionary<string, CxClientRowSource> UnfilteredRowSources = new Dictionary<string, CxClientRowSource>();

    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientRowSource> FilteredRowSources = new List<CxClientRowSource>();

    //----------------------------------------------------------------------------
    [DataMember]
    public CxExceptionDetails Error { get; internal set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets actual entity to calculate expressions.
    /// </summary>
    public CxBaseEntity ActualEntity { get; set; }

   
  }
}
