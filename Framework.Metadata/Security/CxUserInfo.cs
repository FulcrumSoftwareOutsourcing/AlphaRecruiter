using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Metadata
{
  public class CxUserInfo
  {
    //-------------------------------------------------------------------------
    private object m_UserId;
    private string m_UserName;
    private bool m_IsForceToChangePassword;
    //-------------------------------------------------------------------------
    public object UserId
    {
      get { return m_UserId; }
      set { m_UserId = value; }
    }
    //-------------------------------------------------------------------------
    public string UserName
    {
      get { return m_UserName; }
      set { m_UserName = value; }
    }
    //-------------------------------------------------------------------------
    public bool IsForceToChangePassword
    {
      get { return m_IsForceToChangePassword; }
      set { m_IsForceToChangePassword = value; }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxUserInfo()
    {
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    public CxUserInfo(object userId, string userName, bool isForcedToChangePassword)
    {
      UserId = userId;
      UserName = userName;
      IsForceToChangePassword = IsForceToChangePassword;
    }
    //-------------------------------------------------------------------------
  }
}
