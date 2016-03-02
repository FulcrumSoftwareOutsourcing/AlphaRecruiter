using System.Web;
using System.Web.Optimization;

namespace App.Server
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));



            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/app/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/app/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/app/bootstrap.js",
            //          "~/Scripts/app/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));


            //bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
            //           "~/Scripts/app/knockout-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/app").IncludeDirectory("~/Scripts/app/app/", "*.js", true));



            bundles.Add(new ScriptBundle("~/bundles/app").Include(

                        "~/Scripts/app/Common/Utils.js",

                        "~/Scripts/app/Common/Consts.js",
                        "~/Scripts/app/Common/AppSizes.js",

                        "~/Scripts/app/Common/Multilang.js",
                        "~/Scripts/app/Common/HtmlProvider.js",
                        "~/Scripts/app/Common/AppSettings.js",
                        

                        "~/Scripts/app/Infrastructure/WaitDispatcher.js",
                        "~/Scripts/app/Infrastructure/ObservableObject.js",
                        "~/Scripts/app/Infrastructure/Layer.js",
                        "~/Scripts/app/Infrastructure/LayerFasade.js",
                        "~/Scripts/app/Infrastructure/BusinessLayer.js",
                        "~/Scripts/app/Infrastructure/DataLayer.js",
                        "~/Scripts/app/Infrastructure/ViewLayer.js",
                        "~/Scripts/app/Common/Metadata.js",
                        "~/Scripts/app/Common/AttributeUtils.js",
                        "~/Scripts/app/Common/EntityValidator.js",
                        "~/Scripts/app/MsgBox/BMsgBox.js",
                        "~/Scripts/app/MsgBox/VMsgBox.js",
                        "~/Scripts/app/Navigation/VNavigationBar.js",
                        "~/Scripts/app/Root/BRoot.js",
                        "~/Scripts/app/Root/RootDataBorder.js",
                        "~/Scripts/app/Root/RootViewBorder.js",
                        "~/Scripts/app/Root/VRoot.js",
                        "~/Scripts/app/Root/DRoot.js",
                        "~/Scripts/app/LoginForm/BLoginForm.js",
                        "~/Scripts/app/LoginForm/DLoginForm.js",
                        "~/Scripts/app/LoginForm/VLoginForm.js",
                        "~/Scripts/app/LoginForm/LoginFormViewBorder.js",

                        "~/Scripts/app/Popup/BPopup.js",
                        "~/Scripts/app/Popup/VPopup.js",
                        "~/Scripts/app/LoginPanel/BLoginPanel.js",
                        "~/Scripts/app/LoginPanel/VLoginPanel.js",
                        "~/Scripts/app/Sections/BSections.js",
                        "~/Scripts/app/Sections/VSections.js",
                        "~/Scripts/app/Sections/SectionsViewBorder.js",

                        "~/Scripts/app/Tree/VTree.js",
                        "~/Scripts/app/Tree/TreeViewBorder.js",
                        "~/Scripts/app/Tree/BTree.js",

                        "~/Scripts/app/WorkspacePnl/BWorkspacePnl.js",
                        "~/Scripts/app/WorkspacePnl/VWorkspacePnl.js",

                        "~/Scripts/app/Frames/BFramesRoot.js",
                        "~/Scripts/app/Frames/VFramesRoot.js",
                        "~/Scripts/app/UploadDlg/VUploadDlg.js",
                        "~/Scripts/app/UploadDlg/BUploadDlg.js",

                        "~/Scripts/app/Frames/BaseFrame/BBaseFrame.js",
                        "~/Scripts/app/Frames/BaseFrame/VBaseFrame.js",
                        "~/Scripts/app/Frames/BaseFrame/BaseFrameViewBorder.js",
                        "~/Scripts/app/Frames/BaseFrame/BaseFrameDataBorder.js",

                        "~/Scripts/app/Frames/GridFrame/DataGrid/VDataGrid.js",
                        "~/Scripts/app/Frames/GridFrame/DataGrid/BDataGrid.js",
                        "~/Scripts/app/Frames/GridFrame/DataGrid/DataGridViewBorder.js",

                        "~/Scripts/app/Frames/GridFrame/BGridFrame.js",
                        "~/Scripts/app/Frames/GridFrame/VGridFrame.js",
                        "~/Scripts/app/Frames/GridFrame/GridFrameViewBorder.js",
                        "~/Scripts/app/Frames/GridFrame/GridFrameDataBorder.js",
                        "~/Scripts/app/Frames/GridFrame/UnderConstrustionFrame/BUnderConstrustionFrame.js",
                        "~/Scripts/app/Frames/GridFrame/UnderConstrustionFrame/VUnderConstrustionFrame.js",

                        "~/Scripts/app/Frames/GridFrame/Filters/BFilters.js",
                        "~/Scripts/app/Frames/GridFrame/Filters/VFilters.js",
                        "~/Scripts/app/Frames/GridFrame/Filters/FiltersViewBorder.js",

                        "~/Scripts/app/Frames/Commands/BCommandBar.js",
                        "~/Scripts/app/Frames/Commands/CommandBarVBorder.js",
                        "~/Scripts/app/Frames/Commands/VCommandBar.js",
                        "~/Scripts/app/Frames/Commands/CommandHandler.js",

                        "~/Scripts/app/Common/QueryParams.js",

                        "~/Scripts/app/Frames/GridFrame/Paging/BPagingPnl.js",
                        "~/Scripts/app/Frames/GridFrame/Paging/VPagingPnl.js",

                        "~/Scripts/app/Frames/AutoLayoutFrame/LayoutBuilder.js",
                        "~/Scripts/app/Frames/AutoLayoutFrame/FrameBuilder.js",

                        "~/Scripts/app/Frames/AutoLayoutFrame/BAutoLayoutFrame.js",
                        "~/Scripts/app/Frames/AutoLayoutFrame/VAutoLayoutFrame.js",
                        "~/Scripts/app/Frames/AutoLayoutFrame/AutoLayoutFrameViewBorder.js",
                        "~/Scripts/app/Frames/AutoLayoutFrame/AutoLayoutFrameDataBorder.js",

                        "~/Scripts/app/Tracker/ItrGridFrame/BItrGridFrame.js",
                        "~/Scripts/app/Tracker/ItrGridFrame/VItrGridFrame.js",

                        "~/Scripts/app/HR/BGridWithExtraFind.js",
                        "~/Scripts/app/HR/VGridWithExtraFind.js",

                        "~/Scripts/app/Frames/LocalizationFrame/BLocalizationFrame.js",
                        "~/Scripts/app/Frames/LocalizationFrame/VLocalizationFrame.js",

                        "~/Scripts/app/site.js"

                       ));


#if (!DEBUG)
            BundleTable.EnableOptimizations = true;
#endif

#if (DEBUG)
            BundleTable.EnableOptimizations = false;
#endif
        }
    }
}
