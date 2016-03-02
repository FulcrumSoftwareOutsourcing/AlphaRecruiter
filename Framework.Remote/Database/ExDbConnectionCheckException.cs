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
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Represents error that occur during checking DB coonnection.
  /// </summary>
  public class ExDbConnectionCheckException : ExException
  {
    public ExDbConnectionCheckException() { }

    public ExDbConnectionCheckException(string message, params object[] args)
      : base(string.Format(message, args)) { }

    public ExDbConnectionCheckException(Exception innerException, string message, params object[] args)
      : base(string.Format(message, args), innerException) { }

  }
}
