function DRoot()
{
    DataLayer.call(this);
    this.PublicForBusiness = new DRoot_ForBusiness();

    this.GetStartObject = function (postData)
    {
        postData.WebRequestUrl = getStartObjectUrl;
        this.Request(postData);
    };

};
extend(DRoot, DataLayer);

function DRoot_ForBusiness()
{
    this.GetStartObject = function (postData) { };
    this.Request = function () { };
};