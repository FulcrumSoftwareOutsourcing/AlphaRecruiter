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

namespace Framework.Metadata
{
  public interface IxCustomizer
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// The unique identifier.
    /// </summary>
    string Id { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customization context.
    /// </summary>
    IxCustomizationContext Context { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Customization manager the customizer belongs to.
    /// </summary>
    CxCustomizationManager Manager { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Applies the customizer state to the metadata object it belongs to.
    /// </summary>
    bool ApplyToMetadata();

    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets the customizer to the metadata object default values.
    /// </summary>
    void ResetToDefault();
    //-------------------------------------------------------------------------
  }
}
