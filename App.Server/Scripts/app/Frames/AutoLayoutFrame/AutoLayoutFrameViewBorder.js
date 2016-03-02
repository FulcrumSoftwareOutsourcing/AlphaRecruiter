function AutoLayoutFrameViewBorder()
{
    BaseFrameViewBorder.call(this);

    this.FrameDataLoaded = function (bData, validator, templates, openMode) {
        var metadata = Metadata.GetEntityUsage(bData.EntityUsageId);


        var rowsources = { Filtered: bData.EntityList.FilteredRowSources, Unfiltered: bData.EntityList.UnfilteredRowSources, Other: bData.RowSources };

        

        var attrsByIds = {};
        if (bData.EntityList.Rows.length > 0) {
            attrsByIds = bData.EntityList.AttrsByIds;
        }
        else {
            for (var i = 0; i < metadata.GridOrderedAttributes.length; i++) {
                attrsByIds[metadata.Attributes[metadata.GridOrderedAttributes[i]].Id] = metadata.Attributes[metadata.GridOrderedAttributes[i]];
            }
        }

        var attrsInSet;
        if (bData.EntityList.Rows.length > 0)
            attrsInSet = bData.EntityList.AttrsInSet;
        else
            attrsInSet = metadata.GridOrderedAttributes;


        var vRows = [];


        for (var i = 0; i < bData.EntityList.Rows.length; i++) {

            //businessObject, observableProps, validator, rowsources, metadata, attrs, itIsDataRow
            var vRow = new ObservableObject(bData.EntityList.Rows[i], attrsInSet, validator, rowsources, metadata, attrsByIds, false);

            vRows.push(vRow);

        }



        return [bData.EntityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode];

    };

    this.SaveCurentEntity = function(entity, doneDelegate)
    {
        return [entity.ToBusinessObject(), doneDelegate, entity.WaitingToUpload];
    };

    this.DymmicCommandClick = function (entity, attr)
    {
        return [entity.ToBusinessObject(), attr];
    }

};