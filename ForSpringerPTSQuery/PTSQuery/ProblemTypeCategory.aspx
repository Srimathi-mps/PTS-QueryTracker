<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProblemTypeCategory.aspx.cs" Inherits="PTS.ProblemTypeCategory" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>

<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>


<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolKit" %>--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Problem Type</title>
    <link href="Stylesheet/Stylesheet.css" type="text/css" rel="Stylesheet" />

    <script type="text/javascript" language="javascript">
    
    
        function HideModalPopup() {
         
            $find("mdpAddProblemType").hide();
           //return false;
           
           return true;
            }
         

            function Validation() {


                var code = document.form1.txtCode.value;
                var StageId = document.form1.ddlSubStage.options[document.form1.ddlSubStage.selectedIndex].value;
                var Customer = document.form1.ddlCustomer.options[document.form1.ddlCustomer.selectedIndex].value;
                var Department = document.form1.ddlDepartment.options[document.form1.ddlDepartment.selectedIndex].value;
                var description = document.form1.txtDescription.value;
                
              var Pt=document.form1.ProblemTypeDropDown.options[document.form1.ProblemTypeDropDown.selectedIndex].value;
               
                 if (Customer == 0) {
                window.alert("Please select the Customer");
                return false;

                }
                else if (StageId == 0) {
                    window.alert("Please select the Sub Stage");
                    return false;

                }
                else if (Department == 0 || Department == "0") {
                    window.alert("Please select the department");
                    return false;

                }
                 else if  (Pt == 0) {
                window.alert("Please select the ProblemType");
                return false;

                }
                 else if (code == 0) {
                window.alert("Please Enter  the Problem Type Category Code");
                return false;

                }
                else if (description == "") {

                    window.alert("Please enter  Description");
                    return false;
                }
                }
            
        
    </script>
    
    <style  type="text/css">
        
        .panelHeader
         {
             background-color: #f5f5f5;
             border-width: 2px;
             border-style: solid;
             border-color: #2B547E;
             padding: 0px;
             vertical-align: top;
         }
         .PanelText
         {
             font-family: Verdana,Arial;
             font-size: 13px;
             font-weight: bold;
         }
         .LabelText
         {
             font-family: Verdana,Arial;
             font-size: 11px;
             font-weight: normal;
         }
         .scroll_Grid
         {
             width: 99%;
             padding: 2px;
             overflow: auto;
             background: white;
             border: 1px solid #ccc;
         }
        
   .modalbackground1 
{
    background-color:Gray;
    opacity: 0.5;
    filter:Alpha(opacity=50);
}
    </style>

</head>
<body>
    <form id="form1" runat="server">
    
    <%--<AjaxToolKit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </AjaxToolKit:ToolkitScriptManager>--%>
    
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    
    <div>
        

        
        
        <asp:Button ID="btnAddNew" runat="server" Text="AddNew" />
        <AjaxToolKit:ModalPopupExtender ID="mdpAddProblemType" runat="server"
        BackgroundCssClass="modalbackground1" 
        TargetControlID="btnAddNew" PopupControlID="pnlProblemType" X="200" Y="50"
        >
        </AjaxToolKit:ModalPopupExtender>
         <hr />
        
        
        <asp:Panel ID="pnlProblemType" Style="display: none"  CssClass="panelHeader" runat="server">
            <table>
                <tr style="background-color: #2B547E">
                    <td colspan="6" align="center" style="color: white; font-weight: bold; font-size: 15px">
                        <img align="right" style="cursor: pointer" alt="Close" onclick="HideModalPopup()"
                            src="Images/close1.jpg" />Add New Details
                    </td>
                </tr>
                <tr>
                    <td class="PTSLabel">
                        Customer<font color="red">*</font>
                    </td>
                    <td>
                         <asp:DropDownList ID="ddlCustomer" runat="server" Width="180px" 
                             onselectedindexchanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                      <td class="PTSLabel">
                        Sub Stage<font color="red">*</font>
                    </td>
                    <td>
                         <asp:DropDownList ID="ddlSubStage" runat="server" Width="175px">
                        </asp:DropDownList>
                    </td>
                   
                    
                </tr>
                 <tr>
                    <td colspan="5">
                    </td>
                </tr>
                <tr>
                  
                    
                    <td class="PTSLabel">
                        Department<font color="red">*</font>
                    </td>
                    <td>
                       <asp:DropDownList ID="ddlDepartment" runat="server" Width="175px" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged" >
                        </asp:DropDownList>
                    </td>
                      <td class="PTSLabel">
                    ProblemType<font color="red">*</font>
                    </td> 
                    <td>
                    <asp:DropDownList ID="ProblemTypeDropDown" runat="server" Width="175px" >
                    </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                    </td>
                </tr>
                <tr>
                   
                </tr>
                 
               
                
                <tr>
                <td class="PTSLabel">
                        ProblemType Category<font color="red">*</font>
                    </td>
                    <td>
                       <asp:TextBox ID="txtCode" runat="server" class="PTSTextbox" Width="170px"></asp:TextBox>
                    </td>
                    </tr>
                    <tr>
                    <td class="PTSLabel">
                        Description<font color="red">*</font>
                    </td>
                    <td colspan="4">
                        <asp:TextBox ID="txtDescription" runat="server" Width="500" Height="60px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                    </td>
                </tr>
               
                
                <tr>
                    <td colspan="5" align="center">
                        <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" OnClientClick="return Validation();" />&nbsp; &nbsp;&nbsp;
                        <asp:Button ID="btnReset" runat="server" Text="Reset" 
                            onclick="btnReset_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        
        
    </div>
    <div>
        <asp:GridView ID="GVProblemType" runat="server" class="LabelText" AutoGenerateColumns="False"
            AllowPaging="True" PageSize="15" EmptyDataText="No Records Found!!!" 
             HeaderStyle-BackColor="#284775" HeaderStyle-Height="25px" DataKeyNames="ProblemTypeCategoryID" 
            HeaderStyle-ForeColor="White" EmptyDataRowStyle-BackColor="AliceBlue" 
            onrowdatabound="GVProblemType_RowDataBound" 
            onpageindexchanging="GVProblemType_PageIndexChanging" Width="1071px">
            <EmptyDataRowStyle BackColor="AliceBlue" />
            <Columns>
            
             <asp:TemplateField ItemStyle-Width="100px">
                        <ItemTemplate>
                            <asp:LinkButton ID="grdLinkDelete" CausesValidation="False" runat="server" OnClientClick="return confirm('Do you want to delete?');"
                                 Text="Delete" onclick="grdLinkDelete_Click" ></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="25px"></ItemStyle>
                        </asp:TemplateField>
        
     <asp:BoundField HeaderText="Customer" DataField="Cust_sname" />
       <asp:BoundField HeaderText="Stage" DataField="SubStageDescription" />
        <asp:BoundField HeaderText="Department" DataField="DepartmentName" />
        <asp:BoundField HeaderText="Problem Type" DataField="PoblemType" />
                <asp:BoundField HeaderText="Problem Type Category" DataField="ProblemTypeCategory" />
                 <asp:BoundField HeaderText="Description" DataField="Description" />
             
                
               
                
            </Columns>
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" VerticalAlign="Middle"
                Font-Bold="true" />
          <HeaderStyle BackColor="#284775" ForeColor="White" Height="25px"></HeaderStyle>
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
