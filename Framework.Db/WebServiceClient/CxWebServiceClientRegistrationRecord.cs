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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Framework.Utils;

namespace Framework.Db.WebServiceClient
{
	/// <summary>
	/// Registration record of the web service registered client
	/// </summary>
	[Serializable]
	public class CxWebServiceClientRegistrationRecord
	{
    //-------------------------------------------------------------------------
    protected Guid m_ClientID = Guid.Empty;
    protected byte[] m_ClientKey = null;
    protected UInt64 m_EncryptCounter = 0;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    public CxWebServiceClientRegistrationRecord()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="clientId">unique ID of the client to pass to subsequent web service calls</param>
    /// <param name="clientKey">client public key to encrypt user password</param>
		public CxWebServiceClientRegistrationRecord(
      Guid clientId,
      byte[] clientKey)
		{
      m_ClientID = clientId;
      m_ClientKey = clientKey;
		}
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encrypts user password.
    /// </summary>
    public string EncryptPassword(string password)
    {
      if (CxUtils.NotEmpty(password))
      {
        // Deserialize RSA parameters
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(m_ClientKey);
        RSAParameters publicKey = (RSAParameters) formatter.Deserialize(stream);

        // Increment encrypt counter
        lock (this)
        {
          m_EncryptCounter++;
        }

        // Get source for encryption
        stream = new MemoryStream();
        formatter.Serialize(stream, password);
        formatter.Serialize(stream, m_EncryptCounter);
        byte[] encryptSource = stream.ToArray();

        // Encrypt source
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(publicKey);
        byte[] encryptedData = rsa.Encrypt(encryptSource, false);

        return Convert.ToBase64String(encryptedData);
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns unique ID of the client to pass to subsequent web service calls.
    /// </summary>
    public Guid ClientID
    { get { return m_ClientID; } set { m_ClientID = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns client public key to encrypt user password.
    /// </summary>
    public byte[] ClientKey
    { get { return m_ClientKey; } set { m_ClientKey = value; } }
    //-------------------------------------------------------------------------
  }
}