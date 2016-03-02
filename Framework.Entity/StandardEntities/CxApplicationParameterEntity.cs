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
using System.Text;
using Framework.Metadata;
using Framework.Utils;

namespace Framework.Entity
{
  /// <summary>
  /// Application parameter entity. 
  /// Validates different application parameter values.
  /// </summary>
  public class CxApplicationParameterEntity : CxBaseEntity
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Parameter code constants
    /// </summary>
    public const string AUTO_UPDATE_SCHEDULED_MINUTES = "AUTO_UPDATE_SCHEDULED_MINUTES";
    public const string AUTO_UPDATE_DB_VERSION_CHECK = "AUTO_UPDATE_DB_VERSION_CHECK";
    public const string KEYBOARD_NAVIGATION_DELAY = "KEYBOARD_NAVIGATION_DELAY";
    //-------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="metadata">metadata that describes this entity</param>
    public CxApplicationParameterEntity(CxEntityUsageMetadata metadata) : base(metadata)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates entity.
    /// </summary>
    override public void Validate()
    {
      base.Validate();

      switch (CxUtils.ToString(this["Code"]))
      {
        case AUTO_UPDATE_SCHEDULED_MINUTES:
        case KEYBOARD_NAVIGATION_DELAY:
          if (CxInt.Parse(this["Value"], -1) < 0)
          {
            throw new ExValidationException(
              Metadata.Holder.GetErr("Value should be a positive integer or zero."), "Value");
          }
          break;
        case AUTO_UPDATE_DB_VERSION_CHECK:
          if (CxInt.Parse(this["Value"], -1) < 0 &&
              CxBool.ParseEx(CxUtils.ToString(this["Value"]), null) == null)
          {
            throw new ExValidationException(
              Metadata.Holder.GetErr("Value should be a positive integer or zero, or 'Yes' or 'No'."), "Value");
          }
          break;
      }
    }
    //----------------------------------------------------------------------------
  }
}