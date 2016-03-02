'use strict';
function VFilters()
{
    ViewLayer.call(this);

    this.GeneralItems = [];
    this.AdvancedItems = [];
    var isBinded = false;
    this.GeneralOpened = ko.observable(false);
    this.AdvancedOpened = ko.observable(false);
    this.PublicForBusiness = new VFilters_ForBusiness();
    this.TempId = '';

    var dateControlsInitialized = false;
    this.IsRecordCountLimited = ko.observable(false);
    this.RecordCountLimit = ko.observable(0);
    var me = this;

    this.CreateFilters = function (generalItems, advancedItems, isRecordCountLimited, recordCountLimit)
    {
        this.IsRecordCountLimited(isRecordCountLimited);
        this.RecordCountLimit(recordCountLimit);

        this.GeneralItems = generalItems;
        this.AdvancedItems = advancedItems;

        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            this.GeneralItems[i].ValuesCount(GetFilterOperationValueCount(this.GeneralItems[i].SelectedOperation()))
            this.GeneralItems[i].OnPropertyChangedDelegate = new Delegate(FilterChanged, this);
        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            this.AdvancedItems[i].ValuesCount(GetFilterOperationValueCount(this.AdvancedItems[i].SelectedOperation()))
            this.AdvancedItems[i].OnPropertyChangedDelegate = new Delegate(FilterChanged, this);
        }

        if (!isBinded)
        {
            this.TempId = this.ParentLayer.GetTempId();
            var holder = document.getElementById('filters_panel' + this.TempId);

            ko.cleanNode(holder);
            $(holder).html('');
            $(holder).html('<div class="FiltersPanel unselectable" data-bind="template: {name: \'GridFrame/FiltersPanel/FiltersPanelTemplate\' }"></div>');

       

            ko.applyBindings(this, holder);

     
            if (!dateControlsInitialized)
            {

                jQuery('input[id^="' + this.TempId + 'datepicker_"]').datetimepicker({
                    format: Metadata.APPLICATION$CLIENTDATEFORMAT,
                    lang: Metadata.APPLICATION$LANGUAGECODE,
                    // mask: true,
                    timepicker: false,

                    step: 1
                });

                jQuery('input[id^="' + this.TempId + 'datetimepicker_"]').datetimepicker({
                    format: Metadata.APPLICATION$CLIENTDATETIMEFORMAT,
                    lang: Metadata.APPLICATION$LANGUAGECODE,
                    // mask: true,

                    step: 1
                });
                dateControlsInitialized = true;
            }

            isBinded = true;
        }

        
        Resizer.ResizeRequest(new Delegate(this.Resize, this));

    };

    function FilterChanged(attr, newValue)
    {
        if (attr.Autofilter)
        {
            me.FindClick();
        }
    };

    this.Resize = function ()
    {

    };

    this.GeneralItemsExpanderClicked = function ()
    {
        this.GeneralOpened(!this.GeneralOpened());
        Resizer.ResizeRequest(new Delegate(this.Resize, this));
    }

    this.AdvancedItemsExpanderClicked = function ()
    {
        this.AdvancedOpened(!this.AdvancedOpened());
        Resizer.ResizeRequest(new Delegate(this.Resize, this));
    }

    this.GetRows = function (items)
    {
        var pairs = [];
        var pair = [];
        var i = 0;
        while (true)
        {
            if (!items[i])
                break;
            if (pair.length == 2)
            {
                pairs.push(pair);
                pair = [];
            }

            pair.push(items[i]);
            i++;
        }
        if (pair.length > 0)
            pairs.push(pair);

        return pairs;
    }

    this.OnOperationItemClick = function (operation, filter)
    {
        filter.SelectedOperation(operation);
        var count = GetFilterOperationValueCount(operation);
        filter.ValuesCount(count);
        if (count == 0)
        {
            filter.Value1(undefined);
            filter.Value2(undefined);
        }
        if (count == 1)
        {
            filter.Value2(undefined);
        }
    };

    function GetFilterOperationValueCount(operation)
    {
        switch (operation)
        {
            case NxFilterOperation.Equal:
            case NxFilterOperation.NotEqual:
            case NxFilterOperation.Less:
            case NxFilterOperation.Greater:
            case NxFilterOperation.LessEqual:
            case NxFilterOperation.GreaterEqual:
            case NxFilterOperation.Like:
            case NxFilterOperation.NotLike:
            case NxFilterOperation.StartsWith:
            case NxFilterOperation.NotExists:
                return 1;
            case NxFilterOperation.Between:
                return 2;
            case NxFilterOperation.IsNull:
            case NxFilterOperation.IsNotNull:
            case NxFilterOperation.Today:
            case NxFilterOperation.Yesterday:
            case NxFilterOperation.ThisWeek:
            case NxFilterOperation.PrevWeek:
            case NxFilterOperation.ThisMonth:
            case NxFilterOperation.PrevMonth:
            case NxFilterOperation.ThisYear:
            case NxFilterOperation.PrevYear:
            case NxFilterOperation.InThePast:
            case NxFilterOperation.TodayOrLater:
            case NxFilterOperation.Tomorrow:
            case NxFilterOperation.Myself:
                return 0;
        }
        return 0;
    };

    this.GetFilterOperationText = function (operation)
    {
        switch (operation)
        {
            case NxFilterOperation.None:
                return "";
            case NxFilterOperation.Equal:
                return "=";
            case NxFilterOperation.NotEqual:
                return "<>";
            case NxFilterOperation.Less:
                return "<";
            case NxFilterOperation.Greater:
                return ">";
            case NxFilterOperation.LessEqual:
                return "<=";
            case NxFilterOperation.GreaterEqual:
                return ">=";
            case NxFilterOperation.Between:
                return GetTxt("Between");
            case NxFilterOperation.Like:
                return GetTxt("Like");
            case NxFilterOperation.NotLike:
                return GetTxt("Not Like");
            case NxFilterOperation.StartsWith:
                return GetTxt("Starts With");
            case NxFilterOperation.IsNull:
                return GetTxt("Is Null");
            case NxFilterOperation.IsNotNull:
                return GetTxt("Is Not Null");
            case NxFilterOperation.Today:
                return GetTxt("Today");
            case NxFilterOperation.Yesterday:
                return GetTxt("Yesterday");
            case NxFilterOperation.ThisWeek:
                return GetTxt("This Week");
            case NxFilterOperation.PrevWeek:
                return GetTxt("Prev Week");
            case NxFilterOperation.ThisMonth:
                return GetTxt("This Month");
            case NxFilterOperation.PrevMonth:
                return GetTxt("Prev Month");
            case NxFilterOperation.ThisYear:
                return GetTxt("This Year");
            case NxFilterOperation.PrevYear:
                return GetTxt("Prev Year");
            case NxFilterOperation.InThePast:
                return GetTxt("In the Past");
            case NxFilterOperation.TodayOrLater:
                return GetTxt("Today or Later");
            case NxFilterOperation.Tomorrow:
                return GetTxt("Tomorrow");
            case NxFilterOperation.NotExists:
                return GetTxt("Not Exists");
            case NxFilterOperation.Myself:
                return GetTxt("Myself");
        }

    };

    this.OnFilterOperationClicked = function (item)
    {
        if (!item.RsOpen())
        {
            item.RsOpen(true);
            this.ParentLayer.SetNewOpenedRowSource(item);
        }
        else
        {
            item.RsOpen(false);
            this.ParentLayer.SetNewOpenedRowSource(null);
        }
    };

    this.GetFilterControlTemplateName = function(filterItem)
    {
        var attr = filterItem.BFilter.Attr;
        var mode = 'edit_';
     
        if (attr.Type == 'boolean' && !App.Utils.IsStringNullOrWhitespace(attr.RowSourceId) && attr.RowSourceId.toUpperCase() == 'Boolean_Lookup'.toUpperCase())
        {
            return 'filter_' + mode + ControlsTemplatesIds.RowSourceComboBox;
        }

        if (!App.Utils.IsStringNullOrWhitespace(attr.RowSourceId))
        {
            return 'filter_' + mode + ControlsTemplatesIds.RowSourceComboBox;
        }
        else
        {
            var type = attr.Type;
            if (!App.Utils.IsStringNullOrWhitespace(attr.SlControl))
                type = attr.SlControl;

            var template = ControlsTemplatesIds[type];
            if (!template)
                template = ControlsTemplatesIds.string;


            return 'filter_' + mode + template;

        }

    };

  

    this.GetRowSourceImgUrl = function (rsItem, attr)
    {
        if (!rsItem || App.Utils.IsStringNullOrWhitespace(rsItem.ImageId))
            return;

       

        if (!Metadata.RsImgsUrls[attr.RowSourceId])
        {
            Metadata.RsImgsUrls[attr.RowSourceId] = {};
        }

        if (!Metadata.RsImgsUrls[attr.RowSourceId][rsItem.ImageId])
        {
            var imageMeta = Metadata.GetImage(rsItem.ImageId);
            var url = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;
            Metadata.RsImgsUrls[attr.RowSourceId][rsItem.ImageId] = url;
        }
        return Metadata.RsImgsUrls[attr.RowSourceId][rsItem.ImageId];

    };

    this.IsRowSourceItemWithImage = function (rsItem)
    {
        return (rsItem && !App.Utils.IsStringNullOrWhitespace(rsItem.ImageId));
    };

   

    this.OnFilterRowSourceClick = function (item)
    {
        if (item.RsFilterControlOpen())
        {
            item.RsFilterControlOpen(false);
            this.ParentLayer.SetNewOpenedRowSource(null);

        }
        else
        {
            item.RsFilterControlOpen(true);
            this.ParentLayer.SetNewOpenedRowSource(item);
        }


        
    };

    this.OnFilterItemClick = function (item, VFilter, forValue)
    {
        VFilter.SelectedRsItem(item);
        if (forValue == 1)
        {
            VFilter.Value1(item.Value);
        }
        else
        {
            VFilter.Value2(item.Value);
        }
        
    };

    this.FindClick = function ()
    {
        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            this.GeneralItems[i].ToBusinessObject();
        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            this.AdvancedItems[i].ToBusinessObject();
        }

        this.BusinessLayer.Find();
    };

    this.ResetClick = function ()
    {
        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            ResetFiltrValues.call(this.GeneralItems[i]);
        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            ResetFiltrValues.call(this.AdvancedItems[i]);
        }

        //this.BusinessLayer.Find();
    };

    function ResetFiltrValues()
    {
        this.ItsFilterReset = true;
        if (this.RsItems && this.RsItems().length > 0)
        {
            this.SelectedRsItem(this.RsItems()[0]);
        }

        this.Value1(  undefined) ;
        this.Value2(undefined);
        this.SelectedOperation(this.OrigSelectedOperation);
        this.ValuesCount(GetFilterOperationValueCount(this.OrigSelectedOperation));
        this.ToBusinessObject();
        this.ItsFilterReset = false;
    };

    this.OpenPanels = function (toOpen)
    {
        if (toOpen.general)
        {
            this.GeneralOpened(true);
            
        }
        if (toOpen.advanced)
        {
            this.AdvancedOpened(true);
            this.GeneralOpened(true);
        }
            
        if(toOpen.general || toOpen.advanced)
            Resizer.ResizeRequest(new Delegate(this.Resize, this));

    }

    this.Dispose = function ()
    {
        for (var i = 0; i < this.GeneralItems.length; i++)
        {
            this.GeneralItems[i].OnPropertyChangedDelegate = null;
        }
        for (var i = 0; i < this.AdvancedItems.length; i++)
        {
            this.AdvancedItems[i].OnPropertyChangedDelegate = null;
        }
    }
};

extend(VFilters, ViewLayer);

function VFilters_ForBusiness()
{
    this.CreateFilters = function (generalItems, advancedItems) { }
    this.OpenPanels = function (toOpen) { }
}



