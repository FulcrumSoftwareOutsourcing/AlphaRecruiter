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
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace Framework.Utils
{
	/// <summary>
	/// Class TProfiler should be used to measure performance of execution.
	/// To use TProfiller one should do the following:
	/// 1) Initialize the class (method Init)
	/// 2) Declare static public member TProfiler somewhere ( for example TProfiler Prof = new TProfiler("Block1",-1)). 
	/// 3) To measure perfomance of certain block, invoke BlockBegin() at the beging of the block and BlockEnd() at the end of the block.
	/// 4) Use method Report() or TotReport() to obtain perfomance report.
	/// </summary>
	public class TProfiler
	{
		//aMessage - message 
		public TProfiler( string  aMessage,
					         int  aMesDelay )
		{
           mTotCount = 0;
           mCurCount = 0;
           mElapsedTime = 0;
           mTotElapsedTime = 0;
           mMesDelay   = aMesDelay;
           mMessage = aMessage;	
		
		   if (msInstances==null)
			   msInstances = new ArrayList();

           msInstances.Add(this);
		}

		public void BlockBegin() 
		{	mTotCount++;
			mCurCount++;
			mStartTime = clock();
		}

		public void BlockEnd() 
		{ 
			int End = clock();
			mElapsedTime += End - mStartTime;
			mTotElapsedTime += End - mStartTime;
			if ( mCurCount == mMesDelay ) 
			{
				Report(); 
				mCurCount = 0;
				mElapsedTime = 0;
			}
		}

		public void Report() 
		{
            if (msOStream==null) return;

			string Message = mMessage + " called " +
				             mCurCount +
				             " times total time " +
				             (double)mElapsedTime/1000 + "\n";

			StreamWriter Writer = new StreamWriter(msOStream);
			Writer.Write(Message);
			Writer.Flush();
		}
		
		static public void TotReport()
		{
            if (msOStream==null) return;

            if ( msInstances==null) return;

			StreamWriter Writer = new StreamWriter(msOStream);

			double TotTime;
			string Message="";
			foreach ( TProfiler Prof in msInstances )
            {
               TotTime =  Prof.mTotElapsedTime/(double)1000;
               Message = Prof.mMessage + 
                         " called: " +
                         Prof.mTotCount +
                         " times total time: " + 
                         TotTime + 
                         " time per call: " +
                         TotTime/Prof.mTotCount +
                         "\n";
			   Writer.Write(Message);
            }
			Writer.Flush();
		}
		
		
		static public int Init(string aFileName)
	    {
			if (msOStream!=null) return 1;//All ready initialized

			if (aFileName=="")
				aFileName = "profiler.txt";

			msOStream = new FileStream( aFileName, 
				                        FileMode.OpenOrCreate, 
				                        FileAccess.Write, 
				                        FileShare.Read );
			
		    return 1;
		}

		
		[DllImport("Kernel32.dll", EntryPoint = "GetTickCount")]
		public static extern int clock();

		int mCurCount;
		int mMesDelay;
		int mStartTime;
		int mElapsedTime;

		int mTotCount;
		int mTotElapsedTime;

		string mMessage;
		
		static ArrayList msInstances=null;
		static FileStream msOStream=null;
		
	}
}
