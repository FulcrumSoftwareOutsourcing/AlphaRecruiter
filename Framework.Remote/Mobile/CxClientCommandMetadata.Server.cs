using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Metadata;

namespace Framework.Remote.Mobile
{
  public partial class CxClientCommandMetadata
  {
    public CxClientCommandMetadata()
    {
    }

    //----------------------------------------------------------------------------
    public CxClientCommandMetadata(
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
            HiddenWhenDisabled = commandMetadata.IsHiddenWhenDisabled;
      if (!string.IsNullOrEmpty(commandMetadata.SqlCommandText))
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

      if (commandMetadata.DisableConditions.Count > 0)
      {
        foreach (CxErrorConditionMetadata condition in commandMetadata.DisableConditions)
        {
          DisableConditionErrorText = condition.ErrorText;
        }
      }

      DynamicEntityUsageAttrId = commandMetadata.DynamicEntityUsageAttrId;
      DynamicCommandAttrId = commandMetadata.DynamicCommandAttrId;
      ReportCodeParameter = commandMetadata["report_code"];
            AvailableOnEditform = commandMetadata.AvailableOnEditform;
    }
  }
}
