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

using Framework.Db;
using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Custom metadata provider. Reads customization settings from the database.
	/// </summary>
	public class CxCustomDbMetadataProvider : IxCustomMetadataProvider
	{
    private IxConnectionFactory m_ConnectionFactory;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxCustomDbMetadataProvider()
		{
		}
	  //-------------------------------------------------------------------------
	  /// <summary>
	  /// The connection factory to be used by the customization provider.
	  /// </summary>
    public IxConnectionFactory ConnectionFactory
	  {
	    get { return m_ConnectionFactory; }
	    set { m_ConnectionFactory = value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the custom metadata.
    /// </summary>
    public IDictionary<string, IDictionary<string, XmlDocument>> GetCustomMetadata(IxValueProvider valueProvider)
	  {
      IDictionary<string, IDictionary<string, XmlDocument>> result = new Dictionary<string, IDictionary<string, XmlDocument>>();
      string sql = @"select * from Framework_MetadataCustomization where ApplicationCd = :APPLICATION$APPLICATIONCODE";
      CxGenericDataTable table = new CxGenericDataTable();
      CxDbConnection connection = ConnectionFactory.CreateConnection();
      try
      {
        connection.GetQueryResult(table, sql, valueProvider);
      }
      finally
      {
        ConnectionFactory.DisposeConnection(connection);
      }

	    foreach (CxGenericDataRow row in table.Rows)
      {
        string id = Convert.ToString(row["MetadataObjectId"]);
        string typeCd = Convert.ToString(row["MetadataObjectTypeCd"]);
        string xml = Convert.ToString(row["MetadataContent"]);
        if (!string.IsNullOrEmpty(xml))
        {
          IDictionary<string, XmlDocument> documentsById;
          if (!result.ContainsKey(typeCd))
            result[typeCd] = documentsById = new Dictionary<string, XmlDocument>();
          else
            documentsById = result[typeCd];

          xml = "<root>" + xml + "</root>";
          XmlDocument document = new XmlDocument();
          document.LoadXml(xml);
          documentsById.Add(id, document);
        }
      }
      return result;
	  }
	  //-------------------------------------------------------------------------
  }
}