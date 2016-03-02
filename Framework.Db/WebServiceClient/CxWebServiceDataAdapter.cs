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
using System.Data;
using System.Data.Common;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Summary description for CxWebServiceDataAdapter.
	/// </summary>
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	public class CxWebServiceDataAdapter : DbDataAdapter, IDbDataAdapter
	{
    //-------------------------------------------------------------------------
    protected CxWebServiceCommand m_SelectCommand = null;
    protected CxWebServiceCommand m_InsertCommand = null;
    protected CxWebServiceCommand m_UpdateCommand = null;
    protected CxWebServiceCommand m_DeleteCommand = null;
    //-------------------------------------------------------------------------
    /*
     * Inherit from Component through DbDataAdapter. The event
     * mechanism is designed to work with the Component.Events
     * property. These variables are the keys used to find the
     * events in the components list of events.
    */
    static private readonly object m_EventRowUpdated = new object(); 
    static private readonly object m_EventRowUpdating = new object();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceDataAdapter()
		{
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceDataAdapter(IDbCommand command)
    {
      m_SelectCommand = (CxWebServiceCommand) command;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets select command.
    /// </summary>
    new public CxWebServiceCommand SelectCommand
    {
      get { return m_SelectCommand; }
      set { m_SelectCommand = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets select command.
    /// </summary>
    IDbCommand IDbDataAdapter.SelectCommand
	  {
	    get { return m_SelectCommand; }
	    set { m_SelectCommand = (CxWebServiceCommand) value; }
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets insert command.
    /// </summary>
    new public CxWebServiceCommand InsertCommand
    {
      get { return m_InsertCommand; }
      set { m_InsertCommand = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets insert command.
    /// </summary>
    IDbCommand IDbDataAdapter.InsertCommand
    {
      get { return m_InsertCommand; }
      set { m_InsertCommand = (CxWebServiceCommand) value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets update command.
    /// </summary>
    new public CxWebServiceCommand UpdateCommand
    {
      get { return m_UpdateCommand; }
      set { m_UpdateCommand = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets update command.
    /// </summary>
    IDbCommand IDbDataAdapter.UpdateCommand
    {
      get { return m_UpdateCommand; }
      set { m_UpdateCommand = (CxWebServiceCommand) value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets delete command.
    /// </summary>
    new public CxWebServiceCommand DeleteCommand
    {
      get { return m_DeleteCommand; }
      set { m_DeleteCommand = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets delete command.
    /// </summary>
    IDbCommand IDbDataAdapter.DeleteCommand
    {
      get { return m_DeleteCommand; }
      set { m_DeleteCommand = (CxWebServiceCommand) value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row updating event.
    /// </summary>
    override protected RowUpdatingEventArgs CreateRowUpdatingEvent(
      DataRow dataRow, 
      IDbCommand command, 
      StatementType statementType, 
      DataTableMapping tableMapping)
    {
      return new CxWebServiceRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row updated event.
    /// </summary>
    override protected RowUpdatedEventArgs CreateRowUpdatedEvent(
      DataRow dataRow, 
      IDbCommand command, 
      StatementType statementType, 
      DataTableMapping tableMapping)
    {
      return new CxWebServiceRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row updating event.
    /// </summary>
    override protected void OnRowUpdating(RowUpdatingEventArgs value)
    {
      DxWebServiceRowUpdatingEventHandler handler = 
        (DxWebServiceRowUpdatingEventHandler) Events[m_EventRowUpdating];
      if ((null != handler) && (value is CxWebServiceRowUpdatingEventArgs)) 
      {
        handler(this, (CxWebServiceRowUpdatingEventArgs) value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row updated event.
    /// </summary>
    override protected void OnRowUpdated(RowUpdatedEventArgs value)
    {
      DxWebServiceRowUpdatedEventHandler handler = 
        (DxWebServiceRowUpdatedEventHandler) Events[m_EventRowUpdated];
      if ((null != handler) && (value is CxWebServiceRowUpdatedEventArgs)) 
      {
        handler(this, (CxWebServiceRowUpdatedEventArgs) value);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row updating event.
    /// </summary>
    public event DxWebServiceRowUpdatingEventHandler RowUpdating
    {
      add { Events.AddHandler(m_EventRowUpdating, value); }
      remove { Events.RemoveHandler(m_EventRowUpdating, value); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Row updated event.
    /// </summary>
    public event DxWebServiceRowUpdatedEventHandler RowUpdated
    {
      add { Events.AddHandler(m_EventRowUpdated, value); }
      remove { Events.RemoveHandler(m_EventRowUpdated, value); }
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Delegates
  /// </summary>
  public delegate void DxWebServiceRowUpdatingEventHandler(
    object sender, 
    CxWebServiceRowUpdatingEventArgs e);

  public delegate void DxWebServiceRowUpdatedEventHandler(
    object sender, 
    CxWebServiceRowUpdatedEventArgs e);
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Event arguments
  /// </summary>
  public class CxWebServiceRowUpdatingEventArgs : RowUpdatingEventArgs
  {
    //-------------------------------------------------------------------------
    public CxWebServiceRowUpdatingEventArgs(
      DataRow row, 
      IDbCommand command, 
      StatementType statementType, 
      DataTableMapping tableMapping) : base(row, command, statementType, tableMapping) 
    {
    }
    //-------------------------------------------------------------------------
    // Hide the inherited implementation of the command property.
    new public CxWebServiceCommand Command
    {
      get  { return (CxWebServiceCommand)base.Command; }
      set  { base.Command = value; }
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Event arguments
  /// </summary>
  public class CxWebServiceRowUpdatedEventArgs : RowUpdatedEventArgs
  {
    //-------------------------------------------------------------------------
    public CxWebServiceRowUpdatedEventArgs(
      DataRow row, 
      IDbCommand command, 
      StatementType statementType, 
      DataTableMapping tableMapping) : base(row, command, statementType, tableMapping) 
    {
    }
    //-------------------------------------------------------------------------
    // Hide the inherited implementation of the command property.
    new public CxWebServiceCommand Command
    {
      get  { return (CxWebServiceCommand)base.Command; }
    }
  }
  //---------------------------------------------------------------------------
}