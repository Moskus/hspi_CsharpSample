using System;
using System.Text;
using Scheduler;

namespace hspi_CsharpSample.Config
{
	public class WebConfig : PageBuilderAndMenu.clsPageBuilder
	{
		private bool _timerEnabled;
		private Settings _settings;

		///<summary>
		///This creates a new instance of this page
		///</summary>
		///<param name="pagename"></param>
		public WebConfig(string pageName,Settings settings) : base(pageName)
		{
			_settings = settings;
		}



		//Public Sub New(ByVal pagename As String)

		//	MyBase.New(pagename)
		//End Sub

		///<summary>
		///This handles data
		///</summary>
		///<param name="page"></param>
		///<param name="data"></param>
		///<param name="user"></param>
		///<param name="userRights"></param>
		///<returns></returns>
		public string PostBackProc(string page, string data, string user, int userRights)
		{
			//	Dim parts As Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(data)

			//       ''Useful for debugging. :)
			//       'Console.WriteLine("Postback parts:")
			//       'For Each p In parts
			//       '    Console.WriteLine(p.ToString & vbTab & parts(p))
			//       'Next


			//       'Gets the control that caused the postback and handles accordingly
			//       Select Case parts("id")


			//		Case "oTextbox1"
			//               'This gets the value that was entered in the specified textbox
			//               Dim message As String = parts("Textbox1")

			//               '... posts it to the page

			//			PostMessage(message)

			//               '... and rebuilds the viewed textbox to contain the message
			//               BuildTextBox("Textbox1", True, message)


			//		Case "oCheckboxLogTimer"
			//               Console.WriteLine("oCheckboxLogTimer: " & parts("CheckboxLogTimer"))
			//               Dim logtimer As Boolean = IIf(parts("CheckboxLogTimer") = "checked", True, False) 'Why HST just couldn't return the strings "True" or "False" as .NET does I really don't know.
			//               plugin.Settings.LogTimerElapsed = logtimer
			//			BuildCheckbox("CheckboxLogTimer", True)


			//			PostMessage("Log timer: " & logtimer)


			//		Case "oCheckboxDebugLogging"
			//               Console.WriteLine("oCheckboxDebugLogging: " & parts("CheckboxDebugLogging"))
			//               Dim log As Boolean = IIf(parts("CheckboxDebugLogging") = "checked", True, False)

			//			plugin.Settings.DebugLog = log
			//			BuildCheckbox("CheckboxDebugLogging", True)


			//			PostMessage("Log timer: " & log)

			//		Case "oDropListLocation"
			//               Dim location As String = parts("DropListLocation")

			//			plugin.Settings.Location = location
			//			BuildTextBox("TextboxLocation", True, location)

			//			BuildDropList("DropListLocation", True)


			//			PostMessage("New location: " & location)


			//		Case "oTextboxLocation"
			//               Dim location As String = parts("TextboxLocation")

			//			plugin.Settings.Location = location
			//			BuildTextBox("TextboxLocation", True, location)

			//			BuildDropList("DropListLocation", True)


			//			PostMessage("New location: " & location)


			//		Case "oDropListLocation2"
			//               Dim location As String = parts("DropListLocation2")

			//			plugin.Settings.Location2 = location
			//			BuildTextBox("TextboxLocation2", True, location)

			//			BuildDropList("DropListLocation2", True)


			//			PostMessage("New location2: " & location)


			//		Case "oTextboxLocation2"
			//               Dim location As String = parts("TextboxLocation2")

			//			plugin.Settings.Location2 = location
			//			BuildTextBox("TextboxLocation2", True, location)

			//			BuildDropList("DropListLocation2", True)


			//			PostMessage("New location2: " & location)


			//		Case "oButton1"
			//               'This button navigates to the sample status page.
			//               Me.pageCommands.Add("newpage", plugin.Name & "Status")

			//           Case "timer"
			//               'This stops the timer and clears the message
			//               If TimerEnabled Then 'this handles the initial timer post that occurs immediately upon enabling the timer.
			//                   TimerEnabled = False
			//			Else

			//				Me.pageCommands.Add("stoptimer", "")
			//                   Me.divToUpdate.Add("message", "&nbsp;")
			//               End If

			//	End Select


			//       'For some reason I can't figure out, the timespan control doesn't submit it's ID when performing postback, so we need to take special care of it
			//	If parts.Item("oTimespanTimer") IsNot Nothing Then
			//		Dim span As String = parts("oTimespanTimer")

			//		Console.WriteLine("oTimespanTimer: " & span)

			//           'The timespan is a string on this format: days.hours:minutes:seconds, and can be parsed to a .NET timespan. :)
			//           Dim timespan As TimeSpan = timespan.Parse(span)

			//           'Saving the new timer interval and restarting the timer

			//		plugin.Settings.TimerInterval = timespan.TotalSeconds* 1000
			//           plugin.RestartTimer()

			//           '.. and since this doesn't "auto save" (see "Settings.vb") we need to save the settings.
			//		plugin.Settings.Save()


			//		BuildTimespan("TimespanTimer", True)

			//		PostMessage("New timer: " & timespan.ToString)


			//	End If

			return base.postBackProc(page, data, user, userRights);
		}

		public string GetPagePlugin(string pageName, string user, int userRights, string queryString)
		{
			var stb = new StringBuilder();
			var instanceText = "";
			try
			{
				this.reset();

				//		currentPage = Me

				//           ' handle any queries like mode=something
				//           Dim parts As Collections.Specialized.NameValueCollection = Nothing
				//		If queryString<> String.Empty Then parts = HttpUtility.ParseQueryString(queryString)

				//		If instance <> "" Then instancetext = " - " & instance

				//           'For some reason, I can't get the sample to add the title. So let's add it here.

				//		stb.Append("<title>" & plugin.Name & " " & pageName.Replace(plugin.Name, "") & "</title>")

				//           'Add menus and headers
				//           stb.Append(hs.GetPageHeader(pageName, plugin.Name & " " & pageName.Replace(plugin.Name, "") & instancetext, "", "", False, False))

				//           'Adds the div for the plugin page
				//           stb.Append(clsPageBuilder.DivStart("pluginpage", ""))

				//           ' a message area for error messages from jquery ajax postback (optional, only needed if using AJAX calls to get data)
				//           stb.Append(clsPageBuilder.DivStart("errormessage", "class='errormessage'"))
				//           stb.Append(clsPageBuilder.DivEnd)

				//           'Configures the timer that all pages apparently has
				//           Me.RefreshIntervalMilliSeconds = 3000
				//           stb.Append(Me.AddAjaxHandlerPost("id=timer", pageName)) 'This is so we can control it in postback

				//           ' specific page starts here
				//           stb.Append(BuildContent)

				//           'Ends the div end tag for the plugin page
				//           stb.Append(clsPageBuilder.DivEnd)

				//           ' add the body html to the page
				//           Me.AddBody(stb.ToString)

				//           ' return the full page
				return this.BuildPage();
			}
			catch (Exception ex)
			{
				//WriteMon("Error", "Building page: " & ex.Message)
				return "ERROR in GetPagePlugin: " + ex.Message;
			}
		}

		private void PostMessage(string message)
		{
			//Updates the div
			this.divToUpdate.Add("message", message);

			//Starts the pages built-in timer
			this.pageCommands.Add("starttimer", "");

			//... and updates the local variable so we can easily check if the timer is running
			_timerEnabled = true;
		}

		private string BuildContent()
		{
			var stb = new StringBuilder();
			//Lets have a table at the top for messages
			stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>");
			stb.Append(
				" <tr><td width='1000' align='center' style='color:#FF0000; font-size:14pt; height:30px;'><strong><div id='message'>&nbsp;</div></strong></tr>");
			stb.Append(" </table>");

			//... and a table below with our controls 'class='tablecolumn'
			stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>");
			//stb.Append("  <tr class='tableheader'><td width='250'>Choose caption:</td><td width='750'>" + BuildTextBox("Textbox1") + "</td></tr>"); //Leftovers from the HSPI_SAMPLE_BASIC

			stb.Append("  <tr class='tableheader'><td width='250'>" + Utils.PluginName + "</td><td width='750'>" +
					   "Settings for the plugin" + "</td></tr>");
			stb.Append("  <tr class='tableroweven'><td>Select default Floor for new devices (Location2):</td><td>" +
					   BuildDropList("DropListLocation2") + "&nbsp;" + BuildTextBox("TextboxLocation2") + "</td></tr>");
			stb.Append("  <tr class='tableroweven'><td>Select default Room for new devices (Location):</td><td>" +
					   BuildDropList("DropListLocation") + "&nbsp;" + BuildTextBox("TextboxLocation") + "</td></tr>");
			stb.Append("  <tr class='tablerowodd'><td>Write timer to the HomeSeer log:</td><td>" +
					   BuildCheckbox("CheckboxLogTimer") + "</td></tr>");
			stb.Append("  <tr class='tablerowodd'><td>Debug logging:</td><td>" + BuildCheckbox("CheckboxDebugLogging") +
					   "</td></tr>");
			stb.Append("  <tr class='tablerowodd'><td>Select the interval:</td><td>" + BuildTimespan("TimespanTimer") +
					   "</td></tr>");
			//stb.Append("  <tr class='tablerowodd'><td>&nbsp;</td><td>" + BuildButton("Button1") + "</td></tr>"); //Leftovers from the HSPI_SAMPLE_BASIC

			stb.Append(" </table>");
			return stb.ToString();
		}


		public string BuildTextBox(string name, bool rebuilding = false, string text = "")
		{
			var textBox = new clsJQuery.jqTextBox(name, "", text, this.PageName, 20, false);
			var ret = "";

			//	textBox.id = "o" & Name

			//       'If it's a location textbox, we want it to be "filled! with the current setting

			//	Select Case Name
			//		Case "TextboxLocation"

			//			textBox.defaultText = plugin.Settings.Location
			//		Case "TextboxLocation2"

			//			textBox.defaultText = plugin.Settings.Location2
			//	End Select

			//	If Rebuilding Then

			//		ret = textBox.Build
			//		Me.divToUpdate.Add(Name & "_div", ret)

			//	Else
			//		ret = "<div style='float: left;'  id='" & Name & "_div'>" & textBox.Build & "</div>"

			//	End If


			return ret;

		}


		private string BuildButton(string name, bool rebuilding = false)
		{

			var button = new clsJQuery.jqButton(name, "", this.PageName, false);
			var buttonText = "Submit";

			var ret = String.Empty;

			//       'Handles the text for different buttons, based on the button name

			//	Select Case Name
			//		Case "Button1"

			//			buttonText = "Go To Status Page"
			//               button.submitForm = False
			//	End Select

			//	button.id = "o" & Name
			//	button.label = buttonText


			//	ret = button.Build


			//	If Rebuilding Then
			//		Me.divToUpdate.Add(Name & "_div", ret)

			//	Else
			//		ret = "<div style='float: left;' id='" & Name & "_div'>" & ret & "</div>"

			//	End If

			return ret;

		}


		public string BuildDropList(string name, bool rebuilding = false)
		{

			var ddl = new clsJQuery.jqDropList(name, this.PageName, false);
			ddl.id = "o" + name;
			ddl.autoPostBack = true;

			//Get the items we want to have in the drop down list

			//	Dim items As New List(Of String)
			//	Dim selecteditem As String = String.Empty

			//	Select Case Name
			//		Case "DropListLocation"

			//			items = (From d In Devices()
			//					 Select Value = d.Location(hs)

			//					 Order By Value Ascending

			//					 Distinct).ToList
			//			selecteditem = plugin.Settings.Location


			//		Case "DropListLocation2"
			//               items = (From d In Devices()

			//					 Select Value = d.Location2(hs)

			//					 Order By Value Ascending

			//					 Distinct).ToList
			//			selecteditem = plugin.Settings.Location2

			//	End Select

			//       '... and add them to the control with a blank on top

			//	ddl.AddItem("", "", True)
			//       For Each s In items
			//		ddl.AddItem(s, s, (s = selecteditem))
			//       Next


			string ret = string.Empty;
			//	If Rebuilding Then

			//		ret = ddl.Build
			//		Me.divToUpdate.Add(Name & "_div", ret)

			//	Else
			//		ret = "<div style='float: left;'  id='" & Name & "_div'>" & ddl.Build & "</div>"

			//	End If
			return ret;
		}


		public string BuildCheckbox(string name, bool rebuilding = false, string label = "")
		{
			//	Dim checkbox As New clsJQuery.jqCheckBox(Name, label, Me.PageName, True, False)
			//	checkbox.id = "o" & Name

			//	Select Case Name

			//		Case "CheckboxLogTimer"

			//			checkbox.checked = plugin.Settings.LogTimerElapsed
			//		Case "CheckboxDebugLogging"

			//			checkbox.checked = plugin.Settings.DebugLog
			//	End Select

			var ret = String.Empty;

			//	If Rebuilding Then
			//		ret = checkbox.Build

			//		Me.divToUpdate.Add(Name & "_div", ret)
			//	Else
			//		ret = "<div style='float: left;'  id='" & Name & "_div'>" & checkbox.Build & "</div>"
			//	End If
			return ret;
		}

		public string BuildTimespan(string name, bool rebuilding = false, string label = "")
		{
			var timespan = new clsJQuery.jqTimeSpanPicker(name, label, this.PageName, false);
			timespan.id = "o" + name;
			timespan.showDays = false;

			if(name=="TimespanTimer")
			{
				timespan.defaultTimeSpan = new TimeSpan(0, 0, _settings.TimerInterval / 1000);
			}

			var ret = String.Empty;
			if (rebuilding)
			{
				ret = timespan.Build();
				this.divToUpdate.Add(name + "_div", ret);
			}
			else
			{
				ret = "<div style='float: left;'  id='" + name + "_div'>" + timespan.Build() + "</div>";
			}

			return ret;
		}
	}
}