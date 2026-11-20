<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ZipFiles.aspx.cs" Inherits="PTS.ZipFiles" EnableSessionState="True" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Zip files</title>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <script language="javascript" type="text/javascript">
        function CloseWindow() 
        {
//            ////window.close();
//            ////return false;
//            
//            //self.close();
//            //window.close();
//            //opener.focus();
            
            var HiddenZipFileName = document.getElementById('HiddenZipFileName');
            opener.document.getElementById('lblZipFile').innerHTML = HiddenZipFileName.value;
            var lbtnZipFile = opener.document.getElementById('lbtnZipFile');
            if(HiddenZipFileName.value != '')
                lbtnZipFile.innerHTML = "(Download)";
            else
                lbtnZipFile.innerHTML = "";
        }
        
        function ConfirmDelete() {
            var result = confirm("Are you sure you want to delete?");
            return result;
        }
    </script>
    
    
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" style="border-width:1px; border-style:solid; width:550px">
            <tr class="PTSTableRow">
                <td align="center" class="PTSLabel">
                    <strong>Zip files</strong>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td>
                    <asp:FileUpload ID="fileUpload" runat="server" />
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td align="center">
                    <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" />
                  
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td align="center">
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <asp:Panel ID="FilesPanel" runat="server">
            <tr class="PTSTableRow">
                <td>
                    <asp:GridView ID="zipFileGrid" AutoGenerateColumns="false" runat="server"  GridLines="both" 
                        OnRowDeleting="zipFileGrid_RowDeleting" DataKeyNames="fileName"  CssClass="grid-view"  AlternatingRowStyle-CssClass="alternate">
                        <Columns>
                            <asp:BoundField DataField="fileName" HeaderText="FileName" ControlStyle-CssClass="gridText"/>
                            <asp:TemplateField HeaderText="FilePath" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="filePathGridLabel"  CssClass="gridText" runat="server" Text='<%# Bind("sourcePath")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowDeleteButton="true" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td align="center">
                    <asp:Button ID="zipButton" runat="server" Text="Zip Files" OnClick="zipButton_Click" />
                    
                </td>
            </tr>
            </asp:Panel>
            <asp:Panel ID="ZipFilesPanel" runat="server">
            <tr class="PTSTableRow">
                <td>
                    <strong>Operator Uploaded zip file</strong>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td>
                    <asp:GridView ID="ShowZipFileGrid" runat="server" AutoGenerateColumns="false" AllowPaging="false"  CssClass="grid-view"  AlternatingRowStyle-CssClass="alternate">
                    <Columns>
                        <asp:BoundField DataField="zipFileName" HeaderText="FileName" ControlStyle-CssClass="gridText" />
                        <asp:TemplateField HeaderText="Path" ControlStyle-CssClass="gridHeading">
                            <ItemTemplate>
                                <a href='<%#DataBinder.Eval(Container, "DataItem.zipfilepath")%>' target="_blank"><%#DataBinder.Eval(Container, "DataItem.zipfilepath")%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td align="center">
                    <asp:Label ID="zipLabel" runat="server" ForeColor="red"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td align="center">
                    <asp:Button ID="deletebutton" runat="server" Text="Delete Zip File" OnClick="deletebutton_Click" OnClientClick="return ConfirmDelete();" />
                       <asp:Button ID="closeButton" runat="server" Text="Ok" 
                        onclick="closeButton_Click"/>
                </td>
            </tr>
           
            </asp:Panel>
            <tr>
                <td>
                    <input type="hidden" id="HiddenZipFileName" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
