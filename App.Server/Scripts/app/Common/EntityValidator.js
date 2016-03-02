'use strict';
function EntityValidator( entityUsageId )
{
    this.EntityUsage = Metadata.GetEntityUsage(entityUsageId);
    var validators = {};
    validators[AttributeUtils.TYPE_BOOLEAN] = new BooleanValidator();
    validators[AttributeUtils.TYPE_DATE] = new DateValidator()
    validators[AttributeUtils.TYPE_DATETIME] = new DateTimeValidator();
    validators[AttributeUtils.TYPE_FILE] = new FileValidator();
    validators[AttributeUtils.TYPE_FLOAT] = new FloatValidator();
    validators[AttributeUtils.TYPE_ICON] = new IconValidator();
    validators[AttributeUtils.TYPE_IMAGE] = new ImageValidator();
    validators[AttributeUtils.TYPE_INT] = new IntValidator();
    validators[AttributeUtils.TYPE_LINK] = new HyperlinkValidator();
    validators[AttributeUtils.TYPE_LONGSTRING] = new LongStringValidator();
    validators[AttributeUtils.TYPE_STRING] = new StringValidator();
    validators[AttributeUtils.TYPE_TIME] = new TimeValidator();

    this.Validate = function (value, attrName, errors)
    {
        var attr = this.EntityUsage.Attributes[attrName];
        var validator = validators[attr.Type];
        if(validator)
            validator.Validate(attr, value, errors)
        return errors;
    };

    


    this.ValidateAll = function (values)
    {
        //for (var i = 0; i < values.length; i++)
        //{
        //    var val = values[i];
        //    var vilidationMethod = this['Validate_' + val.Name];
        //    if (vilidationMethod)
        //        val.Errors = vilidationMethod(val.Value);
        //}
        //return values;
    };
}

function BaseAttributeValidator()
{
    
    this.TargetAttributeType = null;
    
    this.ValidateOnNullable = function(attributeMetadata, value, errors )
    {
        if(attributeMetadata.Nullable == false && (value == null || value == undefined) )
        {
            errors.push( GetTxt( "The attribute '") + attributeMetadata.FormCaption + GetTxt("' can not be null."));
        }
    
    }

    this.ValidateOnStringMaxLength = function(attributeMetadata, value, errors)
    {
        if (value == null)
        {
            return;
        }
        if (attributeMetadata.MaxLength > 0 && value.Length > attributeMetadata.MaxLength)
        {
            errors.push(GetTxt(  "The length of attribute '") + attributeMetadata.FormCaption + GetTxt(  "' can not be more then ") + attributeMetadata.MaxLength + "."); 
        }
     }   

    this.ValidateOnRange = function(attributeMetadata, value, errors)
    {
            if (value >  attributeMetadata.MaxValue ||
                value < attributeMetadata.MinValue)
            {

                errors.push(GetTxt(  "The range of attribute '") + attributeMetadata.FormCaption + GetTxt( "' should be between ") + attributeMetadata.MinValue + GetTxt( " and " ) + attributeMetadata.MaxValue + ".");
            
            }
        
    }

    this.ValidateIsNormalInteger = function (attributeMetadata, value, errors)
    {
        if (attributeMetadata.Nullable && value == null)
            return;

        var result = /^\+?(0|[1-9]\d*)$/.test(value);
        if(!result)
            errors.push(GetTxt("The attribute '") + attributeMetadata.FormCaption + GetTxt("' must be a number" + "."));
    }

    this.ValidateIsNormalFloat = function (attributeMetadata, value, errors)
    {
        if (attributeMetadata.Nullable && value == null)
            return;

        var result = !isNaN(parseFloat(value)) && isFinite(value);
        if (!result)
            errors.push(GetTxt("The attribute '") + attributeMetadata.FormCaption + GetTxt("' must be a number" + "."));
    }
}

function StringValidator() 
{
    BaseAttributeValidator.call(this);
    
    this.Validate = function (attributeMetadata, value, errors)
    {
        if (attributeMetadata.Nullable == false && App.Utils.IsStringNullOrWhitespace(value))
        {
            errors.push( GetTxt( "The attribute '") + attributeMetadata.FormCaption + GetTxt( "' can not be null."))
        }
        this.ValidateOnStringMaxLength(attributeMetadata, value, errors);
        
    }
}

function LongStringValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        if (attributeMetadata.Nullable == false && App.Utils.IsStringNullOrWhitespace(value))
        {
            errors.push(GetTxt("The attribute '") + attributeMetadata.FormCaption + GetTxt("' can not be null."))
        }
        this.ValidateOnStringMaxLength(attributeMetadata, value, errors);

    }
}

function DateTimeValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function DateValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function TimeValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function IntValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
        this.ValidateOnRange(attributeMetadata, value, errors);
        if (attributeMetadata.RowSourceId == '')
            this.ValidateIsNormalInteger(attributeMetadata, value, errors);
    }
}

function FloatValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
        this.ValidateOnRange(attributeMetadata, value, errors);
        if (attributeMetadata.RowSourceId == '')
            this.ValidateIsNormalFloat(attributeMetadata, value, errors);
    }
}

function BooleanValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function FileValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function ImageValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function IconValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}

function HyperlinkValidator()
{
    BaseAttributeValidator.call(this);

    this.Validate = function (attributeMetadata, value, errors)
    {
        this.ValidateOnNullable(attributeMetadata, value, errors);
    }
}