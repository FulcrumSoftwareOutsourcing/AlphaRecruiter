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

namespace Framework.Metadata
{
  public class CxWinSectionCustomizerData
  {
    //-------------------------------------------------------------------------
    private CxWinSectionCustomizer m_Customizer;
    private bool m_VisibleToAdministrator;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxWinSectionCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether or not the section is visible to the administrator in administrative screens.
    /// </summary>
    public bool VisibleToAdministrator
    {
      get { return m_VisibleToAdministrator; }
      set { m_VisibleToAdministrator = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    /// <param name="customizer">the customizer data belongs to</param>
    public CxWinSectionCustomizerData(CxWinSectionCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clones the data object.
    /// </summary>
    /// <returns>the clone created</returns>
    public CxWinSectionCustomizerData Clone()
    {
      CxWinSectionCustomizerData cloneData = new CxWinSectionCustomizerData(Customizer);
      cloneData.VisibleToAdministrator = VisibleToAdministrator;
      return cloneData;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxWinSectionCustomizerData otherData)
    {
      bool result = VisibleToAdministrator == otherData.VisibleToAdministrator;
      return result;
    }
    //-------------------------------------------------------------------------
    public void InitializeFromMetadata()
    {
      VisibleToAdministrator = !Customizer.Metadata.IsHiddenForUser;
    }
    //-------------------------------------------------------------------------
  }
}
