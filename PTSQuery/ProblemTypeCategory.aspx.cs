using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.Sql;
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using MPS_ManuscriptTracking.PTSBusinessLayer;


namespace PTS
{
    public partial class ProblemTypeCategory : System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();
        PTSProblem Obj = new PTSProblem();
        protected string userName = Global.Validate(); //Changed By sathish

        //protected string userName = "MAC000289";
        int count = 0;
        
        protected void Page_Load(object sender, EventArgs e)
        {

          
            //int i = int.TryParse(ddlSubStage.SelectedValue.ToString().Trim(), out stageID);
            //int j = int.TryParse(ddlDepartment.SelectedValue.ToString().Trim(), out DeptID);
            if (!IsPostBack)
            {
                LoadProblemTypeCategory();
                FillStageDDL();
                FillCustomerDDL();
                FillDepartmentDDL();
            }
        }
        public void LoadProblemTypeCategory()
        {
            DataTable dt = new DataTable();
            dt = ptsQueryService.GetProblemTypeCategoryDetails(userName);
            GVProblemType.DataSource = dt;
            GVProblemType.DataBind();
           
        }

        private void FillCustomerDDL()
        {
            DataTable dtcustomer = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtcustomer = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Customer, ptsQuery, userName);

            ddlCustomer.DataSource = dtcustomer;
            ddlCustomer.DataTextField = Global.DB_FIELD_NAME;
            ddlCustomer.DataValueField = Global.DB_FIELD_ID;
            ddlCustomer.DataBind();
            ddlCustomer.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
            ProblemTypeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

        }

        private void FillDepartmentDDL()
        {
            DataTable dtDepartment = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtDepartment = ptsQueryService.GetDepartment();

            ddlDepartment.DataSource = dtDepartment;
            ddlDepartment.DataTextField = Global.DB_FIELD_NAME;
            ddlDepartment.DataValueField = Global.DB_FIELD_ID;
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
            //mdpAddProblemType.Show();
        }

        private void FillStageDDL()
        {
            DataTable dtStages = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtStages = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Stage, ptsQuery, "");

            ddlSubStage.DataSource = dtStages;
            ddlSubStage.DataTextField = Global.DB_FIELD_NAME;
            ddlSubStage.DataValueField = Global.DB_FIELD_ID;
            ddlSubStage.DataBind();
            ddlSubStage.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }

        public void ClearControl()
        {
            ddlCustomer.SelectedIndex = 0;
            ddlDepartment.SelectedIndex = 0;
           txtCode.Text = "";
            ddlSubStage.SelectedIndex = 0;
            ProblemTypeDropDown.SelectedIndex = 0;
            txtDescription.Text = "";
           
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ClearControl();
            mdpAddProblemType.Show();
        }

        private void ShowMessage(string Message)
        {
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "alert", string.Format("alert('{0}');", Message), true);
        }


        public void InsertRecord()
        {
            Obj.ProblemTypeID = Convert.ToInt32(ProblemTypeDropDown.SelectedValue);
            Obj.Cust_code = Convert.ToInt32(ddlCustomer.SelectedValue);
            
            Obj.SubStageId = Convert.ToInt32(ddlSubStage.SelectedValue);
            Obj.DepartmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
            Obj.Long_Desc = txtDescription.Text;
           // Obj.Short_Desc = txtCode.Text;
               Obj.Code = txtCode.Text;

            Obj.CreatedBy = userName;
            int result = 0;
            result = ptsQueryService.InsertProblemTypeCategory(Obj);
            if (result == 1)
            {
                ClearControl();
                LoadProblemTypeCategory();
                ShowMessage("Record saved successfully!!");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
               Obj.Code = txtCode.Text;
               // Obj.Cust_code = Convert.ToInt32(ddlCustomer.SelectedValue);
               Obj.Cust_code = Convert.ToInt32(ProblemTypeDropDown.SelectedValue);
                Obj.SubStageId = Convert.ToInt32(ddlSubStage.SelectedValue);
              //  Obj.Short_Desc = Convert.ToInt32();
                DataTable dtExistRecord = new DataTable();
                dtExistRecord = ptsQueryService.CheckExistProblemTypeCategory(Obj);
                if (dtExistRecord.Rows.Count > 0)
                {
                    ClearControl();
                    ShowMessage("Record Exist!!!");
                }
                else
                {

                    InsertRecord();
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.ToString());
            }


        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillProblemTypeDDL();
            mdpAddProblemType.Show();
           
        }

        protected void GVProblemType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {



                DataTable dt = new DataTable();
                dt = ptsQueryService.GetProblemTypeCategoryDetails(userName);

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    GridViewRow s = (GridViewRow)e.Row;
                    e.Row.Attributes.Add("onmouseover", "this.originalcolor=this.style.backgroundColor;" + " this.style.backgroundColor='#A9A9A9';");

                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalcolor;");
                    int roleID = ptsQueryService.GetRoleIDByUserName(userName, dt.Rows[count]["Cust_code"].ToString());
                    count++;
                    bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.ProblemTypeDeletion);

                    if (allowSave)
                    {
                        LinkButton btn1 = (LinkButton)e.Row.FindControl("grdLinkDelete");
                        btn1.Visible = true;

                    }
                    else
                    {
                        LinkButton btn1 = (LinkButton)e.Row.FindControl("grdLinkDelete");
                        btn1.Visible = false;

                    }


                }




            }
            catch (Exception ex)
            {
                bool rethrow = false;

            }
        }

       


        protected void grdLinkDelete_Click(object sender, EventArgs e)
        {
            int Id = 0;
            LinkButton btnDelete = (LinkButton)sender;
            GridViewRow objGvRow = (GridViewRow)btnDelete.NamingContainer;
            Id = Convert.ToInt32(GVProblemType.DataKeys[objGvRow.RowIndex].Value);
            Obj.ProblemTypeID = Id;
            int result = 0;
            result = ptsQueryService.DeleteProblemTypeCategory(Obj);
            if (result == 1)
            {
                ClearControl();

                LoadProblemTypeCategory();
                ShowMessage("Record deleted successfully!!");
            }
        }

        protected void GVProblemType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVProblemType.PageIndex = e.NewPageIndex;
            LoadProblemTypeCategory();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                mdpAddProblemType.Show();
                int roleID = ptsQueryService.GetRoleIDByUserName(userName, ddlCustomer.SelectedValue);
                bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.ProblemTypeEntry);
                if (allowSave)
                    btnSave.Enabled = true;
                else
                    btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        private void FillProblemTypeDDL()
        {
            DataTable dtProblemType = new DataTable();
            int DeptId = Convert.ToInt32(ddlDepartment.SelectedValue);
            int SubStageID = Convert.ToInt32(ddlSubStage.SelectedValue);
            int CustomerId = Convert.ToInt32(ddlCustomer.SelectedValue);
            dtProblemType = ptsQueryService.GetProblemTypesByStage(CustomerId,DeptId, SubStageID);

            ProblemTypeDropDown.DataSource = dtProblemType;
            ProblemTypeDropDown.DataValueField = Global.DB_FIELD_ID.Trim();
            ProblemTypeDropDown.DataTextField = Global.DB_FIELD_NAME.Trim();
            ProblemTypeDropDown.DataBind();
            ProblemTypeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

        }
        
       
    }

   
  

}
