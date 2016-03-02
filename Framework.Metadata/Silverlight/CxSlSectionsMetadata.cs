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
using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
    public class CxSlSectionsMetadata : CxMetadataCollection
    {
        //----------------------------------------------------------------------------
        protected List<CxSlSectionMetadata> m_ItemList = new List<CxSlSectionMetadata>(); // Sections list
        protected Hashtable m_ItemMap = new Hashtable(); // Sections dictionary
        protected CxSlSectionMetadata m_Default = null;
        private Dictionary<string, object> jsAppProperties = new Dictionary<string, object>();
        //----------------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="holder">metadata holder</param>
        /// <param name="doc">document to read sections metadata</param>
        public CxSlSectionsMetadata(CxMetadataHolder holder, XmlDocument doc)
          : base(holder, doc)
        {
           
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="holder">metadata holder</param>
        /// <param name="docs">name of file to read assemblies metadata</param>
        public CxSlSectionsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
          : base(holder, docs)
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
            foreach (XmlElement element in doc.DocumentElement.SelectNodes("sl_section"))
            {
                CxSlSectionMetadata section = new CxSlSectionMetadata(Holder, element);
                m_ItemList.Add(section);
                m_ItemMap.Add(section.Id, section);
            }
            LoadOverrides(doc, "sl_section_override", m_ItemMap);

            
            foreach (XmlElement element in doc.DocumentElement.SelectNodes("js_app_properties"))
            {

                foreach (XmlAttribute attr in element.Attributes)
                {
                    if (!jsAppProperties.ContainsKey(attr.Name))
                        jsAppProperties.Add(attr.Name, attr.Value);
                    else
                        jsAppProperties[attr.Name] = attr.Value;
                }

            }

        }

        

        //-------------------------------------------------------------------------
        /// <summary>
        /// Does actions after metadata loaded.
        /// </summary>
        override protected void DoAfterLoad()
        {
            base.DoAfterLoad();
            // Determine default section.
            foreach (CxSlSectionMetadata section in m_ItemList)
            {
                if (section.IsDefault)
                {
                    m_Default = section;
                    break;
                }
            }
            if (m_Default == null && m_ItemList.Count > 0)
            {
                m_Default = m_ItemList[0];
            }
        }
        //-------------------------------------------------------------------------
        /// <summary>
        /// Finds the section by id.
        /// </summary>
        /// <param name="id">section id</param>
        /// <returns>section metadata object or null</returns>
        public CxSlSectionMetadata Find(string id)
        {
            if (CxUtils.NotEmpty(id))
            {
                CxSlSectionMetadata section = (CxSlSectionMetadata)m_ItemMap[id.ToUpper()];
                if (section != null && section.Visible && section.GetIsAllowed())
                {
                    return section;
                }
            }
            return null;
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Refreshes tree items created by tree item provider classes.
        /// </summary>
        public void RefreshDynamicTreeItems(
          Type itemProviderType,
          CxEntityUsageMetadata entityUsage)
        {
            foreach (CxSlSectionMetadata section in m_ItemList)
            {
                section.Items.RefreshDynamicItems(itemProviderType, entityUsage);
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Refreshes tree items created by tree item provider classes.
        /// </summary>
        public void RefreshDynamicTreeItems()
        {
            RefreshDynamicTreeItems(null, null);
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Section with the given ID.
        /// </summary>
        public CxSlSectionMetadata this[string id]
        {
            get
            {
                CxSlSectionMetadata section = Find(id);
                if (section != null)
                    return section;
                else
                    throw new ExMetadataException(string.Format("Section with ID=\"{0}\" not defined", id));
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// List of available sections (allowed by security settings).
        /// </summary>
        public IList<CxSlSectionMetadata> Items
        {
            get
            {
                List<CxSlSectionMetadata> sections = new List<CxSlSectionMetadata>();
                foreach (CxSlSectionMetadata section in m_ItemList)
                {
                    if (section.Visible && section.GetIsAllowed())
                    {
                        sections.Add(section);
                    }
                }
                return sections;
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// List of all registered sections independent from security settings.
        /// </summary>
        public IList<CxSlSectionMetadata> AllItems
        { get { return m_ItemList; } }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns default section.
        /// </summary>
        public CxSlSectionMetadata Default
        {
            get
            {
                IList<CxSlSectionMetadata> items = Items;
                foreach (CxSlSectionMetadata section in items)
                {
                    if (section.IsDefault)
                    {
                        return section;
                    }
                }
                if (items.Count > 0)
                {
                    return items[0];
                }
                return null;
            }
        }
        //----------------------------------------------------------------------------
        /// <summary>
        /// Returns default name for the metadata XML file.
        /// </summary>
        override protected string XmlFileName
        { get { return "SlSections.xml"; } }

        public Dictionary<string, object> JsAppProperties
        {
            get
            {
                return jsAppProperties;
            }

            set
            {
                jsAppProperties = value;
            }
        }
        //-------------------------------------------------------------------------
    }
}
