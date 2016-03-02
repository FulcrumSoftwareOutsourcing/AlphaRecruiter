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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
	/// <summary>
  /// Class to read and hold information about constraints.
  /// </summary>
	public class CxConstraintsMetadata : CxMetadataCollection
	{
    //--------------------------------------------------------------------------
    protected NameValueCollection m_Constraints = new NameValueCollection(); // Constraint IDs vs. message
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxConstraintsMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
		{
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxConstraintsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      : base(holder, docs)
    {
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      foreach (XmlElement element in doc.DocumentElement.SelectNodes("constraint"))
      {
        string id = CxXml.GetAttr(element, "id");
        string message = CxXml.ReadTextElement(element, "message");
        m_Constraints.Add(id, message);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Tries to find constraint that caused the given exception.
    /// If such constraint found returns constraint message.
    /// Otherwise returns null.
    /// </summary>
    /// <param name="e">exception to translate</param>
    /// <returns>constraint message of null if constraint not found</returns>
    public string TranslateException(Exception e)
    {
      string result = null;
      string resultId = null;
      string message = e.Message.ToUpper();
      foreach (string id in m_Constraints.AllKeys)
      {
        if (message.IndexOf(id.ToUpper()) != -1)
        {
          result = m_Constraints[id];
          resultId = id;
          break;
        }
      }
      if (Holder != null &&
          Holder.IsMultilanguageEnabled &&
          Holder.Multilanguage.IsLocalizable(LocalizationObjectTypeCode, LocalizationPropertyCode))
      {
        result = Holder.Multilanguage.GetValue(
          LocalizationObjectTypeCode, LocalizationPropertyCode, resultId, result);
      }
      return result;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns collection of all constraints (id, message)
    /// </summary>
    public NameValueCollection Constraints
    {
      get { return m_Constraints; } 
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Object type code for the constraint localization
    /// </summary>
    public string LocalizationObjectTypeCode
    {
      get { return "Metadata.Constraint"; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Property code for the constraint message localization
    /// </summary>
    public string LocalizationPropertyCode
    {
      get { return "message"; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Constraints.xml"; } }
    //-------------------------------------------------------------------------
  }
}