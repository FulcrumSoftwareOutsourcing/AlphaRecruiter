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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace Framework.Utils
{
  //---------------------------------------------------------------------------
  /// <summary>
  /// Class representing file header.
  /// </summary>
  public class CxBlobFileHeader
  {
    //-------------------------------------------------------------------------
    protected NameValueCollection m_Values = new NameValueCollection();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads header from array of bytes.
    /// </summary>
    /// <param name="byteArray">array of bytes</param>
    public void LoadFrom(byte[] byteArray)
    {
      m_Values.Clear();
      if (byteArray != null && byteArray.Length > 0)
      {
        MemoryStream stream = new MemoryStream(byteArray);
        IFormatter formatter = new BinaryFormatter();
        string[] values = (string[])(formatter.Deserialize(stream));
        for (int i = 0; i + 1 < values.Length; i += 2)
        {
          m_Values[values[i]] = values[i + 1];
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads header from another header object.
    /// </summary>
    /// <param name="header">header object to load from</param>
    public void LoadFrom(CxBlobFileHeader header)
    {
      m_Values.Clear();
      if (header != null)
      {
        m_Values.Add(header.Values);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns as a byte array.
    /// </summary>
    /// <returns>byte array</returns>
    public byte[] GetAsByteArray()
    {
      ArrayList valuesList = new ArrayList();
      foreach (string name in m_Values.AllKeys)
      {
        valuesList.Add(name);
        valuesList.Add(m_Values[name]);
      }
      string[] values = new string[valuesList.Count];
      valuesList.CopyTo(values);
      IFormatter formatter = new BinaryFormatter();
      MemoryStream stream = new MemoryStream();
      formatter.Serialize(stream, values);
      return stream.ToArray();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears all header data.
    /// </summary>
    public void Clear()
    {
      m_Values.Clear();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all header values collection.
    /// </summary>
    public NameValueCollection Values
    { get { return m_Values; } }
    //-------------------------------------------------------------------------
    /*
    /// <summary>
    /// Gets or sets content offset in the BLOB field stream.
    /// </summary>
    public int ContentOffset
    {
      get { return CxUtils.ParseInt(m_Values["ContentOffset"], 0); }
      set { m_Values["ContentOffset"] = value.ToString(); }
    }
    */
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets file name.
    /// </summary>
    public string FileName
    { get { return m_Values["FileName"]; } set { m_Values["FileName"] = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets content type.
    /// </summary>
    public string ContentType
    { get { return m_Values["ContentType"]; } set { m_Values["ContentType"] = value; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets file length.
    /// </summary>
    public int ContentLength
    {
      get { return CxInt.Parse(m_Values["ContentLength"], 0); }
      set { m_Values["ContentLength"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets file upload date and time
    /// </summary>
    public DateTime UploadDateTime
    {
      get
      {
        string s = m_Values["UploadDateTime"];
        return CxUtils.NotEmpty(s) ?
          DateTime.Parse(s, DateTimeFormatInfo.InvariantInfo) : DateTime.MinValue;
      }
      set
      {
        m_Values["UploadDateTime"] = value.ToString(DateTimeFormatInfo.InvariantInfo);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets image width (for image files).
    /// </summary>
    public int ImageWidth
    {
      get { return CxInt.Parse(m_Values["ImageWidth"], 0); }
      set { m_Values["ImageWidth"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets image height (for image files).
    /// </summary>
    public int ImageHeight
    {
      get { return CxInt.Parse(m_Values["ImageHeight"], 0); }
      set { m_Values["ImageHeight"] = value.ToString(); }
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  /// <summary>
  /// Class incapsulating operations with file stored in DB BLOB field.
  /// </summary>
  public class CxBlobFile
  {
    //-------------------------------------------------------------------------
    protected CxBlobFileHeader m_Header = new CxBlobFileHeader();
    protected byte[] m_Data = null;
    //-------------------------------------------------------------------------
    #region Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the name of the file if any.
    /// Otherwise returns an empty string.
    /// </summary>
    public string FileName
    {
      get
      {
        if (Header != null)
          return Header.FileName;
        else
          return string.Empty;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the size of the file content if any.
    /// Otherwise returns zero.
    /// </summary>
    public int FileSize
    {
      get
      {
        if (Header != null)
          return Header.ContentLength;
        else
          return 0;
      }
    }
    //-------------------------------------------------------------------------
    #endregion
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads BLOB file object from the DB field value.
    /// </summary>
    /// <param name="fieldValue">DB field value</param>
    public void LoadFromDbField(byte[] fieldValue)
    {
      m_Header.LoadFrom(fieldValue);
      if (fieldValue != null && fieldValue.Length > 0)
      {
        int headerLength = m_Header.GetAsByteArray().Length;
        m_Data = new byte[m_Header.ContentLength];
        Array.Copy(fieldValue, headerLength, m_Data, 0, m_Header.ContentLength);
      }
      else
      {
        m_Data = null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads BLOB file object from the file input stream.
    /// </summary>
    /// <param name="header">file header</param>
    /// <param name="stream">input stream with file data</param>
    public void LoadFromFileStream(
      CxBlobFileHeader header,
      Stream stream)
    {
      m_Header.LoadFrom(header);
      int length = (int)stream.Length;
      m_Data = new byte[length];
      stream.Seek(0, 0);
      stream.Read(m_Data, 0, length);
      m_Header.ContentLength = length;
      m_Header.UploadDateTime = DateTime.Now;
      //m_Header.ContentOffset = m_Header.GetAsByteArray().Length;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns an instance of CxBlobFile if the byte array was its valid representation.
    /// Otherwise returns null;
    /// </summary>
    public static CxBlobFile LoadBlobIfValid(byte[] headeredData)
    {
      if (headeredData != null && headeredData.Length > 0)
      {
        CxBlobFile result = new CxBlobFile();
        
        try
        {
          result.LoadFromDbField(headeredData);
        }
        catch (SerializationException)
        {
          return null;
        }

        if (result.Header == null || result.Header.Values.Count == 0)
          return null;
        return result;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Loads BLOB file object from set of bytes
    /// </summary>
    /// <param name="header">data header</param>
    /// <param name="data">data</param>
    public void Load(CxBlobFileHeader header, byte[] data)
    {
      if (data == null || header == null || data.Length == 0)
      {
        throw new ExException("Supplied parameter is empty");
      }
      m_Header.LoadFrom(header);
      m_Data = data;
      m_Header.ContentLength = data.Length;
      m_Header.UploadDateTime = DateTime.Now;
    }

    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears file data.
    /// </summary>
    public void Clear()
    {
      Header.Clear();
      m_Data = null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns image data converted to the specified width and height.
    /// </summary>
    /// <param name="imageWidth">image width</param>
    /// <param name="imageHeight">image height</param>
    public byte[] GetImageData(int imageWidth, int imageHeight)
    {
      if (imageWidth > 0 && imageHeight > 0)
      {
        byte[] thumbData = null;
        try
        {
          thumbData = CxImage.GetThumbnail(Data, imageWidth, imageHeight);
        }
        catch
        {
          thumbData = null;
        }
        if (thumbData != null)
        {
          return thumbData;
        }
      }
      return Data;
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns file header object.
    /// </summary>
    public CxBlobFileHeader Header
    { get { return m_Header; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns binary data of the file.
    /// </summary>
    public byte[] Data
    { get { return m_Data; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// True if file content is empty.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return Header == null || Header.ContentLength == 0 || m_Data == null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field value to write to database.
    /// </summary>
    public byte[] FieldValue
    {
      get
      {
        if (!IsEmpty)
        {
          byte[] header = Header.GetAsByteArray();
          byte[] result = new byte[header.Length + Data.Length];
          header.CopyTo(result, 0);
          Data.CopyTo(result, header.Length);
          return result;
        }
        return null;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates image from the given data. 
    /// Tries to convert CxBlobFile, else convert byte array to image. 
    /// </summary>
    /// <param name="value">the given data</param>
    /// <returns>image</returns>
    static public Image GetImageFromBlob(object value)
    {
      Image image;
      try
      {
        CxBlobFile blobFile = new CxBlobFile();
        blobFile.LoadFromDbField((byte[])value);
        MemoryStream stream = new MemoryStream(blobFile.Data);
        stream.Seek(0, SeekOrigin.Begin);
        image = Image.FromStream(stream);
      }
      catch
      {
        byte[] imageArray = null;
        if (!(value is byte[]))
        {
          return null;
        }
        try
        {
          imageArray = (byte[])value;
        }
        catch
        {
          return null;
        }
        MemoryStream stream = new MemoryStream(imageArray);
        stream.Seek(0, SeekOrigin.Begin);

        try
        {
          image = Image.FromStream(stream);
        }
        catch (System.Exception ex)
        {
          return null;
        }        
      }

      return image;
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
}