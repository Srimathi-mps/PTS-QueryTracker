<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SummaryReport.aspx.cs"
    Inherits="PTS.Reports.SummaryReport" EnableSessionState="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Summary Report</title>
    <link href="../Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <link href="../Stylesheet/SummaryReport.css" type="text/css" rel="Stylesheet" />
    <script language="javascript" type="text/javascript">
        function OpenPTSResolved(pageURL) {

            var x = 20;
            var y = 0;
            window.open(pageURL, "_blank", 'directories=no,addressbar=no,titlebar=no,toolbar=no,status=no,menubar=no,scrollbars=yes,resizable=yes,height=' + (screen.height - 150) + ',width=' + (screen.width - 100) + 'top=' + y + ',left=' + x);
        }
        function OpenDueForClose(pageURL) {

            var x = 20;
            var y = 0;
            window.open(pageURL, "_blank", 'directories=no,addressbar=no,titlebar=no,toolbar=no,status=no,menubar=no,scrollbars=yes,resizable=yes,height=' + (screen.height - 150) + ',width=' + (screen.width - 100) + 'top=' + y + ',left=' + x);
        }
        function autoResize(id) {
            //            var newheight;
            //            var newwidth;

            //            if (document.getElementById) {
            //                newheight = document.getElementById(id).contentWindow.document.body.scrollHeight;
            //                newwidth = document.getElementById(id).contentWindow.document.body.scrollWidth;
            //            }

            //            document.getElementById(id).height = (newheight) + "px";
            //            document.getElementById(id).width = (newwidth) + "px";
            // myFrame = document.getElementById("pnl1");
            // myDiv = document.getElementById("ifrm");
            // myDiv.innerHTML = myFrame.document.body.innerText;
            var iframeElement = parent.document.getElementById(id);
            var height = document.documentElement.clientHeight - 100;
            iframeElement.style.height = height + "px";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div align="left">
        <table cellpadding="0" cellspacing="0" width="90%">
            <tr>
                <td colspan="5" align="center" class="SummaryReportHeading">
                    <u>Summary Report</u>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="center">
                    Customer
                </td>
                <td>
                    <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged"
                        Width="180px">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btngoto" runat="server" Text="GoTo QueryPage" Font-Bold="True" Font-Underline="False"
                        OnClick="btngoto_Click" />
                </td>
            </tr>
            <%-- <tr>
            <td colspan="5" align="center"></td>
            </tr>--%>
            <tr>
                <td colspan="5" align="center">
                    <asp:GridView ID="gvSummaryReport" runat="server" AutoGenerateColumns="false" CssClass="grid-view"
                        AlternatingRowStyle-CssClass="alternate" AllowPaging="false" AllowSorting="false"
                        OnRowDataBound="gvSummaryReport_RowDataBound" Width="80%" EmptyDataText="No queries raised for this customer"
                        EmptyDataRowStyle-BorderStyle="None" EmptyDataRowStyle-Font-Bold="true" EmptyDataRowStyle-Font-Size="Medium"
                        EditRowStyle-ForeColor="Red">
                        <Columns>
                            <asp:TemplateField HeaderText="StageID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblStageID" runat="server" Text='<%#Bind("SubStageID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stage">
                                <ItemTemplate>
                                    <asp:Label ID="lblSubStageName" runat="server" Text='<%#Bind("SubStageName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SubStageDescription" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblSubStageDescription" runat="server" Text='<%#Bind("SubStageDescription")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Instruction">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbInstruction" runat="server" Text='<%#Bind("Instruction")%>'
                                        CssClass="linkbutton"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Due for closing" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbDue" runat="server" Text='<%#Bind("Due")%>' CssClass="linkbutton"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Queries pending">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbPending" runat="server" Text='<%#Bind("pending")%>' CssClass="linkbutton"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="Reply received">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbreceived"  runat="server" Text='<%#Bind("received")%>' CssClass="linkbutton" ></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Queries Resolved(Last 30 days)">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbResolved" runat="server" Text='<%#Bind("Resolved")%>' CssClass="linkbutton"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
        <%--   <tr>
                <td align="center" class="SummaryReportLabel" >
                    Stage
                </td>
                <td align="center" class="SummaryReportLabel">
                    Due for closing
                </td>
                <td align="center" class="SummaryReportLabel">
                    Queries pending
                </td>
                <td align="center" class="SummaryReportLabel">
                    Reply received
                </td>
                <td align="center" class="SummaryReportLabel">
                    Queries Resolved
                </td>
            </tr>
         
            
        </table>
        <asp:Table runat="server" ID="mainTBL" Width="70%" >
        </asp:Table>--%>
    </div>
    <%--<div id="div1" style="height:490px">
    <asp:Panel ID="pnl1" runat="server">
    <iframe id="ifrm" src="QueryReport.aspx" scrolling="yes" width="100%" height="100%" frameborder="0" />
   </asp:Panel>   
    </div>--%>
    </form>
</body>
</html>
