<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryReport.aspx.cs" Inherits="QueryTracker.Reports.QueryReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <link href="../Stylesheet/SummaryReport.css" type="text/css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" style="border-width: 1px; width: 100%; border-style: solid;">
            <tr class="PTSTableRow">
                <td>
                    <asp:GridView ID="GVDueArticles" AutoGenerateColumns="false" runat="server" GridLines="both"
                        CssClass="grid-view" AlternatingRowStyle-CssClass="alternate">
                        <Columns>
                            <asp:TemplateField HeaderText="Journal" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelJournal" runat="server" Text='<%# Bind("Journal")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Chapter" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelArticle" runat="server" Text='<%# Bind("Article")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Queries" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelRemarks" runat="server" Text='<%# Bind("PTSRemarks")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ResolvedRemarks" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelDays" runat="server" Text='<%# Bind("ResolvedRemarks")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
