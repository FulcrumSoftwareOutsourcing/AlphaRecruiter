'use strict';
function BTree()
{
    /// <field name='ViewLayer' type='VTree_ForBusiness'/>
    BusinessLayer.call(this);
    this.PublicForParent = new BTree_ForParent();
    this.PublicForView = new BTree_ForView();
    var treeItemsBySectionId = {};

    
    this.CreateTreeItems = function (treeItems, sectionId)
    {
        treeItemsBySectionId[sectionId] = treeItems;
        this.ViewLayer.CreateTreeItems(treeItems, sectionId);
    };


    this.ShowTreeFor = function (sectionId)
    {
        this.ViewLayer.ShowTreeFor(sectionId);
    };

    this.ClearTree = function ()
    {
        treeItemsBySectionId = {};
        this.ViewLayer.ClearTree();
    };

    this.OnSelectedItemChanged = function (item)
    {
        this.ParentLayer.OnSelectedTreeItemChanged(item);
    };
};

extend(BTree, BusinessLayer);

function BTree_ForParent()
{
    this.IsTreeItemsCreated = function (sectionId) { };
    this.CreateTreeItems = function (treeItems, sectionId) { };
    this.ShowTreeFor = function (sectionId) { };
    this.ClearTree = function () { };
};

function BTree_ForView()
{
    this.OnSelectedItemChanged = function (item) { };
};

