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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Framework.Utils
{
  public class CxImage
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns size of the image represented by the given byte array.
    /// </summary>
    /// <param name="imageData">image content data</param>
    static public Size GetSize(byte[] imageData)
    {
      if (imageData != null && imageData.Length > 0)
      {
        try
        {
          using (MemoryStream stream = new MemoryStream(imageData))
          {
            using (Image image = Image.FromStream(stream))
            {
              return image.Size;
            }
          }
        }
        catch
        {
          return Size.Empty;
        }
      }
      return Size.Empty;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Delegate for GetThumbnailImage method call.
    /// </summary>
    static protected bool GetThumbnailImageAbort()
    {
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates a thumbnail from the given image.
    /// </summary>
    /// <param name="image">image to create thumbnail from</param>
    /// <param name="thumbWidth">thumbnail width</param>
    /// <param name="thumbHeight">thumbnail height</param>
    /// <returns>created thumbnail or null</returns>
    static public Image GetThumbnail(
      Image image,
      int thumbWidth,
      int thumbHeight)
    {
      if (image != null && thumbWidth > 0 && thumbHeight > 0)
      {
        Image thumb = image.GetThumbnailImage(
          thumbWidth,
          thumbHeight,
          new Image.GetThumbnailImageAbort(GetThumbnailImageAbort),
          IntPtr.Zero);
        using (Graphics g = Graphics.FromImage(thumb))
        {
          Brush brush = new SolidBrush(Color.White);
          g.FillRectangle(brush, 0, 0, thumbWidth, thumbHeight);
          double scale =
            image.Width > image.Height ?
              (double)thumbWidth / (double)image.Width :
              (double)thumbHeight / (double)image.Height;
          scale = scale > 1 ? 1 : scale;
          int newWidth = (int)(image.Width * scale);
          int newHeight = (int)(image.Height * scale);
          int newLeft = (thumbWidth - newWidth) / 2;
          int newTop = (thumbHeight - newHeight) / 2;
          g.DrawImage(image, newLeft, newTop, newWidth, newHeight);
        }
        return thumb;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates a thumbnail from the given image data.
    /// </summary>
    /// <param name="imageData">image data to create thumbnail from</param>
    /// <param name="thumbWidth">thumbnail width</param>
    /// <param name="thumbHeight">thumbnail height</param>
    /// <returns>created thumbnail data or null</returns>
    static public byte[] GetThumbnail(
      byte[] imageData,
      int thumbWidth,
      int thumbHeight)
    {
      if (imageData != null && imageData.Length > 0 &&
          thumbWidth > 0 && thumbHeight > 0)
      {
        MemoryStream stream = new MemoryStream(imageData);
        Image image = Image.FromStream(stream);
        Image thumb = GetThumbnail(image, thumbWidth, thumbHeight);
        if (thumb != null)
        {
          MemoryStream thumbStream = new MemoryStream();
          thumb.Save(thumbStream, image.RawFormat);
          return thumbStream.ToArray();
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
  }
}