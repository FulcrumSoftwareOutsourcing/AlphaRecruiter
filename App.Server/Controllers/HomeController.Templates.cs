using App.Server.Models.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace App.Server.Controllers
{
    public partial class HomeController : Controller
    {
        [HttpPost]
       
        public ActionResult   GetTemplate(IEnumerable<string> requiredTemplates, string settingsToSave)
        {
            JsonResult json = new JsonResult();
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                SaveSetting(settingsToSave);
                var result = new
                {
                    Templates = new List<object>(),
                };
                result.Templates.AddRange( GetTemplates(requiredTemplates));
                json.Data = result;
                return json;
            }
            catch (Exception ex)
            {
                var result = new { Error = ex.Message };

                json.Data = result;
                return json;
            }
        }

        private   List<object>   GetTemplates(IEnumerable<string> requiredTemplates)
        {
            List<object> templates = new List<object>();
            if (requiredTemplates == null)
                return templates;

            TemplateProvider prov = new TemplateProvider();
            foreach (string templId in requiredTemplates)
            {
                templates.Add(new { Id = templId, Template = prov.GetTemplate(templId, Url.SkinFolder(), this) });
            }
            return templates;
        }
    }

     public static class HtmlTemplateIds
    {
        public const string  PopupTemplate = "Popup/PopupTemplate";
        public const string LoginPnlTemplate = "LoginPnl/LoginPnlTemplate";
        public const string LoginFormTemplate = "LoginForm/LoginForm";
        public const string SectionsTemplate = "Sections/SectionsTemplate";
        public const string TreeViewTemplate = "Tree/TreeViewTemplate";
        public const string WorkspacesPnlTemplate = "Workspaces/WorkspacesPnlTemplate";
        public const string WorkspacesDialogTemplate = "Workspaces/WorkspaceDialogTemplate";
        public const string GridFrameTemplate = "GridFrame/GridFrameTemplate";
        public const string GridTemplate = "GridFrame/GridTemplate";
        public const string FormEditControlsTemplate = "Editors/FormEditControlsTemplate";
        public const string FormViewControlsTemplate = "Editors/FormViewControlsTemplate";

        public const string CommonEditorsTemplate = "Editors/CommonEditorsTemplate";
        public const string GridEditorsTemplate = "Editors/GridEditorsTemplate";
        public const string FormEditorsTemplate = "Editors/FormEditorsTemplate";

        public const string CommandsBarTemplate = "Commands/CommandsBar";
        public const string PagingPnlTemplate = "GridFrame/PagingPnl";
        public const string AutoLayoutFrameTemplate = "AutoLayoutFrame/AutoLayoutFrameTemplate";
        public const string MessageBoxTemplate = "MsgBox/MsgBoxTemplate";
        public const string FiltersPanelTemplate = "GridFrame/FiltersPanel/FiltersPanelTemplate";

        public const string UploadDlgTemplate = "UploadDlg/UploadDlgTemplate";
    }

 

}