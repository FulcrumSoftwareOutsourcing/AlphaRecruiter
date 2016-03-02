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
  /// <summary>
  /// Occurs when some argument, passed to a method, is not valid.
  /// </summary>
  public class ExArgumentException : ExException
  {
    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="argumentName">a name of the argument that has got some
    /// invalid value</param>
    /// <param name="argumentValue">an invalid value of the argument</param>
    public ExArgumentException(string argumentName, string argumentValue)
      : base(string.Format("The argument <{0}> has inappropriate value: <{1}>", argumentName, argumentValue), (ExException)null)
    {
    }
    //----------------------------------------------------------------------------
    #endregion
  }
}
