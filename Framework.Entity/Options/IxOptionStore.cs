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

namespace Framework.Entity
{
  public interface IxOptionStore
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Saves the given options object to the incapsulated store.
    /// </summary>
    /// <param name="options">options object to be saved</param>
    void Save(CxOptions options);

    //-------------------------------------------------------------------------
    /// <summary>
    /// Reads an options object from the incapsulated store.
    /// </summary>
    /// <returns>the options object read</returns>
    CxOptions Read();
    //-------------------------------------------------------------------------
  }
}
