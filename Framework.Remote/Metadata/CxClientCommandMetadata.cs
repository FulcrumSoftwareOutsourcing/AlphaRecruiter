/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

using Framework.Metadata;

namespace Framework.Remote
{
  [DataContract]
  public sealed class CxClientCommandMetadata
  {
    [DataMember]
    public readonly string Id;

    [DataMember]
    public readonly string Text;

    [DataMember]
    public readonly bool IsEntityInstanceRequired;

    [DataMember]
    public readonly string SlHandlerClassId;

    [DataMember]
    public readonly string ImageId;

    [DataMember]
    public readonly bool IsMultiple;

    [DataMember]
    public readonly bool IsHandlerBatch;

    [DataMember]
    public readonly bool IsEnabled;

    [DataMember]
    public readonly string ConfirmationText;

    [DataMember]
    public readonly string CommandType;

    [DataMember]
    public bool Visible { get; protected set; }

    [DataMember]
    public readonly bool HasServerHandler;

    [DataMember]
    public readonly string EntityUsageId;

    [DataMember]
    public readonly string PostCreateCommandId;

    [DataMember]
    public bool IsDbCommand { get; private set;}

    [DataMember]
    public bool RefreshPage { get; private set;}

    [DataMember]
    public readonly string TargetCommandId;

    [DataMember]
    public readonly string DisableConditionErrorText = string.Empty;

    [DataMember]
    public readonly string DynamicEntityUsageAttrId = string.Empty;

    [DataMember]
    public readonly string DynamicCommandAttrId = string.Empty;

    [DataMember]
    public string ReportCodeParameter;
    //----------------------------------------------------------------------------
    public CxClientCommandMetadata()
    {
    }

    //----------------------------------------------------------------------------
    internal CxClientCommandMetadata(
      CxCommandMetadata commandMetadata,
      CxEntityUsageMetadata entityUsage)
    {
      if (commandMetadata == null)
        throw new ArgumentNullException();

      Id = commandMetadata.Id;
      Text = commandMetadata.Text;
      IsEntityInstanceRequired = commandMetadata.IsEntityInstanceRequired;
      ImageId = commandMetadata.ImageId;
      SlHandlerClassId = commandMetadata["sl_handler_class_id"];
      IsMultiple = commandMetadata.IsMultiple;
      string handlerBatchStr = commandMetadata["sl_handler_batch"];
      IsHandlerBatch = string.IsNullOrEmpty(handlerBatchStr) ? false : Convert.ToBoolean(handlerBatchStr);
      IsEnabled = commandMetadata.GetIsEnabled(entityUsage);
      ConfirmationText = commandMetadata.ConfirmationText;
      CommandType = Enum.GetName(typeof(NxCommandType), commandMetadata.CommandType);
      Visible = commandMetadata.Visible;
      PostCreateCommandId = commandMetadata["sl_post_create_command_id"];
      if(!string.IsNullOrEmpty(commandMetadata.SqlCommandText))
      {
        IsDbCommand = true;
      }
      RefreshPage = commandMetadata.IsPageToBeRefreshed;
      // Should hide the command if it should be hidden when disabled.
      if (commandMetadata.IsHiddenWhenDisabled && IsEnabled == false)
        Visible = false;

      if (!string.IsNullOrEmpty(commandMetadata.SqlCommandText) ||
         !string.IsNullOrEmpty(commandMetadata.WindowsHandlerClassId) ||
          !string.IsNullOrEmpty(commandMetadata.StaticMethodName))
      {
        HasServerHandler = true;
      }
      EntityUsageId = commandMetadata.EntityUsageId.ToUpper();

      TargetCommandId = commandMetadata.TargetCommandId;

      if(commandMetadata.DisableConditions.Count > 0)
      {
        foreach (CxErrorConditionMetadata condition in commandMetadata.DisableConditions)
        {
          DisableConditionErrorText = condition.ErrorText;
        }
      }

      DynamicEntityUsageAttrId = commandMetadata.DynamicEntityUsageAttrId;
      DynamicCommandAttrId = commandMetadata.DynamicCommandAttrId;
      ReportCodeParameter = commandMetadata["report_code"];
    }

    //----------------------------------------------------------------------------

  }
}
