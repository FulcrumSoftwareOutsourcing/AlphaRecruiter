function HtmlTemplateIds() { };

HtmlTemplateIds.PopupTemplate = "Popup/PopupTemplate";
HtmlTemplateIds.LoginPnlTemplate = 'LoginPnl/LoginPnlTemplate';
HtmlTemplateIds.LoginFormTemplate = 'LoginForm/LoginForm';
HtmlTemplateIds.SectionsTemplate = 'Sections/SectionsTemplate';
HtmlTemplateIds.TreeViewTemplate = 'Tree/TreeViewTemplate';
HtmlTemplateIds.WorkspacesPnlTemplate = 'Workspaces/WorkspacesPnlTemplate';

HtmlTemplateIds.GridFrameTemplate = 'GridFrame/GridFrameTemplate';

HtmlTemplateIds.FormEditControlsTemplate = 'Editors/FormEditControlsTemplate';
HtmlTemplateIds.FormViewControlsTemplate = 'Editors/FormViewControlsTemplate';
HtmlTemplateIds.CommonEditorsTemplate = "Editors/CommonEditorsTemplate";
HtmlTemplateIds.GridEditorsTemplate = 'Editors/GridEditorsTemplate';
HtmlTemplateIds.FormEditorsTemplate = 'Editors/FormEditorsTemplate';

HtmlTemplateIds.CommandsBarTemplate = 'Commands/CommandsBar';
HtmlTemplateIds.PagingPnlTemplate = 'GridFrame/PagingPnl';
HtmlTemplateIds.AutoLayoutFrameTemplate = "AutoLayoutFrame/AutoLayoutFrameTemplate";
HtmlTemplateIds.MessageBoxTemplate = "MsgBox/MsgBoxTemplate";
HtmlTemplateIds.FiltersPanelTemplate = "GridFrame/FiltersPanel/FiltersPanelTemplate";
HtmlTemplateIds.UploadDlgTemplate = "UploadDlg/UploadDlgTemplate";

function DialogIds() { };
DialogIds.Login = 'Login';

function DialogButtons() { };
DialogButtons.OK = 'OK';
DialogButtons.Yes = 'Yes';
DialogButtons.No = 'No';
DialogButtons.Cancel = 'Cancel';


function MessagingSpecialKeys() { };
MessagingSpecialKeys.WebRequestCallback = '___WebRequestCallback___';
MessagingSpecialKeys.WaitingId = '___WaitingId___';
MessagingSpecialKeys.WebRequestUrl = '___url___';
MessagingSpecialKeys.PostData = '___PostData___';


function WaitersIds() { };
WaitersIds.Popup = 'w_popup_A1A340ED';
WaitersIds.DldWorkingWaiter = 'w_DldWorking_S3N380KU';
WaitersIds.PartLoading = 'w_PartLoading_X5M1J0OZ';
WaitersIds.DialogDataLoading = 'w_DialogDataLoading_ZFM1T0S4Z';


function RequiredTemplates() { };
//RequiredTemplates.TaskPartStart = [TaskList];
//RequiredTemplates.ProjectList = [HtmlTemplateIds.ProjectListTemplate];
//RequiredTemplates.DemoRestriction = [HtmlTemplateIds.DemoRestrictionDlg];
//RequiredTemplates.CreateNewProject = [HtmlTemplateIds.NewProjectDlgTemplate];
RequiredTemplates.Portal = [HtmlTemplateIds.SectionsTemplate, HtmlTemplateIds.TreeViewTemplate, HtmlTemplateIds.WorkspacesPnlTemplate]
RequiredTemplates.LoginForm = [HtmlTemplateIds.LoginFormTemplate];
RequiredTemplates.Sections = [HtmlTemplateIds.SectionsTemplate];
RequiredTemplates.TreeView = [HtmlTemplateIds.TreeViewTemplate];
RequiredTemplates.WorkspacesPnl = [HtmlTemplateIds.WorkspacesPnlTemplate];

RequiredTemplates.GridFrame = [HtmlTemplateIds.GridFrameTemplate, HtmlTemplateIds.GridEditorsTemplate, HtmlTemplateIds.CommonEditorsTemplate, HtmlTemplateIds.CommandsBarTemplate, HtmlTemplateIds.PagingPnlTemplate, HtmlTemplateIds.FiltersPanelTemplate];
RequiredTemplates.AutoLayoutFrame = [HtmlTemplateIds.AutoLayoutFrameTemplate, HtmlTemplateIds.CommonEditorsTemplate, HtmlTemplateIds.FormEditorsTemplate];
RequiredTemplates.UploadDlg = [HtmlTemplateIds.UploadDlgTemplate];



function NxOpenMode() { };
NxOpenMode.View = 'View';
NxOpenMode.Edit = 'Edit';
NxOpenMode.New = 'New';
NxOpenMode.Autodetect = 'Autodetect';
NxOpenMode.ChildView = 'ChildView';
NxOpenMode.ChildEdit = 'ChildEdit';
NxOpenMode.ChildNew = 'ChildNew';


function ControlsTemplatesIds() { };
ControlsTemplatesIds.RowSourceComboBox = 'RowSourceComboBox';
ControlsTemplatesIds.string = 'string';
ControlsTemplatesIds.datetime = 'datetime';
ControlsTemplatesIds.date = 'date';
ControlsTemplatesIds.memo = 'memo';
ControlsTemplatesIds.boolean = 'boolean';
ControlsTemplatesIds.int = 'int';
ControlsTemplatesIds.float = 'float';
ControlsTemplatesIds.file = 'file';
ControlsTemplatesIds.photo = 'photo';
ControlsTemplatesIds.hyperlink = 'hyperlink';

function CommandIDs(){}
CommandIDs.EDIT = "EDIT";
CommandIDs.NEW = "NEW";
CommandIDs.DELETE = "DELETE";
CommandIDs.SAVE = "SAVE";
CommandIDs.VIEW = "VIEW";
CommandIDs.UP = "Up";
CommandIDs.DOWN = "Down";
    // Localization commands
CommandIDs.LOCALIZATION_RESET_CACHE = "LOCALIZATION_RESETCACHE";
CommandIDs.LOCALIZATION_EXPORT_NON_TRANSLATED = "LOCALIZATION_EXPORTNOTTRANSLATED";
CommandIDs.LOCALIZATION_EXPORT_TRANSLATED = "LOCALIZATION_EXPORTTRANSLATED";
CommandIDs.LOCALIZATION_IMPORT_TRANSLATED = "LOCALIZATION_IMPORTTRANSLATED";
CommandIDs.SERVER_LOCALIZATION_COMMAND = "LOCALIZATIONCOMMANDHANDLER";



function NxMessageBoxIcon(){};
NxMessageBoxIcon.Error = 'error_48x48';
NxMessageBoxIcon.Information = 'information_48x48';
NxMessageBoxIcon.Question = 'question_and_answer_48x48';
NxMessageBoxIcon.Warning = 'warning_48x48';

function NxFilterOperation() { }
NxFilterOperation.None = 'None';
NxFilterOperation.Equal = 'Equal';
NxFilterOperation.NotEqual = 'NotEqual';
NxFilterOperation.Less = 'Less';
NxFilterOperation.Greater = 'Greater';
NxFilterOperation.LessEqual = 'LessEqual';
NxFilterOperation.GreaterEqual = 'GreaterEqual';
NxFilterOperation.Between = 'Between';
NxFilterOperation.Like = 'Like';
NxFilterOperation.NotLike = 'NotLike';
NxFilterOperation.StartsWith = 'StartsWith';
NxFilterOperation.IsNull = 'IsNull';
NxFilterOperation.IsNotNull = 'IsNotNull';
NxFilterOperation.Today = 'Today';
NxFilterOperation.ThisWeek = 'ThisWeek';
NxFilterOperation.ThisMonth = 'ThisMonth';
NxFilterOperation.ThisYear = 'ThisYear';
NxFilterOperation.Yesterday = 'Yesterday';
NxFilterOperation.PrevWeek = 'PrevWeek';
NxFilterOperation.PrevMonth = 'PrevMonth';
NxFilterOperation.PrevYear = 'PrevYear';
NxFilterOperation.InThePast = 'InThePast';
NxFilterOperation.TodayOrLater = 'TodayOrLater';
NxFilterOperation.Tomorrow = 'Tomorrow';
NxFilterOperation.NotExists = 'NotExists';
NxFilterOperation.Myself = 'Myself';
NxFilterOperation.Custom = 'Custom';



function NxListSortDirection() { }
NxListSortDirection.None = 'None';
NxListSortDirection.Ascending = 'Ascending';
NxListSortDirection.Descending = 'Descending';

function BlobStateAttrValues() {}
BlobStateAttrValues.BLOB_PRESENT_IN_DB = "BLOB_PRESENT_IN_DB";
BlobStateAttrValues.REMOVE_BLOB_FROM_DB = "REMOVE_BLOB_FROM_DB";
