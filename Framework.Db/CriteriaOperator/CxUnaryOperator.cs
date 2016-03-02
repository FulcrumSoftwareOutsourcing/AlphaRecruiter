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
using System.Runtime.Serialization;

namespace Framework.Db
{
  /// <summary>
  /// A criteria operator, represents some unary operations.
  /// </summary>
  [DataContract]
  public class CxUnaryOperator: CxCriteriaOperator
  {
    #region Properties
    //----------------------------------------------------------------------------
    /// <summary>
    /// A type of the unary operator.
    /// </summary>
    [DataMember]
    public NxUnaryOperatorType OperatorType { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The main operand of the operator.
    /// </summary>
    [DataMember]
    public CxCriteriaOperator Operand { get; set; }
    //----------------------------------------------------------------------------
    #endregion

    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxUnaryOperator()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="operand"></param>
    public CxUnaryOperator(NxUnaryOperatorType type, CxCriteriaOperator operand)
      : this()
    {
      OperatorType = type;
      Operand = operand;
    }
    //----------------------------------------------------------------------------
    #endregion

    //----------------------------------------------------------------------------
    /// <summary>
    /// Clones the criteria operator.
    /// </summary>
    /// <returns>returns a clone</returns>
    public override CxCriteriaOperator Clone()
    {
      return new CxUnaryOperator(OperatorType, Operand.Clone());
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
    public override IEnumerator<CxCriteriaOperator> GetEnumerator()
    {
      yield return this;
      foreach (CxCriteriaOperator @operator in Operand)
        yield return @operator;
    }
    //----------------------------------------------------------------------------
  }
}
