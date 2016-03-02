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
  /// Class to read and hold information about row sources.
  /// </summary>
	public class CxRowSourcesMetadata : CxMetadataCollection
	{
    //----------------------------------------------------------------------------
    // Predefined row source IDs
    public const string ID_WORKSPACE_AVAILABLE_FOR_USER = "RS_WorkspaceAvailableForUser_Lookup";
    //----------------------------------------------------------------------------
    protected Hashtable m_RowSources = new Hashtable(); // Row sources dictionary
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxRowSourcesMetadata(CxMetadataHolder holder, XmlDocument doc): 
      base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxRowSourcesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
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
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("row_source"))
      {
        CxRowSourceMetadata rowSource = new CxRowSourceMetadata(Holder, element);
        Add(rowSource);
      }
      LoadOverrides(doc, "row_source_override", m_RowSources);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds row source to the collection.
    /// </summary>
    protected void Add(CxRowSourceMetadata rowSource)
    {
      m_RowSources.Add(rowSource.Id, rowSource);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads the custom metadata from the given dictionary of XML documents sorted by
    /// attribute usage IDs.
    /// </summary>
    /// <param name="documents">documents containing the custom metadata</param>
    public void LoadCustomMetadata(IDictionary<string, XmlDocument> documents)
    {
      foreach (KeyValuePair<string, XmlDocument> pair in documents)
      {
        CxRowSourceMetadata rowSourceMetadata = Find(pair.Key);
        if (rowSourceMetadata != null)
        {
          foreach (XmlElement element in pair.Value.DocumentElement.SelectNodes("row_source_custom"))
          {
            rowSourceMetadata.LoadCustomMetadata(element);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Seeks for the row source with the given id.
    /// </summary>
    /// <returns>the row source if found, null otherwise</returns>
    public CxRowSourceMetadata Find(string id)
    {
      id = CxText.ToUpper(id);
      CxRowSourceMetadata rowSource = (CxRowSourceMetadata) m_RowSources[id];
      if (rowSource == null)
      {
        // Try to create predefined row source.
        if (id == ID_WORKSPACE_AVAILABLE_FOR_USER.ToUpper())
        {
          rowSource = new CxWorkspaceAvailableForUserRowSourceMetadata(Holder);
          m_RowSources[id] = rowSource;
        }
      }
      return rowSource;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row source with the given ID.
    /// </summary>
    public CxRowSourceMetadata this[string id]
    {
      get
      {
        CxRowSourceMetadata rowSource = Find(id);
        if (rowSource == null)
        {
          // Row source is not found.
          throw new ExMetadataException(string.Format("Row source with ID=\"{0}\" not defined", id));
        }
        // Row source is found.
        return rowSource;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Row source dictionary.
    /// </summary>
    public Hashtable RowSources
    {
      get { return m_RowSources; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "RowSources.xml"; } }
    //-------------------------------------------------------------------------
  }
}