using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Hspi
{
	public static class Connector
	{
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
			Justification = "The function wouldn't do anything without a plugin.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "I don't know what kinds of exceptions it _could_ throw.")]
		public static void Connect<TPlugin>(string[] args) where TPlugin : HspiBase, new()
		{
			var parsedArguments = ParseArguments(args);
			var myPlugin = new TPlugin();
			Console.WriteLine(myPlugin.Name);

			if (Environment.UserInteractive)
			{
				Console.Title = myPlugin.Name;
			}

			// Get our plugin to connect to HomeSeer
			Console.WriteLine($"\nConnecting to HomeSeer at {parsedArguments.Ip}:{parsedArguments.Port} ...");
			try
			{
				myPlugin.Connect(parsedArguments.Ip, parsedArguments.Port);

				// got this far then success
				Console.WriteLine("  connection to HomeSeer successful.\n");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"  connection to HomeSeer failed: {ex.Message}");
				return;
			}

			// let the plugin do it's thing, wait until it shuts down or the connection to HomeSeer fails.
			try
			{
				while (true)
				{
					// do nothing for a bit
					Thread.Sleep(200);

					// test the connection to HomeSeer
					if (!myPlugin.Connected)
					{
						Console.WriteLine("Connection to HomeSeer lost, exiting");
						break;
					}

					// test for a shutdown signal
					if (myPlugin.Shutdown)
					{
						Console.WriteLine("Plugin has been shut down, exiting");
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unhandled exception from Plugin: {ex.Message}");
			}

			//});
			Environment.Exit(0);
		}

		private static ArgumentObject ParseArguments(string[] args)
		{
			var argObject=new ArgumentObject();
			foreach (var argument in args)
			{
				var parts = argument.Split('=');
				switch (parts[0])
				{
					case "port":
						int tempPort = 0;
						if (int.TryParse(parts[1], out tempPort))
						{
							argObject.Port= tempPort;
						}
						break;
					case "server":
						argObject.Ip = parts[1];
						break;
					case "instance":
						try
						{
							//Not in use as it is now
							argObject.Instance = parts[1];
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							argObject.Instance = "";
						}
						break;
				}
			}

			return argObject;

		}
	}

	public class ArgumentObject
	{
		public string  Ip { get; set; }
		public int Port { get; set; }
		public string Instance { get; set; }

		public ArgumentObject()
		{
			//Default arguments
			Ip = "127.0.0.1";
			Port = 10400;
		}
	}
}