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
  public class CxEntityJoin
  {
    //-------------------------------------------------------------------------
    private CxEntityUsageMetadata m_EntityUsage1;
    private CxEntityUsageMetadata m_EntityUsage2;

    private CxCriteriaOperator m_Criteria;
    //-------------------------------------------------------------------------
    public CxEntityUsageMetadata EntityUsage1
    {
      get { return m_EntityUsage1; }
      set { m_EntityUsage1 = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityUsageMetadata EntityUsage2
    {
      get { return m_EntityUsage2; }
      set { m_EntityUsage2 = value; }
    }
    //-------------------------------------------------------------------------
    public CxCriteriaOperator Criteria
    {
      get { return m_Criteria; }
      set { m_Criteria = value; }
    }
    //-------------------------------------------------------------------------
    public CxEntityJoin()
    {
    }
    //-------------------------------------------------------------------------
    public CxEntityJoin(
      CxEntityUsageMetadata entityUsage1, 
      CxEntityUsageMetadata entityUsage2,
      CxCriteriaOperator criteria)
      : this()
    {
      EntityUsage1 = entityUsage1;
      EntityUsage2 = entityUsage2;
      Criteria = criteria;
    }
    //-------------------------------------------------------------------------
  }
}
