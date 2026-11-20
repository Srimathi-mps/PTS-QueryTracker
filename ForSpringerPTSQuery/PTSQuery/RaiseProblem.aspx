<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RaiseProblem.aspx.cs" Inherits="PTS.RaiseProblem"
    EnableSessionState="True" ValidateRequest="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<meta http-equiv="X-UA-Compatible" content="IE=edge" />

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Raise problem</title>
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
        }
        .DescStyle
        {
            width:100%;
            height:150px;
            overflow:auto;                                
            border:inset 1px #E3E9EF;
            word-break:break-all;
        }
        .PTSComboSmall
        {
        	font-family:Verdana,Arial;
	        font-size:11px;
	        font-weight:normal;
	        height:25px;
	        width:150px;	
        }
    </style>
    
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />
    <script language="javascript" src="Javascript/RaiseProblem.js" type="text/javascript"></script>
    <script language="javascript" src="Javascript/strTrim.js" type="text/javascript"></script>

    <script type="text/javascript">
        function ClosePopup() {
            //Hide the modal popup - the update progress
            var popup = $find('<%= modalpopup1.ClientID %>');
            if (popup != null) {
                popup.hide();
            }
        }
        function ClearResolvedPopup(){
            var txtRemarks = document.getElementById('txtRemarks');
            var fileUpload = document.getElementById('fileUpload');
            txtRemarks.value = '';
            fileUpload.value = '';
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <AjaxToolKit:ScriptManager ID="scriptManager1" runat="server">
    </AjaxToolKit:ScriptManager>
    
    <div style="padding-left:5px; padding-right:5px; text-align:center;">
        <table style="width:100%;" cellpadding="0" cellspacing="0">
            <tr class="PTSTableRow">
                <td colspan="6" align="center">
                    <asp:Label ID="MessageLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="6" align="center">
                    <asp:Label ID="ResultLabel" runat="server" CssClass="errorMessage"></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabelNew">
                    Barcode 
                </td>
                <td class="PTSControl" colspan="5">
                    <div style="margin-left: -5px;">
                        <div>
                            <asp:TextBox ID="BarCodeTextBox" runat="server" class="PTSTextbox" Width="250px" Text="" ></asp:TextBox>
                            <asp:Button ID="GoButton" runat="server" Text="Go" OnClick="GoButton_Click" />
                        </div>
                        <div style="padding-left:3px;font-size:10px;color:Red;">
                            (Cust|Jour|Article or Cust|Jour|Vol|Iss )
                        </div>
                    </div>
                    
                    <%--<asp:TextBox ID="BarCodeTextBox" runat="server" class="PTSTextbox" Width="250px"></asp:TextBox>
                    <asp:Button ID="GoButton" runat="server" Text="Go" OnClick="GoButton_Click" />--%>
                </td>
            </tr>
            <tr id="Tr1" class="PTSTableRow" runat="server" visible="false">
                <td class="PTSLabelNew">
                    UserName
                </td>
                <td class="PTSControl" colspan="4">
                    <asp:TextBox ID="UserNameTextBox" ReadOnly="true" runat="server" class="PTSTextbox"></asp:TextBox>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabelNew">
                    Customer 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="CustomerDropDown" runat="server" AutoPostBack="true" CausesValidation="true"
                        CssClass="PTSCombo" OnSelectedIndexChanged="CustomerDropDown_SelectedIndexChanged"
                        ValidationGroup="StageValidation">
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="PTSLabelNew">
                    Journal 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="JournalDropDown" runat="server" AutoPostBack="true" CssClass="PTSCombo"
                        OnSelectedIndexChanged="JournalDropDown_SelectedIndexChanged">
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="MasterCopyJournalLabel" runat="server" CssClass="errorMessage" Text="Mastercopy journal"
                        Visible="false"></asp:Label>
                </td>
                <td colspan="2">&nbsp;</td>
            </tr>
            <asp:Panel ID="ArticlePanel" runat="server">
            <tr class="PTSTableRow" >
                <td class="PTSLabelNew">
                    Article 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="ArticleDropDown" runat="server" CssClass="PTSCombo" OnSelectedIndexChanged="ArticleDropDown_SelectedIndexChanged"
                            AutoPostBack="True">
                            <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="PTSLabelNew">
                    Volume 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="VolumeDropDown" AutoPostBack="true" runat="server" CssClass="PTSCombo"
                        OnSelectedIndexChanged="VolumeDropDown_SelectedIndexChanged">
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td colspan="2">&nbsp;</td>
            </tr>
            </asp:Panel>
            <asp:Panel ID="VolumeIssuePanel" runat="server">
            <tr class="PTSTableRow">
                <td class="PTSLabelNew">
                    Issue 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="IssueDropDown" runat="server" AutoPostBack="true" CssClass="PTSCombo" OnSelectedIndexChanged="IssueDropDown_SelectedIndexChanged"> 
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="PTSLabelNew">
                    Increment
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="ddlRoundCount" runat="server" AutoPostBack="true" CssClass="PTSCombo" >
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td colspan="2">&nbsp;</td>
            </tr>
            </asp:Panel>
            <tr class="PTSTableRow">
                <td class="PTSLabelNew">
                    Stage 
                </td>
                <td class="PTSControl">
                    <%--<asp:DropDownList ID="StageDropDown" runat="server" CssClass="PTSCombo" AutoPostBack="True"
                        OnSelectedIndexChanged="StageDropDown_SelectedIndexChanged">
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>--%>
                    <asp:DropDownList ID="StageDropDown" runat="server" CssClass="PTSCombo" AutoPostBack="True"
                        OnSelectedIndexChanged="StageDropDown_SelectedIndexChanged">
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td class="PTSLabelNew">
                    Department 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="PTSCombo" 
                        OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabelNew">
                    Problem Type 
                </td>
                <td  class="PTSControl">
                    <asp:DropDownList ID="ProblemTypeDropDown" runat="server" CssClass="PTSComboSmall" AutoPostBack="True"
                        OnSelectedIndexChanged="ProblemTypeDropDown_SelectedIndexChanged">
                        <asp:ListItem Value="0">-Select-</asp:ListItem>
                    </asp:DropDownList>
                  
                </td>
                <%--<td  class="PTSControl1">--%>
                   
                <%--</td>--%>
                 <td  class="PTSLabelNew">
                    Problem Type Category 
                </td>
                <td class="PTSControl">
                    <asp:DropDownList ID="ddlProblemTypeCategory" runat="server" CssClass="PTSComboSmall" >
                    </asp:DropDownList>
                      
                </td>
                <td colspan="2">&nbsp;</td>
            </tr>
            
            
            <tr class="PTSTableRow">
                <td class="PTSLabelNew" style="top:0px;">
                    Remarks
                </td>
                <td colspan="4" class="PTSControl" style=" margin-left:0px; padding-left:0px;">
                    <div style="text-align:left; width:100%;">
                        <asp:TextBox ID="txtPTCategoryRemark" runat="server" TextMode="MultiLine" Width="100%"
                            Rows="2"></asp:TextBox>
                    </div>
                </td>
                <td class="PTSControl">
                    <asp:Button ID="btnPTypeAdd" Text="Add" runat="server" onclick="btnPTypeAdd_Click" /> 
                    <asp:HiddenField ID="hfZipFilePath" runat="server" />
                    <%--<asp:TextBox ID="txtZipFilePath" runat="server"></asp:TextBox>--%>
                </td>
            </tr>
            
            <tr class="PTSTableRow">
                <td class="PTSLabelNew" style="top:0px;">
                    Problem Type Details
                </td>
                <td colspan="5" class="PTSControl">
                    <div style="text-align:center; width:100%; max-height:100px; overflow:auto; ">
                        <asp:GridView ID="gvwPTCategory" runat="server" AutoGenerateColumns="False" 
                            CellPadding="4" ForeColor="#333333" GridLines="None" 
                            onrowdeleting="gvwPTCategory_RowDeleting">
                            <RowStyle BackColor="#EFF3FB" />
                            <Columns>
                                <asp:BoundField HeaderText="ProblemTypeId" DataField="ProblemTypeId" Visible="false" />
                                <asp:BoundField HeaderText="ProblemTypeCode" DataField="ProblemTypeCode" />
                                <asp:BoundField HeaderText="ProblemCategoryId" DataField="ProblemCategoryId" Visible="false" />
                                <asp:BoundField HeaderText="ProblemCategory" DataField="ProblemCategory" />
                                <asp:BoundField HeaderText="PTCategoryRemark" DataField="PTCategoryRemark" />
                                <asp:BoundField HeaderText="PTDesc" DataField="PTDesc" Visible="false" />
                                <asp:CommandField HeaderText="Delete" ShowDeleteButton="true" />
                            </Columns>
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#2461BF" />
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>
                </td>
            </tr>
            
            <tr class="PTSTableRow">
                <td class="PTSLabelNew" style="top:0px;">
                    Remarks
                </td>
                <td colspan="5" class="PTSControl">
                    <div style="text-align:center; width:100%; ">
                        <asp:TextBox ID="PTDescriptionTextBox" runat="server" TextMode="MultiLine" Width="100%" 
                            Rows="10"></asp:TextBox>
                        
                        <%--<asp:Label ID="PTDescriptionTextBox" runat="server" Width="100%" Height="150px" CssClass="DescStyle" 
                            BorderColor="#E3E9EF" BorderStyle="Inset" BorderWidth="1px"></asp:Label>--%>
                            
                    </div>
                    <div style="float: left;">
                        <asp:Label ID="lblSequence" runat="server" Text="" Style="color: Red; display: none"
                            Font-Bold="true"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabelNew">
                    <asp:Label ID="lblOpenedRequests" runat="server" Text="Opened Requests"></asp:Label>
                </td>
                <td class="PTSControl" colspan="5">
                    <asp:DropDownList ID="ddlOpenedRequest" runat="server" CssClass="PTSCombo" AutoPostBack="True"
                        OnSelectedIndexChanged="ddlOpenedRequest_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td class="PTSLabelNew" valign="top">
                    <%--<asp:Label ID="lblZipFileCaption" runat="server" Text="ZipFile" Visible="false" style="padding-top:2px;"></asp:Label>--%>
                </td>
                <td class="PTSControl" colspan="4">
                    <asp:LinkButton ID="lbtnZipFile" runat="server" OnClick="lbtnZipFile_Click"></asp:LinkButton>
                    <asp:Label ID="lblZipFile" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td>
                </td>
                 <td colspan="4" style="text-align:left; ">
                    <asp:Button ID="ZipButton" Text="Attachment" runat="server" />
                    <asp:Button ID="SaveButton" Text="Save" runat="server" OnClick="SaveButton_Click" />
                    <asp:Button ID="CloseButton" Text="Close/Raise a Query" runat="server" OnClick="CloseButton_Click" />
                    <asp:Button ID="QueryResolveButton" Text="Query Response" runat="server" OnClientClick="javascript:ClearResolvedPopup();" />
                </td>
            </tr>
            <tr class="PTSTableRow">
                <td colspan="5" style="text-align:left; ">
                    &nbsp;
                </td>
            </tr>
            
            <tr class="PTSTableRow">
                <td class="PTSLabelNew" valign="top">
                    <asp:Label ID="lblPreviousRemarks" runat="server" Text="Previous Remarks"></asp:Label>
                </td>
                <td colspan="5" class="PTSControl">
                    <div style="overflow: auto; text-align:center; width:100%;max-height:250px;">
                        <asp:GridView ID="GVProblemHistory" runat="server" AllowPaging="false" AllowSorting="false" Width="100%"
                            AutoGenerateColumns="false" CssClass="grid-view" AlternatingRowStyle-CssClass="alternate">
                            <Columns>
                                <asp:TemplateField HeaderText="Reported Date">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelReportedDate" CssClass="gridText" runat="server" Text='<%#Bind("ReportedDate","{0:dd-MMM-yyyy}")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Query Count">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelSequence" CssClass="gridText" runat="server" Text='<%#Bind("Sequence")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Remarks">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelRemarks" CssClass="gridText" runat="server" Text='<%#Bind("CustRemarks")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="Resolved Remarks">
                                    <ItemTemplate>
                                        <asp:Label ID="GVLabelResolvedRemarks" CssClass="gridText" runat="server" Text='<%#Bind("ResolvedRemarks")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </td>
            </tr>
            
            <tr class="PTSTableRow">
                <td colspan="5">
                    <asp:ValidationSummary ID="VSStageValidation" runat="server" ShowSummary="false"
                        ValidationGroup="StageValidation" ShowMessageBox="true" />
                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <input type="hidden" id="JobTypeHidden" runat="server" />
                </td>
            </tr>
        </table>
    </div>

    <div>
        <AjaxToolKit:ModalPopupExtender ID="modalpopup1" runat="server" BackgroundCssClass="modalPopup"
            TargetControlID="QueryResolveButton" PopupControlID="pnllogin" X="200" Y="200" />
        <asp:Panel ID="pnllogin" runat="server" Style="display: none">
            <div style="width: 400px">
                <asp:Panel ID="pnldrag" runat="server">
                    <table class="Dialog" width="100%">
                        <tr style="background-image: url('Images/titlebar.jpg')">
                            <td style="color: #000000" align="center">
                                <img src="Images/dialog-close.gif" align="right" style="cursor: pointer; height: 25px;"
                                    alt="Close" onclick="ClosePopup()" />Query Response
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table width="100%" border="0px" align="center" style="">
                                    <tr>
                                        <td>
                                            Attachment
                                        </td>
                                        <td>
                                            <asp:FileUpload ID="fileUpload" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right">
                                            Remarks:
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtRemarks" TextMode="MultiLine" Width="300px" Height="80px" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnsubmit" runat="server" Text="Ok" OnClick="btnsubmit_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:HiddenField ID="hdnUserName" runat="server" />
        <asp:HiddenField ID="hdnReviewerName" runat="server" />
        <asp:HiddenField ID="hdnName" runat="server" />
        <asp:HiddenField ID="hdnXml" runat="server" Value="FirstTime" />
         <asp:HiddenField ID="ndnPtsAddValidate" runat="server" Value="0" />
    </div>
    </form>
</body>
</html>
