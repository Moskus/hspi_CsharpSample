using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Threading;
using hspi_CsharpSample.HomeSeerClasses;

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

		private HsTrigger _trigger =new HsTrigger();
		private HsAction _action = new HsAction();


		public Plugin()
		{
		}

		public Timer UpdateTimer => _updateTimer;
		
		public string Name => Utils.PluginName;

		public Utils Utils
		{
			get => _utils;
			set => _utils = value;
		}

		public Settings Settings
		{
			get => _settings;
			set => _settings = value;
		}

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

			//	Dim triggers() As HomeSeerAPI.IPlugInAPI.strTrigActInfo = callback.GetTriggers(Me.Name)
			//Console.WriteLine("UpdateTimer_Elapsed." & vbTab & "Triggers found: " & triggers.Count & vbTab & "Random value: " & randomValue)


			//'Checking reach event with triggers from our plugin if they should be triggered or not

			//For Each t In triggers


			//	'The name of the key we are looking for

			//	Dim key As String = "SomeValue"


			//	'Get the value from the trigger

			//	Dim triggerValue As Integer = GetTriggerValue(key, t)

			//	'Console.WriteLine("Value found for " & key & ": " & triggervalue) '... for debugging


			//	'Remember from TriggerBuildUI that if "someValue" is equal to "-1", we don't have a properly configured trigger, so we need to skip this if this(the current "t") value is -1

			//	If triggerValue = -1 Then
			//		Console.WriteLine("Event with ref = " & t.evRef & " is not properly configured.")

			//		Continue For

			//	End If


			//	'If not, we do have a working trigger, so let's continue


			//	'We have the option to select between two triggers, so we need to check them both

			//	Select Case t.TANumber
			//		Case TriggerTypes.WithoutSubtriggers  '= 1. The trigger without subtriggers


			//			'If the test is true, then trig the Trigger

			//			If triggerValue >= randomValue Then

			//				Console.WriteLine("Trigging event with reference " & t.evRef)

			//				TriggerFire(t)

			//			End If



			//		Case TriggerTypes.WithSubtriggers '= 2. The trigger with subtriggers


			//			'We have multiple options for checking for values, they are specified by the subtrigger number (1-based)

			//			Select Case t.SubTANumber
			//				Case SubTriggerTypes.LowerThan 'The random value should be lower than the value specified in the event

			//					If triggerValue >= randomValue Then

			//						Console.WriteLine("Value is lower. Trigging event with reference " & t.evRef)

			//						TriggerFire(t)

			//					End If


			//				Case SubTriggerTypes.EqualTo 'The random value should be equal to the value specified in the event

			//					If triggerValue = randomValue Then
			//						Console.WriteLine("Value is equal. Trigging event with reference " & t.evRef)

			//						TriggerFire(t)

			//					End If


			//				Case SubTriggerTypes.HigherThan 'The random value should be higher than the value specified in the event

			//					If triggerValue <= randomValue Then

			//						Console.WriteLine("Value is higher. Trigging event with reference " & t.evRef)

			//						TriggerFire(t)

			//					End If


			//				Case Else

			//					Log("Undefined subtrigger!")


			//			End Select


			//		Case Else

			//			Log("Undefined trigger!")


			//	End Select


			//Next



			////*******
			////DEVICES
			////*******
			////We get all the devices (and Linq is awesome!)
			//Dim devs = (From d In Devices()

			//			Where d.Interface(hs) = Me.Name).ToList


			////In this example there are not any external sources that should update devices, so we're just updating the device value of the basic device and setting it to the new random value.


			////We do this for each "Basic" device we have (usually just one, but still...)

			//For Each dev In(From d In devs
			//				 Where d.Device_Type_String(hs).Contains("Basic"))
			//          hs.SetDeviceValueByRef(dev.Ref(hs), randomValue, True)

			//	hs.SetDeviceString(dev.Ref(hs), "Last random value: " & randomValue, False)

			//Next


			//'... but of course we can do cooler stuff with our devices than that. E.g. add the text stored in the "Advanced Device" to the device string, for example


			//'Again, for all "Advanced" devices

			//For Each dev In(From d In devs Where d.Device_Type_String(hs).Contains("Advanced"))

			//          'Get the PlugExtraData class stored in the device

			//	Dim PED As clsPlugExtraData = dev.PlugExtraData_Get(hs)


			//	'... but we can only do something if there actually IS some PlugExtraData

			//	If PED IsNot Nothing Then


			//		'Get the value belonging to our plugin from the devices PlugExtraData 

			//		Dim savedString As String = PEDGet(PED, Me.Name)


			//		'TODO: Do something with the saved string and the random value?

			//		'hs.SetDeviceValueByRef(dev.Ref(hs), randomvalue, True)

			//		'hs.SetDeviceString(dev.Ref(hs), savedString & " - with value: " & randomvalue, True)

			//	End If

			//Next

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
			_updateTimer = new System.Threading.Timer(new TimerCallback(UpdateRandomValue), null, Timeout.Infinite,
				_settings.TimerInterval);
			RestartTimer();


			Console.WriteLine("Initializing done! Ready...");

			return "";
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
			var diff =(int)nextWhole.Subtract(timeNow).TotalMilliseconds;
			Console.WriteLine("RestartTimer, timeNow: " + timeNow.ToString());
			Console.WriteLine("RestartTimer, nextWhole: " + nextWhole.ToString());
			Console.WriteLine("RestartTimer, diff: " + diff);

			_updateTimer.Change(diff, _settings.TimerInterval);
		}

		public void SaveSettings()
		{
			_settings.Save();
		}
	}
}