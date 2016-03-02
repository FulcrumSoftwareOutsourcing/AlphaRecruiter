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
  public interface IxCustomizationContext
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Current language code.
    /// </summary>
    string CurrentLanguageCd { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Current entity usage context.
    /// </summary>
    CxEntityUsageMetadata CurrentEntityUsage { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// The metadata holder we work in the scope of.
    /// </summary>
    CxMetadataHolder Holder { get; }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Indicates whether the customization object model is in the process of initialization.
    /// </summary>
    bool IsInitializing { get; }
    //-------------------------------------------------------------------------
  }
}
