<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AwaitingReplyReport.aspx.cs"
    Inherits="PTS.AwaitingReplyReport1" EnableSessionState="True" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AwaitingReplyReport</title>
    <link href="../Stylesheet/ReportStylesheet.css" rel="Stylesheet" type="text/css" />
    <link href="../Stylesheet/Stylesheet.css" rel="Stylesheet" type="text/css" />

    <script src="../Javascript/jquery.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(function() {
            var i = 0;
            $('#txtFilterGrid').keyup(function() {
                var val = $(this).val().toUpperCase();
                $('#gvPendingReport > tbody > tr').each(function(index, element) {
                    if ($(this).text().toUpperCase().indexOf(val) < 0)
                        $(this).hide();
                    else
                        $(this).show();

                    var header = $('#gvPendingReport > tbody > tr:first')
                    header.show();
                });
            });



        });
    </script>

    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
            top: 0px;
            bottom: 0px;
        }
        .parentDivIE
        {
            width: 100%;
            max-width: 100%;
            height: 600px;
        }
        .parentDivNIE
        {
            width: 80%;
            max-width: 80%;
            height: 680px;
            overflow: auto;
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
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div id="parDiv">
        <table width="100%" border="0" style="table-layout: fixed; height: 100%; width: 100%">
            <tr>
                <td colspan="5" align="center" valign="middle">
                    <asp:Label ID="ReportTitleLabel" runat="server" CssClass="titleLabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="5">
                </td>
            </tr>
            <tr>
                <td colspan="5">
                </td>
            </tr>
            <tr>
                <td align="left">
                    User:
                    <asp:DropDownList TabIndex="1" ID="ddlUsers" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUsers_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td colspan="3" align="center" valign="middle">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td align="left">
                                From Date:
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtFromDate" AutoCompleteType="Disabled" runat="server" TabIndex="2"
                                    CssClass="DateTextbox"></asp:TextBox>
                                <AjaxToolKit:CalendarExtender ID="clExt1" runat="server" PopupButtonID="txtFinalStartDate"
                                    TargetControlID="txtFromDate" Format="dd-MM-yyyy">
                                </AjaxToolKit:CalendarExtender>
                            </td>
                            <td align="left">
                                To Date:
                            </td>
                            <td align="left">
                                <asp:TextBox ID="txtToDate" AutoCompleteType="Disabled" CssClass="DateTextbox" runat="server"
                                    TabIndex="2"></asp:TextBox>
                                <AjaxToolKit:CalendarExtender ID="clExt2" runat="server" PopupButtonID="txtFinalStartDate"
                                    TargetControlID="txtToDate" Format="dd-MM-yyyy">
                                </AjaxToolKit:CalendarExtender>
                            </td>
                            <td align="left">
                                <asp:Button ID="btnView" runat="server" Text="View" OnClientClick="return Validate();"
                                    OnClick="btnView_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" 
                        onclick="btnExportExcel_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="5" align="center">
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <%--<tr>
                <td colspan="5" valign="top">
                    <asp:TextBox Width="100%" ID="txtMsg" ReadOnly="true" ForeColor="Red" BackColor="Transparent"
                        runat="server" BorderStyle="None" Font-Names="Copperplate Gothic Light" Visible="false"></asp:TextBox>
                    <rsweb:ReportViewer ID="rptAwaitingReply" Height="100%" Width="100%" runat="server" ShowParameterPrompts="False" AsyncRendering="False" CssClass="parentDivIE">
            </rsweb:ReportViewer>
                    <rsweb:ReportViewer ID="rptAwaitingReply" runat="server" Font-Names="Verdana" Font-Size="8pt"
                        Width="100%" Height="650px">
                        <LocalReport ReportPath="Reports\rptAwaitingReplyReport.rdlc">
                            <DataSources>
                                <rsweb:ReportDataSource DataSourceId="ObjectDataSource1" Name="dsAwaitingReplyReport_usp_PTS_GetAwaitingReplyReport" />
                            </DataSources>
                        </LocalReport>
                    </rsweb:ReportViewer>
                    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="GetData" TypeName="PTS.Reports.dsAwaitingReplyReportTableAdapters.usp_PTS_GetAwaitingReplyReportTableAdapter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ReportTitleLabel" DefaultValue="1" Name="stage"
                                PropertyName="Text" Type="String" />
                            <asp:ControlParameter ControlID="ddlUsers" DefaultValue="1" Name="user" PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>--%>
            <tr>
                <td colspan="5" valign="top">
                    <div style="height: 500px; overflow: auto;">
                        <asp:TextBox Width="100%" ID="txtMsg" ReadOnly="true" ForeColor="Red" BackColor="Transparent"
                            runat="server" BorderStyle="None" Font-Names="Copperplate Gothic Light" Visible="false"></asp:TextBox>
                        <asp:GridView ID="gvPendingReport" runat="server" AllowPaging="false" AllowSorting="false"
                            Width="100%" AutoGenerateColumns="false" CssClass="grid-view" AlternatingRowStyle-CssClass="alternate">
                            <Columns>
                            <asp:TemplateField HeaderText="Customer Name">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("cust_sname")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Book">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("Jour_sname")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="CustomerCode">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("CUSTOMER")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="BookCode">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelCustomer" CssClass="gridText" runat="server" Text='<%#Bind("Journal")%>'></asp:Label>
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
                                        <asp:Label ID="GVLabelPTSRemarks" CssClass="gridText" runat="server" Text='<%#Bind("PTSRemarks")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Taskclosed">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelVolume" CssClass="gridText" runat="server" Text='<%#Bind("Taskclosed")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                               
                                <asp:TemplateField HeaderText="Reported Date">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelReportedDate" CssClass="gridText" runat="server" Text='<%#Bind("ReportedDate","{0:dd-MMM-yyyy}")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Days Pending">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabeldays" CssClass="gridText" runat="server" Text='<%#Bind("days")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
    
                               
                                <asp:TemplateField HeaderText="ZipFilePath" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelZipFilePath" CssClass="gridText" runat="server" Text='<%#Bind("ZipFilePath")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="QuerySavedBy">
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
                                </asp:TemplateField>
                                
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
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
                            <asp:BoundColumn HeaderText="Customer Name" DataField="cust_sname" />
                            <asp:BoundColumn HeaderText="Book" DataField="Jour_sname" />
                            <asp:BoundColumn HeaderText="CustomerCode" DataField="CUSTOMER" />
                            <asp:BoundColumn HeaderText="BookCode" DataField="Journal" />
                            
                            <asp:BoundColumn HeaderText="Stage" DataField="Stage" />
                            <asp:BoundColumn HeaderText="Chapter" DataField="Article" />
                            <asp:BoundColumn HeaderText="Queries Raised to Customer" DataField="PTSRemarks" />
                            <asp:BoundColumn HeaderText="Taskclosed" DataField="Taskclosed" />
                            <asp:BoundColumn HeaderText="Reported Date" DataField="ReportedDate" DataFormatString="{0:dd-MMM-yyyy}" />
                            <asp:BoundColumn HeaderText="Days Pending" DataField="days" />
                            
                            <asp:BoundColumn HeaderText="ZipFilePath" DataField="ZipFilePath" />
                            <asp:BoundColumn HeaderText="QuerySavedBy" DataField="QuerySavedBy" />
                            <asp:BoundColumn HeaderText="QuerySavedDate" DataField="QuerySavedDate"  DataFormatString="{0:dd-MMM-yyyy}"/>
                            <asp:BoundColumn HeaderText="QueryRaisedBy" DataField="QueryClosedBy"/>
                            <asp:BoundColumn HeaderText="QueryRaisedDate" DataField="QueryClosedDate"  DataFormatString="{0:dd-MMM-yyyy}"/>
                            
                        </Columns>
                    </asp:DataGrid>
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
