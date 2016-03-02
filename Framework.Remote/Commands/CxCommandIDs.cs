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

namespace Framework.Remote
{
    /// <summary>
    /// ID constants for the standard commands.
    /// </summary>
    public class CxCommandIDs
    {
        public const string EDIT = "EDIT";
        public const string NEW = "NEW";
        public const string DELETE = "DELETE";
        public const string SAVE = "SAVE";
        public const string VIEW = "VIEW";
        //public const string CUT = "CUT";
        //public const string COPY = "COPY";
        //public const string PASTE = "PASTE";
        //public const string FILTER = "FILTER";
        //public const string RECURRENCE = "RECURRENCE"; // Activity recurrence command
        //// Localization commands
        //public const string LOCALIZATION_RESET_CACHE = "LOCALIZATION_RESETCACHE";
        //public const string LOCALIZATION_EXPORT_NON_TRANSLATED = "LOCALIZATION_EXPORTNOTTRANSLATED";
        //public const string LOCALIZATION_EXPORT_TRANSLATED = "LOCALIZATION_EXPORTTRANSLATED";
        //public const string LOCALIZATION_IMPORT_TRANSLATED = "LOCALIZATION_IMPORTTRANSLATED";
        //// Security commands
        //public const string SECURITY_ASSIGN_PERMISSIONS = "SECURITY_ASSIGNPERMISSIONS";

        // Localization commands
        public const string LOCALIZATION_RESET_CACHE = "LOCALIZATION_RESETCACHE";
        public const string LOCALIZATION_EXPORT_NON_TRANSLATED = "LOCALIZATION_EXPORTNOTTRANSLATED";
        public const string LOCALIZATION_EXPORT_TRANSLATED = "LOCALIZATION_EXPORTTRANSLATED";
        public const string LOCALIZATION_IMPORT_TRANSLATED = "LOCALIZATION_IMPORTTRANSLATED";
    }
}
