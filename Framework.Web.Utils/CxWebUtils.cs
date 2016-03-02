/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Framework.Utils;

namespace Framework.Web.Utils
{
	/// <summary>
	/// Static utility methods for work with Web applications, forms, controls, etc.
	/// </summary>
	public class CxWebUtils
	{

        public static System.Web.HttpContext HttpContextStatic;
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if control is enabled and is not readonly.
    /// </summary>
    static public bool IsControlEnabled(Control c)
    {
      if (c == null)
      {
        return false;
      }
      PropertyInfo enabledProp = c.GetType().GetProperty("Enabled", typeof(bool));
      if (enabledProp != null)
      {
        if (!((bool)(enabledProp.GetValue(c, null))))
        {
          return false;
        }
      }
      PropertyInfo readOnlyProp = c.GetType().GetProperty("ReadOnly", typeof(bool));
      if (readOnlyProp != null)
      {
        if ((bool)(readOnlyProp.GetValue(c, null)))
        {
          return false;
        }
      }
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified type among all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="controlTypes">control type to find</param>
    /// <returns>found control</returns>
    static public Control FindVisibleEnabledControlByTypes(Control parent, Type[] controlTypes)
    {
      if (parent != null)
      {
        foreach (Type controlType in controlTypes)
        {
          if (controlType.IsInstanceOfType(parent) && parent.Visible && IsControlEnabled(parent))
          {
            return parent;
          }
        }
        foreach (Control c in parent.Controls)
        {
          Control result = FindVisibleEnabledControlByTypes(c, controlTypes);
          if (result != null)
          {
            return result;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified type among all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="controlTypes">control type to find</param>
    /// <returns>found control</returns>
    static public Control FindVisibleControlByTypes(Control parent, Type[] controlTypes)
    {
      if (parent != null)
      {
        foreach (Type controlType in controlTypes)
        {
          if (controlType.IsInstanceOfType(parent) && parent.Visible)
          {
            return parent;
          }
        }
        foreach (Control c in parent.Controls)
        {
          Control result = FindVisibleControlByTypes(c, controlTypes);
          if (result != null)
          {
            return result;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified type among all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="controlTypes">control type to find</param>
    /// <returns>found control</returns>
    static public Control FindControlByTypes(Control parent, Type[] controlTypes)
    {
      if (parent != null)
      {
        foreach (Type controlType in controlTypes)
        {
          if (controlType.IsInstanceOfType(parent))
          {
            return parent;
          }
        }
        foreach (Control c in parent.Controls)
        {
          Control result = FindControlByTypes(c, controlTypes);
          if (result != null)
          {
            return result;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified type among all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="controlType">control type to find</param>
    /// <returns>found control</returns>
    static public Control FindControlByType(Control parent, Type controlType)
    {
      return FindControlByTypes(parent, new Type[]{controlType});
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified ID all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="id">control ID to find</param>
    /// <returns>found control</returns>
    static public Control FindControlById(Control parent, string id, bool caseInsensitive)
    {
      if (parent != null && CxUtils.NotEmpty(id))
      {
        if (caseInsensitive && CxText.Equals(parent.ID, id))
        {
          return parent;
        }
        else if (parent.ID == id)
        {
          return parent;
        }
        foreach (Control c in parent.Controls)
        {
          Control result = FindControlById(c, id, caseInsensitive);
          if (result != null)
          {
            return result;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified ID all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="id">control ID to find</param>
    /// <returns>found control</returns>
    static public Control FindControlById(Control parent, string id)
    {
      return FindControlById(parent, id, false);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Searches control with the specified UniqueID through all child controls 
    /// of the given parent control.
    /// </summary>
    /// <param name="parent">control to start search from</param>
    /// <param name="uniqueId">control ID to find</param>
    /// <returns>found control</returns>
    static public Control FindControlByUniqueId(Control parent, string uniqueId)
    {
      if (parent != null)
      {
        if (parent.UniqueID == uniqueId)
        {
          return parent;
        }
        foreach (Control c in parent.Controls)
        {
          Control result = FindControlByUniqueId(c, uniqueId);
          if (result != null)
          {
            return result;
          }
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes a URL from two parts - appends second part to the first part.
    /// Checks presence of slash.
    /// </summary>
    /// <param name="baseUrl">first URL part</param>
    /// <param name="additionalUrl">second URL part</param>
    /// <returns>composed URL</returns>
    static public string CombineUrls(string baseUrl, string additionalUrl)
    {
      if (baseUrl.EndsWith("/"))
      {
        baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
      }
      if (additionalUrl.StartsWith("/"))
      {
        additionalUrl = additionalUrl.Substring(1);
      }
      return baseUrl + "/" + additionalUrl;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Composes a URL from parts concatenates all parts, divides parts with slash.
    /// </summary>
    /// <param name="urls">urls to concatenate</param>
    /// <returns>concatenated URL</returns>
    static public string CombineUrls(params string[] urls)
    {
      string url = "";
      foreach (string part in urls)
      {
        string p = part;
        if (CxUtils.NotEmpty(p))
        {
          if (p.StartsWith("/"))
          {
            p = p.Substring(1);
          }
          if (p.EndsWith("/"))
          {
            p = p.Substring(0, p.Length - 1);
          }
          url = url + (url.Length > 0 ? "/" : "") + p;
        }
      }
      return url;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and returns parent of the given control with the given type.
    /// </summary>
    /// <param name="control">control to start search from</param>
    /// <param name="parentType">parent control type to find</param>
    /// <returns>found control or null</returns>
    static public Control FindParentByType(Control control, Type parentType)
    {
      while (control != null)
      {
        if (parentType.IsInstanceOfType(control))
        {
          return control;
        }
        control = control.Parent;
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds and returns parent of the given control with the given type.
    /// </summary>
    /// <param name="control">control to start search from</param>
    /// <param name="parentType">parent control type to find</param>
    /// <returns>found control or null</returns>
    static public Control FindLastParentByType(Control control, Type parentType)
    {
      Control parent = null;
      while (control != null)
      {
        if (parentType.IsInstanceOfType(control))
        {
          parent = control;
        }
        control = control.Parent;
      }
      return parent;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a value from the Attributes collection.
    /// </summary>
    /// <param name="control">control to get value from</param>
    /// <param name="attrName">attribute name</param>
    /// <returns>attribute value</returns>
    static public string GetControlAttr(Control control, string attrName)
    {
      if (control is WebControl)
      {
        return ((WebControl)control).Attributes[attrName];
      }
      else if (control is HtmlControl)
      {
        return ((HtmlControl)control).Attributes[attrName];
      }
      else if (control is UserControl)
      {
        return ((UserControl)control).Attributes[attrName];
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets a value of the attribute from the Attributes collection.
    /// </summary>
    /// <param name="control">control to set value to</param>
    /// <param name="attrName">attribute name</param>
    /// <param name="attrValue">attribute value</param>
    static public void SetControlAttr(Control control, string attrName, string attrValue)
    {
      if (control is WebControl)
      {
        ((WebControl)control).Attributes[attrName] = attrValue;
      }
      else if (control is HtmlControl)
      {
        ((HtmlControl)control).Attributes[attrName] = attrValue;
      }
      else if (control is UserControl)
      {
        ((UserControl)control).Attributes[attrName] = attrValue;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets a CssClass for the given control.
    /// </summary>
    /// <param name="control">control to set value to</param>
    /// <param name="cssClass">css class value to set</param>
    static public void SetCssClass(Control control, string cssClass)
    {
      if (control is WebControl)
      {
        ((WebControl)control).CssClass = cssClass;
      }
      else if (control is HtmlControl)
      {
        ((HtmlControl)control).Attributes["class"] = cssClass;
      }
      else if (control is UserControl)
      {
        ((UserControl)control).Attributes["class"] = cssClass;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets a CssClass from the given control.
    /// </summary>
    /// <param name="control">control to get CSS class from</param>
    static public string GetCssClass(Control control)
    {
      if (control is WebControl)
      {
        return ((WebControl)control).CssClass;
      }
      else if (control is HtmlControl)
      {
        return ((HtmlControl)control).Attributes["class"];
      }
      else if (control is UserControl)
      {
        return ((UserControl)control).Attributes["class"];
      }
      return "";
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets a CssClass for the given control, if CSS class was not set before.
    /// </summary>
    /// <param name="control">control to set value to</param>
    /// <param name="cssClass">css class value to set</param>
    static public void SetCssClassIfEmpty(Control control, string cssClass)
    {
      if (CxUtils.IsEmpty(GetCssClass(control)))
      {
        SetCssClass(control, cssClass);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns call to the confirmation JavaScript function.
    /// Confirmation function displays confirmation dialog with the
    /// given confirmation text, if user asks 'yes', processes given 
    /// original URL.
    /// Utils.js file should be included to the page scripts.
    /// </summary>
    /// <param name="originalUrl">URL to process after confirmation</param>
    /// <param name="confirmationText">confirmation text to display</param>
    /// <returns>composed URL</returns>
    static public string GetConfirmFunction(string originalUrl, string confirmationText)
    {
      if (CxUtils.NotEmpty(originalUrl) && CxUtils.NotEmpty(confirmationText))
      {
        originalUrl = originalUrl.Replace("\"", "'").Replace("'", "\\'");
        confirmationText = confirmationText.Replace("\"", "'").Replace("'", "\\'");
        return "confirmUrl('" + originalUrl + "', '" + confirmationText + "')";
      }
      return originalUrl;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns URL to the confirmation JavaScript function.
    /// Confirmation function displays confirmation dialog with the
    /// given confirmation text, if user asks 'yes', processes given 
    /// original URL.
    /// Utils.js file should be included to the page scripts.
    /// </summary>
    /// <param name="originalUrl">URL to process after confirmation</param>
    /// <param name="confirmationText">confirmation text to display</param>
    /// <returns>composed URL</returns>
    static public string GetConfirmUrl(string originalUrl, string confirmationText)
    {
      if (CxUtils.NotEmpty(originalUrl) && CxUtils.NotEmpty(confirmationText))
      {
        originalUrl = originalUrl.Replace("\"", "'").Replace("'", "\\'");
        confirmationText = confirmationText.Replace("\"", "'").Replace("'", "\\'");
        return "javascript:confirmUrl('" + originalUrl + "', '" + confirmationText + "')";
      }
      return originalUrl;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Makes absolute path from the relative path.
    /// </summary>
    /// <param name="relUrl">relative path</param>
    /// <returns>absolute path</returns>
    static public string GetAbsUrl(string relUrl)
    {
      return CombineUrls(HttpContext.Current.Request.ApplicationPath, relUrl);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Enforces creation and load of all children of the given control.
    /// Recurrently executes EnsureChildControls method 
    /// for the given control and all it's children.
    /// </summary>
    static public void EnsureChildControls(Control control)
    {
      if (control != null)
      {
        MethodInfo method = control.GetType().GetMethod(
          "EnsureChildControls", 
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method != null)
        {
          method.Invoke(control, new object[]{});
        }
        foreach (Control child in control.Controls)
        {
          EnsureChildControls(child);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Encodes text to display correctly all HTML-specific characters.
    /// Replaces line breaks \r\n, etc. with <BR> tag.
    /// </summary>
    static public string PrepareMultilineText(string text)
    {
      return HttpUtility.HtmlEncode(CxUtils.Nvl(text)).
        Replace("\r\n", "<br>").
        Replace("\n\r", "<br>").
        Replace("\r", "<br>").
        Replace("\n", "<br>");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns given control and all it's child controls.
    /// </summary>
    /// <param name="parent">control to return children of</param>
    static public IList GetAllControls(Control parent)
    {
      ArrayList list = new ArrayList();
      if (parent != null)
      {
        list.Add(parent);
        list.AddRange(GetChildControls(parent));
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns all child controls of the given parent.
    /// </summary>
    /// <param name="parent">control to return children of</param>
    static public IList GetChildControls(Control parent)
    {
      ArrayList list = new ArrayList();
      if (parent != null)
      {
        foreach (Control child in parent.Controls)
        {
          list.AddRange(GetAllControls(child));
        }
      }
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Appends CSS class to the web control.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="cssClass"></param>
    static public void AppendCssClass(WebControl control, string cssClass)
    {
      if (control != null && CxUtils.NotEmpty(cssClass))
      {
        control.CssClass += (CxUtils.NotEmpty(control.CssClass) ? " " : "") + cssClass;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets width to the given control.
    /// </summary>
    /// <param name="control">control to set width to</param>
    /// <param name="width">width to set</param>
    static public void SetWidth(Control control, Unit width)
    {
      if (control is WebControl)
      {
        ((WebControl)control).Width = width;
      }
      else if (control is HtmlControl)
      {
        ((HtmlControl)control).Attributes["width"] = width.ToString();
      }
      else if (control is UserControl)
      {
        ((UserControl)control).Attributes["width"] = width.ToString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Initializes blob file object from the posted file.
    /// </summary>
    /// <param name="blobFile">blob file to initialize</param>
    /// <param name="postedFile">posted file to use as a source</param>
    static public void InitBlobFile(
      CxBlobFile blobFile, 
      HttpPostedFile postedFile)
    {
      if (blobFile != null && postedFile != null)
      {
        CxBlobFileHeader header = new CxBlobFileHeader();
        header.FileName = Path.GetFileName(postedFile.FileName);
        header.ContentType = postedFile.ContentType;
        blobFile.LoadFromFileStream(header, postedFile.InputStream);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if current browser is an Internet Explorer
    /// </summary>
    static public bool IsIE
    {
      get
      {
        return HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if current browser is a Netscape
    /// </summary>
    static public bool IsNetscape
    {
      get
      {
        return HttpContext.Current.Request.Browser.Browser.ToUpper() == "NETSCAPE";
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds all child controls of the given type.
    /// </summary>
    static protected void FindChildrenByType(Control control, Type type, IList list)
    {
      if (type.IsInstanceOfType(control))
      {
        list.Add(control);
      }
      foreach (Control child in control.Controls)
      {
        FindChildrenByType(child, type, list);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Finds all child controls of the given type.
    /// </summary>
    /// <param name="control">parent control</param>
    /// <param name="type">type to find</param>
    /// <returns>list of found controls</returns>
    static public IList FindChildrenByType(Control control, Type type)
    {
      ArrayList list = new ArrayList();
      FindChildrenByType(control, type, list);
      return list;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if the given control is a child of the given parent control.
    /// </summary>
    /// <param name="control">control to check</param>
    /// <param name="parent">parent control</param>
    static public bool IsChildOf(Control control, Control parent)
    {
      Control c = control;
      while (c != null)
      {
        if (c == parent)
        {
          return true;
        }
        c = c.Parent;
      }
      return false;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns escaped URL string.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    static public string EscapeUrl(string url)
    {
      return CxUriWrapper.EscapeStringWrapper(url);
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Gets culture info from web request or from current thread.
    /// </summary>
    static public CultureInfo CurrentCulture
    {
      get
      {
        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        if (HttpContext.Current != null &&
            HttpContext.Current.Request != null &&
            HttpContext.Current.Request.UserLanguages != null &&
            HttpContext.Current.Request.UserLanguages.Length > 0 &&
            CxUtils.NotEmpty(HttpContext.Current.Request.UserLanguages[0]))
        {
          string userLanguage = HttpContext.Current.Request.UserLanguages[0];
          try
          {
            cultureInfo = CultureInfo.CreateSpecificCulture(userLanguage);
          }
          catch
          {
            cultureInfo = CultureInfo.CurrentCulture;
          }
        }
        return cultureInfo;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets correct control width on pre-render.
    /// </summary>
    /// <param name="control"></param>
    static public void CorrectControlWidth(WebControl control)
    {
      if (!IsIE && 
          control.Width != Unit.Empty && 
          CxUtils.IsEmpty(control.Style["width"]))
      {
        control.Style["width"] = control.Width.ToString();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns folder that is available for application to write logs,
    /// temporary files, etc. This folder is not available from web.
    /// </summary>
    /// <returns></returns>
    static public string GetApplicationDataFolder()
    {
      string rootFolder = ConfigurationSettings.AppSettings["ApplicationDataFolder"];
      if (CxUtils.IsEmpty(rootFolder))
      {
        rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      }
      string appDataFolder = Path.Combine(rootFolder, "ASPNETApplicationData");
      string appPath = HttpContext.Current.Request.ApplicationPath;
      if (CxUtils.NotEmpty(appPath))
      {
        while ((appPath.StartsWith("/") || appPath.StartsWith("\\")) && appPath.Length > 1)
        {
          appPath = appPath.Substring(1);
        }
        appPath = appPath.Replace("/", "\\");
        if (CxUtils.NotEmpty(appPath) && appPath != "\\")
        {
          appDataFolder = Path.Combine(appDataFolder, appPath);
        }
      }
      if (!Directory.Exists(appDataFolder))
      {
        Directory.CreateDirectory(appDataFolder);
      }
      return appDataFolder;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns folder for saving temporary files.
    /// </summary>
    /// <returns></returns>
    static public string GetApplicationTempFolder()
    {
      string folder = Path.Combine(GetApplicationDataFolder(), "Temp");
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
      return folder;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns folder for saving application log files.
    /// </summary>
    /// <returns></returns>
    static public string GetApplicationLogFolder()
    {
      string folder = Path.Combine(GetApplicationDataFolder(), "Log");
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
      return folder;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns application cache timeout Web.config parameter value.
    /// </summary>
    static public int ApplicationCacheTimeoutSeconds
    {
      get
      {
        return CxInt.Parse(ConfigurationSettings.AppSettings["applicationCacheTimeoutSeconds"], 0);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Object is used to lock application cache.
    /// </summary>
    static protected object m_ApplicationCacheLockObject = new object();
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns cached time map for objects cached on the application level.
    /// </summary>
    static protected Hashtable ApplicationCachedObjectTimeMap
    {
      get 
      {
        lock (m_ApplicationCacheLockObject)
        {

                    Hashtable map;
                    if(HttpContext.Current != null) 
                      map = (Hashtable) HttpContext.Current.Application["APPLICATION_CACHED_OBJECT_TIME_MAP"];
                    else
                      map = (Hashtable)HttpContextStatic.Application["APPLICATION_CACHED_OBJECT_TIME_MAP"];

          if (map == null)
          {
            map = new Hashtable();
                        if(HttpContext.Current != null)
                            HttpContext.Current.Application["APPLICATION_CACHED_OBJECT_TIME_MAP"] = map;
                        else
                            HttpContextStatic.Application["APPLICATION_CACHED_OBJECT_TIME_MAP"] = map;
                    }
          return map;
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns object cached on the Application level with the timeout.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public object GetApplicationCachedObject(string key)
    {
      lock (m_ApplicationCacheLockObject)
      {
        object result = null;
        if (ApplicationCacheTimeoutSeconds <= 0)
        {
          /*if(HttpContext.Current.Session != null)
          {
            result = HttpContext.Current.Session[key];
          }
          else
          {*/
            result = HttpContext.Current.Application[key];
          //}
        }
        else
        {
          if(HttpContext.Current != null)
            result = HttpContext.Current.Application[key];
          else
            result = HttpContextStatic.Application[key];

          object cachedTime = ApplicationCachedObjectTimeMap[key];
          if (result != null)
          {
            bool isValid = cachedTime is DateTime && 
              (DateTime.Now - ((DateTime)cachedTime)).TotalSeconds <= ApplicationCacheTimeoutSeconds;
            if (!isValid)
            {
              HttpContext.Current.Application[key] = null;
              result = null;
            }
          }
        }
        return result;
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sets object cached on the Application level with the timeout.
    /// </summary>
    static public void SetApplicationCachedObject(string key, object cachedObject)
    {
      lock (m_ApplicationCacheLockObject)
      {
        if (ApplicationCacheTimeoutSeconds <= 0)
        {
          /*if(HttpContext.Current.Session != null)
          {
            HttpContext.Current.Session[key] = cachedObject;
          }
          else
          {*/
            HttpContext.Current.Application[key] = cachedObject;
          //}
        }
        else
        {
          HttpContext.Current.Application[key] = cachedObject;
          if (cachedObject != null)
          {
            ApplicationCachedObjectTimeMap[key] = DateTime.Now;
          }
          else
          {
            ApplicationCachedObjectTimeMap[key] = null;
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clears all cached entities with the given entity ID.
    /// </summary>
    /// <param name="entityId"></param>
	  static public void ClearEntityIdCache(string entityId)
    {
      lock (m_ApplicationCacheLockObject)
      {
        List<string> keysToDelete = HttpContext.Current.Application.AllKeys.Where(key =>
          key.StartsWith(entityId + ".", StringComparison.InvariantCultureIgnoreCase)).ToList();
        foreach (string key in keysToDelete)
        {
          HttpContext.Current.Application.Remove(key);
          ApplicationCachedObjectTimeMap.Remove(key);
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Resets all objects cached on the application level with the timeout.
    /// </summary>
    static public void ResetApplicationCachedObjects()
    {
      lock (m_ApplicationCacheLockObject)
      {
        ApplicationCachedObjectTimeMap.Clear();
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Adds additonal parameter ?TS=[File Modification Time Stamp] to the file URL.
    /// </summary>
    static public string GetTimeStampedFileUrl(string fileUrl)
    {
      if (CxUtils.NotEmpty(fileUrl) && fileUrl.IndexOf("?TS=") < 0)
      {
        string timeStamp = null;
        string diskPath = HttpContext.Current.Request.MapPath(fileUrl);
        if (File.Exists(diskPath))
        {
          timeStamp = File.GetLastWriteTime(diskPath).ToString("yyyyMMddHHmmss");
        }
        if (CxUtils.NotEmpty(timeStamp))
        {
          return fileUrl + "?TS=" + timeStamp;
        }
      }
      return fileUrl;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns script HTML element for the given script file URL.
    /// </summary>
    static public string GetScriptFileHtml(string fileUrl)
    {
      return "<script language=\"JavaScript\" src=\"" + GetTimeStampedFileUrl(fileUrl) + "\"></script>";
    }
    //-------------------------------------------------------------------------
	  /// <summary>
	  /// Checks whether the supplied url is absolute
	  /// </summary>
	  /// <param name="url">Url to validate</param>
	  /// <returns>true if url was absolute, false otherwise</returns>
	  public static bool IsUrlAbsolute(string url)
	  {
	    if(CxUtils.NotEmpty(url))
	    {
        return url.StartsWith(HttpContext.Current.Request.ApplicationPath);
      }
      else
	      return false;
	  }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Sign URL with Hash Code.
    /// If attempt to sign fail then original Url will be returned.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string SignUrl(string url)
    {
      if (CxUtils.IsEmpty(url))
      {
        return url;
      }

      int startQs = -1;
      try
      {
        startQs = url.IndexOf('?');
      }
      catch
      {
        startQs = -1;
      }

      string retUrl = "";
      if (startQs > -1)
      {
        try
        {
          CxQueryString qs = new CxQueryString(url.Substring(startQs));
          qs.SignWithHashCode();
          retUrl = url.Substring(0, startQs) + qs.ToString();
        }
        catch
        {
          retUrl = url;
        }
      }

      return retUrl;
    }
    //-------------------------------------------------------------------------
  }
}