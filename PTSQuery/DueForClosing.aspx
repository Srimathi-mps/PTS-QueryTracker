<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DueForClosing.aspx.cs" Inherits="PTS.DueForClosing" EnableSessionState="True" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Due for closing articles</title>
       <script src="Javascript/jquery.js" type="text/javascript"></script>
    <style type="text/css">
        .Dialog
        {
            background: none repeat scroll 0 0 #E5E8D2;
            border: 1px solid #466D77;
        }
        body
        {
            font-family: Verdana,Tahoma,Arial !important;
            font-size: 9pt;
        }
        .modalPopup
        {
            background-color: #696969;
            filter: alpha(opacity=40);
            opacity: 0.7;
            xindex: -1;
            position:absolute;
        }
    </style>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
     <link href="/Stylesheet/ReportStylesheet.css" rel="Stylesheet" type="text/css" />
    <script language="javascript" type="text/javascript" src="Javascript/DueForClosing.js">
    </script>
    <script type="text/javascript">
        function ClosePopup() {
            //Hide the modal popup - the update progress
            var popup = $find('<%= modalpopup1.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
        }

        function ValidateGrid() {

            var gridView = document.getElementById("<%=GVDueArticles.ClientID %>");
            var checkBoxes = gridView.getElementsByTagName("input");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].checked) {
                    return;
                }
            }
            alert("Please select atleast one query");
        }
        $(function() {
        var i = 0;
       
            $('#txtFilterGrid').keyup(function() {

                var val = $(this).val().toUpperCase();
               
                $('#GVDueArticles > tbody > tr').each(function(index, element) {
                    
                    if ($(this).text().toUpperCase().indexOf(val) < 0)
                        $(this).hide();
                    else
                        $(this).show();

                    var header = $('#GVDueArticles > tbody > tr:first')
                    header.show();
                });
            });
        });

        function OpenURL(pageURL) {
            var x = 0;
            var y = 0;
            //alert(pageURL);
            if (pageURL != "")
                window.open(pageURL, "newpage", 'directories=no,titlebar=yes,toolbar=yes,status=yes,menubar=yes,scrollbars=yes,resizable=yes,top=' + y + ',left=' + x);
            else
                alert('Not a valid URL');
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <AjaxToolKit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </AjaxToolKit:ToolkitScriptManager>
   
    <div>

                   <table cellpadding="0" cellspacing="0" 
                       style="border-width:1px; width:159%; border-style:solid;">
            <tr class="PTSTableRow">
                <td align="center" colspan="4">
                    <asp:Label ID="StageLabel" runat="server" CssClass="titleLabel"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td align="left">
                    <asp:Label ID="lblSortOrder" Text="Sort By : " runat="server" CssClass="PTSLabel"></asp:Label>
                      </td>
                   <td align="left">
                   <asp:DropDownList ID="ddlSortOrder" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSortOrder_SelectedIndexChanged">
                    <asp:ListItem Text="INCREMENT" Value="INCREMENT"></asp:ListItem>
                    <asp:ListItem Text="REPORTEDON" Value="REPORTEDON"></asp:ListItem>
                    </asp:DropDownList>
               </td>
                <td align="right">
                                Filter:
                           </td>
                            <td align="left">
                                <asp:TextBox ID="txtFilterGrid" CssClass="DateTextbox" runat="server" Width="200px"></asp:TextBox>
                            </td>
            </tr>
            </table>
                    <table cellpadding="0" cellspacing="0" style="border-width:1px; width:100%; border-style:solid;">
            <tr class="PTSTableRow">
                <td>
                    <asp:GridView  ID="GVDueArticles" AutoGenerateColumns="false" runat="server"  
                        GridLines="both" onrowdatabound="GVDueArticles_RowDataBound" CssClass="grid-view"  AlternatingRowStyle-CssClass="alternate">
                        <Columns>
                            <asp:TemplateField HeaderText="" ItemStyle-Width="2%">
                                <ItemTemplate>
                                    <asp:CheckBox ID="GVCheckBox" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer" ItemStyle-Width="7%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCustomer" runat="server"  Text='<%# Bind("customer")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Journal" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelJournal" runat="server"  Text='<%# Bind("Journal")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                            
                            <asp:TemplateField HeaderText="Article" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelArticle" runat="server"  Text='<%# Bind("Article")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                               <asp:TemplateField HeaderText="Customer Stage Name" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCustomerStageName" runat="server"  Text='<%# Bind("CustomerStageName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                            <asp:TemplateField HeaderText="Vol" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelVolume" runat="server"  Text='<%# Bind("Volume")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Issn" ItemStyle-Width="3%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelIssue" runat="server"  Text='<%# Bind("Issue")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                               <asp:TemplateField HeaderText="Increment"  ItemStyle-Width="2%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelIncrement" runat="server"  Text='<%# Bind("Increment")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                           <asp:TemplateField HeaderText="Inquirer Name" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelEditor" runat="server"  Text='<%# Bind("InquirerName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Queries" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelRemarks" runat="server"  Text='<%# Bind("ptsremarks")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                           <asp:TemplateField HeaderText="Current ProcessName" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCurrentProcessName" runat="server"  Text='<%# Bind("CurrentProcessName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Graphics Status" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelGraphicsStatus" runat="server"  Text='<%# Bind("GraphicsStatus")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField HeaderText="Reported On" ItemStyle-Width="7%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelReportedOn" runat="server"  Text='<%# Bind("reporteddate","{0:dd-MMM-yyyy}")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Days Pending" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelDays" runat="server"  Text='<%# Bind("Days")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CustomerCode" Visible="false" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCustomerCode" runat="server"  Text='<%# Bind("customercode")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="journalCode" Visible="false" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelJournalCode" runat="server"  Text='<%# Bind("journalcode")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="StageID" Visible="false" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelStageID" runat="server"  Text='<%# Bind("StageID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Stage" Visible="false" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelStage" runat="server"  Text='<%# Bind("SubStageDescription")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ZipFilePath" Visible="false" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelZipFilePath" runat="server"  Text='<%# Bind("ZipFilePath")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ProblemTypeCode" Visible="false" ItemStyle-Width="3%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelProblemTypeCode" runat="server"  Text='<%# Bind("ProblemTypeCode")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CustomerGroupName" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelCustomerGroupName" runat="server"  Text='<%# Bind("CustomerGroupName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="S5DispatchStatus"  ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelS5DispatchStatus" runat="server"  Text='<%# Bind("S5DISPATCHSTATUS")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              
                              <asp:TemplateField HeaderText="TextRemarks"  ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelTextRemarks" runat="server"  Text='<%# Bind("TextRemarks")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Button ID="GVZipButton" runat="server" Text="Zip" />
                                    <asp:Image ID="GVZipImage" runat="server" Visible="false" ImageUrl="~/Images/zip.jpeg" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                           <asp:TemplateField HeaderText="ID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelID" runat="server"  Text='<%# Bind("ID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                             <asp:TemplateField HeaderText="Username" ItemStyle-Width="5%" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="GVLabelUsername" runat="server"  Text='<%# Bind("Username")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td>
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td>
                    <asp:Button ID="CloseButton" runat="server" Text="Close" 
                        onclick="CloseButton_Click" />
                    <asp:Button ID="DeleteButton" runat="server" Text="Delete" 
                        onclick="DeleteButton_Click" />
                    <asp:Button ID="btnMerge" Text="Merge Queries" runat="server"  OnClientClick="ValidateGrid()" Visible="false"/>
                    <%--<asp:CustomValidator ID="CustomValidator1" runat="server" 
                            ClientValidationFunction="ValidateGrid" ForeColor="Red"></asp:CustomValidator>--%>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblZipFile" runat="server" Text="" style="background-color:Transparent; color:White;"></asp:Label>
                    <asp:LinkButton ID="lbtnZipFile" runat="server" style="background-color:Transparent; color:White;"></asp:LinkButton>
                </td>
            </tr>
        </table>
    </div>
    <%--added by shiva--%>
    <div>
        <AjaxToolKit:ModalPopupExtender ID="modalpopup1" runat="server" BackgroundCssClass="modalPopup"
            TargetControlID="btnMerge" PopupControlID="pnllogin" X="300" Y="300" />
        <asp:Panel ID="pnllogin" runat="server" Style="display: none">
            <div style="width: 400px">
                <asp:Panel ID="pnldrag" runat="server">
                    <table class="Dialog" width="100%">
                        <tr style="background-image: url('Images/titlebar.jpg')">
                            <td style="color: #000000" align="center">
                                <img src="Images/dialog-close.gif" align="right" style="cursor: pointer; height: 25px;"
                                    alt="Close" onclick="ClosePopup()" />Merge Queries
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table width="100%" border="0px" align="center" style="">
                                    <tr>
                                        <td align="right">
                                            Queries:
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtQueries" TextMode="MultiLine" Width="300px" Height="80px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnsubmit" runat="server" Text="Ok"  />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:HiddenField ID="hdnPopup" runat="server" />
    </div>
    <%--till here added by shiva--%>
    </form>
</body>
</html>
