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
using System.Text;
using Framework.Db;

namespace Framework.Metadata
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// The class is dedicated to automatical resolving a list of possible joins
  /// between two given entities.
  /// </summary>
  public class CxEntityJoinResolver
  {
    //-------------------------------------------------------------------------
    protected virtual CxEntityJoinSetList ProcessAttribute_HyperlinkEntity(
      CxEntityUsageMetadata entityUsage1, CxEntityUsageMetadata entityUsage2)
    {
      CxEntityJoinSetList joinSetList = new CxEntityJoinSetList();
      foreach (CxAttributeMetadata attributeMetadata in entityUsage1.Attributes)
      {
        if (attributeMetadata.HyperLinkEntityUsage == entityUsage2)
        {
          CxAttributeMetadata valueAttributeMetadata = entityUsage1.GetValueAttribute(attributeMetadata);

          CxEntityJoinSet joinSet = new CxEntityJoinSet();
          CxEntityJoin join = new CxEntityJoin();
          join.EntityUsage1 = entityUsage1;
          join.EntityUsage2 = entityUsage2;
          join.Criteria = new CxBinaryOperator(
            new CxAttributeOperand(entityUsage1, valueAttributeMetadata),
            NxBinaryOperatorType.Equal,
            new CxAttributeOperand(entityUsage2, entityUsage2.PrimaryKeyAttribute));

          joinSet.Add(join);
          joinSetList.Add(joinSet);
        }
      }
      return joinSetList;
    }
    //-------------------------------------------------------------------------
    public virtual CxEntityJoinSetList Resolve(
      CxDbConnection connection, 
      CxEntityUsageMetadata entityUsage1, 
      CxEntityUsageMetadata entityUsage2)
    {
      CxEntityJoinSetList joinSetList = new CxEntityJoinSetList();

      joinSetList.AddRange(ProcessAttribute_HyperlinkEntity(entityUsage1, entityUsage2));
      joinSetList.AddRange(ProcessAttribute_HyperlinkEntity(entityUsage2, entityUsage1));

      return joinSetList;
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------
  }
}
