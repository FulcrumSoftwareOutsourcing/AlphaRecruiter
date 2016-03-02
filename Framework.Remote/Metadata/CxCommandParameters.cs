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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Remote
{
    /// <summary>
    /// Container that contains parameters for command from client to server.
    /// </summary>
    [DataContract]
    public class CxCommandParameters
    {
        private string m_commandId;
        private string m_entityUsageId;
        private Dictionary<string, object> m_currentEntity = new Dictionary<string, object>();
        private List<Dictionary<string, object>> m_selectedEntities = new List<Dictionary<string, object>>();
        private CxQueryParams m_queryParams;
        private bool m_isNewEntity = false;
        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets Id of the command.
        /// </summary>
        [DataMember]
        public string CommandId
        {
            get { return m_commandId; }
            set { m_commandId = value; }
        }

        ////----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the entity usage Id.
        /// </summary>
        [DataMember]
        public string EntityUsageId
        {
            get { return m_entityUsageId; }
            set { m_entityUsageId = value; }
        }

        ////----------------------------------------------------------------------------
        /// <summary>
        /// Gets current entity values.
        /// </summary>
        [DataMember]
        public Dictionary<string, object> CurrentEntity
        {
            get { return m_currentEntity; }
            set { m_currentEntity = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets list of Selected Entities values.
        /// </summary>
        [DataMember]
        public List<Dictionary<string, object>> SelectedEntities
        {
            get { return m_selectedEntities; }
            set { m_selectedEntities = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets CxQueryParams which contains parameters for server queries.
        /// </summary>
        [DataMember]
        public CxQueryParams QueryParams
        {
            get { return m_queryParams; }
            set { m_queryParams = value; }
        }

        //----------------------------------------------------------------------------
        /// <summary>
        /// Defines that property 'CurrentEntity' it is new entity. 
        /// </summary>
        [DataMember]
        public bool IsNewEntity
        {
            get { return m_isNewEntity; }
            set { m_isNewEntity = value; }
        }
    }
}
