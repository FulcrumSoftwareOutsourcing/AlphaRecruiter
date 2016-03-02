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
  public class CxCommandStateInfo
  {
    //-------------------------------------------------------------------------
    private NxCommandState m_State;
    private string m_Caption;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Actual command activity state.
    /// </summary>
    public NxCommandState State
    {
      get { return m_State; }
      set { m_State = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Command context caption.
    /// </summary>
    public string Caption
    {
      get { return m_Caption; }
      set { m_Caption = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    protected CxCommandStateInfo()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="state">actual command state</param>
    /// <param name="caption">context-related caption</param>
    public CxCommandStateInfo(NxCommandState state, string caption)
      : this()
    {
      State = state;
      Caption = caption;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="stateInfo">state-info to base the current one on</param>
    public CxCommandStateInfo(CxCommandStateInfo stateInfo)
    {
      State = stateInfo.State;
      Caption = stateInfo.Caption;
    }
    //-------------------------------------------------------------------------
  }
}
