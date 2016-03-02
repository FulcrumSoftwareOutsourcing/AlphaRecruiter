'use strict';
function VGridWithExtraFind()
{
    /// <field name='BusinessLayer' type='BGridWithExtraFind_for_View'/>

    VGridFrame.call(this);
    var me = this;

    var adtSearchTemplateId = 'HR/AdtSearchTemplate';
    this.UsedTemplatesIds.push(adtSearchTemplateId);
    var isBinded = false;
    var adtSearchPanel;
    var wasAdtSearch = false;

    var waiter;

    this.base_FrameDataLoaded1 = this.FrameDataLoaded;
    this.FrameDataLoaded = function (entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode)
    {
        this.base_FrameDataLoaded1(entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode);

        if (!isBinded)
        {
            var template = HtmlProvider.GetTemplateText(adtSearchTemplateId);
            template = template.replace('%id%', 'adt_search' + this.TempId).replace('<!--btn text-->', GetTxt('Reset'));
            $('#filters_panel' + this.TempId).after(template);
            adtSearchPanel = $('#adt_search' + this.TempId)[0];


            $(adtSearchPanel).find('button').on('click', this.AdtResetClick);
            $(adtSearchPanel).find('input').on('keyup', this.AdtFindChanged);

            waiter = new BusyIndicator('AdtSearchWorkIndicator', 16, '#FFFFFF');

            isBinded = true;
        }

        if (wasAdtSearch)
        {
            var input = $(adtSearchPanel).find('input');
            var strLength = input.val().length * 2 ;

            input[0].setSelectionRange(strLength, strLength);

            wasAdtSearch = false;
        }
    };

    this.AdtResetClick = function ()
    {
        $(adtSearchPanel).find('input').val('');
        wasAdtSearch = false;
        me.BusinessLayer.AdtFindChanged('');
    };

    this.AdtFindChanged = function ()
    {
        var val = $(adtSearchPanel).find('input').val();
        wasAdtSearch = true;
        me.BusinessLayer.AdtFindChanged(val);
    };

    this.Dispose_base1 = this.Dispose;
    this.Dispose = function ()
    {
        this.Dispose_base1();

        $(adtSearchPanel).find('button').off('click', this.AdtResetClick);
        $(adtSearchPanel).find('input').off('keyup', this.AdtFindChanged);
    };

    window.Waiting.AddIndividualWaiter('AdtSearch', this.ShowWaiter, this.HideWaiter, this);

    this.ShowWaiter = function ()
    {
        waiter.Show();
    };

    this.HideWaiter = function ()
    {
        waiter.Hide();
    };

}

extend(VGridWithExtraFind, VGridFrame);
