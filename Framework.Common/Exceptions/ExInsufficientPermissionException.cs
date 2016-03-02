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
  //----------------------------------------------------------------------------
  /// <summary>
  /// Should be thrown if current user doesn't have access to some object
  /// despite he/she must have it.
  /// </summary>
  public class ExInsufficientPermissionException : ExException
  {
    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">error message</param>
    public ExInsufficientPermissionException(string message, string debugMessage) 
      : base(message, debugMessage)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="objectName">a name of the object current user doesn't have access to</param>
    public ExInsufficientPermissionException(string objectName)
      : base(string.Format("Current user has no access to the <{0}> object", objectName), (ExException) null)
    {
    }
    //----------------------------------------------------------------------------
    #endregion
  }
}
