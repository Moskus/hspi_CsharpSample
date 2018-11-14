using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using hspi_CsharpSample.HomeSeerClasses;
using HomeSeerAPI;
using Scheduler;

//***************************************************************
//***************************************************************
//
//READ ME:
// In this file most of the magic happens, and a great place to start reading. :)
//
//FAQ
// Q: "What does this plugin do?"
// A: Nothing very useful, it just does "something" to get you started. Here's a quick summary:
//    1. It generates a random value every X minutes or seconds (this Is user changeable by a settings page). This is used to show how triggers and conditions work. See the region "Trigger Interface".
//
//   2. It generates different types of devices, just to show how that could be done and how the plugin reacts to usage of the different controls (CAPI).  See the sub "CheckAndCreateDevices".
//
//   3. It shows how text, values and even custom objects can be stored in a device, and how you create your own "Tab" on the device settings. See subs "ConfigDevice" and "ConfigDevicePost"
//
//   4. It has a simple settings page that demonstrate how you can update settings for your plugin.
//
//
// Q: "What do I need?"
// A: You need Visual Studio (Visual Studio Community Edition should work just fine), HomeSeer 3 running either locally Or On a remote computer, some spare time (the more, the better), And an appetite For coding.
//
//
// Q: "Where do I start?"
// A: 1. Copy "HomeSeerAPI.dll", "HSCF.dll" and "Scheduler.dll" from your HS3 dir to into the dir of this project
//    2. Just start HS3 if it's not running, and Debug this solution.Then you'll see what's going on
//    1. Do some customization, I suggest start by naming your plugin. this is done several places:
//         a. In "utils.vb", see variables "IFACE_NAME" and "INIFILE".
//         b. See in "My Project" in the Solution, and change both
//         c. If you, like me, don't want the solution and project named "MoskusSample" you can edit the .vbproj and .sln files in notepad (but close Visual Studio first).
//    2. Then look into the "Plugin.vb" file. A good place like any other is to start by finding the sub "InitIO", what's where the plugin is initialized.
//    3. However, if you REALLY want to dive right in to it, find the "UpdateTimerTrigger" sub and read to the end.
//
//
// Good luck! And ask a question if you want to! :)
// See the this thread: http://board.homeseer.com/showthread.php?p=1204792
//
// Best regards,
// Moskus
//
//
// If you want, you can always "tip" me via Paypal to "moskus a_t gmail d_o_t com" (where "a_t" = "@" and "d_o_t" = ".")
// It is not expected or required, but highly appreciated. :)
//***************************************************************
//***************************************************************


namespace hspi_CsharpSample
{
	public class Plugin
	{
		private Utils _utils;
		private Settings _settings;
		private readonly string ConfigPageName = Utils.PluginName + "Config";
		private readonly string StatusPageName = Utils.PluginName + "Status";
		private int _lastRandomNumber;
		private Timer _updateTimer;
		private HsCollection _actions = new HsCollection();
		private HsCollection _triggers = new HsCollection();

		private HsTrigger _trigger = new HsTrigger();
		private HsAction _action = new HsAction();
		private IHSApplication _hs;
		private const string Pagename = "Events";

		public Plugin()
		{
		}

		public Timer UpdateTimer => _updateTimer;

		public string Name => Utils.PluginName;

		public Utils Utils
		{
			get => _utils;
			set
			{
				_utils = value;
				_hs = Utils.Hs;
			}
		}

		public Settings Settings
		{
			get => _settings;
			set => _settings = value;
		}
		public int AccessLevel { get; internal set; }

		#region "Init"

		public string InitIO(string port)
		{
			Console.WriteLine("Starting initializiation.");
			//Loading settings before we do anything else
			_settings.Load();
			//Registering two pages
			_utils.RegisterWebPage(ConfigPageName, "Config", "Configuration");
			_utils.RegisterWebPage(StatusPageName, "", "Demo test");

			//Adding a trigger 
			_triggers.Add(null, "Random value is lower than");
			//Adding a second trigger with subtriggers
			//... so first let us create the subtriggers
			var subtriggers = new HsTrigger();
			subtriggers.Add(null, "Random value is lower than");
			subtriggers.Add(null, "Random value is equal to");
			subtriggers.Add(null, "Random value is higher than");

			//... and then the trigger with the subtriggers
			_triggers.Add(subtriggers, "Random value is...");
			//Adding an action
			_actions.Add(null, "Send a custom command somewhere");

			//Checks if plugin devices are present, and create them if not.
			CheckAndCreateDevices();

			//'Starting the update timer; a timer for fetching updates from the web (for example). However, in this sample, the UpdateTimerTrigger just generates a random value. (Should ideally be placed in its own thread, but I use a Timer here for simplicity).
			_updateTimer = new System.Threading.Timer(new TimerCallback(UpdateRandomValue), null, Timeout.Infinite,
				_settings.TimerInterval);
			RestartTimer();

			Console.WriteLine("Initializing done! Ready...");

			return "";
		}

		#endregion

		///<summary>
		///Generate a new random value, check which triggers should be triggered, and update device values.
		///</summary>
		///<remarks>By Moskus</remarks>
		private void UpdateRandomValue(Object obj)
		{
			//************
			//Random value
			//************
			//We need some data. So we're just creating a random value

			//Let's make a nice random number between 0 and 100
			Random rnd = new Random(DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +
									DateTime.Now.Millisecond);

			var randomValue = rnd.Next(100);

			//The triggers can be used as an Condition, so we need to store the last value. "lastRandomNumber" is a class global variable.
			_lastRandomNumber = randomValue;

			//Let's write this random value to the log so that we can see what's going on, if the user has opted to do so
			if (_settings.LogTimerElapsed)
			{
				_utils.Log("Timer elapsed. Huzza! New random value: " + randomValue);
			}


			//******
			//Events
			//******
			//Getting all triggers for this plugin (this only returns triggers where it is the FIRST option in an event, not when it's used as a condition)
			HomeSeerAPI.IPlugInAPI.strTrigActInfo[] triggers = _utils.Callback.GetTriggers(Utils.PluginName);
			Console.WriteLine("UpdateTimer_Elapsed." + "\tTriggers found: " + triggers.Count() + "\tRandom value: " +
							  randomValue);

			//Checking reach event with triggers from our plugin if they should be triggered or not
			foreach (var trigger in triggers)
			{
				//The name of the key we are looking for
				string key = "SomeValue";

				//Get the value from the trigger
				int triggerValue = (int)GetTriggerValue(key, trigger);
				//Console.WriteLine("Value found for " & key & ": " & triggervalue) '... for debugging

				//Remember from TriggerBuildUI that if "someValue" is equal to "-1", we don't have a properly configured trigger, so we need to skip this if this(the current "t") value is -1
				if (triggerValue == -1)
				{
					Console.WriteLine("Event with ref = " + trigger.evRef + " is not properly configured.");
					continue; // For
				}

				//If not, we do have a working trigger, so let's continue
				//We have the option to select between two triggers, so we need to check them both
				switch (trigger.TANumber)
				{
					case (int)TriggerTypes.WithoutSubtriggers: //= 1. The trigger without subtriggers
															   //If the test is true, then trig the Trigger

						if (triggerValue >= randomValue)
						{
							Console.WriteLine("Trigging event with reference " + trigger.evRef);
							TriggerFire(trigger);
						}

						break;


					case (int)TriggerTypes.WithSubtriggers: //= 2. The trigger with subtriggers
															//We have multiple options for checking for values, they are specified by the subtrigger number (1-based)
						switch (trigger.SubTANumber)
						{
							case (int)SubTriggerTypes.LowerThan
								: //The random value should be lower than the value specified in the event

								if (triggerValue >= randomValue)
								{
									Console.WriteLine(
										"Value is lower. Trigging event with reference " + trigger.evRef);
									TriggerFire(trigger);
								}

								break;
							case (int)SubTriggerTypes.EqualTo
								: //The random value should be equal to the value specified in the event
								if (triggerValue == randomValue)
								{
									Console.WriteLine("Value is equal. Trigging event with reference " + trigger.evRef);
									TriggerFire(trigger);
								}

								break;
							case (int)SubTriggerTypes.HigherThan
								: //The random value should be higher than the value specified in the event
								if (triggerValue <= randomValue)
								{
									Console.WriteLine("Value is higher. Trigging event with reference " +
													  trigger.evRef);

									TriggerFire(trigger);
								}

								break;
							default:
								_utils.Log("Undefined subtrigger!");
								break;
						}

						break;
					default:
						_utils.Log("Undefined trigger!");
						break;
				}
			}







			//*******
			//DEVICES
			//*******
			//We get all the devices (and Linq is awesome!)
			var devices = _utils.DevicesOnlyForPlugin();
			//In this example there are not any external sources that should update devices, so we're just updating the device value of the basic device and setting it to the new random value.

			//We do this for each "Basic" device we have (usually just one, but still...)
			foreach (var device in devices.Where(x => x.get_Device_Type_String(_hs).Contains("Basic")))
			{
				_hs.SetDeviceValueByRef(device.get_Ref(_hs), randomValue, true);
				_hs.SetDeviceString(device.get_Ref(_hs), "Last random value: " + randomValue, false);
			}

			//... but of course we can do cooler stuff with our devices than that. E.g. add the text stored in the "Advanced Device" to the device string, for example

			//Again, for all "Advanced" devices
			foreach (var device in devices.Where(x => x.get_Device_Type_String(_hs).Contains("Advanced")))
			{
				//Get the PlugExtraData class stored in the dev
				PlugExtraData.clsPlugExtraData ped = device.get_PlugExtraData_Get(_hs);

				//... but we can only do something if there actually IS some PlugExtraData

				if (ped != null)
				{
					//Get the value belonging to our plugin from the devices PlugExtraData 

					var savedString = _utils.PedGet(ref ped, Utils.PluginName);

					//TODO: Do something with the saved string and the random value?
					//hs.SetDeviceValueByRef(dev.Ref(hs), randomvalue, True)
					//hs.SetDeviceString(dev.Ref(hs), savedString & " - with value: " & randomvalue, True)
				}
			}
		}

		#region "Trigger Interface"

		///<summary>
		///MoskusSample Enum to determine which type of trigger we have
		///</summary>
		private enum TriggerTypes
		{
			WithoutSubtriggers = 1,
			WithSubtriggers = 2
		}


		///<summary>
		///MoskusSample Enum to get the subtrigger, if the current trigger type = TriggerTypes.WithSubTriggers (2) for the current Event.
		///</summary>
		private enum SubTriggerTypes
		{
			LowerThan = 1,
			EqualTo = 2,
			HigherThan = 3
		}

		///<summary>
		///This function is a callback function and is called when a plugin detects that a trigger condition is true.
		///
		///Moskus: This is the sub that actually triggers the event.
		///</summary>
		///<param name="TrigInfo">The TrigInfo structure of the trigger that is triggering</param>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/triggerfire1.htm</remarks>
		public void TriggerFire(HomeSeerAPI.IPlugInAPI.strTrigActInfo trigInfo)
		{
			try
			{
				_utils.Callback.TriggerFire(Utils.PluginName, trigInfo);

				strEventData foundEvent = _utils.Events()
					.SingleOrDefault(x =>
						x.Event_Ref ==
						trigInfo.evRef); //(From ee In Events() Where ee.Event_Ref = trigInfo.evRef).First.Event_Name) ;
				Console.WriteLine("TriggerFire: Fired on event with ref = " + trigInfo.evRef +
								  ("  (name: " + foundEvent.Event_Name + ")"));
			}
			catch (Exception ex)
			{
				_utils.Log("Error while running trigger: " + ex.Message, LogType.Error);
			}
		}

		///<summary>
		///Triggers notify HomeSeer of trigger states using TriggerFire , but Triggers can also be conditions, and that is where this is used.
		///If this function is called, TrigInfo will contain the trigger information pertaining to a trigger used as a condition.
		///
		///Moskus: This is the function that determines if your trigger is true or false WHEN USED AS A CONDITION.
		///</summary>
		///<param name="TrigInfo">The trigger information</param>
		///<returns>True/False</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/triggertrue.htm</remarks>

		private bool TriggerTrue(IPlugInAPI.strTrigActInfo trigInfo)
		{
			//Let's specify the key name of the value we are looking for
			var key = "SomeValue";

			//Get the value from the trigger
			var triggervalue = (int)GetTriggerValue(key, trigInfo);

			Console.WriteLine("Conditional value found for " + key + ": " + triggervalue + "\tLast random: " +
							  _lastRandomNumber);

			//Let's return if this condition is True or False
			return (triggervalue >= _lastRandomNumber);

		}

		///<summary>
		///Given a IPlugInAPI.strTrigActInfo object detect if this this trigger is configured properly, if so, return True, else False.
		///</summary>
		///<param name="TrigInfo">Trigger information of "strTrigActInfo" (which is funny, as it isn't a string at all)[No, it is a struct :-)]</param>
		///<returns>True/False</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/triggerconfigured.htm</remarks>
		public bool TriggerConfigured(HomeSeerAPI.IPlugInAPI.strTrigActInfo trigInfo)
		{
			//var itemsConfigured = 0;
			//var itemsToConfigure = 1;

			var uid = trigInfo.UID.ToString();
			object triggerObject = null;

			if (trigInfo.DataIn != null)
			{
				_utils.DeSerializeObject(ref trigInfo.DataIn, ref triggerObject);
			}

			if (triggerObject == null) return false;
			HsTrigger trigger = (HsTrigger)triggerObject;
			var foundKey = trigger.GetAllKeys().FirstOrDefault(x => x == "SomeValue_" + uid);

			//todo double check the logic here
			if (!string.IsNullOrEmpty((string)trigger[foundKey]))
				return true;

			//	For Each key As String In trigger.Keys
			//	Select Case True
			//Case key.Contains("SomeValue_" & UID) AndAlso trigger(key) <> ""
			//itemsConfigured += 1
			//End Select
			//Next
			//	If itemsConfigured = itemsToConfigure Then Return True
			//End If

			return false;
		}

		//<summary>
		//Return HTML controls for a given trigger.
		//</summary>
		//<param name="sUnique">Apparently some unique string</param>
		//<param name="TrigInfo">Trigger information</param>
		//<returns>Return HTML controls for a given trigger.</returns>
		//<remarks>http://homeseer.com/support/homeseer/HS3/SDK/triggerbuildui.htm</remarks>
		public string TriggerBuildUI(string uniqueString, HomeSeerAPI.IPlugInAPI.strTrigActInfo trigInfo)
		{

			var uid = trigInfo.UID.ToString();

			var stb = new StringBuilder();
			var someValue = -1;//We'll set the default value.This value will indicate that this trigger isn't properly configured

			var dd = new clsJQuery.jqDropList("SomeValue_" + uid + uniqueString, Pagename, true);
			dd.autoPostBack = true;
			dd.AddItem("--Please Select--", "-1", false);//A selected option with the default value (-1) means that the trigger isn't configured
			HsTrigger trigger = null;
			object triggerObject = null;
			if (trigInfo.DataIn != null)
			{
				_utils.DeSerializeObject(ref trigInfo.DataIn, ref triggerObject);
				if (triggerObject != null)
				{
					trigger = (HsTrigger)triggerObject;
				}
			}
			else //new event, so clean out the trigger object
			{
				trigger = new HsTrigger();
			}

			var foundKey = trigger.GetAllKeys().SingleOrDefault(x => x.Contains("SomeValue_" + uid));
			if (!string.IsNullOrEmpty(foundKey))
			{
				someValue = (int)trigger[foundKey];
			}

			//We'll add all the different selectable values(numbers from 0 to 100 with 10 in increments)
			//and we'll select the option that was selected before if it's an old value (see ("i = someValue") which will be true or false)
			for (int i = 0; i < 100; i += 10)
			{
				dd.AddItem(i.ToString(), i.ToString(), (i == someValue));
			}

			//Finally we'll add this to the stringbuilder, and return the value
			stb.Append("Select value:");
			stb.Append(dd.Build());
			return stb.ToString();
		}


		//''' <summary>
		//''' Process a post from the events web page when a user modifies any of the controls related to a plugin trigger. After processing the user selctions, create and return a strMultiReturn object.
		//''' </summary>
		//''' <param name="PostData">The PostData as NameValueCollection</param>
		//''' <param name="TrigInfo">The trigger information</param>
		//''' <returns>A structure, which is used in the Trigger and Action ProcessPostUI procedures, which not only communications trigger and action information through TrigActInfo which is IPlugInAPI.strTrigActInfo , but provides an array of Byte where an updated/serialized trigger or action object from your plug-in can be stored.  See TriggerProcessPostUI and ActionProcessPostUI for more details.</returns>
		//''' <remarks>http://homeseer.com/support/homeseer/HS3/SDK/triggerprocesspostui.htm</remarks>
		public HomeSeerAPI.IPlugInAPI.strMultiReturn TriggerProcessPostUI(System.Collections.Specialized.NameValueCollection postData,
			HomeSeerAPI.IPlugInAPI.strTrigActInfo trigInfo)
		{
			var ret = new HomeSeerAPI.IPlugInAPI.strMultiReturn();
			var uid = trigInfo.UID.ToString();

			ret.sResult = "";
			// HST: We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
			//   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
			//   we can still do that
			ret.DataOut = trigInfo.DataIn;
			ret.TrigActInfo = trigInfo;

			if (postData == null) return ret;
			if (postData.Count < 1) return ret;

			object triggerObject = null;
			HsTrigger trigger = null;
			if (trigInfo.DataIn != null)
			{
				_utils.DeSerializeObject(ref trigInfo.DataIn, ref triggerObject);
				if (triggerObject != null)
				{
					trigger = (HsTrigger)triggerObject;
				}
			}

			System.Collections.Specialized.NameValueCollection parts;
			parts = postData;
			try
			{
				foreach (string key in parts)
				{
					if (string.IsNullOrEmpty(key))
						continue;
					if (key.Contains("SomeValue_" + uid))
					{
						trigger.Add((object)parts[key], key);
					}
				}

				if (!_utils.SerializeObject(trigger, ref ret.DataOut))
				{
					ret.sResult = Utils.PluginName + " Error, Serialization failed. Signal Trigger not added.";
					return ret;
				}
			}
			catch (Exception ex)
			{
				ret.sResult = "ERROR, Exception in Trigger UI of " + Utils.PluginName + ": " + ex.Message;
				return ret;
			}


			//All OK
			ret.sResult = "";
			return ret;

		}


		///<summary>
		///After the trigger has been configured, this function is called in your plugin to display the configured trigger. Return text that describes the given trigger.
		///</summary>
		///<param name="TrigInfo">Information of the trigger</param>
		///<returns>MediaTypeNames.Text describing the trigger</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/triggerformatui.htm</remarks>
		public string TriggerFormatUI(HomeSeerAPI.IPlugInAPI.strTrigActInfo trigInfo)
		{
			var stb = new StringBuilder();
			//string key;
			string someValue = "";
			string uid = trigInfo.UID.ToString();
			object triggerObject = null;
			HsTrigger trigger = null;
			if (trigInfo.DataIn != null)
			{
				_utils.DeSerializeObject(ref trigInfo.DataIn, ref triggerObject);
			}

			if (triggerObject != null)
			{
				trigger = (HsTrigger)triggerObject;
			}

			var foundKey = trigger.GetAllKeys().FirstOrDefault(x => x == "SomeValue_" + uid);
			if (string.IsNullOrEmpty(foundKey)) return string.Empty;

			someValue = (string)trigger[foundKey];

			//We need different texts based on which trigger was used.
			switch (trigInfo.TANumber)
			{
				case (int)TriggerTypes.WithoutSubtriggers://= 1. The trigger without subtriggers only has one option:
					stb.Append(" the random value generator picks a number lower than ");
					stb.Append(someValue);
					break;

				case (int)TriggerTypes.WithSubtriggers:
					//let's start with the regular text for the trigger
					stb.Append(" the random value generator picks a number ");

					//... add the comparer (all subtriggers for the current trigger)
					switch (trigInfo.SubTANumber)
					{
						case (int)SubTriggerTypes.LowerThan://= 1
							stb.Append("lower than ");
							break;

						case (int)SubTriggerTypes.EqualTo: //= 2
							stb.Append("equal to ");
							break;

						case (int)SubTriggerTypes.HigherThan: //=3
							stb.Append("higher than ");
							break;
					}
					//... and end with the selected value
					stb.Append(someValue);
					break;
			}
			_hs.SaveEventsDevices();
			return stb.ToString();
		}

		#endregion

		public void ShutDownIo()
		{
			try
			{
				// * *********************
				//For debugging only, this will delete all devices accociated by the plugin at shutdown, so new devices will be created on startup:
				//DeleteDevices()
				// * *********************

				//Setting a flag that states that we are shutting down, this can be used to abort ongoing commands
				Utils.IsShuttingDown = true;

				//Write any changes in the settings to the ini file

				//_utils.SaveSettings();
				//2018-11-11 Removed since I got error when doing a disconnect due to HS-object not available any more and giving an error 

				//Stopping the timer if it exists and runs
				if (_updateTimer != null)
				{
					_updateTimer.Change(Timeout.Infinite, Timeout.Infinite);
					_updateTimer.Dispose();
				}

				//Save all device changes on plugin shutdown
				//2018-11-11 Removed since it will always fail on disconnect. HS connection gone so no way to save
				//try
				//{
				//	Utils.Hs.SaveEventsDevices();
				//}
				//catch (Exception ex)
				//{
				//	_utils.Log("could not save devices :"+ex.Message, LogType.Error);

				//}
			}
			catch (Exception ex)
			{
				//_utils.Log("Error ending " + Utils.PluginName + " Plug-In :"+ex.Message, LogType.Error);
				Console.WriteLine("Error ending " + Utils.PluginName + " Plug-In :" + ex.Message);
			}

			Console.WriteLine("ShutdownIO complete.");
		}




		///<summary>
		///Get the actual value to check from a trigger
		///</summary>
		///<param name = "key" > The key stored in the trigger, like "SomeValue"</param>
		///<param name = "trigInfo" > The trigger information to check</param>
		///<returns>Object of whatever is stored</returns>
		///<remarks>By Moskus</remarks>
		private object GetTriggerValue(string key, IPlugInAPI.strTrigActInfo trigInfo)
		{

			var trigger = new HsTrigger();

			//Loads the trigger from the serialized object (if it exists, and it should)
			if (!(trigInfo.DataIn == null))
			{
				object deserializedTrigger = null;
				_utils.DeSerializeObject(ref trigInfo.DataIn, ref deserializedTrigger);
				if (deserializedTrigger != null)
				{
					trigger = (HsTrigger)deserializedTrigger;
				}
			}

			//A trigger has "keys" with different stored values, let's go through them all.
			//In my sample we only have one key, which is "SomeValue"
			var foundKey = trigger.GetAllKeys().SingleOrDefault(x => x.Contains(key + "_" + trigInfo.UID));
			if (foundKey != null) return trigger[foundKey];
			//Apparently we didn't find any matching keys in the trigger, so that's all we have to return
			return null;

		}


		///<summary>
		///A routine to restart the timer. Should be used when the user has chosen a different timer interval.
		///</summary>
		///<remarks>By Moskus</remarks>
		private void RestartTimer()
		{
			//Get now time
			var timeNow = DateTime.Now.TimeOfDay;

			//Round time to the nearest whole trigger (if Me.Settings.TimerInterval is set to 5 minutes, the trigger will be exectued 10:00, 10:05, 10:10, etc
			var nextWhole = TimeSpan.FromMilliseconds(
				Math.Ceiling((timeNow.TotalMilliseconds) / _settings.TimerInterval) *
				_settings.TimerInterval);

			//Find the difference in milliseconds
			var diff = (int)nextWhole.Subtract(timeNow).TotalMilliseconds;
			Console.WriteLine("RestartTimer, timeNow: " + timeNow.ToString());
			Console.WriteLine("RestartTimer, nextWhole: " + nextWhole.ToString());
			Console.WriteLine("RestartTimer, diff: " + diff);

			_updateTimer.Change(diff, _settings.TimerInterval);
		}


		#region "Action Properties"

		///<summary>
		///Return the number of actions the plugin supports.
		///</summary>
		///<returns>The plugin count</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/actioncount.htm</remarks>
		public int ActionCount()
		{
			return _actions.Count;

		}

		///<summary>
		///Return the name of the action given an action number. The name of the action will be displayed in the HomeSeer events actions list.
		///</summary>
		///<param name="ActionNumber">The number of the action. Each action is numbered, starting at 1. (BUT WHY 1?!)</param>
		///<returns>The action name from the 1-based index</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/actionname.htm</remarks>
		public string ActionName(int actionNumber)
		{

			//		Get
			if (actionNumber > 0 && actionNumber <= _actions.Count)
			{
				return Utils.PluginName + ": " + _actions.Keys(actionNumber - 1);
			}
			else
			{
				return "";
			}
		}

		#endregion

		#region "Action Interface"

		///<summary>
		///When an event is triggered, this function is called to carry out the selected action.
		///</summary>
		///<param name="ActInfo">Use the ActInfo parameter to determine what action needs to be executed then execute this action.</param>
		///<returns>Return TRUE if the action was executed successfully, else FALSE if there was an error.</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/handleaction.htm</remarks>
		public bool HandleAction(IPlugInAPI.strTrigActInfo actInfo ) {

			//		Dim houseCode As String = ""

			//		Dim deviceCode As String = ""

			//		Dim UID As String = ActInfo.UID.ToString


			//		Try
			//			If Not(ActInfo.DataIn Is Nothing) Then
			//			   DeSerializeObject(ActInfo.DataIn, action)

			//			Else
			//				Return False
			//			End If

			//			For Each key As String In action.Keys

			//				Select Case True
			//					Case key.Contains("HouseCodes_" & UID)

			//						houseCode = action(key)

			//					Case key.Contains("DeviceCodes_" & UID)

			//						deviceCode = action(key)

			//				End Select

			//			Next

			//			Console.WriteLine("HandleAction, Command received with data: " & houseCode & ", " & deviceCode)

			//			SendCommand(houseCode, deviceCode) 'This could also return a value True/False if it was successful or not

			//        Catch ex As Exception

			//			Log("Error executing action: " & ex.Message, LogType.Error)

			//		End Try

			//		Return True

		}

		///summary>
		///eturn TRUE if the given action is configured properly. There may be times when a user can select invalid selections for the action and in this case you would return FALSE so HomeSeer will not allow the action to be saved.
		////summary>
		///param name="ActInfo">Object that contains information about the action like current selections.</param>
		///returns>Return TRUE if the given action is configured properly.</returns>
		///remarks>http://homeseer.com/support/homeseer/HS3/SDK/actionconfigured.htm</remarks>
		public bool ActionConfigured(IPlugInAPI.strTrigActInfo actInfo )
		{



			//		Dim Configured As Boolean = False

			//		Dim itemsConfigured As Integer = 0

			//		Dim itemsToConfigure As Integer = 2

			//		Dim UID As String = ActInfo.UID.ToString


			//		If Not(ActInfo.DataIn Is Nothing) Then
			//		   DeSerializeObject(ActInfo.DataIn, action)

			//			For Each key In action.Keys
			//				Select Case True

			//					Case key.Contains("HouseCodes_" & UID) AndAlso action(key) <> ""
			//                        itemsConfigured += 1
			//                    Case key.Contains("DeviceCodes_" & UID) AndAlso action(key) <> ""
			//                        itemsConfigured += 1
			//                End Select

			//			Next
			//			If itemsConfigured = itemsToConfigure Then Configured = True
			//		End If
			//		Return Configured
		}

		///<summary>
		///This function is called from the HomeSeer event page when an event is in edit mode.
		///Your plug-in needs to return HTML controls so the user can make action selections.
		///Normally this is one of the HomeSeer jquery controls such as a clsJquery.jqueryCheckbox.
		///</summary>
		///<param name="sUnique">A unique string that can be used with your HTML controls to identify the control. All controls need to have a unique ID.</param>
		///<param name="ActInfo">Object that contains information about the action like current selections</param>
		///<returns> HTML controls that need to be displayed so the user can select the action parameters.</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/actionbuildui.htm</remarks>
		public string ActionBuildUI(string uniqueString, HomeSeerAPI.IPlugInAPI.strTrigActInfo actInfo ) {

			//		Dim UID As String

			//		UID = ActInfo.UID.ToString
			//		Dim stb As New StringBuilder

			//		Dim Housecode As String = ""

			//		Dim DeviceCode As String = ""

			//		Dim dd As New clsJQuery.jqDropList("HouseCodes_" & UID & sUnique, Pagename, True)
			//        Dim dd1 As New clsJQuery.jqDropList("DeviceCodes_" & UID & sUnique, Pagename, True)
			//        Dim key As String



			//		dd.autoPostBack = True
			//		dd.AddItem("--Please Select--", "", False)

			//		dd1.autoPostBack = True
			//		dd1.AddItem("--Please Select--", "", False)


			//		If Not(ActInfo.DataIn Is Nothing) Then
			//		   DeSerializeObject(ActInfo.DataIn, action)

			//		Else 'new event, so clean out the action object
			//            action = New Action

			//		End If


			//		For Each key In action.Keys
			//			Select Case True

			//				Case key.Contains("HouseCodes_" & UID)

			//					Housecode = action(key)

			//				Case key.Contains("DeviceCodes_" & UID)

			//					DeviceCode = action(key)

			//			End Select

			//		Next

			//		For Each C In "ABCDEFGHIJKLMNOP"
			//            dd.AddItem(C, C, (C = Housecode))
			//        Next

			//		stb.Append("Select House Code:")

			//		stb.Append(dd.Build)

			//		dd1.AddItem("All", "All", ("All" = DeviceCode))
			//        For i = 1 To 16
			//            dd1.AddItem(i.ToString, i.ToString, (i.ToString = DeviceCode))
			//        Next

			//		stb.Append("Select Unit Code:")

			//		stb.Append(dd1.Build)

			//		Return stb.ToString
		}

		///<summary>
		///When a user edits your event actions in the HomeSeer events, this function is called to process the selections.
		///</summary>
		///<param name="PostData">A collection of name value pairs that include the user's selections.</param>
		///<param name="ActInfo">Object that contains information about the action as "strTrigActInfo" (which is funny, as it isn't a string at all)</param>
		///<returns>Object the holds the parsed information for the action. HomeSeer will save this information for you in the database.</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/actionprocesspostui.htm</remarks>
		public IPlugInAPI.strMultiReturn ActionProcessPostUI(NameValueCollection postData , IPlugInAPI.strTrigActInfo actInfo ) {
			//		Dim ret As New HomeSeerAPI.IPlugInAPI.strMultiReturn
			//		Dim UID As String = ActInfo.UID.ToString


			//		ret.sResult = ""
			//        'HS: We cannot be passed info ByRef from HomeSeer, so turn right around and return this same value so that if we want, 
			//        '   we can exit here by returning 'Ret', all ready to go.  If in this procedure we need to change DataOut or TrigInfo,
			//        '   we can still do that.

			//		ret.DataOut = ActInfo.DataIn

			//		ret.TrigActInfo = ActInfo


			//		If PostData Is Nothing Then Return ret
			//		If PostData.Count< 1 Then Return ret


			//		If Not (ActInfo.DataIn Is Nothing) Then
			//			DeSerializeObject(ActInfo.DataIn, action)

			//		End If


			//		Dim parts As Collections.Specialized.NameValueCollection = PostData

			//		Try

			//			For Each key As String In parts.Keys
			//				If key Is Nothing Then Continue For

			//				If String.IsNullOrEmpty(key.Trim) Then Continue For

			//				Select Case True
			//					Case key.Contains("HouseCodes_" & UID), key.Contains("DeviceCodes_" & UID)
			//                        action.Add(CObj(parts(key)), key)
			//                End Select

			//			Next
			//			If Not SerializeObject(action, ret.DataOut) Then
			//				ret.sResult = Me.Name & " Error, Serialization failed. Signal Action not added."

			//				Return ret

			//			End If

			//		Catch ex As Exception

			//			ret.sResult = "ERROR, Exception in Action UI of " & Me.Name & ": " & ex.Message

			//			Return ret

			//		End Try

			//        ' All OK

			//		ret.sResult = ""

			//		Return ret

		}

		///<summary>
		///"Body of text here"... Okay, my guess:
		///This formats the chosen action when the action is "minimized" based on the user selected options
		///</summary>
		///<param name="ActInfo">Information from the current activity as "strTrigActInfo" (which is funny, as it isn't a string at all)</param>
		///<returns>Simple string. Possibly HTML-formated.</returns>
		///<remarks>http://homeseer.com/support/homeseer/HS3/SDK/actionformatui.htm</remarks>
		public string ActionFormatUI(IPlugInAPI.strTrigActInfo actInfo ) {

			//		Dim stb As New StringBuilder
			//		Dim houseCode As String = ""
			//        Dim deviceCode As String = ""

			//		Dim UID As String = ActInfo.UID.ToString


			//		If Not(ActInfo.DataIn Is Nothing) Then
			//		   DeSerializeObject(ActInfo.DataIn, action)

			//		End If


			//		For Each key As String In action.Keys
			//			Select Case True

			//				Case key.Contains("HouseCodes_" & UID)

			//					houseCode = action(key)

			//				Case key.Contains("DeviceCodes_" & UID)

			//					deviceCode = action(key)

			//			End Select

			//		Next

			//		stb.Append(" the system will do 'something' to a device with ")

			//		stb.Append("HouseCode " & houseCode & " ")
			//        If deviceCode = "ALL" Then
			//			stb.Append("for all Unitcodes")

			//		Else
			//			stb.Append("and Unitcode " & deviceCode)

			//		End If


			//		Return stb.ToString

		}

		#endregion



		#region "Device creation and management"

		///<summary>
		///Checking if the devices have created by the plugin still exists. If not, let's create them.
		///</summary>
		///<remarks>By Moskus</remarks>
		private void CheckAndCreateDevices()
		{
			//Here we wil check if we have all the devices we want or if they should be created.
			//In this example I have said that we want to have:
			// - One "Basic" device
			// - One "Advanced" device with some controls
			// - One "Root" (or Master) device with two child devices

			//HS usually use the deviceenumerator for this kind of stuff, but I prefer to use Linq.
			//As HS haven't provided a way to get a list(or "queryable method") for devices, I've made one (Check the function "Devices()" in utils.vb).
			//Here we are only interessted in the plugin devices for this plugin, so let's do some first hand filtering
			var deviceList = _utils.DevicesOnlyForPlugin();

			//First let's see if we can find any devices belonging to the plugin with device type = "Basic".The device type string should contain "Basic".
			var basicDevice = deviceList.SingleOrDefault(x => x.get_Device_Type_String(_hs).Contains("Basic"));
			if (basicDevice == null)
			{
				//Apparently we don't have a basic device, so we create one with the name "Test basic device"
				CreateBasicDevice("Test basic device");
			}

			//Then let's see if we can find the "Advanced" device, and create it if not
			var advancedDevice = deviceList.SingleOrDefault(x => x.get_Device_Type_String(_hs).Contains("Advanced"));
			if (advancedDevice == null)
			{
				CreateAdvancedDevice("Test advanced device");
			}

			//Checking root devices and child devices
			var rootDevice = deviceList.SingleOrDefault(x => x.get_Device_Type_String(_hs).Contains("Root"));
			if (rootDevice == null)
			{

				//There are no root device so let's create one, and keep the device reference
				var rootDeviceReference = CreateRootDevice("Test Root device");
				var root = (Scheduler.Classes.DeviceClass)_hs.GetDeviceByRef(rootDeviceReference);

				//The point of a root/parent device is to have children, so let's have some fun creating them *badam tish*
				for (int i = 0; i < 2; i++)
				{
					//Let's create the child device
					var childDeviceReference = CreateChildDevice("Child " + i);
					//... and associate it with the root
					if (childDeviceReference > 0)
					{
						root.AssociatedDevice_Add(_hs, childDeviceReference);
					}
				}
			}
			else
			{
				//We have a root device or more, but do we have child devices?
				var roots = deviceList.Where(x => x.get_Device_Type_String(_hs).Contains("Root"));
				foreach (var foundRoot in roots)
				{
					//If we don't have two root devices...
					if (foundRoot.get_AssociatedDevices_Count(_hs) != 2)
					{
						//...we delete them all
						foreach (var childDeviceRef in rootDevice.get_AssociatedDevices(_hs))
						{
							_hs.DeleteDevice(childDeviceRef);
						}

						//... and recreate them
						for (int y = 0; y < 2; y++)
						{
							//First create the device and get the reference
							var childReference = CreateChildDevice("Child " + y);
							//Then associated that child reference with the root.
							if (childReference > 0)
							{
								foundRoot.AssociatedDevice_Add(_hs, childReference);
							}
						}
						//NOTE:
						//This could be handled more elegantly, like checking which child devices are missing,
						//and creating only those.
					}
				}
			}
		}

		///<summary>
		///Creates a basic device without controls. It can show values, though.
		///</summary>
		///<param name="deviceName">The name of the device</param>
		///<remarks>By Moskus and http://www.homeseer.com/support/homeseer/HS3/HS3Help/scripting_devices_deviceclass1.htm </remarks>
		private int CreateBasicDevice(string deviceName)
		{
			try
			{
				//Creating a brand new device, and get the actual device from the device reference
				Scheduler.Classes.DeviceClass device =
					(Scheduler.Classes.DeviceClass)_hs.GetDeviceByRef(_hs.NewDeviceRef(deviceName));
				var reference = device.get_Ref(_hs);

				//Setting the type to plugin device
				var typeInfo = new DeviceTypeInfo_m.DeviceTypeInfo()
				{
					Device_Type = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Plug_In
				};
				device.set_DeviceType_Set(_hs, typeInfo);
				device.set_Can_Dim(_hs, false);

				device.set_Interface(_hs, Utils.PluginName);//Don't change this, or the device won't be associated with your plugin
															//Todo: Checkout pluginInstance handling. Should be for each instance. Now for all instances
				device.set_InterfaceInstance(_hs, Utils.PluginInstance);//Don't change this, or the device won't be associated with that particular instance

				device.set_Device_Type_String(_hs, Utils.PluginName + " " + "Basic");//This you can change to something suitable, though. :)

				//Setting the name and locations
				device.set_Name(_hs, deviceName);//as approved by input variable
				device.set_Location(_hs, Settings.Location);
				device.set_Location2(_hs, Settings.Location2);

				//Misc options
				device.set_Status_Support(_hs, false);//Set to True if the devices can be polled, False if not. (See PollDevice in hspi.vb)
				device.MISC_Set(_hs, Enums.dvMISC.SHOW_VALUES);//If not set, device control options will not be displayed
				device.MISC_Set(_hs, Enums.dvMISC.NO_LOG);//As default, we don't want to log every device value change to the log

				//Committing to the database, clear value-status-pairs and graphic-status pairs
				_hs.SaveEventsDevices();

				_hs.DeviceVSP_ClearAll(reference, true);
				_hs.DeviceVGP_ClearAll(reference, true);

				//Return the reference
				return reference;
			}
			catch (Exception ex)
			{
				_utils.Log("Unable to create basic device. Error: " + ex.Message, LogType.Warning);
			}

			return 0; //if anything fails.
		}

		///<summary>
		///Creates a "advanced" device with some control examples.
		///Based on the device from CreateBasicDevice
		///</summary>
		///<param name="deviceName">The name of the device</param>
		///<remarks>By Moskus and http://www.homeseer.com/support/homeseer/HS3/HS3Help/scripting_devices_deviceclass1.htm </remarks>
		private int CreateAdvancedDevice(string deviceName)
		{
			//Creating BASIC device and getting its device reference
			var device = (Scheduler.Classes.DeviceClass)_hs.GetDeviceByRef(CreateBasicDevice(deviceName));
			var reference = device.get_Ref(_hs);

			//This device type is not Basic, it's Advanced
			device.set_Device_Type_String(_hs, Utils.PluginName + " " + "Advanced");

			//Commit to the database
			_hs.SaveEventsDevices();

			//We'll create three controls, a button with the value 0, a slider for values 1 to 9, and yet another button for the value 10

			//=========
			//Value = 0
			//=========
			//Status pair
			var svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Both);
			svPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
			svPair.Value = 0;
			svPair.Status = "Value " + svPair.Value;
			svPair.ControlUse = ePairControlUse._Off; //For IFTTT/HStouch support
			svPair.Render = Enums.CAPIControlType.Button;
			svPair.IncludeValues = true;
			_hs.DeviceVSP_AddPair(reference, svPair);

			//... and some graphics
			var vgPair = new VSVGPairs.VGPair();
			vgPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
			vgPair.Set_Value = 0;
			vgPair.Graphic = "images/checkbox_disabled_on.png";
			_hs.DeviceVGP_AddPair(reference, vgPair);

			//============
			//Value 1 to 9
			//============
			//Status pair
			svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Both);
			svPair.PairType = VSVGPairs.VSVGPairType.Range;
			svPair.RangeStart = 1;
			svPair.RangeEnd = 9;
			svPair.RangeStatusPrefix = "Value ";
			svPair.ControlUse = ePairControlUse._Dim; //For HStouch support
			svPair.Render = Enums.CAPIControlType.ValuesRangeSlider;
			svPair.IncludeValues = true;
			_hs.DeviceVSP_AddPair(reference, svPair);

			//... and some graphics
			vgPair = new VSVGPairs.VGPair();
			vgPair.PairType = VSVGPairs.VSVGPairType.Range;
			vgPair.RangeStart = 1;
			vgPair.RangeEnd = 9;
			vgPair.Graphic = "images/checkbox_on.png";
			_hs.DeviceVGP_AddPair(reference, vgPair);

			//==========
			//Value = 10
			//==========
			//Status pair
			svPair = new VSVGPairs.VSPair(HomeSeerAPI.ePairStatusControl.Both);
			svPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
			svPair.Value = 10;
			svPair.Status = "Value " + svPair.Value;
			svPair.ControlUse = ePairControlUse._On; //For IFTTT/HStouch support
			svPair.Render = Enums.CAPIControlType.Button;
			svPair.IncludeValues = true;
			_hs.DeviceVSP_AddPair(reference, svPair);

			//... and some graphics
			vgPair = new VSVGPairs.VGPair();
			vgPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
			vgPair.Set_Value = 10;
			vgPair.Graphic = "images/checkbox_hvr.png";
			_hs.DeviceVGP_AddPair(reference, vgPair);

			//return the reference
			return reference;
		}

		///<summary>
		///Creates a root/parent device based on the basic device
		///</summary>
		///<param name="rootName"></param>
		///<returns></returns>
		///<remarks></remarks>
		private int CreateRootDevice(string rootName)
		{
			//Creating BASIC device and getting its device reference
			var device = (Scheduler.Classes.DeviceClass)_hs.GetDeviceByRef(CreateBasicDevice(rootName));

			//This device type is not Basic, it's Advanced
			device.set_Device_Type_String(_hs, Utils.PluginName + " " + "Root");

			//Setting it as a root device
			device.set_Relationship(_hs, HomeSeerAPI.Enums.eRelationship.Parent_Root);

			//Committing to the database and return the reference
			_hs.SaveEventsDevices();
			return device.get_Ref(_hs);
		}

		///<summary>
		///Creates a child device based on the basic device
		///</summary>
		///<param name="childName"></param>
		///<returns></returns>
		///<remarks></remarks>
		private int CreateChildDevice(string childName)
		{
			//Creating BASIC device and getting its device reference
			var device = (Scheduler.Classes.DeviceClass)_hs.GetDeviceByRef(CreateBasicDevice(childName));

			//This device type is not Basic, it's Advanced
			device.set_Device_Type_String(_hs, Utils.PluginName + " " + "Child");

			//Setting it as a child device
			device.set_Relationship(_hs, HomeSeerAPI.Enums.eRelationship.Child);

			//Committing to the database and return the reference
			_hs.SaveEventsDevices();

			return device.get_Ref(_hs);
		}

		#endregion



		#region "Device creation and management"

		//   ''' <summary>
		//   ''' Checking if the devices have created by the plugin still exists. If not, let's create them.
		//   ''' </summary>
		//   ''' <remarks>By Moskus</remarks>
		//   Private Sub CheckAndCreateDevices()
		//       'Here we wil check if we have all the devices we want or if they should be created.
		//       'In this example I have said that we want to have:
		//       ' - One "Basic" device
		//       ' - One "Advanced" device with some controls
		//       ' - One "Root" (or Master) device with two child devices

		//       'HS usually use the deviceenumerator for this kind of stuff, but I prefer to use Linq.
		//       'As HS haven't provided a way to get a list(or "queryable method") for devices, I've made one (Check the function "Devices()" in utils.vb).
		//       'Here we are only interessted in the plugin devices for this plugin, so let's do some first hand filtering

		//	Dim devs = (From d In Devices()

		//				Where d.Interface(hs) = Me.Name).ToList


		//       'First let's see if we can find any devices belonging to the plugin with device type = "Basic".The device type string should contain "Basic".
		//       If(From d In devs Where d.Device_Type_String(hs).Contains("Basic")).Count = 0 Then
		//           'Apparently we don't have a basic device, so we create one with the name "Test basic device"
		//           CreateBasicDevice("Test basic device")

		//	End If


		//       'Then let's see if we can find the "Advanced" device, and create it if not
		//	If(From d In devs Where d.Device_Type_String(hs).Contains("Advanced")).Count = 0 Then
		//	   CreateAdvancedDevice("Test advanced device")

		//	End If


		//       'Checking root devices and child devices
		//       If(From d In devs Where d.Device_Type_String(hs).Contains("Root")).Count = 0 Then

		//           'There are no root device so let's create one, and keep the device reference
		//		Dim rootDeviceReference As Integer = CreateRootDevice("Test Root device")

		//		Dim root As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(rootDeviceReference)

		//           'The point of a root/parent device is to have children, so let's have some fun creating them *badam tish*

		//		For i As Integer = 1 To 2
		//               'Let's create the child devivce
		//			Dim childDeviceReference As Integer = CreateChildDevice("Child " & i)

		//               '... and associate it with the root

		//			If childDeviceReference > 0 Then root.AssociatedDevice_Add(hs, childDeviceReference)
		//		Next


		//	Else
		//           'We have a root device or more, but do we have child devices?

		//		Dim roots = From d In devs
		//					Where d.Device_Type_String(hs).Contains("Root")


		//		For Each root In roots
		//               'If we don't have two root devices...
		//			If root.AssociatedDevices_Count(hs) <> 2 Then

		//                   '...we delete them all
		//                   For Each child In root.AssociatedDevices(hs)
		//					hs.DeleteDevice(child)

		//				Next

		//                   '... and recreate them
		//                   For i As Integer = 1 To 2
		//                       'First create the device and get the reference
		//                       Dim childDeviceReference As Integer = CreateChildDevice("Child " & i)

		//                       'Then associated that child reference with the root.
		//                       If childDeviceReference > 0 Then root.AssociatedDevice_Add(hs, childDeviceReference)

		//				Next

		//                   'NOTE: This could be handled more elegantly, like checking which child devices are missing, and creating only those.
		//               End If

		//		Next


		//	End If

		//End Sub

		//   ''' <summary>
		//   ''' Creates a basic device without controls. It can show values, though.
		//   ''' </summary>
		//   ''' <param name="name">The name of the device</param>
		//   ''' <remarks>By Moskus and http://www.homeseer.com/support/homeseer/HS3/HS3Help/scripting_devices_deviceclass1.htm </remarks>
		//   Private Function CreateBasicDevice(ByVal name As String) As Integer

		//	Try
		//           'Creating a brand new device, and get the actual device from the device reference
		//           Dim device As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(hs.NewDeviceRef(name))

		//		Dim reference As Integer = device.Ref(hs)

		//           'Setting the type to plugin device
		//           Dim typeInfo As New DeviceTypeInfo
		//		typeInfo.Device_Type = DeviceTypeInfo.eDeviceAPI.Plug_In
		//		device.DeviceType_Set(hs) = typeInfo

		//		device.Can_Dim(hs) = False


		//		device.Interface(hs) = Me.Name          'Don't change this, or the device won't be associated with your plugin

		//		device.InterfaceInstance(hs) = instance 'Don't change this, or the device won't be associated with that particular instance


		//		device.Device_Type_String(hs) = Me.Name & " " & "Basic" ' This you can change to something suitable, though. :)

		//           'Setting the name and locations

		//		device.Name(hs) = name 'as approved by input variable

		//		device.Location(hs) = Settings.Location

		//		device.Location2(hs) = Settings.Location2

		//           'Misc options

		//		device.Status_Support(hs) = False               'Set to True if the devices can be polled, False if not. (See PollDevice in hspi.vb)

		//		device.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)   'If not set, device control options will not be displayed.

		//		device.MISC_Set(hs, Enums.dvMISC.NO_LOG)        'As default, we don't want to log every device value change to the log

		//           'Committing to the database, clear value-status-pairs and graphic-status pairs

		//		hs.SaveEventsDevices()

		//		hs.DeviceVSP_ClearAll(reference, True)
		//		hs.DeviceVGP_ClearAll(reference, True)


		//           'Return the reference

		//		Return reference


		//	Catch ex As Exception

		//		Log("Unable to create basic device. Error: " & ex.Message, LogType.Warning)

		//	End Try

		//	Return 0 'if anything fails.
		//   End Function

		//   ''' <summary>
		//   ''' Creates a "advanced" device with some control examples.
		//   ''' Based on the device from CreateBasicDevice
		//   ''' </summary>
		//   ''' <param name="name">The name of the device</param>
		//   ''' <remarks>By Moskus and http://www.homeseer.com/support/homeseer/HS3/HS3Help/scripting_devices_deviceclass1.htm </remarks>
		//   Private Function CreateAdvancedDevice(ByVal name As String) As Integer
		//       'Creating BASIC device and getting its device reference
		//       Dim device As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(CreateBasicDevice(name))

		//	Dim reference As Integer = device.Ref(hs)

		//       'This device type is not Basic, it's Advanced

		//	device.Device_Type_String(hs) = Me.Name & " " & "Advanced"

		//       'Commit to the database
		//       hs.SaveEventsDevices()

		//       'We'll create three controls, a button with the value 0, a slider for values 1 to 9, and yet another button for the value 10

		//       ' =========
		//       ' Value = 0
		//       ' =========
		//       'Status pair
		//       Dim SVpair As New HomeSeerAPI.VSPair(HomeSeerAPI.ePairStatusControl.Both)
		//	SVpair.PairType = HomeSeerAPI.VSVGPairType.SingleValue
		//	SVpair.Value = 0

		//	SVpair.Status = "Value " & SVpair.Value

		//	SVpair.ControlUse = ePairControlUse._Off 'For IFTTT/HStouch support

		//	SVpair.Render = Enums.CAPIControlType.Button

		//	SVpair.IncludeValues = True

		//	hs.DeviceVSP_AddPair(reference, SVpair)

		//       '... and some graphics

		//	Dim VGpair As New HomeSeerAPI.VGPair
		//	VGpair.PairType = HomeSeerAPI.VSVGPairType.SingleValue

		//	VGpair.Set_Value = 0

		//	VGpair.Graphic = "images/checkbox_disabled_on.png"

		//	hs.DeviceVGP_AddPair(reference, VGpair)

		//       ' ============
		//       ' Value 1 to 9
		//       ' ============
		//       'Status pair

		//	SVpair = New HomeSeerAPI.VSPair(HomeSeerAPI.ePairStatusControl.Both)
		//	SVpair.PairType = HomeSeerAPI.VSVGPairType.Range

		//	SVpair.RangeStart = 1

		//	SVpair.RangeEnd = 9

		//	SVpair.RangeStatusPrefix = "Value "

		//	SVpair.ControlUse = ePairControlUse._Dim 'For HStouch support

		//	SVpair.Render = Enums.CAPIControlType.ValuesRangeSlider

		//	SVpair.IncludeValues = True

		//	hs.DeviceVSP_AddPair(reference, SVpair)

		//       '... and some graphics

		//	VGpair = New HomeSeerAPI.VGPair
		//	VGpair.PairType = HomeSeerAPI.VSVGPairType.Range

		//	VGpair.RangeStart = 1

		//	VGpair.RangeEnd = 9

		//	VGpair.Graphic = "images/checkbox_on.png"

		//	hs.DeviceVGP_AddPair(reference, VGpair)

		//       ' ==========
		//       ' Value = 10
		//       ' ==========
		//       'Status pair

		//	SVpair = New HomeSeerAPI.VSPair(HomeSeerAPI.ePairStatusControl.Both)
		//	SVpair.PairType = HomeSeerAPI.VSVGPairType.SingleValue

		//	SVpair.Value = 10

		//	SVpair.Status = "Value " & SVpair.Value

		//	SVpair.ControlUse = ePairControlUse._On 'For IFTTT/HStouch support

		//	SVpair.Render = Enums.CAPIControlType.Button

		//	SVpair.IncludeValues = True

		//	hs.DeviceVSP_AddPair(reference, SVpair)

		//       '... and some graphics

		//	VGpair = New HomeSeerAPI.VGPair
		//	VGpair.PairType = HomeSeerAPI.VSVGPairType.SingleValue

		//	VGpair.Set_Value = 10

		//	VGpair.Graphic = "images/checkbox_hvr.png"

		//	hs.DeviceVGP_AddPair(reference, VGpair)


		//       'return the reference

		//	Return reference

		//End Function

		//   ''' <summary>
		//   ''' Creates a root/parent device based on the basic device
		//   ''' </summary>
		//   ''' <param name="name"></param>
		//   ''' <returns></returns>
		//   ''' <remarks></remarks>

		//Private Function CreateRootDevice(ByVal name As String) As Integer
		//       'Creating BASIC device and getting its device reference
		//       Dim device As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(CreateBasicDevice(name))

		//       'This device type is not Basic, it's Advanced

		//	device.Device_Type_String(hs) = Me.Name & " " & "Root"

		//       'Setting it as a root device
		//       device.Relationship(hs) = HomeSeerAPI.Enums.eRelationship.Parent_Root

		//       'Committing to the database and return the reference
		//       hs.SaveEventsDevices()
		//	Return device.Ref(hs)
		//End Function

		//Private Function CreateChildDevice(ByVal name As String) As Integer
		//       'Creating BASIC device and getting its device reference
		//       Dim device As Scheduler.Classes.DeviceClass = hs.GetDeviceByRef(CreateBasicDevice(name))

		//       'This device type is not Basic, it's Advanced

		//	device.Device_Type_String(hs) = Me.Name & " " & "Child"

		//       'Setting it as a child device
		//       device.Relationship(hs) = HomeSeerAPI.Enums.eRelationship.Child

		//       'Committing to the database and return the reference
		//       hs.SaveEventsDevices()
		//	Return device.Ref(hs)
		//End Function
		#endregion

	}
}