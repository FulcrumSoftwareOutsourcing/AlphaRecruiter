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

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about application class.
  /// </summary>
  public class CxClassMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    protected Type m_Class = null; // .NET class the metadata describes
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    public CxClassMetadata(CxMetadataHolder holder, XmlElement element) : base(holder, element)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    public CxClassMetadata(CxMetadataHolder holder)
      : base(holder)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Class name.
    /// </summary>
    public string Name
    {
      get 
      { 
        CxAssemblyMetadata assembly = AssemblyMetadata;
        string nameSpace = assembly.Namespace;
        string name = this["name"];
        if (name.IndexOf('.') == -1) name = nameSpace + "." + name;
        return name;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// ID of the class assembly.
    /// </summary>
    public string AssemblyId
    {
      get { return CxText.ToUpper(this["assembly_id"]); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Class asseambly metadata.
    /// </summary>
    public CxAssemblyMetadata AssemblyMetadata
    {
      get { return Holder.Assemblies[AssemblyId]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Class created by metadata information.
    /// </summary>
    public Type Class
    {
      get
      {
        if (m_Class == null)
        {
          m_Class = AssemblyMetadata.Assembly.GetType(Name, true); 
        }
        return m_Class;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if class is inherited from the given class.
    /// </summary>
    public bool IsInheritedFrom(Type type)
    {
      return type != null ? Class == type || Class.IsSubclassOf(type) : false;
    }
    //----------------------------------------------------------------------------

   
  }
}