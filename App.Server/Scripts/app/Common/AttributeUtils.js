function AttributeUtils()
{
  

    //----------------------------------------------------------------------------

    // Silverlight control types
    var SL_CONTROL_TEXT = "text";
    var SL_CONTROL_PASSWORD = "password";
    var SL_CONTROL_CHECKBOX = "checkbox";
    var SL_CONTROL_SPIN = "spin";
    var SL_CONTROL_CALC = "calc";
    var SL_CONTROL_DATE = "date";
    var SL_CONTROL_MEMO = "memo";
    var SL_CONTROL_MEMOPOPUP = "memopopup";
    var SL_CONTROL_HTML = "html";
    var SL_CONTROL_COMBOEDIT = "comboedit";
    var SL_CONTROL_DROPDOWN = "dropdown";
    var SL_CONTROL_DROPDOWNIMAGE = "dropdown_image";
    var SL_CONTROL_DROPDOWNIMAGEONLY = "dropdown_imageonly";
    var SL_CONTROL_LOOKUP = "lookup";
    var SL_CONTROL_LOOKUP_MULTI = "lookup_multi";
    var SL_CONTROL_LOOKUP_SELECT = "lookup_select";
    var SL_CONTROL_TIME = "time";
    var SL_CONTROL_HYPERLINK = "hyperlink";
    var SL_CONTROL_HYPERLINK_EDIT = "hyperlinkedit";
    var SL_CONTROL_HYPERLINK_EDIT_FILE = "hyperlinkeditfile";
    var SL_CONTROL_EMAIL = "email";
    var SL_CONTROL_IMAGE = "image";
    var SL_CONTROL_IMAGEPOPUP = "imagepopup";
    var SL_CONTROL_BUTTONEDIT = "buttonedit";
    var SL_CONTROL_BUTTONTEXTEDIT = "buttontextedit";
    var SL_CONTROL_COLOR = "color";
    var SL_CONTROL_COLOR_NOTEXT = "color_notext";
    var SL_CONTROL_FILE = "file";
    var SL_CONTROL_PERCENT = "percent";
    var SL_CONTROL_MEMO_BUTTON_LABEL = "memo_buttonlabel";
    var SL_CONTROL_EMAILADDRESSBUTTONEDIT = "emailaddressbuttonedit";
    var SL_CONTROL_CALCULABLETEXTEDIT = "calculabletextedit";
    var SL_CONTROL_DATETIME = "datetime";
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns default Silverlight control constant by the 'type' metadata attribute.
    /// </summary>
    /// <param name="metadata">Attribute Metadata.</param>
    /// <returns>Name of control type.</returns>
    function GetDefaultSilverlightControl(attribute)
    {

        if (!string.IsNullOrEmpty(attribute.RowSourceId))
        {
            return SL_CONTROL_DROPDOWN;
        }

        if (App.Utils.IsStringNullOrWhitespace(attribute.Type))
        {
            throw new Error(string.Format("The 'type' is not defined for attribute  " + attribute.Id + " ."));
        }

        switch (attribute.Type)
        {
            case TYPE_STRING: return SL_CONTROL_TEXT;
            case TYPE_LONGSTRING: return SL_CONTROL_TEXT;
            case TYPE_DATETIME: return SL_CONTROL_DATETIME;
            case TYPE_DATE: return SL_CONTROL_DATE;
            case TYPE_TIME: return SL_CONTROL_TIME;
            case TYPE_INT: return SL_CONTROL_SPIN;
            case TYPE_FLOAT: return SL_CONTROL_CALC;
            case TYPE_BOOLEAN: return SL_CONTROL_CHECKBOX;
            case TYPE_FILE: return SL_CONTROL_FILE;
            case TYPE_IMAGE: return SL_CONTROL_IMAGE;
            case TYPE_ICON: return SL_CONTROL_DROPDOWNIMAGE;
            case TYPE_LINK: return SL_CONTROL_HYPERLINK;
        }
        return "";
    }

    this.GetControl = function (attribute)
    {
        if (App.Utils.IsStringNullOrWhitespace(attribute.SlControl))
        {
            return GetDefaultSilverlightControl(attribute);
        }
        return attribute.SlControl;
    }


    



}

//Attribute types
AttributeUtils.TYPE_STRING = "string";
AttributeUtils.TYPE_LONGSTRING = "longstring";
AttributeUtils.TYPE_DATETIME = "datetime";
AttributeUtils.TYPE_DATE = "date";
AttributeUtils.TYPE_TIME = "time";
AttributeUtils.TYPE_INT = "int";
AttributeUtils.TYPE_FLOAT = "float";
AttributeUtils.TYPE_BOOLEAN = "boolean";
AttributeUtils.TYPE_FILE = "file";
AttributeUtils.TYPE_IMAGE = "image";
AttributeUtils.TYPE_ICON = "icon";
AttributeUtils.TYPE_LINK = "hyperlink";