'use strict';
function BSections()
{
    /// <field name='ViewLayer' type='VSections_ForBusiness'/>
    /// <field name='ParentLayer' type='BRoot_PublicForChildren'/>
    BusinessLayer.call(this);
    this.PublicForParent = new BSections_ForParent();
    this.PublicForView = new BSections_ForView();

    var sections = null;
    var selected;

    this.OnSectionsLoaded = function (sections)
    {
        sections = sections;
        this.ViewLayer.CreateSections(sections);
    };

    this.SelectedSectionChanged = function (section)
    {
        if (selected != section)
        {
            selected = section;
            this.ParentLayer.SelectedSectionChanged(section);
        }
    };

};

extend(BSections, BusinessLayer);

function BSections_ForParent()
{
    this.OnSectionsLoaded = function (sections){};
};

function BSections_ForView()
{
    this.SelectedSectionChanged = function (section) { };
};