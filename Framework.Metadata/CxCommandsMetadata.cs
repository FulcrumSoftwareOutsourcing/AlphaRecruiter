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
using System.Collections.Generic;

namespace Framework.Metadata
{
	/// <summary>
	/// Collection of common commands metadata
	/// </summary>
	public class CxCommandsMetadata : CxMetadataCollection
	{
    //-------------------------------------------------------------------------
    protected List<CxCommandMetadata> m_CommandList = new List<CxCommandMetadata>();
    protected Hashtable m_CommandMap = new Hashtable();
    protected Hashtable m_CommandCodeMap = new Hashtable();
    protected Hashtable m_DefinedCommandIdMap = new Hashtable();
    protected Hashtable m_UniqueIdMap = new Hashtable();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">parent metadata holder object</param>
    /// <param name="doc">XML document to load metadata from</param>
		public CxCommandsMetadata(
      CxMetadataHolder holder, 
      XmlDocument doc) : base(holder, doc)
		{
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="docs">name of file to read assemblies metadata</param>
    public CxCommandsMetadata(CxMetadataHolder holder, IEnumerable<XmlDocument> docs)
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
      if (doc.DocumentElement != null)
      {
        XmlNodeList commands = doc.DocumentElement.SelectNodes("command");
        if (commands == null)
          throw new ExNullReferenceException("commands");
        foreach (XmlElement element in commands)
        {
          if (Holder.GetIsElementInScope(element))
          {
            CxCommandMetadata command = new CxCommandMetadata(Holder, element);
            if (command.UniqueID > 0)
            {
              if (m_UniqueIdMap.ContainsKey(command.UniqueID))
              {
                throw new ExMetadataException(
                  String.Format("Command unique ID property is not unique. Command ID='{0}'",
                                command.Id));
              }
              m_UniqueIdMap[command.UniqueID] = true;
            }
            m_CommandList.Add(command);
            m_CommandMap.Add(command.Id, command);
          }
          m_DefinedCommandIdMap[CxXml.GetAttr(element, "id").ToUpper()] = true;
        }
      }
      LoadOverrides(doc, "command_override", m_CommandMap);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Does actions after metadata loaded.
    /// </summary>
    override protected void DoAfterLoad()
    {
      base.DoAfterLoad();
      // Initialize command code map
      foreach (CxCommandMetadata command in m_CommandMap.Values)
      {
        if (CxUtils.NotEmpty(command.OperationCode))
        {
          m_CommandCodeMap[command.OperationCode] = command;
        }
      }
      // Initialize child commands lists
      foreach (CxCommandMetadata command in m_CommandMap.Values)
      {
        if (CxUtils.NotEmpty(command.ParentCommandId))
        {
          this[command.ParentCommandId].ChildCommands.Add(command);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Command with the given ID.
    /// </summary>
    public CxCommandMetadata this[string id]
    {
      get
      { 
        CxCommandMetadata command = Find(id);
        if (command != null)
          return command;
        else
          throw new ExMetadataException(string.Format("Command with ID=\"{0}\" not defined", id));
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns command with the given ID.
    /// </summary>
    /// <param name="id">ID of command to find</param>
    /// <returns>command metadata</returns>
    public CxCommandMetadata Find(string id)
    {
      return (CxCommandMetadata) m_CommandMap[id.ToUpper()];
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finds command by it's operation code attribute value.
    /// </summary>
    /// <param name="code">operation code to search</param>
    /// <returns>found command or null</returns>
    public CxCommandMetadata FindByOperationCode(string code)
    {
      return (CxCommandMetadata) m_CommandCodeMap[code.ToUpper()];
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command with the given ID is in current application scope.
    /// Raises an exception if command is not defined in the metadata al all.
    /// </summary>
    internal bool GetIsCommandInScope(string commandId)
    {
      if (!m_DefinedCommandIdMap.ContainsKey(commandId.ToUpper()))
      {
        throw new ExMetadataException(string.Format("Command with ID=\"{0}\" not defined", commandId));
      }
      return Find(commandId) != null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Adds the command metadata to the inner collections.
    /// </summary>
    public void Add(CxCommandMetadata commandMetadata)
    {
      m_CommandList.Add(commandMetadata);
      m_CommandMap[commandMetadata.Id] = commandMetadata;
    }
	  //----------------------------------------------------------------------------
    /// <summary>
    /// Returns dictionary of all commands.
    /// </summary>
    public IDictionary Commands
    { get { return m_CommandMap; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns list of all defined commands.
    /// </summary>
    public IList<CxCommandMetadata> Items
    { get { return m_CommandList; } }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default name for the metadata XML file.
    /// </summary>
    override protected string XmlFileName
    { get { return "Commands.xml"; } }
    //-------------------------------------------------------------------------
  }
}