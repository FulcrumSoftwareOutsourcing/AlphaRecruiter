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
using System.Reflection;
using System.IO;
using System.Xml;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about application assembly.
  /// </summary>
  public class CxAssemblyMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    protected Assembly m_Assembly = null; // .NET assembly the metadata describes
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    public CxAssemblyMetadata(CxMetadataHolder holder, XmlElement element) : 
      base(holder, element)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxAssemblyMetadata(CxMetadataHolder holder)
      : base(holder)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assembly namespace.
    /// </summary>
    public string Namespace
    {
      get { return this["namespace"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assembly file name.
    /// </summary>
    public string FileName
    {
      get 
      { 
        string fileName = this["file_name"];
        //fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
        fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        return fileName;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Assembly created by metadata information.
    /// </summary>
    public Assembly Assembly
    {
      get
      {
        if (m_Assembly == null)
        {
          m_Assembly = AppDomain.CurrentDomain.Load(Path.GetFileNameWithoutExtension(this["file_name"]));
        }
        return m_Assembly;
      }
    }
    //----------------------------------------------------------------------------
  }
}