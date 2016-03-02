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
using System.ComponentModel;
using System.Data;
using System.Collections;

namespace Framework.Db
{
  //----------------------------------------------------------------------------
  /// <summary>
  /// Represents a property based upon a data-column, 
  /// to read data-rows properly.
  /// </summary>
  public class CxColumnPropertyDescriptor : PropertyDescriptor
  {
    //----------------------------------------------------------------------------
    private DataColumn m_Column;
    //----------------------------------------------------------------------------
    
    #region Properties
    //----------------------------------------------------------------------------
    /// <summary>
    /// A data-column that holds a description of the property.
    /// </summary>
    public DataColumn Column
    {
      get { return m_Column; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// A type of the object to be examined by this property descriptor.
    /// </summary>
    public override Type ComponentType
    {
      get { return typeof(DataRow); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a value indicating whether the member is browsable, 
    /// as specified in the System.ComponentModel.BrowsableAttribute.
    /// </summary>
    public override bool IsBrowsable
    {
      get { return ((m_Column.ColumnMapping != MappingType.Hidden) && base.IsBrowsable); }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets a value indicating whether this property is read-only.
    /// </summary>
    public override bool IsReadOnly
    {
      get { return m_Column.ReadOnly; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// A type of the property.
    /// </summary>
    public override Type PropertyType
    {
      get { return m_Column.DataType; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the collection of attributes for this member.
    /// </summary>
    public override AttributeCollection Attributes
    {
      get
      {
        if (typeof(IList).IsAssignableFrom(PropertyType))
        {
          Attribute[] array = new Attribute[base.Attributes.Count + 1];
          base.Attributes.CopyTo(array, 0);
          array[array.Length - 1] = new ListBindableAttribute(false);
          return new AttributeCollection(array);
        }
        return base.Attributes;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the name that can be displayed in a window, 
    /// such as a Properties window.
    /// </summary>
    public override string DisplayName
    {
      get { return Column.Caption; }
    }
    //----------------------------------------------------------------------------
    public override string Name
    {
      get
      {
        return Column.ColumnName;
      }
    }
    #endregion

    #region Ctors
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="dataColumn"></param>
    public CxColumnPropertyDescriptor(DataColumn dataColumn)
      : base(dataColumn.ColumnName, null)
    {
      m_Column = dataColumn;
    }
    //----------------------------------------------------------------------------
    #endregion

    #region Methods
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks if the value is not reset yet.
    /// </summary>
    /// <param name="rowObj"></param>
    /// <returns></returns>
    public override bool CanResetValue(object rowObj)
    {
      DataRow row = (DataRow)rowObj;
      object value = row[m_Column.ColumnName];
      return value != DBNull.Value && value != null;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks if the current property descriptor describes the same
    /// data property as the given one.
    /// </summary>
    /// <param name="other">a property descriptor to compare with</param>
    /// <returns>true if equals</returns>
    public override bool Equals(object other)
    {
      if (other is CxColumnPropertyDescriptor)
      {
        CxColumnPropertyDescriptor descriptor = (CxColumnPropertyDescriptor)other;
        return (descriptor.Column == Column);
      }
      return false;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns a hashcode for the property descriptor.
    /// </summary>
    /// <returns>a hash code</returns>
    public override int GetHashCode()
    {
      return Column.GetHashCode();
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Gets the property's value from the given data-row.
    /// </summary>
    /// <param name="rowObj">a row object</param>
    /// <returns>a value of the property</returns>
    public override object GetValue(object rowObj)
    {
      DataRow row = (DataRow)rowObj;
      return row[m_Column.ColumnName];
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets a value in the given data-row object to its reset state.
    /// </summary>
    /// <param name="rowObj">a data-row to be changed</param>
    public override void ResetValue(object rowObj)
    {
      DataRow row = (DataRow)rowObj;
      row[m_Column.ColumnName] = DBNull.Value;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets a value in the given data-row object.
    /// </summary>
    /// <param name="rowObj">a data-row to be changed</param>
    /// <param name="value">a value to be set</param>
    public override void SetValue(object rowObj, object value)
    {
      DataRow row = (DataRow)rowObj;
      row[m_Column.ColumnName] = value;
      OnValueChanged(rowObj, EventArgs.Empty);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Determines a value indicating whether the value of this 
    /// property needs to be persisted.
    /// </summary>
    /// <param name="rowObj">a data-row that should be examined 
    /// for persistence</param>
    /// <returns>true if the property should be persisted; otherwise false</returns>
    public override bool ShouldSerializeValue(object rowObj)
    {
      return false;
    }
    //----------------------------------------------------------------------------
    #endregion
  }

}