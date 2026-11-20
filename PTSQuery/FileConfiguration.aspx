<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileConfiguration.aspx.cs"
    Inherits="QueryTracker.FileConfiguration" EnableSessionState="True" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PTS Configuration</title>
    <link href="Stylesheet/Stylesheet.css" rel="Stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript" src="Javascript/strTrim.js"></script>

    <script language="javascript" type="text/javascript" src="Javascript/PTSConfiguration.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" style="border-width: 1px; width: 100%; border-style: solid;">
           <tr class="PTSTableRow">
                <td class="PTSLabel" style="width: 15%">
                    Customer
                </td>
                <td>
                    <asp:DropDownList ID="CustomerDropDown" runat="server" CssClass="PTSCombo"  OnSelectedIndexChanged="CustomerDropDown_SelectedIndexChanged" AutoPostBack="true" >
                        <asp:ListItem Value="0">Select Customer</asp:ListItem>
                    </asp:DropDownList>
                    <span class="errorMessage">*</span>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabel" style="width: 15%">
                    Stage
                </td>
                <td>
                    <asp:DropDownList ID="StageDropDown" runat="server" CssClass="PTSCombo">
                        <asp:ListItem Value="0">Select Stage</asp:ListItem>
                    </asp:DropDownList>
                    <span class="errorMessage">*</span>
                </td>
            </tr>
         
            <tr>
                <td class="PTSLabel">
                    Type
                </td>
                <td>
                    <asp:RadioButtonList ID="rdoType" runat="server" RepeatDirection="Horizontal" 
                        OnSelectedIndexChanged="rdoType_SelectedIndexChanged" AutoPostBack="true">
                        <asp:ListItem Value="0" Selected="True">Signal</asp:ListItem>
                        <asp:ListItem Value="1">Email</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <asp:Panel ID="pnlSignal" runat="server">
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        SignalFileName
                    </td>
                    <td>
                        <asp:TextBox ID="SignalFileNameTextBox" CssClass="PTSTextbox" runat="server" Width="200px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        SignalFileDestination
                    </td>
                    <td>
                        <asp:TextBox ID="SignalFileDestinationTextBox" CssClass="PTSTextbox" runat="server"
                            Width="500px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
               
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        Signal Template
                    </td>
                    <td>
                        <asp:FileUpload ID="SignalTemplateFileUpload" runat="server" />
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlEmail" runat="server">
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        From Email
                    </td>
                    <td>
                        <asp:TextBox ID="txtFromEmail" CssClass="PTSTextbox" runat="server" Width="200px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        To Email
                    </td>
                    <td>
                        <asp:TextBox ID="txtToEmail" CssClass="PTSTextbox" runat="server" Width="500px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        Email Subject
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmailSubject" CssClass="PTSTextbox" runat="server" Width="200px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        Email Content
                    </td>
                    <td>
                        <asp:TextBox ID="txtEmailContent" CssClass="PTSTextbox" runat="server" Width="500px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
            </asp:Panel>
             <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        ZipFileName
                    </td>
                    <td>
                        <asp:TextBox ID="ZipFileNameTextBox" CssClass="PTSTextbox" runat="server" Width="200px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
                <tr class="PTSTableRow">
                    <td class="PTSLabel">
                        ZipFileDestination
                    </td>
                    <td>
                        <asp:TextBox ID="ZipFileDestinationTextBox" CssClass="PTSTextbox" runat="server"
                            Width="500px"></asp:TextBox>
                        <span class="errorMessage">*</span>
                    </td>
                </tr>
            <tr class="PTSTableRow">
                <td colspan="2" align="center">
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="2">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                    <asp:Button ID="UpdateButton" runat="server" Text="Update" OnClick="UpdateButton_Click" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="2">
                    <asp:GridView ID="GVPTSConfiguration" runat="server" AutoGenerateColumns="false"
                        AllowPaging="false" AllowSorting="false" DataKeyNames="ConfigurationID" OnSelectedIndexChanged="GVPTSConfiguration_SelectedIndexChanged"
                        OnRowDataBound="GVPTSConfiguration_RowDataBound"  CssClass="grid-view"  AlternatingRowStyle-CssClass="alternate">
                        <Columns>
                         <asp:CommandField ShowSelectButton="True" SelectText="Edit" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="GVLinkDelete" OnClick="GVLinkDelete_Click" runat="server" Text="Delete"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ConfigurationID"
                                Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelConfigurationID"  runat="server" Text='<%#Bind("ConfigurationID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stage">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelStage"  runat="server" Text='<%#Bind("stage")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCustomer"  runat="server" Text='<%#Bind("customer_grname")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                               <asp:TemplateField HeaderText="CustomerName">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCustomerName"  runat="server" Text='<%#Bind("CustomerName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                                <asp:TemplateField HeaderText="Response Type">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelResponseType"  runat="server" Text='<%#Bind("ResponseType")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="SignalFilePath">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelSignalFilePath"  runat="server" Text='<%#Bind("signalfiledestination")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ZipFilePath">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelZipFilePath"  runat="server" Text='<%#Bind("zipfiledestination")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Template File Name">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelTemplateFileName"  runat="server" Text='<%#Bind("SignalTemplateFileName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                             <asp:TemplateField HeaderText="From EmailID">
                                <ItemTemplate>
                                    <asp:Label ID="GVFromEmailID"  runat="server" Text='<%#Bind("FromEmailID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                             <asp:TemplateField HeaderText="To EmailID">
                                <ItemTemplate>
                                    <asp:Label ID="GVToEmailID"  runat="server" Text='<%#Bind("ToEmailID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                         
                            
                             <asp:TemplateField HeaderText="Email Subject">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelEmailSubject"  runat="server" Text='<%#Bind("EmailSubject")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                                <asp:TemplateField HeaderText="Email Body">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelEmailContent"  runat="server" Text='<%#Bind("EmailContent")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                         
                            
                           
                                <asp:TemplateField HeaderText="Response Type" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelResponseTypeValue"  runat="server" Text='<%#Bind("ResponseTypeValue")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <input type="hidden" id="ConfigIDHidden" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
