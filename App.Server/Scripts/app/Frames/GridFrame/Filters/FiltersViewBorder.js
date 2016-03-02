function FiltersViewBorder()
{
    this.CreateFilters = function (bFilters, validator, isRecordCountLimited, recordCountLimit)
    {
        
        var generalItems = [];
        var advancedItems = [];
        

        for (var i = 0; i < bFilters.GeneralItems.length; i++)
        {
            var bFilterItem = bFilters.GeneralItems[i];
            var vFilterItem = new ObservableObject(bFilterItem, ['Value1', 'Value2', 'SelectedOperation', 'ValuesCount'], validator);
            vFilterItem.BFilter = bFilterItem;
            vFilterItem.ItsFilterReset = false;
            vFilterItem.RsOpen = ko.observable(false);
            vFilterItem.ValuesCount = ko.observable(0);

            vFilterItem.OrigValue1 = bFilterItem.Value1;
            vFilterItem.OrigValue2 = bFilterItem.Value2;
            vFilterItem.OrigSelectedOperation = bFilterItem.SelectedOperation;

            AddRowsourceData(vFilterItem, vFilterItem.BFilter.Attr, bFilters.Rowsources);
            generalItems.push(vFilterItem);
        }
        for (var i = 0; i < bFilters.AdvancedItems.length; i++)
        {
            var bFilterItem = bFilters.AdvancedItems[i];
            var vFilterItem = new ObservableObject(bFilterItem, ['Value1', 'Value2', 'SelectedOperation', 'ValuesCount'], validator);
            vFilterItem.BFilter = bFilterItem;
            vFilterItem.ItsFilterReset = false;
            vFilterItem.ValuesCount = ko.observable(0);
            vFilterItem.RsOpen = ko.observable(false);

            vFilterItem.OrigValue1 = bFilterItem.Value1;
            vFilterItem.OrigValue2 = bFilterItem.Value2;
            vFilterItem.OrigSelectedOperation = bFilterItem.SelectedOperation;

            AddRowsourceData(vFilterItem, vFilterItem.BFilter.Attr, bFilters.Rowsources);
            advancedItems.push(vFilterItem);
        }

        return [generalItems, advancedItems, isRecordCountLimited, recordCountLimit];
    }

    

    function AddRowsourceData(vFilter, attr, rowsources)
    {
        if (attr.RowSourceId != '')
        {
            var rs = Metadata.GetStaticRowSource(attr.RowSourceId);
            if (!rs)
            {
                rs = rowsources[attr.RowSourceId];
            }

            for (var k = 0; k < rs.RowSourceData.length; k++)
            {
                var data = rs.RowSourceData[k];
                if (data.Value == 'True')
                    data.Value = true;
                if (data.Value == 'False')
                    data.Value = false;
            }


            vFilter.RsItems = ko.observableArray();
            vFilter.RsItems.pushAll(rs.RowSourceData);
            if (App.Utils.IsStringNullOrWhitespace(vFilter.RsItems()[0].Text))
            {
                vFilter.RsItems()[0].Value = undefined;
            }
            else
            {
                vFilter.RsItems.unshift({ Text: '', Value: undefined, ImageId: null });
            }
            vFilter.SelectedRsItem = ko.observable({ Text: '', Value: undefined, ImageId: null });
            vFilter.RsFilterControlOpen = ko.observable(false);

            if ( !App.Utils.IsStringNullOrWhitespace( attr.FilterDefault1 ) )
            {
                for (var k = 0; k < rs.RowSourceData.length; k++)
                {
                    if (vFilter.OrigValue1 == rs.RowSourceData[k].Value)
                    {
                        vFilter.SelectedRsItem().Text = rs.RowSourceData[k].Text;
                        vFilter.SelectedRsItem().Value = rs.RowSourceData[k].Value;
                        vFilter.SelectedRsItem().ImageId = rs.RowSourceData[k].ImageId;
                    }
                }
            }

        }
    };

    this.OnFilterItemClick = function (item, VFilter)
    {

    };

};

