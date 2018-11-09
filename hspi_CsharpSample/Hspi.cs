using System.Collections.Generic;
using System.Collections.Specialized;
using HomeSeerAPI;

namespace hspi_CsharpSample
{
	//IPlugInAPI - this API is required for ALL plugins
	//IThermostatAPI   ' add this API if this plugin supports thermostats
	public class Hspi: IPlugInAPI
	{
		public int Capabilities()
		{
			throw new System.NotImplementedException();
		}

		public int AccessLevel()
		{
			throw new System.NotImplementedException();
		}

		public bool SupportsMultipleInstances()
		{
			throw new System.NotImplementedException();
		}

		public bool SupportsMultipleInstancesSingleEXE()
		{
			throw new System.NotImplementedException();
		}

		public bool SupportsAddDevice()
		{
			throw new System.NotImplementedException();
		}

		public string InstanceFriendlyName()
		{
			throw new System.NotImplementedException();
		}

		public IPlugInAPI.strInterfaceStatus InterfaceStatus()
		{
			throw new System.NotImplementedException();
		}

		public void HSEvent(Enums.HSEvent EventType, object[] parms)
		{
			throw new System.NotImplementedException();
		}

		public string GenPage(string link)
		{
			throw new System.NotImplementedException();
		}

		public string PagePut(string data)
		{
			throw new System.NotImplementedException();
		}

		public void ShutdownIO()
		{
			throw new System.NotImplementedException();
		}

		public bool RaisesGenericCallbacks()
		{
			throw new System.NotImplementedException();
		}

		public void SetIOMulti(List<CAPI.CAPIControl> colSend)
		{
			throw new System.NotImplementedException();
		}

		public string InitIO(string port)
		{
			throw new System.NotImplementedException();
		}

		public IPlugInAPI.PollResultInfo PollDevice(int dvref)
		{
			throw new System.NotImplementedException();
		}

		public bool SupportsConfigDevice()
		{
			throw new System.NotImplementedException();
		}

		public bool SupportsConfigDeviceAll()
		{
			throw new System.NotImplementedException();
		}

		public Enums.ConfigDevicePostReturn ConfigDevicePost(int @ref, string data, string user, int userRights)
		{
			throw new System.NotImplementedException();
		}

		public string ConfigDevice(int @ref, string user, int userRights, bool newDevice)
		{
			throw new System.NotImplementedException();
		}

		public SearchReturn[] Search(string SearchString, bool RegEx)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// This will allow scripts to call any function in your plugin by name.
		/// Used to allow scripting of your plugin functions.
		/// </summary>
		/// <param name="proc">The name of the process/sub/function to call</param>
		/// <param name="parms">Parameters passed as an object array</param>
		/// <returns></returns>
		/// <remarks>http://homeseer.com/support/homeseer/HS3/SDK/custom_functions.htm</remarks>
		public object PluginFunction(string procName, object[] parms)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// This will allow scripts to call any property in your plugin by name.
		/// Used to allow scripting of your plugin properties.
		/// </summary>
		/// <param name="proc">The name of the Property to get</param>
		/// <param name="parms">Parameters passed as an object array</param>
		/// <returns></returns>
		/// <remarks>http://homeseer.com/support/homeseer/HS3/SDK/custom_functions.htm</remarks>
		public object PluginPropertyGet(string procName, object[] parms)
		{
			throw new System.NotImplementedException();
		}

		
		/// <summary>
		/// This will allow scripts to set any property in your plugin by name.
		/// Used to allow scripting of your plugin properties.
		/// </summary>
		/// <param name="proc">The name of the property to set</param>
		/// <param name="value">The new value for the property</param>
		/// <remarks>http://homeseer.com/support/homeseer/HS3/SDK/custom_functions.htm</remarks>
		public void PluginPropertySet(string procName, object value)
		{
			throw new System.NotImplementedException();
		}

		public void SpeakIn(int device, string txt, bool w, string host)
		{
			throw new System.NotImplementedException();
		}

		public int ActionCount()
		{
			throw new System.NotImplementedException();
		}

		public bool ActionConfigured(IPlugInAPI.strTrigActInfo ActInfo)
		{
			throw new System.NotImplementedException();
		}

		public string ActionBuildUI(string sUnique, IPlugInAPI.strTrigActInfo ActInfo)
		{
			throw new System.NotImplementedException();
		}

		public IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection PostData, IPlugInAPI.strTrigActInfo TrigInfoIN)
		{
			throw new System.NotImplementedException();
		}

		public string ActionFormatUI(IPlugInAPI.strTrigActInfo ActInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool ActionReferencesDevice(IPlugInAPI.strTrigActInfo ActInfo, int dvRef)
		{
			throw new System.NotImplementedException();
		}

		public bool HandleAction(IPlugInAPI.strTrigActInfo ActInfo)
		{
			throw new System.NotImplementedException();
		}

		public string TriggerBuildUI(string sUnique, IPlugInAPI.strTrigActInfo TrigInfo)
		{
			throw new System.NotImplementedException();
		}

		public IPlugInAPI.strMultiReturn TriggerProcessPostUI(NameValueCollection PostData, IPlugInAPI.strTrigActInfo TrigInfoIN)
		{
			throw new System.NotImplementedException();
		}

		public string TriggerFormatUI(IPlugInAPI.strTrigActInfo TrigInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool TriggerTrue(IPlugInAPI.strTrigActInfo TrigInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool TriggerReferencesDevice(IPlugInAPI.strTrigActInfo TrigInfo, int dvRef)
		{
			throw new System.NotImplementedException();
		}

		public string GetPagePlugin(string page, string user, int userRights, string queryString)
		{
			throw new System.NotImplementedException();
		}

		public string PostBackProc(string page, string data, string user, int userRights)
		{
			throw new System.NotImplementedException();
		}
		/// <summary>
		/// Returns the name of your plug-in. This is used to identify your plug-in to HomeSeer and your users. Keep the name to 16 characters or less.
		/// Do not access any hardware in this function as HomeSeer will call this function.
		/// Do NOT use special characters in your plug-in name with the exception of "-", ".", and " " (space).
		/// </summary>
		/// <returns>Plugin name</returns>
		/// <remarks>http://homeseer.com/support/homeseer/HS3/SDK/name.htm</remarks>
		public string Name { get; }

		public bool HSCOMPort { get; }
		public bool ActionAdvancedMode { get; set; }
		public string get_ActionName(int ActionNumber)
		{
			throw new System.NotImplementedException();
		}

		public bool get_HasConditions(int TriggerNumber)
		{
			throw new System.NotImplementedException();
		}

		public bool HasTriggers { get; }
		public int TriggerCount { get; }
		public string get_TriggerName(int TriggerNumber)
		{
			throw new System.NotImplementedException();
		}

		public int get_SubTriggerCount(int TriggerNumber)
		{
			throw new System.NotImplementedException();
		}

		public string get_SubTriggerName(int TriggerNumber, int SubTriggerNumber)
		{
			throw new System.NotImplementedException();
		}

		public bool get_TriggerConfigured(IPlugInAPI.strTrigActInfo TrigInfo)
		{
			throw new System.NotImplementedException();
		}

		public bool get_Condition(IPlugInAPI.strTrigActInfo TrigInfo)
		{
			throw new System.NotImplementedException();
		}

		public void set_Condition(IPlugInAPI.strTrigActInfo TrigInfo, bool Value)
		{
			throw new System.NotImplementedException();
		}
	}
}