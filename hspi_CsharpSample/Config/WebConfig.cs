using System;
using Scheduler;

namespace hspi_CsharpSample.Config
{
	public class WebConfig: PageBuilderAndMenu.clsPageBuilder
	{
		private bool _timerEnabled;

		///<summary>
		///This creates a new instance of this page
		///</summary>
		///<param name="pagename"></param>
		public WebConfig(string pageName) : base(pageName)
		{

		}

		internal string GetPagePlugin(string pageName, string user, int userRights, string queryString)
		{
			throw new NotImplementedException();
		}

		//Public Sub New(ByVal pagename As String)

		//	MyBase.New(pagename)
		//End Sub

		//   ''' <summary>
		//   ''' This handles data
		//   ''' </summary>
		//   ''' <param name="page"></param>
		//   ''' <param name="data"></param>
		//   ''' <param name="user"></param>
		//   ''' <param name="userRights"></param>
		//   ''' <returns></returns>
		//   Public Overrides Function postBackProc(page As String, data As String, user As String, userRights As Integer) As String

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




		//	Return MyBase.postBackProc(page, data, user, userRights)

		//End Function


		//Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String

		//	Dim stb As New StringBuilder
		//	Dim instancetext As String = ""
		//       Try
		//		Me.reset()

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
		//           Return Me.BuildPage()

		//	Catch ex As Exception
		//           'WriteMon("Error", "Building page: " & ex.Message)

		//		Return "ERROR in GetPagePlugin: " & ex.Message
		//	End Try
		//End Function

		//Private Sub PostMessage(ByVal message As String)
		//       'Updates the div
		//       Me.divToUpdate.Add("message", message)

		//       'Starts the pages built-in timer
		//       Me.pageCommands.Add("starttimer", "")

		//       '... and updates the local variable so we can easily check if the timer is running
		//       TimerEnabled = True
		//End Sub

		//Function BuildContent() As String

		//	Dim stb As New StringBuilder
		//       'Lets have a table at the top for messages
		//       stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>")
		//       stb.Append(" <tr><td width='1000' align='center' style='color:#FF0000; font-size:14pt; height:30px;'><strong><div id='message'>&nbsp;</div></strong></tr>")
		//       stb.Append(" </table>")

		//       '... and a table below with our controls 'class='tablecolumn'
		//       stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>")
		//       'stb.Append("  <tr class='tableheader'><td width='250'>Choose caption:</td><td width='750'>" & BuildTextBox("Textbox1") & "</td></tr>") 'Leftovers from the HSPI_SAMPLE_BASIC

		//	stb.Append("  <tr class='tableheader'><td width='250'>" & plugin.Name & "</td><td width='750'>" & "Settings for the plugin" & "</td></tr>")
		//       stb.Append("  <tr class='tableroweven'><td>Select default Floor for new devices (Location2):</td><td>" & BuildDropList("DropListLocation2") & "&nbsp;" & BuildTextBox("TextboxLocation2") & "</td></tr>")
		//       stb.Append("  <tr class='tableroweven'><td>Select default Room for new devices (Location):</td><td>" & BuildDropList("DropListLocation") & "&nbsp;" & BuildTextBox("TextboxLocation") & "</td></tr>")
		//       stb.Append("  <tr class='tablerowodd'><td>Write timer to the HomeSeer log:</td><td>" & BuildCheckbox("CheckboxLogTimer") & "</td></tr>")
		//       stb.Append("  <tr class='tablerowodd'><td>Debug logging:</td><td>" & BuildCheckbox("CheckboxDebugLogging") & "</td></tr>")
		//       stb.Append("  <tr class='tablerowodd'><td>Select the interval:</td><td>" & BuildTimespan("TimespanTimer") & "</td></tr>")
		//       'stb.Append("  <tr class='tablerowodd'><td>&nbsp;</td><td>" & BuildButton("Button1") & "</td></tr>") 'Leftovers from the HSPI_SAMPLE_BASIC

		//	stb.Append(" </table>")
		//       Return stb.ToString

		//End Function


		//Public Function BuildTextBox(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False, Optional ByVal Text As String = "") As String

		//	Dim textBox As New clsJQuery.jqTextBox(Name, "", Text, Me.PageName, 20, False)
		//       Dim ret As String = ""

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


		//	Return ret

		//End Function


		//Function BuildButton(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False) As String

		//	Dim button As New clsJQuery.jqButton(Name, "", Me.PageName, False)
		//       Dim buttonText As String = "Submit"

		//	Dim ret As String = String.Empty

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

		//	Return ret

		//End Function


		//Public Function BuildDropList(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False) As String

		//	Dim ddl As New clsJQuery.jqDropList(Name, Me.PageName, False)
		//	ddl.id = "o" & Name
		//	ddl.autoPostBack = True

		//       'Get the items we want to have in the drop down list

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


		//	Dim ret As String
		//	If Rebuilding Then

		//		ret = ddl.Build
		//		Me.divToUpdate.Add(Name & "_div", ret)

		//	Else
		//		ret = "<div style='float: left;'  id='" & Name & "_div'>" & ddl.Build & "</div>"

		//	End If

		//	Return ret

		//End Function


		//Public Function BuildCheckbox(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False, Optional ByVal label As String = "") As String

		//	Dim checkbox As New clsJQuery.jqCheckBox(Name, label, Me.PageName, True, False)
		//	checkbox.id = "o" & Name

		//	Select Case Name

		//		Case "CheckboxLogTimer"

		//			checkbox.checked = plugin.Settings.LogTimerElapsed
		//		Case "CheckboxDebugLogging"

		//			checkbox.checked = plugin.Settings.DebugLog
		//	End Select

		//	Dim ret As String = String.Empty

		//	If Rebuilding Then
		//		ret = checkbox.Build

		//		Me.divToUpdate.Add(Name & "_div", ret)
		//	Else
		//		ret = "<div style='float: left;'  id='" & Name & "_div'>" & checkbox.Build & "</div>"
		//	End If
		//	Return ret
		//End Function


		//Public Function BuildTimespan(ByVal Name As String, Optional ByVal Rebuilding As Boolean = False, Optional ByVal label As String = "") As String
		//	Dim timespan As New clsJQuery.jqTimeSpanPicker(Name, label, Me.PageName, False)
		//	timespan.id = "o" & Name
		//	timespan.showDays = False

		//	Select Case Name
		//		Case "TimespanTimer"
		//			timespan.defaultTimeSpan = New TimeSpan(0, 0, plugin.Settings.TimerInterval / 1000)
		//	End Select

		//	Dim ret As String = String.Empty
		//	If Rebuilding Then
		//		ret = timespan.Build
		//		Me.divToUpdate.Add(Name & "_div", ret)
		//       Else
		//		ret = "<div style='float: left;'  id='" & Name & "_div'>" & timespan.Build & "</div>"
		//	End If
		//	Return ret
		//End Function
	}
}