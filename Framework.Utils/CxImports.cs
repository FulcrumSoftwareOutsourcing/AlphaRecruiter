using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Framework.Utils
{
  /// <summary>
  /// Class to contains imported WinAPI functions.
  /// </summary>
  public class CxImports
  {
    //----------------------------------------------------------------------------
    public const int WM_LBUTTONDOWN = 0x0201;
    public const int WM_LBUTTONUP = 0x0202;
    public const int WM_LBUTTONDBLCLK = 0x0203;
    public const int MK_LBUTTON = 1;
    public const int EM_GETLINECOUNT = 0x00BA;
    public const int EM_LINESCROLL = 0x00B6;
    public const int WM_USER = 0x0400;
    public const int EM_SETSCROLLPOS = WM_USER + 222;
    public const int WM_SETTEXT = 0x000C;
    public const int WM_SETREDRAW = 0x000B;
    public const int WM_APP = 0x0800;
    public const int WM_APP_ADDTREENODE = WM_APP + 1;
    public const int WM_APP_DELETETREENODE = WM_APP + 2;
    public const int WM_SYSCOMMAND = 0x0112;
    public const int SC_RESTORE = 0xF120;
    public const int WPF_RESTORETOMAXIMIZED = 2;
    //----------------------------------------------------------------------------
    // PeekMessage constants
    public const int PM_NOREMOVE = 0;
    public const int PM_REMOVE = 1;
    //----------------------------------------------------------------------------
    // DrawText constants
    public const uint DT_TOP = 0x0000;
    public const uint DT_LEFT = 0x0000;
    public const uint DT_CENTER = 0x0001;
    public const uint DT_RIGHT = 0x0002;
    public const uint DT_VCENTER = 0x0004;
    public const uint DT_BOTTOM = 0x0008;
    public const uint DT_WORDBREAK = 0x0010;
    public const uint DT_SINGLELINE = 0x0020;
    public const uint DT_EXPANDTABS = 0x0040;
    public const uint DT_TABSTOP = 0x0080;
    public const uint DT_NOCLIP = 0x0100;
    public const uint DT_EXTERNALLEADING = 0x0200;
    public const uint DT_CALCRECT = 0x0400;
    public const uint DT_NOPREFIX = 0x0800;
    public const uint DT_INTERNAL = 0x1000;
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct POINT
    {
      [FieldOffset(0)]
      public int x;

      [FieldOffset(4)]
      public int y;
    }
    //----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public struct MSG
    {
      [FieldOffset(0)]
      public IntPtr hwnd;

      [FieldOffset(4)]
      public uint message;

      [FieldOffset(8)]
      public uint wParam;

      [FieldOffset(12)]
      public int lParam;

      [FieldOffset(16)]
      public uint time;

      [FieldOffset(20)]
      public POINT pt;
    }
    //----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct RECT
    {
      [FieldOffset(0)]
      public int left;

      [FieldOffset(4)]
      public int top;

      [FieldOffset(8)]
      public int right;

      [FieldOffset(12)]
      public int bottom;

      public RECT(int l, int t, int r, int b)
      {
        left = l;
        top = t;
        right = r;
        bottom = b;
      }
    }
    /// <summary>
    /// Point struct used for GetWindowPlacement API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class ManagedPt
    {
      public int x = 0;
      public int y = 0;

      public ManagedPt(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Rect struct used for GetWindowPlacement API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class ManagedRect
    {
      public int x = 0;
      public int y = 0;
      public int right = 0;
      public int bottom = 0;
      //----------------------------------------------------------------------------
      public ManagedRect(int x, int y, int right, int bottom)
      {
        this.x = x;
        this.y = y;
        this.right = right;
        this.bottom = bottom;
      }
    }
    //----------------------------------------------------------------------------
    /// &ltsummary>
    /// WindowPlacement struct used for GetWindowPlacement API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class ManagedWindowPlacement
    {
      public uint length = 0;
      public uint flags = 0;
      public uint showCmd = 0;
      public ManagedPt minPosition = null;
      public ManagedPt maxPosition = null;
      public ManagedRect normalPosition = null;

      public ManagedWindowPlacement()
      {
        length = (uint) Marshal.SizeOf(this);
      }
    }
    //----------------------------------------------------------------------------

    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
    static public extern bool GetCursorPos(ref Point pos);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "ScreenToClient")]
    static public extern bool ScreenToClient(IntPtr handle, ref Point pos);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "ClientToScreen")]
    static public extern bool ClientToScreen(IntPtr handle, ref Point pos);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
    static public extern bool SetForegroundWindow(IntPtr handle);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "SendMessage")]
    static public extern int SendMessage(IntPtr handle, int message, IntPtr wparam, IntPtr lparam);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "SendMessage")]
    static public extern int SendMessage(IntPtr handle, int message, uint wparam, uint lparam);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "PostMessage")]
    static public extern int PostMessage(IntPtr handle, int message, uint wparam, uint lparam);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "PostMessage")]
    static public extern int PostMessage(IntPtr handle, int message, uint wparam, string lparam);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "PeekMessage")]
    static public extern bool PeekMessage(out MSG lpMsg, IntPtr hWnd, int wMsgFilterMin, int wMsgFilterMax, uint wRemoveMsg);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "MessageBeep")]
    static public extern bool MessageBeep(int type);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
    static public extern IntPtr WindowFromPoint(Point point);
    //----------------------------------------------------------------------------
    [DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32")]
    static public extern bool GetTextExtentPoint32(IntPtr hdc, string s, int count, out Size size);
    //----------------------------------------------------------------------------
    [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
    static public extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "InvalidateRect")]
    static public extern int InvalidateRect(IntPtr handle, int rect, int erase);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", EntryPoint = "DrawText")]
    static public extern int DrawText(IntPtr hdc, string lpString, int nCount, ref RECT lpRect, uint uFormat);
    //-------------------------------------------------------------------------
    [DllImport("USER32.DLL", SetLastError = true)]
    public static extern uint GetWindowPlacement(uint hwnd,
        [In, Out]ManagedWindowPlacement lpwndpl);
    //----------------------------------------------------------------------------
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool DestroyIcon(IntPtr handle);
    //----------------------------------------------------------------------------
  }
}