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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Entity;

namespace Framework.Remote
{
  [DataContract]
  public class CxClientEntityMarks
  {
    [DataMember]
    public List<CxClientEntityMark> AllRecentItems { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientEntityMark> RemovedRecentItems { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientEntityMark> AddedRecentItems { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientEntityMark> AllBookmarkItems { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientEntityMark> RemovedBookmarkItems { get; private set; }
    //----------------------------------------------------------------------------
    [DataMember]
    public List<CxClientEntityMark> AddedBookmarkItems { get; private set; }

    //----------------------------------------------------------------------------
    public CxClientEntityMarks()
    {
      AllRecentItems = new List<CxClientEntityMark>();
      RemovedRecentItems = new List<CxClientEntityMark>();
      AddedRecentItems = new List<CxClientEntityMark>();
      AllBookmarkItems = new List<CxClientEntityMark>();
      RemovedBookmarkItems = new List<CxClientEntityMark>();
      AddedBookmarkItems = new List<CxClientEntityMark>();
    }

    //----------------------------------------------------------------------------
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
