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
  public class CxWinFormsMetadata : CxMetadataCollection
  {
    //-------------------------------------------------------------------------
    protected List<CxWinFormMetadata> m_ItemsList = new List<CxWinFormMetadata>();
    protected Hashtable m_ItemsMap = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">XML document to create list of pages</param>
    public CxWinFormsMetadata(CxMetadataHolder holder, XmlDocument doc): base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxWinFormsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      :
      base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement formElement in doc.DocumentElement.SelectNodes("form"))
      {
        CxWinFormMetadata form = new CxWinFormMetadata(Holder, formElement, this);
        AddItem(form);
      }
      LoadOverrides(doc, "form_override", m_ItemsMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds item to list and map.
    /// </summary>
    /// <param name="form">item to add</param>
    protected void AddItem(CxWinFormMetadata form)
    {
      m_ItemsList.Add(form);
      m_ItemsMap.Add(form.Id, form);
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all items.
    /// </summary>
    public IList<CxWinFormMetadata> Items
    {
      get
      {
        return m_ItemsList;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds form by the form ID.
    /// </summary>
    /// <param name="id">ID of form to find</param>
    /// <returns>found form or null</returns>
    public CxWinFormMetadata Find(string id)
    {
      return CxUtils.NotEmpty(id) ? (CxWinFormMetadata) m_ItemsMap[id.ToUpper()] : null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Page with a given ID.
    /// </summary>
    public CxWinFormMetadata this[string id]
    {
      get
      {
        CxWinFormMetadata form = Find(id);
        if (form != null)
          return form;
        else
          throw new ExMetadataException(string.Format("Form with ID=\"{0}\" not defined", id));
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "WinForms.xml"; } }
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
        CxWinFormMetadata winFormMetadata = Find(pair.Key);
        if (winFormMetadata != null)
        {
          foreach (XmlElement element in pair.Value.DocumentElement.SelectNodes("form_custom"))
          {
            winFormMetadata.LoadCustomMetadata(element);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
  }
}