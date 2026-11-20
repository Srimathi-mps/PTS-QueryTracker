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
    public partial class PTSHistory : System.Web.UI.Page
    {
        IPTSReports ptsReportsService = new PTSReportsService();

        protected void Page_Load(object sender, EventArgs e)
        {
            //string stg = Request.QueryString["stage"];

            string stg = "1";
            string stgname = Request.QueryString["stagename"];


            try
            {
                if (!IsPostBack)
                {
                    ReportTitleLabel.Text = "Reply Received report for - " + stgname;
                    displayReport(stg, "");
                    displayUesrs();
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;                
            }
        }

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

        #region Display Methods
        protected void displayUesrs()
        {
            DataTable dtAllPTSUsers = new DataTable();
            dtAllPTSUsers = ptsReportsService.GetAllPTSUsers();

            ddlUsers.DataSource = dtAllPTSUsers;
            ddlUsers.DataTextField = "UserName";
            ddlUsers.DataValueField = "UserCode";
            ddlUsers.DataBind();

            ddlUsers.Items.Insert(0, "All Users");
        }

        protected void displayReport(string stage, string userCode)
        {
            rptPTSHistory.Visible = true;

            DataTable dtValidationQueries = new DataTable();
            string Customer = Request.QueryString["Customer"];


            string fDT = "";
            string tDT = "";
            if (txtFromDate.Text != "")
                fDT = txtFromDate.Text.Split('-')[1] + "/" + txtFromDate.Text.Split('-')[0] + "/" + txtFromDate.Text.Split('-')[2];

            if (txtToDate.Text != "")
                tDT = txtToDate.Text.Split('-')[1] + "/" + txtToDate.Text.Split('-')[0] + "/" + txtToDate.Text.Split('-')[2];

            dtValidationQueries = ptsReportsService.GetPTSHistoryReportByCustomer(stage, userCode, fDT, tDT, Customer);

            if (dtValidationQueries.Rows.Count > 0)
            {
                rptPTSHistory.Visible = true;
                txtMsg.Visible = false;
                ReportDataSource datasource = new ReportDataSource("dsPTSHistory_usp_PTS_GetPTSHistoryReport", dtValidationQueries);

                rptPTSHistory.LocalReport.DataSources.Clear();
                rptPTSHistory.LocalReport.DataSources.Add(datasource);

                rptPTSHistory.LocalReport.Refresh();
            }
            else
            {
                rptPTSHistory.Visible = false;
                txtMsg.Visible = true;
                txtMsg.Text = "No records found for the selected search creteria..";
            }
        }
        #endregion
    }
}
