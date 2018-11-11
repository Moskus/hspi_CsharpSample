using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using HomeSeerAPI;
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

namespace hspi_CsharpSample
{
	class Program
	{
		private static string _instance;
		private static int _port;
		private static Hspi _appApi;
		private static string _ip;
		private static IHSApplication _host;
		private static IAppCallbackAPI _callback;
		private static Utils _utils;

		static void Main(string[] args)
		{
			_ip = "127.0.0.1";//Default ip connecting to the local server
			_port = 10400;//Default port
			
			//Let's check the startup arguments.Here you can set the server
			//location(IP) and port if you are running the plugin remotely,
			//and set an optional instance name
			foreach (var argument in args)
			{
				var parts = argument.Split('=');
				switch (parts[0])
				{
					case "port":
						_port = 10400;
						int tempPort = 0;
						if (int.TryParse(parts[1], out tempPort))
						{
							_port = tempPort;
						}
						break;
					case "server":
						_ip = parts[1];
						break;
					case "instance":
						try
						{
							_instance = parts[1];
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							_instance = "";
						}
						break;
				}
			}

			//var settings = new Settings();
			//_utils = new Utils(settings);

			//var plugin = new Plugin(_utils);
			var plugin = new Plugin();
			_appApi = new Hspi(plugin);
			Console.WriteLine("Connecting to server at " + _ip + ":" + _port + "...");

			_client = ScsServiceClientBuilder.CreateClient<IHSApplication>(new ScsTcpEndPoint(_ip, _port),
					_appApi);
			_clientCallback = ScsServiceClientBuilder.CreateClient<IAppCallbackAPI>(
					new ScsTcpEndPoint(_ip, _port), _appApi);
			_client.Disconnected += Client_Disconnected;

			var retryAttempts = 1;
			var isConnected = false;
			do
			{
				try
				{

					_client.Connect();
					_clientCallback.Connect();
					_host = _client.ServiceProxy;
					var apiVersion = _host.APIVersion;//will cause an error if not really connected
					_callback = _clientCallback.ServiceProxy;
					apiVersion = _callback.APIVersion;//will cause an error if not really connected
					isConnected = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine("Cannot connect! Attempt " + retryAttempts.ToString() + ": " + ex.Message);

					if (ex.Message.ToLower().Contains("timeout occurred."))
					{
						retryAttempts++;
						SleepSeconds(4);
					}

					if (retryAttempts > 5 && isConnected == false)
					{
						_client?.Dispose();
						_client = null;
						_clientCallback?.Dispose();
						_clientCallback = null;
					}
				}
			} while (retryAttempts < 6 && isConnected == false);

			try
			{
				//create the user objects that is the real plugin, accessed from the pluginAPI wrapper
				var settings = new Settings(_host);
				_utils = new Utils(settings);
				_utils.Callback = _callback;
				_appApi.Utils = _utils;
				Utils.Hs = _host;
				plugin.Settings = settings;
				plugin.Utils= _utils;
				// connect to HS so it can register a callback to us
				_host.Connect(Utils.PluginName, "");
				Console.WriteLine("Connected, waiting to be initialized...");
				do
				{
					Thread.Sleep(30);
				} while (_client.CommunicationState ==
						 HSCF.Communication.Scs.Communication.CommunicationStates.Connected && !Utils.IsShuttingDown);
				if(!Utils.IsShuttingDown)
				{
					plugin.ShutDownIo();
					Console.WriteLine("Connection lost, exiting");
				}
				else
				{
					Console.WriteLine("Shutting down plugin");
				}
				//disconnect from server for good here
				_client.Disconnect();
				_clientCallback.Disconnect();
				SleepSeconds(2);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Cannot connect(2): " + ex.Message);
				SleepSeconds(2);
				Environment.Exit(-1);
				return;
			}
		}

		private static void Client_Disconnected(object sender, EventArgs e)
		{
			Console.WriteLine("Disconnected from server - client");
		}

		private static void SleepSeconds(int secondsToSleep)
		{
			Thread.Sleep(secondsToSleep * 1000);
		}

		public static IScsServiceClient<IAppCallbackAPI> _clientCallback { get; set; }

		public static IScsServiceClient<IHSApplication> _client { get; set; }
	}
}
