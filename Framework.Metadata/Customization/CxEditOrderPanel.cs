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

namespace Framework.Metadata
{
  //-------------------------------------------------------------------------
  public class CxEditOrderPanel : CxOrderItem
  {
    //-------------------------------------------------------------------------
    private string m_PanelId;
    //-------------------------------------------------------------------------
    /// <summary>
    /// A panel ID.
    /// </summary>
    public string PanelId
    {
      get { return m_PanelId; }
      set { m_PanelId = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="panelId">a panel ID</param>
    public CxEditOrderPanel(string panelId)
    {
      PanelId = panelId;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an object's hash code.
    /// </summary>
    /// <returns>object's hash code</returns>
    public override int GetHashCode()
    {
      return PanelId.GetHashCode();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the current object equals to the given one.
    /// </summary>
    /// <param name="obj">an object to compare with</param>
    /// <returns>true if equals</returns>
    public override bool Equals(object obj)
    {
      if (obj == this)
        return true;

      CxEditOrderPanel panel = obj as CxEditOrderPanel;
      if (panel != null)
      {
        return
          string.Equals(panel.PanelId, PanelId, StringComparison.OrdinalIgnoreCase);
      }

      return false;
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    ///</summary>
    ///<returns>
    ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public override string ToString()
    {
      return PanelId;
    } 
    //-------------------------------------------------------------------------
  }
}
