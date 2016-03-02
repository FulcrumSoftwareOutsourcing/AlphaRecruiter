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

using Framework.Utils;
using System.Collections.Generic;

namespace Framework.Metadata
{
	/// <summary>
	/// Metadata describing command group.
	/// </summary>
	public class CxCommandGroupMetadata : CxMetadataObject
	{
    //-------------------------------------------------------------------------
    protected CxEntityMetadata m_EntityMetadata = null;
    protected Hashtable m_IncludedIdMap = new Hashtable();
    protected Hashtable m_ExcludedIdMap = new Hashtable();
    protected List<CxErrorConditionMetadata> m_DisableConditions = new List<CxErrorConditionMetadata>();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="holder">metadata holder</param>
    /// <param name="element">source XML element</param>
		public CxCommandGroupMetadata(
      CxMetadataHolder holder, 
      XmlElement element,
      CxEntityMetadata entityMetadata) : base (holder, element)
		{
      m_EntityMetadata = entityMetadata;

      CxErrorConditionMetadata.LoadListFromNode(
        holder, 
        element.SelectSingleNode("disable_conditions"),
        m_DisableConditions,
        this);

      CxList.AppendDictionaryFromList(
        m_IncludedIdMap, 
        CxText.DecomposeWithSeparator(CxText.ToUpper(IncludedCommandIDs), ","));

      CxList.AppendDictionaryFromList(
        m_ExcludedIdMap,
        CxText.DecomposeWithSeparator(CxText.ToUpper(ExcludedCommandIDs), ","));
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if command with the given ID is included into the group.
    /// </summary>
    public bool GetIsCommandInGroup(CxCommandMetadata command)
    {
      if (command != null)
      {
        if (m_ExcludedIdMap.ContainsKey(command.Id))
        {
          return false;
        }
        if (m_IncludedIdMap.ContainsKey(command.Id))
        {
          return true;
        }
        return CommandType.ToUpper() == "ALL" ||
               command.CommandType.ToString().ToUpper() == CommandType.ToUpper();
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns security command type of commands to be included into the group.
    /// </summary>
    public string CommandType
    { get {return this["command_type"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Return comma-separated list of commands to be included into the group.
    /// </summary>
    public string IncludedCommandIDs
    { get {return this["included_commands"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Return comma-separated list of commands to be excluded from the group.
    /// </summary>
    public string ExcludedCommandIDs
    { get {return this["excluded_commands"];} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of command group disable conditions.
    /// List contains CxErrorConditionMetadata objects.
    /// </summary>
    public IList<CxErrorConditionMetadata> DisableConditions
    { get {return m_DisableConditions;} }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns entity metadata group was created within.
    /// </summary>
    internal CxEntityMetadata EntityMetadata
    { get { return m_EntityMetadata; } }
    //-------------------------------------------------------------------------
  }
}