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

namespace Framework.Metadata
{
  //-------------------------------------------------------------------------
  public class CxFilterOrderPanel: CxOrderItem
  {
    //-------------------------------------------------------------------------
    private bool m_ShowOnAdvancedPanel;
    //-------------------------------------------------------------------------
    public bool ShowOnAdvancedPanel
    {
      get { return m_ShowOnAdvancedPanel; }
      set { m_ShowOnAdvancedPanel = value; }
    }
    //-------------------------------------------------------------------------
    public CxFilterOrderPanel(bool showOnAdvancedPanel)
    {
      ShowOnAdvancedPanel = showOnAdvancedPanel;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an object's hash code.
    /// </summary>
    /// <returns>object's hash code</returns>
    public override int GetHashCode()
    {
      return ShowOnAdvancedPanel.GetHashCode();
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

      CxFilterOrderPanel panel = obj as CxFilterOrderPanel;
      if (panel != null)
      {
        return panel.ShowOnAdvancedPanel == ShowOnAdvancedPanel;
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
      if (ShowOnAdvancedPanel)
        return "Advanced";
      else
        return "Default";
    }
    //-------------------------------------------------------------------------
  }
}
