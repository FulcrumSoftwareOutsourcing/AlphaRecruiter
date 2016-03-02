function BaseFrameViewBorder()
{
    this.FrameDataLoaded = function (bData, validator, templates, openMode)
    {
        var metadata = Metadata.GetEntityUsage(bData.EntityUsageId);
        //var rowsources = { Filtered: bData.EntityList.FilteredRowSources, Unfiltered: bData.EntityList.UnfilteredRowSources, Other: bData.RowSources };
       

        var attrsByIds = {};
        if (bData.EntityList.Rows.length > 0)
        {
            attrsByIds = bData.EntityList.AttrsByIds;
        }
        else
        {
            for (var i = 0; i < metadata.GridOrderedAttributes.length; i++)
            {
                attrsByIds[metadata.Attributes[metadata.GridOrderedAttributes[i]].Id] = metadata.Attributes[metadata.GridOrderedAttributes[i]];
            }
        }

        var attrsInSet;
        if (bData.EntityList.Rows.length > 0)
            attrsInSet = bData.EntityList.AttrsInSet;
        else
            attrsInSet = metadata.GridOrderedAttributes;


        //var vRows = [];


        //for (var i = 0; i < bData.EntityList.Rows.length; i++)
        //{

         
        //    var vRow = new ObservableObject(bData.EntityList.Rows[i], attrsInSet, validator, rowsources, metadata, attrsByIds, true);

        //    vRows.push(vRow);

        //}



        return [bData.EntityUsageId, attrsInSet, attrsByIds, bData.EntityList.Rows, templates, openMode];

    };

    this.FrameButtonsCreated = function (saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr)
    {
        return [
            {
                Visible: ko.observable(saveAndStayCmdDescr.Visible),
                ExecDelegate:saveAndStayCmdDescr.ExecDelegate
            },
            {
                Visible: ko.observable(saveAndCloseCmdDescr.Visible),
                ExecDelegate: saveAndCloseCmdDescr.ExecDelegate
            },
            {
                Visible: ko.observable(cancelCmdDescr.Visible),
                ExecDelegate: cancelCmdDescr.ExecDelegate
            },
        ];

        
    };

};