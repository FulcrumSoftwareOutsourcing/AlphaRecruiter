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

using System.Xml;

namespace Framework.Metadata
{
  public class CxSlFrameMetadata : CxSlLayoutElementMetadata
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// The id of the class representing the frame which is responsible 
    /// for displaying the content.
    /// </summary>
    public string FrameClassId { get { return this["frame_class_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The entity usage the frame belongs to.
    /// </summary>
    public string EntityUsageId { get { return this["entity_usage_id"]; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="parentElement">Parent layout element</param>
    public CxSlFrameMetadata(CxMetadataHolder holder, XmlElement element,
      CxMetadataObject parentElement) 
      : base(holder, element, parentElement)
    {
      m_ParentObject = parentElement;
    }
    //-------------------------------------------------------------------------
  }
}
