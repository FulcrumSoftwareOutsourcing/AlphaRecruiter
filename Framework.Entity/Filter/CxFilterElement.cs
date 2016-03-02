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
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Class implementing IxFilterElement interface
  /// </summary>
  public class CxFilterElement : IxFilterElement
  {
    //-------------------------------------------------------------------------
    protected string m_Name = null;
    protected NxFilterOperation m_Operation = NxFilterOperation.None;
    protected ArrayList m_Values = new ArrayList();
    protected ArrayList m_ActualValues = new ArrayList();
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">element name</param>
    public CxFilterElement(string name)
    {
      m_Name = name;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="filterElement">filter element to copy properties from</param>
    public CxFilterElement(IxFilterElement filterElement)
    {
      m_Name = filterElement.Name;
      m_Operation = filterElement.Operation;
      foreach (object value in filterElement.Values)
      {
        m_Values.Add(value);
      }
      UpdateActualValues();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="filterElement">filter element to copy properties from</param>
    /// <param name="operation">overridden operation</param>
    /// <param name="firstValue">overridden first value</param>
    public CxFilterElement(IxFilterElement filterElement, NxFilterOperation operation, object firstValue)
    {
      m_Name = filterElement.Name;
      m_Operation = operation;
      m_Values.Add(firstValue);
      UpdateActualValues();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="filterElement">filter element to copy properties from</param>
    /// <param name="operation">overridden operation</param>
    /// <param name="firstValue">overridden first value</param>
    /// <param name="secondValue">overridden second value</param>
    public CxFilterElement(
      IxFilterElement filterElement, 
      NxFilterOperation operation, 
      object firstValue,
      object secondValue)
    {
      m_Name = filterElement.Name;
      m_Operation = operation;
      m_Values.Add(firstValue);
      m_Values.Add(secondValue);
      UpdateActualValues();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Updates actual values list.
    /// </summary>
    protected void UpdateActualValues()
    {
      m_ActualValues.Clear();
      for (int i = 0; i < m_Values.Count && i < GetFilterOperationValueCount(m_Operation); i++)
      {
        m_ActualValues.Add(m_Values[i]);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns count of filter values for the given filter operation.
    /// </summary>
    static public int GetFilterOperationValueCount(NxFilterOperation operation)
    {
      switch (operation)
      {
        case NxFilterOperation.Equal:
        case NxFilterOperation.NotEqual:
        case NxFilterOperation.Less:
        case NxFilterOperation.Greater:
        case NxFilterOperation.LessEqual:
        case NxFilterOperation.GreaterEqual:
        case NxFilterOperation.Like:
        case NxFilterOperation.NotLike:
        case NxFilterOperation.StartsWith:
        case NxFilterOperation.NotExists:
          return 1;
        case NxFilterOperation.Between:
          return 2;
        case NxFilterOperation.IsNull:
        case NxFilterOperation.IsNotNull:
        case NxFilterOperation.Today:
        case NxFilterOperation.Yesterday:
        case NxFilterOperation.ThisWeek:
        case NxFilterOperation.PrevWeek:
        case NxFilterOperation.ThisMonth:
        case NxFilterOperation.PrevMonth:
        case NxFilterOperation.ThisYear:
        case NxFilterOperation.PrevYear:
        case NxFilterOperation.InThePast:
        case NxFilterOperation.TodayOrLater:
        case NxFilterOperation.Tomorrow:
        case NxFilterOperation.Myself:
          return 0;
      }
      return 0;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns display text for each filter operation.
    /// </summary>
    /// <param name="holder">metadata holder to get localized texts</param>
    /// <param name="operation">operation to get text for</param>
    /// <returns>text to display</returns>
    static public string GetFilterOperationText(
      CxMetadataHolder holder,
      NxFilterOperation operation)
    {
      switch (operation)
      {
        case NxFilterOperation.None:
          return "";
        case NxFilterOperation.Equal:
          return "=";
        case NxFilterOperation.NotEqual:
          return "<>";
        case NxFilterOperation.Less:
          return "<";
        case NxFilterOperation.Greater:
          return ">";
        case NxFilterOperation.LessEqual:
          return "<=";
        case NxFilterOperation.GreaterEqual:
          return ">=";
        case NxFilterOperation.Between:
          return holder.GetTxt("Between");
        case NxFilterOperation.Like:
          return holder.GetTxt("Like");
        case NxFilterOperation.NotLike:
          return holder.GetTxt("Not Like");
        case NxFilterOperation.StartsWith:
          return holder.GetTxt("Starts With");
        case NxFilterOperation.IsNull:
          return holder.GetTxt("Is Null");
        case NxFilterOperation.IsNotNull:
          return holder.GetTxt("Is Not Null");
        case NxFilterOperation.Today:
          return holder.GetTxt("Today");
        case NxFilterOperation.Yesterday:
          return holder.GetTxt("Yesterday");
        case NxFilterOperation.ThisWeek:
          return holder.GetTxt("This Week");
        case NxFilterOperation.PrevWeek:
          return holder.GetTxt("Prev Week");
        case NxFilterOperation.ThisMonth:
          return holder.GetTxt("This Month");
        case NxFilterOperation.PrevMonth:
          return holder.GetTxt("Prev Month");
        case NxFilterOperation.ThisYear:
          return holder.GetTxt("This Year");
        case NxFilterOperation.PrevYear:
          return holder.GetTxt("Prev Year");
        case NxFilterOperation.InThePast:
          return holder.GetTxt("In the Past");
        case NxFilterOperation.TodayOrLater:
          return holder.GetTxt("Today or Later");
        case NxFilterOperation.Tomorrow:
          return holder.GetTxt("Tomorrow");
        case NxFilterOperation.NotExists:
          return holder.GetTxt("Not Exists");
        case NxFilterOperation.Myself:
          return holder.GetTxt("Myself");
      }
      return "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns array of available filter operations for the given attribute.
    /// </summary>
    /// <param name="attribute">attribute metadata</param>
    static public NxFilterOperation[] GetAttributeFilterOperations(CxAttributeMetadata attribute)
    {
      NxFilterOperation[] operations = null;
      if (attribute.RowSource != null)
      {
        if (!attribute.IsMultiValueLookup)
        {
          operations = new NxFilterOperation[] 
            { NxFilterOperation.Equal, 
              NxFilterOperation.NotEqual,
              NxFilterOperation.IsNull,
              NxFilterOperation.IsNotNull };
        }
        else
        {
          if (!attribute.IsFilteredByCustomObject)
          {
            operations = new NxFilterOperation[] 
              { NxFilterOperation.Equal, 
                NxFilterOperation.NotEqual };
          }
          else
          {
            operations = new NxFilterOperation[] 
              { NxFilterOperation.Equal, 
                NxFilterOperation.NotEqual,
                NxFilterOperation.NotExists };
          }
        }
        if (attribute.FilterMyself)
        {
          List<NxFilterOperation> list = new List<NxFilterOperation>(operations);
          list.Add(NxFilterOperation.Myself);
          operations = list.ToArray();
        }
      }
      else
      {
        switch (attribute.Type)
        {
          case CxAttributeMetadata.TYPE_LONGSTRING:
            operations = new NxFilterOperation[]
              { 
                NxFilterOperation.StartsWith,
                NxFilterOperation.Like,
                NxFilterOperation.NotLike,
                NxFilterOperation.IsNull,
                NxFilterOperation.IsNotNull
              };
            break;

          case CxAttributeMetadata.TYPE_STRING:
          case CxAttributeMetadata.TYPE_LINK:
            operations = new NxFilterOperation[]
            { NxFilterOperation.StartsWith,
              NxFilterOperation.Like,
              NxFilterOperation.Equal,
              NxFilterOperation.Between,
              NxFilterOperation.NotLike,
              NxFilterOperation.NotEqual,
              NxFilterOperation.IsNull,
              NxFilterOperation.IsNotNull };
            break;

          case CxAttributeMetadata.TYPE_DATETIME:
          case CxAttributeMetadata.TYPE_DATE:
            operations = new NxFilterOperation[]
            { NxFilterOperation.Between,
              NxFilterOperation.Today,
              NxFilterOperation.ThisWeek,
              NxFilterOperation.ThisMonth,
              NxFilterOperation.ThisYear,
              NxFilterOperation.Yesterday,
              NxFilterOperation.PrevWeek,
              NxFilterOperation.PrevMonth,
              NxFilterOperation.PrevYear,
              NxFilterOperation.Equal,
              NxFilterOperation.LessEqual,
              NxFilterOperation.GreaterEqual,
              NxFilterOperation.Less,
              NxFilterOperation.Greater,
              NxFilterOperation.NotEqual,
              NxFilterOperation.InThePast,
              NxFilterOperation.TodayOrLater,
              NxFilterOperation.Tomorrow,
              NxFilterOperation.IsNull,
              NxFilterOperation.IsNotNull };
            break;

          case CxAttributeMetadata.TYPE_TIME:
            operations = new NxFilterOperation[]
            { NxFilterOperation.Between,
              NxFilterOperation.Equal,
              NxFilterOperation.LessEqual,
              NxFilterOperation.GreaterEqual,
              NxFilterOperation.Less,
              NxFilterOperation.Greater,
              NxFilterOperation.NotEqual,
              NxFilterOperation.IsNull,
              NxFilterOperation.IsNotNull };
            break;

          case CxAttributeMetadata.TYPE_INT:
          case CxAttributeMetadata.TYPE_FLOAT:
            operations = new NxFilterOperation[]
            { NxFilterOperation.Equal,
              NxFilterOperation.LessEqual,
              NxFilterOperation.GreaterEqual,
              NxFilterOperation.Less,
              NxFilterOperation.Greater,
              NxFilterOperation.Between,
              NxFilterOperation.NotEqual };
            break;

          case CxAttributeMetadata.TYPE_BOOLEAN:
            operations = new NxFilterOperation[] 
            { 
              NxFilterOperation.Equal,
              NxFilterOperation.IsNull,
              NxFilterOperation.IsNotNull 
            };
            break;

          case CxAttributeMetadata.TYPE_FILE:
          case CxAttributeMetadata.TYPE_IMAGE:
            operations = new NxFilterOperation[] 
            { NxFilterOperation.None,
              NxFilterOperation.IsNull, 
              NxFilterOperation.IsNotNull };
            break;

          case CxAttributeMetadata.TYPE_ICON: break;
        }
      }

      if (attribute.WinControl == CxWinControlNames.WIN_CONTROL_COLOR ||
          attribute.WinControl == CxWinControlNames.WIN_CONTROL_COLOR_NOTEXT)
      {
        operations = new NxFilterOperation[] 
          { NxFilterOperation.Equal, 
            NxFilterOperation.NotEqual,
            NxFilterOperation.IsNull, 
            NxFilterOperation.IsNotNull };
      }

      if (operations != null)
      {
        if (CxUtils.NotEmpty(attribute.EnabledFilterOperations))
        {
          ArrayList list = new ArrayList();
          Hashtable enabledMap = new Hashtable();
          CxList.AppendDictionaryFromList(
            enabledMap,
            CxText.DecomposeWithSeparator(attribute.EnabledFilterOperations.ToUpper(), ","));
          foreach (NxFilterOperation operation in operations)
          {
            if (enabledMap.ContainsKey(operation.ToString().ToUpper()))
            {
              list.Add(operation);
            }
          }
          operations = new NxFilterOperation[list.Count];
          list.CopyTo(operations);
        }

        NxFilterOperation defaultOperation = 
          CxEnum.Parse(attribute.FilterOperation, NxFilterOperation.None);
        if (defaultOperation != NxFilterOperation.None)
        {
          int index = Array.IndexOf(operations, defaultOperation);
          if (index > 0)
          {
            ArrayList list = new ArrayList(operations);
            list.RemoveAt(index);
            list.Insert(0, defaultOperation);
            list.CopyTo(operations);
          }
        }
      }

      return operations;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns name of the filter element.
    /// </summary>
    public string Name
    {
      get { return m_Name; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets current filter operation.
    /// </summary>
    public NxFilterOperation Operation
    {
      get 
      { 
        return m_Operation; 
      }
      set 
      {
        if (m_Operation != value)
        {
          m_Operation = value;
          UpdateActualValues();
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns list of values.
    /// </summary>
    public IList Values
    { get { return m_ActualValues; } }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets filter value at the specified index.
    /// </summary>
    /// <param name="index">value index</param>
    /// <param name="value">value to set</param>
    public void SetValue(int index, object value)
    {
      while (index > m_Values.Count - 1)
      {
        m_Values.Add(null);
      }
      m_Values[index] = value;
      UpdateActualValues();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if this element is empty.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        if (m_Operation != NxFilterOperation.None && 
            Values.Count == GetFilterOperationValueCount(Operation))
        {
          bool allValuesEmpty = true;
          foreach (object value in Values)
          {
            if (CxUtils.NotEmpty(value))
            {
              allValuesEmpty = false;
              break;
            }
          }
          return allValuesEmpty && Values.Count > 0;
        }
        return true;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns value count for the current filter operation.
    /// </summary>
    public int OperationValueCount
    { get { return GetFilterOperationValueCount(Operation); } }
    //-------------------------------------------------------------------------
  }
}