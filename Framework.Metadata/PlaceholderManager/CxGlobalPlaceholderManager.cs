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
using Framework.Utils;

namespace Framework.Metadata
{
  public class CxGlobalPlaceholderManager: CxPlaceholderManagerBase
  {
    //-------------------------------------------------------------------------
    private static CxGlobalPlaceholderManager m_Instance;
    private readonly static object m_LockObject = new object();
    //-------------------------------------------------------------------------
    public static CxGlobalPlaceholderManager Instance
    {
      get 
      {
        if (m_Instance == null)
        {
          lock (m_LockObject)
          {
            if (m_Instance == null)
              m_Instance = new CxGlobalPlaceholderManager();
          }
        }
        return m_Instance; 
      }
    }
    //-------------------------------------------------------------------------
    private Dictionary<string, string> m_Properties;
    //-------------------------------------------------------------------------
    protected Dictionary<string, string> Properties
    {
      get { return m_Properties; }
    }
    //-------------------------------------------------------------------------
    public CxGlobalPlaceholderManager()
    {
      Initialize();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes properties for replacing place holder.
    /// </summary>
    private void Initialize()
    {
      m_Properties = new Dictionary<string, string>();
      m_Properties.Add("ApplicationName", CxAppInfo.ApplicationName);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Processes the given placeholder. Returns null if no procession was done - placeholder was not recognized.
    /// </summary>
    /// <param name="placeholder">placeholder</param>
    /// <param name="languageCd">language code to export (current if empty)</param>
    /// <returns>value to replace placeholder with; null if the placeholder was invalid</returns>
    protected override string ProcessPlaceholder(string placeholder, string languageCd)
    {
      if (Properties.ContainsKey(placeholder))
        return Properties[placeholder];
      return base.ProcessPlaceholder(placeholder, languageCd);
    }
    //-------------------------------------------------------------------------
  }
}