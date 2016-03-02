function VNavigationBar()
{
    var TreeItem = null;
    this.Frames = ko.observableArray([]);
    
    var treeItemChanged = false;
  
    var me = this;

    this.SelectedTreeItemChanged = function (treeItem)
    {
        //if (TreeItem != treeItem)
        //{
            TreeItem = treeItem;
            treeItemChanged = true;

            for (var i = 0; i < this.Frames().length; i++)
            {
                var current = this.Frames()[i];
                if (current.Dispose)
                    current.Dispose();
            }

            this.Frames.removeAll();
            


        //}

    };

   

    this.CheckCurrentHasChanges = function ()
    {
        if (this.Frames().length > 0)
        {
            var current = this.Frames()[this.Frames().length - 1]
            return current.HasChanges();
        }
        return false;
    }

    this.ShowFrame = function(frame)
    {
        if (treeItemChanged)
        {
            for (var i = 0; i < this.Frames().length; i++)
            {
                var f = this.Frames()[i];

                


                var node = document.getElementById(f.TempId);
                if(node)
                    ko.cleanNode(node);
                if (f.Dispose)
                    f.Dispose();
            }
            $('#FrameHolder').html('');
            this.Frames.removeAll();
            treeItemChanged = false;
        }

        var found;
        for (var i = 0; i < this.Frames().length; i++)
        {
            
            if (this.Frames()[i] == frame)
            {
                found = this.Frames()[i];
            }
        }

        if (!found)
        {
            for (var i = 0; i < this.Frames().length; i++)
            {
                $('#' + this.Frames()[i].TempId).hide();
            }


            this.Frames.push(frame);

            var fr = '<div style="display: none;" id="' + frame.TempId + '" data-bind="template:{name:\'' + frame.UsedTemplatesIds[0] + '\'}" c-type="' + frame.Type + '" >  </div>'
            $('#FrameHolder').append(fr);
            $('#' + frame.TempId).show();
        }
        else
        {
            for (var i = 0; i < this.Frames().length; i++)
            {
                $('#' + this.Frames()[i].TempId).hide();
            }
            $('#' + found.TempId).show();
        }
       
    }

    this.CloseCurrentFrame = function ()
    {
        var current = this.Frames()[this.Frames().length - 1]
        if (!current)
            return;

        if (current && current.CanClose && !current.CanClose.call(current))
        {
            return;
        }

        this.Frames.pop();

        var node = document.getElementById(current.TempId);
        if (node)
            ko.cleanNode(node);

        $('#' + current.TempId).remove();

        if (current.Dispose)
            current.Dispose();

        if (this.Frames().length > 0)
        {
            var newCurrent = this.Frames()[this.Frames().length - 1];
            $('#' + newCurrent.TempId).show();

            if (newCurrent.Refresh)
                newCurrent.Refresh();
        }

    }

    this.BackClick = function ()
    {
        if (me.Frames().length == 1)
        {
            var f = me.Frames()[me.Frames().length - 1];
            if (f.Refresh)
                f.Refresh();
        }
        else
        {
            me.CloseCurrentFrame();
        }
        
    }

    this.GetCurrentFrame = function ()
    {
        if (this.Frames().length > 0)
            return this.Frames()[this.Frames().length - 1];
    }

    this.IsLastFrame = function (frame)
    {
        return (this.Frames().length > 0) && (this.Frames()[this.Frames().length - 1] == frame);
    }

    this.NavigationItemClick = function (frame)
    {
        if (frame == me.Frames()[me.Frames().length - 1])
        {
            if (frame.Refresh)
                frame.Refresh();
            return;
        }


        //for (var i = 0; i < me.Frames().length; i++)
        //{
        //    var f = me.Frames()[i]
            
            

        //    if (f == frame)
        //    {
        //        var toClose = [];
        //        for (var k = me.Frames().length - 1; k > i; k--)
        //        {
        //            toClose.push(me.Frames()[k]);
        //        }

        //        for (var s = 0; s < toClose.length; s++)
        //        {
        //            me.CloseCurrentFrame();
        //        }

        //    }
        //}


        
    }

    ko.applyBindings(this, document.getElementById('navigationBar'));

};