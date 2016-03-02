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
using System.Xml;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
	/// <summary>
	/// Class to read and hold information about application assemblies.
	/// </summary>
	public class CxAssembliesMetadata : CxMetadataCollection
	{
    //----------------------------------------------------------------------------
    protected Hashtable m_Assemblies = new Hashtable(); // Assemblies dictionary
    protected string m_ApplicationCode = null;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">name of file to read assemblies metadata</param>
    public CxAssembliesMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
		{
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxAssembliesMetadata(CxMetadataHolder holder)
      : base(holder)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="assemblies">the list of assemblies to add</param>
    public CxAssembliesMetadata(CxMetadataHolder holder, IEnumerable<CxAssemblyMetadata> assemblies)
      : base(holder)
    {
      if (assemblies == null)
        throw new ExNullArgumentException("assemblies");
      foreach (CxAssemblyMetadata assemblyMetadata in assemblies)
      {
        if (assemblyMetadata != null)
          m_Assemblies.Add(assemblyMetadata.Id, assemblyMetadata);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxAssembliesMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
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
      if (CxUtils.IsEmpty(m_ApplicationCode))
      {
        m_ApplicationCode = CxAppInfo.ApplicationCode;
      }
      if (CxUtils.IsEmpty(m_ApplicationCode))
      {
        m_ApplicationCode = CxXml.GetAttr(doc.DocumentElement, "application_cd");
      }
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("assembly"))
      {
        if (Holder.GetIsElementInScope(element))
        {
          CxAssemblyMetadata assembly = new CxAssemblyMetadata(Holder, element);
          m_Assemblies.Add(assembly.Id, assembly);
        }
      }
      LoadOverrides(doc, "assembly_override", m_Assemblies);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Assembly with the given ID.
    /// </summary>
    public CxAssemblyMetadata this[string id]
    {
      get
      { 
        CxAssemblyMetadata assembly = (CxAssemblyMetadata) m_Assemblies[id.ToUpper()];
        if (assembly != null)
          return assembly;
        else
          throw new ExMetadataException(string.Format("Assembly with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assemblies dictionary.
    /// </summary>
    public Hashtable Assemblies
    {
      get { return m_Assemblies; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns application code. For backward compatibility only, do not use.
    /// </summary>
    internal string ApplicationCode
    { get { return m_ApplicationCode; } set { m_ApplicationCode = value; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Assemblies.xml"; } }
    //-------------------------------------------------------------------------
  }
}