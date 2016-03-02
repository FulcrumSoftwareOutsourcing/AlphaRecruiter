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

namespace Framework.Db
{
  public class CxOdbcScriptGenerator : CxDbScriptGenerator
  {
    //----------------------------------------------------------------------------
    /// <summary>
    /// OLEDB-connection context.
    /// </summary>
    protected CxOdbcConnection OdbcConnection
    {
      get { return Connection as CxOdbcConnection; }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="connection">connection context</param>
    public CxOdbcScriptGenerator(CxOdbcConnection connection)
      : base(connection)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns substitute for the parameter in the provider-dependent format.
    /// </summary>
    /// <param name="name">cross-provider parameter name</param>
    /// <param name="index">parameter index</param>
    /// <returns>substitute for the parameter in the provider-dependent format</returns>
    override public string PrepareParameterSubstitute(string name, int index)
    {
      return (name.StartsWith("@") ? name : "@" + name);
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns parameter name in the provider-dependent format.
    /// </summary>
    /// <param name="name">cross-provider parameter name</param>
    /// <param name="index">parameter index</param>
    /// <returns>parameter name in the provider-dependent format</returns>
    override public string PrepareParameterName(string name, int index)
    {
      return (name.StartsWith("@") ? name : "@" + name);
    }
    //----------------------------------------------------------------------------
  }
}
