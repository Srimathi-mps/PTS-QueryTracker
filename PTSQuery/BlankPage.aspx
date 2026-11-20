<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BlankPage.aspx.cs" Inherits="PTS.BlankPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
     <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div >
     <asp:Label ID="lblUserError" runat="server" Font-Size="15px" Text="You are not authorized for this action" CssClass="errorMessageText"></asp:Label>
    </div>
    </form>
</body>
</html>
