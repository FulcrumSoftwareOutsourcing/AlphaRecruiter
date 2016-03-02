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
  public class CxSlSkinsMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    protected List<CxSlSkinMetadata> m_ItemList = new List<CxSlSkinMetadata>(); 
    protected Dictionary<string, CxSlSkinMetadata> m_ItemMap = new Dictionary<string, CxSlSkinMetadata>();
    protected CxSlSkinMetadata m_Default;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">document to read skins metadata</param>
    public CxSlSkinsMetadata(CxMetadataHolder holder, XmlDocument doc)
      : base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxSlSkinsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
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
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("sl_skin"))
      {
        CxSlSkinMetadata skinMetadata = new CxSlSkinMetadata(Holder, element);
        m_ItemList.Add(skinMetadata);
        m_ItemMap.Add(skinMetadata.Id, skinMetadata);
      }
      LoadOverrides(doc, "sl_skin_override", m_ItemMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after metadata loaded.
    /// </summary>
    override protected void DoAfterLoad()
    {
      base.DoAfterLoad();
      // Determine default skin.
      foreach (CxSlSkinMetadata skin in m_ItemList)
      {
        if (skin.IsDefault)
        {
          m_Default = skin;
          break;
        }
      }
      if (m_Default == null && m_ItemList.Count > 0)
      {
        m_Default = m_ItemList[0];
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds the skin by id.
    /// </summary>
    /// <param name="id">skin id</param>
    /// <returns>skin metadata object or null</returns>
    public CxSlSkinMetadata Find(string id)
    {
      if (CxUtils.NotEmpty(id))
      {
        id = id.ToUpper();
        if (m_ItemMap.ContainsKey(id))
        {
          CxSlSkinMetadata skinMetadata = m_ItemMap[id];
          if (skinMetadata.Visible && skinMetadata.GetIsAllowed())
          {
            return skinMetadata;
          }
        }
      }
      return null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Skin with the given ID.
    /// </summary>
    public CxSlSkinMetadata this[string id]
    {
      get
      {
        CxSlSkinMetadata skinMetadata = Find(id);
        if (skinMetadata != null)
          return skinMetadata;
        else
          throw new ExMetadataException(string.Format("Skin with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of available skins (allowed by security settings).
    /// </summary>
    public IList<CxSlSkinMetadata> Items
    {
      get 
      {
        List<CxSlSkinMetadata> skins = new List<CxSlSkinMetadata>();
        foreach (CxSlSkinMetadata skin in m_ItemList)
        {
          if (skin.Visible)
          {
            skins.Add(skin);
          }
        }
        return skins; 
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of all registered skins independent from security settings.
    /// </summary>
    public IList<CxSlSkinMetadata> AllItems
    { get { return m_ItemList; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default skin.
    /// </summary>
    public CxSlSkinMetadata Default
    { 
      get 
      {
        IList<CxSlSkinMetadata> items = Items;
        foreach (CxSlSkinMetadata skin in items)
        {
          if (skin.IsDefault)
          {
            return skin;
          }
        }
        if (items.Count > 0)
        {
          return items[0];
        }
        return null;
      } 
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "SlSkins.xml"; } }
    //-------------------------------------------------------------------------
  }
}

