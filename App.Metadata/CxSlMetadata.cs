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
using System.Collections.Generic;
using System.Xml;

using Framework.Metadata;
using Framework.Remote;

namespace App.Metadata
{
  public class CxSlMetadata : CxSlMetadataHolder
  {
    public CxSlMetadata()
    {
      
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads metadata from XML files.
    /// </summary>
    override protected void Initialize()
    {
      base.Initialize();

      m_Config = new CxConfigMetadata(this, LoadMetadata("ConfigSl.xml"));

      IEnumerable<XmlDocument> metadataProjectDocs = LoadMetadataWithIncludes("MetadataProject.xml");

      m_Assemblies = new CxAssembliesMetadata(this, metadataProjectDocs);
      m_Classes = new CxClassesMetadata(this, metadataProjectDocs);
      m_Images = new CxImagesMetadata(this, metadataProjectDocs);
      m_Commands = new CxCommandsMetadata(this, metadataProjectDocs);
      m_Entities = new CxEntitiesMetadata(this, metadataProjectDocs);
      m_EntityUsages = new CxEntityUsagesMetadata(this, metadataProjectDocs);
      m_EntityCommands = new CxEntityCommandsMetadata(this, metadataProjectDocs);
      m_Attributes = new CxAttributesMetadata(this, metadataProjectDocs);
      m_AttributeUsages = new CxAttributeUsagesMetadata(this, metadataProjectDocs);
      m_RowSources = new CxRowSourcesMetadata(this, metadataProjectDocs);
      m_Constraints = new CxConstraintsMetadata(this, metadataProjectDocs);

      SlSections = new CxSlSectionsMetadata(this, metadataProjectDocs);
      SlFrames = new CxSlFramesMetadata(this, metadataProjectDocs);
      SlSkins = new CxSlSkinsMetadata(this, metadataProjectDocs);
      SlDashboards = new CxSlDashboardsMetadata(this, metadataProjectDocs);


      m_Security = new CxSecurityMetadata(this, LoadMetadata("Security.xml"));

      ImageSmallThumbnailSize = 64;
      ImageLargeThumbnailSize = 64;

            int rLimit = -1;
            string lString = ConfigurationManager.AppSettings["DefaultRecordCountLimit"];
            if (int.TryParse(lString, out rLimit))
                DefaultRecordCountLimit = rLimit;
            else
                DefaultRecordCountLimit = -1;

       
       

    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns XML document with a specified file name to load metadata from.
    /// </summary>
    /// <param name="fileName">file name to load metadata from</param>
    override protected XmlDocument GetMetadataDocument(string fileName)
    {
      return LoadResourceFile(fileName);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns connection string.
    /// </summary>
    /// <returns>Connection string.</returns>
    public override string GetConnectionString()
    {
      return ConfigurationManager.ConnectionStrings["EntityDb"].ConnectionString;
    }
    //----------------------------------------------------------------------------
  }
}
