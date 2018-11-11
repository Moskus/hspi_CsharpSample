using System;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using hspi_CsharpSample.HomeSeerClasses;
using HomeSeerAPI;

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

		private HsTriggers _trigger = new HsTriggers();
		private HsActions _action = new HsActions();
		private IHSApplication _hs;


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

			//Adding a trigger 
			_triggers.Add(null, "Random value is lower than");
			//Adding a second trigger with subtriggers
			//... so first let us create the subtriggers
			var subtriggers = new HsTriggers();
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
			var deviceList = _utils.Devices();

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


		private int CreateChildDevice(object p0)
		{
			throw new NotImplementedException();
		}

		private int CreateRootDevice(string testRootDevice)
		{
			throw new NotImplementedException();
		}

		private void CreateAdvancedDevice(string testAdvancedDevice)
		{
			throw new NotImplementedException();
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

		public void SaveSettings()
		{
			_settings.Save();
		}
	}
}