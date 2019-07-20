using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using HomeSeerAPI;
using Scheduler;
using Scheduler.Classes;

namespace HSPI_CsharpSample
{
	public class Utils
	{
		public static string PluginName = "CsharpSample";
		private string _pluginInstance="";
		public static string IniFile = "CsharpSample.ini";

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

		public string PluginInstance
		{
			get
			{
				return _pluginInstance;
			}
			set
			{
				if (value == null)
				{
					_pluginInstance = "";
				}
				else
				{
					_pluginInstance = value;
				}
				
			}
		}

		public IAppCallbackAPI Callback { get; set; }
		public static IHSApplication Hs { get; set; }

        ///<summary>
        ///Registers the web page in HomeSeer
        ///</summary>
        ///<param name="link">A short link to the page</param>
        ///<param name="linktext">The text to be shown</param>
        ///<param name="page_title">The title of the page when loaded</param>
        ///<remarks>HSPI_SAMPLE_BASIC</remarks>

        public void RegisterWebPage(string link, string linkText = "", string pageTitle = "")            
        {
            try
            {
                var theLink = link;
                Hs.RegisterPage(theLink, Utils.PluginName, _pluginInstance);

                if (linkText == "hidden")
                {
                    linkText = "";
                }
                else if (string.IsNullOrEmpty(linkText))
                {
                    linkText = link;
                }

                linkText = linkText.Replace(Utils.PluginName, "").Replace("_", " ");

                if (pageTitle == "hidden")
                {
                    pageTitle = "";
                }
                else if (string.IsNullOrEmpty(pageTitle))
                {
                    pageTitle = linkText;
                }

                var webPageDescription = new HomeSeerAPI.WebPageDesc();
                webPageDescription.plugInName = Utils.PluginName;

                webPageDescription.link = theLink;

                webPageDescription.linktext = linkText + _pluginInstance;
                webPageDescription.page_title = pageTitle + _pluginInstance;
                Callback.RegisterLink(webPageDescription);
            }
            catch (Exception ex)
            {
                Log("Registering Web Links (RegisterWebPage): " + ex.Message, LogType.Error);
            }
        }

        public void RegisterConfigPage(string link, string linkText = "", string pageTitle = "")
		{
			try
			{
				var theLink = link;
				Hs.RegisterPage(theLink, Utils.PluginName, _pluginInstance);

				if (string.IsNullOrEmpty(linkText))
				{
					linkText = link;
				}

				linkText = linkText.Replace(Utils.PluginName, "").Replace("_", " ");

				if (string.IsNullOrEmpty(pageTitle))
				{
					pageTitle = linkText;
				}

				var webPageDescription = new HomeSeerAPI.WebPageDesc();
				webPageDescription.plugInName = Utils.PluginName;

				webPageDescription.link = theLink;

				webPageDescription.linktext = linkText + _pluginInstance;
				webPageDescription.page_title = pageTitle + _pluginInstance;
				Callback.RegisterConfigLink(webPageDescription);
			}
			catch (Exception ex)
			{
				Log("Registering Web Links (RegisterWebPage): " + ex.Message, LogType.Error);
			}
		}

        public void RegisterHelpPage(string link, string linkText = "", string pageTitle = "")
        {
            try
            {
                var theLink = link;
                Hs.RegisterPage(theLink, Utils.PluginName, _pluginInstance);

                if (string.IsNullOrEmpty(linkText))
                {
                    linkText = link;
                }

                linkText = linkText.Replace(Utils.PluginName, "").Replace("_", " ");

                if (string.IsNullOrEmpty(pageTitle))
                {
                    pageTitle = linkText;
                }

                var webPageDescription = new HomeSeerAPI.WebPageDesc();
                webPageDescription.plugInName = Utils.PluginName;

                webPageDescription.link = theLink;

                webPageDescription.linktext = linkText + _pluginInstance;
                webPageDescription.page_title = pageTitle + _pluginInstance;
                Hs.RegisterHelpLink(webPageDescription);
            }
            catch (Exception ex)
            {
                Log("Registering Help Page Link (RegisterHelpPage): " + ex.Message, LogType.Error);
            }
        }



        ///<summary>
        ///Logging
        ///</summary>
        ///<param name="Message">The message to be logged</param>
        ///<param name="Log_Level">Normal, Warning or Error</param>
        ///<remarks>HSPI_SAMPLE_BASIC</remarks>
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

		//<summary>
		//List of all devices in HomeSeer belonging to the plugin. Used to enable Linq queries on devices.
		//</summary>
		//<returns>Generic.List() of all devices for only this plugin</returns>
		//<remarks>By Moskus</remarks>
		public List<DeviceClass> DevicesOnlyForPlugin()
		{
			var ret = new List<Scheduler.Classes.DeviceClass>();
			DeviceClass device;
			clsDeviceEnumeration deviceEnumeration;
			deviceEnumeration = (clsDeviceEnumeration)Hs.GetDeviceEnumerator();
			while (!deviceEnumeration.Finished)
			{
				device = deviceEnumeration.GetNext();
				if (device.get_Interface(Hs) == Utils.PluginName)
					ret.Add(device);
			}
			return ret;
		}

		///<summary>
		///Adds serialized PluginExtraData to a device, removes and adds if it already exists
		///</summary>
		///<param name="PED">The PED object to return data to (ByRef)</param>
		///<param name="PEDName">The key to look for. Moskus: I only use the pl</param>
		///<param name="PEDValue"></param>
		///<remarks></remarks>
		public void PedAdd(ref PlugExtraData.clsPlugExtraData ped, string pedName, object pedValue)
		{
			byte[] byteObject = null;
			if (ped == null)
			{
				ped = new PlugExtraData.clsPlugExtraData();
			}

			SerializeObject(pedValue, ref byteObject);
			if (!ped.AddNamed(pedName, byteObject)) //'AddNamed will return False if "PEDName" it already exists
			{
				ped.RemoveNamed(pedName);
				ped.AddNamed(pedName, byteObject);
			}
		}



		///<summary>
		///Returns serialized pluginExtraData from a device
		///</summary>
		///<param name="ped"></param>
		///<param name="pedName"></param>
		///<returns></returns>
		///<remarks></remarks>
		public object PedGet(ref PlugExtraData.clsPlugExtraData ped, string pedName)
		{
			var returnValue = new object();

			byte[] byteObject = (byte[])ped.GetNamed(pedName);

			if (byteObject == null) return null;

			DeSerializeObject(ref byteObject, ref returnValue);

			return returnValue;

		}

		///<summary>
		///Used to serialize an object to a bytestream, which can be stored in a device ("PDE" or "clsPlugExtraData"), Action or Trigger
		///</summary>
		///<param name="objIn">Input object</param>
		///<param name="byteOut">Output bytes</param>
		///<returns>True/False success</returns>
		///<remarks>By HomeSeer</remarks>
		public bool SerializeObject(object objIn, ref byte[] byteOut)
		{
			if (objIn == null)
				return false;
			var memStream = new MemoryStream();
			var formatter = new BinaryFormatter();

			try
			{

				formatter.Serialize(memStream, objIn);
				byteOut = new byte[memStream.Length - 1];

				byteOut = memStream.ToArray();
				return true;
			}
			catch (Exception ex)
			{
				if (Hs != null)
					Log("Serializing object " + objIn.ToString() + " :" + ex.Message, LogType.Error);
				return false;
			}
		}

		///<summary>
		///Used to deserialze bytestream to an object, stored in a device ("PDE" or "clsPlugExtraData"), Action or Trigger
		///</summary>
		///<param name="byteIn">Input bytes</param>
		///<param name="objOut">Output object</param>
		///<returns>True/False success</returns>
		///<remarks>By HomeSeer</remarks>
		public bool DeSerializeObject(ref byte[] byteIn, ref object objOut)
		{
			//The following is a comment from HST. Make of it what you want:
			//   "Almost immediately there is a test to see if ObjOut is NOTHING.  The reason for this
			//   when the ObjOut is suppose to be where the deserialized object is stored, is that 
			//   I could find no way to test to see if the deserialized object and the variable to 
			//   hold it was of the same type.  If you try to get the type of a null object, you get
			//   only a null reference exception!  If I do not test the object type beforehand and 
			//   there is a difference, then the InvalidCastException is thrown back in the CALLING
			//   procedure, not here, because the cast is made when the ByRef object is cast when this
			//   procedure returns, not earlier.  In order to prevent a cast exception in the calling
			//   procedure that may or may not be handled, I made it so that you have to at least 
			//   provide an initialized ObjOut when you call this - ObjOut is set to nothing after it 
			//   is typed."
			//Did that make sense to you?

			//If input and/or output is nothing then it failed (we need some objects to work with), so return False
			if (byteIn == null) return false;
			if (byteIn.Length < 1) return false;
			if (objOut == null) return false;

			//Else: Let's deserialize the bytes
			var memStream = new MemoryStream();
			var formatter = new BinaryFormatter();
			object objTest;
			System.Type tType;
			System.Type oType;

			try
			{
				oType = objOut.GetType();
				objOut = null;
				memStream = new MemoryStream(byteIn);

				objTest = formatter.Deserialize(memStream);
				if (objTest == null)
					return false;

				tType = objTest.GetType();
				//if(!tType.Equals(oType) return false;

				objOut = objTest;
				if (objOut == null)
					return false;
				return true;
			}
			catch (InvalidCastException exIC)
			{
				Log("DeSerializing object - Invalid cast exception: " + exIC.Message, LogType.Error);
				return false;
			}
			catch (Exception ex)
			{
				Log("DeSerializing object: " + ex.Message, LogType.Error);
				return false;
			}
		}

		///<summary>
		///Deletes all devices associated with the plugin.
		///</summary>
		///<remarks>Linq-ified by Moskus</remarks>
		public void DeleteDevices()
		{
			//Get all devices belonging to the plugin
			var pluginDevices = Devices().Where(x => x.get_Interface(Hs) == Utils.PluginName);

			//Deleting the devices
			foreach (var pluginDevice in pluginDevices)
			{
				try
				{
					Hs.DeleteDevice(pluginDevice.get_Ref(Hs));
				}
				catch (Exception ex)
				{
					Log(
						"Could not delete device '" + pluginDevice.get_Location2(Hs) + " " +
						pluginDevice.get_Location(Hs) + " " + pluginDevice.get_Name(Hs) +
						"' (ref = " + pluginDevice.get_Ref(Hs) + "). Exception: " + ex.Message, LogType.Error);
				}
			}
		}

		///<summary>
		///SAMPLE PLUGIN SUB, not really used here.
		///Creates a new device
		///</summary>
		///<param name="pName"></param>
		///<param name="modNum"></param>
		///<param name="counter"></param>
		///<param name="reference"></param>
		///<returns></returns>
		///<remarks>By HomeSeer</remarks>
		private bool InitDevice(string pName, int modNum, int counter, int reference = 0)
		{
			Scheduler.Classes.DeviceClass dv = null;
			Log("Initiating Device " + pName, LogType.Normal);

			try
			{
				if (!Hs.DeviceExistsRef(reference))
				{
					reference = Hs.NewDeviceRef(pName);
					try
					{
						dv = (DeviceClass)Hs.GetDeviceByRef(reference);
						InitHSDevice(ref dv, pName);
						return true;
					}
					catch (Exception ex)
					{
						Log("Error initializing device " + pName + ": " + ex.Message, LogType.Error);
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Log("InitDevice: Error getting RefID from deviceCode within InitDevice. (" + ex.Message + ")",
					LogType.Error);
			}
			return false;
		}

		///<summary>
		///SAMPLE PLUGIN SUB, not really used.
		///</summary>
		///<param name="dv"></param>
		///<param name="Name"></param>
		public void InitHSDevice(ref Scheduler.Classes.DeviceClass dv, string name = "Optional_Sample_device_name")
		{
			var dt = new DeviceTypeInfo_m.DeviceTypeInfo();
			dt.Device_Type = (int)DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Plug_In;
			dv.set_DeviceType_Set(Hs, dt);
			dv.set_Interface(Hs, Utils.PluginName);
			dv.set_InterfaceInstance(Hs, _pluginInstance);
			dv.set_Last_Change(Hs, DateTime.Now);
			dv.set_Name(Hs, name);
			dv.set_Location(Hs, Utils.PluginName);
			dv.set_Device_Type_String(Hs, Utils.PluginName);
			dv.MISC_Set(Hs, Enums.dvMISC.SHOW_VALUES);
			dv.MISC_Set(Hs, Enums.dvMISC.NO_LOG);
			dv.set_Status_Support(Hs, false);//Set to True if the devices can be polled,  false if not
		}

		///<summary>
		///SAMPLE PROJECT SUB.
		///This is just an example to process some data provided when SetIOmulti was triggered.
		///SetIO multi sends the HouseCode, deviceCode and Action to this function.
		///</summary>
		///<param name="houseCode"></param>
		///<param name="deviceCode"></param>
		///<remarks></remarks>
		public void SendCommand(string houseCode, string deviceCode)
		{
			//Send a command somewhere, but for now,
			//	just log it

			Hs.WriteLog(Utils.PluginName,
				"utils.cs -> SendCommand. HouseCode: " + houseCode + " - DeviceCode: " + deviceCode);

		}

		///<summary>
		///List of all devices in HomeSeer. Used to enable Linq queries on devices.
		///</summary>
		///<returns>Generic.List() of all devices</returns>
		///<remarks>By Moskus</remarks>
		public List<Scheduler.Classes.DeviceClass> Devices()
		{
			List<Scheduler.Classes.DeviceClass> ret = new List<Scheduler.Classes.DeviceClass>();
			Scheduler.Classes.DeviceClass device;
			Scheduler.Classes.clsDeviceEnumeration deviceEnumeration;
			deviceEnumeration = (Scheduler.Classes.clsDeviceEnumeration)Hs.GetDeviceEnumerator();
			while (!deviceEnumeration.Finished)
			{
				device = deviceEnumeration.GetNext();
				ret.Add(device);
			}
			return ret;
		}

		///<summary>
		///List of all events in HomeSeer. Used to enable Linq queries on events.
		///</summary>
		///<returns>Generic.List() of all EventData</returns>
		///<remarks>By Moskus</remarks>
		public List<HomeSeerAPI.strEventData> Events()
		{
			var ret = new List<HomeSeerAPI.strEventData>();
			foreach (HomeSeerAPI.strEventData eventData in Hs.Event_Info_All())
			{
				ret.Add(eventData);
			}
			return ret;
		}

		///<summary>
		///List of all events in HomeSeer. Used to enable Linq queries on events.
		///</summary>
		///<returns>Generic.List() of all EventData</returns>
		///<remarks>By Moskus</remarks>
		public List<HomeSeerAPI.strEventData> Events1()
		{
			var ret = new List<HomeSeerAPI.strEventData>();
			var allEvents = Hs.Event_Info_All().ToList();
			return allEvents;
		}

        ///<summary>
		///Some common conversion methods
		///</summary>	
		///<remarks>By simplextech</remarks>
        public double FtoC(double F)
        {
            var C = Math.Round((F - 32) * 5 / 9);

            return C;
        }

        public double inHgTommHg(double inHg)
        {
            var mmHg = Math.Round(inHg * 25.4);

            return mmHg;
        }

        public double inHgTohPa(double inHg)
        {
            var hPa = Math.Round(inHg * 3386.389);

            return hPa;
        }

        public double mphToFtsec(double mph)
        {
            var ftSec = Math.Round(mph * 1.467);

            return ftSec;
        }

        public double mphToMsec(double mph)
        {
            var mSec = Math.Round(mph / 2.237);

            return mSec;
        }

        public double mphToKmHr(double mph)
        {
            var kmHr = Math.Round(mph * 1.609);

            return kmHr;
        }

        public double mphToKnots(double mph)
        {
            var knots = Math.Round(mph / 1.151);

            return knots;
        }

        public double inHrTommHr(double inHr)
        {
            var mmHr = Math.Round(inHr * 25.4);

            return mmHr;
        }

        public double wm2ToLux(double wm2)
        {
            var lux = Math.Round(wm2 / 0.0079);

            return lux;
        }

        public double percentToIndex(double percent)
        {
            double index = 0;

            if (percent == 0)
            {
                index = 1;
                return index;
            }
            else if (percent == 7)
            {
                index = 2;
                return index;
            }
            else if (percent == 13)
            {
                index = 3;
                return index;
            }
            else if (percent == 20)
            {
                index = 4;
                return index;
            }
            else if (percent == 27)
            {
                index = 5;
                return index;
            }
            else if (percent == 33)
            {
                index = 6;
                return index;
            }
            else if (percent == 40)
            {
                index = 7;
                return index;
            }
            else if (percent == 47)
            {
                index = 8;
                return index;
            }
            else if (percent == 53)
            {
                index = 9;
                return index;
            }
            else if (percent == 60)
            {
                index = 10;
                return index;
            }
            else if (percent == 67)
            {
                index = 11;
                return index;
            }
            else if (percent == 73)
            {
                index = 12;
                return index;
            }
            else if (percent == 80)
            {
                index = 13;
                return index;
            }
            else if (percent == 87)
            {
                index = 14;
                return index;
            }
            else if (percent == 93)
            {
                index = 15;
                return index;
            }
            else if (percent == 99)
            {
                index = 16;
                return index;
            }
            else { return 0; }
        }

        public double kgToPound(double kg)
        {
            var pound = Math.Round(kg * 2.05);

            return pound;
        }

        public double MeterToMile(double meter)
        {
            double mile = Math.Round(meter / 1609.344, 2);

            return mile;
        }

        public string SecondsToTime(double seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            //string str = time.ToString(@"hh\:mm\:ss\:fff");
            string str = time.ToString(@"hh\:mm\:ss");

            return str;
        }
    }
}