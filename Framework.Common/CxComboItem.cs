/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2010 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Drawing;

namespace Framework.Utils
{
	/// <summary>
	/// Clas to use as combobox item.
	/// </summary>
	public class CxComboItem : IComparable
	{
    //--------------------------------------------------------------------------
    protected object m_Value = null; // Value ot the item
    protected string m_Description = ""; // Description ot the item
    protected string m_ImageReference = null; // Reference to the related image
    protected string m_ColorReference = null; // Reference to the related color
	  private Image m_Image;// Image to the item
    //--------------------------------------------------------------------------
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="value">value ot the item</param>
    /// <param name="description">description ot the item</param>
		public CxComboItem(object value, string description)
		{
      m_Value = value;
      m_Description = description;
		}
    //--------------------------------------------------------------------------
    /// <summary>
    /// Value ot the item.
    /// </summary>
    public object Value
    {
      get { return m_Value; }
      set { m_Value = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Description ot the item.
    /// </summary>
    virtual public string Description
    {
      get { return m_Description; }
      set { m_Description = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Reference to the image related to this item.
    /// </summary>
    virtual public string ImageReference
    {
      get { return m_ImageReference; }
      set { m_ImageReference = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Reference to the color related to this item.
    /// </summary>
    virtual public string ColorReference
    {
      get { return m_ColorReference; }
      set { m_ColorReference = value; }
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Image to the item
    /// </summary>
	  public Image Image
	  {
	    get { return m_Image; }
	    set { m_Image = value; }
	  }
	  //--------------------------------------------------------------------------
    /// <summary>
    /// Returns string representation of the object.
    /// </summary>
    /// <returns>string representation of the object</returns>
    override public string ToString()
    {
      return Description;
    }
    //--------------------------------------------------------------------------
    /// <summary>
    /// Compares this object with another one.
    /// </summary>
    /// <param name="obj">object to compare with</param>
    /// <returns>1 if description of this objects is greater that one of the another object,
    /// 0 if they are equal or -1 otherwise</returns>
    public int CompareTo(object obj)
    {
      return Description.CompareTo(((CxComboItem) obj).Description);
    }
    //--------------------------------------------------------------------------
  }
}
