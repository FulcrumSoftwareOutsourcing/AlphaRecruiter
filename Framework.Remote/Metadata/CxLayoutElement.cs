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
using System.Xml;
using System.Xml.Linq;

using Framework.Metadata;
using Framework.Utils;

namespace Framework.Remote
{
  [DataContract]
  public class CxLayoutElement
  {
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Type;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Id;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string Text = string.Empty;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly int Row = 0;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly int Column = 0;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly int RowSpan = 1;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly int ColumnSpan = 1;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly int RowsCount = 0;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly int ColumnsCount = 0;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string ColumnsWidth = "*";
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string RowsHeight = "*";
    //----------------------------------------------------------------------------
    [DataMember]
    public CxLayoutElement[] Children;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string ControlClassId;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly bool IsBorderVisible;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string FrameClassId;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly string EntityUsageId;
    //----------------------------------------------------------------------------
    [DataMember]
    public readonly bool ShowBorder = true;
    //---------------------------------------------------------------------------
    [DataMember]
    public string SlAutoLayoutFrameId { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxLayoutElement()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor, initializes the layout element with the data taken from the given
    /// metadata object (frame).
    /// </summary>
    /// <param name="layoutElementMetadata">metadata object to initialize by</param>
    public CxLayoutElement(CxSlLayoutElementMetadata layoutElementMetadata)
    {
      if (layoutElementMetadata is CxSlFrameMetadata)
      {
        Type = "frame";
        CxSlFrameMetadata frameMetadata = (CxSlFrameMetadata) layoutElementMetadata;
        FrameClassId = frameMetadata.FrameClassId;
        EntityUsageId = frameMetadata.EntityUsageId;
        SlAutoLayoutFrameId = frameMetadata.EntityUsageId;
      }
      else if (layoutElementMetadata is CxSlPanelMetadata)
      {
        Type = "panel";
        CxSlPanelMetadata panelMetadata = (CxSlPanelMetadata) layoutElementMetadata;
        ControlClassId = panelMetadata.ControlClassId;
        IsBorderVisible = panelMetadata.IsBorderVisible;
      }
      else if (layoutElementMetadata is CxSlTabControlMetadata)
        Type = "tab_control";
      else if (layoutElementMetadata is CxSlTabMetadata)
        Type = "tab";
      else if (layoutElementMetadata is CxSlHintMetadata)
        Type = "hint";
      else
        throw new ExException("Cannot recognize the type of the layout element");
      
      Id = layoutElementMetadata.Id;
      Text = layoutElementMetadata.Text;
      Row = layoutElementMetadata.Row;
      Column = layoutElementMetadata.Column;
      RowSpan = layoutElementMetadata.RowSpan;
      ColumnSpan = layoutElementMetadata.ColumnSpan;
      RowsCount = layoutElementMetadata.RowsCount;
      ColumnsCount = layoutElementMetadata.ColumnsCount;
      RowsHeight = layoutElementMetadata.RowsHeight;
      ColumnsWidth = layoutElementMetadata.ColumnsWidth;

      List<CxLayoutElement> children = new List<CxLayoutElement>();
      foreach (CxSlLayoutElementMetadata childMetadata in layoutElementMetadata.Children)
      {
        CxLayoutElement child = new CxLayoutElement(childMetadata);
        children.Add(child);
      }
      Children = children.ToArray();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor, initializes the layout element with the data taken from the given
    /// XML element.
    /// </summary>
    /// <param name="frameNode"></param>
    internal CxLayoutElement(XElement frameNode)
    {
      Type = frameNode.Name.LocalName;

      XAttribute idAttr = frameNode.Attribute("id");
      Id = idAttr != null ? idAttr.Value : string.Empty;

      XAttribute textAttr = frameNode.Attribute("text");
      Text = textAttr != null ? textAttr.Value : string.Empty;

      XAttribute rowAttr = frameNode.Attribute("row");
      Row = rowAttr != null ? Convert.ToInt32(rowAttr.Value) : 0;

      XAttribute columnAttr = frameNode.Attribute("column");
      Column = columnAttr != null ? Convert.ToInt32(columnAttr.Value) : 0;

      XAttribute rowSpanAttr = frameNode.Attribute("row_span");
      RowSpan = rowSpanAttr != null ? Convert.ToInt32(rowSpanAttr.Value) : 1;

      XAttribute columnSpanAttr = frameNode.Attribute("column_span");
      ColumnSpan = columnSpanAttr != null ? Convert.ToInt32(columnSpanAttr.Value) : 1;

      XAttribute rowCountAttr = frameNode.Attribute("rows_count");
      RowsCount = rowCountAttr != null ? Convert.ToInt32(rowCountAttr.Value) : 0;

      XAttribute columnCountAttr = frameNode.Attribute("columns_count");
      ColumnsCount = columnCountAttr != null ? Convert.ToInt32(columnCountAttr.Value) : 0;

      XAttribute columnsWidthAttr = frameNode.Attribute("columns_width");
      ColumnsWidth = columnsWidthAttr != null ? columnsWidthAttr.Value : "*";

      XAttribute rowsHeightAttr = frameNode.Attribute("rows_height");
      RowsHeight = rowsHeightAttr != null ? rowsHeightAttr.Value : "*";

      XAttribute controlClassIdAttr = frameNode.Attribute("sl_control_class_id");
      ControlClassId = controlClassIdAttr != null ? controlClassIdAttr.Value : string.Empty;

      XAttribute entityUsageIdAttr = frameNode.Attribute("entity_usage_id");
      EntityUsageId = entityUsageIdAttr != null ? entityUsageIdAttr.Value : string.Empty;

      XAttribute frameClassIdAttr = frameNode.Attribute("frame_class_id");
      FrameClassId = frameClassIdAttr != null ? frameClassIdAttr.Value : string.Empty;

      XAttribute showBorderAttr = frameNode.Attribute("show_border");
      ShowBorder = showBorderAttr != null ? Convert.ToBoolean(showBorderAttr.Value) : true;

      List<CxLayoutElement> childElements = new List<CxLayoutElement>();
      foreach (XElement childNode in frameNode.Elements())
      {
        if (childNode.NodeType == XmlNodeType.Element)
          childElements.Add(new CxLayoutElement(childNode));
      }
      Children = childElements.ToArray();
    }
  }
}
