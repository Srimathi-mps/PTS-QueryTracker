using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using MPS_ManuscriptTracking.PTSBusinessLayer;
//using MPS_ManuscriptTracking.PTSDataAccessLayer;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.Mail;
using QueryTracker;
using QueryTrackerSetUp;
using System.Configuration;
using System.Data.SqlClient;

namespace QueryTracker.Reports
{
    public partial class QueryReport : System.Web.UI.Page
    {
        static string ptsConstr = ConfigurationManager.AppSettings["jetsConstr"].ToString().Trim();
        
        

        protected void Page_Load(object sender, EventArgs e)
        {
            
            try
            {
                if (!Page.IsPostBack)
                {
                   // int Customer = Convert.ToInt32(Request.QueryString["Customer"]);
                   // if (Request.QueryString["Customer"] == "32")
                   // {
                        BindQueries();
                   // }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void BindQueries()
        {
            DataTable resultTable = new DataTable();
            SqlConnection sqlcon = new SqlConnection(ptsConstr);
            try
            {
                SqlCommand sqlcmd = new SqlCommand("usp_PTS_GetQueries", sqlcon);
                if (sqlcon.State == ConnectionState.Closed)
                { sqlcon.Open(); }

                SqlDataAdapter adp = new SqlDataAdapter(sqlcmd);
                adp.Fill(resultTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlcon.State == ConnectionState.Open)
                { sqlcon.Close(); }
                sqlcon = null;
            }
            GVDueArticles.DataSource = resultTable;
            GVDueArticles.DataBind();
        }
    }
}
