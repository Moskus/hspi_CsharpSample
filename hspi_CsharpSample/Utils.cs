using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HomeSeerAPI;
using Scheduler;
using Scheduler.Classes;

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

		//<summary>
		//List of all devices in HomeSeer belonging to the plugin. Used to enable Linq queries on devices.
		//</summary>
		//<returns>Generic.List() of all devices</returns>
		//<remarks>By Moskus</remarks>
		public List<DeviceClass> Devices()
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
			SerializeObject(ref pedValue, ref byteObject);
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

			byte[] byteObject = ped.GetNamed(pedName);

			if (byteObject == null) return null;

			DeSerializeObject(byteObject, returnValue);

			return returnValue;

		}

		///<summary>
		///Used to serialize an object to a bytestream, which can be stored in a device ("PDE" or "clsPlugExtraData"), Action or Trigger
		///</summary>
		///<param name="objIn">Input object</param>
		///<param name="byteOut">Output bytes</param>
		///<returns>True/False success</returns>
		///<remarks>By HomeSeer</remarks>
		public bool SerializeObject(ref object objIn, ref byte[] byteOut)
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
			if (ObjOut == null) return false;

		//Else: Let's deserialize the bytes
			var memStream = new MemoryStream();

		Dim formatter As New Binary.BinaryFormatter

		Dim ObjTest As Object

		Dim TType As System.Type

		Dim OType As System.Type

		Try

			OType = ObjOut.GetType

			ObjOut = Nothing

			memStream = New MemoryStream(bteIn)

			ObjTest = formatter.Deserialize(memStream)

			If ObjTest Is Nothing Then Return False
			TType = ObjTest.GetType

			'If Not TType.Equals(OType) Then Return False


			ObjOut = ObjTest
			If ObjOut Is Nothing Then Return False

			Return True

		Catch exIC As InvalidCastException

			Return False

		Catch ex As Exception

			Log("DeSerializing object: " & ex.Message, LogType.Error)

			Return False

		End Try


		}

    ''' <summary>
    ''' Deletes all devices associated with the plugin.
    ''' </summary>
    ''' <remarks>Linq-ified by Moskus</remarks>

	Public Sub DeleteDevices()
        'Get all devices belonging to the plugin
        Dim devs = (From d In Devices()

					Where d.Interface(hs) = plugin.Name)

        'Deleting the devices
        For Each d In devs
			Try

				hs.DeleteDevice(d.Ref(hs))
            Catch ex As Exception

				Log("Could not delete device '" & d.Location2(hs) & " " & d.Location(hs) & " " & d.Name(hs) & "' (ref = " & d.Ref(hs) & "). Exception: " & ex.Message, LogType.Error)

			End Try


		Next
	End Sub

    ''' <summary>
    ''' SAMPLE PLUGIN SUB, not really used here.
    ''' Creates a new device
    ''' </summary>
    ''' <param name="PName"></param>
    ''' <param name="modNum"></param>
    ''' <param name="counter"></param>
    ''' <param name="ref"></param>
    ''' <returns></returns>
    ''' <remarks>By HomeSeer</remarks>
    Function InitDevice(ByVal PName As String, ByVal modNum As Integer, ByVal counter As Integer, Optional ByVal ref As Integer = 0) As Boolean

		Dim dv As Scheduler.Classes.DeviceClass = Nothing
		Log("Initiating Device " & PName, LogType.Normal)


		Try
			If Not hs.DeviceExistsRef(ref) Then
				ref = hs.NewDeviceRef(PName)
				Try

					dv = hs.GetDeviceByRef(ref)
					InitHSDevice(dv, PName)

					Return True

				Catch ex As Exception

					Log("Error initializing device " & PName & ": " & ex.Message, LogType.Error)

					Return False

				End Try

			End If

		Catch ex As Exception

			Log("InitDevice: Error getting RefID from deviceCode within InitDevice. (" & ex.Message & ")", LogType.Error)

		End Try

		Return False

	End Function

    ''' <summary>
    ''' SAMPLE PLUGIN SUB, not really used.
    ''' </summary>
    ''' <param name="dv"></param>
    ''' <param name="Name"></param>

	Sub InitHSDevice(ByRef dv As Scheduler.Classes.DeviceClass, Optional ByVal Name As String = "Optional_Sample_device_name")

		Dim DT As New DeviceTypeInfo_m.DeviceTypeInfo
		DT.Device_Type = DeviceTypeInfo_m.DeviceTypeInfo.eDeviceAPI.Plug_In
		dv.DeviceType_Set(hs) = DT

		dv.Interface(hs) = plugin.Name

		dv.InterfaceInstance(hs) = instance

		dv.Last_Change(hs) = Now

		dv.Name(hs) = Name

		dv.Location(hs) = plugin.Name

		dv.Device_Type_String(hs) = plugin.Name

		dv.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES)
		dv.MISC_Set(hs, Enums.dvMISC.NO_LOG)
		dv.Status_Support(hs) = False 'Set to True if the devices can be polled, false if not

	End Sub

    ''' <summary>
    ''' SAMPLE PROJECT SUB.
    ''' This is just an example to process some data provided when SetIOmulti was triggered.
    ''' SetIO multi sends the HouseCode, deviceCode and Action to this function.
    ''' </summary>
    ''' <param name="houseCode"></param>
    ''' <param name="Devicecode"></param>
    ''' <remarks></remarks>

	Public Sub SendCommand(ByVal houseCode As String, ByVal Devicecode As String)
        'Send a command somewhere, but for now, just log it
        hs.WriteLog("MoskusSample", "utils.vb -> SendCommand. HouseCode: " & houseCode & " - DeviceCode: " & Devicecode)
    End Sub

    ''' <summary>
    ''' Registers the web page in HomeSeer
    ''' </summary>
    ''' <param name="link">A short link to the page</param>
    ''' <param name="linktext">The text to be shown</param>
    ''' <param name="page_title">The title of the page when loaded</param>
    ''' <remarks>HSPI_SAMPLE_BASIC</remarks>

	Public Sub RegisterWebPage(ByVal link As String, Optional linktext As String = "", Optional page_title As String = "")

		Try
			Dim the_link As String = link
			hs.RegisterPage(the_link, plugin.Name, instance)


			If linktext = "" Then linktext = link

			linktext = linktext.Replace("_", " ").Replace(plugin.Name, "")

			If page_title = "" Then page_title = linktext


			Dim webPageDescription As New HomeSeerAPI.WebPageDesc
			webPageDescription.plugInName = plugin.Name
			webPageDescription.link = the_link

			webPageDescription.linktext = linktext & instance

			webPageDescription.page_title = page_title & instance


			callback.RegisterLink(webPageDescription)
		Catch ex As Exception
			Log("Registering Web Links (RegisterWebPage): " & ex.Message, LogType.Error)

		End Try

	End Sub

    ''' <summary>
    ''' Logging
    ''' </summary>
    ''' <param name="Message">The message to be logged</param>
    ''' <param name="Log_Level">Normal, Warning or Error</param>
    ''' <remarks>HSPI_SAMPLE_BASIC</remarks>

	Public Sub Log(ByVal Message As String, Optional ByVal Log_Level As LogType = LogType.Normal)

		Select Case Log_Level
			Case LogType.Debug
				If plugin.Settings.DebugLog Then hs.WriteLog(plugin.Name & " Debug", Message)

            Case LogType.Normal

				hs.WriteLog(plugin.Name, Message)

			Case LogType.Warning
				hs.WriteLog(plugin.Name & " Warning", Message)


			Case LogType.Error

				hs.WriteLog(plugin.Name & " ERROR", Message)

		End Select

	End Sub

    ''' <summary>
    ''' List of all devices in HomeSeer. Used to enable Linq queries on devices.
    ''' </summary>
    ''' <returns>Generic.List() of all devices</returns>
    ''' <remarks>By Moskus</remarks>

	Public Function Devices() As List(Of Scheduler.Classes.DeviceClass)

		Dim ret As New List(Of Scheduler.Classes.DeviceClass)

		Dim device As Scheduler.Classes.DeviceClass

		Dim DE As Scheduler.Classes.clsDeviceEnumeration

		DE = hs.GetDeviceEnumerator()

		Do While Not DE.Finished
			device = DE.GetNext

			ret.Add(device)
		Loop


		Return ret

	End Function

    ''' <summary>
    ''' List of all events in HomeSeer. Used to enable Linq queries on events.
    ''' </summary>
    ''' <returns>Generic.List() of all EventData</returns>
    ''' <remarks>By Moskus</remarks>

	Public Function Events() As List(Of HomeSeerAPI.strEventData)

		Dim ret As New List(Of HomeSeerAPI.strEventData)


		For Each e As HomeSeerAPI.strEventData In hs.Event_Info_All
			ret.Add(e)

		Next

		Return ret
	End Function

	}
}