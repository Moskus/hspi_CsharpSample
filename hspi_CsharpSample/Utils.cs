using System;
using HomeSeerAPI;
using Scheduler;

namespace hspi_CsharpSample
{
	public class Utils
	{
		public static string PluginName = "Hspi_CsharpSample";
		public static string PluginInstance = "";
		public static string IniFile = "Hspi_CsharpSample.ini";

		private static bool _isShuttingDown = false;
		private readonly Settings _settings;

		public Utils(Settings settings)
		{
			_settings = settings;
		}

		public static bool IsShuttingDown
		{
			get => _isShuttingDown;
			set => _isShuttingDown = value;
		}
		public IAppCallbackAPI Callback { get; set; }
		public static IHSApplication Hs { get; set; }
		
		public void RegisterWebPage(string link, string linkText = "", string pageTitle = "")
		{
			try
			{
				var theLink = link;
				Hs.RegisterPage(theLink, Utils.PluginName, Utils.PluginInstance);

				if (string.IsNullOrEmpty(linkText))
				{
					linkText = link;
				}
				linkText = linkText.Replace("_", " ").Replace(Utils.PluginName, "");

				if (string.IsNullOrEmpty(pageTitle))
				{
					pageTitle = linkText;
				}

				var webPageDescription = new HomeSeerAPI.WebPageDesc();
				webPageDescription.plugInName = Utils.PluginName;

				webPageDescription.link = theLink;

				webPageDescription.linktext = linkText + Utils.PluginInstance;
				webPageDescription.page_title = pageTitle + Utils.PluginInstance;
				Callback.RegisterLink(webPageDescription);

			}
			catch (Exception ex)
			{
				Log("Registering Web Links (RegisterWebPage): " + ex.Message, LogType.Error);
			}
		}

		public void Log(string message, LogType logLevel = LogType.Normal)
		{
			switch (logLevel)
			{
				case LogType.Debug:
					if (_settings.DebugLog)
					{
						Hs.WriteLog(Utils.PluginName + " Debug", message);
					}
					break;
				case LogType.Normal:
					Hs.WriteLog(Utils.PluginName, message);
					break;


				case LogType.Warning:
					Hs.WriteLog(Utils.PluginName + " Warning", message);
					break;

				case LogType.Error:
					Hs.WriteLog(Utils.PluginName + " ERROR", message);
					break;
			}
		}

		public void SaveSettings()
		{
			_settings.Save();
		}
	}
}