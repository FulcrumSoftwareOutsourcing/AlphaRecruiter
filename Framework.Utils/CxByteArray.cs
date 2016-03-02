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
using System.Text;

namespace Framework.Utils
{
  public class CxByteArray
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if byte arrays contain the same content.
    /// </summary>
    /// <param name="b1">first byte array</param>
    /// <param name="b2">second byte array</param>
    static public bool Equals(byte[] b1, byte[] b2)
    {
      if (b1 == null && b2 == null)
      {
        return true;
      }
      if (b1 != null && b2 != null)
      {
        if (b1.Equals(b2))
        {
          return true;
        }
        if (b1.Length == b2.Length)
        {
          for (int i = 0; i < b1.Length; i++)
          {
            if (!b1[i].Equals(b2[i]))
            {
              return false;
            }
          }
          return true;
        }
      }
      return false;
    }
    //-------------------------------------------------------------------------
  }
}