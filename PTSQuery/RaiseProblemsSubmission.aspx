<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RaiseProblemsSubmission.aspx.cs"
    Inherits="QueryTracker.RaiseProblemsSubmission" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">

</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Problem Details</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link href="assets/global/plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet"
        type="text/css" />
    <link href="assets/Select/bootstrap-select.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/global/css/components_New.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/layouts/layout2/css/layout.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/toastr/toastr_new.css" rel="stylesheet" type="text/css" />
    <script src="assets/global/plugins/jquery.min.js" type="text/javascript"></script>
    <script src="assets/global/plugins/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="assets/Select/bootstrap-select.min.js" type="text/javascript"></script>
    <script src="assets/Select/bootstrap-select.min.js" type="text/javascript"></script>
    <script src="assets/toastr/notification.js" type="text/javascript"></script>
    <script src="assets/toastr/toastr.js" type="text/javascript"></script>
    <script type="text/javascript">


        function ShowssMessage(msgtype, title, msg, top_bottom, left_center_right) {
            debugger;
            if (top_bottom == '') {
                top_bottom = 'top';
            }
            if (left_center_right == '') {
                left_center_right == 'right';
            }
            var cls = "toast-" + top_bottom + "-" + left_center_right;

            if (msgtype == 'error') {
                toastr.info(title, msg,
            toastr.options = {
                closeButton: true,
                progressBar: true,
                positionClass: cls,
                timeout: 2000
            })
            }
            else if (msgtype == 'success') {
                toastr.success(title, msg,
            toastr.options = {
                closeButton: true,
                progressBar: true,
                positionClass: cls
            })
            }
        }
    </script>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <style type="text/css">
        .BodyContaint
        {
            width: 100%;
            margin: 0;
            padding: 0;
        }
        .BodyContaint111
        {
            width: 100%;
            margin: 0;
            padding: 0;
        }
        .tab
        {
            border: solid 1px black;
            background-color: #eeeeee;
            padding: 2px 10px;
        }
        .selectedTab
        {
            background-color: white;
            border-bottom: solid 1px white;
        }
        .centeredText
        {
            text-align: center;
            vertical-align: middle;
        }
        select.expand
        {
            width: auto;
        }
        select > option
        {
            width: auto;
        }
        .hiddenGridColumn
        {
            font-size: 0;
            height: 0;
            width: 0;
        }
    </style>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <script type="text/javascript">
        var customerCodeT = '<%= Session["Customer"] %>';
        var journalCodeT = '<%= Session["Journal"] %>';
        var ArticleT = '<%= Session["Article"] %>';
        var VolumeT = '<%= Session["Volume"] %>';
        var IssueT = '<%= Session["Issue"] %>';
        var stageIDT = '<%= Session["Stage"] %>';
        var stageNameT = '<%= Session["StageName"] %>';
        var OpenedRequestIdT = '<%= Session["OpenedRequestId"] %>';
        var jobTypeT = '<%= Session["jobType"] %>';
        var usernameT = '<%= Session["userName"] %>';
        var ProblemTypeIdT = '<%= Session["OpenedRequestId"] %>';
        var ZipFilePathT = ''; //'<%= Session["ZipFilePath"] %>';

        function ClosePopup() {
            //Hide the modal popup - the update progress
            var popup = $find('<%= modalpopup1.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
        }
        function ClearResolvedPopup() {
            var txtRemarks = document.getElementById('<%=txtRemarks.ClientID%>');
            var fileUpload = document.getElementById('<%=fileUpload.ClientID%>');
            txtRemarks.value = '';
            fileUpload.value = '';
        }
    </script>
    <script language="javascript" src="Javascript/RaiseProblem.js" type="text/javascript"></script>
    <script language="javascript" src="Javascript/strTrim.js" type="text/javascript"></script>
    <style type="text/css">
        .gridHeader
        {
            border: solid 1px #A0A0A0;
            font-size: small;
            background: rgba(254,254,254,1);
            background: -moz-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: -webkit-gradient(left top, left bottom, color-stop(0%, rgba(254,254,254,1)), color-stop(49%, rgba(209,209,209,1)), color-stop(50%, rgba(219,219,219,1)), color-stop(100%, rgba(226,226,226,1)));
            background: -webkit-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: -o-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: -ms-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: linear-gradient(to bottom, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr=  '#fefefe' , endColorstr= '#e2e2e2' , GradientType=0 );
        }
        .lblHeader1
        {
            font-size: 14px;
            font-weight: bolder;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div class="BodyContaint">
        <div style="margin-bottom: 5px;">
            <asp:Label ID="lblHeader" runat="server" Text="Query details of " CssClass="lblHeader1"></asp:Label>
        </div>
        <div>
            <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
            <asp:Label ID="ResultLabel" runat="server" CssClass="errorMessage"></asp:Label>
        </div>
        <div runat="server" id="dvHeader" style="margin-bottom: 5px;">
            <table style="width: 100%; background-color: #dbeaf9;">
                <tr>
                    <td class="PTSLabelNew" style="width: 10%;">
                        Created By
                    </td>
                    <td class="PTSControl" style="width: 23%;">
                        <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
                    </td>
                    <td class="PTSLabelNew" style="width: 10%;">
                        Sent By
                    </td>
                    <td class="PTSControl" style="width: 23%;">
                        <asp:Label runat="server" ID="lblSentBy"></asp:Label>
                    </td>
                    <td class="PTSLabelNew" style="width: 10%;">
                        Resolved By
                    </td>
                    <td class="PTSControl" style="width: 23%;">
                        <asp:Label runat="server" ID="lblResolvedBy"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="PTSLabelNew" style="width: 10%;">
                        Created Date
                    </td>
                    <td class="PTSControl" style="width: 23%;">
                        <asp:Label runat="server" ID="lblCreatedDate"></asp:Label>
                    </td>
                    <td class="PTSLabelNew" style="width: 10%;">
                        Sent Date
                    </td>
                    <td class="PTSControl" style="width: 23%;">
                        <asp:Label runat="server" ID="lblSentDate"></asp:Label>
                    </td>
                    <td class="PTSLabelNew" style="width: 10%;">
                        Resolved Date
                    </td>
                    <td class="PTSControl" style="width: 23%;">
                        <asp:Label runat="server" ID="lblResolvedDate"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dvPTInsEdit" runat="server" style="margin-top: 10px;">
            <asp:GridView ID="gvwPTInsEdit" runat="server" Width="100%" AutoGenerateColumns="False"
                CellPadding="4" ForeColor="Black" OnRowDataBound="gvwPTInsEdit_RowDataBound"
                EmptyDataText="No records found !" Font-Size="Small">
                <RowStyle BackColor="#dbeaf9" BorderColor="#A0A0A0" />
                <Columns>
                    <asp:TemplateField HeaderText="Departments" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <select  id="gddlDepartment" class="selectpicker show-menu-arrow My_reset Radius_Form "
                                multiple data-actions-box="true" data-header="Chapters" data-live-search="true"
                                data-size="8" runat="server">
                                <%--<option value="0" disabled>select</option>--%>
                            </select>
                            <%--<asp:DropDownList ID="gddlDepartment" runat="server" Width="100%"
                                class="selectpicker show-menu-arrow My_reset Radius_Form " multiple data-actions-box="true"
                                data-header="Departments"  data-live-search="true" data-size="8">
                                
                            </asp:DropDownList>--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Problem Type" ItemStyle-Width="25%">
                        <ItemTemplate>
                            <asp:DropDownList ID="gddlPT" runat="server" AutoPostBack="true" Width="100%" OnSelectedIndexChanged="gddlPT_SelectedIndexChanged">
                                <%--<asp:ListItem Value="--SELECT--">--SELECT--</asp:ListItem>--%>
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Problem Category" ItemStyle-Width="20%">
                        <ItemTemplate>
                            <asp:DropDownList ID="gddlPTCategory" runat="server" Width="100%" OnSelectedIndexChanged="gddlPTCategory_SelectedIndexChanged">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Body Prefix" Visible="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="glblBodyPrefix" Width="100%"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="35%">
                        <ItemTemplate>
                            <asp:TextBox ID="gtxtPTRemarks" runat="server" Width="95%" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Width="10%" Visible="false">
                        <ItemTemplate>
                            <asp:Button ID="gbtnAddNew" runat="server" Text="+" Enabled="false" OnClick="gbtnAddNew_Click"
                                Font-Bold="true" Font-Size="Larger" />
                            <asp:Button ID="gbtnDel" runat="server" Text="-" Font-Bold="true" OnClick="gbtnDel_Click"
                                Font-Size="Larger" />
                            <asp:Label ID="lblPTCurrent" runat="server" CssClass="hiddenGridColumn"></asp:Label>
                            <%--<asp:Label ID="glblHiddenDesc" runat="server" CssClass="hiddenGridColumn1"></asp:Label>--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <%--<asp:TemplateField ItemStyle-CssClass="hiddenGridColumn">
                        <ItemTemplate>
                            <asp:Label ID="lblPTCurrent" runat="server" ></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <%--<asp:TemplateField ItemStyle-BorderStyle="None" HeaderStyle-BorderStyle="None">
                        <ItemTemplate>
                            <asp:Button ID="gbtnDel" runat="server" Text="-" OnClick="gbtnDel_Click" Font-Bold="true" Font-Size="Larger"/>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <%--<asp:CommandField ShowDeleteButton="true" DeleteText="Del" HeaderText="" HeaderStyle-CssClass="centeredText"
                        ItemStyle-CssClass="centeredText" ControlStyle-CssClass="centeredText" ButtonType="Button">
                        <ControlStyle CssClass="centeredText"></ControlStyle>
                        <HeaderStyle CssClass="centeredText"></HeaderStyle>
                        <ItemStyle CssClass="centeredText"></ItemStyle>
                    </asp:CommandField>--%>
                </Columns>
                <HeaderStyle Font-Bold="True" BorderColor="#A0A0A0" BorderStyle="Solid" BorderWidth="1px"
                    CssClass="gridHeader" />
                <AlternatingRowStyle BackColor="White" BorderColor="#A0A0A0" BorderStyle="Solid"
                    BorderWidth="1px" />
            </asp:GridView>
            <div style="margin-top: 10px; width: 100%;">
                <asp:Button ID="SaveButton" runat="server" Text="Save" Enabled="false" Visible="false" />
             
                <asp:Button ID="ZipButton" runat="server" Text="Attachment" OnClick="ZipButton_Click"
                    Visible="true" />
                <asp:Button ID="CloseButton" runat="server" Text="Raise Query   " OnClick="CloseButton_Click" />
                <asp:Button ID="QueryResolveButton" runat="server" Text="Query Response" OnClientClick="javascript:ClearResolvedPopup();"
                    OnClick="QueryResolveButton_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                   <asp:Label ID="ProceedWithQuery_lbl" runat="server" Text="Proceed with Query "  CssClass="lblHeader1"></asp:Label>
                <asp:DropDownList ID="ProceedWithQuery_DropDownList1" runat="server">
                <asp:ListItem Value="0" Text="YES"></asp:ListItem>
                <asp:ListItem Value="1" Text="NO"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div style="margin-top: 10px; width: 100%; text-align: right;">
                <asp:LinkButton ID="lbtnZipFile" runat="server" OnClick="lbtnZipFile_Click"></asp:LinkButton>
                <asp:Label ID="lblZipFile" runat="server" Text=""></asp:Label>
            </div>
        </div>
        <div>
            <asp:ModalPopupExtender ID="modalpopup1" runat="server" BackgroundCssClass="modalPopup"
                TargetControlID="QueryResolveButton" PopupControlID="pnllogin" X="200" Y="200" />
            <asp:Panel ID="pnllogin" runat="server" Style="display: none; background-color: silver;">
                <div style="width: 1000px">
                    <asp:Panel ID="pnldrag" runat="server">
                        <table class="Dialog" width="100%">
                            <tr style="background-image: url('Images/titlebar.jpg')">
                                <td style="color: #000000" align="center">
                                    <img src="Images/dialog-close.gif" align="right" style="cursor: pointer; height: 25px;"
                             onclick="ClosePopup()"            alt="Close" />Query Response
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table width="100%" border="0px" align="center" style="">
                                        <tr>
                                            <td>
                                                Attachment
                                            </td>
                                            <td>
                                                <asp:FileUpload ID="fileUpload" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">
                                                Remarks:
                                            </td>
                                            <td align="left">
                                                <asp:TextBox ID="txtRemarks" TextMode="MultiLine" Width="895px" Height="242px"  runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnsubmit" runat="server" Text="Ok" OnClick="btnsubmit_Click" />
                                                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                                                <asp:Button ID="btnCancelResponse" runat="server" Text="Cancel"  OnClientClick="ClosePopup()" />
                                            </td>
                                             
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </asp:Panel>
        </div>
        <div style="margin-top: 10px;">
            <asp:HiddenField ID="PTDescriptionTextBox" runat="server" />
            <asp:HiddenField ID="hfZipFilePath" runat="server" />
            <asp:HiddenField ID="hdnUserName" runat="server" />
            <asp:HiddenField ID="hdnReviewerName" runat="server" />
            <asp:HiddenField ID="hdnName" runat="server" />
            <asp:HiddenField ID="hdnXml" runat="server" Value="FirstTime" />
            <%--<asp:HiddenField ID="ndnPtsAddValidate" runat="server" Value="0" />--%>
            <input type="hidden" id="JobTypeHidden" runat="server" />
            <asp:HiddenField ID="ndnPtsAddValidate" runat="server" Value="0" />
            <%--<asp:HiddenField ID="hfZipFilePathNew" runat="server" />--%>
        </div>
        <div>
        </div>
    </div>
    </form>
</body>
</html>
