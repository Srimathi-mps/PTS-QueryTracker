<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RaiseProblems.aspx.cs"
    Inherits="PTS.RaiseProblems" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Problem Tracking</title>
 
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
            font-family:Verdana,Arial;
            font-size:11px;
            font-weight:normal;
            height:25px;
            width:100%;	
            border:none;
        }
        .hiddenGridColumn
		{
			font-size: 0;
            height: 0;
            width: 0;
		}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>
    <div class="BodyContaint">
        <div style="font-size: 14px; font-weight: bolder; margin-bottom: 5px;">
            Problem Tracking
        </div>
        <div style="margin-bottom: 5px;">
            <table style="background-color: #dbeaf9;">
                <tr>
                    <td class="PTSLabelNew">
                        <div class="PTSLabelNew" style="text-align: left; padding-left: 20px;">
                            Barcode
                            <asp:TextBox ID="txtBarcode" runat="server" Width="350px"></asp:TextBox>
                            <asp:TextBoxWatermarkExtender ID="TextBoxWatermarkExtender1" runat="server" TargetControlID="txtBarcode"
                                WatermarkText="Cust|Jour|Art   or   Cust|Jour|Vol|Iss" WatermarkCssClass="WaterMark">
                            </asp:TextBoxWatermarkExtender>
                            <asp:Button ID="btnGoBarcode" runat="server" Text="Go" OnClick="btnGoBarcode_Click" />
                            <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div style="margin-bottom: 5px; width: 85%;">
            <div style="width: 90%; background-color: #dbeaf9; float: left;">
                <div style="width: 95%; background-color: #dbeaf9; float: left;">
                    <table style="width: 100%; background-color: #dbeaf9;">
                        <tr>
                            <td class="PTSLabelNew">
                                Customer
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="CustomerDropDown" runat="server" AutoPostBack="true" CausesValidation="true"
                                    CssClass="PTSCombo" ValidationGroup="StageValidation" OnSelectedIndexChanged="CustomerDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="PTSLabelNew">
                                Journal
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="JournalDropDown" runat="server" AutoPostBack="true" CssClass="PTSCombo"
                                    OnSelectedIndexChanged="JournalDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="MasterCopyJournalLabel" runat="server" CssClass="errorMessage" Text="Mastercopy journal"
                                    Visible="False" Font-Size="9px"></asp:Label>
                            </td>
                            <td class="PTSLabelNew">
                                Article
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="ArticleDropDown" runat="server" CssClass="PTSCombo" AutoPostBack="True"
                                    OnSelectedIndexChanged="ArticleDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <%--<td rowspan="5">
                            <asp:Button ID="btnGoDdl" runat="server" Text="Go" OnClick="btnGoDdl_Click" Visible="false" />
                        </td>--%>
                        </tr>
                        <tr>
                            <td class="PTSLabelNew">
                                Stage
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="StageDropDown" runat="server" CssClass="PTSCombo" AutoPostBack="True" Enabled="false">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="PTSLabelNew">
                                Volume
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="VolumeDropDown" AutoPostBack="true" runat="server" CssClass="PTSCombo"
                                    OnSelectedIndexChanged="VolumeDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="PTSLabelNew">
                                Issue
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="IssueDropDown" runat="server" AutoPostBack="true" CssClass="PTSCombo"
                                    OnSelectedIndexChanged="IssueDropDown_SelectedIndexChanged">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="PTSLabelNew">
                                Increment
                            </td>
                            <td class="PTSControl">
                                <asp:DropDownList ID="ddlRoundCount" runat="server" AutoPostBack="true" CssClass="PTSCombo" Enabled="false">
                                    <asp:ListItem Value="0">-Select-</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="width: 5%; float: left;">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <asp:Button ID="btnGoDdl" runat="server" Text="Go" OnClick="btnGoDdl_Click" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="width: 10%; float: right;">
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
                            <asp:Button ID="btnRaiseProblem" runat="server" Text="New Query" OnClick="btnRaiseProblem_Click"
                                CssClass="btClass" Visible="false" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div style="margin-bottom: 5px; width: 85%; float: left; margin-top: 5px; height: 400px;">
            <table class="gridHeader" style="width: 100%;">
                <tr>
                    <th style="width: 10%">
                        <span class="gridButton">Code</span>
                        <%--<asp:Button ID="gbtnPTCodeSort" runat="server" Text="Code" OnClick="gbtnPTCodeSort_Click"
                            CssClass="gridButton" ToolTip="Sort" />--%>
                    </th>
                    <th style="width: 65%">
                        <span class="gridButton">Remarks</span>
                        <%--<asp:Button ID="gbtnRemarksSort" runat="server" Text="Remarks" OnClick="gbtnRemarksSort_Click"
                            CssClass="gridButton" ToolTip="Sort"  />--%>
                    </th>
                    <th style="width: 5%">
                        <span class="gridButton">Status</span>
                        <%--<asp:Button ID="gbtnStatusSort" runat="server" Text="Status" OnClick="gbtnStatusSort_Click"
                            CssClass="gridButton" ToolTip="Sort" />--%>
                    </th>
                    <th style="width: 15%">
                        <span class="gridButton">Response</span>
                        <%--<asp:Button ID="gbtnResponseSort" runat="server" Text="Response" OnClick="gbtnResponseSort_Click"
                            CssClass="gridButton" ToolTip="Sort" />--%>
                    </th>
                    <th style="width: 5%">
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </th>
                </tr>
            </table>
            <div style="width: 100%; float: left; height: 400px; overflow: auto;">
                <asp:GridView ID="gvwOpenedRequest" runat="server" AutoGenerateColumns="False" CellPadding="4"
                    EmptyDataText="No records found !" OnRowEditing="gvwOpenedRequest_RowEditing"
                    DataKeyNames="PTId,CreatedBy,CreatedDate,SentBy,SentDate,ResolvedBy,ResolvedDate,DeptAndPT"
                    OnRowDataBound="gvwOpenedRequest_RowDataBound" BackColor="White" BorderColor="ActiveBorder"
                    BorderStyle="None" BorderWidth="1px" Font-Size="Small" CssClass="gridview" ShowHeader="false">
                    <RowStyle BackColor="White" ForeColor="#330099" BorderColor="#707070" BorderStyle="Solid"
                        BorderWidth="1px" />
                    <Columns>
                        <asp:BoundField HeaderText="PTId" DataField="PTId" Visible="false" />
                        <asp:BoundField HeaderText="DepartmentIds" DataField="PTDepartmentId" Visible="false" />
                        <asp:BoundField HeaderText="Code" DataField="PTCode" ItemStyle-CssClass="gridRow"
                            ItemStyle-Width="10%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="PTCategoryId" DataField="PTCId" Visible="false" />
                        <asp:BoundField HeaderText="PTCategoryDesc" DataField="PTCDesc" Visible="false" />
                        <asp:BoundField HeaderText="Remarks" DataField="PTSRemarks" ItemStyle-CssClass="gridRow"
                            ItemStyle-Width="65%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Status" DataField="Status" ItemStyle-CssClass="gridRow"
                            ItemStyle-Width="5%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Response" DataField="ResolvedRemarks" ItemStyle-CssClass="gridRow"
                            ItemStyle-Width="15%">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Created By" DataField="CreatedBy" Visible="false" />
                        <asp:BoundField HeaderText="Created Date" DataField="CreatedDate" Visible="false" />
                        <asp:BoundField HeaderText="Sent By" DataField="SentBy" Visible="false" />
                        <asp:BoundField HeaderText="Sent Date" DataField="SentDate" Visible="false" />
                        <asp:BoundField HeaderText="Resolved By" DataField="ResolvedBy" Visible="false" />
                        <asp:BoundField HeaderText="Resolved Date" DataField="ResolvedDate" Visible="false" />
                        <asp:BoundField HeaderText="DeptAndPT" DataField="DeptAndPT" Visible="false" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="imgbtnEdit" runat="server" ImageUrl="~/Images/Edit.png" CommandName="Edit" />
                                <%--<asp:Label ID="lblCloseStatus" runat="server" CssClass="hiddenGridColumn"></asp:Label>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <%--<asp:CommandField ShowEditButton="true" ItemStyle-CssClass="gridRow" ItemStyle-Width="5%"
                            ButtonType="Image" EditImageUrl="~/Images/Edit.png" ItemStyle-HorizontalAlign="Center">
                            <ItemStyle CssClass="gridRow"></ItemStyle>
                        </asp:CommandField>--%>
                        
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
