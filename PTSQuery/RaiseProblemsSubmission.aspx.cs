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
using System.IO;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web.Mail;
using System.Net;
using System.Text;
using System.Net.Mail;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using QueryTrackerSetUp;
using System.Data.SqlClient;
//using MPS_ManuscriptTracking.BookDataAccessLayer;

namespace QueryTracker
{
    public partial class RaiseProblemsSubmission : System.Web.UI.Page
    {
        #region Fields
        IPTSQuery ptsQueryService = new PTSQueryService();
        BizPTSQuery objBizPTSQuery = new BizPTSQuery();
        PTSProblem objPTSProblem = new PTSProblem();
        PTSProblem ptsProblem = new PTSProblem();
        int Temp_pass = 0;
        bool Status_Check = false;
        public string CustCode { get; set; }
        public string JourCode { get; set; }
        public string ArtCode { get; set; }
        public string StageId { get; set; }
        public string Volume { get; set; }
        public string Issue { get; set; }
        public string IsOpenedQuery { get; set; }
        public string isOpenedQueryById { get; set; }
        string Get_Deprtments = "";
        int customerCode;
        int journalCode;
        string article = string.Empty;
        string stage = string.Empty;
        string volume = string.Empty;
        string issue = string.Empty;
        int stageID = 0, Temp_Round = 0;
        protected string jobType = string.Empty;

        int deptCode = 0, Grid_TempChek = 0;
        string articleType = ConfigurationManager.AppSettings["ArticleType"].ToString().Trim();
        string issueType = ConfigurationManager.AppSettings["IssueType"].ToString().Trim();
        public string SmtpAddr = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPAddr");
        string xmlValue = string.Empty;
        protected int roleID;
        protected string userName = ""; //Session["userName"].ToString().Trim();

        string SubForLog = string.Empty;
        string MsgBodyForLog = string.Empty;
        List<string> lstPType = new List<string>();
        List<string> lstPTypeCurrent = new List<string>();
        List<string> lstPTypeCurrentTemp = new List<string>();
        string gtxtPTRemarksText = string.Empty;
        string gddlPTText = string.Empty;

        #endregion Fields

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                SaveButton.Attributes.Add("onclick", "return ValidationForSaveAndCloseNew()");
                CloseButton.Attributes.Add("onclick", "return ValidationForSaveAndCloseNew()");
                ZipButton.Attributes.Add("onclick", "return ZipFilesNew()");

                if (Session["lstPType"] == null)
                {
                    List<string> lstPTypenew = new List<string>();
                    lstPTypenew.Add(""); Session["lstPType"] = lstPTypenew;
                }

                //Added by Sathish.J on 10-05-2018 for online Instruction
                string _quryt = Session["queryType"].ToString().Trim();

                if (_quryt == "INSTRUCTION") { QueryResolveButton.Visible = false; CloseButton.Text = "Add instruction"; }
                else { QueryResolveButton.Visible = true; CloseButton.Text = "Raise Query"; }
                WriteLog("Line1 at RaiseProblemssub" );
                if (!Page.IsPostBack)
                {
                    if (Session["Customer"] != null)
                    {
                        if (Session["ZipFilePath"] != null)
                            hfZipFilePath.Value = Session["ZipFilePath"].ToString().Trim();
                        //Session["userName"] = userName;
                        userName = Session["userName"].ToString().Trim();
                        WriteLog("Line1 at RaiseProblemssub"+ userName);
                        CustCode = Session["Customer"].ToString();
                        JourCode = Session["Journal"].ToString();
                        ArtCode = Session["Article"].ToString();
                        StageId = Session["Stage"].ToString();
                        //////Volume = Session["Volume"].ToString();
                        //////Issue = Session["Issue"].ToString();
                        IsOpenedQuery = Session["IsOpenedQuery"].ToString();

                        lstPType = (List<string>)Session["lstPType"];
                        lstPTypeCurrent = new List<string>();
                        Session["lstPTypeCurrent"] = lstPTypeCurrent;
                        lstPTypeCurrentTemp = new List<string>();
                        Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;
                        // =======================
                        if (Session["CreatedBy"].ToString().Trim() == string.Empty)
                            dvHeader.Visible = false;
                        else
                        {
                            dvHeader.Visible = true;
                            lblCreatedBy.Text = Session["CreatedBy"].ToString();
                            lblCreatedDate.Text = Session["CreatedDate"].ToString();
                            lblSentBy.Text = Session["SentBy"].ToString();
                            lblSentDate.Text = Session["SentDate"].ToString();
                            lblResolvedBy.Text = Session["ResolvedBy"].ToString();
                            lblResolvedDate.Text = Session["ResolvedDate"].ToString();
                        }

                        //Query Submission for:  IOPP --- MTM -- Article -- Vol/ISS -- Stage

                        if (_quryt != "INSTRUCTION")
                        {
                            if (ArtCode != string.Empty)
                                lblHeader.Text = "Query details of  &nbsp;&nbsp;" + Session["Customer"].ToString().Trim() + "-" +
                                Request.QueryString["JourName"].Trim() + "-" + ArtCode + "-" + Session["StageName"].ToString();
                            else if (Volume != string.Empty && Issue != string.Empty)
                                lblHeader.Text = "Query details of  &nbsp;&nbsp;" + Request.QueryString["CustName"].Trim() + "-" +
                                Request.QueryString["JourName"].Trim() + "-" + Volume + "/" + Issue + "-" + Session["StageName"].ToString();
                            ProceedWithQuery_DropDownList1.Visible = true;
                            ProceedWithQuery_lbl.Visible = true;

                        }
                        else
                        {
                            if (ArtCode != string.Empty)
                                lblHeader.Text = "Communication details of  &nbsp;&nbsp;" + Request.QueryString["CustName"].Trim() + "-" +
                                Request.QueryString["JourName"].Trim() + "-" + ArtCode + "-" + Session["StageName"].ToString();
                            else if (Volume != string.Empty && Issue != string.Empty)
                                lblHeader.Text = "Communication details of  &nbsp;&nbsp;" + Request.QueryString["CustName"].Trim() + "-" +
                                Request.QueryString["JourName"].Trim() + "-" + Volume + "/" + Issue + "-" + Session["StageName"].ToString();
                            //ProceedWithQuery_DropDownList1.Visible = true;
                            //ProceedWithQuery_lbl.Visible = true;
                            //ProceedWithQuery_lbl.Text = "Proceed with Instruction ";
                            if (Session["StageName"].ToString().ToUpper() == "S200" || Session["StageName"].ToString().ToUpper() == "S300")
                            {
                                ProceedWithQuery_DropDownList1.Visible = true;
                                ProceedWithQuery_lbl.Visible = true;
                                ProceedWithQuery_lbl.Text = "Proceed with Instruction ";
                            }
                            else
                            {
                                ProceedWithQuery_DropDownList1.Visible = false;
                                ProceedWithQuery_lbl.Visible = false;
                            }

                            #region CheckRaisedByPM
                            bool isPM = false;
                            //string[] params1 = { "MAC001087" };
                            string[] params1 = { Session["userName"].ToString().Trim() };
                            SqlHelper sql = new SqlHelper();
                            DataSet dsIsPM = sql.GetDataSetFromSP("USP_PTS_CheckPM", params1, 2);

                            if ((dsIsPM.Tables.Count > 0) && (dsIsPM.Tables[0].Rows.Count > 0))
                            {
                                isPM = dsIsPM.Tables[0].Rows[0][0].ToString().Trim().Equals("1");
                            }
                            if (isPM == true)
                            {
                                ProceedWithQuery_DropDownList1.SelectedItem.Text = "NO";
                            }
                            
                            #endregion CheckRaisedByPM


                        }
                        // =======================

                        // =======================
                        // GetPTSConfiguration
                        string strPTypeHeader = string.Empty;
                        DataTable dtPTypeHeader = new DataTable();
                        dtPTypeHeader = ptsQueryService.GetPTSConfiguration(Convert.ToInt32(Session["Stage"].ToString().Trim()), Session["Customer"].ToString().Trim());
                        if (dtPTypeHeader != null)
                            if (dtPTypeHeader.Rows.Count > 0)
                                strPTypeHeader = dtPTypeHeader.Rows[0]["PTypeHeader"].ToString().Trim();
                        Session["strPTypeHeader"] = strPTypeHeader;
                        // =======================



                        WriteLog("User Name at RaiseProblems : " + userName);



                        roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");

                        WriteLog("roleID at RaiseProblems : " + roleID);

                        if (roleID <= 0)
                        {
                            //MessageLabel.Text = "User role not defined";
                            System.Web.HttpContext.Current.Response.Redirect("BlankPage.aspx");
                            return;
                        }
                        ClearFields();
                        CloseButton.Enabled = true;//Added By sathish.J on 28-07-2017 for Query raise directly
                        hdnUserName.Value = userName;
                        hdnName.Value = userName;

                        // =======================
                        AddFirstRow();

                        if (Session["isOpenedQueryById"] != null)
                            if (Session["isOpenedQueryById"].ToString().Trim() != string.Empty)
                                isOpenedQueryById = Session["isOpenedQueryById"].ToString().Trim();


                        if (IsOpenedQuery == "1")
                        {
                            BindOpenedQueries();
                        }
                        else if (isOpenedQueryById == "1")
                        {
                            BindOpenedQueriesById();
                            btnCancel.Enabled = false;
                        }
                        else
                        {
                            //  SaveButton.Enabled = true;//Commented By sathish.J on 28-07-2017 for Query raise directly
                            //ZipButton.Enabled = true;
                            ZipButton.Enabled = true;
                            //CloseButton.Enabled = false;//Commented By sathish.J on 28-07-2017 for Query raise directly
                            QueryResolveButton.Enabled = false;
                        }
                    }
                    else
                        ShowMessage("Session has expired");
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        private void ShowMessage(string Message)
        {
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), Guid.NewGuid().ToString(), "alert('" + QueryTrackerSetUp.GeneralHelper.RemoveSpecialChars(Message) + "');", true);
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
        private void ClearFields()
        {
            SaveButton.Enabled = false;
            CloseButton.Enabled = false;
            ZipButton.Enabled = false;
            QueryResolveButton.Enabled = false;

            DataTable dtPTCategory = new DataTable();
            hdnXml.Value = "FirstTime";
            ViewState["xml"] = "";
        }
        private void AddFirstRow()
        {
            try
            {
                DataTable dtPTInsEdit = new DataTable();
                DataRow dr = null;
                dtPTInsEdit.Columns.Add(new DataColumn("RowNumber", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("gddlDepartment", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("gddlPT", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("gddlPTCategory", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("glblBodyPrefix", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("gtxtPTRemarks", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("gbtnAddNew", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("gbtnDel", typeof(string)));
                dtPTInsEdit.Columns.Add(new DataColumn("lblPTCurrent", typeof(string)));
                //dtPTInsEdit.Columns.Add(new DataColumn("glblHiddenDesc", typeof(string))); 


                dr = dtPTInsEdit.NewRow();
                dr["RowNumber"] = 1;
                dr["gddlDepartment"] = Request.QueryString["DeptName"].ToString();
                dr["gddlPT"] = string.Empty;
                dr["gddlPTCategory"] = string.Empty;
                dr["glblBodyPrefix"] = string.Empty;
                dr["gtxtPTRemarks"] = string.Empty;
                dr["gbtnAddNew"] = string.Empty;
                dr["gbtnDel"] = string.Empty;
                dr["lblPTCurrent"] = string.Empty;
                //dr["glblHiddenDesc"] = string.Empty;
                dtPTInsEdit.Rows.Add(dr);

                ViewState["dtPTInsEdit"] = dtPTInsEdit;

                gvwPTInsEdit.DataSource = dtPTInsEdit;
                gvwPTInsEdit.DataBind();
                // ==========================================================
            }
            catch { throw; }
        }

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            // #12Dec2014
            // ======================
            WriteLog("QueryReponse");
            string fileName = Path.GetFileName(fileUpload.PostedFile.FileName);
            WriteLog("FileName"+ fileName);
            WriteLog("GetQueryDetails" + Session["Customer"].ToString().Trim()+"-"+ Session["Journal"].ToString().Trim()+"-"+ Session["Stage"].ToString().Trim());
            if (Session["Customer"].ToString().Trim() != string.Empty && Session["Journal"].ToString().Trim() != string.Empty && Session["Stage"].ToString().Trim() != string.Empty)
            {
                string Paths = string.Empty;

                int CustCode = 0;
                int.TryParse(Session["Customer"].ToString().Trim(), out CustCode);
                Paths = ptsQueryService.GetQueryResolvedUploadPath(CustCode, "QueryResolvedUploadPath");
                WriteLog("Path" + Paths);
                WriteLog("GetDetails" + Session["CustomerName"].ToString().Trim()+"-"+ Session["JournalName"].ToString().Trim()+"-"+ Session["Article"].ToString().Trim()+"-"+ Session["Volume"].ToString().Trim()+"-"+ Session["Issue"].ToString().Trim()+"-"+ Session["StageName"].ToString().Trim());
                string[] path = Paths.Split(';');
                // \\192.168.4.208\Temp\<<Customer>>\<<Jouranl>>\<<Article>>\<<Vol>>\<<Iss>>\<<Stage>>\QueryResovled-<<Datetime>>
                for (int i = 0; i < path.Length; i++)
                {
                    string configPath = path[i].ToString();
                    configPath = configPath.Replace("<<Customer>>", Session["CustomerName"].ToString().Trim());
                    configPath = configPath.Replace("<<Jouranl>>", Session["JournalName"].ToString().Trim());
                    if (Session["Article"].ToString().Trim() != string.Empty)
                    {
                        configPath = configPath.Replace("<<Article>>", Session["Article"].ToString().Trim());
                        configPath = configPath.Replace("<<Vol>>\\", string.Empty);
                        configPath = configPath.Replace("<<Iss>>\\", string.Empty);
                    }
                    else
                    {
                        configPath = configPath.Replace("<<Article>>\\", string.Empty);
                        if (Session["Volume"].ToString().Trim() != string.Empty)
                            configPath = configPath.Replace("<<Vol>>", Session["Volume"].ToString().Trim());
                        if (Session["Issue"].ToString().Trim() != string.Empty)
                            configPath = configPath.Replace("<<Iss>>", Session["Issue"].ToString().Trim());
                    }
                    configPath = configPath.Replace("<<Stage>>", Session["StageName"].ToString().Trim());
                    configPath = configPath.Replace("<<Datetime>>", DateTime.Now.ToString("dd_MMM_yyyy_HHmmss"));




                    WriteLog("Success");

                    if (configPath.ToString() != string.Empty)
                    {
                        //Path.GetFileName(fileUpload.PostedFile.FileName)
                        //using (new Impersonator("iuser", "macmill", "BMW_i20^Sx4"))
                        //{
                        WriteLog("Success1");
                        if (fileName != string.Empty)
                        {
                            WriteLog("Success2");
                            if (!Directory.Exists(configPath))
                            {
                                //using (new Impersonator("iuser", "MPS-chennai", "BMW_i20^Sx4"))
                                //{
                                Directory.CreateDirectory(configPath);
                                //   }
                            }

                            fileUpload.PostedFile.SaveAs(configPath + @"\" + fileName);
                            WriteLog("Success3");
                        }
                        WriteLog("Success4");
                        if (txtRemarks.Text.Trim() != string.Empty)
                        {
                            if (!Directory.Exists(configPath))
                            {
                                //using (new Impersonator("iuser", "MPS-chennai", "BMW_i20^Sx4"))
                                //{
                                Directory.CreateDirectory(configPath);
                                //   }
                            }

                            StreamWriter sw = new StreamWriter(configPath + @"\" + "QueryResponse.txt");
                            sw.Write(GeneralHelper.RemoveSpecialChars(txtRemarks.Text.Trim()));
                            sw.Flush();
                            sw.Close();
                            sw.Dispose();
                        }
                        //}
                    }
                    else
                    {
                        ShowMessage("");
                    }
                }
            }
            // ======================

            MessageLabel.Text = string.Empty;
            PTSProblem ptsProblem = new PTSProblem();
            string validationMessage = string.Empty;
            string customerGroupName = string.Empty;

            stageID = 0;
            customerCode = 0;
            journalCode = 0;
            article = string.Empty;
            stage = string.Empty;

            int.TryParse(Session["Stage"].ToString().Trim(), out stageID);
            int.TryParse(Session["Customer"].ToString().Trim(), out customerCode);
            int.TryParse(Session["Journal"].ToString().Trim(), out journalCode);

            article = Session["Article"].ToString().Trim(); // ArticleDropDown.SelectedValue.ToString().Trim();
            stage = Session["StageName"].ToString().Trim();
            //volume = Session["Volume"].ToString().Trim();
            //issue = Session["Issue"].ToString().Trim();
            try
            {
                WriteLog("QueryReponse-"+ Session["Customer"].ToString().Trim()+ Session["Journal"].ToString().Trim()+Session["Article"].ToString().Trim()+Session["StageName"].ToString().Trim());
                jobType = JobTypeHidden.Value;
                WriteLog("QueryReponsebefore");
                DataTable dtCustomerGrName = new DataTable();
                dtCustomerGrName = ptsQueryService.GetCustomerGroupNameAndJournalName(customerCode, journalCode);
              
                if (dtCustomerGrName.Rows.Count > 0)
                {
                    customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                }

                ptsProblem.Article = article;
                ptsProblem.Volume = volume;
                ptsProblem.Issue = issue;


                ptsProblem.StageID = stageID;
                ptsProblem.Stage = stage;
                ptsProblem.CustomerCode = customerCode;
                ptsProblem.JournalCode = journalCode;

                ptsProblem.PtsRemarks = string.Empty; //PTDescriptionTextBox.Text.Trim();

                int tempUserId = 0;
                int.TryParse(Global.userId, out tempUserId);

                //ptsProblem.QuerySavedBy = int.Parse(Global.userId);
                ptsProblem.TaskClosed = true;
                ptsProblem.ProblemID = string.Empty;


                string zipFileName = string.Empty;
                DataTable dtProblems = ptsQueryService.GetPTSProblems(customerCode, journalCode, article, stageID);
                if (dtProblems.Rows.Count > 0)
                    zipFileName = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim();

                if (File.Exists(zipFileName))
                    ptsProblem.ZipFileUpload = true;
                else
                    ptsProblem.ZipFileUpload = false;

                ptsProblem.UserName = Session["UserName"].ToString();
                ptsProblem.ID = Convert.ToInt32(Session["OpenedRequestId"].ToString().Trim());
                ptsProblem.UserID = tempUserId;
                ptsProblem.QueryResolveRemarks = txtRemarks.Text.Trim();
                ptsProblem.QueryResolveRemarks = ptsProblem.QueryResolveRemarks.Replace("&", "&amp;");
                ptsProblem.QueryResolveRemarks = ptsProblem.QueryResolveRemarks.Replace("<", "&lt;");
                ptsProblem.QueryResolveRemarks = ptsProblem.QueryResolveRemarks.Replace(">", "&gt;");
                //ShowMessage(ptsQueryService.ResolvePTSProblemByID(ptsProblem));
                WriteLog("SessionDetails"+ Session["UserName"].ToString()+ Session["OpenedRequestId"].ToString().Trim());
                WriteLog("QueryReponsebefore1");
                string task_msg = ptsQueryService.ResolvePTSProblemByID(ptsProblem);

                WriteLog("QueryReponseafter" + task_msg);
                Session["task_msg"] = task_msg;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', ' " + task_msg + "', 'Success', 'top', 'right');", true);
                ResultLabel.Text = string.Empty;


                // code added by prasanta start
                // delete the file if exist 
                string problemID = string.Empty;
                problemID = ptsQueryService.GetPTSProblemID();
                WriteLog("QueryproblemID" + problemID);
                string zipFileDestination = "";
                string signalTemplate = "";
                string signalFileDestination = "";

                //zipFileName = zipFileDestination + problemID + ".zip";

                // updated by prasanta
                string strZipFileBkp = string.Empty;
                WriteLog("QueryproblemID1");
                if (File.Exists(zipFileName))
                {
                    WriteLog("QueryproblemID2");
                    try
                    {
                        ////signalTemplate = signalTemplate.Replace("[zipfilename]", problemID + ".zip");
                        File.Delete(zipFileName);
                    }
                    catch
                    {
                    }

                }
                else
                {
                    signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                    signalFileDestination = zipFileDestination;
                }
                ClearFields();
                Session["ResultMsg"] = string.Empty;
                btnCancel.Text = "Back";
                string PtsRemarks1 = dtProblems.Rows[0]["PtsRemarks"].ToString();
                #region TriggerMail
                DataTable dtQueryMail = new DataTable();
                dtQueryMail = ptsQueryService.GetPTSConfiguration(stageID, customerCode.ToString(), Convert.ToInt64(journalCode), article, 0);//usp_PTS_GetAllPTSConfiguration_Newone
                bool IsQueryResolveMailReqd = Convert.ToBoolean(dtQueryMail.Rows[0]["IsQueryResolveMailReqd"].ToString().Trim());
                if (IsQueryResolveMailReqd)
                {
                    string Query_FrmMailID = dtQueryMail.Rows[0]["FromEmailID"].ToString().Trim();
                    string Query_ToMailID = dtQueryMail.Rows[0]["QueryResolv_To"].ToString().Trim();
                    string Query_CCMailID = dtQueryMail.Rows[0]["QueryResolv_CC"].ToString().Trim();
                    // string Query_MailSubject = dtQueryMail.Rows[0]["QueryMailSubject"].ToString().Trim();
                    // string Query_MailContent = dtQueryMail.Rows[0]["QueryMailContent"].ToString().Trim();
                    Mail_QueryResolve(customerCode, Session["JournalName"].ToString().Trim(), article, ptsProblem.QueryResolveRemarks, PtsRemarks1, Query_FrmMailID, Query_ToMailID, Query_CCMailID, "QUERY RESOLVED SUCCESSFULLY");

                }
                #endregion TriggerMail
                WriteLog("QueryproblemID4");
                //Query resolve trigger added on 13-07-17
                //DataTable dtMail = new DataTable();
                //Commented on 02-01-2019
                //FrmPMMailID = ptsQueryService.QueryResolvedByPM(ptsProblem.UserName, FrmPMMailID);

                //Commented on 02-01-2019
                //dtMail = ptsQueryService.GetMailDetailsforQueryResolve(customerCode, stageID);//Modified BY Sath
                // //string ToMailID = "Sathish.j@mps-in.com"; //dtMail.Rows[0]["QueryResolv_To"].ToString(); //Commnented by Sathish.j on 13-07-17
                // //string CCMailID = "ashokkumar.d@mps-in.com";// dtMail.Rows[0]["QueryResolv_CC"].ToString();//Commnented by Sathish.j on 13-07-17ish.j

                //string ToMailID = FrmPMMailID;// dtMail.Rows[0]["QueryResolv_To"].ToString(); //Commnented by Sathish.j on 13-07-17
                //Commented on 02-01-2019
                //string CCMailID = dtMail.Rows[0]["QueryResolv_CC"].ToString();//Commnented by Sathish.j on 13-07-17
                //return;//Modified BY sathish.j

                // Mail_QueryResolve(customerCode, Session["JournalName"].ToString().Trim(), article, ptsProblem.QueryResolveRemarks, PtsRemarks1, FrmPMMailID, ToMailID, CCMailID, task_msg);


                //Response.Redirect("RaiseProblems.aspx");
                //ShowMessage("Query response made successfully");
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', 'Query response made successfully', 'Required', 'top', 'right');", true);

                WriteLog("Before Raise : " + Session["userName"].ToString());
                Session["userName"] = Session["userName"].ToString();
                WriteLog("RaiseProblems.aspx?Username=" + Session["userName"].ToString() + "&Redirect_msg=" + Session["task_msg"].ToString());
                Response.Redirect("RaiseProblems.aspx?Username=" + Session["Encrypted User"].ToString() + "&Redirect_msg=" + Session["task_msg"].ToString(), false);
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        //Mail Trigger for query Resolve
        private void Mail_QueryResolve(int customer, string book, string chap, string resolvermk, string raisedquery, string FromId, string ToId, string CcId, string task_msg)
        {
            if (task_msg == "QUERY RESOLVED SUCCESSFULLY") { } else { Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Something went wrong', 'Success', 'top', 'right');", true); return; }
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress(FromId);

            string[] tempMail = ToId.Split(',');
            for (int i = 0; i < tempMail.Length; i++)
                message.To.Add(new System.Net.Mail.MailAddress(tempMail[i]));

            tempMail = CcId.Split(',');
            for (int i = 0; i < tempMail.Length; i++)
                if (tempMail[i].Trim() != "")
                    message.CC.Add(new System.Net.Mail.MailAddress(tempMail[i]));

            message.CC.Add(new System.Net.Mail.MailAddress(FromId));

            string MsgBody = "<p style=\"font-family: Times New Roman;font-size:12pt;\">Dear All,<br/><br/>Raised Query:<br/>" + raisedquery + "<br/><br/>Resolved Remarks:<br/>" + resolvermk + "</p><br/><p style=\"color:red;font-style: italic;\">This is an Auto generated mail... </p>";

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(SmtpAddr);

            message.IsBodyHtml = true;
            message.Priority = System.Net.Mail.MailPriority.Normal;
            message.Subject = "Query Resolved for " + book + "_" + chap;
            message.Body = MsgBody;

            client.Send(message);
            //WriteLog(1, message.Subject, message.Body, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "After Sending mail");
            //System.Threading.Thread.Sleep(500);
            string ResultMsg = "";
            ResultMsg += "Mail triggered to production.";
            // ShowMessage(ResultMsg);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', '" + ResultMsg + "', 'Success', 'top', 'right');", true);
        }

        private void BindOpenedQueries()
        {
            try
            {
                int CloseStatus = 0;
                int rowId = (int)Session["rowId"];
                ViewState["editRowIndex"] = rowId;

                DataTable dtOpenedRequest = (DataTable)Session["OpenedRequest"];
                string strPTId = dtOpenedRequest.Rows[rowId]["PTId"].ToString();
                string strDeptIds = dtOpenedRequest.Rows[rowId]["PTDepartmentId"].ToString();
                string strPTypeIds = dtOpenedRequest.Rows[rowId]["ProblemTypeId"].ToString();
                string strPTCode = dtOpenedRequest.Rows[rowId]["PTCode"].ToString();
                string strPTDesc = dtOpenedRequest.Rows[rowId]["PTSRemarks"].ToString();
                string strPTShortDesc = dtOpenedRequest.Rows[rowId]["PTShortDesc"].ToString();
                string strPTDescLong = dtOpenedRequest.Rows[rowId]["PTLongDesc"].ToString();
                string strPTCRemarks = dtOpenedRequest.Rows[rowId]["PTCRemarks"].ToString();
                string strPTCId = dtOpenedRequest.Rows[rowId]["PTCId"].ToString();
                ////string strPTCDesc = dtOpenedRequest.Rows[rowId]["PTCDesc"].ToString();
                string strHiddenDesc = dtOpenedRequest.Rows[rowId]["HiddenDesc"].ToString();

                //strPTDescLong = strPTDescLong.Replace("$", "&#36;");
                //strPTShortDesc = strPTShortDesc.Replace("$", "&#36;");
                //strPTCRemarks = strPTCRemarks.Replace("$", "&#36;");


                string[] arrStrDeptIds = strDeptIds.Split(',');
                string[] arrStrPTypeIds = strPTypeIds.Split(',');
                string[] arrStrPTCode = strPTCode.Split(',');
                string[] arrStrPTDesc = strPTDesc.Split(',');
                string[] arrStrPTDescLong = strPTDescLong.Split('$');
                string[] arrStrPTShortDesc = strPTShortDesc.Split('$');
                string[] arrStrPTCRemarks = strPTCRemarks.Split('$');
                ////string[] arrStrPTCId = strPTCId.Split(',');
                string[] arrHiddenDesc = strHiddenDesc.Split(',');



                string[] arrStrPTCId = null;
                if (strPTCId != string.Empty)
                    arrStrPTCId = strPTCId.Split(',');

                string strPTypeHeader = Session["strPTypeHeader"].ToString().Trim();
                string totalRemarks = string.Empty;


                //string[] arrPTDesc = Regex.Split(arrStrPTDesc[0].Trim(), "\r\n");
                //string[] arrPTDesc = Regex.Split(arrStrPTDesc[0].Substring(arrStrPTDesc[0].IndexOf("(")).Trim(), "\r\n");
                //arrStrPTDesc[0].Split("\\r\\n", StringSplitOptions.None);

                lstPType = new List<string>();
                for (int i = 0; i < arrStrPTypeIds.Length; i++)
                {
                    if (i == 0)
                        AddFirstRow();
                    else
                        AddNewRow();
                    // =================
                    HtmlSelect gddlDepartment = (HtmlSelect)gvwPTInsEdit.Rows[i].FindControl("gddlDepartment");
                    DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPT");
                    DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPTCategory");
                    TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[i].FindControl("gtxtPTRemarks");
                    gddlDepartment.SelectedIndex = gddlDepartment.Items.IndexOf(gddlDepartment.Items.FindByValue(arrStrDeptIds[i].Trim()));
                    Label lblPTCurrent = (Label)gvwPTInsEdit.Rows[i].FindControl("lblPTCurrent");
                    //Label glblHiddenDesc = (Label)gvwPTInsEdit.Rows[i].FindControl("glblHiddenDesc");

                    DataTable dtPT = GetProblemTypeDDL(Convert.ToInt32(Session["Customer"]), Convert.ToInt32(arrStrDeptIds[i].Trim()), Convert.ToInt32(Session["Stage"]));
                    gddlPT.DataSource = dtPT;
                    gddlPT.DataTextField = Global.DB_FIELD_NAME;
                    gddlPT.DataValueField = Global.DB_FIELD_ID;
                    gddlPT.DataBind();
                    gddlPT.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                    gddlPT.SelectedIndex = gddlPT.Items.IndexOf(gddlPT.Items.FindByValue(arrStrPTypeIds[i].Trim()));
                    lstPType = (List<string>)Session["lstPType"];

                    //lstPType.Add(gddlPT.SelectedValue);
                    //Session["lstPType"] = lstPType;
                    ////gddlPT.SelectedIndex = gddlPT.Items.IndexOf(gddlPT.Items.FindByValue(dt.Rows[i]["gddlPT"].ToString().Trim()));

                    //lstPType.Add(gddlDepartment.SelectedValue + "|" + gddlPT.SelectedValue);//.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim());
                    //Session["lstPType"] = lstPType;

                    // =================
                    DataTable dtPTCat = GetProblemTypeCategoryDDL(gddlPT.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")), Convert.ToInt32(Session["Customer"]), Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()), Convert.ToInt32(Session["Stage"].ToString().Trim()));
                    if (dtPTCat != null)
                    {
                        if (dtPTCat.Rows.Count > 0)
                        {
                            gddlPTCategory.DataSource = dtPTCat;
                            gddlPTCategory.DataTextField = Global.DB_FIELD_NAME;
                            gddlPTCategory.DataValueField = Global.DB_FIELD_ID;
                            gddlPTCategory.DataBind();
                            gddlPTCategory.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                            gddlPTCategory.SelectedIndex = gddlPTCategory.Items.IndexOf(gddlPTCategory.Items.FindByValue(arrStrPTCId[i].Trim()));
                        }
                    }
                    // =================
                    string strDesc = GetPTSDesc(arrStrPTypeIds[i].Trim());
                    Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[i].FindControl("glblBodyPrefix");
                    glblBodyPrefix.Text = strDesc;
                    // =================

                    string tPTCategory = string.Empty;
                    string tPTRemarks = string.Empty;
                    if (gddlPTCategory.SelectedIndex > 0)
                        tPTCategory = gddlPTCategory.SelectedItem.Text.Trim();

                    // ########################
                    gtxtPTRemarks.Text = arrStrPTCRemarks[i].ToString().Trim();
                    gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&amp;", "&");
                    gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&lt;", "<");
                    gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&gt;", ">");
                    gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&#36;", "$");



                    if (gtxtPTRemarks.Text != string.Empty)
                        tPTRemarks = gtxtPTRemarks.Text.Trim();
                    // ===================
                    tPTCategory = tPTCategory.Trim() != string.Empty ? (" (" + tPTCategory.Trim() + ") ") : string.Empty;
                    if (gddlPT.SelectedItem.Text.Trim().IndexOf("--") > 0)
                        gddlPTText = gddlPT.SelectedItem.Text.Trim() != string.Empty ? (" (" + (gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim()) + ") ") : string.Empty;
                    else
                        gddlPTText = string.Empty;
                    //gddlPTText = gddlPT.SelectedItem.Text.Trim() != string.Empty ? (" (" + (gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim()) + ") ") : string.Empty;
                    gtxtPTRemarksText = gtxtPTRemarks.Text.Trim() != string.Empty ? (" (" + gtxtPTRemarks.Text.Trim() + ") ") : string.Empty;
                    if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                    {
                        totalRemarks += "\r\n" + gddlPTText + tPTCategory + gtxtPTRemarks.Text.Trim();
                    }
                    else
                        totalRemarks += "\r\n" + gtxtPTRemarks.Text.Trim();
                    // ===================

                    //if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                    //{
                    //    if (gtxtPTRemarks.Text.Trim() != string.Empty)
                    //        ////totalRemarks += "\r\n" + " (" + arrStrPTCId[i].ToString() + ") ( " + gddlPT.SelectedItem.Text.Trim() + glblBodyPrefix.Text.ToString().Trim() + " ( " + gtxtPTRemarks.Text.Trim() + " )";
                    //        //totalRemarks += "\r\n" + " () ( " + gddlPT.SelectedItem.Text.Trim() + glblBodyPrefix.Text.ToString().Trim() + " ( " + gtxtPTRemarks.Text.Trim() + " )";
                    //        totalRemarks += "\r\n" + gddlPTText + gddlPT.SelectedItem.Text.Trim() + glblBodyPrefix.Text.ToString().Trim() + gtxtPTRemarksText;
                    //    else
                    //        ////totalRemarks += "\r\n" + " (" + arrStrPTCId[i].ToString().Trim() + ") ( " + gddlPT.SelectedItem.Text.Trim() + glblBodyPrefix.Text.ToString().Trim();// +" ( " + gtxtPTRemarks.Text.Trim() + " )";
                    //        //totalRemarks += "\r\n" + " (" + gddlPT.SelectedItem.Text.Trim() + glblBodyPrefix.Text.ToString().Trim();// +" ( " + gtxtPTRemarks.Text.Trim() + " )";
                    //        totalRemarks += "\r\n" + gddlPTText + gddlPT.SelectedItem.Text.Trim() + glblBodyPrefix.Text.ToString().Trim(); // +" ( " + gtxtPTRemarks.Text.Trim() + " )";
                    //}
                    //else //if (strPTypeHeader.ToLower().ToString().Trim() == "false" || strPTypeHeader.ToString().Trim() == "0")
                    //    ////totalRemarks += "\r\n" + arrStrPTCId[i].ToString().Trim() + glblBodyPrefix.Text.ToString().Trim() + "(" + gtxtPTRemarks.Text.Trim() + ")";
                    //    //totalRemarks += "\r\n" + glblBodyPrefix.Text.ToString().Trim() + "(" + gtxtPTRemarks.Text.Trim() + ")";
                    //    totalRemarks += "\r\n" + glblBodyPrefix.Text.ToString().Trim() + gtxtPTRemarksText;


                    //gtxtPTRemarks.Text = arrStrPTCRemarks[i].ToString().Trim();
                    //glblHiddenDesc.Text = arrHiddenDesc[i].ToString().Trim();

                    // =================
                    lblPTCurrent.Text = arrStrDeptIds[i].ToString().Trim() + "|" + arrStrPTypeIds[i].ToString().Trim();

                    lstPTypeCurrent.Add(lblPTCurrent.Text.Trim());
                    Session["lstPTypeCurrent"] = lstPTypeCurrent;

                    lstPTypeCurrentTemp.Add(lblPTCurrent.Text.Trim());
                    Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;

                    //lstPType = (List<string>)Session["lstPType"];
                    //lstPType.Remove(lblPTCurrent.Text.Trim());
                    //Session["lstPType"] = lstPType;
                }

                lstPType = (List<string>)Session["lstPType"];
                lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                foreach (string lst in lstPTypeCurrentTemp)
                {
                    lstPType.Remove(lst.Trim());
                    Session["lstPType"] = lstPType;
                }


                if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                    totalRemarks = DateTime.Now.ToString("dd-MMM-yyyy") + totalRemarks;
                //ptsProblem.PtsRemarks = GeneralHelper.RemoveSpecialChars(totalRemarks); // need
                ptsProblem.PtsRemarks = totalRemarks;

                Session["totalRemarks"] = ptsProblem.PtsRemarks;
                // ===================================================
                int tCustCode = 0;
                int.TryParse(Session["Customer"].ToString().Trim(), out tCustCode);
                roleID = ptsQueryService.GetRoleIDByUserName(userName, tCustCode.ToString());

                EnableDisableBasedOnRole();
                GetCloseStatusNew(Session["Customer"].ToString(), -1);

                //Added by - Ram for loading dept
                DataTable dtProblems = new DataTable();
                string deptId = "0";
                dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(strPTId.Trim()));
                DataRow[] dr = dtProblems.Select();
                // ================================
                if (dr.Length > 0)
                {
                    hfZipFilePath.Value = dr[0]["ZipFilePath"].ToString().Trim();
                    if (dr[0]["ZipFilePath"].ToString().Trim() != string.Empty)
                    {
                        if (File.Exists(dr[0]["ZipFilePath"].ToString().Trim()))
                        {
                            string zipFileName = dr[0]["ZipFilePath"].ToString().Trim();
                            lbtnZipFile.Visible = true;
                            lbtnZipFile.Text = "(Download)";
                            lblZipFile.Text = dr[0]["ZipFilePath"].ToString().Trim();
                            Session["zipFile"] = zipFileName;
                        }
                        else
                        {
                            lbtnZipFile.Visible = false;
                            lbtnZipFile.Text = string.Empty;
                            lblZipFile.Text = string.Empty;
                        }
                    }
                    else
                    {
                        lbtnZipFile.Text = string.Empty;
                        lblZipFile.Text = string.Empty;
                    }
                    deptId = dr[0]["departmentId"].ToString().Trim();

                    stage = dr[0]["SUBSTAGEID"].ToString().Trim();

                    if (Convert.ToBoolean(dr[0]["TaskClosed"].ToString()))
                        CloseStatus = 1;

                    DataTable dtProblemDetails = new DataTable();//**ptsQueryService.GetProblemDetails(Convert.ToInt32(dr[0]["Id"].ToString().Trim()));
                    //gvwPTCategory.DataSource = dtProblemDetails;
                    //gvwPTCategory.DataBind();
                    DataTable dtPTCategory = dtProblemDetails;
                    ViewState["dtPTCategory"] = dtPTCategory;
                    dtPTCategory.TableName = "ProblemType";
                    string xmlString = string.Empty;
                    string xml = string.Empty;
                    using (TextWriter writer = new StringWriter())
                    {
                        dtPTCategory.WriteXml(writer);
                        xml = writer.ToString();
                    }
                    xml = xml.Replace("DocumentElement", "Details");
                    ViewState["xml"] = xml;


                    hdnReviewerName.Value = dr[0]["QuerySavedBy"].ToString();
                    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), CloseStatus);
                }
                else
                {
                    //ddlDepartment.SelectedIndex = 0;
                    hdnReviewerName.Value = userName;
                    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), 0);
                }
                objBizPTSQuery.Article = Session["Article"].ToString().Trim();
                //objBizPTSQuery.Volume = Session["Volume"].ToString().Trim();
                //objBizPTSQuery.Issue = Session["Issue"].ToString().Trim();

                //===================================
                string customerGroupName = string.Empty;
                string zipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString();
                zipFilePath = clsServerConfig.GetStringReplacement(zipFilePath, Convert.ToInt32(Session["Customer"].ToString().Trim()));
                zipFilePath = zipFilePath.Replace("[stage]", Session["Stage"].ToString().Trim());
                ViewState["zipFilePath"] = zipFilePath;

                DataTable dtCustomerGrName = new DataTable();
                dtCustomerGrName = ptsQueryService.GetCustomerGroupNameAndJournalName(ptsProblem.CustomerCode, ptsProblem.JournalCode);
                if (dtCustomerGrName.Rows.Count > 0)
                {
                    customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                }
                hdnXml.Value = "1";

                //===================================
                string Status = dtOpenedRequest.Rows[rowId]["Status"].ToString();
                if (Status.Trim().ToLower() == "resolved")
                {
                    ZipButton.Enabled = false;
                    SaveButton.Enabled = false;
                    CloseButton.Enabled = false;
                    QueryResolveButton.Enabled = false;
                    gvwPTInsEdit.Enabled = false;
                    btnCancel.Text = "Back";
                }
                else if (Status.Trim().ToLower() == "raised")
                {
                    gvwPTInsEdit.Enabled = false;
                    btnCancel.Text = "Cancel";
                }
                else
                    btnCancel.Text = "Cancel";
                // ===================================================
                if (CloseStatus != 1)
                {
                    Button gbtnAddNew = (Button)gvwPTInsEdit.Rows[arrStrPTypeIds.Length - 1].FindControl("gbtnAddNew");
                    gbtnAddNew.Enabled = true;
                }
            }
            catch { throw; }
        }
        private void BindOpenedQueriesById()
        {
            try
            {
                int CloseStatus = 0;
                int rowId = 0; // (int)Session["rowId"];
                //ViewState["editRowIndex"] = rowId;

                DataTable dtOpenedRequestById = (DataTable)Session["dtOpenedRequestById"];
                string strPTId = dtOpenedRequestById.Rows[rowId]["PTId"].ToString();
                string strDeptIds = dtOpenedRequestById.Rows[rowId]["PTDepartmentId"].ToString();
                string strPTypeIds = dtOpenedRequestById.Rows[rowId]["ProblemTypeId"].ToString();
                string strPTCode = dtOpenedRequestById.Rows[rowId]["PTCode"].ToString();
                string strPTDesc = dtOpenedRequestById.Rows[rowId]["PTSRemarks"].ToString();
                string strPTShortDesc = dtOpenedRequestById.Rows[rowId]["PTShortDesc"].ToString();
                string strPTDescLong = dtOpenedRequestById.Rows[rowId]["PTLongDesc"].ToString();
                string strPTCRemarks = dtOpenedRequestById.Rows[rowId]["PTCRemarks"].ToString();
                string strPTCId = dtOpenedRequestById.Rows[rowId]["PTCId"].ToString();
                //string strPTCDesc = dtOpenedRequestById.Rows[rowId]["PTCDesc"].ToString();

                //strPTDescLong = strPTDescLong.Replace("$", "&#36;");
                //strPTShortDesc = strPTShortDesc.Replace("$", "&#36;");
                //strPTCRemarks = strPTCRemarks.Replace("$", "&#36;");

                string[] arrStrDeptIds = strDeptIds.Split(',');
                string[] arrStrPTypeIds = strPTypeIds.Split(',');
                string[] arrStrPTCode = strPTCode.Split(',');
                string[] arrStrPTDesc = strPTDesc.Split(',');
                string[] arrStrPTDescLong = strPTDescLong.Split('$');
                string[] arrStrPTShortDesc = strPTShortDesc.Split('$');
                string[] arrStrPTCRemarks = strPTCRemarks.Split('$');

                string[] arrStrPTCId = null;
                if (strPTCId != string.Empty)
                    arrStrPTCId = strPTCId.Split(',');

                string strPTypeHeader = Session["strPTypeHeader"].ToString().Trim();
                string totalRemarks = string.Empty;

                lstPType = new List<string>();
                for (int i = 0; i < arrStrPTypeIds.Length; i++)
                {
                    if (i == 0)
                        AddFirstRow();
                    else
                        AddNewRow();
                    // =================
                    HtmlSelect gddlDepartment = (HtmlSelect)gvwPTInsEdit.Rows[i].FindControl("gddlDepartment");
                    DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPT");
                    DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPTCategory");
                    TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[i].FindControl("gtxtPTRemarks");
                    gddlDepartment.SelectedIndex = gddlDepartment.Items.IndexOf(gddlDepartment.Items.FindByValue(arrStrDeptIds[i].Trim()));
                    Label lblPTCurrent = (Label)gvwPTInsEdit.Rows[i].FindControl("lblPTCurrent");

                    DataTable dtPT = GetProblemTypeDDL(Convert.ToInt32(Session["customerCode"]), Convert.ToInt32(arrStrDeptIds[i].Trim()), Convert.ToInt32(Session["stageId"]));
                    gddlPT.DataSource = dtPT;
                    gddlPT.DataTextField = Global.DB_FIELD_NAME;
                    gddlPT.DataValueField = Global.DB_FIELD_ID;
                    gddlPT.DataBind();
                    gddlPT.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                    gddlPT.SelectedIndex = gddlPT.Items.IndexOf(gddlPT.Items.FindByValue(arrStrPTypeIds[i].Trim()));
                    lstPType = (List<string>)Session["lstPType"];

                    // =================
                    DataTable dtPTCat = GetProblemTypeCategoryDDL(gddlPT.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")), Convert.ToInt32(Session["Customer"]), Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()), Convert.ToInt32(Session["stageId"].ToString().Trim()));
                    if (dtPTCat != null)
                    {
                        if (dtPTCat.Rows.Count > 0)
                        {
                            gddlPTCategory.DataSource = dtPTCat;
                            gddlPTCategory.DataTextField = Global.DB_FIELD_NAME;
                            gddlPTCategory.DataValueField = Global.DB_FIELD_ID;
                            gddlPTCategory.DataBind();
                            gddlPTCategory.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                            gddlPTCategory.SelectedIndex = gddlPTCategory.Items.IndexOf(gddlPTCategory.Items.FindByValue(arrStrPTCId[i].Trim()));
                        }
                    }
                    // =================
                    string strDesc = GetPTSDesc(arrStrPTypeIds[i].Trim());
                    Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[i].FindControl("glblBodyPrefix");
                    glblBodyPrefix.Text = strDesc;
                    // =================

                    string tPTCategory = string.Empty;
                    string tPTRemarks = string.Empty;
                    if (gddlPTCategory.SelectedIndex > 0)
                        tPTCategory = gddlPTCategory.SelectedItem.Text.Trim();

                    // ########################

                    gtxtPTRemarks.Text = arrStrPTCRemarks[i].ToString().Trim();
                    gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&#36;", "$");

                    if (gtxtPTRemarks.Text != string.Empty)
                        tPTRemarks = gtxtPTRemarks.Text.Trim();
                    // ===================
                    tPTCategory = tPTCategory.Trim() != string.Empty ? (" (" + tPTCategory.Trim() + ") ") : string.Empty;
                    if (gddlPT.SelectedItem.Text.Trim().IndexOf("--") > 0)
                        gddlPTText = gddlPT.SelectedItem.Text.Trim() != string.Empty ? (" (" + (gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim()) + ") ") : string.Empty;
                    else
                        gddlPTText = string.Empty;
                    gtxtPTRemarksText = gtxtPTRemarks.Text.Trim() != string.Empty ? (" (" + gtxtPTRemarks.Text.Trim() + ") ") : string.Empty;
                    if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                    {
                        totalRemarks += "\r\n" + gddlPTText + tPTCategory + gtxtPTRemarks.Text.Trim();
                    }
                    else
                        totalRemarks += "\r\n" + gtxtPTRemarks.Text.Trim();
                    // ===================

                    // =================
                    lblPTCurrent.Text = arrStrDeptIds[i].ToString().Trim() + "|" + arrStrPTypeIds[i].ToString().Trim();

                    lstPTypeCurrent.Add(lblPTCurrent.Text.Trim());
                    Session["lstPTypeCurrent"] = lstPTypeCurrent;

                    lstPTypeCurrentTemp.Add(lblPTCurrent.Text.Trim());
                    Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;
                }

                if (Session["lstPType"] != null)
                    lstPType = (List<string>)Session["lstPType"];
                else
                {
                    lstPType = new List<string>();
                    lstPType.Add("0|0");
                }

                lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                foreach (string lst in lstPTypeCurrentTemp)
                {
                    lstPType.Remove(lst.Trim());
                    Session["lstPType"] = lstPType;
                }


                if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                    totalRemarks = DateTime.Now.ToString("dd-MMM-yyyy") + totalRemarks;
                ptsProblem.PtsRemarks = totalRemarks;

                Session["totalRemarks"] = ptsProblem.PtsRemarks;
                // ===================================================
                int tCustCode = 0;
                int.TryParse(Session["customerCode"].ToString().Trim(), out tCustCode);
                roleID = ptsQueryService.GetRoleIDByUserName(userName, tCustCode.ToString());

                EnableDisableBasedOnRole();
                GetCloseStatusNew(Session["customerCode"].ToString(), -1);

                //Added by - Ram for loading dept
                DataTable dtProblems = new DataTable();
                string deptId = "0";
                dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(strPTId.Trim()));
                DataRow[] dr = dtProblems.Select();
                // ================================
                if (dr.Length > 0)
                {
                    hfZipFilePath.Value = dr[0]["ZipFilePath"].ToString().Trim();
                    if (dr[0]["ZipFilePath"].ToString().Trim() != string.Empty)
                    {
                        if (File.Exists(dr[0]["ZipFilePath"].ToString().Trim()))
                        {
                            string zipFileName = dr[0]["ZipFilePath"].ToString().Trim();
                            lbtnZipFile.Visible = true;
                            lbtnZipFile.Text = "(Download)";
                            lblZipFile.Text = dr[0]["ZipFilePath"].ToString().Trim();
                            Session["zipFile"] = zipFileName;
                        }
                        else
                        {
                            lbtnZipFile.Visible = false;
                            lbtnZipFile.Text = string.Empty;
                            lblZipFile.Text = string.Empty;
                        }
                    }
                    else
                    {
                        lbtnZipFile.Text = string.Empty;
                        lblZipFile.Text = string.Empty;
                    }
                    deptId = dr[0]["departmentId"].ToString().Trim();

                    int stageId = 0;
                    int.TryParse(dr[0]["SUBstageId"].ToString().Trim(), out stageId);

                    if (Convert.ToBoolean(dr[0]["TaskClosed"].ToString()))
                        CloseStatus = 1;

                    DataTable dtProblemDetails = new DataTable();//**ptsQueryService.GetProblemDetails(Convert.ToInt32(dr[0]["Id"].ToString().Trim()));
                    DataTable dtPTCategory = dtProblemDetails;
                    ViewState["dtPTCategory"] = dtPTCategory;
                    dtPTCategory.TableName = "ProblemType";
                    string xmlString = string.Empty;
                    string xml = string.Empty;
                    using (TextWriter writer = new StringWriter())
                    {
                        dtPTCategory.WriteXml(writer);
                        xml = writer.ToString();
                    }
                    xml = xml.Replace("DocumentElement", "Details");
                    ViewState["xml"] = xml;


                    hdnReviewerName.Value = dr[0]["QuerySavedBy"].ToString();
                    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), CloseStatus);
                }
                else
                {
                    hdnReviewerName.Value = userName;
                    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), 0);
                }
                //objBizPTSQuery.Article = Session["Article"].ToString().Trim();
                //objBizPTSQuery.Volume = Session["volume"].ToString().Trim();
                //objBizPTSQuery.Issue = Session["issue"].ToString().Trim();

                //===================================
                string customerGroupName = string.Empty;
                string zipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString();
                zipFilePath = clsServerConfig.GetStringReplacement(zipFilePath, Convert.ToInt32(Session["customerCode"].ToString().Trim()));
                zipFilePath = zipFilePath.Replace("[stage]", Session["stageId"].ToString().Trim());
                ViewState["zipFilePath"] = zipFilePath;

                DataTable dtCustomerGrName = new DataTable();
                //** dtCustomerGrName = ptsQueryService.GetCustomerGroupName(ptsProblem.Cust_code);
                if (dtCustomerGrName.Rows.Count > 0)
                {
                    customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                }
                hdnXml.Value = "1";

                //===================================
                string Status = dtOpenedRequestById.Rows[rowId]["Status"].ToString();
                if (Status.Trim().ToLower() == "resolved")
                {
                    ZipButton.Enabled = false;
                    SaveButton.Enabled = false;
                    CloseButton.Enabled = false;
                    QueryResolveButton.Enabled = false;
                    gvwPTInsEdit.Enabled = false;
                    btnCancel.Text = "Back";
                }
                else if (Status.Trim().ToLower() == "raised")
                {
                    gvwPTInsEdit.Enabled = false;
                    btnCancel.Text = "Cancel";
                }
                else
                    btnCancel.Text = "Cancel";
                // ===================================================
                if (CloseStatus != 1)
                {
                    Button gbtnAddNew = (Button)gvwPTInsEdit.Rows[arrStrPTypeIds.Length - 1].FindControl("gbtnAddNew");
                    gbtnAddNew.Enabled = true;
                }
            }
            catch { throw; }
        }
        private void AddNewRow()
        {
            int rowIndex = 0;

            if (ViewState["dtPTInsEdit"] != null)
            {
                DataTable dtPTInsEdit = (DataTable)ViewState["dtPTInsEdit"];
                DataRow dr = null;
                if (dtPTInsEdit.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtPTInsEdit.Rows.Count; i++)
                    {
                        HtmlSelect gddlDepartment =
                          (HtmlSelect)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlDepartment");
                        DropDownList gddlPT =
                          (DropDownList)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlPT");
                        DropDownList gddlPTCategory =
                          (DropDownList)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlPTCategory");
                        TextBox gtxtPTRemarks =
                          (TextBox)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gtxtPTRemarks");
                        Label glblBodyPrefix =
                          (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("glblBodyPrefix");

                        Label lblPTCurrent = (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("lblPTCurrent");
                        //Label glblHiddenDesc = (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("glblHiddenDesc");

                        Button gbtnDel = (Button)gvwPTInsEdit.Rows[rowIndex].FindControl("gbtnDel");
                        gbtnDel.Enabled = true;

                        dr = dtPTInsEdit.NewRow();
                        dr["RowNumber"] = i + 1;

                        //dtPTInsEdit.Rows[i - 1]["RowNumber"] = i;
                        dtPTInsEdit.Rows[i - 1]["gddlDepartment"] = Request.QueryString["DeptValue"].ToString().Trim().ToString();
                        dtPTInsEdit.Rows[i - 1]["gddlPT"] = gddlPT.SelectedValue.ToString();
                        dtPTInsEdit.Rows[i - 1]["gddlPTCategory"] = gddlPTCategory.SelectedValue.ToString();
                        dtPTInsEdit.Rows[i - 1]["glblBodyPrefix"] = glblBodyPrefix.Text.Trim();
                        dtPTInsEdit.Rows[i - 1]["gtxtPTRemarks"] = gtxtPTRemarks.Text;
                        dtPTInsEdit.Rows[i - 1]["lblPTCurrent"] = lblPTCurrent.Text;
                        //dtPTInsEdit.Rows[i - 1]["glblHiddenDesc"] = glblHiddenDesc.Text;

                        rowIndex++;
                    }
                    dtPTInsEdit.Rows.Add(dr);
                    ViewState["dtPTInsEdit"] = dtPTInsEdit;

                    gvwPTInsEdit.DataSource = dtPTInsEdit;
                    gvwPTInsEdit.DataBind();


                }
            }
            else
            {
                ShowMessage("ViewState is null");
            }
            SetPreviousData();
        }
        private void SetPreviousData()
        {
            int rowIndex = 0;
            if (ViewState["dtPTInsEdit"] != null)
            {
                DataTable dt = (DataTable)ViewState["dtPTInsEdit"];
                if (dt.Rows.Count > 0)
                {
                    //lstPType = new List<string>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        HtmlSelect gddlDepartment =
                          (HtmlSelect)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlDepartment");
                        DropDownList gddlPT =
                          (DropDownList)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlPT");
                        DropDownList gddlPTCategory =
                          (DropDownList)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlPTCategory");
                        Label glblBodyPrefix =
                          (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("glblBodyPrefix");
                        TextBox gtxtPTRemarks =
                          (TextBox)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gtxtPTRemarks");

                        Label lblPTCurrent = (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("lblPTCurrent");

                        if (gddlDepartment.Items.Count > 1)
                        {
                            if (dt.Rows[i]["gddlDepartment"].ToString() != "" || dt.Rows[i]["gddlDepartment"].ToString() != "0")
                                gddlDepartment.SelectedIndex = gddlDepartment.Items.IndexOf(gddlDepartment.Items.FindByValue(dt.Rows[i]["gddlDepartment"].ToString().Trim()));
                            else
                                gddlDepartment.SelectedIndex = -1;
                        }
                        else
                            gddlDepartment.SelectedIndex = -1;

                        if (gddlPT.Items.Count > 1)
                        {
                            if (dt.Rows[i]["gddlPT"].ToString() != "" || dt.Rows[i]["gddlPT"].ToString() != "0")
                                gddlPT.SelectedIndex = gddlPT.Items.IndexOf(gddlPT.Items.FindByValue(dt.Rows[i]["gddlPT"].ToString().Trim()));
                            else
                                gddlPT.SelectedIndex = -1;
                        }
                        else
                            gddlPT.SelectedIndex = -1;

                        if (gddlPTCategory.Items.Count > 1)
                        {
                            if (dt.Rows[i]["gddlPTCategory"].ToString() != "" || dt.Rows[i]["gddlPTCategory"].ToString() != "0")
                                gddlPTCategory.SelectedIndex = gddlPTCategory.Items.IndexOf(gddlPTCategory.Items.FindByValue(dt.Rows[i]["gddlPTCategory"].ToString().Trim()));
                            else
                                gddlPTCategory.SelectedIndex = -1;
                        }
                        else
                            gddlPTCategory.SelectedIndex = -1;

                        glblBodyPrefix.Text = dt.Rows[i]["glblBodyPrefix"].ToString();
                        gtxtPTRemarks.Text = dt.Rows[i]["gtxtPTRemarks"].ToString();
                        lblPTCurrent.Text = dt.Rows[i]["lblPTCurrent"].ToString();
                        rowIndex++;
                    }
                }
            }
        }
        private void SetRowData()
        {
            int rowIndex = 0;

            if (ViewState["dtPTInsEdit"] != null)
            {
                DataTable dtPTInsEdit = (DataTable)ViewState["dtPTInsEdit"];
                DataRow dr = null;
                if (dtPTInsEdit.Rows.Count > 0)
                {
                    for (int i = 1; i <= dtPTInsEdit.Rows.Count; i++)
                    {
                        HtmlSelect gddlDepartment =
                          (HtmlSelect)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlDepartment");
                        DropDownList gddlPT =
                          (DropDownList)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlPT");
                        DropDownList gddlPTCategory =
                          (DropDownList)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gddlPTCategory");
                        Label glblBodyPrefix =
                          (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("glblBodyPrefix");
                        TextBox gtxtPTRemarks =
                          (TextBox)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("gtxtPTRemarks");

                        Label lblPTCurrent = (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("lblPTCurrent");
                        //Label glblHiddenDesc = (Label)gvwPTInsEdit.Rows[rowIndex].Cells[1].FindControl("glblHiddenDesc"); 

                        if (gddlDepartment.SelectedIndex > 0 && gddlPT.SelectedIndex > 0)
                        {
                            dr = dtPTInsEdit.NewRow();
                            dr["RowNumber"] = i + 1;

                            dtPTInsEdit.Rows[i - 1]["gddlDepartment"] = Request.QueryString["DeptValue"].ToString().Trim().ToString();
                            dtPTInsEdit.Rows[i - 1]["gddlPT"] = gddlPT.SelectedValue.ToString();
                            dtPTInsEdit.Rows[i - 1]["gddlPTCategory"] = gddlPTCategory.SelectedValue.ToString();
                            dtPTInsEdit.Rows[i - 1]["glblBodyPrefix"] = glblBodyPrefix.Text;
                            dtPTInsEdit.Rows[i - 1]["gtxtPTRemarks"] = gtxtPTRemarks.Text;
                            dtPTInsEdit.Rows[i - 1]["lblPTCurrent"] = lblPTCurrent.Text;
                            //dtPTInsEdit.Rows[i - 1]["glblHiddenDesc"] = glblHiddenDesc.Text;
                            // =======================
                            rowIndex++;
                        }
                    }

                    ViewState["dtPTInsEdit"] = dtPTInsEdit;
                    //gvwPTInsEdit.DataSource = dtdtPTInsEdit;
                    //gvwPTInsEdit.DataBind();
                }
            }
            else
            {
                ShowMessage("ViewState is null");
            }
            //SetPreviousData();
        }
        private void EnableDisableBasedOnRole()
        {
            #region Enable/Disable buttons based on role

            //bool allowSave = ptsQueryService.IsSaveAllowed(int.Parse(Global.RoleID));
            bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQuerySavebutton);
            if (allowSave)
                SaveButton.Enabled = true;
            else
                SaveButton.Enabled = false;

            //bool allowCloseSignal=ptsQueryService.IsCloseSignalAllowed(int.Parse(Global.RoleID));
            bool allowCloseSignal = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryCloseButton);

            if (allowCloseSignal)
                CloseButton.Enabled = true;
            else
                CloseButton.Enabled = false;

            bool allowResolveQuery = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryResolveButton);

            if (allowResolveQuery)
                QueryResolveButton.Enabled = true;
            else
                QueryResolveButton.Enabled = false;

            #endregion
        }
        private void GetCloseStatusNew(string strCust, int CloseStatus)
        {
            roleID = ptsQueryService.GetRoleIDByUserName(userName, strCust);
            if (CloseStatus == 1)
            {
                SaveButton.Enabled = false;
                ZipButton.Enabled = false;  // RMT#11Dec2014
                CloseButton.Enabled = false;
                // Added by Ram
                bool allowResolveQuery = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryResolveButton);

                if (allowResolveQuery)
                    QueryResolveButton.Enabled = true;
                else
                    QueryResolveButton.Enabled = false;

                for (int i = 0; i < gvwPTInsEdit.Rows.Count; i++)
                {
                    Button gbtnAddNew = (Button)gvwPTInsEdit.Rows[i].FindControl("gbtnAddNew");
                    gbtnAddNew.Enabled = false;

                    Button gbtnDel = (Button)gvwPTInsEdit.Rows[i].FindControl("gbtnDel");
                    gbtnDel.Enabled = false;
                }
            }
            else if (CloseStatus == 0)
            {
                SaveButton.Enabled = false;
                ZipButton.Enabled = true;
                // Add by Ram
                bool allowClose = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryCloseButton);
                if (allowClose)
                    CloseButton.Enabled = true;
                else
                    CloseButton.Enabled = false;

                QueryResolveButton.Enabled = false;
            }
            else
            {
                bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQuerySavebutton);
                if (allowSave)
                {
                    SaveButton.Enabled = true;
                    ZipButton.Enabled = false;
                }
                else
                {
                    SaveButton.Enabled = false;
                    ZipButton.Enabled = true;
                }
                //ZipButton.Enabled = true;//pks 06Mar2015
                CloseButton.Enabled = false;
                QueryResolveButton.Enabled = false;
            }
        }
        protected void lbtnZipFile_Click(object sender, EventArgs e)
        {
            try
            {
                MessageLabel.Text = string.Empty;
                string ZipFilePath = string.Empty;
                if (Session["zipFile"] != null)
                    ZipFilePath = Session["zipFile"].ToString().Trim(); // @" \\192.168.37.15\macipd\Track\PTSEntry\xml\PageProof First Round\\IOPP_IOPPTest_400015_PageProof First Round_spadmin.zip"; 
                if (File.Exists(ZipFilePath))
                {
                    //ZipFilePath = ZipFilePath.Replace(" ", string.Empty);
                    Response.Redirect("DownloadFile.aspx?file=" + ZipFilePath, false);
                }
            }
            catch
            {
            }
        }
        private bool DownloadableProduct_Tracking(string fname)
        {
            System.IO.FileInfo FileName = new System.IO.FileInfo(fname);
            FileStream myFile = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            //Reads file as binary values
            BinaryReader _BinaryReader = new BinaryReader(myFile);

            //Ckeck whether user is eligible to download the file

            //Check whether file exists in specified location
            if (FileName.Exists)
            {
                try
                {
                    long startBytes = 0;
                    string lastUpdateTiemStamp = System.IO.File.GetLastWriteTimeUtc(fname).ToString("r");
                    string str = System.IO.Path.GetFileName(fname);
                    string _EncodedData = HttpUtility.UrlEncode(str, Encoding.UTF8) + lastUpdateTiemStamp;

                    Response.Clear();
                    Response.Buffer = false;
                    Response.AddHeader("Accept-Ranges", "bytes");
                    Response.AppendHeader("ETag", "\"" + _EncodedData + "\"");
                    Response.AppendHeader("Last-Modified", lastUpdateTiemStamp);
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName.Name);
                    Response.AddHeader("Content-Length", (FileName.Length - startBytes).ToString());
                    Response.AddHeader("Connection", "Keep-Alive");
                    Response.ContentEncoding = Encoding.UTF8;

                    //Send data
                    _BinaryReader.BaseStream.Seek(startBytes, SeekOrigin.Begin);

                    //Dividing the data in 1024 bytes package
                    int maxCount = (int)Math.Ceiling((FileName.Length - startBytes + 0.0) / 1024);

                    //Download in block of 1024 bytes
                    int i;
                    for (i = 0; i < maxCount && Response.IsClientConnected; i++)
                    {
                        Response.BinaryWrite(_BinaryReader.ReadBytes(1024));
                        Response.Flush();
                    }

                    // if blocks transfered not equals total number of blocks
                    if (i < maxCount)
                        return false;

                    return true;

                }
                catch
                {
                    return false;
                }
                finally
                {
                    _BinaryReader.Close();
                    myFile.Close();
                    _BinaryReader = null;
                    FileName = null;
                    GC.Collect();

                    //* delete temporary folders *//
                    string FileCopyPath = ConfigurationManager.AppSettings["DownloadPath"]; //* source to download
                    string username = Request.Params["username"];
                    if (string.IsNullOrEmpty(username))
                    {
                        username = "Anything";
                    }

                    if (Directory.Exists(FileCopyPath + username))
                    { Directory.Delete(FileCopyPath + username, true); }

                    //* delete temporary zip file *//
                    if (File.Exists(FileCopyPath + username + ".zip"))
                    { File.Delete(FileCopyPath + username + ".zip"); } //* delete zip file *//

                    Response.End();
                }
            }

            return false;
        }
        private DataTable GetProblemTypeDDL(int CustomerId, int DeptId, int SubStageID)
        {
            DataTable dtProblemType = new DataTable();
            dtProblemType = ptsQueryService.GetProblemTypesByStage(DeptId, CustomerId, SubStageID);

            //gddlPT.DataSource = dtProblemType;
            //gddlPT.DataValueField = Global.DB_FIELD_ID.Trim();
            //gddlPT.DataTextField = Global.DB_FIELD_NAME.Trim();
            //gddlPT.DataBind();
            //gddlPT.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

            return dtProblemType;
        }
        private DataTable GetProblemTypeCategoryDDL(string code, int CustomerId, int DepartmentId, int SubStageID)
        {
            DataTable dtPTCategory = new DataTable();
            dtPTCategory = ptsQueryService.GetProblemTypeCategoryByProbelmType(code, CustomerId, DepartmentId, SubStageID);

            //ddlProblemTypeCategory.DataSource = dtPTCategory;
            //ddlProblemTypeCategory.DataValueField = Global.DB_FIELD_ID.Trim();
            //ddlProblemTypeCategory.DataTextField = Global.DB_FIELD_NAME.Trim();
            //ddlProblemTypeCategory.DataBind();
            //ddlProblemTypeCategory.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

            return dtPTCategory;
        }
        private string GetPTSDesc(string PTypeId)
        {
            string strDesc = string.Empty;
            try
            {
                // =======================
                // GetPTSConfiguration
                string strPTypeHeader = string.Empty;
                DataTable dtPTypeHeader = new DataTable();
                dtPTypeHeader = ptsQueryService.GetPTSConfiguration(Convert.ToInt32(Session["Stage"].ToString().Trim()), Session["Customer"].ToString().Trim());
                if (dtPTypeHeader != null)
                    if (dtPTypeHeader.Rows.Count > 0)
                        strPTypeHeader = dtPTypeHeader.Rows[0]["PTypeHeader"].ToString().Trim();
                Session["strPTypeHeader"] = strPTypeHeader;
                // =======================

                // Load Desc 

                DataTable dtProblemTypeDetails = (DataTable)Session["dtProblemTypeDetails"];
                if (dtProblemTypeDetails != null)
                {
                    if (dtProblemTypeDetails.Rows.Count > 0)
                    {
                        // ================
                        string expression;
                        expression = "ProblemTypeID = " + PTypeId; //  gddlPT.SelectedValue;
                        DataRow[] foundRows;
                        foundRows = dtProblemTypeDetails.Select(expression);
                        if (foundRows.Length > 0)
                        {
                            if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                strDesc = foundRows[0]["Short_Desc"].ToString().Trim();
                            else
                                strDesc = foundRows[0]["Long_Desc"].ToString().Trim();

                        }
                        // ================
                    }
                }
            }
            catch { throw; }
            return strDesc;
        }
        //ADded by Sathish.J on 14-05-2018
        int GetColumnIndexByName(GridViewRow row, string columnName)
        {
            int columnIndex = 0;
            Grid_TempChek = 0;
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.ContainingField is TemplateField)
                    if (((TemplateField)cell.ContainingField).HeaderText.Equals(columnName))
                    { Grid_TempChek = 1; break; }
                columnIndex++; // keep adding 1 while we don't have the correct name
            }
            return columnIndex;
        }

        protected void gvwPTInsEdit_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                DataTable dtDepartment = new DataTable();
                BizPTSQuery ptsQuery = new BizPTSQuery();





                if (e.Row.RowType == DataControlRowType.Header)
                {
                    string _quryt = Session["queryType"].ToString().Trim();
                    if (_quryt == "INSTRUCTION")
                    {

                        int index_Remarks = GetColumnIndexByName(e.Row, "Remarks");
                        e.Row.Cells[index_Remarks].Text = "Instructions";
                    }
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //ADded by Sathish.J on 14-05-2018
                    string _quryt = Session["queryType"].ToString().Trim();
                    if (_quryt == "INSTRUCTION")
                    {
                        int index_Type = GetColumnIndexByName(e.Row, "Problem Type");
                        int index = GetColumnIndexByName(e.Row, "Problem Category");
                        int index_Remarks = GetColumnIndexByName(e.Row, "Remarks");
                        e.Row.Cells[index_Type].Visible = false;
                        e.Row.Cells[index].Visible = false;
                        if (Grid_TempChek == 1)
                        {
                            gvwPTInsEdit.Columns[index].Visible = false; gvwPTInsEdit.Columns[index_Type].Visible = false; //gvwPTInsEdit.Columns[index_Remarks].HeaderText = "Instructions"; 
                        }
                    }


                    Label lblPTCurrent = (Label)e.Row.FindControl("lblPTCurrent");
                    lstPType = (List<string>)Session["lstPType"];
                    if (lblPTCurrent.Text.Trim() != "") lstPType.Remove(lblPTCurrent.Text.Trim());
                    Session["lstPType"] = lstPType;
                    //=================
                    // Load Problem type based on department

                    int idxcat = e.Row.RowIndex + 1;
                    DataTable dtProbCategory = new DataTable();
                    int CustomerID = Convert.ToInt32(Convert.ToInt32(Session["Customer"]).ToString().Trim());
                    int StageID = Convert.ToInt32(Convert.ToInt32(Session["Stage"]).ToString().Trim());
                    if (CustomerID > 0)
                        dtProbCategory = GetProblemTypeDDL(CustomerID, Convert.ToInt32(Request.QueryString["DeptValue"].ToString()), StageID);
                    if (dtProbCategory != null)
                        ViewState["dtPT"] = dtProbCategory;
                    DropDownList gddlPT = (DropDownList)e.Row.FindControl("gddlPT");
                    //DropDownList gddlPTCat = (DropDownList)gvwPTInsEdit.Rows[idxcat].FindControl("gddlPT");
                    gddlPT.DataSource = dtProbCategory;
                    gddlPT.DataValueField = "FieldId";
                    gddlPT.DataTextField = "FieldName";
                    gddlPT.DataBind();
                    gddlPT.Items.Insert(0, "---Select---");

                    //=================

                    // =================
                    // Load department
                    // =================
                    //dtDepartment = ptsQueryService.GetDepartment();
                    //Commented by anitha
                    HtmlSelect gddlDepartment = (HtmlSelect)e.Row.FindControl("gddlDepartment");
                    //gddlDepartment.DataSource = dtDepartment;
                    //gddlDepartment.DataValueField = "FieldId";
                    //gddlDepartment.DataTextField = "FieldName";
                    //gddlDepartment.DataBind();
                    //Commented by anitha
                    //gddlDepartment.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                    if (_quryt == "INSTRUCTION")
                    {
                        string[] params1 = { CustCode, StageId };
                        SqlHelper sql = new SqlHelper();
                        DataSet DS_dept = sql.GetDataSetFromSP("USP_GetDept_New", params1, 2);
                        gddlDepartment.Items.Insert(0, Request.QueryString["DeptName"].Trim());

                        gddlDepartment.DataSource = DS_dept.Tables[0];
                        gddlDepartment.DataTextField = "FieldName";
                        gddlDepartment.DataValueField = "FieldValue";
                        gddlDepartment.DataBind();
                        //  gddlDepartment.Items.Insert(0, new ListItem("---Select---", "0"));

                    }
                    else
                    {
                        ListItem l = new ListItem(Request.QueryString["DeptName"].Trim(), Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()).ToString(), true);
                        gddlDepartment.Items.Add(l); //.Insert(0, Request.QueryString["DeptName"].Trim());
                        gddlDepartment.Items[0].Value = Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()).ToString();

                        gddlDepartment.SelectedIndex = gddlDepartment.Items.IndexOf(gddlDepartment.Items.FindByValue(Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()).ToString()));


                        gddlDepartment.Disabled = true;

                        //gddlDepartment.Items.Insert(0, new ListItem("---Select---", "0"));
                    }
                    // gddlDepartment.SelectedValue = Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()).ToString();
                    if (ViewState["dtPTInsEdit"] != null)
                    {
                        int idx = e.Row.RowIndex;
                        DataTable dtPTInsEdit = (DataTable)ViewState["dtPTInsEdit"];

                        // =================
                        // Load PT
                        // =================
                        int tDeptId = 0;
                        int.TryParse(dtPTInsEdit.Rows[idx]["gddlDepartment"].ToString(), out tDeptId);

                        DataTable dtPT = new DataTable();
                        int tCustomer = Convert.ToInt32(Session["Customer"]);
                        int tStage = Convert.ToInt32(Session["Stage"]);
                        //string tVolume = Session["Volume"].ToString();
                        //string tIssue = Session["Issue"].ToString();

                        if (ArtCode != "")
                        {
                            if (dtPTInsEdit.Rows[idx][1].ToString() != string.Empty || gddlDepartment.SelectedIndex > 0)
                                dtPT = GetProblemTypeDDL(tCustomer, tDeptId, tStage);
                        }
                        //if (tVolume != "" || tIssue != "")
                        //{
                        //    if (Convert.ToInt32(tVolume) > 0 && Convert.ToInt32(tIssue) > 0)
                        //        dtPT = GetProblemTypeDDL(tCustomer, tDeptId, tStage);
                        //}
                        if (dtPT != null)
                            ViewState["dtPT"] = dtPT;

                        //DropDownList gddlPT = (DropDownList)e.Row.FindControl("gddlPT");
                        //gddlPT.DataSource = dtPT;
                        //gddlPT.DataValueField = "FieldId";
                        //gddlPT.DataTextField = "FieldName";
                        //gddlPT.DataBind();
                        //gddlPT.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                        // =================
                        // Load PTCategory
                        // =================
                        DataTable dtPTCategory = new DataTable();
                        if (dtPTInsEdit.Rows[idx][2].ToString() != string.Empty || tDeptId.ToString() != string.Empty)
                        {
                            DropDownList gddlPTCategory = (DropDownList)e.Row.FindControl("gddlPTCategory");
                            //if (gddlPT.SelectedIndex > 0)
                            if (dtPTInsEdit.Rows[idx][2].ToString() != string.Empty || gddlPT.SelectedIndex > 0)
                            {
                                gddlPT.SelectedIndex = gddlPT.Items.IndexOf(gddlPT.Items.FindByValue(dtPTInsEdit.Rows[idx][2].ToString().Trim()));

                                dtPTCategory = GetProblemTypeCategoryDDL(gddlPT.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")), Convert.ToInt32(Session["Customer"]), tDeptId, tStage);
                                //ViewState["dtPTCategory"] = dtPTCategory;
                                if (dtPTCategory != null)
                                {
                                    gddlPTCategory.DataSource = dtPTCategory;
                                    gddlPTCategory.DataValueField = "FieldId";
                                    gddlPTCategory.DataTextField = "FieldName";
                                    gddlPTCategory.DataBind();
                                    gddlPTCategory.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                                }
                                else
                                    gddlPTCategory.Items.Add(new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                            }
                            else
                                gddlPTCategory.Items.Add(new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                        }

                        // ================================
                        TextBox gtxtPTRemarks = (TextBox)e.Row.FindControl("gtxtPTRemarks");
                        gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&amp;", "&");
                        gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&lt;", "<");
                        gtxtPTRemarks.Text = gtxtPTRemarks.Text.Replace("&gt;", ">");



                        ////if (tDeptId != 0 && dtPTInsEdit.Rows[idx][2].ToString().Trim() != string.Empty)
                        ////{
                        ////    Label lblPTCurrent = (Label)e.Row.FindControl("lblPTCurrent");
                        ////    lblPTCurrent.Text = tDeptId + "|" + dtPTInsEdit.Rows[idx][2].ToString();
                        ////}
                        ////else
                        ////{
                        ////    Label lblPTCurrent = (Label)e.Row.FindControl("lblPTCurrent");
                        ////    lblPTCurrent.Text = "test";
                        ////}
                    }
                }


            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }

        //protected void gddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        //private void getproblemcategory()
        //{
        //    try
        //    {
        //        //DropDownList gddlDepartment = (DropDownList)sender;
        //        GridViewRow row = (GridViewRow)gddlDepartment.Parent.Parent;
        //        int idx = row.RowIndex;
        //        DataTable dtPT = new DataTable();
        //        // ========================================
        //        //if (gddlDepartment.SelectedIndex > 0)
        //        //{
        //            int CustomerID = Convert.ToInt32(Convert.ToInt32(Session["Customer"]).ToString().Trim());
        //            int StageID = Convert.ToInt32(Convert.ToInt32(Session["Stage"]).ToString().Trim());

        //            //int VolumeTemp = 0;
        //            //int.TryParse(Session["Volume"].ToString().Trim(), out VolumeTemp);
        //            //int IssueTemp = 0;
        //            //int.TryParse(Session["Issue"].ToString().Trim(), out IssueTemp);

        //            if (CustomerID > 0)
        //                dtPT = GetProblemTypeDDL(CustomerID, Convert.ToInt32(Request.QueryString["DeptValue"].ToString()), StageID);
        //            //if (VolumeTemp > 0 && IssueTemp > 0)
        //            //    dtPT = GetProblemTypeDDL(CustomerID, Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()), StageID);

        //            if (dtPT != null)
        //                ViewState["dtPT"] = dtPT;

        //            DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[idx].FindControl("gddlPT");
        //            gddlPT.DataSource = dtPT;
        //            gddlPT.DataValueField = "FieldId";
        //            gddlPT.DataTextField = "FieldName";
        //            gddlPT.DataBind();
        //            ////gddlPT.Items.Insert(0, new ListItem("-Select-"));
        //            gddlPT.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        //        //}
        //        //else
        //        //{
        //        //    GridViewRow rowTemp = (GridViewRow)gddlDepartment.Parent.Parent;
        //        //    int idxTemp = rowTemp.RowIndex;
        //        //    Label lblPTCurrent = (Label)rowTemp.FindControl("lblPTCurrent");

        //        //    lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
        //        //    lstPTypeCurrentTemp.Remove(lblPTCurrent.Text.Trim());
        //        //    Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;

        //        //    dtPT = new DataTable();
        //        //    DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[idx].FindControl("gddlPT");
        //        //    gddlPT.DataSource = dtPT;
        //        //    gddlPT.DataValueField = "FieldId";
        //        //    gddlPT.DataTextField = "FieldName";
        //        //    gddlPT.DataBind();
        //        //    gddlPT.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

        //        //    Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[idx].FindControl("glblBodyPrefix");
        //        //    TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[idx].FindControl("gtxtPTRemarks");
        //        //    glblBodyPrefix.Text = string.Empty;
        //        //    gtxtPTRemarks.Text = string.Empty;
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage(ex.Message);
        //    }
        //}

        protected void gddlPT_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList gddlPT = (DropDownList)sender;
                GridViewRow row = (GridViewRow)gddlPT.Parent.Parent;
                int idx = row.RowIndex;
                string StageID = Convert.ToInt32(Session["Stage"]).ToString().Trim();
                HtmlSelect gddlDepartment = (HtmlSelect)gvwPTInsEdit.Rows[idx].FindControl("gddlDepartment");
                DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[idx].FindControl("gddlPTCategory");
                //Button gbtnAddNew = (Button)row.FindControl("gbtnAddNew");
                //Button gbtnDel = (Button)row.FindControl("gbtnDel");

                Button gbtnAddNew = (Button)gvwPTInsEdit.Rows[gvwPTInsEdit.Rows.Count - 1].FindControl("gbtnAddNew");
                Button gbtnDel = (Button)gvwPTInsEdit.Rows[gvwPTInsEdit.Rows.Count - 1].FindControl("gbtnDel");

                Label glblBodyPrefix = (Label)row.FindControl("glblBodyPrefix");
                TextBox gtxtPTRemarks = (TextBox)row.FindControl("gtxtPTRemarks");
                Label lblPTCurrent = (Label)row.FindControl("lblPTCurrent");
                //Label glblHiddenDesc = (Label)row.FindControl("glblHiddenDesc");

                if (gddlPT.SelectedIndex > 0)
                {
                    lstPType = (List<string>)Session["lstPType"];
                    lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];

                    //if (lstPType.Contains(gddlPT.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim()) == true)

                    //Commented by Sathish.J on 22-June-2018s
                    //if (lstPType.Contains(Request.QueryString["DeptValue"].ToString().Trim() + "|" + gddlPT.SelectedValue))
                    //{
                    //    lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                    //    lstPTypeCurrentTemp.Remove(lblPTCurrent.Text.Trim());
                    //    Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;

                    //    //ShowMessage("Problem Type already exists in the saved list.");
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Problem Type already exists in the saved list.', 'Required', 'top', 'right');", true);
                    //    gddlPT.SelectedIndex = -1;
                    //    gddlPTCategory.SelectedIndex = -1;
                    //    glblBodyPrefix.Text = string.Empty;
                    //    gtxtPTRemarks.Text = string.Empty;
                    //    gbtnAddNew.Enabled = false;
                    //    //gbtnDel.Enabled = false;
                    //}
                    //else

                    if (lstPTypeCurrentTemp.Contains(Request.QueryString["DeptValue"].ToString().Trim() + "|" + gddlPT.SelectedValue))
                    {
                        ////lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                        ////lstPTypeCurrentTemp.Remove(lblPTCurrent.Text.Trim());
                        ////Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;

                        //ShowMessage("Problem Type already exists in the current list.");
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Problem Type already exists in the current list.', 'Required', 'top', 'right');", true);
                        gddlPT.SelectedIndex = -1;
                        gddlPTCategory.SelectedIndex = -1;
                        glblBodyPrefix.Text = string.Empty;
                        gtxtPTRemarks.Text = string.Empty;
                        gbtnAddNew.Enabled = false;
                        //gbtnDel.Enabled = false;
                    }
                    else
                    {
                        //lstPType = (List<string>)Session["lstPType"];
                        //lstPType.Add(gddlDepartment.SelectedValue + "|" + gddlPT.SelectedValue);//.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim());
                        //Session["lstPType"] = lstPType;

                        lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                        lstPTypeCurrentTemp.Remove(lblPTCurrent.Text.Trim());
                        lstPTypeCurrentTemp.Add(Request.QueryString["DeptValue"].ToString().Trim() + "|" + gddlPT.SelectedValue);//.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim());
                        Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;

                        lblPTCurrent.Text = Request.QueryString["DeptValue"].ToString().Trim() + "|" + gddlPT.SelectedValue;
                        DataTable dtPTCat = new DataTable();
                        dtPTCat = GetProblemTypeCategoryDDL(gddlPT.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")), Convert.ToInt32(Session["Customer"]), Convert.ToInt32(Request.QueryString["DeptValue"].ToString().Trim()), Convert.ToInt32(Session["Stage"].ToString().Trim()));
                        //if (dtPTCat != null)
                        //    ViewState["dtPTCategory"] = dtPTCat;



                        gddlPTCategory.DataSource = dtPTCat;
                        gddlPTCategory.DataValueField = "FieldId";
                        gddlPTCategory.DataTextField = "FieldName";
                        gddlPTCategory.DataBind();
                        gddlPTCategory.Items.Insert(0, "----SELECT----");
                        //gddlPTCategory.SelectedItem.Text = dtPTCat.Rows[0]["fieldname"].ToString();
                        gddlPTCategory.SelectedIndex = 1;

                        //Commented by anitha
                        string strDesc = GetPTSDesc(gddlPT.SelectedValue.ToString().Trim());
                        //Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[idx].FindControl("glblBodyPrefix");
                        glblBodyPrefix.Text = strDesc;

                        string strDescLong = ptsQueryService.GetProblemTypeDescription(StageID, gddlPT.SelectedItem.Text.Split('-')[0].ToString().Trim(), Session["Customer"].ToString().Trim(), Request.QueryString["DeptValue"].ToString().Trim());
                        //string strDescLong = GetPTSDescLong(gddlPT.SelectedValue.ToString().Trim());
                        gtxtPTRemarks.Text = strDescLong.Trim();

                        gbtnAddNew.Enabled = true;
                        gbtnDel.Enabled = true;
                        //Commented by anitha
                    }
                }
                else
                {
                    // =================================
                    lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                    lstPTypeCurrentTemp.Remove(lblPTCurrent.Text.Trim());
                    //lstPTypeCurrentTemp.Add(gddlDepartment.SelectedValue + "|" + gddlPT.SelectedValue);//.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim());
                    Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;
                    //lblPTCurrent.Text = gddlDepartment.SelectedValue + "|" + gddlPT.SelectedValue;

                    lstPType = (List<string>)Session["lstPType"];
                    lstPType.Remove(lblPTCurrent.Text.Trim());
                    Session["lstPType"] = lstPType;
                    // =================================
                    gddlPTCategory.SelectedIndex = -1;
                    glblBodyPrefix.Text = string.Empty;
                    gtxtPTRemarks.Text = string.Empty;
                    gbtnAddNew.Enabled = false;
                    //gbtnDel.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }


        protected void gddlPTCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // DropDownList gddlPTCategory = (DropDownList)sender;
                // GridViewRow row = (GridViewRow)gddlPTCategory.Parent.Parent;
                // TextBox gtxtPTRemarks = (TextBox)row.FindControl("gtxtPTRemarks");
                //// string strDescLong = ptsQueryService.GetProblemTypeDescription(stage, gddlPT.SelectedItem.Text.Split('-')[0].ToString().Trim());
                // gtxtPTRemarks.Text = strDescLong.Trim();
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        protected void gbtnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                Button gbtnAddNew = (Button)sender;
                GridViewRow row = (GridViewRow)gbtnAddNew.Parent.Parent;
                int idx = row.RowIndex;
                DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[idx].FindControl("gddlPT");
                DropDownList gddlDepartment = (DropDownList)gvwPTInsEdit.Rows[idx].FindControl("gddlDepartment");
                if (gddlPT.SelectedIndex > 0)
                {
                    AddNewRow();

                    ////lstPType = (List<string>)Session["lstPType"];
                    ////lstPType.Add(gddlPT.SelectedItem.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim());
                    ////Session["lstPType"] = lstPType;

                    //lstPType = (List<string>)Session["lstPType"];
                    //lstPType.Add(gddlDepartment.SelectedValue + "|" + gddlPT.SelectedValue);//.Text.ToString().Trim().Substring(0, gddlPT.SelectedItem.Text.ToString().Trim().IndexOf("--")).Trim());
                    //Session["lstPType"] = lstPType;

                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        protected void gbtnDel_Click(object sender, EventArgs e)
        {

            try
            {
                Button gbtnDel = (Button)sender;
                GridViewRow row = (GridViewRow)gbtnDel.Parent.Parent;
                int rowIndex = row.RowIndex;

                // SetRowData(rowIndex);
                SetRowDataFromGrid(rowIndex);
                if (ViewState["dtPTInsEdit"] != null)
                {
                    DataTable dt = (DataTable)ViewState["dtPTInsEdit"];
                    DataRow drPTInsEdit = null;

                    Label lblPTCurrent = (Label)row.FindControl("lblPTCurrent");


                    lstPType = (List<string>)Session["lstPType"];
                    lstPTypeCurrentTemp = (List<string>)Session["lstPTypeCurrentTemp"];
                    lstPTypeCurrentTemp.Remove(lblPTCurrent.Text.Trim());
                    Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;

                    if (dt.Rows.Count > 0)
                    {
                        //if (dt.Rows.Count > rowIndex)
                        //    dt.Rows.Remove(dt.Rows[rowIndex]);

                        // drPTInsEdit = dt.NewRow();

                        ViewState["dtPTInsEdit"] = dt;
                        gvwPTInsEdit.DataSource = dt;
                        gvwPTInsEdit.DataBind();

                        SetPreviousData();

                        Button gbtnAddNew = new Button();
                        if (dt.Rows.Count > 0)
                        {
                            gbtnAddNew = (Button)gvwPTInsEdit.Rows[dt.Rows.Count - 1].FindControl("gbtnAddNew");
                            gbtnAddNew.Enabled = true;
                        }
                    }

                    else
                    {
                        AddFirstRow();

                        DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[rowIndex].FindControl("gddlPT");
                        DropDownList gddlDepartment = (DropDownList)gvwPTInsEdit.Rows[rowIndex].FindControl("gddlDepartment");

                        //gddlDepartment = (DropDownList)gvwPTInsEdit.Rows[rowIndex].FindControl("gddlDepartment");
                        gddlPT = (DropDownList)gvwPTInsEdit.Rows[rowIndex].FindControl("gddlPT");
                        DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[rowIndex].FindControl("gddlPTCategory");

                        Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[rowIndex].FindControl("glblBodyPrefix");
                        TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[rowIndex].FindControl("gtxtPTRemarks");
                        Button gbtnAddNew = (Button)gvwPTInsEdit.Rows[rowIndex].FindControl("gbtnAddNew");

                        if (gddlPT.SelectedIndex > 0)
                        {
                            lstPTypeCurrentTemp.Remove(Request.QueryString["DeptValue"].ToString().Trim() + "|" + gddlPT.SelectedValue.ToString().Trim());
                            Session["lstPTypeCurrentTemp"] = lstPTypeCurrentTemp;
                        }

                        gddlDepartment.SelectedIndex = -1;
                        gddlPT.SelectedIndex = -1;
                        gddlPTCategory.SelectedIndex = -1;
                        glblBodyPrefix.Text = string.Empty;
                        gtxtPTRemarks.Text = string.Empty;
                        gbtnAddNew.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }


        }
        private void SetRowDataFromGrid(int delRowIndex)
        {
            try
            {
                int rowIndex = 0;

                if (ViewState["dtPTInsEdit"] != null)
                {
                    DataTable dtPTInsEdit = (DataTable)ViewState["dtPTInsEdit"];
                    dtPTInsEdit.Rows.Clear();
                    DataRow dr = null;
                    for (int i = 0; i < gvwPTInsEdit.Rows.Count; i++)
                    {
                        if (delRowIndex != i)
                        {
                            HtmlSelect gddlDepartment =
                              (HtmlSelect)gvwPTInsEdit.Rows[i].FindControl("gddlDepartment");
                            DropDownList gddlPT =
                              (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPT");
                            DropDownList gddlPTCategory =
                              (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPTCategory");
                            TextBox gtxtPTRemarks =
                              (TextBox)gvwPTInsEdit.Rows[i].FindControl("gtxtPTRemarks");
                            Label glblBodyPrefix =
                              (Label)gvwPTInsEdit.Rows[i].FindControl("glblBodyPrefix");

                            Label lblPTCurrent = (Label)gvwPTInsEdit.Rows[i].FindControl("lblPTCurrent");

                            Button gbtnDel = (Button)gvwPTInsEdit.Rows[i].FindControl("gbtnDel");
                            gbtnDel.Enabled = true;

                            //if (gddlDepartment.SelectedIndex > 0 && gddlPT.SelectedIndex > 0)
                            if (gddlPT.SelectedIndex > 0)
                            {
                                dr = dtPTInsEdit.NewRow();
                                dr["RowNumber"] = rowIndex;// +1;

                                //dtPTInsEdit.Rows[i - 1]["RowNumber"] = i;
                                dr["gddlDepartment"] = Request.QueryString["DeptValue"].ToString().Trim().ToString();
                                dr["gddlPT"] = gddlPT.SelectedValue.ToString();
                                dr["gddlPTCategory"] = gddlPTCategory.SelectedValue.ToString();
                                dr["glblBodyPrefix"] = glblBodyPrefix.Text.Trim();
                                dr["gtxtPTRemarks"] = gtxtPTRemarks.Text;
                                dr["lblPTCurrent"] = lblPTCurrent.Text;

                                dtPTInsEdit.Rows.Add(dr);
                                //rowIndex++;
                            }
                            rowIndex++;
                        }
                        else
                            rowIndex--;
                    }
                    ViewState["dtPTInsEdit"] = dtPTInsEdit;
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        private string GetPTSDescLong(string PTypeId)
        {
            string strDesc = string.Empty;
            try
            {
                DataTable dtProblemTypeDetails = (DataTable)Session["dtProblemTypeDetails"];
                if (dtProblemTypeDetails != null)
                {
                    if (dtProblemTypeDetails.Rows.Count > 0)
                    {
                        // ================
                        string expression;
                        expression = "ProblemTypeID = " + PTypeId;
                        DataRow[] foundRows;
                        foundRows = dtProblemTypeDetails.Select(expression);
                        if (foundRows.Length > 0)
                        {
                            strDesc = foundRows[0]["Long_Desc"].ToString().Trim();
                        }
                        // ================
                    }
                }
            }
            catch { throw; }
            return strDesc;
        }
        //Commented By sathish.J on 28-07-2017 for Query raise directly
        //  protected void SaveButton_Click(object sender, EventArgs e)
        public void SaveButton_Click()
        {
            try
            {
                bool isMasterCopyJournal = false;
                bool isDeptSelected = false;

                MessageLabel.Text = string.Empty;
                PTSProblem ptsProblem = new PTSProblem();
                string validationMessage = string.Empty;

                //============================
                ptsProblem.CustomerCode = Convert.ToInt32(Session["Customer"].ToString().Trim());
                ptsProblem.JournalCode = Convert.ToInt32(Session["Journal"].ToString().Trim());
                ptsProblem.Article = Session["Article"].ToString().Trim().Split(',')[Temp_Round];
                // ptsProblem.Volume = Session["Volume"].ToString().Trim();
                //ptsProblem.Issue = Session["Issue"].ToString().Trim();
                ptsProblem.StageID = Convert.ToInt32(Session["Stage"].ToString().Trim());
                ptsProblem.Stage = Session["StageName"].ToString().Trim();
                int Dept_value = 0;
                if (Request.QueryString["DeptValue"] != null)
                {
                    Dept_value = Convert.ToInt32(Request.QueryString["DeptValue"]);
                }
                userName = Session["userName"].ToString().Trim();
                ptsProblem.DepartmentId = Dept_value;
                ptsProblem.ProblemTypeCode = "";
                ptsProblem.QuerySavedBy = Session["userName"].ToString().Trim();
                ptsProblem.QueryClosedBy = string.Empty;
                ptsProblem.UserName = Session["userName"].ToString().Trim();

                // int temp = 0;
                // int.TryParse(Session["RoundCount"].ToString(), out temp);
                // ptsProblem.RoundCount = temp;
                //============================
                string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
                tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, ptsProblem.CustomerCode);
                tempZipFilePath = tempZipFilePath.Replace("[stage]", ptsProblem.Stage);

                string problemID = string.Empty;
                problemID = ptsQueryService.GetPTSProblemID();
                ptsProblem.ProblemID = problemID;

                tempZipFileName = Request.QueryString["CustName"].Trim() + "_" + Request.QueryString["JourName"].Trim() + "_" + Session["Article"].ToString().Trim().Split(',')[Temp_Round] + "_" + Request.QueryString["StageName"].Trim() + "_" + Session["userName"].ToString().Trim();// + problemID.ToString().Trim()
                tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                string zipFileName = tempZipFilePath + @"\" + tempZipFileName + ".zip";
                ptsProblem.ZipFilePath = zipFileName;
                Session["zipFile"] = zipFileName;
                //============================
                ptsProblem.TaskClosed = false;
                // ============================
                if (File.Exists(zipFileName))
                    ptsProblem.ZipFileUpload = true;
                else
                    ptsProblem.ZipFileUpload = false;
                ptsProblem.ZipFilePath = zipFileName;

                hfZipFilePath.Value = zipFileName;

                ptsProblem.Sequence = 0;

                // code added by prasanta start
                if (Session["zipFile"] != null)
                {
                    ptsProblem.ZipFilePath = Session["zipFile"].ToString().Trim();
                    lblZipFile.Text = Session["zipFile"].ToString().Trim();
                    if (File.Exists(Session["zipFile"].ToString().Trim()))
                    {
                        lbtnZipFile.Text = "(Download)";
                    }
                    else
                    {
                        lbtnZipFile.Text = string.Empty;
                        lblZipFile.Text = string.Empty;
                    }
                }
                else
                {
                    ptsProblem.ZipFilePath = string.Empty;
                    lblZipFile.Text = string.Empty;
                }

                // =======================// =======================
                // GetPTSConfiguration
                string totalRemarks = string.Empty;
                string strPTypeHeader = string.Empty;
                strPTypeHeader = Session["strPTypeHeader"].ToString().Trim();
                // =======================// =======================
                StringBuilder sb = new StringBuilder();
                sb.Append("<Details>");
                string tPTRemarks = string.Empty, Temp_Remarks = "";
                for (int i = 0; i < gvwPTInsEdit.Rows.Count; i++)
                {
                    sb.Append("<ProblemType>");
                    DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPT");
                    if (gddlPT.SelectedIndex > 0)
                    {
                        string strPTCode = gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.IndexOf("--")).Trim();
                        sb.Append("<ProblemTypeId>" + gddlPT.SelectedValue.ToString().Trim() + "</ProblemTypeId>");
                        sb.Append("<ProblemTypeCode>" + strPTCode + "</ProblemTypeCode>");

                        DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPTCategory");
                        sb.Append("<ProblemCategoryId>" + gddlPTCategory.SelectedValue.ToString().Trim() + "</ProblemCategoryId>");

                        TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[i].FindControl("gtxtPTRemarks");
                        string rem = Temp_Remarks = gtxtPTRemarks.Text.Trim();
                        rem = rem.Trim().Replace("&", "&amp;");
                        rem = rem.Trim().Replace("<", "&lt;");
                        rem = rem.Trim().Replace(">", "&gt;");
                        //rem = rem.Trim().Replace("$", "&#36;");
                        //sb.Append("<PTCategoryRemark>" + GeneralHelper.RemoveSpecialChars(rem.Trim()) + "</PTCategoryRemark>");
                        sb.Append("<PTCategoryRemark>" + rem.Trim() + "</PTCategoryRemark>");

                        HtmlSelect gddlDepartment = (HtmlSelect)gvwPTInsEdit.Rows[i].FindControl("gddlDepartment");
                        sb.Append("<DepartmentID>" + Request.QueryString["DeptValue"].ToString().Trim().ToString().Trim() + "</DepartmentID>");

                        Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[i].FindControl("glblBodyPrefix");
                        string tPTCategory = string.Empty;

                        if (gddlPTCategory.SelectedIndex > 0)
                            tPTCategory = gddlPTCategory.SelectedItem.Text.Trim();
                        if (gtxtPTRemarks.Text != string.Empty)
                            tPTRemarks = gtxtPTRemarks.Text.Trim();
                        //tPTRemarks = tPTRemarks.Replace("$", "&#36;");
                        // ===================

                        tPTCategory = tPTCategory.Trim() != string.Empty ? (" (" + tPTCategory.Trim() + ") ") : string.Empty;
                        if (gddlPT.SelectedItem.Text.Trim().IndexOf("--") > 0)
                            if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                                gddlPTText = gddlPT.SelectedItem.Text.Trim() != string.Empty ? (" (" + (gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim()) + ") ") : string.Empty;
                            else
                                gddlPTText = gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim();
                        else
                            gddlPTText = string.Empty;

                        if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                        {
                            //gtxtPTRemarksText = gtxtPTRemarks.Text.Trim() != string.Empty ? (" (" + gtxtPTRemarks.Text.Trim() + ") ") : string.Empty;
                            //totalRemarks += "\r\n" + gddlPTText + tPTCategory + glblBodyPrefix.Text.ToString().Trim() + gtxtPTRemarksText;
                            totalRemarks += "\r\n" + gddlPTText + tPTCategory + gtxtPTRemarks.Text.Trim();
                        }
                        else
                            totalRemarks += "\r\n" + gtxtPTRemarks.Text.Trim();
                        //totalRemarks += "\r\n" + glblBodyPrefix.Text.ToString().Trim() + " " + gtxtPTRemarks.Text.Trim();
                        // ===================
                        Button gbtnAddNew = (Button)gvwPTInsEdit.Rows[i].FindControl("gbtnAddNew");

                        if (gbtnAddNew.Enabled)
                            isDeptSelected = true;
                        //else
                        //    ShowMessage("Please select Department");
                        //// =======================// =======================
                        sb.Append("</ProblemType>");
                    }
                    else
                    {
                        isDeptSelected = false;
                    }
                }
                sb.Append("</Details>");


                if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                    totalRemarks = DateTime.Now.ToString("dd-MMM-yyyy") + totalRemarks;
                //ptsProblem.PtsRemarks = GeneralHelper.RemoveSpecialChars(totalRemarks); // need
                ptsProblem.PtsRemarks = totalRemarks;
                Session["totalRemarks"] = ptsProblem.PtsRemarks;


                ViewState["xml"] = sb.ToString().Trim();
                ptsProblem.FileContent = sb.ToString().Trim();
                string _quryt = Session["queryType"].ToString().Trim();
                if (_quryt == "INSTRUCTION")
                {
                    TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[0].FindControl("gtxtPTRemarks");
                    string rem = Temp_Remarks = ptsProblem.PtsRemarks = gtxtPTRemarks.Text.Trim();
                    isDeptSelected = true;
                    string TT = "<Details><ProblemType><ProblemTypeId>1</ProblemTypeId><ProblemTypeCode>1</ProblemTypeCode><ProblemCategoryId>1</ProblemCategoryId><PTCategoryRemark>" + Temp_Remarks.ToString().Trim() + "</PTCategoryRemark><DepartmentID>" + Request.QueryString["DeptValue"].ToString().Trim().ToString().Trim() + "</DepartmentID></ProblemType></Details>";
                    ViewState["xml"] = TT.ToString().Trim();
                    ptsProblem.FileContent = TT.ToString().Trim();
                    // ptsProblem.Proceed_Req = false;
                    ptsProblem.Proceed_Req = Convert.ToString(ProceedWithQuery_DropDownList1.SelectedItem.Text.ToLower()) == "yes" ? true : false;
                }
                else { ptsProblem.Proceed_Req = Convert.ToString(ProceedWithQuery_DropDownList1.SelectedItem.Text.ToLower()) == "yes" ? true : false; }
                // ===============================================
                // SAVE
                // ===============================================
                isMasterCopyJournal = ptsQueryService.IsMasterCopyJournal(Convert.ToInt32(Session["Journal"]));

                if (isMasterCopyJournal)
                    ShowMessage(ConfigurationManager.AppSettings["MasterCopyJournalText"].ToString().Trim());
                else
                    ResultLabel.Text = string.Empty;
                if (isDeptSelected)
                {
                    ptsProblem.QueryType = Session["queryType"].ToString();
                    ptsProblem.intprocessidlist = Get_Deprtments;




                    string msg = ptsQueryService.InsertOrUpdatePTSProblem(ptsProblem);
                    WriteLog(Session["Journal"].ToString().Trim()+"-"+ Session["Article"].ToString().Trim().Split(',')[Temp_Round]+"-"+msg);
                    Session["OpenedRequestId"] = msg.Split('|')[1].ToString();
                    WriteLog(Session["Journal"].ToString().Trim() + "-"+Session["OpenedRequestId"].ToString().Trim());
                    WriteLog(Session["Journal"].ToString().Trim() + "-" + Session["totalRemarks"].ToString().Trim() + Session["userName"].ToString().Trim());
                    //ShowMessage(ptsQueryService.InsertOrUpdatePTSProblem(ptsProblem));
                    ////Response.Redirect("RaiseProblems.aspx");

                    //Commented By sathish.J on 28-07-2017 for Query raise directly
                    //  Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', '" + msg + "', 'Success', 'top', 'right');", true);
                    ZipButton.Enabled = false;
                    SaveButton.Enabled = false;
                    //    CloseButton.Enabled = false;  //Commented By sathish.J on 28-07-2017 for Query raise directly
                    btnCancel.Text = "Back";
                    Session["Redirect_msg"] = msg;

                    WriteLog(Session["Journal"].ToString().Trim() + "-" + Session["Redirect_msg"].ToString().Trim());
                    //Commented By sathish.J on 28-07-2017 for Query raise directly
                    //  if (msg == "Insertion successful.") { Response.Redirect("RaiseProblem.aspx?Username=" + Session["userName"].ToString().Trim() + "&Redirect_msg="+msg,false); }
                    //this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                }
                else
                {
                    //ShowMessage("Please select department of all rows.");
                    //ShowMessage("Please select all departments/problem types");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select problem types', 'Required', 'top', 'right');", true);
                    Status_Check = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Session["ResultMsg"] = string.Empty;

                lstPTypeCurrent = (List<string>)Session["lstPTypeCurrent"];
                lstPType = (List<string>)Session["lstPType"];

                lstPType.AddRange(lstPTypeCurrent);
                Session["lstPType"] = lstPType;

                lstPTypeCurrent.Clear();
                lstPTypeCurrent = (List<string>)Session["lstPTypeCurrent"];

                //Response.Redirect("RaiseProblem.aspx");

                Response.Redirect("RaiseProblems.aspx?Username=" + Session["Encrypted User"].ToString(), false);
                //Response.Redirect("RaiseProblems.aspx?Username=" + System.Web.HttpContext.Current.Session["userName"], false);
                WriteLog("BackButton : " + Session["Encrypted User"].ToString());
              
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        private string Department_selection1()
        {
            string Chapter_selection = "";

            foreach (GridViewRow row in gvwPTInsEdit.Rows)
            {
                GridViewRow gvr = row;
                HtmlSelect gddlDepartmentNew = ((HtmlSelect)gvr.FindControl("gddlDepartment"));
                // }
                int count = gddlDepartmentNew.Items.Count;
                for (int i = 0; i <= count - 1; i++)
                {

                    if (gddlDepartmentNew.Items[i].Selected == true)
                    {
                        Chapter_selection += gddlDepartmentNew.Items[i].Value + ",";
                    }
                }
            }
            Chapter_selection = Chapter_selection.TrimEnd(',');

            return Chapter_selection;
        }
        protected void CloseButton_Click(object sender, EventArgs e)
        {
            Get_Deprtments = ""; string ResultMsg = "";
            Get_Deprtments = Department_selection1();
            bool isDeptSelected = false;
            //  return;
            Temp_Round = 0;
            for (Temp_Round = 0; Temp_Round < Session["Article"].ToString().Trim().Split(',').Length; Temp_Round++)
            {
                SaveButton_Click(); //added by sathish.J on 28-07-2017 for Query raise diectly

                if (Status_Check == true) { return; }
                #region Local Fields

                bool mailstatus = false;
                MessageLabel.Text = string.Empty;
                PTSProblem ptsProblem = new PTSProblem();
                string validationMessage = string.Empty;


                stageID = 0;
                customerCode = 0;
                journalCode = 0;
                article = string.Empty;
                stage = string.Empty;
                int roundCount = 0;
                int OpenedRequestId = 0;




                int.TryParse(Session["Stage"].ToString().Trim(), out stageID);
                int.TryParse(Session["Customer"].ToString().Trim(), out customerCode);
                int.TryParse(Session["Journal"].ToString().Trim(), out journalCode);
                article = Session["Article"].ToString().Trim().Split(',')[Temp_Round];// ArticleDropDown.SelectedValue.ToString().Trim();
                stage = Session["StageName"].ToString().Trim();
                //volume = Session["Volume"].ToString().Trim();
                //issue = Session["Issue"].ToString().Trim();
                //int.TryParse(Session["RoundCount"].ToString().Trim(), out roundCount);
                int.TryParse(Session["OpenedRequestId"].ToString().Trim(), out OpenedRequestId);
                string JournalName = Session["JournalName"].ToString().Trim();
                string CustomerName = Session["CustomerName"].ToString().Trim();

                string piiNumber = string.Empty;
                string formattedPIINumber = string.Empty;

                string ArtvolIssue = "";
                #endregion Local Fields
                WriteLog("AfterSaveButton" + Session["Journal"].ToString().Trim() + "-" + Session["Article"].ToString().Trim().Split(',')[Temp_Round] );
                WriteLog("AfterSaveButton-ReqID" +Session["OpenedRequestId"].ToString().Trim());
                try
                {

                    #region try

                    DataTable dtPTCategory = (DataTable)ViewState["dtPTCategory"];
                    //if (dtPTCategory.Rows.Count > 0)
                    if (gvwPTInsEdit.Rows.Count > 0)
                    {

                        if (article != string.Empty)
                            ArtvolIssue = article;
                        else
                            ArtvolIssue = volume + "-" + issue + "-" + roundCount.ToString().Trim();

                        jobType = JobTypeHidden.Value;



                        string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                        string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
                        //tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);

                        tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);
						tempZipFilePath = tempZipFilePath.Replace("[customer]", CustomerName.Trim()).Replace("[journal]", JournalName.Trim()).Replace("[article]", article.Trim());
                        //using (new Impersonator("iuser", "MPS-chennai", "BMW_i20^Sx4"))
                        //{
                        if (!Directory.Exists(tempZipFilePath))
                            Directory.CreateDirectory(tempZipFilePath);
                        // }
                        string zipFileName = string.Empty;
                        //end of getting the uploaded the zip file path and name

                        DataTable dtProblems = new DataTable();
                        string ptsProblemUser = string.Empty;

                        if (Session["OpenedRequestId"].ToString().Trim() != string.Empty)
                        {
                            int tOpenedRequestId = 0;
                            int.TryParse(Session["OpenedRequestId"].ToString().Trim(), out tOpenedRequestId);
                            ////dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(ddlOpenedRequest.SelectedValue.Trim()));
                            dtProblems = ptsQueryService.GetPTSProblemsByID(tOpenedRequestId);
                        }
                        if (article != string.Empty)
                        {

                            if (dtProblems.Rows.Count > 0)
                            {
                                ptsProblemUser = dtProblems.Rows[0]["username"].ToString().Trim();
                                tempZipFileName = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim();
                                tempZipFileName = tempZipFileName.Replace(" ", string.Empty);
                            }
                            else
                                ptsProblemUser = Global.UserName;

                            WriteLog("AfterSaveButton-ReqID1" + Session["OpenedRequestId"].ToString().Trim());

                            piiNumber = ptsQueryService.GetArticlePIINumber(customerCode, journalCode, article, stage);
                            // formattedPIINumber = GeneralHelper.FormatArticlePIINumber(piiNumber);

                            formattedPIINumber = piiNumber;
                            ptsProblem.Article = article;
                            ptsProblem.Volume = volume;
                            ptsProblem.Issue = issue;
                            ptsProblem.DepartmentId = 0;// Convert.ToInt32(ddlDepartment.SelectedValue); // ?
                            ptsProblem.ProblemID = "0";
                            if (roundCount.ToString().Trim() != "")
                                ptsProblem.RoundCount = Convert.ToInt32(roundCount.ToString().Trim());
                            else
                                ptsProblem.RoundCount = 0;
                        }


                        ptsProblem.StageID = stageID;
                        ptsProblem.Stage = stage;
                        ptsProblem.CustomerCode = customerCode;
                        ptsProblem.JournalCode = journalCode;

                        int tempUserID = 0;
                        int.TryParse(Global.userId, out tempUserID);
                        ptsProblem.QuerySavedBy = ptsProblemUser;
                        ptsProblem.TaskClosed = true;

                        if (File.Exists(tempZipFileName))
                            ptsProblem.ZipFileUpload = true;
                        else
                            ptsProblem.ZipFileUpload = false;

                        ptsProblem.UserName = Session["userName"].ToString().Trim();
                        ptsProblem.QueryClosedBy = Session["userName"].ToString().Trim();

                        int tempUserId = 0;
                        int.TryParse(Global.userId, out tempUserId);

                        ptsProblem.UserID = tempUserId;

                        //Get the xml template and replace the values/
                        WriteLog("AfterSaveButton-ReqID2" + Session["userName"].ToString().Trim());
                        string problemID = string.Empty;
                        if (dtProblems.Rows.Count > 0)
                        {
                            problemID = dtProblems.Rows[0]["ProblemMsgId"].ToString().Trim();
                            ptsProblem.ProblemID = problemID;
                        }
                        DataTable dtConfig = new DataTable();


                        //rmt #9677
                        string tempArt = string.Empty;
                        if (article.Trim() != string.Empty)
                            tempArt = article.Trim();
                        else
                            tempArt = volume.Trim().Trim() + ',' + issue.Trim().Trim() + ',' + roundCount.ToString().Trim();

                        int PTID = Convert.ToInt32(Session["OpenedRequestId"]);
                        if (PTID == 0) { Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please refresh the page', 'Success', 'top', 'right');", true); return; }
                        WriteLog("AfterSaveButton-ReqID3" + Session["userName"].ToString().Trim());
                        dtConfig = ptsQueryService.GetPTSConfiguration(stageID, customerCode.ToString(), Convert.ToInt64(journalCode), tempArt, PTID);//usp_PTS_GetAllPTSConfiguration_Newone
                        string Temp_Remarks = "";
                        WriteLog("AfterSaveButton-ReqID4" + Session["userName"].ToString().Trim());
                        if (dtConfig.Rows.Count > 0)
                        {
                            // =======================// =======================
                            // GetPTSConfiguration
                            string totalRemarks = string.Empty;
                            string strPTypeHeader = string.Empty;
                            strPTypeHeader = Session["strPTypeHeader"].ToString().Trim();
                            // =======================// =======================
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<Details>");
                            for (int i = 0; i < gvwPTInsEdit.Rows.Count; i++)
                            {
                                sb.Append("<ProblemType>");
                                DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPT");
                                string strPTCode = string.Empty;
                                if (gddlPT.SelectedIndex > 0)
                                {
                                    isDeptSelected = true;
                                    strPTCode = gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.IndexOf("--")).Trim();
                                }
                                else
                                {
                                    isDeptSelected = false;
                                    break;
                                }

                                sb.Append("<ProblemTypeId>" + gddlPT.SelectedValue.ToString().Trim() + "</ProblemTypeId>");
                                sb.Append("<ProblemTypeCode>" + strPTCode + "</ProblemTypeCode>");

                                DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPTCategory");
                                sb.Append("<ProblemCategoryId>" + gddlPTCategory.SelectedValue.ToString().Trim() + "</ProblemCategoryId>");

                                TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[i].FindControl("gtxtPTRemarks");
                                string tempRem = gtxtPTRemarks.Text.Trim();
                                tempRem = tempRem.Replace("&", "&amp;");
                                tempRem = tempRem.Replace("<", "&lt;");
                                tempRem = tempRem.Replace(">", "&gt;");
                                //tempRem = tempRem.Replace("$", "&#36;");
                                ////sb.Append("<PTCategoryRemark>" + GeneralHelper.RemoveSpecialChars(tempRem.Trim()) + "</PTCategoryRemark>");
                                sb.Append("<PTCategoryRemark>" + tempRem.Trim() + "</PTCategoryRemark>");

                                HtmlSelect gddlDepartment = (HtmlSelect)gvwPTInsEdit.Rows[i].FindControl("gddlDepartment");
                                //if (gddlDepartment.SelectedIndex > 0)
                                //{
                                isDeptSelected = true;
                                sb.Append("<DepartmentID>" + gddlDepartment.Items[i].Value + "</DepartmentID>");
                                //}
                                //else
                                //{
                                //    isDeptSelected = false;
                                //    break;
                                //}

                                Label glblBodyPrefix = (Label)gvwPTInsEdit.Rows[i].FindControl("glblBodyPrefix");
                                string tPTCategory = string.Empty;
                                string tPTRemarks = string.Empty;
                                if (gddlPTCategory.SelectedIndex > 0)
                                    tPTCategory = gddlPTCategory.SelectedItem.Text.Trim();
                                if (gtxtPTRemarks.Text != string.Empty)
                                    tPTRemarks = gtxtPTRemarks.Text.Trim();

                                // ===================
                                tPTCategory = tPTCategory.Trim() != string.Empty ? (" [" + tPTCategory.Trim() + "] ") : string.Empty;
                                if (gddlPT.SelectedItem.Text.Trim().IndexOf("--") > 0)
                                    gddlPTText = gddlPT.SelectedItem.Text.Trim() != string.Empty ? (" (" + (gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim()) + ") ") : string.Empty;
                                else
                                    gddlPTText = string.Empty;
                                //gddlPTText = gddlPT.SelectedItem.Text.Trim() != string.Empty ? (" (" + (gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.Trim().IndexOf("--")).Trim()) + ") ") : string.Empty;

                                if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                                {
                                    //gtxtPTRemarksText = gtxtPTRemarks.Text.Trim() != string.Empty ? (" (" + gtxtPTRemarks.Text.Trim() + ") ") : string.Empty;
                                    //totalRemarks += "\r\n" + gddlPTText + tPTCategory + glblBodyPrefix.Text.ToString().Trim() + gtxtPTRemarksText;

                                    totalRemarks += "\r\n" + gddlPTText + tPTCategory + gtxtPTRemarks.Text.Trim();
                                }
                                else
                                    totalRemarks += "\r\n" + gtxtPTRemarks.Text.Trim();
                                //totalRemarks += "\r\n" + glblBodyPrefix.Text.ToString().Trim() + " " + gtxtPTRemarks.Text.Trim();

                                sb.Append("</ProblemType>");
                            }
                            sb.Append("</Details>");

                            if (strPTypeHeader.ToLower().ToString().Trim() == "true" || strPTypeHeader.ToString().Trim() == "1")
                                totalRemarks = DateTime.Now.ToString("dd-MMM-yyyy") + totalRemarks;

                            //ptsProblem.PtsRemarks = GeneralHelper.RemoveSpecialChars(totalRemarks); // need
                            ptsProblem.PtsRemarks = totalRemarks;
                            ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Trim().Replace("&", "&amp;");
                            ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace("<", "&lt;");
                            ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace(">", "&gt;");
                            ////ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace("$", "&#36;");

                            string _quryt = Session["queryType"].ToString().Trim();
                            if (_quryt == "INSTRUCTION")
                            {
                                TextBox gtxtPTRemarksNew = (TextBox)gvwPTInsEdit.Rows[0].FindControl("gtxtPTRemarks");
                                string rem = Temp_Remarks = ptsProblem.PtsRemarks = gtxtPTRemarksNew.Text.Trim();

                                isDeptSelected = true;
                                string TT = "<Details><ProblemType><ProblemTypeId>1</ProblemTypeId><ProblemTypeCode>1</ProblemTypeCode><ProblemCategoryId>1</ProblemCategoryId><PTCategoryRemark>" + Temp_Remarks.ToString().Trim() + "</PTCategoryRemark><DepartmentID>" + Request.QueryString["DeptValue"].ToString().Trim().ToString().Trim() + "</DepartmentID></ProblemType></Details>";
                                ViewState["xml"] = TT.ToString();
                            }
                            else
                            {
                                ViewState["xml"] = sb.ToString();
                            }
                            Session["totalRemarks"] = ptsProblem.PtsRemarks;
                            // =======================// =======================
                            WriteLog("AfterSaveButton-ReqID5" + Session["totalRemarks"].ToString().Trim());
                            WriteLog("Type-" + Session["queryType"].ToString().Trim());
                            //if (isDeptSelected)
                            //{
                            if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "0")
                            //if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "1")
                            {
                                #region 1240 if
                                //WriteLogText("========================================================================================================");
                                //WriteLogText("Starting Signal File Creation");
                                //WriteLogText("JourName|Article :" + JournalName.Trim() + "|" + article.Trim());
                                //WriteLogText("---------------------------------------------------------------");

                                string tempSignalFilePath = ConfigurationManager.AppSettings["tempSignalFilePath"].ToString().Trim();
                                tempSignalFilePath = clsServerConfig.GetStringReplacement(tempSignalFilePath, customerCode);

                                string dtdName = ConfigurationManager.AppSettings["DTDName"].ToString().Trim();
                                string signalFileName = dtConfig.Rows[0]["SignalFileName"].ToString().Trim();
                                string signalFileDestination = dtConfig.Rows[0]["SignalFileDestination"].ToString().Trim();
                                signalFileDestination = clsServerConfig.GetStringReplacement(signalFileDestination, customerCode);

                                string signalTemplate = dtConfig.Rows[0]["SignalTemplate"].ToString().Trim();
                                string zipFileDestination = dtConfig.Rows[0]["ZipFileDestination"].ToString().Trim();
                                zipFileDestination = clsServerConfig.GetStringReplacement(zipFileDestination, customerCode);

                                signalTemplate = signalTemplate.Replace("[dtdname]", dtdName);
                                signalTemplate = signalTemplate.Replace("[day]", DateTime.Now.ToString("dd")).Replace("[month]", DateTime.Now.ToString("MM")).Replace("[year]", DateTime.Now.ToString("yyyy")).Replace("[hour]", DateTime.Now.ToString("hh")).Replace("[minutes]", DateTime.Now.ToString("mm")).Replace("[seconds]", DateTime.Now.ToString("ss"));
                                signalTemplate = signalTemplate.Replace("[stage]", stage);
                                signalTemplate = signalTemplate.Replace("[piinumber]", formattedPIINumber);
                                string ProblemTypeCodes = "";
                                string ProblemTags = "";
                                string PtsRemarks = "";
                                string PtsLongDesc = "";

                                for (int i = 0; i < gvwPTInsEdit.Rows.Count; i++)
                                {
                                    DropDownList gddlPT = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPT");
                                    string strPTCode = string.Empty;
                                    if (gddlPT.SelectedIndex > 0)
                                        strPTCode = gddlPT.SelectedItem.Text.Trim().Substring(0, gddlPT.SelectedItem.Text.IndexOf("--")).Trim();
                                    ProblemTypeCodes = strPTCode.Trim();
                                    TextBox gtxtPTRemarks = (TextBox)gvwPTInsEdit.Rows[i].FindControl("gtxtPTRemarks");
                                    PtsRemarks = gtxtPTRemarks.Text.Trim();

                                    DropDownList gddlPTCategory = (DropDownList)gvwPTInsEdit.Rows[i].FindControl("gddlPTCategory");
                                    string strDesc = GetPTSDesc(gddlPT.SelectedValue.ToString().Trim());
                                    string hiddenDesc = string.Empty;
                                    if (gddlPTCategory.SelectedIndex > 0)
                                    {
                                        //string strPTCategory = ptsQueryService.GetPTCategoryById(gddlPTCategory.SelectedValue.ToString().Trim());
                                        //hiddenDesc = (gddlPTCategory.SelectedItem.Text.Trim() != string.Empty ? "[" + strPTCategory + "] " : string.Empty) + strDesc;

                                        hiddenDesc = gddlPTCategory.SelectedItem.Text.Trim() != string.Empty ? "[" + gddlPTCategory.SelectedItem.Text.Trim() + "]" : string.Empty;
                                    }
                                    else
                                        hiddenDesc = strDesc;

                                    //PtsLongDesc = hiddenDesc.Trim().Replace("&", "&amp;").Replace("/", "&frasl;");
                                    PtsLongDesc = hiddenDesc.Trim().Replace("&", "&amp;");
                                    PtsLongDesc = hiddenDesc.Trim().Replace("<", "&lt;");
                                    PtsLongDesc = hiddenDesc.Trim().Replace(">", "&gt;");
                                    ////PtsLongDesc = hiddenDesc.Trim().Replace("$", "&#36;");

                                    PtsRemarks = PtsRemarks.Trim().Replace("&", "&amp;");
                                    PtsRemarks = PtsRemarks.Replace("<", "&lt;");
                                    PtsRemarks = PtsRemarks.Replace(">", "&gt;");
                                    ////PtsRemarks = PtsRemarks.Replace("$", "&#36;");

                                    //ProblemTags += "<problem><problem-type type=\"" + ProblemTypeCodes + "\"/><problem-remark>" + PtsLongDesc + "-" + PtsRemarks + "</problem-remark>";
                                    ProblemTags += "<problem><problem-type type=\"" + ProblemTypeCodes + "\"/><problem-remark>" + PtsLongDesc + "-" + PtsRemarks + "</problem-remark>";
                                    if (i == dtPTCategory.Rows.Count - 1 && File.Exists(tempZipFileName))
                                        ProblemTags += "<file-attached>" + "JQ" + problemID + ".zip" + "</file-attached></problem>";
                                    else
                                        ProblemTags += "</problem>";


                                }
                                signalTemplate = signalTemplate.Replace("[problems]", ProblemTags);

                                signalTemplate = signalTemplate.Replace("[problemtypecode]", ProblemTypeCodes.TrimEnd(','));
                                if (Session["totalRemarks"] != null)
                                    if (Session["totalRemarks"].ToString().Trim() != string.Empty)
                                        ptsProblem.PtsRemarks = Session["totalRemarks"].ToString().Trim();
                                    else
                                        ptsProblem.PtsRemarks = string.Empty;
                                else
                                    ptsProblem.PtsRemarks = string.Empty;


                                if (ptsProblem.PtsRemarks != null)
                                    signalTemplate = signalTemplate.Replace("[problemremarks]", ptsProblem.PtsRemarks.ToString().Trim());

                                signalTemplate = signalTemplate.Replace("[msgid]", "JQ" + problemID);

                                if (ptsProblem.ZipFileUpload)
                                {
                                    //WriteLogText("ZIP file is attached");
                                    zipFileName = zipFileDestination + problemID + ".zip";
                                    ptsProblem.ZipFilePath = zipFileName;

                                    if (File.Exists(tempZipFileName))
                                    {
                                        signalTemplate = signalTemplate.Replace("[zipfilename]", "JQ" + problemID + ".zip");
                                    }
                                    else
                                    {
                                        //WriteLogText("No ZIP file ");
                                        signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                                        signalFileDestination = zipFileDestination;
                                    }
                                }
                                else
                                {
                                    ptsProblem.ZipFilePath = lblZipFile.Text.Trim();
                                    signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                                    signalFileDestination = zipFileDestination;
                                }

                                signalFileName = signalFileName.Replace("[msgid]", problemID);
                                tempSignalFilePath = tempSignalFilePath.Replace("[stage]", stage);

                                if (!Directory.Exists(tempSignalFilePath))
                                    Directory.CreateDirectory(tempSignalFilePath);
                                //WriteLogText("Creating Signal File");
                                StreamWriter sw = new StreamWriter(tempSignalFilePath + signalFileName);
                                sw.Write(signalTemplate);
                                sw.Close();
                                //WriteLogText("Signal File Created in Temp Path :" + tempSignalFilePath + signalFileName);
                                try
                                {
                                    XmlDocument xDoc = new XmlDocument();
                                    xDoc.Load(tempSignalFilePath + signalFileName);
                                    //WriteLogText("Parsing Signal File");
                                    //string ResultMsg = "";
                                    if (Session["totalRemarks"] != null)
                                        if (Session["totalRemarks"].ToString().Trim() != string.Empty)
                                            //ptsProblem.PtsRemarks = Session["totalRemarks"].ToString().Trim(); //  PTDescriptionTextBox.Text.Trim(); // 
                                            //ptsProblem.PtsRemarks = GeneralHelper.RemoveSpecialChars(Session["totalRemarks"].ToString().Trim());
                                            ptsProblem.PtsRemarks = Session["totalRemarks"].ToString().Trim();
                                        else
                                            ptsProblem.PtsRemarks = string.Empty;
                                    else
                                        ptsProblem.PtsRemarks = string.Empty;

                                    ptsProblem.ID = OpenedRequestId;
                                    ptsProblem.FileContent = ViewState["xml"].ToString();

                                    //ptsProblem.FileContent = ptsProblem.FileContent.Trim().Replace("&", "&amp;");
                                    //ptsProblem.FileContent = ptsProblem.FileContent.Replace("<", "&lt;");
                                    //ptsProblem.FileContent = ptsProblem.FileContent.Replace(">", "&gt;");

                                    //ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Trim().Replace("&", "&amp;");
                                    //ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace("<", "&lt;");
                                    //ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace(">", "&gt;");


                                    // commented on 09-03-2017
                                    //  DataTable dt = ptsQueryService.UpdateStatus(ptsProblem);
                                    //  ResultMsg = ptsQueryService.UpdateStatus(ptsProblem);
                                    ResultMsg = UpdateStatus(ptsProblem);
                                    if (ResultMsg == "Task Raised successfully")
                                    {




                                        if (File.Exists(tempZipFileName))
                                        {
                                            //WriteLogText("Copying Zip File To destination Path - " + zipFileName);
                                            File.Copy(tempZipFileName, zipFileName, true);
                                            //WriteLogText("Zip File copied");
                                            File.Delete(tempZipFileName);
                                        }

                                        if (!Directory.Exists(signalFileDestination))
                                            Directory.CreateDirectory(signalFileDestination);
                                        //WriteLogText("Copying Signal File To destination Path");
                                        File.Copy(tempSignalFilePath + signalFileName, signalFileDestination + "JQ" + signalFileName, true);
                                        ResultMsg += ". File " + signalFileDestination + "JQ" + signalFileName + " created successfully!";
                                        //WriteLogText("Signal File Created Successfully :" + signalFileDestination + "JQ" + signalFileName);



                                        Session["ResultMsg"] = ResultMsg;
                                        //ShowMessage(ResultMsg);
                                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', '" + ResultMsg + "', 'Required', 'top', 'right');", true);
                                        //WriteLogText(ResultMsg);
                                        ClearFields();
                                    }
                                    else
                                    {
                                        ResultMsg = "Error in close/raise a query";
                                        //WriteLogText(ResultMsg);
                                    }
                                    // commented on 09-03-2017
                                }
                                catch
                                {
                                    if (File.Exists(tempSignalFilePath + signalFileName))
                                        File.Delete(tempSignalFilePath + signalFileName);
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Parsing failed - Unable to Close/Raise query', 'Required', 'top', 'right');", true);
                                    //ShowMessage("Parsing failed - Unable to Close/Raise query");
                                    //WriteLogText("Parsing failed - Unable to Close/Raise query");
                                    return;
                                }
                                //WriteLogText("========================================================================================================");

                                #endregion 1240 if
                            }
                            else
                            {
                                WriteLog("AfterSaveButton-ReqID6");
                                bool Mail_Req = false;
                                bool IsQueryMailReqd = Convert.ToBoolean(dtConfig.Rows[0]["IsQueryMailReqd"].ToString()); // updt in db
                                bool IsInstrnMailReqd = Convert.ToBoolean(dtConfig.Rows[0]["IsInstrnMailReqd"].ToString()); // updt in db
                                bool IsQueryMailForPMOnly = Convert.ToBoolean(dtConfig.Rows[0]["IsQueryMailForPMOnly"].ToString()); // updt in db
                                bool IsInstrnMailForPMOnly = Convert.ToBoolean(dtConfig.Rows[0]["IsInstrnMailForPMOnly"].ToString()); // updt in db
                                bool isPM = false;
                                if ((IsQueryMailForPMOnly) || (IsInstrnMailForPMOnly))
                                {
                                    #region CheckRaisedByPM
                                    string[] params1 = { Session["userName"].ToString().Trim() };
                                    SqlHelper sql = new SqlHelper();
                                    DataSet dsIsPM = sql.GetDataSetFromSP("USP_PTS_CheckPM", params1, 2);
                                    
                                    if ((dsIsPM.Tables.Count > 0) && (dsIsPM.Tables[0].Rows.Count > 0))
                                    {
                                        isPM = dsIsPM.Tables[0].Rows[0][0].ToString().Trim().Equals("1");
                                    }


                                    #endregion CheckRaisedByPM
                                }
                                if ((IsQueryMailReqd) && (_quryt == "QUERY") && (IsQueryMailForPMOnly) && (isPM))
                                    Mail_Req = true;
                                else   if ((IsQueryMailReqd) && (_quryt == "QUERY"))
                                    Mail_Req = true;
                                
                                if ((IsInstrnMailReqd) && (_quryt == "INSTRUCTION"))
                                {
                                    if ((IsInstrnMailForPMOnly) && (isPM))
                                        Mail_Req = true;
                                    else if (!IsInstrnMailForPMOnly)
                                        Mail_Req = true;
                                }
                                   

                              
                                #region 1240 else
                                string Emailfrom = dtConfig.Rows[0]["FromEmailID"].ToString(); // updt in db
                               


                                string EmailTo = dtConfig.Rows[0]["ToEmailID"].ToString().Trim();// Commented By sathish.J on 13-07-17
                                string EmailToJobAuthor = dtConfig.Rows[0]["ToMailJobAuthorContact"].ToString().Trim();// Commented By sathish.J on 13-07-17
                                ////
                                 if((Emailfrom=="") ||(EmailToJobAuthor==""))
                                     Mail_Req = false;
                                string CCEmail = dtConfig.Rows[0]["CCEmail"].ToString().Trim();


                                // RMT #9677
                                #region NewMailCode
                                DateTime mailtime = DateTime.Now;
                                string mailsendtime = Convert.ToString(mailtime);

                                string Subject = dtConfig.Rows[0]["EmailSubject"].ToString().Trim();
                                string MsgBody = dtConfig.Rows[0]["EmailContent"].ToString();

                                WriteLog("ISBN" + Session["Isbn"].ToString().Trim());


                                WriteLog("stage" +  stage.Trim());
                                //WriteLog(1, Subject, MsgBody, stage, JournalName, ArtvolIssue, "Before resolving the variables");
                                if (article.ToLower() == "book")
                                    Subject = Subject.Replace("<Journal>", JournalName.Trim()).Replace("<Stage>", stage.Trim()).Replace("<ISBN>", Session["Isbn"].ToString().Trim()).Replace(" - <article>", "");
                                else if (article != string.Empty)
                                    Subject = Subject.Replace("<Journal>", JournalName.Trim()).Replace("<article>", article.Trim()).Replace("<Stage>", stage.Trim()).Replace("<ISBN>", Session["Isbn"].ToString().Trim());
                                
                                else
                                    Subject = Subject.Replace("<Journal>", JournalName.Trim()).Replace("<article>", volume.Trim() + issue.Trim()).Replace("<Stage>", stage.Trim()).Replace("<ISBN>", Session["Isbn"].ToString().Trim());

                                Subject = Subject.Replace("<BATCHID>", roundCount.ToString().Trim());
                                Subject = Subject.Replace("<QueryType>", _quryt.ToString().Trim());

                                if (Session["totalRemarks"] != null)
                                    if (Session["totalRemarks"].ToString().Trim() != string.Empty)
                                        ptsProblem.PtsRemarks = Session["totalRemarks"].ToString().Trim();

                                string tempPTDescription = Session["totalRemarks"].ToString().Trim();
                                WriteLog("Remarks" + Session["totalRemarks"].ToString().Trim());
                                WriteLog("Desc" + tempPTDescription);
                                WriteLog("GetDetails" + CustomerName.Trim()+"-"+ JournalName+"-"+ article.Trim()+"-"+ stage.Trim());

                                if (article != string.Empty)
                                {
                                    MsgBody = MsgBody.Replace("<Customer>", CustomerName.Trim()).Replace("<Journal>", JournalName.Trim()).Replace("<article>", article.Trim()).Replace("<Stage>", stage.Trim()).Replace("<ProblemDesc>", tempPTDescription);
                                    MsgBody = MsgBody.Replace("<ISBN>", Session["Isbn"].ToString().Trim());
                                }
                                else
                                    MsgBody = MsgBody.Replace("<Customer>", CustomerName.Trim()).Replace("<Journal>", JournalName.Trim()).Replace("<article>", volume.Trim() + ',' + issue.Trim()).Replace("<Stage>", stage.Trim()).Replace("<ProblemDesc>", tempPTDescription);


                                tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);
                                tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);

                                if (dtProblems.Rows.Count > 0)
                                    ptsProblem.ZipFilePath = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim();

                                WriteLog("ZipPath" + tempZipFilePath);

                                //WriteLog(1, "", "", Convert.ToString(stage), JournalName.Trim(), ArtvolIssue, "Db updation started");
                                if (Session["totalRemarks"] != null)
                                    if (Session["totalRemarks"].ToString().Trim() != string.Empty)
                                        ptsProblem.PtsRemarks = Session["totalRemarks"].ToString().Trim();

                                ptsProblem.ID = OpenedRequestId;
                                ptsProblem.FileContent = ViewState["xml"].ToString();

                                ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace("&", "&amp;");
                                ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace("<", "&lt;");
                                ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace(">", "&gt;");
                                //ptsProblem.PtsRemarks = ptsProblem.PtsRemarks.Replace("$", "&#36;");
                               
                              //  ResultMsg = ptsQueryService.UpdateStatus(ptsProblem);


                                ResultMsg= UpdateStatus(ptsProblem);


                                WriteLog("ResultMsg" + ResultMsg.ToString().Trim());

                                if (Mail_Req == true)
                                {
                                    if (ResultMsg == "Task Raised successfully")
                                    {

                                        if (ResultMsg == "Task Raised successfully")//Modified by sathish.J on 13-07-2017
                                        {
                                            //WriteLog(1, "", "", Convert.ToString(stage), JournalName, ArtvolIssue, "Db updation completed");

                                            ptsProblem.SubStageId = stageID;

                                            MsgBody = MsgBody.Replace("<InquirerName>", hdnReviewerName.Value);
                                            MsgBody = MsgBody.Replace("<ReviewerName>", hdnUserName.Value);
                                            MsgBody = MsgBody.Replace("|", "<br \\>");

                                            MsgBody = MsgBody.Replace("\r\n", "<br \\>");

                                            //WriteLog(1, Subject, MsgBody, stage, JournalName, ArtvolIssue, "After resolving the variables");

                                            try
                                            {
                                                string chkSubject = Subject.ToUpper();
                                                if (chkSubject.Contains("<STAGE>") || chkSubject.Contains("<JOURNAL>") || chkSubject.Contains("<ARTICLE>"))
                                                {
                                                    ResultMsg += "Error in sending mail. Please contact software team.";
                                                    WriteLog(Subject+"-"+MsgBody + "-" + stage + "-" + JournalName + "-" + ArtvolIssue + "-" + "Subject contains special characters");
                                                    //ShowMessage(ResultMsg);
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', '" + ResultMsg + "', 'Required', 'top', 'right');", true);
                                                    return;
                                                }
                                                else if (chkSubject.Contains("<") || chkSubject.Contains(">"))
                                                {
                                                    ResultMsg += "Error in sending mail. Please contact software team.";
                                                    WriteLog(Subject + "-" + MsgBody + "-" + stage + "-" + JournalName + "-" + ArtvolIssue + "-" + "Subject contains special characters");
                                                    //WriteLog(1, Subject, MsgBody, stage, JournalName, ArtvolIssue, "Subject contains special characters");
                                                    //ShowMessage(ResultMsg);
                                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', '" + ResultMsg + "', 'Required', 'top', 'right');", true);
                                                    return;
                                                }
                                                else
                                                {
                                                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                                                    message.From = new System.Net.Mail.MailAddress(Emailfrom);

                                                    string[] tempMail;
                                                    if (_quryt == "QUERY")
                                                        message.To.Add(new System.Net.Mail.MailAddress(EmailTo));
                                                    else
                                                    {
                                                        tempMail = EmailToJobAuthor.Split(',');
                                                        for (int i = 0; i < tempMail.Length; i++)
                                                            message.To.Add(new System.Net.Mail.MailAddress(tempMail[i]));
                                                    }

                                                    tempMail = CCEmail.Split(',');
                                                    for (int i = 0; i < tempMail.Length; i++)
                                                        if (tempMail[i].Trim() != "")
                                                            message.CC.Add(new System.Net.Mail.MailAddress(tempMail[i]));

                                                    //WriteLog(1, message.Subject, message.Body, stage, JournalName, ArtvolIssue, "Before Sending mail");
                                                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(SmtpAddr);

                                                    System.Net.Mail.Attachment MAttNew;
                                                    string ZipFilePathName = string.Empty;
                                                    if (Session["zipFile"] != null)
                                                    {
                                                        ZipFilePathName = Session["zipFile"].ToString().Trim();
                                                    }
                                                    if (ZipFilePathName != string.Empty)
                                                    {
                                                        if (File.Exists(ZipFilePathName))
                                                        {
                                                            MAttNew = new System.Net.Mail.Attachment(ZipFilePathName);
                                                            System.Net.Mime.ContentDisposition objCD = new System.Net.Mime.ContentDisposition();
                                                            objCD = MAttNew.ContentDisposition;

                                                            string tempFileName = dtConfig.Rows[0]["EMAILZIPFILENAME"].ToString().Trim();
                                                            if (article != string.Empty)
                                                                tempFileName = tempFileName.Replace("<JOURNAL>", JournalName).Replace("<ARTICLE>", article).Replace("<STAGE>", stage.Trim());
                                                            else
                                                                tempFileName = tempFileName.Replace("<JOURNAL>", JournalName).Replace("<ARTICLE>", volume + ',' + issue).Replace("<STAGE>", stage);

                                                            tempFileName = tempFileName.Replace("<BATCHID>", OpenedRequestId.ToString().Trim());


                                                            objCD.FileName = tempFileName;
                                                            message.Attachments.Add(MAttNew);
                                                        }
                                                    }

                                                    message.IsBodyHtml = true;
                                                    message.Priority = System.Net.Mail.MailPriority.Normal;
                                                    message.Subject = Subject;
                                                    message.Body = MsgBody;
                                                    WriteLog("Before Sending mail");
                                                    client.Send(message);
                                                    WriteLog("After Sending mail");
                                                    System.Threading.Thread.Sleep(500);
                                                    // ResultMsg += ". Mail has been sent to " + EmailTo + "; " + EmailToJobAuthor;// commented by sathish.J on 13-07-17
                                                    ClearFields();
                                                    btnCancel.Text = "Back";
                                                    Session["Redirect_msg"] = ResultMsg;

                                                    // if (ResultMsg == "Task Raised successfully") { Response.Redirect("RaiseProblem.aspx?Username=" + Session["userName"].ToString().Trim() + "&Redirect_msg=" + ResultMsg, false); }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                //WriteLog(1, SubForLog, MsgBodyForLog, stage, JournalName, ArtvolIssue, ex.Message);
                                                throw new Exception(ex.Message, ex);
                                            }
                                        }
                                        else
                                        {
                                            Session["ResultMsg"] = ResultMsg;
                                        }
                                        //ShowMessage(ResultMsg);


                                    }
                                    else
                                    {
                                        ResultMsg = "Error in close/raise a query";
                                    }


                                #endregion NewMailCode

                                #endregion 1240 else
                                }
                            }
                            //}
                            //else
                            //{
                            //    ShowMessage("Please select all departments/problem types.");
                            //}
                        }
                        else
                        {
                            // ShowMessage("No configuration is available for this stage");
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'No configuration is available for this stage', 'Required', 'top', 'right');", true);
                            return;
                        }
                    }
                    else
                    {
                        //ShowMessage("Please select at least one problem type");
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select at least one problem type', 'Required', 'top', 'right');", true);
                    }
                    #endregion try


                    //if (mailstatus)
                    //{
                    //    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', 'MailSent', 'Success', 'top', 'right');", true);
                    //}
                }
                catch (Exception ex)
                {
                   
                    WriteLog("exception : " + ex.ToString());
                }

            }
            if (ResultMsg == "Task Raised successfully")
            {
                WriteLog("afterRaiseClick : " + Session["userName"].ToString());
                Session["userName"] = Session["userName"].ToString();
                // Session["userName"] = "MAC008188";
                //WriteLog("Passed");
                // WriteLog("RaiseProblems.aspx?Username=" + Session["userName"].ToString() + "&Redirect_msg=" + Session["task_msg"].ToString());
                //WriteLog("Query"+Request.QueryString["userName"].ToString().Trim());
                Response.Redirect("RaiseProblems.aspx?Username=" + Session["Encrypted User"].ToString() + "&Redirect_msg=" + ResultMsg, false);
            }


            if (isDeptSelected)
            {

                GetCloseStatusNew(Session["Customer"].ToString(), 1);
                Session["CloseStatus"] = 1;
                //Response.Redirect("RaiseProblem.aspx?Username=" + System.Web.HttpContext.Current.Session["userName"], false);
            }
        }
        //public  string UpdateStatus(int OpenedRequestId1, bool taskClosed1, string tempUserId1, bool zipUpload1, string ZipFilePath1, string Remarks1, string FileContent1,"0")
        //{

        //    try
        //    {
        //      string   StatusVal = "";
        //           SqlConnection conn = null;
        //        string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
        //        conn = new SqlConnection(constr1);
        //        if (conn.State == ConnectionState.Open)
        //            conn.Close();
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("usp_PTS_UpdateStatus", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = OpenedRequestId1;
        //        cmd.Parameters.Add("@taskclosed", SqlDbType.Bit).Value = taskClosed1;
        //        cmd.Parameters.Add("@userCode", SqlDbType.VarChar).Value = tempUserId1;
        //        cmd.Parameters.Add("@zipFileUpload", SqlDbType.Bit).Value = zipUpload1;
        //        cmd.Parameters.Add("@ZipFilePath", SqlDbType.NVarChar).Value = ZipFilePath1;
        //        cmd.Parameters.Add("@Remarks", SqlDbType.VarChar).Value = Remarks1;
                
        //        cmd.Parameters.Add("@FILECONTENT", SqlDbType.NVarChar).Value = FileContent1;
                

        //        cmd.Parameters.Add("@Status", SqlDbType.VarChar).Direction = ParameterDirection.Output;

        //        StatusVal = (string)cmd.Parameters["@Status"].Value;
        //    }
        //    catch (Exception ex)
        //    {
               
        //    }
        //    return StatusVal;
        //}
        public static string UpdateStatus(PTSProblem ptsProblem)
        {
            SqlConnection conn = null;
            string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
            conn = new SqlConnection(constr1);
            if (conn.State == ConnectionState.Open)
                conn.Close();
            conn.Open();
            string taskClosed = string.Empty;
            if (ptsProblem.TaskClosed)
                taskClosed = "1";
            else
                taskClosed = "0";

            string zipUpload = string.Empty;

            if (ptsProblem.ZipFileUpload)
                zipUpload = "1";
            else
                zipUpload = "0";

            if (ptsProblem.FileContent == null)
                ptsProblem.FileContent = string.Empty;

            // code added by prasanta start
            string[] paramArr = new string[] { ptsProblem.ID.ToString(), taskClosed, ptsProblem.UserName.ToString(), zipUpload, ptsProblem.ZipFilePath.ToString(), ptsProblem.PtsRemarks, ptsProblem.FileContent, "0" };
            // code added by prasanta end

            string result = string.Empty;
            result = DBHelper.ExecuteSPWithOutputParameter("usp_PTS_UpdateStatusNew", paramArr, constr1);

            return result;
        }
        protected void ZipButton_Click(object sender, EventArgs e)
        {
            //     Response.Redirect("Zipfiles.aspx?Username=" + System.Web.HttpContext.Current.Session["userName"], false);
        }

        protected void QueryResolveButton_Click(object sender, EventArgs e)
        {

        }
    }
}
