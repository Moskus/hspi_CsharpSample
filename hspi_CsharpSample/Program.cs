using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using HomeSeerAPI;
using Hspi;
using HSCF.Communication.Scs.Communication;
using HSCF.Communication.Scs.Communication.EndPoints.Tcp;
using HSCF.Communication.ScsServices.Client;

//********************************************************************
//********************************************************************
//
// READ ME:
// This file is used to set up the communication with HomeSeer 3.
//
// The only reasons to change something here is when 
// 1. you know what you're doing, and
// 2. you want to add remote plugin capabilities (and stuff like that)
//
// This file does contain some messy code
// (hopefully this comment can be deleted)
//
//********************************************************************
//********************************************************************

namespace HSPI_CsharpSample
{
	class Program
	{
		private static string _instance;
		private static HSPI _appApi;
		private static IHSApplication _host;
		private static IAppCallbackAPI _callback;
		private static Utils _utils;

		static void Main(string[] args)
		{

			//Start from remote with command arguments server for ip and port for port
			Connector.Connect<HSPI>(args);
		}

	}
}
