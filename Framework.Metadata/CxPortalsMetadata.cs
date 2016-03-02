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

using System.Collections;
using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to read and hold information about application classes.
  /// </summary>
  public class CxPortalsMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    protected List<CxPortalMetadata> m_PortalList = new List<CxPortalMetadata>(); // Portals list
    protected Hashtable m_PortalMap = new Hashtable(); // Portals dictionary
    protected CxPortalMetadata m_Default = null;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">name of file to read classes metadata</param>
    public CxPortalsMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("portal"))
      {
        CxPortalMetadata portal = new CxPortalMetadata(Holder, element);
        m_PortalList.Add(portal);
        m_PortalMap.Add(portal.Id, portal);
      }
      LoadOverrides(doc, "portal_override", m_PortalMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after metadata loaded.
    /// </summary>
    override protected void DoAfterLoad()
    {
      base.DoAfterLoad();
      // Determine default portal.
      foreach (CxPortalMetadata portal in m_PortalList)
      {
        if (portal.IsDefault)
        {
          m_Default = portal;
          break;
        }
      }
      if (m_Default == null && m_PortalList.Count > 0)
      {
        m_Default = m_PortalList[0];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds the portal by id.
    /// </summary>
    /// <param name="id">portal id</param>
    /// <returns>portal metadata object or null</returns>
    public CxPortalMetadata Find(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        CxPortalMetadata portal = (CxPortalMetadata) m_PortalMap[id.ToUpper()];
        if (portal != null && portal.Visible && portal.GetIsAllowed())
        {
          return portal;
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Portal with the given ID.
    /// </summary>
    public CxPortalMetadata this[string id]
    {
      get
      { 
        CxPortalMetadata portal = Find(id);
        if (portal != null)
          return portal;
        else
          throw new ExMetadataException(string.Format("Portal with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of available portals (allowed by security settings).
    /// </summary>
    public IList<CxPortalMetadata> Items
    {
      get 
      {
        List<CxPortalMetadata> portals = new List<CxPortalMetadata>();
        foreach (CxPortalMetadata portal in m_PortalList)
        {
          if (portal.Visible && portal.GetIsAllowed())
          {
            portals.Add(portal);
          }
        }
        return portals; 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of all registered portals independent from security settings.
    /// </summary>
    public IList<CxPortalMetadata> AllItems
    { get {return m_PortalList;} }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default portal.
    /// </summary>
    public CxPortalMetadata Default
    { 
      get 
      {
        IList<CxPortalMetadata> portals = Items;
        foreach (CxPortalMetadata portal in portals)
        {
          if (portal.IsDefault)
          {
            return portal;
          }
        }
        if (portals.Count > 0)
        {
          return (CxPortalMetadata) portals[0];
        }
        return null;
      } 
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default portal for login form.
    /// </summary>
    public CxPortalMetadata LoginPortal
    {
      get
      {
        CxPortalMetadata portal = Default;
        if (portal == null)
        {
          portal = m_Default;
        }
        return portal;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "WebPortals.xml"; } }
    //-------------------------------------------------------------------------
  }
}