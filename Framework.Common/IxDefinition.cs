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

namespace Framework.Utils
{
  /// <summary>
  /// Interface to implement definition classes.
  /// </summary>
  public interface IxDefinition: IxValueProvider
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// List of editable properies.
    /// </summary>
    IList<string> EditableProperties { get; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// List of storable properies.
    /// </summary>
    IList<string> StorableProperties { get; }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns title for definition views (dialog, widget, etc.)
    /// </summary>
    string GetViewTitle();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Starts update of the definition.
    /// </summary>
    void BeginUpdate();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Finishes update of the definition.
    /// </summary>
    void ApplyUpdate();
    //----------------------------------------------------------------------------
    /// <summary>
    /// Cancels update of the definition.
    /// </summary>
    void CancelUpdate();
    //----------------------------------------------------------------------------
  }
}
