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
  public class CxTabCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxTabCustomizer m_Customizer;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxTabCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="customizer">the customizer the tab belongs to</param>
    public CxTabCustomizerData(CxTabCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxTabCustomizerData otherData)
    {
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxTabCustomizerData Clone()
    {
      CxTabCustomizerData clone = new CxTabCustomizerData(Customizer);
      return clone;
    }
    //-------------------------------------------------------------------------
  }
}
