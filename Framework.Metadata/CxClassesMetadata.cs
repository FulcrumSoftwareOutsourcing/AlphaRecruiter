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
using System.Collections;
using System.Xml;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to read and hold information about application classes.
  /// </summary>
  public class CxClassesMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    protected Hashtable m_Classes = new Hashtable(); // Classes dictionary
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">name of file to read classes metadata</param>
    public CxClassesMetadata(CxMetadataHolder holder, XmlDocument doc) 
      : base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxClassesMetadata(CxMetadataHolder holder)
      : base(holder)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="classes">the list of classes to add</param>
    public CxClassesMetadata(CxMetadataHolder holder, IEnumerable<CxClassMetadata> classes)
      : base(holder)
    {
      if (classes == null)
        throw new ExNullArgumentException("classes");
      foreach (CxClassMetadata classMetadata in classes)
      {
        if (classMetadata != null)
          m_Classes.Add(classMetadata.Id, classMetadata);
      }
    }    
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxClassesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
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
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("class"))
      {
        CxClassMetadata klass = new CxClassMetadata(Holder, element);
        m_Classes.Add(klass.Id, klass);
      }
      LoadOverrides(doc, "class_override", m_Classes);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Class with the given ID.
    /// </summary>
    public CxClassMetadata this[string id]
    {
      get
      { 
        CxClassMetadata klass = (CxClassMetadata) m_Classes[id.ToUpper()];
        if (klass != null)
          return klass;
        else
          throw new ExMetadataException(string.Format("Class with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Classes dictionary.
    /// </summary>
    public Hashtable Classes
    {
      get { return m_Classes; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Classes.xml"; } }
    //-------------------------------------------------------------------------
  }
}