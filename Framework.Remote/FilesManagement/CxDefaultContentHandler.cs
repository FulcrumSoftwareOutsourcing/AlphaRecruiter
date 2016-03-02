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
using Framework.Remote.Mobile;

namespace Framework.Remote
{
  /// <summary>
  /// Provides base logic for uploading files.
  /// </summary>
  public class CxDefaultContentHandler : CxUploadHandler
  {
    private List<byte> m_Bytes;
    //----------------------------------------------------------------------------
    /// <summary>
    /// Starts uploading routine.
    /// </summary>
    /// <param name="data">CxUploadData with first file chunk.</param>
    /// <param name="response">CxUploadResponse that will used as response.</param>
    public override void OnStartUploading(CxUploadData data, CxUploadResponse response)
    {
      m_Bytes = new List<byte>((int)UploadParams.FileLenght);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Does something with given file chunk (usually save it).
    /// </summary>
    /// <param name="data">CxUploadData with file chunk.</param>
    /// <param name="response">CxUploadResponse that will used as response.</param>
    public override void OnHandleChunk(CxUploadData data, CxUploadResponse response)
    {
      m_Bytes.AddRange(data.Data);
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns stored data.
    /// </summary>
    /// <returns>Stored data.</returns>
    public override byte[] GetData()
    {
      return m_Bytes.ToArray();
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Finish uploading routine.
    /// </summary>
    /// <param name="data">CxUploadData with first file chunk.</param>
    /// <param name="response">CxUploadResponse that will used as response.</param>
    public override void OnFinishUpload(CxUploadData data, CxUploadResponse response){}

    //----------------------------------------------------------------------------
    /// <summary>
    ///                     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <filterpriority>2</filterpriority>
    public override void Dispose()
    {
      m_Bytes.Clear();
    }
  }
}
