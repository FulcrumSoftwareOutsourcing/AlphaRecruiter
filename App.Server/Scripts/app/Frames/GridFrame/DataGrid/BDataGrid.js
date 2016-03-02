'use strict';
function BDataGrid()
{
    /// <field name='ViewLayer' type='VDataGrid_ForBusiness'/>
    BusinessLayer.call(this);
    this.PublicForParent = new BDataGrid_ForParent();
    this.PublicForView = new BDataGrid_ForView();
    var metadata;
    var selected;
    var me = this;

    this.LoadData = function (data, selectedItems, openMode)
    {
        metadata = Metadata.GetEntityUsage(data.EntityUsageId);
        selected = selectedItems;
        this.ViewLayer.LoadData(data, selectedItems, openMode);
        
        if (data.EntityList.Rows.length > 0)
        {
            var totals = [];
            for (var i = 0; i < metadata.GridOrderedAttributes.length; i++)
            {
                var attr = metadata.Attributes[metadata.GridOrderedAttributes[i]];

                

                if (attr.ShowTotal)
                {
                    
                    for (var k = 0; k < metadata.AttributesList.length; k++)
                    {
                        if (metadata.AttributesList[k].Id == attr.Id)
                        {
                            var total = CalcTotal(attr, data, k, metadata);
                            totals.push({ Total: total, Attr: attr, IndexInSet: i });
                        }
                    }



                }

            }
            if(totals.length > 0)
                this.ViewLayer.ShowTotals(totals);

        }
        

    }

    function CalcTotal(attr, data, indexInSet, metadata)
    {
        

        var calacFunction;
        if (attr.CalcTotalJs)
        {
            var funcName = "CalcTotal_" + attr.Id + metadata.Id;
            if(!window[funcName])
            {
                me.ViewLayer.CreateTotalCalcFunction(funcName, attr.CalcTotalJs);
            }
            calacFunction = window[funcName];
        }

        var dataToSum = [];
        for (var i = 0; i < data.EntityList.Rows.length; i++)
        {
            dataToSum.push(data.EntityList.Rows[i][indexInSet].Value);
        }
        
        var total = 0;
        if (calacFunction)
        {
            total = calacFunction(attr, dataToSum, metadata);
        }
        else
        {
            for (var i = 0; i < dataToSum.length; i++)
            {
                if (dataToSum[i] != undefined)
                {
                    try
                    {
                        var n = new Number(dataToSum[i]);
                    
                        if ( !isNaN(n) )
                            total += n;
                    }
                    catch(e){}
                }
                
            }
        }
        return total;
    }

    this.SelectedItemChanged = function (rows)
    {
        selected = rows;
        if (rows.length > 1)
            this.ViewLayer.SetSelectionMode('m');
        else
            this.ViewLayer.SetSelectionMode('s');
        
        this.ParentLayer.SelectedItemChanged(selected);
    }

    function IsFakeRow(row, metadata)
    {
        
    }

    this.SortingChanged = function (sorts)
    {
        if (sorts.length)
        {
            this.ParentLayer.SortingChanged(sorts); 
        }
    }

    this.Download = function (attrId, item)
    {
        this.ParentLayer.Download(attrId, item);
    }

    this.DymmicCommandClick = function (attrId, item)
    {
        this.ParentLayer.DymmicCommandClick(attrId, item);
    }

};

extend(BDataGrid, BusinessLayer);

function BDataGrid_ForParent()
{
    this.LoadData = function (data) { }
}

function BDataGrid_ForView()
{
    this.SelectedItemChanged = function (data) { }
    this.SortingChanged = function (sorts) { }
    this.Download = function (attrId, item) { }
    this.DymmicCommandClick = function (attrId, item) { }
    
}