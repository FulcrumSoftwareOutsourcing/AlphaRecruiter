using System;
using System.Collections;
using System.ComponentModel;

namespace Framework.Db.LightWeight
{
  public class CxLwColumnDescriptor : PropertyDescriptor
  {
    public CxLwColumn Column { get; set; }

    public CxLwColumnDescriptor(CxLwColumn column)
      : base(column.Name, null)
    {
      Column = column;
    }

    /// <summary>
    /// When overridden in a derived class, gets the type of the component this property is bound to.
    /// </summary>
    /// <value></value>
    /// <returns>
    /// A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.
    /// </returns>
    public override Type ComponentType
    {
      get { return typeof(CxLwRow); }
    }

    /// <summary>
    /// Gets a value indicating whether the member is browsable, as specified in the <see cref="T:System.ComponentModel.BrowsableAttribute"/>.
    /// </summary>
    /// <value></value>
    /// <returns>true if the member is browsable; otherwise, false. If there is no <see cref="T:System.ComponentModel.BrowsableAttribute"/>, the property value is set to the default, which is true.
    /// </returns>
    public override bool IsBrowsable
    {
      get { return base.IsBrowsable; }
    }

    /// <summary>
    /// When overridden in a derived class, gets a value indicating whether this property is read-only.
    /// </summary>
    /// <value></value>
    /// <returns>true if the property is read-only; otherwise, false.
    /// </returns>
    public override bool IsReadOnly
    {
      get { return false; }
    }

    public override Type PropertyType
    {
      get
      {
        if (Column != null && Column.DataType != null)
        {
          return Column.DataType;
        }
        return typeof(string);
      }
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

    public override string DisplayName
    {
      get { return base.DisplayName; }
    }

    public override string Name
    {
      get { return base.Name; }
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Checks if the value is not reset yet.
    /// </summary>
    /// <param name="rowObj"></param>
    /// <returns></returns>
    public override bool CanResetValue(object rowObj)
    {
      CxLwRow row = (CxLwRow) rowObj;
      object value = row[Column.Name];
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
      if (other is CxLwColumnDescriptor)
      {
        var descriptor = (CxLwColumnDescriptor) other;
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
      var row = (CxLwRow) rowObj;
      return row[Column.Name];
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets a value in the given data-row object to its reset state.
    /// </summary>
    /// <param name="rowObj">a data-row to be changed</param>
    public override void ResetValue(object rowObj)
    {
      var row = (CxLwRow) rowObj;
      row[Column.Name] = DBNull.Value;
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Sets a value in the given data-row object.
    /// </summary>
    /// <param name="rowObj">a data-row to be changed</param>
    /// <param name="value">a value to be set</param>
    public override void SetValue(object rowObj, object value)
    {
      var row = (CxLwRow) rowObj;
      row[Column.Name] = value;
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
    public override string ToString()
    {
      return DisplayName ?? base.ToString();
    }
    //----------------------------------------------------------------------------
  }
}
