<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PTSHistory.aspx.cs" Inherits="PTS.PTSHistory"
    EnableSessionState="True" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PTS History Report</title>
    <link href="../Stylesheet/ReportStylesheet.css" rel="Stylesheet" type="text/css" />
    <link href="../Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <AjaxToolKit:ScriptManager ID="ScriptManager1" runat="server">
    </AjaxToolKit:ScriptManager>
    <table width="100%" border="0" style="table-layout: fixed; height: 100%;">
        <tr  valign="top">
            <td colspan="6" align="center" valign="middle" style="height: 75px;">
                <asp:Label ID="ReportTitleLabel" runat="server" CssClass="titleLabel"></asp:Label>
            </td>
        </tr>
       
        <tr  valign="top">
            <td align="left">
                Select User:
                <asp:DropDownList TabIndex="1" AutoPostBack="true" ID="ddlUsers" runat="server" OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        
            <td align="right">
                From Date :
            </td>
            <td align="left">
                <asp:TextBox ID="txtFromDate" AutoCompleteType="Disabled" runat="server" CssClass="DateTextbox"
                    TabIndex="2"></asp:TextBox>
                <AjaxToolKit:CalendarExtender ID="clExt1" runat="server" PopupButtonID="txtFinalStartDate"
                    TargetControlID="txtFromDate" Format="dd-MM-yyyy">
                </AjaxToolKit:CalendarExtender>
            </td>
            <td align="right">
                To Date :
            </td>
            <td width="129px" align="left">
                <asp:TextBox ID="txtToDate" AutoCompleteType="Disabled" runat="server" CssClass="DateTextbox"
                    TabIndex="2"></asp:TextBox>
                <AjaxToolKit:CalendarExtender ID="clExt2" runat="server" PopupButtonID="txtFinalStartDate"
                    TargetControlID="txtToDate" Format="dd-MM-yyyy">
                </AjaxToolKit:CalendarExtender>
            </td>
            <td align="left">
                <asp:Button ID="btnView" runat="server" Text="View" OnClick="btnView_Click" />
            </td>
        </tr>
        <tr valign="top">
            <td colspan="5" align="center" valign="top">
                <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="5"  align="center" valign="top">
                <asp:TextBox Width="100%" ID="txtMsg" ReadOnly="true" ForeColor="Red" BackColor="Transparent"
                    runat="server" BorderStyle="None" Font-Names="Copperplate Gothic Light" Font-Size="17pt"
                    Visible="false"></asp:TextBox>
                <rsweb:ReportViewer ID="rptPTSHistory" runat="server" Font-Names="Verdana" Font-Size="8pt"
                    Height="650px" Width="100%">
                    <LocalReport ReportPath="Reports\rptPTSHistory.rdlc">
                        <DataSources>
                            <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="dsPTSHistory_usp_PTS_GetPTSHistoryReport" />
                        </DataSources>
                    </LocalReport>
                </rsweb:ReportViewer>
                <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="GetData"
                    TypeName="PTS.dsAwaitingReplyReportTableAdapters.usp_PTS_GetAwaitingReplyReportTableAdapter">
                </asp:ObjectDataSource>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetData"
                    TypeName="PTS.dsPTSHistoryTableAdapters.usp_PTS_GetPTSHistoryReportTableAdapter">
                </asp:ObjectDataSource>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
