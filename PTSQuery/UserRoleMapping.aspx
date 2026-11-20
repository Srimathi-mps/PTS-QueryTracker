<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserRoleMapping.aspx.cs" Inherits="PTS.UserRoleMapping" EnableSessionState="True" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>User Role Mapping</title>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <script language ="javascript" type ="text/javascript" src="Javascript/UserRole.js">
    </script>
</head>
<body>
    <form id="form1" runat="server">
       
    <div> 
     <AjaxToolKit:ScriptManager ID="scriptManager1" runat="server"></AjaxToolKit:ScriptManager>
        <table cellpadding="0" cellspacing="0" style="border-width:1px; width:100%; border-style:solid;">
            <tr class="PTSTableRow">
                <td class="UserRoleLabel"> 
                    UserName
                </td>
                <td>
                     <asp:TextBox ID="UserNameTextbox" runat="server" CssClass="PTSTextbox" Width="250px">  </asp:TextBox>
                
              
                   
                     <AjaxToolKit:AutoCompleteExtender runat="server" ID="autoComplete1" TargetControlID="UserNameTextbox"
                                       ServiceMethod="GetCompletionList" MinimumPrefixLength="1" 
                                        CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20" />
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td class="UserRoleLabel">
                    UserRole
                </td>
                <td>
                    <asp:DropDownList ID="RoleDropDown" runat="server" ></asp:DropDownList>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="2">
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="2">
                    <asp:Button ID="MapButton" runat="server" Text="MapUser" 
                        onclick="MapButton_Click" />
                    <asp:Button ID="UpdateButton" runat="server" Text="Update" 
                        onclick="UpdateButton_Click" />
                    <asp:Button ID="ResetButton" runat="server" Text="Reset" 
                        onclick="ResetButton_Click" />
                </td>
           </tr>
           <tr class="PTSTableRow">
            <td colspan="2">
                <table cellpadding="0" cellspacing="0" style="border-width:1px; width:100%; border-style:solid;">
                    <tr class="PTSTableRow">
                        <td class="UserRoleLabel"> 
                            UserName
                        </td>
                        <td>
                            <asp:TextBox ID="SearchUserNameTextBox" runat="server" CssClass="PTSTextbox">
                            </asp:TextBox>
                            <asp:Button ID="SearchButton" runat="server" Text="Search" 
                                onclick="SearchButton_Click" />
                        </td>
                    </tr>
                    <tr class="PTSTableRow">
                        <td colspan="2">
                            <asp:GridView ID="GVUserRoles" Width="50%" runat="server" AutoGenerateColumns="false" AllowPaging="false"  CssClass="grid-view"  AlternatingRowStyle-CssClass="alternate">
                                <Columns>
                                    <asp:TemplateField HeaderText="UserRoleMappingID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="GVLabelRoleMappingID" CssClass="gridText" runat="server" Text='<%#Bind("MappingID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="UserName">
                                        <ItemTemplate>
                                            <asp:Label ID="GVLabelUserName" CssClass="gridText" runat="server" Text='<%#Bind("UserName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="RoleID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="GVLabelRoleID" runat="server" Text='<%#Bind("roleid")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <asp:Label ID="GVLabelRole" CssClass="gridText" runat="server" Text='<%#Bind("rolename")%>'></asp:Label>
                                        </ItemTemplate>
                                        </asp:TemplateField>                        
                                    <asp:TemplateField  HeaderText ="select">
                                        <ItemStyle HorizontalAlign ="Center" />
                                        <ItemTemplate >
                                          <input type ="radio" id= "radbtn" CssClass="gridText" name="radbtn" onclick="ViewUserRole()"  value='<%#DataBinder.Eval(Container.DataItem, "MappingID")+ "~" + DataBinder.Eval(Container.DataItem,"UserName")+ "~" + DataBinder.Eval(Container.DataItem,"roleid") + "~" + DataBinder.Eval(Container.DataItem,"rolename")%>'   />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
           </tr>
           <tr>
                <td colspan="2">
                    <input type="hidden" id="MappingIDHidden" runat="server" />
                </td>
           </tr>
        </table>
    </div>
    </form>
</body>
</html>
