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
using System.Collections;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to hold information about application report.
  /// </summary>
  public class CxReportMetadata : CxMetadataObject
  {
    //----------------------------------------------------------------------------
    protected ArrayList m_Reports = new ArrayList(); // List of sub reports
    protected CxReportMetadata m_Group = null; // Group this report belongs to
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="element">XML element that holds metadata</param>
    /// <param name="group">group this report belongs to</param>
    public CxReportMetadata(CxMetadataHolder holder, XmlElement element, CxReportMetadata group) : 
      base(holder, element)
    {
      AddNodeToProperties(element, "description");
      m_Group = group;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Report name.
    /// </summary>
    public string Name 
    {
      get { return this["name"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Report URL (the same as name if not defined).
    /// </summary>
    public string Url 
    {
      get { return CxUtils.Nvl(this["url"], Name.Replace(' ', '+')); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Report folder.
    /// </summary>
    public string Folder
    {
      get 
      { 
        return (CxUtils.NotEmpty(this["folder"]) || m_Group == null ? 
                this["folder"] : 
                m_Group.Folder);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Report name.
    /// </summary>
    public string Description 
    {
      get { return this["description"]; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// true if this object is rather group than report itself.
    /// </summary>
    public bool IsGroup 
    {
      get { return (this["is_group"].ToLower() == "true"); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of sub reports.
    /// </summary>
    public ArrayList Reports
    {
      get { return m_Reports; }
    }
    //----------------------------------------------------------------------------
  }
}
