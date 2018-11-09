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

		static void Main(string[] args)
		{
			_ip = "127.0.0.1";//'"" 'Default is connecting to the local server
			_port = 10400;//Default port

			//Let's check the startup arguments.Here you can set the server location(IP) if you are running the plugin remotely, and set an optional instance name

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

			_appApi = new Hspi();
			Console.WriteLine("Connecting to server at " + _ip + ":" + _port + "...");

			_client = ScsServiceClientBuilder.CreateClient<IHSApplication>(new ScsTcpEndPoint(_ip, _port),
					_appApi);
			_callbackClient = ScsServiceClientBuilder.CreateClient<IAppCallbackAPI>(
					new ScsTcpEndPoint(_ip, _port), _appApi);

			var retryAttempts = 1;
			var isConnected= false;
			do
			{
				try
				{
					
					_client.Connect();
					_callbackClient.Connect();
					_host = _client.ServiceProxy;
					var apiVersion = _host.APIVersion;//will cause an error if not really connected
					_callback = _callbackClient.ServiceProxy;
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

					if (retryAttempts > 5 && isConnected==false)
					{
						_client?.Dispose();
						_client = null;
						_callbackClient?.Dispose();
						_callbackClient = null;
					}
				}
			} while (retryAttempts < 6 && isConnected == false);

			try
			{

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
			//		appAPI = New hspi

			//			Console.WriteLine("Connecting to server at " & ip & "...");

			//		client = ScsServiceClientBuilder.CreateClient(Of IHSApplication)(New ScsTcpEndPoint(ip, 10400), appAPI)

			//		clientCallback = ScsServiceClientBuilder.CreateClient(Of IAppCallbackAPI)(New ScsTcpEndPoint(ip, 10400), appAPI)


			//		Dim Attempts As Integer = 1

			//TryAgain:
			//			Try

			//			client.Connect()

			//			clientCallback.Connect()


			//			host = client.ServiceProxy

			//			Dim APIVersion As Double = host.APIVersion  ' will cause an error if not really connected


			//			callback = clientCallback.ServiceProxy

			//			APIVersion = callback.APIVersion  ' will cause an error if not really connected

			//		Catch ex As Exception

			//			Console.WriteLine("Cannot connect attempt " & Attempts.ToString & ": " & ex.Message)

			//			If ex.Message.ToLower.Contains("timeout occurred.") Then
			//				Attempts += 1

			//				If Attempts< 6 Then GoTo TryAgain
			//		   End If

			//		   If client IsNot Nothing Then

			//				client.Dispose()

			//				client = Nothing

			//			End If

			//			If clientCallback IsNot Nothing Then

			//				clientCallback.Dispose()

			//				clientCallback = Nothing

			//			End If

			//			SleepSeconds(4)

			//			Return
			//		End Try

			//		Try

			//			' create the user object that is the real plugin, accessed from the pluginAPI wrapper

			//			callback = callback


			//		hs = host

			//			' connect to HS so it can register a callback to us

			//			host.Connect(plugin.Name, "")

			//			Console.WriteLine("Connected, waiting to be initialized...")

			//			Do

			//				Threading.Thread.Sleep(30)

			//			Loop While client.CommunicationState = HSCF.Communication.Scs.Communication.CommunicationStates.Connected And Not IsShuttingDown
			//			If Not IsShuttingDown Then

			//				plugin.ShutdownIO()

			//				Console.WriteLine("Connection lost, exiting")

			//			Else

			//				Console.WriteLine("Shutting down plugin")

			//			End If

			//			' disconnect from server for good here

			//			client.Disconnect()

			//			clientCallback.Disconnect()

			//			SleepSeconds(2)

			//			End
			//		Catch ex As Exception

			//			Console.WriteLine("Cannot connect(2): " & ex.Message)

			//			SleepSeconds(2)

			//			End
			//			Return

			//		End Try
		}

		private static void SleepSeconds(int secondsToSleep)
		{
			Thread.Sleep(secondsToSleep * 1000);
		}

		public static IScsServiceClient<IAppCallbackAPI> _callbackClient { get; set; }

		public static IScsServiceClient<IHSApplication> _client { get; set; }
	}
}
