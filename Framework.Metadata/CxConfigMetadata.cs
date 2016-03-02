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
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Xml;
using Framework.Utils;
using Framework.Common;

namespace Framework.Metadata
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Enumeration to define application scope metadata are used for.
  /// </summary>
  public enum NxApplicationScope {All, Windows, Web, Silverlight}
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Class to hold metadata configuration settings.
  /// </summary>
  public class CxConfigMetadata
  {
    //-------------------------------------------------------------------------
    protected CxMetadataHolder m_Holder = null;
    protected NameValueCollection m_Properties = new NameValueCollection();
    protected NxApplicationScope m_ApplicationScope = NxApplicationScope.All;
    protected IndexedDictionary<string, Assembly> m_Plugins = new IndexedDictionary<string, Assembly>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="doc">XML document to load data from</param>
    public CxConfigMetadata(CxMetadataHolder holder, XmlDocument doc)
    {
      m_Holder = holder;
      foreach (XmlAttribute attribute in doc.DocumentElement.Attributes)
      {
        m_Properties.Add(attribute.Name, attribute.Value);
      }
      m_ApplicationScope = 
        CxEnum.Parse<NxApplicationScope>(this["application_scope"], NxApplicationScope.All);

      XmlNode pluginNode = doc.DocumentElement.SelectSingleNode("plugins");
      if (pluginNode != null)
      {
        IList<string> names = CxText.RemoveEmptyStrings(CxText.DecomposeWithWhiteSpaceAndComma(pluginNode.InnerText));
        foreach (string name in names)
        {
          try
          {
            Assembly assembly;
            string assemblyFolder = CxConfigurationHelper.AssemblyFolder;
            if (!string.IsNullOrEmpty(assemblyFolder))
              assembly = CxType.GetAssembly(assemblyFolder, name + ".dll");
            else
              assembly = CxType.GetAssembly(name + ".dll");
            if (assembly != null)
            {
              m_Plugins.Add(name, assembly);
            }
          }
          catch
          {
            
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Metadata configuration settings by name.
    /// </summary>
    public string this[string name]
    { 
      get 
      { 
        return m_Properties[name]; 
      }
      set
      {
        m_Properties[name] = value;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets application code to identify application in database.
    /// </summary>
    public string ApplicationCode
    {
      get
      {
        return this["application_code"];
      }
      set
      {
        this["application_code"] = value;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns current application scope metadata are used for.
    /// </summary>
    public NxApplicationScope ApplicationScope
    {
      get
      {
        return m_ApplicationScope;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns default child entity usage visibility in hierarchical grids.
    /// </summary>
    public NxBoolEx IsChildEntityUsageVisibleInHierarchy
    {
      get
      {
        switch (this["child_entity_usage_visible_in_hierarchy"].ToLower())
        {
          case "true": return NxBoolEx.True;
          case "false": return NxBoolEx.True;
        }
        return NxBoolEx.Undefined;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Windows scope property. Indicates whether the commands should be placed into
    /// entity related GUI in addition to using the global merged GUI.
    /// </summary>
    public bool WinPlaceCommandsPerFrame
    {
      get { return this["win_place_commands_per_grid"] == "true"; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if all the linked plugins contain metadata project file, that should
    /// be loaded in the first hand.
    /// </summary>
    public bool PluginsHaveMetadataProjectFile
    {
      get { return CxBool.Parse(this["plugins_have_metadata_project_file"], false); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns dictionary of metadata plugins.
    /// </summary>
    public IndexedDictionary<string, Assembly> Plugins
    { get { return m_Plugins; } }
    //-------------------------------------------------------------------------
  }
}
