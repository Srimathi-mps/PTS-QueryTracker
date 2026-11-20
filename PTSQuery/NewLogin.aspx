<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewLogin.aspx.cs" Inherits="QueryTracker.NewLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
    <style type="text/css">
        .style1
        {
            height: 21px;
            width: 74px;
        }
        .style2
        {
            height: 26px;
            width: 74px;
        }
        body, html 
        {
        height: 100%;
        width: 100%;
        margin: 0;
        padding: 0;
        }
     <%--  #image 
        {
        width: 100%;
        height: 100%;
        background: #FFF url('Images/Query_tracker1.jpg') center no-repeat;
        background-size: auto 100%;
        }--%>
    </style>
</head>
<body style="background-image: url(Images/bg.JPG)" class="container">
<img id="Img1" alt="MPS" src="Images/Logo_NEW(2).jpg" style="height: 72px" /><br />
    <br />
    <hr />
    <form id="image" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    &nbsp;&nbsp;&nbsp;&nbsp;<span class="head1"></span>
    <br />
    <center>
        &nbsp;&nbsp;
       <asp:Panel ID="Panel1" runat="server" Width="622px" Style="table-layout: fixed; background-image: url(Images/Login6.JPG); background-repeat:no-repeat;"
            Height="188px">
            <div>
                <br />
                <br />
                <br />
                <table style="position:relative; ">
                    <tr>
                        <td style="width: 213px; height: 21px">
                            <asp:Label ID="Label1" runat="server" Font-Names="Arial" ForeColor="Black" Text="User Name: " style="padding-left:106px;"
                                Font-Bold="True" Font-Size="12pt"></asp:Label>
                        </td>
                        <td class="style1">
                            <asp:TextBox ID="UserNameTextBox" runat="server" Font-Bold="False" 
                                Style="text-transform: uppercase" Width="150px"></asp:TextBox>
                        </td>
                        <td style="width: 104px; height: 21px">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="UserNameTextBox"
                                ErrorMessage="UserName Required">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 213px; height: 26px">
                            <asp:Label ID="Label2" runat="server" Font-Names="Arial" ForeColor="Black" Text="Password: " style="padding-left:106px;"
                                Font-Bold="True" Font-Size="12pt"></asp:Label>
                        </td>
                        <td class="style2">
                            <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" Width="149px"></asp:TextBox>
                        </td>
                        <td style="width: 104px; height: 26px">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="PasswordTextBox"
                                ErrorMessage="Password Required">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 213px; height: 26px;">
                        </td>
                        <td>
                            <asp:Button ID="Button1" runat="server" Text="Login" Font-Bold="true" Width="66px"
                                Style="margin-left: 0px" OnClick="Button1_Click" />
                            <asp:Button ID="Button2" runat="server" Text="Reset" Font-Bold="true" Width="60px"
                                Style="margin-left: 0px" CausesValidation="false" 
                                />
                            <%--<asp:Button ID="Button3" runat="server" Text="Report" Font-Bold="true" Width="68px"
                                Style="margin-left: 0px" CausesValidation="false" 
                                onclick="Button3_Click" />--%>
                        </td>
                        <td style="width: 104px; height: 26px">
                        </td>
                    </tr>
                </table>
            </div>
            <center>
                &nbsp;&nbsp;<br />
                <br />
                <br />
            </center>
            <center>
                &nbsp;</center>
            <center>
                &nbsp;</center>
            <center>
                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;
                <asp:Label ID="lblMsg" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="Small"
                    ForeColor="Red"></asp:Label>&nbsp;&nbsp; &nbsp;&nbsp;<asp:ValidationSummary ID="ValidationSummary2"
                        runat="server" ShowMessageBox="True" ShowSummary="False" />
            </center>
        </asp:Panel>
        </center>
       
    </form>
</body>
</html>
