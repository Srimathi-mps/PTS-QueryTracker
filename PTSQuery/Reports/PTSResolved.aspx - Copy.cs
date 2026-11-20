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
using PTS.Classes;
using Microsoft.Reporting.WebForms;

namespace PTS
{
    public partial class PTSResolved : System.Web.UI.Page
    {
        public string ReportURL { get; set; }
        public string ReportPath { get; set; }
        public string Report { get; set; }

        IPTSReports ptsReportService = new PTSReportsService();

        protected void Page_Load(object sender, EventArgs e)
        {
			   
			 MessageLabel.Text = "OK Sathish";
            string stg = Request.QueryString["stage"];
            string stgname = Request.QueryString["stagename"];
            //string stg = "36";
            //string stgname = "Precheck";
			 //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Signal Created Successfully')", true);
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
                MessageLabel.Text = ex.Message+"Ok Techno";
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
            try
            {
                ReportURL = System.Configuration.ConfigurationManager.AppSettings["ReportURL"].ToString();
                ReportPath = System.Configuration.ConfigurationManager.AppSettings["ReportPath"].ToString();
                Report = "PTSResolvedReport";
                rptPTSResolved.Visible = true;
				IReportServerCredentials ssrscredentials = new CustomSSRSCredentials("iuser", "BMW_i20^Sx4", "macmill");
                rptPTSResolved.ServerReport.ReportServerCredentials = ssrscredentials;
                DataTable dtValidationQueries = new DataTable();
                string Customer = Request.QueryString["Customer"];

                string fromDate = string.Empty;
                string toDate = string.Empty;
                string filter=string.Empty;
                if(txtFilter.Text.ToString().Trim()!=string.Empty)
                filter =txtFilter.Text;
                else
                    filter=null;
                if (txtFromDate.Text != string.Empty)
                    fromDate = txtFromDate.Text.Split('-')[1] + "/" + txtFromDate.Text.Split('-')[0] + "/" + txtFromDate.Text.Split('-')[2];

                if (txtToDate.Text != string.Empty)
                    toDate = txtToDate.Text.Split('-')[1] + "/" + txtToDate.Text.Split('-')[0] + "/" + txtToDate.Text.Split('-')[2];
                
                rptPTSResolved.ShowCredentialPrompts = false;
                rptPTSResolved.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;

                rptPTSResolved.ServerReport.ReportServerUrl = new System.Uri(ReportURL);
                rptPTSResolved.ServerReport.ReportPath = ReportPath + Report;

                rptPTSResolved.ShowParameterPrompts = false;

                if (userCode == "")
                {
                    userCode = "0";
                }
                if (txtFromDate.Text == "" && txtToDate.Text == "")
                {
                    fromDate = null;
                    toDate = null;
                }

                ReportParameter[] parm = new ReportParameter[6];
                parm[0] = new ReportParameter("stage", stage);
                parm[1] = new ReportParameter("user", userCode);
                parm[2] = new ReportParameter("fDate", fromDate);
                parm[3] = new ReportParameter("tDate", toDate);
                parm[4] = new ReportParameter("Customer", Customer);
                parm[5] = new ReportParameter("filter",filter );


                rptPTSResolved.ServerReport.SetParameters(parm);

                rptPTSResolved.ServerReport.Refresh();
           
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message+"Ok TechnoDisplay Report";
            }
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
    }
}
