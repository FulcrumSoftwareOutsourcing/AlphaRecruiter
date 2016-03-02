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
using System.Security.Cryptography;
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  /// <summary>
  /// Provides base logic for uploading some data.
  /// </summary>
  public abstract class CxUploadHandler : IDisposable
  {
    private readonly SHA1Managed m_SHA = new SHA1Managed();
    private long m_chunksCount;
    
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default .ctor
    /// </summary>
    protected CxUploadHandler()
    {
      UploadId = Guid.NewGuid();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets global Id for one uploading.
    /// </summary>
    public Guid UploadId { get; set; }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the current uploading parameters.
    /// </summary>
    protected CxUploadParams UploadParams { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Starts uploading routine.
    /// </summary>
    /// <param name="data">CxUploadData with first file chunk.</param>
    /// <param name="response">CxUploadResponse that will used as response.</param>
    public abstract void OnStartUploading(CxUploadData data, CxUploadResponse response);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Does something with given file chunk (usually save it).
    /// </summary>
    /// <param name="data">CxUploadData with file chunk.</param>
    /// <param name="response">CxUploadResponse that will used as response.</param>
    public abstract void OnHandleChunk(CxUploadData data, CxUploadResponse response);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finish uploading routine.
    /// </summary>
    /// <param name="data">CxUploadData with first file chunk.</param>
    /// <param name="response">CxUploadResponse that will used as response.</param>
    public abstract void OnFinishUpload(CxUploadData data, CxUploadResponse response);
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns stored data.
    /// </summary>
    /// <returns>Stored data.</returns>
    public abstract byte[] GetData();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Handles current file chunk.
    /// </summary>
    /// <param name="data">CxUploadData with first file chunk.</param>
    /// <param name="uploadParams">CxUploadParams with upload parameters.</param>
    /// <returns>CxUploadResponse with handling result.</returns>
    public CxUploadResponse HandleUpload(CxUploadData data, CxUploadParams uploadParams)
    {
      CxUploadResponse response = new CxUploadResponse
                                    {
                                      UploadId = data.UploadId,
                                      ChunkNumber = data.ChunkNumber,
                                    };
      if(data.ChunkNumber == 0)//first uploading request with first file chunk
      {
        UploadParams = uploadParams;
        m_chunksCount = uploadParams.FileLenght/uploadParams.ChunkSize;
        OnStartUploading(data, response);
      }
      OnHandleChunk(data, response);//do something with file chunk(save it or some else)
      if (data.ChunkNumber  == m_chunksCount)//last uploading request with last file chunk
      {
        OnFinishUpload(data, response);
      }

      //all stages completed for current chunk
      //send check sum to client as success signal.
      response.CheckSum = m_SHA.ComputeHash(data.Data);
      response.UploadId = UploadId;
      return response;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Calculates and return pointer for uploading data array using given chunk number.
    /// </summary>
    protected int GetPointerByChunkNumber(int chunkNumber)
    {
      return UploadParams.ChunkSize * chunkNumber;
    }
    //----------------------------------------------------------------------------

    /// <summary>
    /// Creates and return one of CxUploadHandler inheritor depends on given CxAttributeMetadata.
    /// </summary>
    /// <param name="attribute">CxAttributeMetadata</param>
    /// <returns>Created CxUploadHandler.</returns>
    public static CxUploadHandler Create(Metadata.CxAttributeMetadata attribute)
    {
      //todo: add creating from metadata definition, if needed
      return new CxDefaultContentHandler();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    ///                     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public abstract void Dispose();
    
    
  }
}
