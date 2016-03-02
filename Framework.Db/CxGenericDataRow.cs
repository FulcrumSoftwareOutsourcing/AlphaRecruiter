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

using System.Data;
using System.ComponentModel;
using Framework.Utils;
using System;
using System.Collections.Generic;

namespace Framework.Db
{
  //-------------------------------------------------------------------------
  /// <summary>
  /// Represents a generalized data-row.
  /// </summary>
  public class CxGenericDataRow : DataRow, IEditableObject, IRevertibleChangeTracking, IxValueProvider
  {
    #region Properties
    //-------------------------------------------------------------------------
    private object m_Tag;
    //-------------------------------------------------------------------------
    public object Tag
    {
      get { return m_Tag; }
      set { m_Tag = value; }
    }
    //-------------------------------------------------------------------------
    
    #endregion

    #region Ctors
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="builder">A datarow builder to be used to build the row</param>
    public CxGenericDataRow(DataRowBuilder builder)
      : base(builder)
    {
    }
    //-------------------------------------------------------------------------
    #endregion

    #region Methods
    //-------------------------------------------------------------------------
    #endregion

    #region IEditableObject implementation
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates the beginning of the row editing.
    /// </summary>
    void IEditableObject.BeginEdit()
    {
      BeginEdit();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Rolls back all the changes done to the row in the current edit session.
    /// </summary>
    void IEditableObject.CancelEdit()
    {
      CancelEdit();
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Commits all the changes done to the row in the current edit session.
    /// </summary>
    void IEditableObject.EndEdit()
    {
      EndEdit();
    }
    //-------------------------------------------------------------------------
    #endregion

    #region IRevertibleChangeTracking implementation
    //-------------------------------------------------------------------------
    ///<summary>
    ///Resets the object�s state to unchanged by rejecting the modifications.
    ///</summary>
    ///
    void IRevertibleChangeTracking.RejectChanges()
    {
      RejectChanges();
    }
    //-------------------------------------------------------------------------
    ///<summary>
    ///Resets the object�s state to unchanged by accepting the modifications.
    ///</summary>
    ///
    void IChangeTracking.AcceptChanges()
    {
      AcceptChanges();
    }
    //-------------------------------------------------------------------------
    public bool IsChanged
    {
      get { return RowState != DataRowState.Unchanged; }
    }

        public IDictionary<string, string> ValueTypes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //-------------------------------------------------------------------------
        #endregion

        #region IxValueProvider implementation
        //-------------------------------------------------------------------------
        object IxValueProvider.this[string name]
    {
      get
      {
        if (Table != null && Table.Columns.Contains(name))
          return this[name];
        else
          return null;
      }
      set
      {
        if (Table != null && Table.Columns.Contains(name))
          this[name] = value;
      }
    }
    //-------------------------------------------------------------------------
    #endregion
  }
}
