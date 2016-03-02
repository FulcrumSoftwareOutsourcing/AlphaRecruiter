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

using System.Xml;
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxSlPanelMetadata: CxSlLayoutElementMetadata
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the class representing the panel which is responsible 
    /// for displaying the content.
    /// </summary>
    public string ControlClassId { get { return this["control_class_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the border of the panel should be visible.
    /// If the border is invisible, the caption of the panel is invisible as well.
    /// </summary>
    public bool IsBorderVisible { get { return CxBool.Parse(this["is_border_visible"], true); } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="parentElement">Parent layout element</param>
    public CxSlPanelMetadata(CxMetadataHolder holder, XmlElement element, CxMetadataObject parentElement) 
      : base(holder, element, parentElement)
    {
      m_ParentObject = parentElement;
    }
    //------------------------------------------------------------------------- 
    /// <summary>
    /// Returns object type code for localization.
    /// </summary>
    override public string LocalizationObjectTypeCode
    {
      get
      {
        return "Metadata.SlPanel";
      }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns unique object name for localization.
    /// </summary>
    override public string LocalizationObjectName
    {
      get
      {
        return ParentObject != null ? ParentObject.LocalizationObjectName + "." + Id : base.LocalizationObjectName;
      }
    }
  }
}
