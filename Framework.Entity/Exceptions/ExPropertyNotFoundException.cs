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
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Exception to raise when property not found.
  /// </summary>
  public class ExPropertyNotFoundException : ExException
  {
    //----------------------------------------------------------------------------
    protected string m_PropertyName; // Name of the not found property
    //----------------------------------------------------------------------------
    public ExPropertyNotFoundException(string propertyName) :
      base(propertyName + " property not found")
    {
      m_PropertyName = propertyName;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Name of the not found property.
    /// </summary>
    public string PropertyName
    {
      get { return m_PropertyName; }
    }
    //----------------------------------------------------------------------------
  }
}
