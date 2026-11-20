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
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;


using Microsoft.Reporting.WebForms;

namespace PTS
{
    public partial class PTSResolved : System.Web.UI.Page
    {
        public string ReportURL { get; set; }
        public string ReportPath { get; set; }
        public string Report { get; set; }
        string QueryType = "";
        IPTSQuery ptsQueryService = new PTSQueryService();
        PTSProblem objPTSProblem = new PTSProblem();
        IPTSReports ptsReportService = new PTSReportsService();
        PTSQueryService objPTSQueryService = new PTSQueryService();

        protected void Page_Load(object sender, EventArgs e)
        {
            string stg = Request.QueryString["stage"];
            string stgname = Request.QueryString["stagename"];

           QueryType = Request.QueryString["QueryType"];
            try
            {
                if (!IsPostBack)
                {
                    ReportTitleLabel.Text = "Queries Resolved (Archive) - " + stgname;
                    displayReport(stg, "");
                    displayUesrs();
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        #region Display Methods
        protected void displayUesrs()
        {
            DataTable dtAllPTSUsers = new DataTable();
            dtAllPTSUsers = ptsReportService.GetAllPTSUsers();

            ddlUsers.DataSource = dtAllPTSUsers;
            ddlUsers.DataTextField = "UserName";
            ddlUsers.DataValueField = "UserCode";
            ddlUsers.DataBind();

            ddlUsers.Items.Insert(0, "All Users");
        }

        protected void displayReport(string stage, string userCode)
        {
            #region ssrs
            //try
            //{
            //    ReportURL = System.Configuration.ConfigurationManager.AppSettings["ReportURL"].ToString();
            //    ReportPath = System.Configuration.ConfigurationManager.AppSettings["ReportPath"].ToString();
            //    Report = "PTSResolvedReport";
            //    rptPTSResolved.Visible = true;

            //    DataTable dtValidationQueries = new DataTable();
            //    string Customer = Request.QueryString["Customer"];

            //    string fromDate = string.Empty;
            //    string toDate = string.Empty;
            //    if (txtFromDate.Text != string.Empty)
            //        fromDate = txtFromDate.Text.Split('-')[1] + "/" + txtFromDate.Text.Split('-')[0] + "/" + txtFromDate.Text.Split('-')[2];

            //    if (txtToDate.Text != string.Empty)
            //        toDate = txtToDate.Text.Split('-')[1] + "/" + txtToDate.Text.Split('-')[0] + "/" + txtToDate.Text.Split('-')[2];

            //    rptPTSResolved.ShowCredentialPrompts = false;
            //    rptPTSResolved.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

            //    rptPTSResolved.ServerReport.ReportServerUrl = new System.Uri(ReportURL);
            //    rptPTSResolved.ServerReport.ReportPath = ReportPath + Report;

            //    rptPTSResolved.ShowParameterPrompts = false;

            //    if (userCode == "")
            //    {
            //        userCode = "0";
            //    }
            //    if (txtFromDate.Text == "" && txtToDate.Text == "")
            //    {
            //        fromDate = null;
            //        toDate = null;
            //    }

            //    ReportParameter[] parm = new ReportParameter[5];
            //    parm[0] = new ReportParameter("stage", stage);
            //    parm[1] = new ReportParameter("user", userCode);
            //    parm[2] = new ReportParameter("fDate", fromDate);
            //    parm[3] = new ReportParameter("tDate", toDate);
            //    parm[4] = new ReportParameter("Customer", Customer);


            //    rptPTSResolved.ServerReport.SetParameters(parm);

            //    rptPTSResolved.ServerReport.Refresh();
           
            //}
            //catch (Exception ex)
            //{
            //    MessageLabel.Text = ex.Message;
            //}
            #endregion ssrs

            DataTable dtValidationQueries = new DataTable();
            string Customer = Request.QueryString["Customer"];

            string fromDate = string.Empty;
            string toDate = string.Empty;
            if (txtFromDate.Text != string.Empty)
                fromDate = txtFromDate.Text.Split('-')[1] + "/" + txtFromDate.Text.Split('-')[0] + "/" + txtFromDate.Text.Split('-')[2];

            if (txtToDate.Text != "")
                toDate = txtToDate.Text.Split('-')[1] + "/" + txtToDate.Text.Split('-')[0] + "/" + txtToDate.Text.Split('-')[2];

            if (userCode == "")
            {
                userCode = "0";
            }
            if (txtFromDate.Text == "" && txtToDate.Text == "")
            {
                fromDate = null;
                toDate = null;
            }

            PTSProblem objPTSProblem = new PTSProblem();
            objPTSProblem.StageID = Convert.ToInt32(stage.Trim());
            objPTSProblem.UserName = userCode;
            if (fromDate != null)
                objPTSProblem.FromDate = fromDate.Trim();
            else
                objPTSProblem.FromDate = null;
            if (toDate != null)
                objPTSProblem.ToDate = toDate.Trim();
            else
                objPTSProblem.ToDate = null;

            objPTSProblem.QueryType = QueryType;
            objPTSProblem.CustomerCode = Convert.ToInt32(Customer.Trim());
            DataTable dtPTSResolved = new DataTable();
            dtPTSResolved = objPTSQueryService.GetPTSResolved(objPTSProblem);
            gvPendingReport.DataSource = dtPTSResolved;
            gvPendingReport.DataBind();
            dgExcel.DataSource = dtPTSResolved;
            dgExcel.DataBind();
        }
        #endregion

        #region Controls Events
        protected void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ddlUsers.SelectedIndex != 0)
                {
                    displayReport(Request.QueryString["stage"], ddlUsers.SelectedValue.ToString().Trim());
                }
                else
                {
                    displayReport(Request.QueryString["stage"], "");
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlUsers.SelectedIndex != 0)
                {
                    displayReport(Request.QueryString["stage"], ddlUsers.SelectedValue.ToString().Trim());
                }
                else
                {
                    displayReport(Request.QueryString["stage"], "");
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }
        #endregion

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                string stg = Request.QueryString["stage"];
                ExportTable.Visible = true;
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.Charset = "";
                this.EnableViewState = false;
                System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
                displayReport(stg, "");

                ExportTable.RenderControl(oHtmlTextWriter);
                Response.Write(oStringWriter.ToString());
                Response.AddHeader("content-disposition", "attachment; filename=PendingReport.xls");
                ExportTable.Visible = false;
                Response.End();

            }
            catch (Exception ex)
            {
                //bool rethrow = false;
                //rethrow = UserInterfaceExceptionHandler.HandleExcetion(ref ex);
                //ShowMessage(Global.RemoveSpecialChars(ex.Message));
            }
        }

        
    }
}
