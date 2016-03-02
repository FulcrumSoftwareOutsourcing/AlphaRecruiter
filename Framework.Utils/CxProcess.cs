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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace Framework.Utils
{
  public class CxProcess
  {
    //-------------------------------------------------------------------------
    /// <summary>
    /// Runs the given executable with the arguments.
    /// </summary>
    /// <param name="exeFileName">executable file to run</param>
    /// <param name="args">arguments to execute with</param>
    /// <returns>executable exit code</returns>
    static public int Run(string exeFileName, string args)
    {
      ProcessStartInfo psi = new ProcessStartInfo(exeFileName);
      psi.Arguments = args;
      psi.WindowStyle = ProcessWindowStyle.Minimized;
      Process process = Process.Start(psi);
      process.WaitForExit();
      int exitCode = process.ExitCode;
      process.Close();
      return exitCode;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Runs the given executable.
    /// </summary>
    /// <param name="exeFileName">executable file to run</param>
    /// <returns>executable exit code</returns>
    static public int Run(string exeFileName)
    {
      return Run(exeFileName, "");
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Starts service or raises an exception if failed to run.
    /// </summary>
    /// <param name="serviceName">name of service to start</param>
    /// <param name="timeOut">timeout in seconds; 0 means unlimited timeout</param>
    /// <param name="performDoEvents">if true, Application.DoEvents method is called during wait</param>
    static public void StartService(
      string serviceName,
      int timeOut,
      bool performDoEvents)
    {
      ServiceController sc = new ServiceController(serviceName);
      if (sc == null)
      {
        throw new ExServiceStartException(serviceName, "Service is not found.");
      }
      ServiceControllerStatus status = sc.Status;
      if (status != ServiceControllerStatus.Stopped &&
          status != ServiceControllerStatus.Running)
      {
        throw new ExServiceStartException(
          serviceName, 
          String.Format("Status of the service is '{0}'.", status));
      }
      sc.Start();
      DateTime t1 = DateTime.Now;
      while (sc.Status != ServiceControllerStatus.Running)
      {
        if (performDoEvents)
        {
          System.Windows.Forms.Application.DoEvents();
        }
        sc.Refresh();
        DateTime t2 = DateTime.Now;
        TimeSpan ts = t2 - t1;
        if (timeOut > 0 && ts.TotalSeconds > timeOut)
        {
          throw new ExServiceStartException(serviceName, "Timeout was expired.");
        }
      }
    }
    //-------------------------------------------------------------------------

    //---------------------------------------------------------------------------
    /// <summary>
    /// Service start exception.
    /// </summary>
    public class ExServiceStartException : ApplicationException
    {
      //-------------------------------------------------------------------------
      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="serviceName">name of the problem service</param>
      public ExServiceStartException(string serviceName, string errorMessage) :
        base(String.Format("Could not start service '{0}'. {1}", serviceName, errorMessage))
      {
      }
      //-------------------------------------------------------------------------
    }
    //---------------------------------------------------------------------------
  }
}