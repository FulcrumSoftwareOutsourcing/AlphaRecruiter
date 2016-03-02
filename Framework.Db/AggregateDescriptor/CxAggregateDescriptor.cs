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

using Framework.Db;

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// Represents a query's summary item.
  /// </summary>
  public class CxAggregateDescriptor
  {
    //----------------------------------------------------------------------------
    private string m_FieldName;
    private NxAggregateDescriptorType m_DescriptorType;
    //----------------------------------------------------------------------------
    #region Properties
    //----------------------------------------------------------------------------
    /// <summary>
    /// A name of the field to be described.
    /// </summary>
    public string FieldName
    {
      get { return m_FieldName; }
      set { m_FieldName = value; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// The type of the descriptor.
    /// </summary>
    public NxAggregateDescriptorType DescriptorType
    {
      get { return m_DescriptorType; }
      set { m_DescriptorType = value; }
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxAggregateDescriptor()
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="fieldName">a name of the field to be described</param>
    /// <param name="itemType">a type of the descriptor</param>
    public CxAggregateDescriptor(string fieldName, NxAggregateDescriptorType itemType)
      : this()
    {
      FieldName = fieldName;
      DescriptorType = itemType;
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Methods
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sort of corrects the value returned by a query.
    /// </summary>
    /// <param name="queryResult">actual query result</param>
    /// <returns>transformed query result</returns>
    public string GetValueByQueryResult(string queryResult)
    {
      return queryResult;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    ///                     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </summary>
    /// <returns>
    ///                     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      if (!string.IsNullOrEmpty(FieldName))
        return FieldName;
      return base.ToString();
    }
    //----------------------------------------------------------------------------
    #endregion
  }
}
