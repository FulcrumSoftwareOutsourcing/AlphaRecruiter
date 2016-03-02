'use strict';
function VTree()
{
    
    ViewLayer.call(this);
    this.PublicForBusiness = new VTree_ForBusiness();
    this.PublicForParent = new VTree_ForParent();

    var isBinded = false;
    this.TreeItemsBySectionId = {All: ko.observableArray()};

    this.SelectedItem = null;

    var TreeMode = {};
    TreeMode.Pin = 'pin';
    TreeMode.Float = 'float';
    TreeMode.PinByUser = 'pin_by_u';
    
    var treeMode = TreeMode.Pin;
    var nodesElsIds = [];
   
    
    var me = this;

    this.CreateTreeItems = function (itemsObj)
    {
        this.TreeItemsBySectionId[itemsObj.SectionId] = itemsObj;
        this.TreeItemsBySectionId.All.push(itemsObj);
        AssignTreeEventsRecursive(itemsObj.TreeItems, this.OnExpandClicked, this.OnCollapseClicked, this.OnTreeNodeClicked,  this);

        if (isBinded == false)
        {
            $('#sectionCaption').click(AltMenuClick);
            //SetTreeMode(TreeMode.Pin);
            $('#treePinBtn').click(PinBtnClick);
       
            if (window.IsMobile)
                SetTreeMode(TreeMode.Float);
            else
                SetTreeMode(TreeMode.Pin);

            Resize();
            Resizer.AddResizable('sections', new Delegate(Resize, this));

            ko.applyBindings(this, document.getElementById('TreeHolder'));
            //    this.OnResize(AppSizes);

            $(window).click(function ()
            {
                if (treeMode == TreeMode.Float)
                    $('#left').fadeOut(120);
            })

           
            //for (var i = 0; i < nodesElsIds.length; i++)
            //{
            //    var el = document.getElementById(nodesElsIds[i]);
                
                
            //    //el.innerHTML = el.innerHTML.replace('<!--selection-->', '<div class="Selection" ></div>');
            //    //$('#' + nodesElsIds[i]).append('<div class="Selection" ></div>');
            //}

            isBinded = true;
            
        }
        itemsObj.TreeElement = document.getElementById('tree_' + itemsObj.SectionId);
       
        
    };
    
    
    this.TreeNodeCreating = function()
    {
        var id = App.Utils.ShortGuid();
        nodesElsIds.push(id);
        return id;
    }

    function AltMenuClick(event)
    {
        

        event.stopPropagation();
        if (treeMode == TreeMode.PinByUser || treeMode == TreeMode.Pin)
        {
            return;
        }

        var disp = $('#left').css('display');
        
        if (disp == 'none' )
        {
            $('#left').fadeIn(120);
            //$('#left').css('display', 'block');
        }
        else
        {
            
            $('#left').fadeOut(120);
            //$('#left').css('display', 'none');
        }

    }

    function PinBtnClick()
    {
        if (treeMode != TreeMode.Float)
        {
            SetTreeMode(TreeMode.PinByUser);
        }
        if (treeMode != TreeMode.PinByUser)
        {
            SetTreeMode(TreeMode.PinByUser);
        }

        var pos = $('#left').css('position');

        $('#left').css('position', 'absolute');
    }

    function Resize()
    {

        

        if (treeMode == TreeMode.PinByUser)
            return;

        var w = $(document).width();
        if (w > 1000)
        {
            SetTreeMode(TreeMode.Pin);
        }
        else
        {
            SetTreeMode(TreeMode.Float);
        }

        //$(menuContentEl).width(225);


    }

    function SetTreeMode(mode)
    {
        treeMode = mode;
        if (treeMode == TreeMode.PinByUser || treeMode == TreeMode.Pin)
        {
            $('#left').removeClass('LeftFloat');
            $('#left').addClass('LeftPin');
     //       $('#left').css ('z-index', '');
       //     $('#left').css('position', 'relative');
            $('#left').width(250);
            $('#left').show();
            
            $('#treeOpenArr').css('display', 'none');
            //$('#altMenuBtn').css('display', 'none');
            $('#backLbl').show();
        }
        if (treeMode == TreeMode.Float)
        {
            $('#left').addClass('LeftFloat');
            //$('#left').css('z-index', 9999999);
            //$('#left').css('position', 'absolute');
            $('#left').hide();
            //$('#left').width(0);
            $('#navigationBar').css('display', 'none');
            $('#treeOpenArr').css('display', 'inline-block');
            //$('#altMenuBtn').css('display', 'inline-block');
            $('#navigationBar').css('display', 'inline-block');
            $('#backLbl').hide();
            

        }
    }

    function AssignTreeEventsRecursive(treeItems, onExpandClicked, onCollapseClicked, onTreeNodeClicked,  context)
    {
        for (var i = 0; i < treeItems.length; i++)
        {
            var item = treeItems[i];
            item.OnExpandClicked = onExpandClicked;
            item.OnCollapseClicked = onCollapseClicked;
            item.OnTreeNodeClicked = onTreeNodeClicked;
            
            item.Context = context;
            AssignTreeEventsRecursive(item.TreeItems, onExpandClicked, onCollapseClicked, onTreeNodeClicked, context);
        }
    };

    this.IsTreeItemsCreatedForSection = function (sectionId)
    {
        if (this.TreeItemsBySectionId[sectionId])
            return true;
        else
            return false;
    };

    this.ShowTreeFor = function (sectionId)
    {
        this.SelectedItem = null;

        for (var i = 0; i < this.TreeItemsBySectionId.All().length; i++)
        {
            $(this.TreeItemsBySectionId.All()[i].TreeElement).hide();
            $(this.TreeItemsBySectionId.All()[i].CaptionElement).hide();
        }

        var treeObj = this.TreeItemsBySectionId[sectionId];
        //$('#treeCaption').text(treeObj.SectionText);
        $(treeObj.TreeElement).show();
      
        this.SelectedItem = FindSelectedRecursive(treeObj.ItemsList);

        
        if (!this.SelectedItem && treeObj.ItemsList.length > 0)
        {
            treeObj.ItemsList[0].Selected(true);
            this.SelectedItem = treeObj.ItemsList[0];
        }
        if (this.SelectedItem)
            this.OnSelectedItemChanged(this.SelectedItem);
        else
            this.OnSelectedItemChanged(null);

        $('#tree_' + sectionId).height($('#tree_' + sectionId).height() + 86);

        
    };

 
    function FindSelectedRecursive( vItems )
    {
        for (var i = 0; i < vItems.length; i++)
        {
            if (vItems[i].Selected())
            {
                return vItems[i];
            }
            FindSelectedRecursive(vItems[i].TreeItems);
        }
        return null;
    };


    this.OnExpandClicked = function (item)
    {
        item.Expanded(true);
    };

    this.OnCollapseClicked = function (item)
    {
        item.Expanded(false);
    };

    var lastClick = new Date();

    var waitingTreeItem;
    this.OnTreeNodeClicked = function (item)
    {
        if (item.IsRootLevel)
        {
            item.Expanded(!item.Expanded());
        }
        else
            item.Expanded(true);
        

        if (item.IsRootLevel)
        {
            return;
        }

        //var now = new Date();
        //if ((now - lastClick) <= 400)
        //{
            
        //}
        //lastClick = now;

        if (treeMode == TreeMode.Float)
        {
            $('#left').fadeOut(120);
        }

     //   if (item.Selected())
       //     return;

        if (Navigation.CheckCurrentHasChanges())
        {
            waitingTreeItem = item;
            ShowMessage([GetTxt('Frame has changes.'), GetTxt('Do you want to save changes?')], GetTxt('Question'), [DialogButtons.Yes, DialogButtons.No, DialogButtons.Cancel], NxMessageBoxIcon.Question, new Delegate(ContinueAfterSaveDialog, this));
        }
        else
        {
            TreeNodeClicked(item);
        }
    };

    function TreeNodeClicked(item)
    {
        item.Context.SelectedItem.Selected(false)
        item.Selected(true);
        item.Context.SelectedItem = item;
        Settings.AddChangedSettings('app', 'tree_sel_item_' + item.SectionId, item.BItem.Id);
        item.Context.OnSelectedItemChanged.call(item.Context, item);
    }

    function ContinueAfterSaveDialog(dlgResult)
    {
        if (dlgResult == 'yes')
        {
            Navigation.GetCurrentFrame().SaveChanges(new Delegate(ContinueAfterSaveDone, this));
        }
        if (dlgResult == 'no')
        {
            TreeNodeClicked(waitingTreeItem);
            waitingTreeItem = null;
        }
        if (dlgResult == 'cancel')
        {
            waitingTreeItem = null;
        }
    }

    function ContinueAfterSaveDone()
    {
        TreeNodeClicked(waitingTreeItem);
        waitingTreeItem = null;
    }

    this.OnSelectedItemChanged = function (item)
    {
        Navigation.SelectedTreeItemChanged(item);
        if(item)
            this.BusinessLayer.OnSelectedItemChanged(item.BItem);
    };

    this.ClearTree = function ()
    {
        var isBinded = false;
        //var holderElement = document.getElementById('TreeHolder');
        this.TreeItemsBySectionId.All.removeAll();
        var newTreesObj = { All: this.TreeItemsBySectionId.All };
        this.TreeItemsBySectionId = newTreesObj;
        //ko.cleanNode(holderElement);
        
        //this.TreeItemsBySectionId.All.destroyAll();
        //this.TreeItemsBySectionId = { All: ko.observableArray() };
        this.SelectedItem = null;
        
    };

};

extend(VTree, ViewLayer);

function VTree_ForBusiness()
{
    this.CreateTreeItems = function (itemsObj) { };
    this.IsTreeItemsCreated = function (sectionId) { };
    this.ShowTreeFor = function (sectionId) { };
    this.ClearTree = function () { };
};

function VTree_ForParent()
{
    this.IsTreeItemsCreatedForSection = function (sectionId){};
};
