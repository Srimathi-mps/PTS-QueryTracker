<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryReminder.aspx.cs"
    Inherits="PTS.QueryReminder" ValidateRequest="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<%--<%@ Register Assembly="FreeTextBox" Namespace="FreeTextBoxControls" TagPrefix="FTB" %> 
--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PTS Query Reminder</title>
    <style type="text/css">
        .style1
        {
            width: 133px;
        }
        .modalbackground1
        {
            background-color: Gray;
            opacity: 0.5;
            filter: Alpha(opacity=50);
        }
        .trNoSpace
        {
            font-family: sans-serif;
            font-size: 13px;
            font-weight: bold;
            background-color: #F0F0F0;
            margin: 1;
            padding: 1;
            border-bottom: 1px;
            border-bottom-color: Red;
        }
        .panelHeaderJB
        {
            background-color: White;
            border-width: 1px;
            border-style: groove;
            border-color: #F0F0F0;
            padding: 0px;
            vertical-align: top;
        }
    </style>
    <%--  
   theme_advanced_buttons1: "save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,styleselect,formatselect,fontselect,fontsizeselect",
   theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen",
   theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,spellchecker,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,blockquote,pagebreak,|,insertfile,insertimage",
--%>

    <script type="text/javascript" src="tinymce/jscripts/tiny_mce/tiny_mce.js"></script>

    <script type="text/javascript">
    tinyMCE.init({
        mode: "textareas",
        theme: "advanced",
        plugins: "safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,imagemanager,filemanager",

        theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",


        theme_advanced_toolbar_location: "top",
        theme_advanced_toolbar_align: "left",
        theme_advanced_statusbar_location: "bottom",
        theme_advanced_resizing: false,
        template_external_list_url: "js/template_list.js",
        external_link_list_url: "js/link_list.js",
        external_image_list_url: "js/image_list.js",
        media_external_list_url: "js/media_list.js"
    });
    </script>

    <script type="text/javascript">
         //        function HideModalPopup() {
         //                $find("mdpAddBulkJobChapter").hide();
         //                return false;
         //            }
         var tStrMsg = "";

         function HideModalPopup() {
             $find("ModalPopupExtenderEmailBody").hide();
             return false;
         }
         function echeck(str, rVal, eType) {

             var at = "@";
             var dot = ".";
             var lat = str.indexOf(at);
             var lstr = str.length;
             var ldot = str.indexOf(dot);
             if (str.indexOf(at) == -1) {
                
                 return false;  
             }
             if (str.indexOf(at) == -1 || str.indexOf(at) == 0 || str.indexOf(at) == lstr) {
                  
                 return false;  
             }
             if (str.indexOf(dot) == -1 || str.indexOf(dot) == 0 || str.indexOf(dot) == lstr) {
                 
                 return false;  
             }
             if (str.indexOf(at, (lat + 1)) != -1) {
                 
                 return false;  
             }
             if (str.substring(lat - 1, lat) == dot || str.substring(lat + 1, lat + 2) == dot) {
                  
                 return false;  
             }
             if (str.indexOf(dot, (lat + 2)) == -1) {
                 
                 return false;  
             }
             if (str.indexOf(" ") != -1) {
                 
                 return false;  
             }

             return true
         }




         function ValidateSave() {

             var rowCount = document.getElementById("gv_Reminders").rows.length - 1;
             var gvRowCount;
             var dtRowCount;
             var newRows;
             var rows;
             var Count = 0;
             var strMessage = "";
             var status = true;
             var i;
             var j = 0;

             //alert("rowCount : " + rowCount);
             for (i = 0; i < rowCount; i++) {
                 dtRowCount = 0;
                 gvRowCount = 0;
                 if (i + 2 < 10) {
                     rows = "0" + (i + 2);
                     dtRowCount = "0" + (Count);
                     gvRowCount = "0" + rowCount;
                     newRows = "0" + (i + 2);
                 }
                 else {
                     rows = i + 2;
                     dtRowCount = Count;
                     gvRowCount = rowCount;
                     newRows = i + 2;
                 }

                 if (i == 0)
                     document.getElementById("lbl_showMsg").innerText = ""
                 var x = i + 1;
                 // From mail id
                 if (document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailFrom").value.trim() != "") {
                     if (!echeck(document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailFrom").value, i + 1, "Column: Email From")) {
                         status = false;
                         document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ") Email From";
                     }

                 }
                 else {
                      
                     status = false;
                     document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ")  Email From";

                 }

                 // To mail id
                 if (document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailTo").value.trim() != "") {
                     if (!echeck(document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailTo").value, i + 1, "Column:  Email To")) {
                         status = false;
                         document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ") Email To";
                     }
                 }
                 else {
                    
                     status = false;
                     document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ")  Email To";
                 }


                 // CC mail id
                 if (document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailCC").value.trim() != "") {
                     if (!echeck(document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailCC").value, i + 1, "Email CC")) {
                         status = false;
                         document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ") Email CC";
                     }
                 }


                 // CC mail id
                 if (document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailBCC").value.trim() != "") {
                     if (!echeck(document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_EmailBCC").value, i + 1, "Email BCC")) {
                         status = false;
                         document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ") Email BCC";
                     }
                 }

                 //   mail subject
                 if ((document.getElementById("gv_Reminders_ctl" + rows + "_gv_TxtBx_Subject").value).trim() == "") {
 
                     status = false;
                     document.getElementById("lbl_showMsg").innerText = document.getElementById("lbl_showMsg").innerText + " (R"  + x + ")  Subject";

                 }
             }
             if (!status)
                 document.getElementById("lbl_showMsg").innerText = "Confirm above row(s) for : " + document.getElementById("lbl_showMsg").innerText 
             return status
         }
 
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <input id="Hid_emailBody" type="hidden" name="hddclick" runat="server" />
    <asp:ScriptManager ID="scriptMgrQryRem" runat="server" EnablePartialRendering="false">
    </asp:ScriptManager>
    <AjaxToolKit:ModalPopupExtender ID="ModalPopupExtenderAddReminder" runat="server"
        BackgroundCssClass="modalbackground1" Drag="true" PopupControlID="pnl_Add" TargetControlID="btn_Add">
    </AjaxToolKit:ModalPopupExtender>
    <AjaxToolKit:ModalPopupExtender ID="ModalPopupExtenderEmailBody" runat="server" BackgroundCssClass="modalbackground1"
        Drag="true" PopupControlID="Panel_EmailBody" TargetControlID="RichTextBox" X="50"
        Y="50">
    </AjaxToolKit:ModalPopupExtender>
    <div style="width: 100%; background: #99ccff;">
        <table style="width: 100%; vertical-align: middle; top: 80pt">
            <tr style="height: 20pt">
                <td align="center" colspan="6">
                </td>
            </tr>
            <tr valign="middle">
                <td colspan="6" align="center" valign="middle" style="background-color: White; vertical-align: middle">
                    <asp:Label ID="lblQryRemHead" Text="PTS Query Reminder Details" runat="server" Font-Bold="True"
                        Font-Size="X-Large" ForeColor="#3333CC" Height="65pt"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="6">
                    <hr />
                </td>
            </tr>
            <tr align="left">
                <td align="right" colspan="0">
                    <asp:Label ID="lbl_Customer" runat="server" Text="Customer : " Width="80pt"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddl_Customer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Customer_SelectedIndexChanged"
                        Width="120pt">
                    </asp:DropDownList>
                </td>
                <td align="right">
                    <asp:Label ID="lbl_QueryStatus" runat="server" Text="Query Status : " Width="80pt"> </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddl_QueryStatus" AutoPostBack="true" OnSelectedIndexChanged="QueryStage_SelectedIndexChanged"
                        runat="server" Width="120pt">
                    </asp:DropDownList>
                </td>
                <%--  <td valign="middle" class="style1" style="width:30%"> 
    <asp:Label ID="lblRemindersNo" runat="server" Width="88pt" ToolTip="Total eminders" Text="Reminders :"></asp:Label>
    <asp:TextBox ID="txtBx_Reminders" runat="server" Width="63px" ></asp:TextBox>
    &nbsp;&nbsp; &nbsp;&nbsp;
    </td>
     ForeColor="#333333"
    --%>
            </tr>
            <tr>
                <td align="center" colspan="6">
                    <hr />
                </td>
            </tr>
            <tr>
                <td align="center" colspan="6">
                </td>
            </tr>
            <tr>
                <td colspan="6" align="center" style="background-color: Transparent">
                    <asp:UpdatePanel ID="updtPnlReport" runat="server" RenderMode="Inline">
                        <contenttemplate>
                    <asp:GridView ID="gv_Reminders" runat="server" AutoGenerateColumns="False" CellPadding="2"
                        ForeColor="BlueViolet" GridLines="None" Font-Names="Arial" Font-Size="X-small"
                        Font-Bold="False" Font-Overline="False" Width="82%" OnSelectedIndexChanged="gv_Reminders_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField HeaderText="ReminderNo">
                                <HeaderTemplate>
                                    Reminder<asp:Label ID="lbl_Star1" Text="*" runat="server" ForeColor="Red" Font-Size="16px"> </asp:Label>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_ReminderNo" runat="server" Font-Size="small" Style="text-align: center"
                                        ToolTip="To send mail of reminder" Text='<%# Bind("ReminderNo") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="40px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="IntervalInHrs">
                                <HeaderTemplate>
                                    Interval(Hrs)<asp:Label ID="lbl_Star2" Text="*" runat="server" ForeColor="Red" Font-Size="16px"> </asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_IntervalInHrs" ToolTip="Mail interval on this hours" runat="server"
                                        Font-Size="11px" Text='<%# Bind("IntervalInHrs") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="62px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailFrom">
                                <HeaderTemplate>
                                    Email From<asp:Label ID="lbl_Star3" Text="*" runat="server" ForeColor="Red" Font-Size="16px"> </asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_EmailFrom" runat="server" Font-Size="11px" Text='<%# Bind("EmailFrom") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="155px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailTo">
                                <HeaderTemplate>
                                    Email To<asp:Label ID="lbl_Star4" Text="*" runat="server" ForeColor="Red" Font-Size="16px"> </asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_EmailTo" runat="server" Font-Size="11px" Text='<%# Bind("EmailTo") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="155px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailCC">
                                <HeaderTemplate>
                                    Email CC</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_EmailCC" runat="server" Font-Size="11px" Text='<%# Bind("EmailCC") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="155px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailBCC">
                                <HeaderTemplate>
                                    Email BCC</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_EmailBCC" runat="server" Font-Size="11px" Text='<%# Bind("EmailBCC") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="155px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Subject">
                                <HeaderTemplate>
                                    Email Subject<asp:Label ID="lbl_Star5" Text="*" runat="server" ForeColor="Red" Font-Size="16px"> </asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_Subject" runat="server" Font-Size="11px" Text='<%# Bind("Subject") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="180px" />
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_EmailBody" runat="server" Visible="false" Font-Size="11px"
                                        Text='<%# Bind("EmailContent") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailContentBtn">
                                <HeaderTemplate>
                                    Email Body<asp:Label ID="lbl_Star6" Text="*" runat="server" ForeColor="Red" Font-Size="16px"> </asp:Label></HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Button ID="gv_Btn_EmailBody" runat="server" Text='Email Body' OnClick="gv_Btn_EmailBody_Click"
                                        CommandArgument='<%# Container.DataItemIndex %>' ToolTip='<%# Bind("EmailContent") %>' />
                                    <%--<asp:LinkButton ID="gv_Btn_EmailBody" runat="server" Text='<%# Bind("EmailContent") %>' OnClick="gv_Btn_EmailBody_Click" CommandArgument='<%# Container.DataItemIndex %>' Font-Size="Small" Width="100px" Height="5px"  ToolTip='<%# Bind("EmailContent") %>'> </asp:LinkButton>--%>
                                </ItemTemplate>
                                <ControlStyle Width="100px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CreatedBy" Visible="false">
                                <HeaderTemplate>
                                    Created By</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_CreatedBy" runat="server" Font-Size="small" Text='<%# Bind("CreatedBy") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CreatedOn" Visible="false">
                                <HeaderTemplate>
                                    Created On</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_CreatedOn" runat="server" Font-Size="small" Text='<%# Bind("CreatedOn") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ModifiedBy" Visible="false">
                                <HeaderTemplate>
                                    Modified By</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_ModifiedBy" runat="server" Font-Size="small" Text='<%# Bind("ModifiedBy") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="80px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ModifiedOn" Visible="false">
                                <HeaderTemplate>
                                    Modified On</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_ModifiedOn" runat="server" Width="40px" Font-Size="small"
                                        Text='<%# Bind("ModifiedOn") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="40%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <HeaderTemplate>
                                    Delete</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:ImageButton ID="gv_imgBtn_DeleteRec" runat="server" ToolTip="Delete this row"
                                        ImageUrl="~/images/Del02.jpg" OnClick="DetleRecord_Click" Width="40%" Height="40%" />
                                </ItemTemplate>
                                <ControlStyle Width="25px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailConfigID" Visible="false">
                                <HeaderTemplate>
                                    Email Config ID</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_EmailConfigID" runat="server" Font-Size="small" Text='<%# Bind("EmailConfigID") %>'></asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="40px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="EmailDetailsID" Visible="false">
                                <HeaderTemplate>
                                    Sl.No.</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:TextBox ID="gv_TxtBx_SrNo" runat="server" Text='<%# Bind("EmailDetailsID") %>'
                                        ForeColor="Black">&nbsp;111&nbsp;&nbsp;</asp:TextBox>
                                </ItemTemplate>
                                <ControlStyle Width="10px" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#3333CC" ForeColor="White" Font-Size="X-Small" />
                    </asp:GridView>
                    </contenttemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="6" style="height: 30pt">
                    <hr />
                </td>
            </tr>
            <tr align="center">
                <td align="center" colspan="5" style="background-color: White">
                    <asp:Button ID="btn_Add" runat="server" Text="Add Reminder" OnClick="btn_Add_Click1"
                        Width="80pt" Enabled="false" />
                    <asp:Button ID="btn_Save" runat="server" Text="Update" OnClick="btn_Save_Click" Width="50pt"
                        Enabled="false" />
                    <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" Width="50pt" OnClick="btn_Cancel_Click"
                        Enabled="false" />
                </td>
            </tr>
            <tr>
                <td colspan="6" align="center" style="height: 10px">
                </td>
            </tr>
            <tr>
                <td colspan="6" align="center">
                    <asp:Label ID="lbl_showMsg" runat="server" ForeColor="Red"> </asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="6" align="center" style="height: 10px">
                </td>
        </table>
    </div>
    <asp:Panel ID="pnl_Add" Style="display: none" runat="server">
        <div align="center" style="width: 200pt; height: 200pt;">
            <table width="250px">
                <tr style="background-color: #3333CC;">
                    <td align="center" style="color: white; font-weight: bold; font-size: 15px; width: 50%">
                        Add Reminders
                    </td>
                </tr>
                <tr class="trNoSpace">
                    <td>
                        <hr />
                        <asp:Label ID="lbl_remVal" runat="server" Text="Reminders :"></asp:Label>
                        <asp:TextBox ID="txtBx_Reminders" runat="server" Width="20pt"></asp:TextBox>
                        <asp:CheckBox ID="chkBx_Replicate" runat="server" Text="Replicate" ToolTip="To replicate last row values to new all" />
                        <hr />
                        &nbsp;&nbsp; &nbsp;&nbsp;
                        <asp:Button ID="btn_Add_Reminders" runat="server" Text="Add" Width="40pt" OnClick="btn_Add_Click1" />
                        &nbsp;&nbsp; &nbsp;&nbsp;
                        <asp:Button ID="Button2" runat="server" Text="Cancel" Width="40pt" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel ID="Panel_EmailBody" Style="display: none" runat="server">
        <div align="center" style="width: 200pt; height: 200pt;">
            <table width="250px">
                <tr style="background-color: #3333CC;">
                    <td align="center" style="color: white; font-weight: bold; font-size: 15px; width: 50%">
                        Email body
                        <asp:TextBox ID="RichTextBox" runat="server" TextMode="MultiLine"></asp:TextBox>
                        <%--<FTB:FreeTextBox  ID="RichTextBoxx" runat="server"  ></FTB:FreeTextBox>--%>
                    </td>
                </tr>
                <tr class="trNoSpace">
                    <td>
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                            OnClientClick="return HideModalPopup();" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    </form>
</body>
</html>
