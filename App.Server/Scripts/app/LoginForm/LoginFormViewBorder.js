'use strict';
function LoginFormViewBorder()
{
    this.DialogDataLoaded = function (loginInfo, validator)
    {
        return {
            LoginInfo: new ObservableObject(loginInfo, ['UserName', 'Password', 'RememberMe'], validator),
                
            };
    };

    this.TryLogin = function (loginInfo, tokenDescr)
    {
        return [loginInfo.ToBusinessObject(), tokenDescr];
    };
};