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

using System.Collections;
using System.Xml;
using System.Data;

namespace Framework.Metadata
{
  /// <summary>
  /// Class to read and hold information about application reports.
  /// </summary>
  public class CxReportsMetadata : CxMetadataCollection
  {
    //----------------------------------------------------------------------------
    protected ArrayList m_Reports = new ArrayList(); // First-level menu items
    protected Hashtable m_AllReports = new Hashtable(); // All reports
    protected DataTable m_DataTable = null; // Data table for grid lookups
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="doc">XML doc to read metadata from</param>
    public CxReportsMetadata(CxMetadataHolder holder, XmlDocument doc) : 
      base(holder, doc)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);
      XmlElement reportsElement = (XmlElement) doc.DocumentElement.SelectSingleNode("reports");
      ReadLevel(reportsElement, m_Reports, null);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads reports lying under the given node and adds them into the given list.
    /// Then tries to deep report structure recursively.
    /// </summary>
    /// <param name="reportsElement">"reports" element reports are lying under</param>
    /// <param name="reports">list to all created reports</param>
    /// <param name="group">report group toadd this level reports</param>
    protected void ReadLevel(XmlElement reportsElement, IList reports, CxReportMetadata group)
    {
      foreach (XmlElement element in reportsElement.SelectNodes("report"))
      {
        CxReportMetadata report = new CxReportMetadata(Holder, element, group);
        reports.Add(report);
        m_AllReports.Add(report.Id, report);
        XmlElement subReportsElement = (XmlElement) element.SelectSingleNode("reports");
        if (subReportsElement != null)
        {
          ReadLevel(subReportsElement, report.Reports, report);
        }
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Report with the given ID.
    /// </summary>
    public CxReportMetadata this[string id]
    {
      get
      { 
        CxReportMetadata report = (CxReportMetadata) m_AllReports[id.ToUpper()];
        if (report != null)
          return report;
        else
          throw new ExMetadataException(string.Format("Report with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// First-level reports.
    /// </summary>
    public IList Reports
    {
      get { return m_Reports; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns data table for grid lookups.
    /// </summary>
    /// <returns>data table for grid lookups</returns>
    public DataTable GetDataTable()
    {
      if (m_DataTable == null)
      {
        m_DataTable = new DataTable();
        m_DataTable.Columns.Add("ID", typeof(string));
        m_DataTable.Columns.Add("Name", typeof(string));
        foreach (string id in m_AllReports.Keys)
        {
          CxReportMetadata report = (CxReportMetadata) m_AllReports[id];
          m_DataTable.Rows.Add(new object[] {id, report.Name});
        }
      }
      return m_DataTable;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Reports.xml"; } }
    //-------------------------------------------------------------------------
  }
}