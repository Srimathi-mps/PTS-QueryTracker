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
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using QueryTracker;
using System.IO;

namespace QueryTracker
{
    public partial class ProblemType : System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();
        PTSProblem Obj = new PTSProblem();
        //protected string userName = "";//Global.Validate();
        protected string userName = "MAC006081";
        int count = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // Session["userName"] = "MAC006081";
                if (Session["userName"] == null)
                {
                    //userName = Global.Validate(); 
                    WriteLog("No session : " + userName); Session["userName"] = userName; }
                else { userName = Session["userName"].ToString(); WriteLog("session" + userName); }
                LoadProblemTypes();
                // FillStageDDL();
                FillCustomerDDL();
                FillDepartmentDDL();
            }
        }
        public void WriteLog(string textMessage)
        {
            // WriteLogTest(textMessage);
            string logPath = string.Empty;
            //if (appsSettings.ContainsKey("LogPath"))
            //{
            //    logPath = appsSettings["LogPath"];
            //}
            //  WriteLogTest(logPath);

            logPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"].ToString();
            if (Directory.Exists(logPath))
            {
                //nothing 
            }
            else
            {
                Directory.CreateDirectory(logPath);
            }

            string fileName = logPath + "\\Query_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".html";
            StreamWriter sw = new StreamWriter(fileName, true);
            if (textMessage != string.Empty)
            {
                sw.WriteLine("<html><Table border=0 width='80%'><TR><TD width='80%'>" + textMessage + "</TD><TD>  " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt") + "</TD></TR>");
            }
            else
            {
                sw.WriteLine("<html><Table border=0 width='80%'><TR><TD width='100%' style='height:15px;'><HR/></TD></TR>");
            }
            sw.Flush();
            sw.Close();
        }
        public void LoadProblemTypes()
        {
            DataTable dt = new DataTable();
            dt = ptsQueryService.GetProblemTypeDetails(userName);
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

        }

        private void FillStageDDL()
        {
            DataTable dtStages = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();
            ptsQuery.CustomerCode = Convert.ToInt32(ddlCustomer.SelectedValue.ToString());
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
            txtLongDescription.Text = "";
            txtShortDescription.Text = "";
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
            Obj.Code = txtCode.Text;
            Obj.Cust_code = Convert.ToInt32(ddlCustomer.SelectedValue);
            Obj.SubStageId = Convert.ToInt32(ddlSubStage.SelectedValue);
            Obj.DepartmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
            Obj.Long_Desc = txtLongDescription.Text;
            Obj.Short_Desc = txtShortDescription.Text;
            Obj.CreatedBy = userName;
            int result = 0;
            result = ptsQueryService.InsertProblemType(Obj);
            if (result >= 1)
            {
                ClearControl();
                LoadProblemTypes();
                ShowMessage("Record saved successfully!!");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Obj.Code = txtCode.Text;
                Obj.Cust_code = Convert.ToInt32(ddlCustomer.SelectedValue);
                Obj.SubStageId = Convert.ToInt32(ddlSubStage.SelectedValue);

                DataTable dtExistRecord = new DataTable();
                dtExistRecord = ptsQueryService.CheckExistProblemType(Obj);
                if (dtExistRecord.Rows.Count > 0)
                {
                    //ClearControl();
                    ShowMessage("Record Exist!!!");
                }
                else
                {

                    InsertRecord();
                }
            }
            catch (Exception ex)
            {
              
            }


        }

        protected void GVProblemType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {



                DataTable dt = new DataTable();
                dt = ptsQueryService.GetProblemTypeDetails(Session["userName"].ToString());

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    GridViewRow s = (GridViewRow)e.Row;
                    e.Row.Attributes.Add("onmouseover", "this.originalcolor=this.style.backgroundColor;" + " this.style.backgroundColor='#A9A9A9';");

                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=this.originalcolor;");
                    int roleID = ptsQueryService.GetRoleIDByUserName(Session["userName"].ToString(), dt.Rows[count]["Cust_code"].ToString());
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
            result = ptsQueryService.DeleteProblemType(Obj);
            if (result == 1)
            {
                ClearControl();
                LoadProblemTypes();
                ShowMessage("Record deleted successfully!!");
            }
        }

        protected void GVProblemType_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVProblemType.PageIndex = e.NewPageIndex;
            LoadProblemTypes();
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                mdpAddProblemType.Show();
                WriteLog("Customer Bind : " + Session["userName"].ToString());
                int roleID = ptsQueryService.GetRoleIDByUserName(Session["userName"].ToString(), ddlCustomer.SelectedValue);
                bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.ProblemTypeEntry);
                if (allowSave)
                    btnSave.Enabled = true;
                else
                    btnSave.Enabled = false;

                FillStageDDL();
            }
            catch (Exception ex)
            {
                
            }
        }



    }
}
