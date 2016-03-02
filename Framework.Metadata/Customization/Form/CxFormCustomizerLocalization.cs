using System;

namespace Framework.Metadata
{
  public class CxFormCustomizerLocalization
  {
    //-------------------------------------------------------------------------
    private CxFormCustomizer m_Customizer;
    //-------------------------------------------------------------------------
    /// <summary>
    /// The customizer the data belongs to.
    /// </summary>
    public CxFormCustomizer Customizer
    {
      get { return m_Customizer; }
      set { m_Customizer = value; }
    }
    //-------------------------------------------------------------------------
    public CxFormCustomizerLocalization(CxFormCustomizer customizer)
    {
      Customizer = customizer;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Compares the data with another.
    /// </summary>
    /// <param name="otherData">the object to compare with</param>
    /// <returns>true if equal</returns>
    public bool Compare(CxFormCustomizerLocalization otherData)
    {
      return true;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns a clone of the customizer data.
    /// </summary>
    public CxFormCustomizerLocalization Clone()
    {
      CxFormCustomizerLocalization clone = new CxFormCustomizerLocalization(Customizer);
      return clone;
    }
    //-------------------------------------------------------------------------
  }
}
