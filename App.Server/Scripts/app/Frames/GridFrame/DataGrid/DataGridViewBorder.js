function DataGridViewBorder()
{
    this.LoadData = function (data, selected, openMode)
    {
        var meta = Metadata.GetEntityUsage(data.EntityUsageId);

        var headerTemplate = HtmlProvider.GetTemplateText('DataGridColHeaderTemplate');
        var rowTemplate = HtmlProvider.GetTemplateText('DataGridRowTemplate');
        var cellTemplate = HtmlProvider.GetTemplateText('DataGridCellTemplate');
        var headRowTemplate = HtmlProvider.GetTemplateText('DataGridHeadRowTemplate');
        var cursorTemplate = HtmlProvider.GetTemplateText('CursorColumn');
        var cursorHeaderTemplate = HtmlProvider.GetTemplateText('CursorHeaderTemplate');
        
        var multiselect = false;
        for (var i = 0; i < meta.Commands.length; i++)
        {
            if (meta.Commands[i].IsMultiple)
            {
                multiselect = true;
                break;
            }
        }

        if (!meta.MultilselectionAllowded)
        {
            multiselect = false;
        }

        var headers = [];
        var inGridAttrs = {};
        var attrsIndexes = {};

        var fileStateAttrs = {};

        for (var k = 0; k < meta.GridOrderedAttributes.length; k++)
        {
            var attrObj = meta.Attributes[meta.GridOrderedAttributes[k]];
            if (attrObj.Type == 'file' || attrObj.Type == 'photo')
            {
                fileStateAttrs[attrObj.Id + 'STATE'] = true;
            }

        }



        if (meta.GridOrderedAttributes.length > 0)
        {
            headers.push(new VCursorColumnHeader(cursorHeaderTemplate, multiselect));
        }
        for (var i = 0; i < meta.GridOrderedAttributes.length; i++)
        {
            var attrObj = meta.Attributes[meta.GridOrderedAttributes[i]];

           

            inGridAttrs[attrObj.Id] = attrObj;
            if (fileStateAttrs[attrObj.Id])
                continue;
            headers.push(new VColumnHeader(attrObj, headerTemplate));
        }
        for (var i = 0; i < data.EntityList.AttrsInSet.length; i++)
        {
            var inGridAttr = inGridAttrs[data.EntityList.AttrsInSet[i]]
            if (inGridAttr)
                inGridAttr.IndexInSet = i;

        }



    


        var headRow = new VGridHeaderRow(headRowTemplate, headers, cursorTemplate, multiselect);


        var rowsources = { Filtered: data.EntityList.FilteredRowSources, Unfiltered: data.EntityList.UnfilteredRowSources, Other: data.RowSources };

        var rows = [];
        for (var i = 0; i < data.EntityList.Rows.length; i++)
        {
            var isSelected = false;
            for (var x = 0; x < selected.length; x++)
            {
                if (selected[x] == data.EntityList.Rows[i])
                    isSelected = true;
            }

            var row = new VGridRow(meta.GridOrderedAttributes, rowTemplate, i, data.EntityList.Rows[i], cursorTemplate, isSelected);
         
            rows.push(row);

          
            
            for (var k = 0; k < meta.GridOrderedAttributes.length; k++)
            {
                var attrObj = meta.Attributes[meta.GridOrderedAttributes[k]];

                if(fileStateAttrs[attrObj.Id])
                    continue;

                var inGridAttrObj = inGridAttrs[attrObj.Id];

                var bDataItem = data.EntityList.Rows[i][inGridAttrObj.IndexInSet];

                var fileState;
                var pkVals;
                if (attrObj.Type == 'file' || attrObj.Type == 'photo')
                {
                    var inGridStateAttrObj = inGridAttrs[attrObj.Id + 'STATE'];
                    var state = data.EntityList.Rows[i][inGridStateAttrObj.IndexInSet];
                    fileState = state.Value;
                    
                }

                row.AddCell(new VGridCell(attrObj, cellTemplate, bDataItem, rowsources, meta.WordwrapRowdata, fileState, meta));
                
            }

        }

        var sorts = {};

        for (var i = 0; i < data.EntityList.SortDescriptions.length; i++)
        {
            sorts[data.EntityList.SortDescriptions[i].AttributeId] = data.EntityList.SortDescriptions[i];
        }


      



        return [rows, headRow, sorts, multiselect, data.EntityUsageId, openMode];
    };

  

    this.SelectedItemChanged = function (items)
    {
        var bItems = [];
        for (var i = 0; i < items.length; i++)
        {
            bItems.push(items[i].DataItem());
        }
        return [bItems];
    }

    this.Download = function (attrId, item)
    {
        return [attrId, item.DataItem()];
    }

};









