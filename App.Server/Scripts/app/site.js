
'use strict';
var bRoot;

var fullScreenWaiter;
var Settings = new AppSettings();

ko.bindingHandlers.numeric = {
    init: function (element, valueAccessor)
    {
        var value = valueAccessor();
        $(element).val(value());
        $(element).on("keydown", function (event)
        {

            if (event.char && (event.char == 'ю' || event.char == 'Ю'))
            {
                event.preventDefault();
                return;
            }

            // Allow: backspace, delete, tab, escape, and enter
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: . ,
                (event.keyCode == 188 || event.keyCode == 190 || event.keyCode == 110) ||
                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39) )
                {
                // let it happen, don't do anything
                return;
            }
            else
            {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105))
                {
                    event.preventDefault();
                }
            }
        });

        $(element).change(function ()
        {
            value($(element).val());
        });
    }
};




$(function ()
{
    window.pingTimerId = null;

    window.IsMobile = App.Utils.IsMobile();
    var multilang = new Multilang();
    window.GetTxt = Metadata.GetTxt;

    
    

    window.HtmlProvider = new HtmlProvider();

    window.Waiting = new WaitDispatcher();
    window.Waiting.AddFullScreenWaiter(ShowFullScreenWaiter, HideFullScreenWaiter, this);

    window.Navigation = new VNavigationBar();

    fullScreenWaiter = new BusyIndicator('WaiterImg', 150, '#FFFFFF');
    

    //window.ResizeDispatcher = new ResizeDispatcher();
    window.Resizer = new Resizer();

    bRoot = new BRoot();
    bRoot.ViewLayer = new VRoot();
    bRoot.DataLayer = new DRoot();
    bRoot.ViewBorder = new RootViewBorder();
    bRoot.DataBorder = new RootDataBorder();
    
    var bLoginPanel = new BLoginPanel();
    bLoginPanel.ViewLayer = new VLoginPanel();
    bRoot.AddChild(bLoginPanel, "LoginPanel");

    var bPopup = new BPopup();
    bPopup.ViewLayer = new VPopup();
    bRoot.AddChild(bPopup, "Popup");
    window.VPopup = bPopup.ViewLayer;

    var bLoginForm = new BLoginForm();
    bLoginForm.ViewLayer = new VLoginForm();
    bLoginForm.DataLayer = new DLoginForm();
    bLoginForm.ViewBorder = new LoginFormViewBorder();
    bRoot.AddChild(bLoginForm, 'LoginForm');
    var loginView = bLoginForm.ViewLayer
    window.ShowLoginForm = function (context, postData, token)
    {
        App.Utils.AddRequestVerificationToken(token);
        loginView.Show.apply(loginView, [context, postData]);
    }; 

    var bMsgBox = new BMsgBox();
    bMsgBox.ViewLayer = new VMsgBox();
    var msgBoxView = bMsgBox.ViewLayer;
    bRoot.AddChild(bMsgBox, 'MessageBox');
    window.ShowMessage = function (text, caption, buttons, icon, resultDelegate)
    {
        msgBoxView.Show.apply(msgBoxView, [text, caption, buttons, icon, resultDelegate]);
    };


    var bSections = new BSections();
    bSections.ViewLayer = new VSections();
    bSections.ViewBorder = new SectionsViewBorder();
    bRoot.AddChild(bSections, "Sections");

    var bTree = new BTree();
    bTree.ViewLayer = new VTree();
    bTree.ViewBorder = new TreeViewBorder();
    bRoot.AddChild(bTree, "Tree");

    var bWorspacePnl = new BWorkspacePnl();
    bWorspacePnl.ViewLayer = new VWorkspacePnl();
    bRoot.AddChild(bWorspacePnl, 'Workspaces');

    

    var bFrame = new BFramesRoot();
    bFrame.ViewLayer = new VFramesRoot();
    bRoot.AddChild(bFrame, "Frame");

    window.OpenFrame = function (frame)
    {
        bFrame.OpenFrame.call(bFrame, frame);
    };

 
        

    bRoot.ConnectLayers();

    if (IsMobile)
    {
        $('#right').css('overflow-x', 'auto');
        $('#right').css('overflow-y', 'auto');
    }


    bRoot.Run();
   
  

});


var rtime = new Date(1, 1, 2000, 12, 0, 0);
var timeout = false;
var delta = 150;
function OnClientResized ()
{
    rtime = new Date();
    if (timeout === false)
    {
        timeout = true;
        setTimeout(ResizeEnd, delta);
    }
}

function ResizeEnd()
{
    if (new Date() - rtime < delta)
    {
        setTimeout(ResizeEnd, delta);
    } else
    {
        timeout = false;

        AppSizes.ClientHeight = GetClientHeight();
        AppSizes.ClientWidth = GetClientWidth();

        if (bRoot)
            bRoot.ViewLayer.OnResize();
    }
}



function extend(Child, Parent)
{
    var F = function () { };
    F.prototype = Parent.prototype;
    Child.prototype = new F();
    Child.prototype.constructor = Child;
    Child.superclass = Parent.prototype;
};

function ShowFullScreenWaiter()
{
    //if (animati)
    //{
        //$('#WaiterOver').show();
       // $('#WaiterImg').show();
        fullScreenWaiter.Show();
        $('#WaiterImg').fadeTo(1, 0);
        $('#WaiterOver').fadeTo(3000, 0.7);
        $('#WaiterImg').fadeTo(3000, 1);
    //}
    //else
    //{
    //    $('#WaiterOver').show();
    //    $('#WaiterImg').show();
    //    $('#WaiterOver').attr('class', 'WaiterOver fade-in');
    //    $('#WaiterImg').attr('class', 'WaiterImg fade-in-img');
    //}

};

function HideFullScreenWaiter()
{
    //$('#WaiterOver').attr('class', 'WaiterOver');
    //$('#WaiterImg').attr('class', 'WaiterImg');
    $('#WaiterOver').fadeTo(1, 0);
    $('#WaiterImg').fadeTo(1, 0);
    fullScreenWaiter.Hide();
    $('#WaiterOver').hide();
    $('#WaiterImg').hide();

    
};











window.PressedKey = null;
$(document).keydown(function (e)
{
   
    switch (e.keyCode)
    {
        
        case 16:
            window.PressedKey = 'shift';
            break;
        case 38:
            window.PressedKey = 'up';
            break;
        case 40:
            window.PressedKey = 'down';
            break;
        case 13:
            window.PressedKey = 'enter';
            break;
        case 17:
            window.PressedKey = 'control';
            break;
        default:
            window.PressedKey = 'other';

    }

});

$(document).keyup(function (e)
{
    window.PressedKey = null;
});


function Resizer()
{
    var delta = 150;
    var resizables = [];

    var OnClientResized = function ()
    {
        ResizeEnd();
        //rtime = new Date();
        //if (timeout === false)
        //{
        //    timeout = true;
        //    setTimeout(ResizeEnd, delta);
        //}
    }

    if (typeof window.addEventListener !== 'undefined')
        window.addEventListener('resize', OnClientResized);
    else if (typeof window.attachEvent !== 'undefined')
        window.attachEvent('onresize', OnClientResized);

 



    function ResizeEnd()
    {
        //if (new Date() - rtime < delta)
        //{
        //    setTimeout(ResizeEnd, delta);
        //} else
        //{
        timeout = false;

        var clW = GetClientWidth();
        var clH = GetClientHeight();
            
        var tp = $('#topPanel').height();
        var nb = 0;//$('#navigationBar').height();
        var tc = $('#treeCaption').height();
        var sh = $('#SectionsHolder').height();
        var l = 0;//$('#fwLink').height();
        // $('#TreeHolder').height(clH - tp - nb - tc - sh - l );

            
            
        var fc = $('#frameCaption').height();

        var child = $('#FrameHolder').children();
        for (var i = 0; i < child.length; i++)
        {
            var item = child[i];
            if ($(item).css('display') == 'block' && $(item).attr('c-type') == 'grid')
            {
                $('#FrameHolder').css('height', '100%');
            }
            if ($(item).css('display') == 'block' && $(item).attr('c-type') == 'auto-layout')
            {
                $('#FrameHolder').height(clH - tp - nb - fc - 50);
            }
        }

        //if ($('#FrameHolder').first().attr('c-type') == 'grid')
        //{
          //  $('#FrameHolder').css('height', '100%');
            //$('#FrameHolder').height(clH - tp - nb - fc - 0);
        //}



            var s = $('#SectionsHolder').width();
            var sp = $('#splitter').width();
            $('#FrameHolder').width(clW );
            

           
            for (var i = 0; i < resizables.length; i++)
            {
                if (resizables[i])
                {
                    resizables[i].Invoke();
                }
            }

           
           


           

       // }
    };

    this.AddResizable = function (name, resizeFinishedDelegate)
    {

        resizables.push(resizeFinishedDelegate);
    }

    this.RemoveResizable = function (context)
    {
        for (var i = 0; i < resizables.length; i++)
        {
            if (resizables[i] && resizables[i].IsTheSameContext(context))
            {
                resizables.splice(i, 1) ;
            }
        }
    }

    this.ResizeRequest = function (resizeFinishedDelegate)
    {
        ResizeEnd();
        if(resizeFinishedDelegate)
            resizeFinishedDelegate.Invoke();
    };


};


function DownloadLink ()
{
   
};







