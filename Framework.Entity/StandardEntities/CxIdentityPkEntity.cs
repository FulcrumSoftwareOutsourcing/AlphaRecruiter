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

using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Entity class for DB objects with autoincremental identity PK
  /// Insert obtains autoincremted PK from SQL-server
  /// </summary>
  public class CxIdentityPkEntity : CxBaseEntity
  {
    //-------------------------------------------------------------------------
    public CxIdentityPkEntity(CxEntityUsageMetadata metadata)
      : base(metadata)
    {
    }
    //-------------------------------------------------------------------------
    public override void Insert(CxDbConnection connection)
    {
      DxStatementComposerDelegate sqlComposer = new DxStatementComposerDelegate(ComposeInsert);
      string sql = sqlComposer();
      IxValueProvider valueProvider = Metadata.PrepareValueProvider(this);
      using (CxDbCommand command = connection.CreateCommand(sql, valueProvider))
      {
        // Perform insert.
        object pkValue = connection.ExecuteScalar(command);
        // Try to receive PK identity value.
        if (CxUtils.IsEmpty(pkValue))
        {
          pkValue = connection.ExecuteScalar("SELECT @@IDENTITY");
        }
        if (CxUtils.NotEmpty(pkValue))
        {
          this[m_Metadata.PrimaryKeyAttribute.Id] = pkValue;
        }
      }
      //similar to base entity
      DoAfterInsert(connection);
      InsertExtraDbObjects(connection);
      InsertEntitiesBeforeChildren(connection);
      InsertChildren(connection);
    }
    //-------------------------------------------------------------------------
  }
}