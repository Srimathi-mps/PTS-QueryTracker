<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="QueryTracker.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>QueryTracker</title>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" style="border-width:1px; width:550px; border-style:solid;">
            <tr class="PTSTableRow">
                <td class="PTSLabel"> 
                    User Name:
                </td>
                <td>
                    <asp:TextBox ID="UserNameTextBox" runat="server" class="PTSTextbox"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvUserName" ValidationGroup="userLogin" runat="server" ControlToValidate="UserNameTextBox" ErrorMessage="Username is blank">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabel"> 
                    Password:
                </td>
                <td>
                    <asp:TextBox ID="PasswordTextBox" runat="server" class="PTSTextbox" TextMode="Password"></asp:TextBox>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="2" align="center">
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                    <asp:ValidationSummary ID="VSUserLogin" runat="server" ValidationGroup="userLogin" ShowSummary="false" ShowMessageBox="true" />
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="2" align="center">
                    <asp:Button ID="LoginButton" runat="server" Text="Login" 
                        CausesValidation="true" ValidationGroup="userLogin" onclick="LoginButton_Click"
                         />
                    <asp:Button ID="ResetButton" runat="server" Text="Reset" CausesValidation="false" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
