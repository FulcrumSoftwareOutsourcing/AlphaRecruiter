'use strict';
function VLocalizationFrame()
{
    /// <field name='BusinessLayer' type='BGridWithExtraFind_for_View'/>

    VGridFrame.call(this);
    var me = this;

    var adtSearchTemplateId = 'ML/LangFilterTemplate';
    this.UsedTemplatesIds.push(adtSearchTemplateId);

    var adtSearchPanel;


    var selectedLangFilter;
    var isBinded = false;
 
    this.base_FrameDataLoaded1 = this.FrameDataLoaded;
    this.FrameDataLoaded = function (entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode)
    {
        HtmlProvider.AddTemplates(templates);
       
        this.base_FrameDataLoaded1(entityUsageId, attrsInSet, attrsByIds, vRows, templates, openMode);

        if (!isBinded)
        {
            var template = HtmlProvider.GetTemplateText(adtSearchTemplateId);
            template = template.replace('%id%', 'adt_search' + this.TempId);

            $('#filters_panel' + this.TempId).before (template);
            adtSearchPanel = $('#adt_search' + this.TempId)[0];

            $(adtSearchPanel).find('[c-type="langLbl"]').text(GetTxt('Language:'));
            
            $(adtSearchPanel).find('[c-type="lang-rs"]').on('click', this.OnLangFilterClick);

            $(document).on('click', this.OnDocumentClick);
            isBinded = true;
        }

        var crRoot = $(adtSearchPanel).find('[c-type="cr-root"]');
        var langItem = $(document.getElementById('LangFilterItem')).html();
        $(crRoot).children().off('click');
        $(crRoot).html('');
        if (!selectedLangFilter)
            selectedLangFilter = Metadata.APPLICATION$LANGUAGECODE.toUpperCase();
        for (var i = 0; i < Metadata.Languages.length; i++)
        {
            var item = Metadata.Languages[i];
            var langItemHtml = langItem.replace('<!--text-->', item.Name).replace('%lid%', item.LanguageCd);
            if (selectedLangFilter == item.LanguageCd)
            {
                langItemHtml = langItemHtml.replace('RsItem', 'RsItem selected');
            }
            $(crRoot).append(langItemHtml);
            if (selectedLangFilter == item.LanguageCd)
            {
                $(adtSearchPanel).find('[c-type="sel-lang"]').text(item.Name);
            }

        }

        $(crRoot).children().on('click', this.OnLangItemClick);
       
      
    };

    this.OnLangFilterClick = function (item)
    {
        var popup = $(adtSearchPanel).find('[c-type="cr-root"]');
        if ($(popup).css('display') == 'none')
        {
            $(popup).css('display', 'block');
            return false;
        }
        if ($(popup).css('display') == 'block')
        {
            $(popup).css('display', 'none');
            return false;
        }
        


    }


    this.base_Resize = this.Resize;
    this.Resize = function ()
    {
        this.base_Resize();

        var gh = $('#' + this.TempId + '_dataGridRoot').height();
        var fg = $(adtSearchPanel).height();

        $('#' + this.TempId + '_dataGridRoot').height(gh - fg);
        
    }

    this.OnLangItemClick = function (item)
    {
        var langCd = $(item.currentTarget).attr('l-id');
        for (var i = 0; i < Metadata.Languages.length; i++)
        {
            var it = Metadata.Languages[i];
            if (langCd == it.LanguageCd)
            {
                selectedLangFilter = langCd;
                //$(adtSearchPanel).find('[c-type="sel-lang"]').text(it.Name);
            }
        }

        me.BusinessLayer.OnLangFilterChanged(langCd);

    }

    this.OnDocumentClick = function (item)
    {
        var popup = $(adtSearchPanel).find('[c-type="cr-root"]');
        $(popup).css('display', 'none');
    }

    this.Dispose_base1 = this.Dispose;
    this.Dispose = function ()
    {
        this.Dispose_base1();

        $(adtSearchPanel).find('[c-type="lang-rs"]').off('click');
        var crRoot = $(adtSearchPanel).find('[c-type="cr-root"]');
        $(crRoot).children().off('click');
        $(document).off('click');
    };

  

}

extend(VLocalizationFrame, VGridFrame);
