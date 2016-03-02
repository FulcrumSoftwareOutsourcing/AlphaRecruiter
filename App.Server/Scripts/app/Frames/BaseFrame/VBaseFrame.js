'use strict';
function VBaseFrame(className)
{
    ViewLayer.call(this);

    this.Type = 'base-fame';

    this.PublicForBusiness = new VBaseFrame_ForBusiness();
    this.PublicForChildren = new VBaseFrame_ForChild();

    this.EntityUsage = null;
    this.AttrsInSet = null;
    this.AttrsByIds = null;
    this.Caption = ko.observable('');

    this.UsedTemplatesIds = [];
    this.TempId = App.Utils.ShortGuid();

    var OpenedRowSource = null;

    var isBinded = false;
    this.DataLoaded = false;
    this.OpenMode = '';

    this.FrameHolderId = null;
    this.CurrentEntity = ko.observable();

    var me = this;

    this.SaveAndStayCmdDescr = { Visible: ko.observable(false) };
    this.SaveAndCloseCmdDescr = { Visible: ko.observable(false) };
    this.CancelCmdDescr = { Visible: ko.observable(false) };


    this.SaveAndStayCmdImageUrl;
    this.SaveAndCloseCmdImageUrl;
    this.CancelCmdImageUrl;

    this.DisplayAllRecordsWithoutFooter = false;

    var imageMeta = Metadata.GetImage('BlueDisk_48x48');
    if (imageMeta && imageMeta.Folder && imageMeta.FileName)
        this.SaveAndCloseCmdImageUrl = this.SaveAndStayCmdImageUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;

    var imageMeta = Metadata.GetImage('Undo_48x48');
    if (imageMeta && imageMeta.Folder && imageMeta.FileName)
        this.CancelCmdImageUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;


    this.OnBodyClick = function ()
    {


        if (OpenedRowSource)
        {
            if (OpenedRowSource.RsOpen)
                OpenedRowSource.RsOpen(false);
            if (OpenedRowSource.RsFilterControlOpen)
                OpenedRowSource.RsFilterControlOpen(false);
            OpenedRowSource = null;

            $('[ctrl-type="paging-popup"]').hide();
        }

    };

    function OnBodyKeyDown(e)
    {

        if (e.keyCode != 40 && e.keyCode != 38 && e.keyCode != 13)
        {
            if (OpenedRowSource)
            {
                if (OpenedRowSource.RsOpen)
                    OpenedRowSource.RsOpen(false);
                if (OpenedRowSource.RsFilterControlOpen)
                    OpenedRowSource.RsFilterControlOpen(false);
                OpenedRowSource = null;
            }
            $('[ctrl-type="paging-popup"]').hide();
        }

    };

    $(document).on("click", this.OnBodyClick);
    //  $(document).on("mousedown", this.OnBodyClick);
    $(document).on('keydown', OnBodyKeyDown);



    this.GetRequiredTemplatesIds = function ()
    {
        return HtmlProvider.GetTemplatesIdsToLoad(this.UsedTemplatesIds);
    };

    this.FrameDataLoaded = function (entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode)
    {
        HtmlProvider.AddTemplates(templates);




        this.EntityUsage = Metadata.GetEntityUsage(entityUsageId);
        this.AttrsInSet = attrsInSet;
        this.AttrsByIds = attrsByIds;
        this.OpenMode = openMode;

        this.Caption(this.GetFrameCaption(this));

        prevFileStates = {};
        for (var i = 0; i < attrsInSet.length; i++)
        {

            if (this.OpenMode == "Edit" || this.OpenMode == "View")
            {
                if (attrsByIds[attrsInSet[i]] && (attrsByIds[attrsInSet[i]].Type == 'file' || attrsByIds[attrsInSet[i]].Type == 'photo') && vRows.length > 0)
                {
                    prevFileStates[attrsInSet[i]] = vRows[0][attrsInSet[i] + 'STATE']();
                }

            }

        }



        if (!this.FrameHolderId)
        {
            Navigation.ShowFrame(this);
        }
        if (!isBinded)
        {
            if (this.FrameHolderId)
            {
                var holder = document.getElementById(this.FrameHolderId);
                ko.cleanNode(holder);
                $(holder).html('');
                $(holder).attr('data-bind', 'template:{name:"' + this.UsedTemplatesIds[0] + '"}');
                ko.applyBindings(this, holder);

            }
            else
            {
                ko.applyBindings(this, document.getElementById(this.TempId));


            }

            //for (var i = 0; i < attrsInSet.length; i++)
            //{

            //    if (this.OpenMode == "View")
            //    {

                  

            //        if (attrsByIds[attrsInSet[i]] && attrsByIds[attrsInSet[i]].Type == 'photo')
            //        {
            //            if (vRows[0][attrsInSet[i] + 'STATE']() == BlobStateAttrValues.BLOB_PRESENT_IN_DB)
            //            {
                           
            //                $('#thumb_view' + attrsInSet[i] + 'zzzz' + me.TempId).visible();
            //            }
                        
            //        }

            //    }

            //}

            isBinded = true;

        }





        Resizer.AddResizable('frame', new Delegate(this.Resize, this));
        Resizer.ResizeRequest(new Delegate(this.Resize, this));
        this.DataLoaded = true;
    };



    this.GetRowSourceImgUrl = function (rsItem, propObj, attrId)
    {
        if (!rsItem || App.Utils.IsStringNullOrWhitespace(rsItem.ImageId))
            return;

        var attr = this.AttrsByIds[attrId];

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

    this.OnRowSourceClick = function (propObj, attr, context)
    {
        if (propObj.RsOpen())
        {
            propObj.RsOpen(false);
            OpenedRowSource = null;
            return;
        }


        if (!propObj.RsOpen())
        {
            if (OpenedRowSource)
            {
                if (OpenedRowSource.RsOpen)
                    OpenedRowSource.RsOpen(false);
                if (OpenedRowSource.RsFilterControlOpen)
                    OpenedRowSource.RsFilterControlOpen(false);
                OpenedRowSource = null;
            }
            propObj.RsOpen(true);
            OpenedRowSource = propObj;
        }
    };

    this.OnRowSourceItemClick = function (clickedRsItem, propObj, attr, context)
    {
        if (propObj.SelectedRsItem() != clickedRsItem)
        {
            propObj.SelectedRsItem(clickedRsItem);
            propObj(clickedRsItem.Value);


        }
    };

    this.Resize = function ()
    {


    };

    this.GetTempId = function ()
    {
        return this.TempId;
    };

    this.Dispose = function ()
    {
        $(document).off("click", this.OnBodyClick);
        //$(document).off("mousedown", this.OnBodyClick);
        $(document).off('keydown', OnBodyKeyDown);
        Resizer.RemoveResizable(this);

        this.SaveAndStayCmdDescr = null;
        this.SaveAndCloseCmdDescr = null;
        this.CancelCmdDescr = null;
        if (this.BusinessLayer.Dispose)
            this.BusinessLayer.Dispose.call(this.BusinessLayer);
    };

    this.SaveAndClose = function ()
    {

    };

    this.SetFrameHolderId = function (id)
    {
        this.FrameHolderId = id;
    };

    this.GetIsFrameCaptionVisible = function (context)
    {
        if (!context.OpenMode)
            return true;
        if (context.OpenMode.indexOf('Child') == -1)
            return true;
        return false;
    }

    this.GetFrameCaption = function ()
    {
        if (!this.EntityUsage)
            return '';

        if (App.Utils.IsStringNullOrWhitespace(this.OpenMode))
            return this.EntityUsage.SingleCaption;


        var displayNane = '';
        if (this.EntityUsage && this.CurrentEntity())
        {
            for (var i = 0; i < this.EntityUsage.AttributesList.length; i++)
            {
                var attr = this.EntityUsage.AttributesList[i]
                if (attr.IsDisplayName)
                {
                    displayNane = this.CurrentEntity()[attr.Id]();
                    break;
                }
            }
            if (App.Utils.IsStringNullOrWhitespace(displayNane))
            {
                for (var i = 0; i < this.EntityUsage.AttributesList.length; i++)
                {
                    var attr = this.EntityUsage.AttributesList[i]
                    if (attr.Id == 'NAME')
                    {
                        displayNane = this.CurrentEntity()[attr.Id]();
                        break;
                    }
                }
            }
            if (App.Utils.IsStringNullOrWhitespace(displayNane))
            {
                for (var i = 0; i < this.EntityUsage.AttributesList.length; i++)
                {
                    var attr = this.EntityUsage.AttributesList[i]
                    if (attr.Id == 'TITLE')
                    {
                        displayNane = this.CurrentEntity()[attr.Id]();
                        break;
                    }
                }
            }
            if (App.Utils.IsStringNullOrWhitespace(displayNane))
            {
                for (var i = 0; i < this.EntityUsage.AttributesList.length; i++)
                {
                    var attr = this.EntityUsage.AttributesList[i]
                    if (attr.Id == 'CAPTION')
                    {
                        displayNane = this.CurrentEntity()[attr.Id]();
                        break;
                    }
                }
            }

            if (!App.Utils.IsStringNullOrWhitespace(displayNane))
                displayNane = ' "' + displayNane + '"';
            else
                displayNane = '';
        }






        if (this.OpenMode == NxOpenMode.Edit || this.OpenMode == NxOpenMode.ChildEdit)
            return GetTxt("Edit") + ' ' + this.EntityUsage.SingleCaption + displayNane;
        if (this.OpenMode == NxOpenMode.View || this.OpenMode == NxOpenMode.ChildView)
            return GetTxt("View") + ' ' + this.EntityUsage.SingleCaption + displayNane;
        if (this.OpenMode == NxOpenMode.New || this.OpenMode == NxOpenMode.ChildNew)
            return GetTxt("New") + ' ' + this.EntityUsage.SingleCaption + displayNane;
    }

    this.SetNewOpenedRowSource = function (rs)
    {
        if (OpenedRowSource)
        {
            if (OpenedRowSource.RsOpen)
                OpenedRowSource.RsOpen(false);
            if (OpenedRowSource.RsFilterControlOpen)
                OpenedRowSource.RsFilterControlOpen(false);
        }
        OpenedRowSource = rs;
    }

    var dateControlsInitialized = false;
    this.InitDateControls = function ()
    {
        if (!dateControlsInitialized)
        {
            jQuery('input[id^="' + this.TempId + 'datepicker"]').datetimepicker({
                format: Metadata.APPLICATION$CLIENTDATEFORMAT,
                lang: Metadata.APPLICATION$LANGUAGECODE,
                mask: true,
                timepicker: false,
                step: 1
            });

            jQuery('input[id^="' + this.TempId + 'datetimepicker"]').datetimepicker({
                format: Metadata.APPLICATION$CLIENTDATETIMEFORMAT,
                lang: Metadata.APPLICATION$LANGUAGECODE,
                mask: true,
                step: 1
            });

            dateControlsInitialized = true;
        }

    };

    this.ExpressionValuesChanged = function (data)
    {
    };

    this.ExecuteCommand = function (commandData)
    {
        this.BusinessLayer.ExecuteCommand(commandData);
    };

    this.UploadFileClick = function (attr, image)
    {
        if (window.File && window.FileReader && window.FileList && window.Blob)
        {

            $('#file_input_' + attr.Id + 'zzzz' + me.TempId).click();


        } else
        {
            ShowMessage(GetTxt('The File APIs are not fully supported in this browser.'), 'Not Supported', [DialogButtons.OK, DialogButtons.Cancel], NxMessageBoxIcon.Error);
        }
    }

    var prevFileStates = {};
    this.HandleFileSelect = function (data, evt)
    {
        var files = evt.target.files; // FileList object
        if (files.length == 0)
            return;

        var file = files[0];

        if (!App.Utils.CheckFileLenght(file))
        {
            return;
        }

        var attr = data.Attr;
        var stateAttrValue = me.CurrentEntity()[me.GetBlobStateAttributeId(attr.Id)];
        me.CurrentEntity()[attr.Id]('1');

        prevFileStates[attr.Id] = stateAttrValue();
        stateAttrValue(BlobStateAttrValues.BLOB_PRESENT_IN_DB);

        var attrValue = me.CurrentEntity()[attr.Id];

        //set file name in appropriated attribute 
        var fileNameAttr = attr.BlobFileNameAttributeId.toUpperCase()
        if (!App.Utils.IsStringNullOrWhitespace(fileNameAttr))
        {
            me.CurrentEntity()[fileNameAttr](file.name);
        }

        ///set file size in appropriated attribute 
        var fileSizeAttr = attr.BlobFileSizeAttributeId.toUpperCase();
        if (!App.Utils.IsStringNullOrWhitespace(fileSizeAttr))
        {
            me.CurrentEntity()[fileSizeAttr](file.size);
        }


        AddFileToUpload(file, attr.Id);
        $('#file_name_lbl_' + attr.Id + 'zzzz' + me.TempId).text(file.name);

    }




    function AddFileToUpload(file, attributeId)
    {
        if (!me.CurrentEntity().WaitingToUpload)
            me.CurrentEntity().WaitingToUpload = {};
        file.AttrId = attributeId;
        me.CurrentEntity().WaitingToUpload[attributeId] = file;

    }

    function RemoveFileToUpload(attributeId)
    {
        if (!me.CurrentEntity().WaitingToUpload)
            me.CurrentEntity().WaitingToUpload = {};

        delete me.CurrentEntity().WaitingToUpload[attributeId];
    }

    this.IsDownloadPossible = function (attr)
    {
        if (this.OpenMode == "New")
        {
            return false;
        }

    //    if (this.OpenMode == "Edit" && prevFileStates[attr] && prevFileStates[attr] != BlobStateAttrValues.BLOB_PRESENT_IN_DB || prevFileStates[attr] == "")
        //    return false;

        if (prevFileStates[attr] && prevFileStates[attr] != BlobStateAttrValues.BLOB_PRESENT_IN_DB)
            return false;

        if ((this.OpenMode == "Edit" || this.OpenMode == "View") &&
                this.CurrentEntity()[this.GetBlobStateAttributeId(attr)]() == BlobStateAttrValues.BLOB_PRESENT_IN_DB)
        {
            return true;
        }


        if (!App.Utils.IsStringNullOrWhitespace($('#file_input_' + attr + 'zzzz' + me.TempId).val()) &&
                this.CurrentEntity()[this.GetBlobStateAttributeId(attr)]() == BlobStateAttrValues.BLOB_PRESENT_IN_DB)
        {
            return true;
        }



        return false;
    }

    this.ClearFileClick = function (attr)
    {
        $('#file_input_' + attr.Id + 'zzzz' + me.TempId).val('');
        $('#file_name_lbl_' + attr.Id + 'zzzz' + me.TempId).text('');

        var stateAttrValue = this.CurrentEntity()[this.GetBlobStateAttributeId(attr.Id)];

        stateAttrValue(BlobStateAttrValues.REMOVE_BLOB_FROM_DB);

        var attrValue = this.CurrentEntity()[attr.Id];

        //set file name in appropriated attribute 
        var fileNameAttr = attr.BlobFileNameAttributeId.toUpperCase()
        if (!App.Utils.IsStringNullOrWhitespace(fileNameAttr))
        {
            this.CurrentEntity()[fileNameAttr](null);
        }

        ///set file size in appropriated attribute 
        var fileSizeAttr = attr.BlobFileSizeAttributeId.toUpperCase();
        if (!App.Utils.IsStringNullOrWhitespace(fileSizeAttr))
        {
            this.CurrentEntity()[fileSizeAttr](null);
        }

        RemoveFileToUpload(attr.Id);

    };

    this.GetBlobStateAttributeId = function (attributeId)
    {
        return attributeId + "STATE";
    }

    this.FrameButtonsCreated = function (saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr)
    {
        if (!this.SaveAndStayCmdDescr)
            this.SaveAndStayCmdDescr = saveAndStayCmdDescr;
        else
        {
            this.SaveAndStayCmdDescr.Visible(saveAndStayCmdDescr.Visible());
            this.SaveAndStayCmdDescr.ExecDelegate = saveAndStayCmdDescr.ExecDelegate;
        }

        if (!this.SaveAndCloseCmdDescr)
            this.SaveAndCloseCmdDescr = saveAndCloseCmdDescr;
        else
        {
            this.SaveAndCloseCmdDescr.Visible(saveAndCloseCmdDescr.Visible());
            this.SaveAndCloseCmdDescr.ExecDelegate = saveAndCloseCmdDescr.ExecDelegate;
        }

        if (!this.CancelCmdDescr)
            this.CancelCmdDescr = cancelCmdDescr;
        else
        {
            this.CancelCmdDescr.Visible(cancelCmdDescr.Visible());
            this.CancelCmdDescr.ExecDelegate = cancelCmdDescr.ExecDelegate;
        }


    };

    this.AdtCommandClick = function (cmd, context)
    {
        if (cmd == 'SaveAndStay' && context.SaveAndStayCmdDescr && context.SaveAndStayCmdDescr.ExecDelegate)
        {
            context.SaveAndStayCmdDescr.ExecDelegate.Invoke();
        }
        if (cmd == 'SaveAndClose' && context.SaveAndCloseCmdDescr && context.SaveAndCloseCmdDescr.ExecDelegate)
        {
            context.SaveAndCloseCmdDescr.ExecDelegate.Invoke();
        }
        if (cmd == 'Cancel' && context.CancelCmdDescr && context.CancelCmdDescr.ExecDelegate)
        {
            context.CancelCmdDescr.ExecDelegate.Invoke();
        }
    }
};

extend(VBaseFrame, ViewLayer);

function VBaseFrame_ForBusiness()
{
    this.GetRequiredTemplatesIds = function () { };
    this.FrameDataLoaded = function (entityUsageId, attrsInSet, attrsByIds, vRows, templates) { };
    this.SaveAndClose = function () { };
    this.SetFrameHolderId = function (id) { };
    this.ExpressionValuesChanged = function (data) { };
    this.Dispose = function (data) { };
    this.FrameButtonsCreated = function (saveAndStayCmdDescr, saveAndCloseCmdDescr, cancelCmdDescr) { };
};

function VBaseFrame_ForChild()
{
    this.GetTempId = function () { };
    this.SetNewOpenedRowSource = function (rs) { }

};




