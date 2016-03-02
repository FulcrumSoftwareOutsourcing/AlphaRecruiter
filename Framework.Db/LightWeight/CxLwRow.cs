using System.ComponentModel;
using Framework.Utils;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Framework.Db.LightWeight
{
  public class CxLwRow : IxValueProvider, IEditableObject, IDictionary
  {
    protected Hashtable InnerStorage { get; set; }
    internal protected CxLwRowList RowList { get; set; }

    public CxLwRow()
    {
      InnerStorage = new Hashtable(StringComparer.OrdinalIgnoreCase);
    }

    #region IEnumerable implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      return InnerStorage.GetEnumerator();
    }

    #endregion

    #region ICollection implementation
    //----------------------------------------------------------------------------
    void ICollection.CopyTo(Array array, int index)
    {
      InnerStorage.CopyTo(array, index);
    }
    //----------------------------------------------------------------------------
    bool ICollection.IsSynchronized
    {
      get { return ((ICollection) InnerStorage).IsSynchronized; }
    }
    //----------------------------------------------------------------------------
    object ICollection.SyncRoot
    {
      get { return ((ICollection) InnerStorage).SyncRoot; }
    }
    //----------------------------------------------------------------------------
    int ICollection.Count
    {
      get { return InnerStorage.Count; }
    }
    #endregion

    #region IDictionary implementation
    bool IDictionary.IsFixedSize { get { return false; } }
    bool IDictionary.IsReadOnly { get { return false; } }
    ICollection IDictionary.Keys { get { return InnerStorage.Keys; } }
    ICollection IDictionary.Values { get { return InnerStorage.Values; } }
    void IDictionary.Clear()
    {
      InnerStorage.Clear();
    }
    void IDictionary.Add(object key, object value)
    {
      SetEntry(Convert.ToString(key), value);
    }
    void IDictionary.Remove(object key)
    {
      InnerStorage.Remove(key);
    }
    bool IDictionary.Contains(object key)
    {
      return InnerStorage.ContainsKey(key);
    }
    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return InnerStorage.GetEnumerator();
    }
    object IDictionary.this[object key]
    {
      get { return this[Convert.ToString(key)]; }
      set { this[Convert.ToString(key)] = value; }
    }
    #endregion

    #region IxValueProvider implementation
    public object this[string key]
    {
      get { return GetEntry(key); }
      set { SetEntry(key, value); }
    }
    #endregion

    public object GetEntry(string key)
    {
      if (string.IsNullOrEmpty(key))
        return null;
      return InnerStorage[key];
    }

    public void SetEntry(string key, object value)
    {
      if (!string.IsNullOrEmpty(key))
        InnerStorage[key] = value;
    }

    public void CopyFrom(CxLwRow other)
    {
      Status = other.Status;
      InnerStorage = (Hashtable) other.InnerStorage.Clone();
    }

    /// <summary>
    /// Clones this entity instance.
    /// </summary>
    /// <returns></returns>
    public CxLwRow Clone()
    {
      var clone = new CxLwRow();
      clone.CopyFrom(this);
      return clone;
    }


    #region IEditableObject implementation

    private CxLwRow InitialRow { get; set; }
    public NxLwRowStatus Status { get; set; }
    public bool IsEditing { get; set; }

        public IDictionary<string, string> ValueTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        void IEditableObject.BeginEdit()
    {
      if (Status == NxLwRowStatus.NonChanged)
      {
        InitialRow = this.Clone();
        Status = NxLwRowStatus.Updated;
      }
      IsEditing = true;
    }

    void IEditableObject.EndEdit()
    {
      if (Status != NxLwRowStatus.NonChanged)
      {
        InitialRow = null;
      }
      IsEditing = false;
    }

    void IEditableObject.CancelEdit()
    {
      if (Status != NxLwRowStatus.NonChanged)
      {
        if (Status == NxLwRowStatus.New && RowList != null && RowList.Contains(this))
          RowList.Remove(this);
  
        if (InitialRow != null)
          this.CopyFrom(InitialRow);
        InitialRow = null;
        Status = NxLwRowStatus.NonChanged;
      }
      IsEditing = false;
    }

    #endregion

  }
}
