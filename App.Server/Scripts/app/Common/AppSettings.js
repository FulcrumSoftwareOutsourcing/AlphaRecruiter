'use strict';

function AppSettings()
{
    var settingsToSave = null;
    var appSettings = {};

    this.AddChangedSettings = function (groupKey, key, value)
    {
        if (!settingsToSave)
            settingsToSave = {};

        if (!settingsToSave[groupKey])
        {
            settingsToSave[groupKey] = {};
        }

        var val = value;
        if (valueHandlers['ToSettings_' + key])
        {
            val = valueHandlers['ToSettings_' + key](value);
        }

        settingsToSave[groupKey][key] = val;

        if (!appSettings[groupKey])
        {
            appSettings[groupKey] = {};
        }

        appSettings[groupKey][key] = JSON.stringify( val );
    }

    this.SettingsLoaded = function (settings)
    {


        for (var i = 0; i < settings.length; i++)
        {
            var item = settings[i];
            if (!appSettings[item.Group])
                appSettings[item.Group] = {};


            var val = item.Value;
            if (valueHandlers['ToSettings_' + item.Key])
            {
                val = valueHandlers['ToSettings_' + item.Key](item.Value);
            }

            appSettings[item.Group][item.Key] = val;
        }
    }

    this.GetSettings = function (groupKey, key)
    {
        var group = appSettings[groupKey];
        if (!group)
        {
            if (valueHandlers['FromSettings_' + key])
                return valueHandlers['FromSettings_' + key](groupKey, undefined);
            else
                return undefined;
        }

        if (valueHandlers['FromSettings_' + key])
            return valueHandlers['FromSettings_' + key](groupKey, group[key]);
        else
            return group[key];
    }

    this.GetSettingsToSave = function ()
    {
        var toSave = settingsToSave;
        settingsToSave = null;
        return toSave;
    }

    var valueHandlers =
        {
            ToSettings_page_size: function(value)
            {
                return value;
            },

            FromSettings_page_size: function(groupKey, value)
            {
                var m_PageSize = 50;
                try
                {
                    m_PageSize = value;

                    if (m_PageSize != 25 &&
                       m_PageSize != 50 &&
                       m_PageSize != 100 &&
                       m_PageSize != 200 &&
                       m_PageSize != 500)
                    {
                        m_PageSize = 50;
                        window.Settings.AddChangedSettings(groupKey, 'page_size', m_PageSize);

                    }

                    if (App.Utils.IsIE() && m_PageSize == 500)
                    {
                        m_PageSize = 200;
                        Settings.AddChangedSettings(groupKey, 'page_size', m_PageSize);
                    }
                }
                catch(error)
                {
                    m_PageSize = 50;
                }


                return m_PageSize;
            }

        }

}

    


    




