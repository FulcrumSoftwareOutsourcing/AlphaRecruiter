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
using System.Data;

namespace Framework.Utils
{
	/// <summary>
	/// Summary description for CxDataRowValueProvider.
	/// </summary>
	public class CxDataRowValueProvider : IxValueProvider
	{
    //-------------------------------------------------------------------------
    protected DataRow m_DataRow = null;
    protected DataRowVersion m_Version = DataRowVersion.Default;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
		public CxDataRowValueProvider(DataRow dataRow)
		{
      m_DataRow = dataRow;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxDataRowValueProvider(DataRow dataRow, DataRowVersion version) : this(dataRow)
    {
      m_Version = version;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public object this[string name]
    {
      get
      {
        if (m_DataRow != null &&
            m_DataRow.Table != null &&
            m_DataRow.Table.Columns.Contains(name))
        {
          return m_Version == DataRowVersion.Default ? m_DataRow[name] : m_DataRow[name, m_Version];
        }
        return null;
      }
      set
      {
        if (m_DataRow != null &&
            m_DataRow.Table != null &&
            m_DataRow.Table.Columns.Contains(name))
        {
          m_DataRow[name] = value;
        }
      }
    }

        private Dictionary<string, string> valueTypes = new Dictionary<string, string>();
        public IDictionary<string, string> ValueTypes
        {
            get
            {
                return valueTypes;
            }
        }
        //-------------------------------------------------------------------------
    }
}