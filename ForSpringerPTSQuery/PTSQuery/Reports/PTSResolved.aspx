<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PTSResolved.aspx.cs" Inherits="PTS.PTSResolved"
    EnableSessionState="True" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Query Resolved</title>
    <link href="../Stylesheet/ReportStylesheet.css" rel="Stylesheet" type="text/css" />
    <link href="../Stylesheet/Stylesheet.css" rel="Stylesheet" type="text/css" />
     <script src="../Javascript/jquery.js" type="text/javascript"></script>
     <style type="text/css">
        body
        {
        	margin:0;
        	padding:0;
        	top:0px;
        	bottom:0px;
        }
        .parentDivIE
        {
        	width:100%;
        	max-width:100%;
        	height:600px;
        }
        .parentDivNIE
        {
        	width:80%;
        	max-width:80%;
        	height:680px;
        	overflow:auto;
        }
    </style>
    
    <script type="text/javascript">
        function Validate() {
            var fdate = $('#txtFromDate').val();
            var tdate = $('#txtToDate').val();
            //if ((fdate != '' && tdate == '') || (fdate == '' && tdate != '')) {
            if (fdate.trim() == '' || tdate.trim() == '') {
                alert("Please enter the From Date and To Date");
                return false;
            }
        }

    </script>
    
</head>
<body >
    <form id="form1" runat="server">
     <div id="parDiv">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
   
    <table width="100%" style="table-layout: fixed; height: 100%;">
        <tr>
            <td colspan="6" align="center" valign="middle">
                <asp:Label ID="ReportTitleLabel" runat="server" CssClass="titleLabel"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="6" align="center">
                <asp:Label ID="MessageLabel" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
        <td colspan="6" ></td>
        </tr>
        <tr>
        <td colspan="6" ></td>
        </tr>
        <tr>
            <td>
                Select User :
                <asp:DropDownList TabIndex="1"  ID="ddlUsers" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="ddlUsers_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
      
            <td align="left">
                From Date :
            </td>
            <td>
                <asp:TextBox ID="txtFromDate" AutoCompleteType="Disabled" runat="server" CssClass="DateTextbox"
                    TabIndex="2"></asp:TextBox>
                <AjaxToolKit:CalendarExtender ID="clExt1" runat="server" PopupButtonID="txtFinalStartDate"
                    TargetControlID="txtFromDate" Format="dd-MM-yyyy">
                </AjaxToolKit:CalendarExtender>
            </td>
            <td align="left">
                To Date :
            </td>
            <td>
                <asp:TextBox ID="txtToDate" AutoCompleteType="Disabled" runat="server" CssClass="DateTextbox"
                    TabIndex="2"></asp:TextBox>
                <AjaxToolKit:CalendarExtender ID="clExt2" runat="server" PopupButtonID="txtFinalStartDate"
                    TargetControlID="txtToDate" Format="dd-MM-yyyy">
                </AjaxToolKit:CalendarExtender>
            </td>
            <td>
                <asp:Button ID="btnView" runat="server" Text="View" OnClientClick="return Validate();" OnClick="btnView_Click" />
            </td>
            <td>
                <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" 
                    onclick="btnExportExcel_Click" />
                </td>
        </tr>
      
        <tr>
                <td colspan="7" valign="top">
                    <div style="height: 500px; overflow:auto;">
                        <asp:TextBox Width="100%" ID="txtMsg" ReadOnly="true" ForeColor="Red" BackColor="Transparent"
                            runat="server" BorderStyle="None" Font-Names="Copperplate Gothic Light" Visible="false"></asp:TextBox>
                        <asp:GridView ID="gvPendingReport" runat="server" AllowPaging="false" AllowSorting="false"
                            Width="100%" AutoGenerateColumns="false" CssClass="grid-view" AlternatingRowStyle-CssClass="alternate">
                            <Columns>
                            <asp:TemplateField HeaderText="Customer Name">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("Cust_name")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Book">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("Jour_sname")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CustomerCode">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("Cust_code")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="BookCode">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("Jour_code")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                               
                                <asp:TemplateField HeaderText="Stage">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelStage" CssClass="gridText" runat="server" Text='<%#Bind("Stage")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Chapter">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelVolume" CssClass="gridText" runat="server" Text='<%#Bind("Article")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Queries Raised to Customer">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelPTSRemarks" CssClass="gridText" runat="server" Text='<%#Bind("CustRemarks")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Resolved Remarks">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelVolume" CssClass="gridText" runat="server" Text='<%#Bind("ResolveRemarks")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                               
                                <asp:TemplateField HeaderText="Reported Date">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelReportedDate" CssClass="gridText" runat="server" Text='<%#Bind("ReportedDate","{0:dd-MMM-yyyy}")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reopened Date">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelReportedDate" CssClass="gridText" runat="server" Text='<%#Bind("Reopeneddate","{0:dd-MMM-yyyy}")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="QueryResolved Date">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelReportedDate" CssClass="gridText" runat="server" Text='<%#Bind("ResolvedDate","{0:dd-MMM-yyyy}")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="TotalTimeTaken(Days)">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabeldays" CssClass="gridText" runat="server" Text='<%#Bind("TimeTaken")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
    
                               
                                <asp:TemplateField HeaderText="ZipFilePath">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelZipFilePath" CssClass="gridText" runat="server" Text='<%#Bind("ZipFilePath")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <%--<asp:TemplateField HeaderText="QuerySavedBy">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelQuerySavedBy" CssClass="gridText" runat="server" Text='<%#Bind("QuerySavedBy")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="QuerySavedDate">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelQuerySavedDate" CssClass="gridText" runat="server" Text='<%#Bind("QuerySavedDate")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="QueryRaisedBy">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelQueryClosedBy" CssClass="gridText" runat="server" Text='<%#Bind("QueryClosedBy")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="QueryRaisedDate">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelQueryClosedDate" CssClass="gridText" runat="server" Text='<%#Bind("QueryClosedDate")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                
                            </Columns>
                        </asp:GridView></div>
                       </td> </tr>
    </table>
    
    </div>
    <asp:Panel ID="pnl" runat="server">
        <table cellpadding="0" cellspacing="0" runat="server" id="ExportTable" visible="false">
            <tr>
                <td>
                    <asp:DataGrid ID="dgExcel" runat="server" Width="100%" AutoGenerateColumns="False"
                        EmptyDataRowStyle-BackColor="AliceBlue" EnableTheming="false" RowStyle-Height="20px"
                        HeaderStyle-Font-Bold="true">
                        <Columns>
                            <asp:BoundColumn HeaderText="Customer Name" DataField="Cust_name" />
                            <asp:BoundColumn HeaderText="Book" DataField="Jour_sname" />
                            <asp:BoundColumn HeaderText="CustomerCode" DataField="Cust_code" />
                            <asp:BoundColumn HeaderText="BookCode" DataField="Jour_code" />
                            
                            <asp:BoundColumn HeaderText="Stage" DataField="Stage" />
                            <asp:BoundColumn HeaderText="Chapter" DataField="Article" />
                            <asp:BoundColumn HeaderText="Queries Raised to Customer" DataField="CustRemarks" />
                            <asp:BoundColumn HeaderText="Resolved Remarks" DataField="ResolveRemarks" />
                            <asp:BoundColumn HeaderText="Reported Date" DataField="ReportedDate" DataFormatString="{0:dd-MMM-yyyy}" />
                            <asp:BoundColumn HeaderText="Reopened Date" DataField="Reopeneddate"  DataFormatString="{0:dd-MMM-yyyy}" />
                            <asp:BoundColumn HeaderText="QueryResolved Date" DataField="ResolvedDate"  DataFormatString="{0:dd-MMM-yyyy}" />
                             <asp:BoundColumn HeaderText="TotalTimeTaken(Days)" DataField="TimeTaken" />
                            <asp:BoundColumn HeaderText="ZipFilePath" DataField="ZipFilePath" />
                            <%--<asp:BoundColumn HeaderText="QuerySavedBy" DataField="QuerySavedBy" />
                            <asp:BoundColumn HeaderText="QuerySavedDate" DataField="QuerySavedDate"  DataFormatString="{0:dd-MMM-yyyy}"/>
                            <asp:BoundColumn HeaderText="QueryRaisedBy" DataField="QueryClosedBy"/>
                            <asp:BoundColumn HeaderText="QueryRaisedDate" DataField="QueryClosedDate"  DataFormatString="{0:dd-MMM-yyyy}"/>
                            --%>
                        </Columns>
                    </asp:DataGrid>
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
