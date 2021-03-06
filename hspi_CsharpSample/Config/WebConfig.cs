﻿using System;
using System.Text;
using HomeSeerAPI;
using Scheduler;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scheduler.Classes;

namespace HSPI_CsharpSample.Config
{
	public class WebConfig : PageBuilderAndMenu.clsPageBuilder
	{
		private bool _timerEnabled;
		private Plugin _plugin;
		private Settings _settings;
		private IHSApplication _hs;
		private Utils _utils;
		///<summary>
		///This creates a new instance of this page
		///</summary>
		///<param name="pagename"></param>
		///<param name="settings"></param>
		///<param name="hs"></param>
		public WebConfig(string pageName, Settings settings, IHSApplication hs, Plugin plugin, Utils utils) : base(pageName)
		{
			_plugin = plugin;
			_settings = settings;
			_hs = hs;
			_utils = utils;
		}

		///<summary>
		///This handles data
		///</summary>
		///<param name="page"></param>
		///<param name="data"></param>
		///<param name="user"></param>
		///<param name="userRights"></param>
		///<returns></returns>
		public override string postBackProc(string page, string data, string user, int userRights)
		{
			NameValueCollection parts = HttpUtility.ParseQueryString(data);

			//Useful for debugging. :)
			//Console.WriteLine("Postback parts:")
			//foreach(var p In parts)
			//{
			//    Console.WriteLine(p.ToString + "\t" + parts[p]);
			//}


			//Gets the control that caused the postback and handles accordingly
			string location;
			switch (parts["id"])
			{
				case "oTextbox1":
					//This gets the value that was entered in the specified textbox
					var message = parts["Textbox1"];
					//... posts it to the page
					PostMessage(message);
					//... and rebuilds the viewed textbox to contain the message
					BuildTextBox("Textbox1", true, message);
					break;

				case "oCheckboxLogTimer":
					Console.WriteLine("oCheckboxLogTimer: " + parts["CheckboxLogTimer"]);
					bool logtimer = (parts["CheckboxLogTimer"] == "checked") ? true : false; //Why HST just couldn't return the strings "True" or "False" as .NET does I really don't know
					_settings.LogTimerElapsed = logtimer;
					BuildCheckbox("CheckboxLogTimer", true);
					PostMessage("Log timer: " + logtimer);
					break;

				case "oCheckboxDebugLogging":
					Console.WriteLine("oCheckboxDebugLogging: " + parts["CheckboxDebugLogging"]);
					bool log = (parts["CheckboxDebugLogging"] == "checked") ? true : false;
					_settings.DebugLog = log;
					BuildCheckbox("CheckboxDebugLogging", true);
					PostMessage("Debug logging: " + log);
					break;

				case "oDropListLocation":
					location = parts["DropListLocation"];
					_settings.Location = location;
					BuildTextBox("TextboxLocation", true, location);
					BuildDropList("DropListLocation", true);
					PostMessage("New location: " + location);
					break;

				case "oTextboxLocation":
					location = parts["TextboxLocation"];
					_settings.Location = location;
					BuildTextBox("TextboxLocation", true, location);
					BuildDropList("DropListLocation", true);
					PostMessage("New location: " + location);
					break;

				case "oDropListLocation2":
					location = parts["DropListLocation2"];
					_settings.Location2 = location;
					BuildTextBox("TextboxLocation2", true, location);
					BuildDropList("DropListLocation2", true);
					PostMessage("New location2: " + location);
					break;

				case "oTextboxLocation2":
					location = parts["TextboxLocation2"];
					_settings.Location2 = location;
					BuildTextBox("TextboxLocation2", true, location);
					BuildDropList("DropListLocation2", true);
					PostMessage("New location2: " + location);
					break;

				case "oButton1":
					//This button navigates to the sample status page.
					this.pageCommands.Add("newpage", Utils.PluginName + "Status");
					break;

				case "timer":
					//This stops the timer and clears the message
					if (_timerEnabled) //'this handles the initial timer post that occurs immediately upon enabling the timer.
					{
						_timerEnabled = false;
					}
					else
					{
						this.pageCommands.Add("stoptimer", "");
						this.divToUpdate.Add("message", "&nbsp;");
					}
					break;
			}

			//For some reason I can't figure out, the timespan control doesn't submit it's ID when performing postback, so we need to take special care of it
			if (!string.IsNullOrEmpty(parts["oTimespanTimer"]))
			{
				var span = parts["oTimespanTimer"];
				Console.WriteLine("oTimespanTimer: " + span);
				//The timespan is a string on this format: days.hours:minutes:seconds, and can be parsed to a .NET timespan. :)
				var timespan = TimeSpan.Parse(span);
				//Saving the new timer interval and restarting the timer
				_settings.TimerInterval = (int)timespan.TotalSeconds * 1000;
				_plugin.RestartTimer();
				//.. and since this doesn't "auto save" (see "Settings.vb") we need to save the settings.
				_settings.Save();
				BuildTimespan("TimespanTimer", true);
				PostMessage("New timer: " + timespan.ToString());
			}
			return base.postBackProc(page, data, user, userRights);
		}

		public string GetPagePlugin(string pageName, string user, int userRights, string queryString)
		{
			var stb = new StringBuilder();
			var instanceText = "";
			try
			{
				this.reset();

				// handle any queries like mode=something
				NameValueCollection parts = null;
				if (!string.IsNullOrEmpty(queryString))
				{
					parts = HttpUtility.ParseQueryString(queryString);
				}

				if (!string.IsNullOrEmpty(_plugin.PluginInstance))
				{
					instanceText = " - " + _plugin.PluginInstance;
				}

				//For some reason, I can't get the sample to add the title. So let's add it here.
				stb.Append("<title>" + Utils.PluginName + " " + pageName.Replace(Utils.PluginName, "") + "</title>");

				//Add menus and headers
				stb.Append(_hs.GetPageHeader(pageName,
					Utils.PluginName + " " + pageName.Replace(Utils.PluginName, "") + instanceText, "",
					"", false, false));

				//Adds the div for the plugin page
				stb.Append(PageBuilderAndMenu.clsPageBuilder.DivStart("pluginpage", ""));

				// a message area for error messages from jquery ajax postback (optional, only needed if using AJAX calls to get data)
				stb.Append(PageBuilderAndMenu.clsPageBuilder.DivStart("errormessage", "class='errormessage'"));
				stb.Append(PageBuilderAndMenu.clsPageBuilder.DivEnd());

				//Configures the timer that all pages apparently has
				this.RefreshIntervalMilliSeconds = 3000;
				stb.Append(this.AddAjaxHandlerPost("id=timer", pageName)); //This is so we can control it in postback

				// specific page starts here
				stb.Append(BuildContent());

				//Ends the div end tag for the plugin page
				stb.Append(PageBuilderAndMenu.clsPageBuilder.DivEnd());

				// add the body html to the page
				this.AddBody(stb.ToString());

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

			textBox.id = "o" + name;
			//If it's a location textbox, we want it to be "filled! with the current setting

			switch (name)
			{
				case "TextboxLocation":
					textBox.defaultText = _settings.Location;
					break;
				case "TextboxLocation2":
					textBox.defaultText = _settings.Location2;
					break;
			}

			if (rebuilding)
			{
				ret = textBox.Build();
				this.divToUpdate.Add(name + "_div", ret);
			}
			else
			{
				ret = "<div style='float: left;'  id='" + name + "_div'>" + textBox.Build() + "</div>";
			}
			return ret;
		}


		private string BuildButton(string name, bool rebuilding = false)
		{
			var button = new clsJQuery.jqButton(name, "", this.PageName, false);
			var buttonText = "Submit";

			var ret = string.Empty;

			//'Handles the text for different buttons, based on the button name
			switch (name)
			{
				case "Button1":
					buttonText = "Go To Status Page";
					button.submitForm = false;
					break;
			}

			button.id = "o" + name;
			button.label = buttonText;
			ret = button.Build();

			if (rebuilding)
			{
				this.divToUpdate.Add(name + "_div", ret);
			}
			else
			{
				ret = "<div style='float: left;' id='" + name + "_div'>" + ret + "</div>";
			}

			return ret;
		}


		public string BuildDropList(string name, bool rebuilding = false)
		{
			var ddl = new clsJQuery.jqDropList(name, this.PageName, false);
			ddl.id = "o" + name;
			ddl.autoPostBack = true;

			//Get the items we want to have in the drop down list
			var items = new List<string>();
			var selectedItem = string.Empty;

			switch (name)
			{
				case "DropListLocation":
					items = _utils.Devices().Select(x => x.get_Location(_hs)).Distinct().OrderBy(x => x).ToList();
					selectedItem = _settings.Location;
					break;

				case "DropListLocation2":
					items = _utils.Devices().Select(x => x.get_Location2(_hs)).Distinct().OrderBy(x => x).ToList();
					selectedItem = _settings.Location2;
					break;
			}

			//... and add them to the control with a blank on top
			ddl.AddItem("", "", true);
			foreach (var item in items)
			{
				ddl.AddItem(item, item, (item == selectedItem));
			}

			string ret = string.Empty;
			if (rebuilding)
			{
				ret = ddl.Build();
				this.divToUpdate.Add(name + "_div", ret);
			}
			else
			{
				ret = "<div style='float: left;'  id='" + name + "_div'>" + ddl.Build() + "</div>";
			}
			return ret;
		}


		public string BuildCheckbox(string name, bool rebuilding = false, string label = "")
		{
			var checkbox = new clsJQuery.jqCheckBox(name, label, this.PageName, true, false);
			checkbox.id = "o" + name;
			switch (name)
			{
				case "CheckboxLogTimer":
					checkbox.@checked = _settings.LogTimerElapsed;
					break;
				case "CheckboxDebugLogging":
					checkbox.@checked = _settings.DebugLog;
					break;
			}

			var ret = string.Empty;
			if (rebuilding)
			{
				ret = checkbox.Build();
				this.divToUpdate.Add(name + "_div", ret);
			}
			else
			{
				ret = "<div style='float: left;'  id='" + name + "_div'>" + checkbox.Build() + "</div>";
			}
			return ret;
		}

		public string BuildTimespan(string name, bool rebuilding = false, string label = "")
		{
			var timespan = new clsJQuery.jqTimeSpanPicker(name, label, this.PageName, false);
			timespan.id = "o" + name;
			timespan.showDays = false;
			if (name == "TimespanTimer")
			{
				timespan.defaultTimeSpan = new TimeSpan(0, 0, _settings.TimerInterval / 1000);
			}

			var ret = string.Empty;
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