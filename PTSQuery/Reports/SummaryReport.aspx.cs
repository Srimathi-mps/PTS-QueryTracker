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
using QueryTracker;
using System.Data.SqlClient;
using System.IO;

namespace PTS.Reports
{
    public partial class SummaryReport : System.Web.UI.Page
    {
        IPTSReports ptsReportService = new PTSReportsService();
       // Global.UserName = "MAC004342";
      //  protected string userName = "";
      protected string userName = "test";
    // protected string userName = "CB1790";

        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //if (Session["userName"] != null)
                    userName = Session["userName"].ToString();
                // else
                // userName = "mac004483";
                WriteLog("UserName-" + userName);
                if (!IsPostBack)
                {
                    FillCustomerDDL();
                    //ifrm.Attributes.Add("src", "QueryReport.aspx?Customer=" + ddlCustomer.SelectedValue + "");
                   
                }
            }
            catch (Exception ex)
            {
                //MessageLabel.Text = ex.Message;
            }
        }

        protected void ddlCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillStages();
            }
            catch (Exception ex)
            {
            }
        }
        private void FillCustomerDDL()
        {
            DataTable dtCustomer = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtCustomer = new PTSQueryService().GetPTSQueryMaster(BizPTSQuery.tName.Customer, ptsQuery, userName);

            ddlCustomer.DataSource = dtCustomer;
            ddlCustomer.DataTextField = Global.DB_FIELD_NAME;
            ddlCustomer.DataValueField = Global.DB_FIELD_ID;
            ddlCustomer.DataBind();

            ddlCustomer.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, "0"));
            if (dtCustomer.Rows.Count == 1)
            {
                ddlCustomer.SelectedIndex = 1;
                FillStages();
            }

        }

        protected void gvSummaryReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string SubStageId = ((Label)e.Row.FindControl("lblStageID")).Text;
                    string stg = ((Label)e.Row.FindControl("lblSubStageName")).Text;
                    string StageDescription = ((Label)e.Row.FindControl("lblSubStageDescription")).Text;
                    LinkButton lbDue = (LinkButton)e.Row.FindControl("lbDue");
                    if (Convert.ToInt32(lbDue.Text.Trim()) > 0)
                    {
                       // userName = Global.Validate();
                       //lbDue.PostBackUrl = "../DueForClosing.aspx?Username=" + userName + "&stage=" + SubStageId + "&stagename=" + StageDescription + "&Customer=" + ddlCustomer.SelectedValue;
                         lbDue.Attributes.Add("onclick", "OpenDueForClose('DueForClosing.aspx?Username=" + userName + " &stage=" + SubStageId + "&stagename=" + StageDescription + "&Customer=" + ddlCustomer.SelectedValue + "')");
                    }
                    LinkButton lbPending = (LinkButton)e.Row.FindControl("lbPending");
                    if (Convert.ToInt32(lbPending.Text.Trim()) > 0)
                    {
                        //lbPending.PostBackUrl = "AwaitingReplyReport.aspx?stage=" + SubStageId + "&stagename=" + stg + "&Customer=" + ddlCustomer.SelectedValue;
                        lbPending.Attributes.Add("onclick", "OpenPTSResolved('AwaitingReplyReport.aspx?Username=" + userName + " &stage=" + SubStageId + "&stagename=" + stg + "&Customer=" + ddlCustomer.SelectedValue + "&QueryType=QUERY')");
                    }
                    LinkButton lbResolved = (LinkButton)e.Row.FindControl("lbResolved");
                    if (Convert.ToInt32(lbResolved.Text.Trim()) > 0)
                    {
                        //lbResolved.PostBackUrl = "PTSResolved.aspx?stage=" + SubStageId + "&stagename=" + stg + "&Customer=" + ddlCustomer.SelectedValue; 

                        lbResolved.Attributes.Add("onclick", "OpenPTSResolved('PTSResolved.aspx?Username=" + userName + " &stage=" + SubStageId + "&stagename=" + stg + "&Customer=" + ddlCustomer.SelectedValue + "&QueryType=QUERY')");
                    }

                    //Added by Sathish.J on 16-05-2018
                    LinkButton lbInstruction = (LinkButton)e.Row.FindControl("lbInstruction");
                    if (Convert.ToInt32(lbInstruction.Text.Trim()) > 0)
                    {
                        //lbPending.PostBackUrl = "AwaitingReplyReport.aspx?stage=" + SubStageId + "&stagename=" + stg + "&Customer=" + ddlCustomer.SelectedValue;
                        lbInstruction.Attributes.Add("onclick", "OpenPTSResolved('AwaitingReplyReport.aspx?Username=" + userName + " &stage=" + SubStageId + "&stagename=" + stg + "&Customer=" + ddlCustomer.SelectedValue + "&QueryType=INSTRUCTION')");
                    }

                }
            }
            catch (Exception ex)
            {
               // MessageLabel.Text = ex.Message;
            }
        }
        public static DataTable GetAllStagesWithQueries(string CustomerCode)
        {
            DataTable dtProblems = new DataTable();
            string[] paramArr = new string[] { CustomerCode };
            SqlConnection conn = null;
            string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
            conn = new SqlConnection(constr1);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            dtProblems = DBHelper.GetDataTableFromSP("usp_PTS_GetAllStagesWithQueriesNew", paramArr, constr1);
            conn.Close();
            return dtProblems;
        }
        public static int GetStageRecordCountByCustomer(string stage, int taskClosed, int CustomerCode)
        {
            SqlConnection conn = null;
            string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
            conn = new SqlConnection(constr1);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            int recCount = 0;
            string[] paramArr = new string[] { stage, CustomerCode.ToString(), taskClosed.ToString() };
            string spStr = "";
            if (taskClosed == 1)
            {
                spStr = "usp_PTS_GetAwaitingReplyRecordCountByCustomerNew";
            }
            else
            {
                spStr = "usp_PTS_Report_RecordCountByCustomerNew";
            }

            object obj = DBHelper.GetScalarValue(spStr, paramArr, constr1);

            if (obj != null)
                recCount = int.Parse(obj.ToString());
            conn.Close();
            return recCount;
        }
        public static int GetPTSHistoryRecordCountByCustomer(string stage, string CustomerCode)
        {
            SqlConnection conn = null;
            string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
            conn = new SqlConnection(constr1);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            int recCount = 0;
            string[] paramArr = new string[] { stage, CustomerCode };

            object obj = DBHelper.GetScalarValue("usp_PTS_GetPTSHistoryRecordCountByCustomerNew", paramArr, constr1);

            if (obj != null)
                recCount = int.Parse(obj.ToString());
            conn.Close();
            return recCount;
        }
        public static int GetPTSResolvedRecordCountByCustomer(string stage, string CustomerCode)
        {
            SqlConnection conn = null;
            string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
            conn = new SqlConnection(constr1);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            int recCount = 0;
            string[] paramArr = new string[] { stage, CustomerCode };

            object obj = DBHelper.GetScalarValue("usp_PTS_GetPTSResolvedRecordCountByCustomerNew", paramArr, constr1);
            conn.Close();
            if (obj != null)
                recCount = int.Parse(obj.ToString());
           
            return recCount;
        }
        public static int GetStageRecordInstructionCountByCustomer(string stage, int taskClosed, int CustomerCode)
        {
            SqlConnection conn = null;
            string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
            conn = new SqlConnection(constr1);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            int recCount = 0;
            string[] paramArr = new string[] { stage, CustomerCode.ToString(), taskClosed.ToString() };
            string spStr = "";
            if (taskClosed == 1)
            {
                spStr = "[usp_PTS_Instruction_GetAwaitingReplyRecordCountByCustomerNew]";
            }
            else
            {
                spStr = "[usp_PTS_Instruction_RecordCountByCustomerNew]";
            }

            object obj = DBHelper.GetScalarValue(spStr, paramArr, constr1);

            if (obj != null)
                recCount = int.Parse(obj.ToString());
            conn.Close();
            return recCount;
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
        private void FillStages()
        {
           // mainTBL.Controls.Clear();
            DataTable dtAllStages = new DataTable();

           // dtAllStages = ptsReportService.GetAllStagesWithQueries(ddlCustomer.SelectedValue);
            dtAllStages =GetAllStagesWithQueries(ddlCustomer.SelectedValue);
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("SubStageName"));
            dt.Columns.Add(new DataColumn("SubStageDescription"));
            dt.Columns.Add(new DataColumn("SubStageId"));
            dt.Columns.Add(new DataColumn("Due"));
            dt.Columns.Add(new DataColumn("Pending"));
            dt.Columns.Add(new DataColumn("Received"));
            dt.Columns.Add(new DataColumn("Resolved"));
            //Added By Sathish.J on 16-May-2018
            dt.Columns.Add(new DataColumn("Instruction"));

            foreach (DataRow dr in dtAllStages.Rows)
            {
                DataRow dr1 = dt.NewRow();
                int SubStageId = Convert.ToInt32(dr["PTS_StageId"].ToString());

                dr1["SubStageName"] = dr["SubStageName"].ToString();
                dr1["SubStageDescription"] = dr["SubStageDescription"].ToString();
                dr1["SubStageId"] = dr["PTS_StageId"].ToString();

                //  string recCount = ptsReportService.GetStageRecordCountByCustomer(SubStageId.ToString(), 0, Convert.ToInt32(ddlCustomer.SelectedValue)).ToString();
                string recCount = GetStageRecordCountByCustomer(SubStageId.ToString(), 0, Convert.ToInt32(ddlCustomer.SelectedValue)).ToString();
                dr1["Due"] = recCount.ToString();

                recCount = GetStageRecordCountByCustomer(SubStageId.ToString(), 1, Convert.ToInt32(ddlCustomer.SelectedValue)).ToString();
                //recCount = ptsReportService.GetStageRecordCountByCustomer(SubStageId.ToString(), 1, Convert.ToInt32(ddlCustomer.SelectedValue)).ToString();
                dr1["Pending"] = recCount.ToString();

                recCount = GetPTSHistoryRecordCountByCustomer(SubStageId.ToString(), ddlCustomer.SelectedValue).ToString();
                //recCount = ptsReportService.GetPTSHistoryRecordCountByCustomer(SubStageId.ToString(), ddlCustomer.SelectedValue).ToString();
                dr1["Received"] = recCount.ToString();

                //recCount = ptsReportService.GetPTSResolvedRecordCountByCustomer(SubStageId.ToString(), ddlCustomer.SelectedValue).ToString();
                recCount = GetPTSResolvedRecordCountByCustomer(SubStageId.ToString(), ddlCustomer.SelectedValue).ToString();
                dr1["Resolved"] = recCount.ToString();

                //Added By Sathish.J on 16-May-2018
                recCount = GetStageRecordInstructionCountByCustomer(SubStageId.ToString(), 1, Convert.ToInt32(ddlCustomer.SelectedValue)).ToString();
                //recCount = ptsReportService.GetStageRecordInstructionCountByCustomer(SubStageId.ToString(), 1, Convert.ToInt32(ddlCustomer.SelectedValue)).ToString();
                dr1["Instruction"] = recCount.ToString();

                dt.Rows.Add(dr1);

            }
            
            gvSummaryReport.DataSource = dt;
            gvSummaryReport.DataBind();

                //TableRow rowNew = new TableRow();
                #region Commented
                //mainTBL.Controls.Add(rowNew);
                ////for (int j = 0; j < 5; j++)
                ////{

                ////    TableCell cellNew = new TableCell();

                ////    Label lblNew = new Label();

                ////    #region LinkLabel & Label objects
                ////    Label lblCol1 = new Label();
                ////    LinkButton lbCol1 = new LinkButton();

                ////    Label lblCol2 = new Label();
                ////    LinkButton lbCol2 = new LinkButton();

                ////    Label lblCol3 = new Label();
                ////    LinkButton lbCol3 = new LinkButton();

                ////    Label lblCol4 = new Label();
                ////    LinkButton lbCol4 = new LinkButton();
                ////    #endregion
                ////    if (j == 0)
                ////    {
                ////        #region Col-I - Stages
                ////        cellNew.Width = 120;
                ////        lblNew.Text = dr["SubStageName"].ToString();

                ////        cellNew.Controls.Add(lblNew);
                ////        #endregion
                ////    }
                ////    else if (j == 1)
                ////    {
                ////        #region Col-II
                ////        cellNew.Width = 100;
                ////        string stg = dr["SubStageName"].ToString();
                        
                ////        //stg = stg.Replace(" ", string.Empty);
                ////        string recCount = ptsReportService.GetStageRecordCount(SubStageId.ToString(), 0).ToString();

                ////        if (recCount != "0" && recCount != "")
                ////        {
                ////            lbCol1.Text = recCount;
                ////            //lbCol1.PostBackUrl = "dueforclosing.aspx?stage=" + stg;
                ////            userName = Global.GetUserIdstring();
                ////            //userName = "CB1790";
                ////            lbCol1.PostBackUrl = "../DueForClosing.aspx?Username=" + userName + "&stage=" + SubStageId + "&stagename=" + stg;
                ////            //lbCol1.PostBackUrl = "../DueForClosing.aspx?Username=" + userName + "&stage=" + SubStageId;
                ////            cellNew.Controls.Add(lbCol1);
                ////        }
                ////        else
                ////        {
                ////            lblCol1.Text = "0";
                ////            cellNew.Controls.Add(lblCol1);
                ////        }
                ////        #endregion
                ////    }
                ////    else if (j == 2)
                ////    {
                ////        #region Col-III
                ////        cellNew.Width = 100;

                ////        string stg = dr["SubStageName"].ToString();
                ////        stg = stg.Replace(" ", string.Empty);
                ////        string recCount = ptsReportService.GetStageRecordCount(SubStageId.ToString(), 1).ToString();

                ////        if (recCount != "0" && recCount != "")
                ////        {
                ////            lbCol2.Text = recCount;
                ////            lbCol2.PostBackUrl = "AwaitingReplyReport.aspx?stage=" + SubStageId + "&stagename=" + stg;
                ////            cellNew.Controls.Add(lbCol2);
                ////        }
                ////        else
                ////        {
                ////            lblCol2.Text = "0";
                ////            cellNew.Controls.Add(lblCol2);
                ////        }
                ////        #endregion
                ////    }
                ////    else if (j == 3)
                ////    {
                ////        #region Col-IV
                ////        cellNew.Width = 100;

                ////        string stg = dr["SubStageName"].ToString();
                ////        stg = stg.Replace(" ", string.Empty);
                ////        string recCount = ptsReportService.GetPTSHistoryRecordCount(SubStageId.ToString()).ToString();

                ////        if (recCount != "0" && recCount != "")
                ////        {
                ////            lbCol3.Text = recCount;
                ////            lbCol3.PostBackUrl = "PTSHistory.aspx?stage=" + SubStageId + "&stagename=" + stg; 
                ////            cellNew.Controls.Add(lbCol3);
                ////        }
                ////        else
                ////        {
                ////            lblCol3.Text = "0";
                ////            cellNew.Controls.Add(lblCol3);
                ////        }
                ////        #endregion
                ////    }
                ////    else if (j == 4)
                ////    {
                ////        #region Col-V
                ////        cellNew.Width = 100;

                ////        string stg = dr["SubStageName"].ToString();
                ////        stg = stg.Replace(" ", string.Empty);
                ////        string recCount = ptsReportService.GetPTSResolvedRecordCount(SubStageId.ToString()).ToString();

                ////        if (recCount != "0" && recCount != "")
                ////        {
                ////            lbCol4.Text = recCount;
                ////            lbCol4.PostBackUrl = "PTSResolved.aspx?stage=" + SubStageId + "&stagename=" + stg; ;
                ////            cellNew.Controls.Add(lbCol4);
                ////        }
                ////        else
                ////        {
                ////            lblCol4.Text = "0";
                ////            cellNew.Controls.Add(lblCol4);
                ////        }
                ////        #endregion
                ////    }

                ////    rowNew.Controls.Add(cellNew);
                ////}
                //i++;
                #endregion


        }

        protected void btngoto_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/RaiseProblems.aspx");
        }

        

    }

}
