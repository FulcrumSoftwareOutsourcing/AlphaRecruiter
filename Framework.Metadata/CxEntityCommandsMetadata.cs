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

using System.Collections.Generic;
using System.Xml;

using Framework.Utils;

namespace Framework.Metadata
{
	/// <summary>
	/// Metadata reader for entity and entity usages commands.
	/// </summary>
	public class CxEntityCommandsMetadata : CxMetadataCollection
	{
    //-------------------------------------------------------------------------
    protected List<CxCommandMetadata> m_Items = new List<CxCommandMetadata>();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">XML document to read metadata from</param>
		public CxEntityCommandsMetadata(
      CxMetadataHolder holder, 
      XmlDocument doc) : base(holder, doc)
		{
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxEntityCommandsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
      :
      base(holder, docs)
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads metadata collection from the XML document.
    /// </summary>
    /// <param name="doc">document to load data from</param>
    override protected void Load(XmlDocument doc)
    {
      base.Load(doc);

      foreach (XmlElement entityElement in doc.DocumentElement.SelectNodes("entity"))
      {
        string entityId = CxXml.GetAttr(entityElement, "id");
        CxEntityMetadata entity = Holder.Entities[entityId];

        XmlElement commandsElement = (XmlElement) entityElement.SelectSingleNode("commands");
        if (commandsElement != null)
        {
          foreach (XmlElement commandElement in commandsElement.SelectNodes("command"))
          {
            if (GetIsCommandElementInScope(commandElement))
            {
              CxCommandMetadata command =
                new CxCommandMetadata(Holder, commandElement, entity);
              entity.AddCommand(command);
              m_Items.Add(command);
            }
          }
        }

        // Read command group elements.
        XmlElement groupsElement = (XmlElement) entityElement.SelectSingleNode("command_groups");
        if (groupsElement != null)
        {
          foreach (XmlElement groupElement in groupsElement.SelectNodes("command_group"))
          {
            CxCommandGroupMetadata group = new CxCommandGroupMetadata(Holder, groupElement, entity);
            foreach (CxCommandMetadata command in entity.Commands)
            {
              if (group.GetIsCommandInGroup(command))
              {
                CxErrorConditionMetadata.CombineLists(command.DisableConditions, group.DisableConditions, group);
              }
            }
            entity.CommandGroups.Add(group);
          }
        }
      }

      foreach (XmlElement entityElement in doc.DocumentElement.SelectNodes("entity_usage"))
      {
        string entityId = CxXml.GetAttr(entityElement, "id");
        CxEntityUsageMetadata entityUsage = Holder.EntityUsages[entityId];

        // Command order
        string commandOrder = CxXml.ReadTextElement(entityElement, "command_order");
        bool hasCommandOrder = entityElement.SelectSingleNode("command_order") != null;

        entityUsage.SetCommandOrder(commandOrder, hasCommandOrder);
        // Set command order to all descendant entity usages (with inherited entity usage ID).
        foreach (CxEntityUsageMetadata descendant in entityUsage.DescendantEntityUsages)
        {
          descendant.SetCommandOrder(commandOrder, false);
        }

        // Commands
        XmlElement commandsElement = (XmlElement) entityElement.SelectSingleNode("commands");
        if (commandsElement != null)
        {
          foreach (XmlElement commandElement in commandsElement.SelectNodes("command"))
          {
            if (GetIsCommandElementInScope(commandElement))
            {
              CxCommandMetadata command =
                new CxCommandMetadata(Holder, commandElement, entityUsage);
              entityUsage.AddCommand(command);
              m_Items.Add(command);
            }
          }
        }

        // Read command group elements.
        XmlElement groupsElement = (XmlElement) entityElement.SelectSingleNode("command_groups");
        if (groupsElement != null)
        {
          foreach (XmlElement groupElement in groupsElement.SelectNodes("command_group"))
          {
            CxCommandGroupMetadata group = new CxCommandGroupMetadata(Holder, groupElement, entityUsage);
            foreach (CxCommandMetadata command in entityUsage.Commands)
            {
              if (group.GetIsCommandInGroup(command))
              {
                CxErrorConditionMetadata.CombineLists(command.DisableConditions, group.DisableConditions, group);
              }
            }
            entityUsage.CommandGroups.Add(group);
          }
        }
      }

      // Load overrides
      foreach (XmlElement entityElement in doc.DocumentElement.SelectNodes("entity_override"))
      {
        string entityId = CxXml.GetAttr(entityElement, "id");
        CxEntityMetadata entity = Holder.Entities.Find(entityId);
        if (entity != null)
        {
          XmlElement commandsElement = (XmlElement) entityElement.SelectSingleNode("commands");
          if (commandsElement != null)
          {
            foreach (XmlElement commandElement in commandsElement.SelectNodes("command"))
            {
              CxCommandMetadata command = entity.GetCommand(CxXml.GetAttr(commandElement, "id"));
              if (command != null)
              {
                command.LoadOverride(commandElement);
              }
              else
              {
                command = new CxCommandMetadata(Holder, commandElement, entity);
                entity.AddCommand(command);
                m_Items.Add(command);
              }
            }
          }
        }
      }
      foreach (XmlElement entityElement in doc.DocumentElement.SelectNodes("entity_usage_override"))
      {
        string entityUsageId = CxXml.GetAttr(entityElement, "id");
        CxEntityUsageMetadata entityUsage = Holder.EntityUsages.Find(entityUsageId);
        if (entityUsage != null)
        {
          bool hasCommandOrder = entityElement.SelectSingleNode("command_order") != null;
          if (hasCommandOrder)
          {
            string commandOrder = CxXml.ReadTextElement(entityElement, "command_order");
            entityUsage.SetCommandOrder(commandOrder, hasCommandOrder);
            // Set command order to all descendant entity usages (with inherited entity usage ID).
            foreach (CxEntityUsageMetadata descendant in entityUsage.DescendantEntityUsages)
            {
              descendant.SetCommandOrder(commandOrder, false);
            }
          }
          XmlElement commandsElement = (XmlElement) entityElement.SelectSingleNode("commands");
          if (commandsElement != null)
          {
            foreach (XmlElement commandElement in commandsElement.SelectNodes("command"))
            {
              CxCommandMetadata command = entityUsage.GetCommand(CxXml.GetAttr(commandElement, "id"));
              if (command != null)
              {
                command.LoadOverride(commandElement);
              }
              else
              {
                command = new CxCommandMetadata(Holder, commandElement, entityUsage);
                entityUsage.AddCommand(command);
                m_Items.Add(command);
              }
            }
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command element is in the current application scope.
    /// </summary>
    protected bool GetIsCommandElementInScope(XmlElement element)
    {
      return Holder.Commands.GetIsCommandInScope(CxXml.GetAttr(element, "id"));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all command metadata.
    /// </summary>
    public IList<CxCommandMetadata> Items
    { get { return m_Items; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "EntityCommands.xml"; } }
    //-------------------------------------------------------------------------
  }
}