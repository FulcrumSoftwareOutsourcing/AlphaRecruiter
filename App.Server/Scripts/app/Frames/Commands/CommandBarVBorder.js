function CommandBarVBorder()
{
    this.CreateCommands = function (commands, selectedItem, openMode, isGridFrame)
    {
        var VCmds = [];
        for (var i = 0; i < commands.length; i++)
        {
            var bCmd = commands[i];
            var vCmd = {
                BCommand: bCmd,
                Visible: ko.observable(bCmd.Visible),
                ImageUrl: null,
                IsEnabled: ko.observable(bCmd.IsEnabled),
                
            };

            if (bCmd.IsEntityInstanceRequired && !selectedItem)
            {
                vCmd.IsEnabled(false);
            }

            if (bCmd.ImageId != '')
            {
                var imageMeta = Metadata.GetImage(bCmd.ImageId);
                if (imageMeta && imageMeta.Folder && imageMeta.FileName)
                    vCmd.ImageUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;
            }

            if (!isGridFrame)
            {

                if (openMode && (
                  openMode.toUpperCase() == NxOpenMode.Edit.toUpperCase() ||
                    //  openMode.toUpperCase() == NxOpenMode.View.toUpperCase() ||
                  openMode.toUpperCase() == NxOpenMode.New.toUpperCase() ||
                  openMode.toUpperCase() == NxOpenMode.ChildEdit.toUpperCase() ||
                    //   openMode.toUpperCase() == NxOpenMode.ChildView.toUpperCase() ||
                  openMode.toUpperCase() == NxOpenMode.ChildNew.toUpperCase()))
                {
                    if (!bCmd.AvailableOnEditform)
                    {
                        vCmd.Visible(false);

                    }
                }

            }


            VCmds.push(vCmd);
        }

        return [VCmds];
    };

    this.ExecuteCommand = function (command)
    {
        return command.BCommand;
    };

};
