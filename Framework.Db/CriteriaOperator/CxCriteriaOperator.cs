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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Db
{
  /// <summary>
  /// A base class for all types of criteria operators.
  /// </summary>
  [DataContract]
  abstract public class CxCriteriaOperator: IEnumerable<CxCriteriaOperator>
  {
    #region Static methods
    //----------------------------------------------------------------------------
    /// <summary>
    /// Creates one criteria operator that represents the given array
    /// of operators.
    /// </summary>
    /// <param name="operators">operators to include to the group</param>
    /// <param name="operatorType">group operator type</param>
    /// <returns>a group operator</returns>
    static public CxCriteriaOperator Combine(
      CxCriteriaOperator[] operators,
      NxGroupOperatorType operatorType)
    {
      return new CxGroupOperator(operators, operatorType);
    }
    //----------------------------------------------------------------------------
    #endregion

    //----------------------------------------------------------------------------
    /// <summary>
    /// Clones the criteria operator.
    /// </summary>
    /// <returns>returns a clone</returns>
    virtual public CxCriteriaOperator Clone()
    {
      throw new NotImplementedException();
    }
    //----------------------------------------------------------------------------
    ///<summary>
    ///Returns an enumerator that iterates through the collection.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>1</filterpriority>
    virtual public IEnumerator<CxCriteriaOperator> GetEnumerator()
    {
      throw new NotImplementedException();
    }
    //----------------------------------------------------------------------------
    ///<summary>
    ///Returns an enumerator that iterates through a collection.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<CxCriteriaOperator>) this).GetEnumerator();
    }
    //----------------------------------------------------------------------------
  }
}
