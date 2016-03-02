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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Db
{
  /// <summary>
  /// A criteria operator that holds a group of other 
  /// criteria operators.
  /// </summary>
  [DataContract]
  public class CxGroupOperator : CxCriteriaOperator
  {
    #region Properties
    //----------------------------------------------------------------------------
    /// <summary>
    /// A list of criteria operators retained by this group operator.
    /// </summary>
    [DataMember]
    public CxCriteriaOperatorList Operators { get; set; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// A type of relations between the retained criteria operators.
    /// </summary>
    [DataMember]
    public NxGroupOperatorType OperatorType { get; set; }
    //----------------------------------------------------------------------------
    #endregion

    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxGroupOperator()
    {
      Operators = new CxCriteriaOperatorList();
      OperatorType = NxGroupOperatorType.And;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="operatorType">group operator type</param>
    public CxGroupOperator(NxGroupOperatorType operatorType)
      : this()
    {
      OperatorType = operatorType;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="operators">operators to include to the group</param>
    public CxGroupOperator(CxCriteriaOperator[] operators)
      : this()
    {
      for (int i = 0; i < operators.Length; i++)
      {
        if (operators[i] != null)
          Operators.Add(operators[i]);
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="operators">operators to include to the group</param>
    /// <param name="operatorType">group operator type</param>
    public CxGroupOperator(CxCriteriaOperator[] operators, NxGroupOperatorType operatorType)
      : this(operators)
    {
      OperatorType = operatorType;
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
      CxCriteriaOperator[] operators = new CxCriteriaOperator[Operators.Count];
      for (int i = 0; i < Operators.Count; i++)
      {
        operators[i] = Operators[i].Clone();
      }
      return new CxGroupOperator(operators, OperatorType);
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
      foreach (CxCriteriaOperator topOperator in Operators)
      {
        foreach (CxCriteriaOperator @operator in topOperator)
          yield return @operator;
      }
    }
    //----------------------------------------------------------------------------
  }
}
