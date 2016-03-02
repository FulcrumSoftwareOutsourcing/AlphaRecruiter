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

using System.Configuration;
using System.Xml;

namespace Framework.Remote
{
  /// <summary>
  /// Class reader for databaseConnections section of the Web.Config file.
  /// </summary>
  public class CxDbConnectionsSectionHandler : IConfigurationSectionHandler
  {
    //-------------------------------------------------------------------------
    public CxDbConnectionsSectionHandler()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates CxDbConnections object from the section XML node.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="configContext"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public object Create(
      object parent,
      object configContext,
      XmlNode section)
    {
      return new CxDbConnections(section);
    }
    //-------------------------------------------------------------------------
  }
}
