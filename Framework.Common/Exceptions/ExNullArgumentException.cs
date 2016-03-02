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
  /// Should be thrown if the method's input argument is null 
  /// but not supposed to be.
  /// </summary>
  public class ExNullArgumentException : ExException
  {
    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="argumentName">a name of the argument equal to null</param>
    public ExNullArgumentException(string argumentName)
      : base(string.Format("The argument <{0}> is null.", argumentName), (ExException)null)
    {
    }
    //----------------------------------------------------------------------------
    #endregion
  }
}
