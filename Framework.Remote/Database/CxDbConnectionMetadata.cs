/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Xml;

using Framework.Db;
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Database connection description. Metadata to create DB connection from.
  /// </summary>
  public class CxDbConnectionMetadata
  {
    //-------------------------------------------------------------------------
    protected string m_Id;
    protected NxDataProviderType m_ProviderType;
    protected string m_ConnectionString;
    protected bool m_LoggedUserInto_CONTEXT_INFO = false;
    protected bool m_SetContextInfo = false;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor. Loads connection metadata from XML element.
    /// </summary>
    /// <param name="element">XML element to load data from</param>
    public CxDbConnectionMetadata(XmlElement element)
    {
      m_Id = GetRequiredAttr(element, "id");
      string providerType = GetRequiredAttr(element, "provider");
      try
      {
        m_ProviderType =
          (NxDataProviderType) Enum.Parse(typeof(NxDataProviderType), providerType, true);
      }
      catch (Exception e)
      {
        throw new ExDbConnectionReadException(e.Message, e);
      }
      m_ConnectionString = GetRequiredAttr(element, "connectionString");
      m_LoggedUserInto_CONTEXT_INFO = CxBool.Parse(CxXml.GetAttr(element, "LoggedUserInto_CONTEXT_INFO"));
      m_SetContextInfo = CxBool.Parse(CxXml.GetAttr(element, "setContextInfo"));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns attribute value from the XML element or raises an exception if
    /// value is empty.
    /// </summary>
    /// <param name="element">element to get attribute from</param>
    /// <param name="attrName">attribute name</param>
    /// <returns>attribute value</returns>
    protected string GetRequiredAttr(XmlElement element, string attrName)
    {
      string result = CxXml.GetAttr(element, attrName);
      if (CxUtils.IsEmpty(result))
      {
        throw new ExDbConnectionReadException("Required connection attribute " + attrName + " is not specified.");
      }
      return result;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique connection ID.
    /// </summary>
    public string Id
    { get { return m_Id; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns .NET provider type for the DB connection.
    /// </summary>
    public NxDataProviderType ProviderType
    { get { return m_ProviderType; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection string.
    /// </summary>
    public string ConnectionString
    { get { return m_ConnectionString; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if current user ID should be stored in the DB context info
    /// each time transaction is started
    /// </summary>
    public bool LoggedUserInto_CONTEXT_INFO
    { get { return m_LoggedUserInto_CONTEXT_INFO; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if context info including current user ID, 
    /// current application code and current language code
    /// should be stored in the DB context info each time transaction is started
    /// </summary>
    public bool SetContextInfo
    { get { return m_SetContextInfo; } }
    //-------------------------------------------------------------------------
  }
}
