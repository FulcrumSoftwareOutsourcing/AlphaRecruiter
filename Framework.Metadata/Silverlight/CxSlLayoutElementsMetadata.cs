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
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxSlLayoutElementsMetadata : CxMetadataCollection
  {
    //-------------------------------------------------------------------------
    protected List<CxSlLayoutElementMetadata> m_ItemList 
      = new List<CxSlLayoutElementMetadata>(); // Frames list
    protected Dictionary<string, CxSlLayoutElementMetadata> m_ItemMap 
      = new Dictionary<string, CxSlLayoutElementMetadata>(); // Frames dictionary
    //-------------------------------------------------------------------------
    public CxSlLayoutElementsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs) 
      : base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    public CxSlLayoutElementsMetadata(CxMetadataHolder holder, XmlDocument doc)
      : base(holder, doc)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds the section by id.
    /// </summary>
    /// <param name="id">section id</param>
    /// <returns>section metadata object or null</returns>
    public CxSlLayoutElementMetadata Find(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        CxSlLayoutElementMetadata layoutElement = m_ItemMap[id.ToUpper()];
        if (layoutElement != null && layoutElement.Visible)
        {
          return layoutElement;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Section with the given ID.
    /// </summary>
    public CxSlLayoutElementMetadata this[string id]
    {
      get
      {
        CxSlLayoutElementMetadata layoutElement = Find(id);
        if (layoutElement != null)
          return layoutElement;
        else
          throw new ExMetadataException(string.Format("Layout element with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of available sections (allowed by security settings).
    /// </summary>
    public IList<CxSlFrameMetadata> Items
    {
      get
      {
        List<CxSlFrameMetadata> frames = new List<CxSlFrameMetadata>();
        foreach (CxSlFrameMetadata frame in m_ItemList)
        {
          if (frame.Visible)
          {
            frames.Add(frame);
          }
        }
        return frames;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of all registered sections independent from security settings.
    /// </summary>
    public IList<CxSlLayoutElementMetadata> AllItems
    { get { return m_ItemList; } }
    //-------------------------------------------------------------------------
  }
}
