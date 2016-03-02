'use strict';
function BaseFrameDataBorder()
{
    this.FrameDataLoaded = function (data)
    {
        Metadata.AddEntityUsage(data.Metadata);

        var rows = [];
        var entityUsage = Metadata.GetEntityUsage(data.EntityUsageId);

        data.EntityList.AttrsByIds = {};

        for (var i = 0; i < data.EntityList.AttrsInSet.length; i++)
        {
            var attrId = data.EntityList.AttrsInSet[i];
            data.EntityList.AttrsByIds[entityUsage.Attributes[attrId].Id] = entityUsage.Attributes[attrId];
        }


        //for (var i = 0; i < data.EntityList.Data.length; i++)
        //{
        //    var row = data.EntityList.Data[i];

        //    var rowObj = {};

        //    for (var k = 0; k < row.length; k++)
        //    {
        //        var bDataItem = row[k];
        //        var propName = data.EntityList.AttrsInSet[k];
        //        rowObj[propName] = bDataItem.Value;
        //        rowObj[propName + '_Readonly'] = bDataItem.Readonly;
        //        rowObj[propName + '_Visible'] = bDataItem.Visible;
        //        rowObj[propName + '_DisabledCommandIds'] = bDataItem.DisabledCommandIds;
        //        rowObj[propName + '_Attr'] = data.Metadata.Attributes[propName];
        //    }

        //    rows.push(rowObj);

        //}





        data.EntityList.AttrsInSet;
        return data;

    };
};