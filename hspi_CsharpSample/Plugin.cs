using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;

namespace hspi_CsharpSample
{
	public class Plugin
	{
		private Utils _utils;
		private Settings _settings;
		private readonly string ConfigPageName=Utils.PluginName+"Config";
		private readonly string StatusPageName = Utils.PluginName+ "Status";

		public Plugin()
		{
		}

		public Plugin(Utils utils)
		{
			_utils = utils;
		}

		public string Name => Utils.PluginName;

		public Utils Utils
		{
			get => _utils;
			set => _utils = value;
		}

		public Settings Settings
		{
			get =>_settings;
			set => _settings = value;
		}

		public void ShutDownIo()
		{
			throw new System.NotImplementedException();
		}

		public string InitIO(string port)
		{
			Console.WriteLine("Starting initializiation.");
			//Loading settings before we do anything else
			_settings.Load();
			//Registering two pages
			_utils.RegisterWebPage(ConfigPageName, "Config", "Configuration");
			_utils.RegisterWebPage(StatusPageName, "", "Demo test");

			//'Adding a trigger 
			//triggers.Add(CObj(Nothing), "Random value is lower than")
			//'Adding a second trigger with subtriggers
			//'... so first let us create the subtriggers
			//Dim subtriggers As New Trigger
			//subtriggers.Add(CObj(Nothing), "Random value is lower than")
			//subtriggers.Add(CObj(Nothing), "Random value is equal to")
			//subtriggers.Add(CObj(Nothing), "Random value is higher than")

			//'... and then the trigger with the subtriggers
			//triggers.Add(subtriggers, "Random value is...")

			//'Adding an action
			//actions.Add(CObj(Nothing), "Send a custom command somewhere")

			//'Checks if plugin devices are present, and create them if not.
			//	CheckAndCreateDevices()

			//'Starting the update timer; a timer for fetching updates from the web (for example). However, in this sample, the UpdateTimerTrigger just generates a random value. (Should ideally be placed in its own thread, but I use a Timer here for simplicity).
			//updateTimer = New Threading.Timer(AddressOf UpdateRandomValue, Nothing, Timeout.Infinite, Me.Settings.TimerInterval)
			//RestartTimer()


			Console.WriteLine("Initializing done! Ready...");

			return "";
		}
	}
}