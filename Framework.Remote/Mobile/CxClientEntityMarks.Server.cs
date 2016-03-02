using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Entity;

namespace Framework.Remote.Mobile
{
    public partial class CxClientEntityMarks
    {
        public static CxClientEntityMarks Greate()
        {
            CxAppServerContext serverContext = new CxAppServerContext();
            CxEntityMarks entityMarks = serverContext.EntityMarks;
            CxClientEntityMarks clientEntityMarks = new CxClientEntityMarks();
            if (entityMarks != null)
            {
                foreach (CxEntityMark recentItem in entityMarks.RecentItems)
                {
                    clientEntityMarks.AllRecentItems.Add(new CxClientEntityMark(recentItem));
                }
                foreach (CxEntityMark bookmarkItem in entityMarks.BookmarkItems)
                {
                    clientEntityMarks.AllBookmarkItems.Add(new CxClientEntityMark(bookmarkItem));
                }
            }
            return clientEntityMarks;
        }
    }
}
