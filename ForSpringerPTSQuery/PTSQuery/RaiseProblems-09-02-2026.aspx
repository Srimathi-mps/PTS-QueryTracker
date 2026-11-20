<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RaiseProblems.aspx.cs" Inherits="QueryTracker.RaiseProblem" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Communication Tracker</title>
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
     <script type="text/javascript">

         function onclickEvent(objRef, evt, Journal, Customer, ArtCode, StageTransactionId) {


             var obj = {};
             obj.Journal = Journal;
             obj.Customer = Customer;
             obj.ArtCode = ArtCode;
             obj.StageTransactionId = StageTransactionId;
             $.ajax({
                 type: "POST",
                 url: "RaiseProblems.aspx/GetTransDetails",
                 data: JSON.stringify(obj),
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (r) {
                     $('#TransTable').empty();
                     $('#TransTable').append(r.d);
                     $('#TransDetails').modal('show');
                 }
             });

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
        .btnDummy
        {
            background-color: Transparent;
            color: White;
        }
        .btClass
        {
            color: red;
            font-weight: bold;
            height: 35px;
        }
        .gridRow
        {
            border: solid 1px #707070;
        }
        .gridHeader
        {
            border: solid 1px #707070;
            color: Navy;
            font-size: small;
            background: rgba(254,254,254,1);
            background: -moz-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: -webkit-gradient(left top, left bottom, color-stop(0%, rgba(254,254,254,1)), color-stop(49%, rgba(209,209,209,1)), color-stop(50%, rgba(219,219,219,1)), color-stop(100%, rgba(226,226,226,1)));
            background: -webkit-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: -o-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: -ms-linear-gradient(top, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            background: linear-gradient(to bottom, rgba(254,254,254,1) 0%, rgba(209,209,209,1) 49%, rgba(219,219,219,1) 50%, rgba(226,226,226,1) 100%);
            filter: progid:DXImageTransform.Microsoft.gradient( startColorstr=      '#fefefe' , endColorstr= '#e2e2e2' , GradientType=0 );
        }
        .FixedHeader
        {
            position: absolute;
            font-weight: bold;
            vertical-align: text-bottom;
            width: 99%;
        }
        .FixedGridHeight
        {
            height: 200px;
            overflow: auto;
        }
        .gridview
        {
            width: 100%;
        }
        .WaterMark
        {
            font-style: italic;
            font-weight: bold;
            color: #A9A9B7;
        }
        .gridButton
        {
            background-color: Transparent;
            border: medium none;
            color: Navy;
            font-family: Verdana,Arial;
            font-size: 10px;
            font-weight: bold;
            height: 100%;
            width: 100%; /*text-decoration:underline;
            cursor:pointer;*/
        }
        .disabledDDL
        {
            font-family: Verdana,Arial;
            font-size: 11px;
            font-weight: normal;
            height: 25px;
            width: 100%;
            border: none;
        }
        .hiddenGridColumn
        {
            font-size: 0;
            height: 0;
            width: 0;
        }
        .style1
        {
            font-family: Verdana,Arial;
            font-size: 11px;
            font-weight: normal;
            font-weight: bold; /*width: 11%;
	max-width:11%;*/;
            text-align: right;
            padding-right: 10px;
            vertical-align: text-top;
            width: 87px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" class="container">
    <cc1:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </cc1:ToolkitScriptManager>
    <div class="BodyContaint">
        <asp:LinkButton ID="lkbtn" runat="server" Text="Logout" Font-Underline="true" OnClick="lkbtn_Click"
            Style="float: right;" />
        <div style="font-size: 14px; font-weight: bolder; margin-bottom: 5px">
            <table>
                <tr>
                    <td style="width: 500pt">
                        Communication Tracker
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-bottom: 5px;">
            <table style="background-color: #dbeaf9;" class="table table-advance table-condensed ">
                <tr>
                    <td class="PTSLabelNew">
                        <div class="PTSLabelNew" style="text-align: left; padding-left: 20px;">
                            Barcode
                            <asp:TextBox ID="txtBarcode" runat="server" Width="350px"></asp:TextBox>
                            <cc1:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtBarcode"
                                WatermarkText="Customer|Project|Job" WatermarkCssClass="WaterMark">
                            </cc1:TextBoxWatermarkExtender>
                            <asp:Button ID="btnGoBarcode" runat="server" Text="Go" OnClick="btnGoBarcode_Click" />
                            <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Report" OnClick="Button1_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-bottom: 5px; width: 100%;">
            <div style="width: 100%; background-color: #dbeaf9; float: left;">
            <table>
            <tr><td>
                <div style="width: 71%; background-color: #dbeaf9; float: left;">
                    <table style="width: 100%; background-color: #dbeaf9;" class="  table-condensed table-hover ">
                        <tr>
                            <td class="PTSLabelNew">
                                Customer
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="CustomerDropDown" runat="server" AutoPostBack="true" CausesValidation="true"
                                    CssClass="PTSCombo selectpicker show-menu-arrow show-tick " ValidationGroup="StageValidation"
                                    data-size="8" data-live-search="true" data-header="Please select Customer" OnSelectedIndexChanged="CustomerDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="PTSLabelNew">
                                Project
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="JournalDropDown" runat="server" AutoPostBack="true" data-size="8"
                                    data-live-search="true" CssClass="PTSCombo  selectpicker show-menu-arrow show-tick "
                                    OnSelectedIndexChanged="JournalDropDown_SelectedIndexChanged" data-header="Please select Book">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="MasterCopyJournalLabel" runat="server" CssClass="errorMessage" Text="Mastercopy journal"
                                    Visible="False" Font-Size="9px"></asp:Label>
                            </td>
                            <td class="style1">
                                Job
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="ArticleDropDown" runat="server" CssClass="PTSCombo" AutoPostBack="True"
                                    Visible="false" OnSelectedIndexChanged="ArticleDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                                <select onkeypress="Reset()" id="select1_ArticleDropDown" class="selectpicker show-menu-arrow My_reset Radius_Form "
                                    multiple data-actions-box="true" data-header="Chapters" data-live-search="true"
                                    data-size="8" runat="server">
                                    <%--<option value="0" disabled>select</option>--%>
                                </select>
                            </td>
                            <%--<td rowspan="5">
                            <asp:Button ID="btnGoDdl" runat="server" Text="Go" OnClick="btnGoDdl_Click" Visible="false" />
                        </td>--%>
						
                        </tr>
                        <tr>
                        <td class ="PTSLabelNew">
                        ISBN
                        </td>
                        <td class="PTSControl">
                            <asp:TextBox ID="TextBox3" runat="server" CssClass="PTSTextbox" Width="210px" Height="25px"></asp:TextBox>
                        </td>
                            <td class="PTSLabelNew">
                                Stage
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="StageDropDown" runat="server" CssClass="PTSCombo" AutoPostBack="true"
                                    OnSelectedIndexChanged="StageDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                    <asp:ListItem Value="1">S100</asp:ListItem>
                                    <asp:ListItem Value="2">S200</asp:ListItem>
                                    <asp:ListItem Value="3">S300</asp:ListItem>
									<asp:ListItem Value="4">ALL</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="StageDropDown1" runat="server" CssClass="PTSCombo" AutoPostBack="true"
                                    OnSelectedIndexChanged="StageDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                    <asp:ListItem Value="35">S5</asp:ListItem>
                                    <asp:ListItem Value="36">S50</asp:ListItem>
                                    <asp:ListItem Value="37">S200</asp:ListItem>
                                    <asp:ListItem Value="40">S600</asp:ListItem>
                                    <asp:ListItem Value="41">S600-EvaluateEPUB</asp:ListItem>
                                    <asp:ListItem Value="38">S300-eProof</asp:ListItem>
                                    <asp:ListItem Value="39">S300-PerformProof</asp:ListItem>
                                    <asp:ListItem Value="42">S650-eProof</asp:ListItem>
                                    <asp:ListItem Value="43">S650-PerformProof</asp:ListItem>
									<asp:ListItem Value="10">ALL</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="style1" visible="false">
                                Department
                            </td>
                            <td class="PTSControl" visible="false">
                                <asp:DropDownList ID="DeptDropDown" AutoPostBack="true" runat="server" CssClass="PTSCombo">
                                </asp:DropDownList>
                            </td>

                            <%--<td class="PTSLabelNew" visible="false">
                                Volume
                            </td>
                            <td class="PTSControl" visible="false">
                                <asp:DropDownList ID="VolumeDropDown" Enabled="false" AutoPostBack="true" runat="server" CssClass="PTSCombo"
                                    >
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="PTSLabelNew" visible="false">
                                Issue
                            </td>
                            <td class="PTSControl" visible="false">
                                <asp:DropDownList ID="IssueDropDown"  Enabled="false" runat="server" AutoPostBack="true" CssClass="PTSCombo"
                                    >
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>--%>
<td>
<div style="width: 5%; float: left;">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <asp:Button ID="btnGoDdl" runat="server" Text="Go" Visible="false" OnClick="btnGoDdl_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
</td>
                        </tr>
                        <tr runat="server" visible="false">
                            <td class="PTSLabelNew">
                                Increment
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="ddlRoundCount" runat="server" AutoPostBack="true" CssClass="PTSCombo"
                                    Enabled="false">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
               </td> </tr><tr><td>
                
                <div style="width: 10%; float: left;">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label2" runat="server" Text="." Font-Bold="true" CssClass="btnDummy"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label1" runat="server" Text="." Font-Bold="true" CssClass="btnDummy"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                        <td>
                        <table align="center">
                        <tr>
                            <td style="width:2500px">
                                <asp:Button ID="btnRaiseProblem" runat="server" Text="New Query" CssClass="btClass"
                                    OnClick="btnRaiseProblem_Click" Height="22px" />
                            </td>
                             <td>
                                <asp:Button ID="btnNew_Instruction" runat="server" Text="New Instruction" CssClass="btClass"
                                    OnClick="btnNew_Instruction_Click" Height="22px" />
                            </td>
                            </tr>
                            </table>
                            </td>
                        </tr>
                        <tr>
                           
                        </tr>
                    </table>
                </div>
                </td></tr>
                </table>
            </div>
        </div>
        <div style="margin-bottom: 5px; width: 85%; float: left; margin-top: 5px; height: 400px;">
            <%--<table class="gridHeader" style="width: 100%;">
                <tr>
                    <th style="width: 10%">
                        <span class="gridButton">Code</span>
                    </th>
                    <th style="width: 10%">
                        <span class="gridButton">Job</span>
                    </th>
                    <th style="width: 62%">
                        <span class="gridButton">Remarks</span>
                    </th>
                    <th style="width: 6%">
                        <span class="gridButton">Status</span>
                    </th>
                    <th style="width: 6%">
                        <span class="gridButton">Type</span>
                    </th>
                    <th style="width: 15%">
                        <span class="gridButton">Response</span>
                    </th>
                    <th style="width: 5%">
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </th>
                </tr>
            </table>--%>
            <div style="width: 100%; float: left; height: 800px;overflow:inherit; ">
                <asp:GridView ID="gvwOpenedRequest" runat="server" AutoGenerateColumns="False" CellPadding="4"
                    EmptyDataText="No records found !" BackColor="White" BorderColor="ActiveBorder"
                    OnRowEditing="gvwOpenedRequest_RowEditing" OnRowDataBound="gvwOpenedRequest_RowDataBound"
                    DataKeyNames="PTId,DeptAndPT,PTCode,CreatedBy,CreatedDate,SentBy,SentDate,ResolvedBy,ResolvedDate,QueryType,Article"
                    BorderStyle="None" BorderWidth="1px" Font-Size="Small" CssClass="gridview" ShowHeader="true"
                    OnRowDeleting="OnRowDeleting">
                    <%--DataKeyNames="PTId,CreatedBy,CreatedDate,SentBy,SentDate,ResolvedBy,ResolvedDate,DeptAndPT"--%>
                    <RowStyle BackColor="White" ForeColor="#330099" BorderColor="#707070" BorderStyle="Solid"
                        BorderWidth="1px" />
                    <Columns>
                         <asp:BoundField HeaderText="PTId" DataField="PTId" HeaderStyle-CssClass="hidden"
                            ItemStyle-CssClass="hidden" ReadOnly="true"  />
                       <%-- <asp:BoundField HeaderText="PTId" DataField="PTId" Visible="false" ReadOnly="true"  />--%>
                        <asp:BoundField HeaderText="DepartmentIds" DataField="PTDepartmentId" Visible="false" ReadOnly="true"   />
                        <asp:BoundField HeaderText="Code" DataField="PTCode" ItemStyle-CssClass="gridRow"   ReadOnly="true"
                            ItemStyle-Width="5%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="PTCategoryId" DataField="PTCId" Visible="false"   ReadOnly="true" />
                        <asp:BoundField HeaderText="PTCategoryDesc" DataField="PTCDesc" Visible="false"  ReadOnly="true"  />
                        <asp:BoundField HeaderText="Job" DataField="Article" ItemStyle-CssClass="gridRow"   ReadOnly="true"
                            ItemStyle-Width="6%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <%--<asp:BoundField HeaderText="Remarks" DataField="PTSRemarks" ItemStyle-CssClass="gridRow"
                            ItemStyle-Width="65%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>--%>

                           <asp:TemplateField HeaderText="Remarks">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%# Bind("PTSRemarks") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("PTSRemarks") %>' TextMode="MultiLine"  Width="80%"></asp:TextBox>
                            </EditItemTemplate>
                            <ItemStyle CssClass="gridRow" Width="39%" />
                        </asp:TemplateField>
                        <%--<asp:TemplateField>
                         <ItemTemplate>
                             <asp:Label ID="Label3" runat="server"  Text='<%# Eval("PTSRemarks") %>'></asp:Label></ItemTemplate>
                         <EditItemTemplate>
                             <asp:TextBox ID="Edit_remarks" runat="server" Text='<%# Eval("PTSRemarks") %>' ></asp:TextBox>
                         </EditItemTemplate>
                         </asp:TemplateField>--%>
                        <asp:BoundField HeaderText="Status" DataField="Status" ItemStyle-CssClass="gridRow"   ReadOnly="true"
                            ItemStyle-Width="5%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Type" DataField="QueryType" ItemStyle-CssClass="gridRow"   ReadOnly="true"
                            ItemStyle-Width="5%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <%--<asp:BoundField HeaderText="Response" DataField="ResolvedRemarks" ItemStyle-CssClass="gridRow"   ReadOnly="true" 
                            ItemStyle-Width="15%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>--%>
                           <asp:TemplateField HeaderText="Response">
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server"  Text='<%# Bind("ResolvedRemarks") %>' ></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox2" runat="server"  Text='<%# Bind("ResolvedRemarks") %>' TextMode="MultiLine" Width="100%"  ></asp:TextBox>
                        </EditItemTemplate>
                        <ItemStyle CssClass="gridRow" Width="40%" />
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Created By" DataField="CreatedBy" Visible="false"   ReadOnly="true" />
                        <asp:BoundField HeaderText="Created Date" DataField="CreatedDate" Visible="false"  ReadOnly="true"  />
                        <asp:BoundField HeaderText="Sent By" DataField="SentBy" Visible="false"    ReadOnly="true"/>
                        <asp:BoundField HeaderText="Sent Date" DataField="SentDate" Visible="false"   ReadOnly="true" />
                        <asp:BoundField HeaderText="Resolved By" DataField="ResolvedBy" Visible="false"    ReadOnly="true"/>
                        <asp:BoundField HeaderText="Resolved Date" DataField="ResolvedDate" Visible="false"  ReadOnly="true"  />
                        <asp:BoundField HeaderText="DeptAndPT" DataField="DeptAndPT" Visible="false"   ReadOnly="true" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnEdit" runat="server" ImageUrl="~/Images/Edit.png" CommandName="Edit" OnClick="ImageEdit" />
                                <%--<asp:Label ID="lblCloseStatus" runat="server" CssClass="hiddenGridColumn"></asp:Label>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:CommandField ShowEditButton="true" ItemStyle-CssClass="gridRow" ItemStyle-Width="5%"
                            ButtonType="Image" EditImageUrl="~/Images/Edit.png" ItemStyle-HorizontalAlign="Center">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:CommandField>--%>
                        <asp:CommandField ButtonType="Link" ShowDeleteButton="true" ItemStyle-Width="150" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" Text="Edit" runat="server" CommandName="Edit" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="LinkButton2" Text="Update" runat="server" OnClick="OnUpdate" />
                                <asp:LinkButton ID="LinkButton3" Text="Cancel" runat="server" OnClick="OnCancel" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                         <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button ID="AppproveBtn" runat="server" CausesValidation="false"  OnClick="ApproveBnClick"
                                    CommandArgument='Approve' Text="Approve" />
                                <%--  <asp:LinkButton ID="AppproveBtn" runat="server" Text="Approve" CommandName="Edit" OnClick="ApproveBnClick" />--%>
                            </ItemTemplate>

                           <%-- <EditItemTemplate>
                                <asp:LinkButton ID="AppproveBtnID" Text="Approve" runat="server" OnClick="OnApprove" />
                            </EditItemTemplate>--%>
                        </asp:TemplateField>
                        
                        <asp:BoundField HeaderText="Date" ControlStyle-Width="120px" DataField="CreatedDate" ItemStyle-CssClass="gridRow"   ReadOnly="true"
                            ItemStyle-Width="100%" DataFormatString="{0:MM/dd/yyyy hh:mm tt}">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                    </Columns>
                    <HeaderStyle BackColor="#dbeaf9" Font-Bold="True" ForeColor="navy" BorderColor="#707070"
                        BorderStyle="Solid" BorderWidth="1px" CssClass="gridHeader" />
                    <AlternatingRowStyle BorderColor="#707070" BorderStyle="Solid" BorderWidth="1px" />
                </asp:GridView>
            </div>
        </div>
    </div>

    <div id="dvHide" style="width: 100%; float: left;">
        <input type="hidden" id="JobTypeHidden" runat="server" />
        <asp:HiddenField ID="hdnXml" runat="server" Value="FirstTime" />
        <%--<asp:TextBox ID="UserNameTextBox" ReadOnly="true" runat="server" class="PTSTextbox"></asp:TextBox>--%>
    </div>
    </form>
</body>
</html>
