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

namespace Framework.Utils
{
  //-------------------------------------------------------------------------
  static public class CxArray
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given array contains the given element
    /// </summary>
    /// <typeparam name="T">type of the element in the array</typeparam>
    /// <param name="array">an array to search in</param>
    /// <param name="element">an element to seek for</param>
    static public bool Contains<T>(T[] array, T element)
    {
      for (int i = 0; i < array.Length; i++)
      {
        if (CxUtils.Compare(array[i], element))
          return true;
      }
      return false;
    }
    //-------------------------------------------------------------------------
  }
}
