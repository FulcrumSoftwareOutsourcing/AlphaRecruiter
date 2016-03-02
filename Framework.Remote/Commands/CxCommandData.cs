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

using System.Collections.Generic;
using Framework.Entity;
using Framework.Metadata;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
    /// <summary>
    /// Class holding information required to perform command.
    /// </summary>
    public class CxCommandData
    {
        private Metadata.CxCommandMetadata m_command;
        private CxEntityUsageMetadata m_entityUsage;
        private readonly List<CxBaseEntity> m_selectedEntities = new List<CxBaseEntity>();
        private CxBaseEntity m_currentEntity;
        private bool m_isMultiple;
        private CxQueryParams m_qParams;
        private string m_commandId;
        private bool m_isNewEntity;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets command.
        /// </summary>
        public Metadata.CxCommandMetadata Command
        {
            get { return m_command; }
            set { m_command = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the entity usage.
        /// </summary>
        public CxEntityUsageMetadata EntityUsage
        {
            get { return m_entityUsage; }
            set { m_entityUsage = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets list of Selected Entities.
        /// </summary>
        public List<CxBaseEntity> SelectedEntities
        {
            get { return m_selectedEntities; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the value that define, that this command has multiple execution.
        /// </summary>
        public bool IsMultiple
        {
            get { return m_isMultiple; }
            set { m_isMultiple = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets current entity.
        /// </summary>
        public CxBaseEntity CurrentEntity
        {
            get { return m_currentEntity; }
            set { m_currentEntity = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the CxQueryParams that contains parameters for server queries.
        /// </summary>
        public CxQueryParams QueryParams
        {
            get { return m_qParams; }
            set { m_qParams = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Id of the command.
        /// </summary>
        public string CommandId
        {
            get { return m_commandId; }
            set { m_commandId = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Defines that property 'CurrentEntity' it is new entity. 
        /// </summary>
        public bool IsNewEntity
        {
            get { return m_isNewEntity; }
            set { m_isNewEntity = value; }
        }
    }
}
