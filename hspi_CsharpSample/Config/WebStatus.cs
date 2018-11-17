using System;
using System.Collections.Generic;
using HomeSeerAPI;
using Scheduler;
using System.Web;
using System.Collections.Specialized;
using System.Data;
using System.Text;

namespace hspi_CsharpSample.Config
{

	public class WebStatus : PageBuilderAndMenu.clsPageBuilder
	{
		private bool _timerEnabled;
		private List<string> _lbList = new List<string>();
		private DataTable _ddTable = null;
		private Settings _settings;

		///<summary>
		///This creates a new instance of this page
		///</summary>
		///<param name="pagename"></param>
		public WebStatus(string pageName, Settings settings) : base(pageName)
		{
			_settings = settings;
		}

		internal string GetPagePlugin(string pageName, string user, int userRights, string queryString)
		{
			throw new NotImplementedException();
		}

		public override string postBackProc(string page, string data, string user, int userRights)
		{
			NameValueCollection parts;
			string value = "";
			var name = "";

			parts = HttpUtility.ParseQueryString(data);

			switch (parts["id"])
			{
				case "oTabLB":
					PostMessage("The Listbox tab was selected.");
					break;

				case "oTabCB":
					PostMessage("The Checkbox tab was selected.");
					break;

				case "oTabDD":
					PostMessage("The Dropdown tab was selected.");
					break;

				case "oButtonLB":
					_lbList.Add(parts["TextboxLB"]);
					PostMessage("'" + parts["TextboxLB"] + "' has been added.");
					BuildTabLB(true);
					break;
				case "oButtonDD1":
					_ddTable.Rows.Add("DD1", parts["tb1"], parts["tb2"]);
					PostMessage("'" + parts["tb1"] + "' with a value of " + parts["tb2"] + " has been added.");
					BuildTabDD(true);
					break;

				case "oButtonDD2":
					_ddTable.Rows.Add("DD2", parts["tb1"], parts["tb2"]);
					PostMessage("'" + parts["tb1"] + "' with a value of " + parts["tb2"] + " has been added.");
					BuildTabDD(true);
					break;

				case "oCB1":
				case "oCB2":
					name = parts["id"];

					name = name.Substring(1); //strip the 'o' off the ID to get the name
					value = parts[name];
					PostMessage("Something was " + value + ".");
					break;

				case "oSlider":
					value = parts["Slider"];
					PostMessage("Value is " + value + ".");
					break;

				case "timer": //this stops the timer and clears the message
					if (_timerEnabled) //this handles the initial timer post that occurs immediately upon enabling the timer.
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

			return base.postBackProc(page, data, user, userRights);
		}

		//Public Function GetPagePlugin(ByVal pageName As String, ByVal user As String, ByVal userRights As Integer, ByVal queryString As String) As String

		//	var stb =new StringBuilder();
		//	Dim instancetext As String = ""
		//       Try

		//		Me.reset()

		//           ' handle any queries like mode=something
		//           Dim parts As Collections.Specialized.NameValueCollection = Nothing
		//		If (queryString<> "") Then
		//		   parts = HttpUtility.ParseQueryString(queryString)

		//		End If

		//		If instance<> "" Then instancetext = " - " & instance

		//		stb.Append(hs.GetPageHeader(pageName, plugin.Name & instancetext, "", "", False, False))

		//           stb.Append(PageBuilderAndMenu.clsPageBuilder.DivStart("pluginpage", ""))

		//           ' a message area for error messages from jquery ajax postback (optional, only needed if using AJAX calls to get data)
		//           stb.Append(PageBuilderAndMenu.clsPageBuilder.DivStart("errormessage", "class='errormessage'"))
		//           stb.Append(PageBuilderAndMenu.clsPageBuilder.DivEnd)

		//		Me.RefreshIntervalMilliSeconds = 3000
		//           stb.Append(Me.AddAjaxHandlerPost("id=timer", pageName))

		//           ' specific page starts here

		//           stb.Append(BuildContent)

		//		stb.Append(PageBuilderAndMenu.clsPageBuilder.DivEnd)

		//           ' add the body html to the page
		//           Me.AddBody(stb.ToString)

		//           ' return the full page
		//           Return Me.BuildPage()

		//	Catch ex As Exception
		//           'WriteMon("Error", "Building page: " & ex.Message)

		//		Return "error - " & ex.Message 'Err.Description
		//       End Try

		//End Function


		//Function BuildContent() As String

		//	var stb =new StringBuilder();
		//	stb.Append("<script>function userConfirm() {  confirm(""hey - are you sure??"");  }</script>")

		//	stb.Append(" <table border='0' cellpadding='0' cellspacing='0' width='1000'>")
		//       stb.Append(" <tr><td width='1000' align='center' style='color:#FF0000; font-size:14pt; height:30px;'><strong><div id='message'>&nbsp;</div></strong></tr>")
		//       stb.Append("  <tr><td class='tablecolumn' width='270'>" & BuildTabs() & "</td></tr>")
		//       stb.Append(" </table>")
		//       Return stb.ToString

		//End Function


		//Public Function BuildTabs() As String
		//	var stb =new StringBuilder();

		//	Dim tabs As clsJQuery.jqTabs = New clsJQuery.jqTabs("oTabs", Me.PageName)

		//	Dim tab=new clsJQuery.Tab

		//	tabs.postOnTabClick = True
		//	tab.tabTitle = "Listbox"

		//	tab.tabDIVID = "oTabLB"

		//	tab.tabContent = "<div id='TabLB_div'>" & BuildTabLB() & "</div>"

		//	tabs.tabs.Add(tab)
		//	tab = New clsJQuery.Tab
		//	tab.tabTitle = "Checkbox"

		//	tab.tabDIVID = "oTabCB"

		//	tab.tabContent = "<div id='TabCB_div'>" & BuildTabCB() & "</div>"

		//	tabs.tabs.Add(tab)
		//	tab = New clsJQuery.Tab
		//	tab.tabTitle = "Drop Down"

		//	tab.tabDIVID = "oTabDD"

		//	tab.tabContent = "<div id='TabDD_div'>" & BuildTabDD() & "</div>"

		//	tabs.tabs.Add(tab)
		//	tab = New clsJQuery.Tab
		//	tab.tabTitle = "Slider"

		//	tab.tabDIVID = "oTabSL"

		//	tab.tabContent = "<div id='TabSL_div'>" & BuildTabSL() & "</div>"

		//	tabs.tabs.Add(tab)
		//	tab = New clsJQuery.Tab
		//	tab.tabTitle = "Sliding Tab"

		//	tab.tabDIVID = "oTabST"

		//	tab.tabContent = "<div id='TabST_div'>" & BuildTabST() & "</div>"

		//	tabs.tabs.Add(tab)

		//	Return tabs.Build
		//End Function

		private string BuildTabLB(bool rebuilding = false)
		{
			var stb = new StringBuilder();
			var lb = new clsJQuery.jqListBox("lb", this.PageName);

			var ms1 = new clsJQuery.jqMultiSelect("ms1", this.PageName, true);
			var ms2 = new clsJQuery.jqMultiSelect("ms2", this.PageName, false);
			var sel = new clsJQuery.jqSelector("sel", this.PageName, false);
			var sel2 = new clsJQuery.jqSelector("sel2", this.PageName, false);
			ms2.width = 300;
			ms2.Size = 6;
			lb.style = "height:100px;width:300px;";

			foreach (var item in _lbList)
			{
				lb.items.Add(item);
				ms1.AddItem(item, item, false);
				ms2.AddItem(item, item, false);
				sel.AddItem(item, item, false);
			}


			stb.Append(PageBuilderAndMenu.clsPageBuilder.FormStart("frmTab1", "ListBox", "Post"));
			stb.Append(sel.Build() + ms1.Build() + ms2.Build() + "<br>");
			stb.Append(lb.Build() + " " + BuildTextbox("TextboxLB") + " " + BuildButton("ButtonLB"));
			stb.Append(sel2.Build() + "<br>");

			stb.Append(PageBuilderAndMenu.clsPageBuilder.FormEnd());
			if (rebuilding)
			{
				this.divToUpdate.Add("TabLB_div", stb.ToString());
			}
			return stb.ToString();

		}

		//Function BuildTabCB() As String
		//	var stb =new StringBuilder();

		//	Dim cb1=new clsJQuery.jqCheckBox("CB1", "check 1", Me.PageName, True, False)

		//	Dim cb2=new clsJQuery.jqCheckBox("CB2", "check 2", Me.PageName, True, True)
		//       Dim cb3=new clsJQuery.jqCheckBox("CB3", "check 3", Me.PageName, False, True)
		//       Dim cb4=new clsJQuery.jqCheckBox("CB4", "check 4", Me.PageName, False, False)

		//       cb1.id = "oCB1"
		//       cb2.id = "oCB2"
		//       cb3.id = "oCB3"
		//       cb4.id = "oCB4"

		//       stb.Append(PageBuilderAndMenu.clsPageBuilder.FormStart("frmTab2", "checkbox", "Post"))
		//       stb.Append(cb1.Build())
		//       stb.Append(cb2.Build())
		//       stb.Append(cb3.Build())
		//       stb.Append(cb4.Build())
		//       stb.Append(BuildButton("ButtonCB1") & " ")
		//       stb.Append(PageBuilderAndMenu.clsPageBuilder.FormEnd())
		//       Return stb.ToString

		//End Function


		private string BuildTabDD(bool rebuilding = false)
		{
			var stb = new StringBuilder();
			stb.Append(PageBuilderAndMenu.clsPageBuilder.FormStart("frmTab3", "DropDown", "Post"));
			stb.Append(BuildDD("dd1") + " ");
			stb.Append(BuildDD("dd2") + " ");
			stb.Append("Name:" + BuildTextbox("tb1") + " ");
			stb.Append("Value:" + BuildTextbox("tb2") + " ");
			stb.Append(BuildButton("ButtonDD1") + " ");
			stb.Append(BuildButton("ButtonDD2") + " ");
			stb.Append(PageBuilderAndMenu.clsPageBuilder.FormEnd());
			if (rebuilding)
			{
				this.divToUpdate.Add("TabDD_div", stb.ToString());
			}

			return stb.ToString();

		}

		//Function BuildTabSL() As String
		//	var stb =new StringBuilder();

		//	stb.Append(BuildSlider("Slider"))
		//       Return stb.ToString

		//End Function



		//Function BuildTabST(Optional ByVal Rebuilding As Boolean = False) As String

		//	var stb =new StringBuilder();
		//	Dim st=new clsJQuery.jqSlidingTab("myslide1ID", Me.PageName, False)

		//	st.initiallyOpen = True
		//	st.callGetOnOpenClose = False
		//	st.tab.AddContent("my sliding tab")

		//	st.tab.name = "myslide_name"
		//       st.tab.tabName.Unselected = "Unselected Tab Title"
		//       st.tab.tabName.Selected = "Selected Tab Title"


		//       stb.Append(st.Build)
		//	If Rebuilding Then Me.divToUpdate.Add("TabST_div", stb.ToString)
		//       Return stb.ToString

		//End Function


		//Function BuildSlider(ByVal Name As String, Optional ByVal value As Integer = 0) As String

		//	Dim slider=new clsJQuery.jqSlider(Name, 0, 100, value, clsJQuery.jqSlider.jqSliderOrientation.horizontal, 200, Me.PageName, True)
		//       slider.id = "o" & Name
		//	Return slider.build
		//End Function

		private string BuildTextbox(string name)
		{
			var tb = new clsJQuery.jqTextBox(name, "", "", this.PageName, 20, true);
			tb.id = "o" + name;
			tb.editable = true;
			return tb.Build();
		}


		private string BuildDD(string name, string selectedValue = "")
		{
			var dd = new clsJQuery.jqDropList("dd", this.PageName, false);
			bool sel;
			DataRow[] rows;

			dd.id = "o" + name;
			dd.autoPostBack = true;
			dd.AddItem("", "", false);
			//save the visible text of the options for later use
			if (_ddTable == null)
			{
				_ddTable = new DataTable();
				_ddTable.Columns.Add("ObjectName", typeof(string));
				_ddTable.Columns.Add("OptionName", typeof(string));
				_ddTable.Columns.Add("OptionValue", typeof(string));
			}

			rows = _ddTable.Select("ObjectName='" + name + "'");
			foreach (var row in rows)
			{
				if ((string)row["OptionValue"] == selectedValue)
				{
					sel = true;
				}
				else
				{
					sel = false;
				}
				dd.AddItem((string)row["OptionName"], (string)row["OptionValue"], sel);
			}
			_ddTable.AcceptChanges();
			return dd.Build();
		}


		private string BuildButton(string name)
		{
			var ButtonText = "Submit";
			var b = new clsJQuery.jqButton(name, "", this.PageName, true);
			switch (name)
			{
				case "ButtonLB":
					ButtonText = "Add to Listbox";
					break;

				case "ButtonDD1":
					ButtonText = "Add to Dropdown 1";
					break;

				case "ButtonDD2":
					ButtonText = "Add to Dropdown 2";
					break;
			}

			b.functionToCallOnClick = "userConfirm()";
			b.includeClickFunction = true;
			b.id = "o" + name;
			b.label = ButtonText;

			return b.Build();
		}

		//Public Function DDText(ByVal DDName As String, DDValue As String) As String

		//	Dim Rows() As DataRow

		//	Dim Row As DataRow

		//	Dim ReturnValue As String = ""

		//	Rows = ddTable.Select("ObjectName='" & DDName & "' AND OptionValue='" & DDValue & "'")
		//       For Each Row In Rows
		//		ReturnValue = Row.Item("OptionName")

		//	Next
		//	Return ReturnValue
		//End Function

		//Public Function DDValue(ByVal DDName As String, DDText As String) As String

		//	Dim Rows() As DataRow

		//	Dim Row As DataRow

		//	Dim ReturnValue As String = ""

		//	Rows = ddTable.Select("ObjectName='" & DDName & "' AND OptionName='" & DDText & "'")
		//       For Each Row In Rows
		//		ReturnValue = Row.Item("OptionValue")

		//	Next
		//	Return ReturnValue
		//End Function

		private void PostMessage(string message)
		{
			this.divToUpdate.Add("message", message);
			this.pageCommands.Add("starttimer", "");
			_timerEnabled = true;
		}



	}
}