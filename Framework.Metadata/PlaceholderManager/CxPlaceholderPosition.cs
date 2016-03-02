using System;

namespace Framework.Metadata.PlaceholderManager
{
  public class CxPlaceholderPosition
  {
    //-------------------------------------------------------------------------
    private int m_StartIndex;
    private int m_Length;
    //-------------------------------------------------------------------------
    public int StartIndex
    {
      get { return m_StartIndex; }
      set { m_StartIndex = value; }
    }
    //-------------------------------------------------------------------------
    public int Length
    {
      get { return m_Length; }
      set { m_Length = value; }
    }
    //-------------------------------------------------------------------------
    public CxPlaceholderPosition()
    {

    }
    //-------------------------------------------------------------------------
    public CxPlaceholderPosition(int startIndex, int length)
      : this()
    {
      StartIndex = startIndex;
      Length = length;
    }
    //-------------------------------------------------------------------------
  }
}
