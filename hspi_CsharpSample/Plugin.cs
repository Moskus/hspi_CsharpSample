using System;
using System.Threading;

namespace hspi_CsharpSample
{
	public class Plugin
	{
		public string Name => Utils.IFACE_NAME;

		public void ShutDownIo()
		{
			throw new System.NotImplementedException();
		}

		public string InitIO(string port)
		{
			Console.WriteLine("Starting initializiation.");
			//'Loading settings before we do anything else
			//Me.Settings = New Settings
			//'Registering two pages
			//RegisterWebPage(link:= configPageName, linktext:= "Config", page_title:= "Configuration")
			//RegisterWebPage(link:= statusPageName, linktext:= "", page_title:= "Demo test")
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