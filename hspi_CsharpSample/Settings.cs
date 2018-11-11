using HomeSeerAPI;

namespace hspi_CsharpSample
{
	public class Settings
	{
		public Settings(IHSApplication hs)
		{
			_hs = hs;
		}
		private bool _debugLog;
		private readonly IHSApplication _hs;
		private string _location2;
		private string _location;
		private int _timerInterval;
		private bool _logTimerElapsed;

		public bool DebugLog
		{
			get => _debugLog;
			set
			{
				_debugLog = value;
				_hs.SaveINISetting("Settings", "DebugLog", _debugLog.ToString(), Utils.IniFile);
			}
		}

		public string Location
		{
			get => _location;
			set
			{
				_location = value;
				_hs.SaveINISetting("Settings", "Location", _location, Utils.IniFile);
			}
		}

		public string Location2
		{
			get => _location2;
			set
			{
				_location2 = value;
				_hs.SaveINISetting("Settings", "Location2", _location2, Utils.IniFile);
			}
		}

		public bool LogTimerElapsed
		{
			get { return _logTimerElapsed; }
			set
			{
				_logTimerElapsed = value;
				//The following line with save the change directly to the ini file. Useful if you want to avoid using a "Submit" or "Done" button on your settings page
				_hs.SaveINISetting("Settings", "LogTimerElapsed", _logTimerElapsed.ToString(), Utils.IniFile);
			}
		}

		public int TimerInterval
		{
			get => _timerInterval;
			set => _timerInterval = value;
			//... as you see, this value is not automatically saved, and that is because 
			//1) I want the user to be able to discard any changes, and/or
			//2) the timer needs to be updated when this is changed
		}

		public void Load()
		{
			TimerInterval = int.Parse(_hs.GetINISetting("Settings", "TimerInterval", "60000", Utils.IniFile));//Default value is a refresh every minute
			Location = _hs.GetINISetting("Settings", "Location", Utils.PluginName, Utils.IniFile);//I Like it When I can Set a Default location myself
			Location2 = _hs.GetINISetting("Settings", "Location2", Utils.PluginName, Utils.IniFile);
			LogTimerElapsed = bool.Parse(_hs.GetINISetting("Settings", "LogTimerElapsed", "false", Utils.IniFile));
			DebugLog = bool.Parse(_hs.GetINISetting("Settings", "DebugLog", "false", Utils.IniFile));
		}

		public void Save()
		{
			_hs.SaveINISetting("Settings", "TimerInterval", TimerInterval.ToString(), Utils.IniFile);
			//'The following lines could be commented out as the "Set" part of the Property actually saves the settings directly.
			_hs.SaveINISetting("Settings", "Location", Location, Utils.IniFile);
			_hs.SaveINISetting("Settings", "Location2", Location2, Utils.IniFile);
			_hs.SaveINISetting("Settings", "LogTimerElapsed", LogTimerElapsed.ToString(), Utils.IniFile);
			_hs.SaveINISetting("Settings", "DebugLog", DebugLog.ToString(), Utils.IniFile);
		}

	}
}