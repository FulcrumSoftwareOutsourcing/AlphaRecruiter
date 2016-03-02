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
using Framework.Utils;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Class describing web service client transaction.
	/// </summary>
	public class CxWebServiceTransaction : IDbTransaction
	{
    //-------------------------------------------------------------------------
    protected Guid m_ID = Guid.NewGuid();
    protected CxWebServiceConnection m_Connection = null;
    protected IsolationLevel m_IsolationLevel = IsolationLevel.ReadCommitted;
    protected bool m_IsClosed = false;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    internal CxWebServiceTransaction(CxWebServiceConnection connection)
		{
      connection.ValidateOpenState();
      m_Connection = connection;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    internal CxWebServiceTransaction(
      CxWebServiceConnection connection,
      IsolationLevel isolationLevel) : this(connection)
    {
      if (isolationLevel != IsolationLevel.ReadCommitted)
      {
        throw new ExException("Isolation levels different from ReadCommitted are not supported by the web service transaction.");
      }
      m_IsolationLevel = isolationLevel;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls web service to commit transaction.
    /// </summary>
    protected object WebServiceCommit()
    {
      CxDbService service = m_Connection.CreateWebService();
      service.Commit(
        m_Connection.RegistrationRecord.ClientID,
        m_Connection.Database,
        m_Connection.UserID,
        m_Connection.EncryptedPassword,
        m_ID);
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Calls web service to rollback transaction.
    /// </summary>
    protected object WebServiceRollback()
    {
      CxDbService service = m_Connection.CreateWebService();
      service.Rollback(
        m_Connection.RegistrationRecord.ClientID,
        m_Connection.Database,
        m_Connection.UserID,
        m_Connection.EncryptedPassword,
        m_ID);
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Commits transaction.
    /// </summary>
    public void Commit()
    {
      if (!m_IsClosed)
      {
        m_Connection.CallWebServiceMethod(new DxWebServiceCallMethod(WebServiceCommit));
        m_IsClosed = true;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Rollbacks transaction.
    /// </summary>
    public void Rollback()
    {
      if (!m_IsClosed)
      {
        m_Connection.CallWebServiceMethod(new DxWebServiceCallMethod(WebServiceRollback));
        m_IsClosed = true;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection.
    /// </summary>
    public CxWebServiceConnection Connection
    {
      get
      {
        return m_Connection;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns connection.
    /// </summary>
    IDbConnection IDbTransaction.Connection
    {
      get
      {
        return m_Connection;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns transaction isolation level.
    /// </summary>
    public IsolationLevel IsolationLevel
    {
      get
      {
        return m_IsolationLevel;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Disposes transaction.
    /// </summary>
    public void Dispose()
    {
      Rollback();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique transaction ID.
    /// </summary>
    public Guid ID
    {
      get
      {
        return m_ID;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if transaction was closed (committed or rolled back)
    /// </summary>
    public bool IsClosed
    {
      get
      {
        return m_IsClosed;
      }
    }
    //-------------------------------------------------------------------------
  }
}