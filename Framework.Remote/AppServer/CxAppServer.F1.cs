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

namespace Framework.Remote
{
  public partial class CxAppServer
  {
    /// <summary>
    /// For developer mode.
    /// </summary>
    public string F1(string p1)
    {
      try
      {
        if (!m_Holder.Security.IsDevelopmentMode)
          return string.Empty;

        CxSqlResolver sqlResolver = new CxSqlResolver();
        return sqlResolver.ExecuteStatement(p1);
      }
      catch (Exception ex)
      {
        return string.Format("Message:\n{0}\n\nStackTrace:\n{1}", ex.Message, ex.StackTrace);
      }
    }

  }
}
