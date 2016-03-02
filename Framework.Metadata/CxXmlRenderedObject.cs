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
using System.IO;
using System.Text;
using System.Xml;

namespace Framework.Metadata
{
  public class CxXmlRenderedObject
  {
    //-------------------------------------------------------------------------
    private XmlElement m_Element;
    private bool m_IsEmpty;
    //-------------------------------------------------------------------------
    public XmlElement Element
    {
      get { return m_Element; }
      set { m_Element = value; }
    }
    //-------------------------------------------------------------------------
    public bool IsEmpty
    {
      get { return m_IsEmpty; }
      set { m_IsEmpty = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      if (!IsEmpty)
      {
        XmlWriter writer = new XmlTextWriter(new StringWriter(sb));
        Element.WriteTo(writer);
      }
      return sb.ToString();
    }
    //-------------------------------------------------------------------------
  }
}
