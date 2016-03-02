/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
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
using System.Text;
using System.Xml;

namespace Framework.Metadata
{
  public class CxSlTabsMetadata : CxSlLayoutElementsMetadata
  {
        //-------------------------------------------------------------------------
    public CxSlTabsMetadata(CxMetadataHolder holder, XmlDocument doc)
      : base(holder, doc)
    {
    }
    //-------------------------------------------------------------------------
    public CxSlTabsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    protected override string XmlFileName
    {
      get { return "TabsSl.xml"; }
    }
    //-------------------------------------------------------------------------
    protected override void Load(XmlDocument doc)
    {
      // Tabs should not be loaded as is.
    }
    //-------------------------------------------------------------------------

  }
}
