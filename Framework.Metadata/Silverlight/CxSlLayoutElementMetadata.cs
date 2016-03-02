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
using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxSlLayoutElementMetadata : CxMetadataObject
  {
    //-------------------------------------------------------------------------
    private List<CxSlLayoutElementMetadata> m_Children;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The index of the row the element belongs to.
    /// </summary>
    public int Row { get { return CxInt.Parse(this["row"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The index of the column the element belongs to.
    /// </summary>
    public int Column { get { return CxInt.Parse(this["column"], 0); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The span of the rows the element should fill.
    /// </summary>
    public int RowSpan { get { return CxInt.Parse(this["row_span"], 1); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The span of the columns the element should fill.
    /// </summary>
    public int ColumnSpan { get { return CxInt.Parse(this["column_span"], 1); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Amount of rows in the layout element. By default = 1.
    /// </summary>
    public int RowsCount { get { return CxInt.Parse(this["rows_count"], 1); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Amount of columns in the layout element. By default = 1.
    /// </summary>
    public int ColumnsCount { get { return CxInt.Parse(this["columns_count"], 1); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Comma-separated list of rows height. Asterisks can be used.
    /// </summary>
    public string RowsHeight { get { return this["rows_height"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Comma-separated list of columns width. Asterisks can be used.
    /// </summary>
    public string ColumnsWidth { get { return this["columns_width"]; } }
    //-------------------------------------------------------------------------
    public List<CxSlLayoutElementMetadata> Children
    {
      get { return m_Children; }
      protected set { m_Children = value; }
    }
    //-------------------------------------------------------------------------
    protected CxMetadataObject m_ParentObject;
    //---------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="parentElement">Parent layout element</param>
    public CxSlLayoutElementMetadata(CxMetadataHolder holder, XmlElement element,
      CxMetadataObject parentElement)
      : base(holder, element)
    {
      m_ParentObject = parentElement;
      Children = new List<CxSlLayoutElementMetadata>();
      foreach (XmlNode childNode in element.ChildNodes)
      {
        if (childNode.NodeType != XmlNodeType.Element)
          continue;
        XmlElement childElement = (XmlElement) childNode;
        switch (childNode.LocalName)
        {
          case "sl_frame":
            Children.Add(new CxSlFrameMetadata(holder, childElement, this));
            break;
          case "sl_panel":
            Children.Add(new CxSlPanelMetadata(holder, childElement, this));
            break;
          case "sl_tab_control":
            Children.Add(new CxSlTabControlMetadata(holder, childElement, this));
            break;
          case "sl_tab":
            Children.Add(new CxSlTabMetadata(holder, childElement, this));
            break;
          case "sl_hint":
            Children.Add(new CxSlHintMetadata (holder, childElement, this));
            break;
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns parent metadata object.
    /// </summary>
    public override CxMetadataObject ParentObject
    {
      get { return m_ParentObject; }
    }
  }
}
