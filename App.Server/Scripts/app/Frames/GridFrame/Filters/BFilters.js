'use strict';
function BFilters()
{
    /// <field name='ViewLayer' type='VFilters_ForBusiness'/>
    BusinessLayer.call(this);
    this.PublicForParent = new BFilters_ForParent();
    this.PublicForView = new BFilters_ForView();
    this.GeneralItems = [];
    this.AdvancedItems = [];
    this.HasDefaultValues = false;
    this.Rowsources = null;
    var initialized = false;

    this.Run = function (metadataId, rowsorses)
    {
        var metadata = Metadata.GetEntityUsage(metadataId);

        if (!metadata.IsFilterEnabled)
            return;

        if (!initialized)
        {
            this.Rowsources = rowsorses;
            
           


            for (var i = 0; i < metadata.FilterableIds.length; i++)
            {
                var filterableId = metadata.FilterableIds[i];
                var attributeMetadata = metadata.Attributes[filterableId];

                var filterItem = new FilterItem(attributeMetadata);

                if (attributeMetadata.FilterAdvanced)
                {
                    this.AdvancedItems.push(filterItem);
                }
                else
                {
                    this.GeneralItems.push(filterItem);
                }
                if (!App.Utils.IsStringNullOrWhitespace(attributeMetadata.FilterDefault1) ||
                    !App.Utils.IsStringNullOrWhitespace(attributeMetadata.FilterDefault2))
                {
                    this.HasDefaultValues = true;
                }
            }
            this.ViewLayer.CreateFilters(this, null, metadata.IsRecordCountLimited, metadata.RecordCountLimit);
            initialized = true;
        }

        this.ViewLayer.OpenPanels(GetToOpenPanels.call(this, metadata));
        
    }

    this.Find = function ()
    {
        var errors = ValidateFilters.call(this);
        if (errors.length > 0)
        {
            ShowMessage(errors, GetTxt('Filters validation'), [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Warning);
            return;
        }

        var filters = []
        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            var filter = this.GeneralItems[i];

            if(filter.ValuesCount == 0 && filter.DefaultOperation != filter.SelectedOperation)
            {
                filters.push(ToFrameFilterItem(filter));
            }
            if (filter.Value1 != undefined || filter.Value2 != undefined)
            {
                filters.push(ToFrameFilterItem(filter));
            }
            if (filter.DefaultOperation == 'Today' ||
                filter.DefaultOperation == 'ThisWeek' ||
                filter.DefaultOperation == 'ThisMonth' ||
                filter.DefaultOperation == 'ThisYear' ||
                filter.DefaultOperation == 'Yesterday' ||
                filter.DefaultOperation == 'PrevWeek' ||
                filter.DefaultOperation == 'PrevMonth' ||
                filter.DefaultOperation == 'PrevYear' ||
                filter.DefaultOperation == 'InThePast' ||
                filter.DefaultOperation == 'TodayOrLater' ||
                filter.DefaultOperation == 'Tomorrow')
            {
                filters.push(ToFrameFilterItem(filter));
            }


        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            var filter = this.AdvancedItems[i];

            if (filter.ValuesCount == 0 && filter.DefaultOperation != filter.SelectedOperation)
            {
                filters.push(ToFrameFilterItem(filter));
            }
            if (filter.Value1 != undefined || filter.Value2 != undefined)
            {
                filters.push(ToFrameFilterItem( filter ) );
            }
            if (filter.DefaultOperation == 'Today' ||
                filter.DefaultOperation == 'ThisWeek' ||
                filter.DefaultOperation == 'ThisMonth' ||
                filter.DefaultOperation == 'ThisYear' ||
                filter.DefaultOperation == 'Yesterday' ||
                filter.DefaultOperation == 'PrevWeek' ||
                filter.DefaultOperation == 'PrevMonth' ||
                filter.DefaultOperation == 'PrevYear' ||
                filter.DefaultOperation == 'InThePast' ||
                filter.DefaultOperation == 'TodayOrLater' ||
                filter.DefaultOperation == 'Tomorrow')
            {
                filters.push(ToFrameFilterItem(filter));
            }
        }

        this.ParentLayer.Find(filters);

    };

    function GetToOpenPanels( metadata)
    {
        var toOpen = { general: false, advanced: false };

        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            var filter = this.GeneralItems[i];

            if (filter.ValuesCount == 0 && filter.DefaultOperation != filter.SelectedOperation)
            {
                toOpen.general = true;
                break;
            }
            if (filter.Value1 != undefined || filter.Value2 != undefined)
            {
                toOpen.general = true;
                break;
            }

        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            var filter = this.AdvancedItems[i];

            if (filter.ValuesCount == 0 && filter.DefaultOperation != filter.SelectedOperation)
            {
                toOpen.advanced = true;
                break;
            }
            if (filter.Value1 != undefined || filter.Value2 != undefined)
            {
                toOpen.advanced = true;
                break;
            }
        }

        if(metadata.SlFilterOnStart )
            toOpen.general = true;

        return toOpen;
    };

    function ToFrameFilterItem(filter)
    {
        return { Name: filter.Id, OperationAsString: filter.SelectedOperation, Values: [filter.Value1, filter.Value2] };
    }

    this.Reset = function ()
    {

    };

    

    function ValidateFilters()
    {
        var messages = [];
        var err = GetTxt("Please fill '") + "#" + GetTxt("' filter field.");
        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            var editor = this.GeneralItems[i];
           
            if (editor.FilterMandatory &&
                editor.ValuesCount == 0)
            {
                continue;
            }

            if (editor.FilterMandatory && 
                editor.ValuesCount == 1 &&
                editor.Value1 == undefined)
            {
                messages.push(err.replace('#', editor.Text));
            }

            if (editor.FilterMandatory &&
                editor.ValuesCount == 2 &&
              (editor.Value1 == undefined || editor.Value2== undefined))
            {
                messages.push(err.replace('#', editor.Text));
            }

          
            
        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            var editor = this.AdvancedItems[i];

            if (editor.FilterMandatory &&
                editor.ValuesCount == 0)
            {
                continue;
            }

            if (editor.FilterMandatory &&
                editor.ValuesCount == 1 &&
                editor.Value1 == undefined)
            {
                messages.push(err.replace('#', editor.Text));
            }

            if (editor.FilterMandatory &&
                editor.ValuesCount == 2 &&
              (editor.Value1 == undefined || editor.Value2 == undefined))
            {
                messages.push(err.replace('#', editor.Text));
            }

          
        }

        return messages;
    }

    this.Apply = function ()
    {
        this.Find();
    };

}
extend(BFilters, BusinessLayer);

function BFilters_ForParent()
{
    this.Run = function (metadataId) { };
    this.Apply = function () { };
};

function BFilters_ForView()
{
    this.Find = function () { };
    this.Reset = function () { };
};



function FilterItem(metadata)
{
    this.Value1 = undefined;
    this.Value2 = undefined;
    this.Id = metadata.Id;
    this.Text = metadata.Caption;
    this.DefaultOperation = null;
    this.SelectedOperation = null;
    this.Operations = [];
    this.DefaultValue1 = undefined;
    this.DefaultValue2 = undefined;
    this.FilterMandatory = null;
    this.Type = metadata.Type;
    this.Attr = metadata;
    this.ValuesCount = 0;

    if (App.Utils.IsStringNullOrWhitespace (metadata.FilterDefaultOperation))
    {
        if (metadata.FilterOperations.length > 0)
        {
            this.SelectedOperation = this.DefaultOperation =  metadata.FilterOperations[0];
        }
        else
        {
            this.SelectedOperation = this.DefaultOperation = NxFilterOperation.None;
        }
    }
    else
    {
        this.SelectedOperation = this.DefaultOperation = metadata.FilterDefaultOperation;
    }

    for(var i = 0; i < metadata.FilterOperations.length; i++)
    {
        var strOperation = metadata.FilterOperations[i];
        this.Operations.push(strOperation);
    }

    if (!App.Utils.IsStringNullOrWhitespace(metadata.FilterDefault1))
    {
        this.Value1 = this.DefaultValue1 = metadata.FilterDefault1;
    }
    if (!App.Utils.IsStringNullOrWhitespace(metadata.FilterDefault2))
    {
        this.Value2 = this.DefaultValue2 = metadata.FilterDefault2;
    }
    if (metadata.Type == "date" || metadata.Type == "datetime")
    {
        this.Value1 = this.DefaultValue1 = undefined;
        this.Value2 = this.DefaultValue2 = undefined;
    }
   
    


    //Today,
    //   ThisWeek,
    //   ThisMonth,
    //   ThisYear,
    //   Yesterday,
    //   PrevWeek,
    //   PrevMonth,
    //   PrevYear,
    //   InThePast,
    //   TodayOrLater,
    //   Tomorrow,


    this.FilterMandatory = metadata.FilterMandatory;
   
};

