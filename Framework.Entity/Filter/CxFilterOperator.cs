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
using System.Globalization;
using System.Text;
using Framework.Db;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity.Filter
{
  /// <summary>
  /// Abstract base class for all filter operators.
  /// </summary>
  abstract public class CxFilterOperator
  {
    //-------------------------------------------------------------------------
    private CxEntityUsageMetadata m_EntityUsage;
    private CxAttributeMetadata m_Attribute;
    private IxFilterElement m_FilterElement;
    private NxFilterOperation m_Operation;
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Creates and returns filter operator.
    /// </summary>
    /// <param name="entityUsage">entity usage metadata</param>
    /// <param name="filterElement">filter element</param>
    /// <returns>created filter operator instance</returns>
    static public CxFilterOperator Create(
      CxEntityUsageMetadata entityUsage,
      IxFilterElement filterElement)
    {
      if (entityUsage != null && filterElement != null)
      {
        CxAttributeMetadata attribute = entityUsage.GetAttribute(filterElement.Name);
        if (attribute != null)
        {
          if (attribute.IsMultiValueLookup)
          {
            switch (filterElement.Operation)
            {
              case NxFilterOperation.Equal:
              case NxFilterOperation.NotEqual:
              case NxFilterOperation.NotExists:
                return new CxMultiLookupFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Myself:
                return new CxMyselfFilterOperator(entityUsage, filterElement);
            }
          }
          else
          {
            switch (filterElement.Operation)
            {
              case NxFilterOperation.Equal:
                return new CxEqualFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.NotEqual:
                return new CxNotEqualFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Less:
                return new CxLessFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Greater:
                return new CxGreaterFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.LessEqual:
                return new CxLessEqualFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.GreaterEqual:
                return new CxGreaterEqualFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Between:
                if (filterElement.Values.Count > 1 &&
                    CxUtils.NotEmpty(filterElement.Values[0]) &&
                    CxUtils.NotEmpty(filterElement.Values[1]))
                {
                  return new CxBetweenFilterOperator(entityUsage, filterElement);
                }
                else if (filterElement.Values.Count > 0 && CxUtils.NotEmpty(filterElement.Values[0]))
                {
                  CxFilterElement element = new CxFilterElement(filterElement, NxFilterOperation.GreaterEqual, filterElement.Values[0]);
                  return new CxGreaterEqualFilterOperator(entityUsage, element);
                }
                else if (filterElement.Values.Count > 1 && CxUtils.NotEmpty(filterElement.Values[1]))
                {
                  CxFilterElement element = new CxFilterElement(filterElement, NxFilterOperation.LessEqual, filterElement.Values[1]);
                  return new CxLessEqualFilterOperator(entityUsage, element);
                }
                break;
              case NxFilterOperation.Like:
                return new CxLikeFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.NotLike:
                return new CxNotLikeFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.StartsWith:
                return new CxStartsWithFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.IsNull:
                return new CxIsNullFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.IsNotNull:
                return new CxIsNotNullFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Today:
                return new CxTodayFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.ThisWeek:
                return new CxThisWeekFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.ThisMonth:
                return new CxThisMonthFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.ThisYear:
                return new CxThisYearFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Yesterday:
                return new CxYesterdayFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.PrevWeek:
                return new CxPrevWeekFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.PrevMonth:
                return new CxPrevMonthFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.PrevYear:
                return new CxPrevYearFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.InThePast:
                return new CxInThePastFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.TodayOrLater:
                return new CxTodayOrLaterFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Tomorrow:
                return new CxTomorrowFilterOperator(entityUsage, filterElement);
              case NxFilterOperation.Myself:
                return new CxMyselfFilterOperator(entityUsage, filterElement);
            }
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entityUsage">entity usage</param>
    /// <param name="filterElement">filter element</param>
    protected CxFilterOperator(CxEntityUsageMetadata entityUsage, IxFilterElement filterElement)
    {
      if (entityUsage == null)
      {
        throw new ExNullArgumentException("entityUsage");
      }
      if (filterElement == null)
      {
        throw new ExNullArgumentException("filterElement");
      }
      m_EntityUsage = entityUsage;
      m_FilterElement = filterElement;
      m_Attribute = m_EntityUsage.GetAttribute(m_FilterElement.Name);
      if (m_Attribute == null)
      {
        throw new ExNullReferenceException("m_Attribute");
      }
      m_Operation = m_FilterElement.Operation;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter WHERE condition text.
    /// </summary>
    /// <returns>filter WHERE condition text</returns>
    abstract protected internal string GetConditionInternal();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes value provider.
    /// </summary>
    /// <param name="valueProvider">value provider to initialize</param>
    abstract protected internal void InitializeValueProviderInternal(IxValueProvider valueProvider);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter parameter name depending on the parameter index.
    /// </summary>
    /// <param name="valueIndex">parameter index</param>
    /// <returns>filter parameter name</returns>
    protected string GetParameterName(int valueIndex)
    {
      return String.Format("f_{0}_{1}", FilterElement.Name, valueIndex);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter parameter value.
    /// </summary>
    /// <param name="valueIndex">value index</param>
    /// <returns>parameter value</returns>
    virtual protected object GetValueInternal(int valueIndex)
    {
      return FilterElement.Values[valueIndex];
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter parameter value.
    /// </summary>
    /// <param name="valueIndex">value index</param>
    /// <returns>parameter value</returns>
    protected object GetValue(int valueIndex)
    {
      object value = GetValueInternal(valueIndex);
      if (OnGetFilterValue != null)
      {
        CxGetFilterValueEventArgs args = new CxGetFilterValueEventArgs(value);
        OnGetFilterValue(this, args);
        value = args.Result;
      }
      return value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text for representation of filter value.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <param name="filterValue">value to represent</param>
    virtual protected string GetValueText(CxDbConnection connection, object filterValue)
    {
      string text;
      if (Attribute.RowSource != null && connection != null)
      {
        text = Attribute.RowSource.GetDescriptionByValue(
                connection, filterValue, Attribute.IsMultiValueLookup);
      }
      else
      {
        switch (Attribute.Type)
        {
          case CxAttributeMetadata.TYPE_DATETIME:
            text = Convert.ToDateTime(filterValue).ToString(
              DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " +
              DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
            break;
          case CxAttributeMetadata.TYPE_DATE:
            switch (Attribute.WebControl)
            {
              case CxAttributeMetadata.WEB_CONTROL_MONTH:
              case CxAttributeMetadata.WEB_CONTROL_FUTURE_MONTH:
                text = Convert.ToDateTime(filterValue).ToString(
                  "MM" + DateTimeFormatInfo.CurrentInfo.DateSeparator + "yyyy");
                break;
              default:
                text = Convert.ToDateTime(filterValue).ToString(
                  DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
                break;
            }
            break;
          case CxAttributeMetadata.TYPE_TIME:
            text = Convert.ToDateTime(filterValue).ToString(
              DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
            break;
          case CxAttributeMetadata.TYPE_BOOLEAN:
            text = CxBool.Parse(filterValue) ?
              EntityUsage.Holder.GetTxt("Yes") : EntityUsage.Holder.GetTxt("No");
            break;
          case CxAttributeMetadata.TYPE_INT:
          case CxAttributeMetadata.TYPE_FLOAT:
          case CxAttributeMetadata.TYPE_STRING:
          case CxAttributeMetadata.TYPE_LONGSTRING:
          default:
            text = CxUtils.ToString(filterValue);
            break;
        }
      }
      return CxUtils.NotEmpty(text) ? "'" + text + "'" : "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter WHERE condition text.
    /// </summary>
    /// <returns>filter WHERE condition text</returns>
    public string GetCondition()
    {
      string condition = GetConditionInternal();
      if (!String.IsNullOrEmpty(condition))
      {
        if (!String.IsNullOrEmpty(Attribute.FilterCondition))
        {
          condition = Attribute.FilterCondition.Replace("%filter%", condition);
        }
        else if (Attribute.IsFilteredByCustomObject)
        {
          condition = String.Format(
            "EXISTS (SELECT 1 FROM {0} WHERE ({1}) AND ({2}))",
            Attribute.FilterSearchObject,
            Attribute.FilterSearchObjectJoin,
            condition);
          if (Operation == NxFilterOperation.NotExists)
          {
            condition = String.Format("NOT {0}", condition);
          }
        }
      }
      return condition;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes value provider.
    /// </summary>
    /// <param name="valueProvider">value provider to initialize</param>
    public void InitializeValueProvider(IxValueProvider valueProvider)
    {
      InitializeValueProviderInternal(valueProvider);
      valueProvider[String.Format("f_{0}_Operation", FilterElement.Name)] = Operation.ToString();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text for the filter condition.
    /// </summary>
    /// <param name="connection">database connection</param>
    /// <returns>display text</returns>
    abstract public string GetDisplayText(CxDbConnection connection);
    //-------------------------------------------------------------------------
    /// <summary>
    /// Entity usage metadata.
    /// </summary>
    public CxEntityUsageMetadata EntityUsage
    {
      get { return m_EntityUsage; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Attribute metadata.
    /// </summary>
    public CxAttributeMetadata Attribute
    {
      get { return m_Attribute; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Filter element.
    /// </summary>
    public IxFilterElement FilterElement
    {
      get { return m_FilterElement; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Filter operation.
    /// </summary>
    public NxFilterOperation Operation
    {
      get { return m_Operation; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field name to put into filter condition.
    /// </summary>
    public string FieldName
    {
      get
      {
        if (!String.IsNullOrEmpty(Attribute.FilterSearchField))
        {
          return Attribute.FilterSearchField;
        }
        else
        {
          return Attribute.Id;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns field display text.
    /// </summary>
    public string FieldText
    {
      get
      {
        return Attribute.GetCaption(EntityUsage, NxAttributeContext.Filter);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns filter operation display text.
    /// </summary>
    public string OperationText
    {
      get
      {
        return CxFilterElement.GetFilterOperationText(EntityUsage.Holder, Operation);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Get filter value event.
    /// </summary>
    public event DxOnGetFilterValue OnGetFilterValue;
    //-------------------------------------------------------------------------
  }

  //---------------------------------------------------------------------------
  public delegate void DxOnGetFilterValue(object sender, CxGetFilterValueEventArgs e);
  //---------------------------------------------------------------------------

  //---------------------------------------------------------------------------
  public class CxGetFilterValueEventArgs : EventArgs
  {
    private object m_Value;
    private object m_Result;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value"></param>
    public CxGetFilterValueEventArgs(object value)
    {
      m_Value = value;
      m_Result = value;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Filter value
    /// </summary>
    public object Value
    {
      get { return m_Value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resulting filter value
    /// </summary>
    public object Result
    {
      get { return m_Result; }
      set { m_Result = value; }
    }
    //-------------------------------------------------------------------------
  }
  //---------------------------------------------------------------------------
}