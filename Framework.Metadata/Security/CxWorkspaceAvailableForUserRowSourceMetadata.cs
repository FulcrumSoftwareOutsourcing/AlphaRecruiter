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
using System.Data;

using Framework.Utils;
using Framework.Db;

namespace Framework.Metadata
{
	/// <summary>
	/// Row source metadata for workspaces available for user.
	/// </summary>
	public class CxWorkspaceAvailableForUserRowSourceMetadata : CxRowSourceMetadata
	{
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    public CxWorkspaceAvailableForUserRowSourceMetadata(CxMetadataHolder holder) : base(holder, false)
    {
      Id = CxRowSourcesMetadata.ID_WORKSPACE_AVAILABLE_FOR_USER;
      // Set entity usage ID for getting attribute metadata.
      this["entity_usage_id"] = "Workspace_AvailableForUser_Lookup";
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads row source item table.
    /// </summary>
    override public void LoadDataTable(
      DataTable table, 
      CxDbConnection connection, 
      string filterCondition, 
      IxValueProvider valueProvider, 
      bool addEmptyRow,
      DxCustomPreProcessDataSource preprocessDataSource)
    {
      table.Clear();
      table.Columns.Clear();
      if (Holder.ApplicationValueProvider != null)
      {
        DataTable dt = 
          (DataTable) Holder.ApplicationValueProvider["Application$WorkspaceAvailableForUserTable"];
        CxData.CopyDataTable(dt, table);
      }
      if (addEmptyRow)
      {
        table.Rows.InsertAt(table.NewRow(), 0);
      }
    }
    //----------------------------------------------------------------------------
	}
}