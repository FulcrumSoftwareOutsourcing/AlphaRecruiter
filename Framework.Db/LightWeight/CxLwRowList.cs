using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;

namespace Framework.Db.LightWeight
{
  public class CxLwRowList : List<CxLwRow>, ITypedList, IBindingList
  {
    public List<CxLwColumn> Columns { get; set; }

    public CxLwRowList()
    {
      Columns = new List<CxLwColumn>();
    }

    //void IList.Remove(object value)
    //{
    //  var row = (CxLwRow) value;
    //  int oldIndex = this.IndexOf(row);
    //  this.Remove(row);
    //  DoOnListChanged(ListChangedType.ItemDeleted, -1, oldIndex);
    //}

    //void IList.RemoveAt(int index)
    //{
    //  Remove(this[index]);
    //}

    #region ITypedList implementation
    //----------------------------------------------------------------------------
    string ITypedList.GetListName(PropertyDescriptor[] descriptors)
    {
      return "DataRow list";
    }
    //----------------------------------------------------------------------------
    PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] descriptors)
    {
      PropertyDescriptorCollection collection = new PropertyDescriptorCollection(new PropertyDescriptor[0]);
      foreach (var column in Columns)
      {
        PropertyDescriptor descriptor = new CxLwColumnDescriptor(column);
        collection.Add(descriptor);
      }
      return collection;
    }
    //----------------------------------------------------------------------------
    #endregion

    #region IBindingList implementation
    //----------------------------------------------------------------------------
    public event ListChangedEventHandler ListChanged;
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    /// <summary>
    /// Should be called when some outer subscribers should know about changes
    /// done to the data-source.
    /// </summary>
    /// <param name="listChangedType">a type of change(s) that happened</param>
    /// <param name="newIndex">(optional) an index of 
    /// newly added, inserted or moved element</param>
    public void DoOnListChanged(
      ListChangedType listChangedType, int newIndex)
    {
      DoOnListChanged(listChangedType, newIndex, newIndex);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Should be called when some outer subscribers should know about changes
    /// done to the data-source.
    /// </summary>
    /// <param name="listChangedType">a type of change(s) that happened</param>
    /// <param name="newIndex">(optional) an index of 
    /// newly added, inserted or moved element</param>
    /// <param name="oldIndex">(optional) an index of element that has been
    /// moved before the move</param>
    public void DoOnListChanged(
      ListChangedType listChangedType, int newIndex, int oldIndex)
    {
      if (ListChanged != null)
      {
        ListChangedEventArgs args;
        if (oldIndex > -1)
          args = new ListChangedEventArgs(listChangedType, newIndex, oldIndex);
        else
          args = new ListChangedEventArgs(listChangedType, newIndex);
        ListChanged(this, args);
      }
    }

    #region Properties
    //----------------------------------------------------------------------------
    bool IBindingList.AllowEdit
    {
      get { return true; }
    }
    //----------------------------------------------------------------------------
    bool IBindingList.AllowNew
    {
      get { return true; }
    }
    //----------------------------------------------------------------------------
    bool IBindingList.AllowRemove
    {
      get { return true; }
    }
    //----------------------------------------------------------------------------
    bool IBindingList.SupportsSorting
    {
      get { return true; }
    }
    //----------------------------------------------------------------------------
    bool IBindingList.SupportsSearching
    {
      get { return true; }
    }
    //----------------------------------------------------------------------------
    bool IBindingList.SupportsChangeNotification
    {
      get { return true; }
    }
    //----------------------------------------------------------------------------
    PropertyDescriptor IBindingList.SortProperty
    {
      get
      {
        return null;
      }
    }
    //----------------------------------------------------------------------------
    ListSortDirection IBindingList.SortDirection
    {
      get
      {
          return ListSortDirection.Ascending;
      }
    }
    //----------------------------------------------------------------------------
    bool IBindingList.IsSorted
    {
      get { return false; }
    }
    #endregion

    #region Methods
    //----------------------------------------------------------------------------
    object IBindingList.AddNew()
    {
      var row = new CxLwRow()
      {
        Status = NxLwRowStatus.New,
        RowList = this
      };
      Add(row);
      return row;
    }
    //----------------------------------------------------------------------------
    void IBindingList.AddIndex(PropertyDescriptor descriptor)
    {
      // The datasource isn't able to manipulate with search indices.
    }
    //----------------------------------------------------------------------------
    void IBindingList.RemoveIndex(PropertyDescriptor descriptor)
    {
      // The datasource isn't able to manipulate with search indices.
    }
    //----------------------------------------------------------------------------
    int IBindingList.Find(PropertyDescriptor descriptor, object obj)
    {
      // As we don't have all the data at once, 
      // we cannot find the right row without subloading.

      // Performing full enumeration.

      var columnDescriptor = descriptor as CxLwColumnDescriptor;
      if (columnDescriptor != null)
      {
        for (int i = 0; i < Count; i++)
        {
          object rowValue = this[i][columnDescriptor.Column.Name];
          if (rowValue == obj)
            return i;
        }
      }
      return -1;
    }
    //----------------------------------------------------------------------------
    void IBindingList.ApplySort(PropertyDescriptor descriptor, ListSortDirection direction)
    {
    }
    //----------------------------------------------------------------------------
    void IBindingList.RemoveSort()
    {
    }
    //----------------------------------------------------------------------------
    #endregion

    #endregion
  }
}
