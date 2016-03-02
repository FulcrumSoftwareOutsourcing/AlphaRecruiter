function TreeViewBorder()
{
    this.CreateTreeItems = function (treeItems, sectionId)
    {
        var treeObj = {ItemsList: []};
        var vItems = [];
        var sel = window.Settings.GetSettings('app', 'tree_sel_item_' + sectionId);
        CreateViewTreeItemsRecursive(treeItems, vItems, treeObj, true, sel, sectionId);
        
        treeObj.TreeItems = vItems;
        treeObj.SectionId = sectionId;
        treeObj.SectionText = Metadata.GetSection(sectionId).Text;

        return treeObj;
    };

    function CreateViewTreeItemsRecursive(bItems, vItems, containerObj, isRootLevel, sel, sectionId)
    {
        for (var i = 0; i < bItems.length; i++)
        {
            var vItem = CreateViewTreeItem(bItems[i], isRootLevel, sel);
            vItem.SectionId = sectionId;
            containerObj.ItemsList.push(vItem);
            vItems.push(vItem);
            CreateViewTreeItemsRecursive(bItems[i].TreeItems, vItem.TreeItems, containerObj, false, sel, sectionId);
        }
    };


   

    function CreateViewTreeItem(bItem, isRootLevel, sel)
    {
        var vItem = new ObservableObject(bItem, ['Id', 'Text', 'Visible']);
        var isSelectedByDefault = bItem.IsDefault;
        vItem.BItem = bItem;
        vItem.Selected = ko.observable(false);
        if (!sel && isSelectedByDefault)
            vItem.Selected = ko.observable(true);
        if (sel && bItem.Id.toUpperCase() == sel.toUpperCase())
            vItem.Selected = ko.observable(true);


        vItem.TreeItems = [];
        vItem.Expanded = ko.observable(true /*bItem.Expanded*/);
        if (isRootLevel)
            vItem.IsRootLevel = true;
        else
            vItem.IsRootLevel = false;
        //vItem.SelectionHtml = '<div class="Selection" ></div>';

        var imageMeta = Metadata.GetImage(bItem.ImageId);
        if (imageMeta && imageMeta.Folder && imageMeta.FileName)
            vItem.ImageUrl = imagesFolder + imageMeta.Folder + '/' + imageMeta.FileName;
        else
            vItem.ImageUrl = null;

        vItem.OnExpandClicked = function (item){};

        vItem.OnCollapseClicked = function (item) { };

        vItem.OnTreeNodeClicked = function (item) { };

       

        return vItem;



    };

};