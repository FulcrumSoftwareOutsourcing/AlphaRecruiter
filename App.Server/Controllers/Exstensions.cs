using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static class Exstensions
    {
        public static string SkinFolder(this UrlHelper helper)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                //IUserProfile userProfile = HttpContext.Current.Session[SessionKeys.UserProfile] as IUserProfile;
                //if (userProfile != null)
                //    return userProfile.SkinId;
                return "Default";
            }
            return "Default";

        }

        public static string RenderViewToString(this Controller controller, string viewName, string masterName = null)
        {
            var content = string.Empty;
            var view = ViewEngines.Engines.FindView(controller.ControllerContext, viewName, null);
            using (var writer = new StringWriter())
            {
                var context = new ViewContext(controller.ControllerContext, view.View, controller.ViewData, controller.TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();
            }
            return content;
        }


        public class JsonDictionary : Dictionary<string, object>
        {
            public JsonDictionary() { }

            public void Add(JsonDictionary jsonDictionary)
            {
                if (jsonDictionary != null)
                {
                    foreach (var k in jsonDictionary.Keys)
                    {
                        this.Add(k, jsonDictionary[k]);
                    }
                }
            }
        }

        public class JsonDictionaryModelBinder : IModelBinder
        {
            #region IModelBinder Members

            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {



                if (bindingContext.ModelMetadata.Model == null) { bindingContext.ModelMetadata.Model = new JsonDictionary(); }
                var model = bindingContext.ModelMetadata.Model as JsonDictionary;

                if (bindingContext.ModelType == typeof(JsonDictionary))
                {
                    // Deserialize each form/querystring item specified in the "includeProperties"
                    // parameter that was passed to the "UpdateModel" method call

                    // Check/Add Form Collection
                    this.addRequestValues(
                        model,
                        controllerContext.RequestContext.HttpContext.Request.Form,
                        controllerContext, bindingContext);

                    // Check/Add QueryString Collection
                    this.addRequestValues(
                        model,
                        controllerContext.RequestContext.HttpContext.Request.QueryString,
                        controllerContext, bindingContext);
                }

                return model;
            }

            #endregion

            private void addRequestValues(JsonDictionary model, NameValueCollection nameValueCollection, ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                foreach (string key in nameValueCollection.Keys)
                {
                    if (bindingContext.ModelName == key)
                    {
                        var jsonText = nameValueCollection[key];
                        var newModel = deserializeJson(jsonText);
                        // Add the new JSON key/value pairs to the Model
                        model.Add(newModel);
                    }
                }
            }

            private JsonDictionary deserializeJson(string json)
            {
                // Must Reference "System.Web.Extensions" in order to use the JavaScriptSerializer
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                return serializer.Deserialize<JsonDictionary>(json);
            }
        }
    }
}