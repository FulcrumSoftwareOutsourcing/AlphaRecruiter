using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Metadata
{
  public class CxAttributeTypePresenter
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns the presentation string for the given attribute metadata.
    /// Can be overridden in an descendant class.
    /// </summary>
    public virtual string GetFieldType(CxAttributeMetadata attributeMetadata)
    {
      string type = attributeMetadata.Type;
      string winControl = attributeMetadata.WinControl;
      string length = Convert.ToString(attributeMetadata.MaxLength);
      bool rowSourceDefined = !string.IsNullOrEmpty(attributeMetadata.RowSourceId);

      string result = null;

      if (type == CxAttributeMetadata.TYPE_LINK ||
          winControl == CxWinControlNames.WIN_CONTROL_HYPERLINK ||
          winControl == CxWinControlNames.WIN_CONTROL_HYPERLINK_EDIT ||
          winControl == CxWinControlNames.WIN_CONTROL_HYPERLINK_EDIT_FILE ||
          winControl == CxWinControlNames.WIN_CONTROL_HYPERLINK_EDIT_URL ||
          winControl == CxWinControlNames.WIN_CONTROL_LOOKUP_SELECT)
      {
        result = "Hyperlink";
      }

      if (winControl == CxWinControlNames.WIN_CONTROL_DROPDOWN ||
          winControl == CxWinControlNames.WIN_CONTROL_DROPDOWNIMAGE ||
          winControl == CxWinControlNames.WIN_CONTROL_DROPDOWNIMAGEONLY ||
          winControl == CxWinControlNames.WIN_CONTROL_LOOKUP)
      {
        result = "Drop-down";
      }

      if (winControl == CxWinControlNames.WIN_CONTROL_LOOKUP_MULTI)
      {
        result = "Drop-down (Multi)";
      }

      if (winControl == CxWinControlNames.WIN_CONTROL_COMBOEDIT)
      {
        result = "Drop-down (Unbound)";
      }

      if (winControl == CxWinControlNames.WIN_CONTROL_EMAIL ||
          winControl == CxWinControlNames.WIN_CONTROL_EMAILADDRESSBUTTONEDIT)
      {
        result = "Email";
      }

      if (result == null &&
         (type == CxAttributeMetadata.TYPE_STRING ||
          type == CxAttributeMetadata.TYPE_LONGSTRING))
      {
        if (length != "0")
          result = string.Format("String ({0})", length);
        else
          result = "Text";
      }

      if (type == CxAttributeMetadata.TYPE_BOOLEAN)
      {
        result = "Checkmark";
      }

      if (type == CxAttributeMetadata.TYPE_DATE)
      {
        result = "Date";
      }

      if (type == CxAttributeMetadata.TYPE_TIME)
      {
        result = "Time";
      }

      if (type == CxAttributeMetadata.TYPE_DATETIME)
      {
        result = "Datetime";
      }

      if (winControl == CxWinControlNames.WIN_CONTROL_PERCENT &&
         (type == CxAttributeMetadata.TYPE_INT ||
          type == CxAttributeMetadata.TYPE_FLOAT))
      {
        result = "Percentage";
      }

      if (winControl == CxWinControlNames.WIN_CONTROL_FILE)
      {
        result = "File";
      }

      if (result == null &&
         (type == CxAttributeMetadata.TYPE_INT) && !rowSourceDefined)
      {
        result = "Numeric (Integer)";
      }

      if (result == null &&
         (type == CxAttributeMetadata.TYPE_FLOAT) && !rowSourceDefined)
      {
        result = "Numeric (Floating-Point)";
      }

      if (type == CxAttributeMetadata.TYPE_IMAGE)
      {
        result = "Image";
      }

      return result;
    }
    //-------------------------------------------------------------------------
  }
}
