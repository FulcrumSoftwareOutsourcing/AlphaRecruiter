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

using System.Collections.Generic;
using System.Xml;

namespace Framework.Metadata
{
  public class CxSlFramesMetadata : CxSlLayoutElementsMetadata
  {
    //-------------------------------------------------------------------------
    public CxSlFramesMetadata(CxMetadataHolder holder, XmlDocument doc)
      : base(holder, doc)
    {
    }
    //-------------------------------------------------------------------------
    public CxSlFramesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    protected override string XmlFileName
    {
      get { return "FramesSl.xml"; }
    }
    //-------------------------------------------------------------------------
    protected override void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("sl_frame"))
      {
        CxSlFrameMetadata frame = new CxSlFrameMetadata(Holder, element, null);
        m_ItemList.Add(frame);
        m_ItemMap.Add(frame.Id, frame);
      }
      LoadOverrides(doc, "sl_frame_override", m_ItemMap);
    }
    //-------------------------------------------------------------------------
  }
}
