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
using System.Text;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using System.IO;

using System.Text.RegularExpressions;
using System.Web.Mail;
using QueryTracker;
using QueryTrackerSetUp;
using System.Data.SqlClient;

namespace PTS
{
    public partial class DueForClosing : System.Web.UI.Page
    {
        IDueForClose dueForCloseService = new DueForCloseService();
        IPTSQuery ptsQueryService = new PTSQueryService();
        //clsServerConfiguration serverConfig = SANGlobal.ServerConfig;

        string stage = string.Empty;
        string user = string.Empty;

        string articleType = ConfigurationManager.AppSettings["ArticleType"].ToString().Trim();
        string issueType = ConfigurationManager.AppSettings["IssueType"].ToString().Trim();
        string jobType = string.Empty;

        int customerCode,journalCode,stageID;
        string article, volume, issue;
        string customerGroupName, journalName, ProblemTypeId, StageName;

        bool raiseQuery = true;
        public string SmtpAddr = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPAddr");

        protected string userName = Global.Validate();

        //protected string userName ="MAC004342";
        protected int roleID;

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            CloseButton.Attributes.Add("onclick", "return ValidateGridViewCheckBoxes()");
            DeleteButton.Attributes.Add("onclick", "return ConfirmDelete()");
            //stage = "P100";
            //Global.userId = "67";
            //Global.RoleID = "3";

            if (Request.QueryString["stage"] == null)
            {
                MessageLabel.Text = "Stage not found";
                return;
            }

            stage = Request.QueryString["stage"];
            StageName = Request.QueryString["stagename"];
            user = Request.QueryString["Username"];
            userName = user;

            try
            {
                roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");
                if (roleID <= 0)
                {
                    MessageLabel.Text = "User role not defined";
                    System.Web.HttpContext.Current.Response.Redirect("BlankPage.aspx");
                    return;
                }

                if (!Page.IsPostBack)
                {
                    string stgname = Request.QueryString["stagename"];
                    StageLabel.Text = "Due for closing - " + stgname;
                    jobType = ptsQueryService.GetJobTypeByStage(stage).ToString();
                    ViewState["jobType"] = jobType.ToString().Trim();

                    #region Enable/Disable buttons based on role

                    bool allowCloseSignal = false;
                    allowCloseSignal = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryCloseButton);
                    CloseButton.Enabled = allowCloseSignal;


                    bool allowDelete = false;
                    allowDelete = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.DeleteButtonNewQuery);
                    DeleteButton.Enabled = allowDelete;

                    #endregion

                    BindDueArticles("INCREMENT");
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void GVDueArticles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    Label gvLabel = (Label)e.Row.FindControl("GVLabelCustomerCode");
                    int.TryParse(gvLabel.Text.Trim(), out customerCode);

                    gvLabel = (Label)e.Row.FindControl("GVLabelJournalCode");
                    int.TryParse(gvLabel.Text.Trim(), out journalCode);

                    gvLabel = (Label)e.Row.FindControl("GVLabelJournal");
                    journalName = gvLabel.Text.Trim();

                    gvLabel = (Label)e.Row.FindControl("GVLabelUsername");
                    userName = gvLabel.Text.Trim();

                    gvLabel = (Label)e.Row.FindControl("GVLabelArticle");
                    article = gvLabel.Text.Trim();
                    gvLabel = (Label)e.Row.FindControl("GVLabelVolume");
                    volume = gvLabel.Text.Trim();
                    gvLabel = (Label)e.Row.FindControl("GVLabelIssue");
                    issue = gvLabel.Text.Trim();

                    if (ViewState["jobType"] != null)
                        jobType = (string)ViewState["jobType"];

                    gvLabel = (Label)e.Row.FindControl("GVLabelStageID");
                    int.TryParse(gvLabel.Text, out stageID);

                    gvLabel = (Label)e.Row.FindControl("GVLabelStage");
                    stage = gvLabel.Text.Trim();
                    

                    gvLabel = (Label)e.Row.FindControl("GVLabelUsername");
                    userName = gvLabel.Text.Trim();

                    gvLabel = (Label)e.Row.FindControl("GVLabelCustomerGroupName");
                    customerGroupName = gvLabel.Text.Trim();

                    gvLabel = (Label)e.Row.FindControl("GVLabelProblemTypeCode");
                    ProblemTypeId = gvLabel.Text.Trim();

                    Button gvZipButton = (Button)e.Row.FindControl("GVZipButton");

                    //if (jobType==articleType)
                        gvZipButton.Attributes.Add("onclick", "ZipFiles(" + customerCode.ToString() + "," + journalCode.ToString() + ",'" + article + "'," + stageID.ToString() + ",'" + stage.Trim() + "','" + userName + "'," + jobType.ToString() + ",'" + ProblemTypeId.ToString().Trim() + "'" + ")");
                    //else if (jobType == issueType)
                    //{
                    //    string volumeIssue = volume + '~' + issue;
                    //    gvZipButton.Attributes.Add("onclick", "ZipFiles(" + customerCode.ToString() + "," + journalCode.ToString() + ",'" + volumeIssue + "'," + stageID.ToString() + ",'" + stage.Trim() + "','" + userName + "'," + jobType.ToString() + ")");
                    //}

                    string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                    string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();

                    tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);
                    tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);

                    //tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", journalName).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[username]", userName);

                    tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", journalName).Replace("[article]", article.Trim()).Replace("[problemtypeid]", ProblemTypeId).Replace("[stage]", stage).Replace("[username]", userName);
                    tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                    Image gvImage = (Image)e.Row.FindControl("GVZipImage");
                    if (File.Exists(tempZipFilePath + tempZipFileName))
                    {
                        gvImage.Visible = true;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void CloseButton_Click(object sender, EventArgs e)
        {
            StringBuilder signalMessage = new StringBuilder();
            PTSProblem ptsProblem;
            
            string problemRemarks = string.Empty;
            string problemTypeCode = string.Empty;
            string validationMessage;
            string  EmailTo;

            string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
            string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
            string tempSignalFilePath = ConfigurationManager.AppSettings["tempSignalFilePath"].ToString().Trim();
            string stgname = Request.QueryString["stagename"];
            try
            {
                if (ViewState["jobType"] != null)
                    jobType = (string)ViewState["jobType"];

                foreach (GridViewRow gRow in GVDueArticles.Rows)
                {
                    string customerGroupName = string.Empty;
                    string journalName = string.Empty;
                    string CustCode = string.Empty;
                    string piiNumber = string.Empty;
                    string formattedPIINumber = string.Empty;
                    string volumeFirst = string.Empty;
                    string volumeLast = string.Empty;
                    string issueFirst = string.Empty;
                    string issueLast = string.Empty;
                    string suppl = string.Empty;
                    string InquirerName = string.Empty;
                    string ReviewerName = string.Empty;
                        

                    //read customer-journal-artticle details from the grid view
                    CheckBox gvCheckBox = (CheckBox)gRow.FindControl("GVCheckBox");
                    if (gvCheckBox.Checked)
                    {
                        ptsProblem = new PTSProblem();
                        
                        Label gvLabel = (Label)gRow.FindControl("GVLabelCustomerCode");
                        int.TryParse(gvLabel.Text.Trim(), out customerCode);

                        gvLabel = (Label)gRow.FindControl("GVLabelJournalCode");
                        int.TryParse(gvLabel.Text.Trim(), out journalCode);

                        //if (jobType == articleType)
                        //{
                            gvLabel = (Label)gRow.FindControl("GVLabelArticle");
                            article = gvLabel.Text.Trim();

                            //volume = string.Empty;
                            //issue = string.Empty;
                        //}
                        //else if (jobType == issueType)
                        //{
                            gvLabel = (Label)gRow.FindControl("GVLabelVolume");
                            volume = gvLabel.Text.Trim();

                            gvLabel = (Label)gRow.FindControl("GVLabelIssue");
                            issue = gvLabel.Text.Trim();

                            //article = string.Empty;
                        //}

                        gvLabel = (Label)gRow.FindControl("GVLabelStageID");
                        int.TryParse(gvLabel.Text, out stageID);

                        gvLabel = (Label)gRow.FindControl("GVLabelUsername");
                        userName = gvLabel.Text.Trim();

                        gvLabel = (Label)gRow.FindControl("GVLabelRemarks");
                        problemRemarks = gvLabel.Text;

                        gvLabel = (Label)gRow.FindControl("GVLabelJournal");
                        journalName = gvLabel.Text.Trim();

                        gvLabel = (Label)gRow.FindControl("GVLabelProblemTypeCode");
                        problemTypeCode = gvLabel.Text.Trim();

                        gvLabel = (Label)gRow.FindControl("GVLabelRemarks");
                        string problemTypeDesc = gvLabel.Text.Trim();

                        gvLabel = (Label)gRow.FindControl("GVLabelIncrement");
                        string BatchID = gvLabel.Text.Trim();

                        gvLabel = (Label)gRow.FindControl("GVLabelUsername");
                        ReviewerName = userName;

                        gvLabel = (Label)gRow.FindControl("GVLabelEditor");
                        InquirerName = gvLabel.Text.Trim();

                        gvLabel = (Label)gRow.FindControl("GVLabelID");
                        string ID = gvLabel.Text.Trim();
                         

                        //end of read customer-journal-artticle details from the grid view

                        raiseQuery = DoValidations(out validationMessage);

                         string ptsProblemUser = string.Empty;
                        DataTable dtConfig = new DataTable();
                        string zipFileName = string.Empty;
                        
                        if (raiseQuery)
                        {

                            //get the customer group name to test uploaded zip file
                            DataTable dtCustomerGrName = new DataTable();
                            dtCustomerGrName = ptsQueryService.GetCustomerGroupNameAndJournalName(customerCode, journalCode);

                            if (dtCustomerGrName.Rows.Count > 0)
                            {
                                customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                                CustCode = dtCustomerGrName.Rows[0]["cust_code"].ToString().Trim();
                            }
                    
                            tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);
                            tempZipFilePath = tempZipFilePath.Replace("[stage]", stgname);

                            if (!Directory.Exists(tempZipFilePath))
                                Directory.CreateDirectory(tempZipFilePath);

                            string tempArt = string.Empty;
                            if (article != "")
                                tempArt = article;
                            else
                                tempArt = volume + "-" + issue + "-" + BatchID;
                         
                            dtConfig = ptsQueryService.GetPTSConfiguration(stageID, customerCode.ToString(), Convert.ToInt64(journalCode), tempArt,userName);

                            

                            DataTable dtProblems = new DataTable();
                           
                        
                            dtProblems = ptsQueryService.GetPTSProblems(customerCode, journalCode, article, stageID);
                                if (dtProblems.Rows.Count > 0)
                                    ptsProblemUser = dtProblems.Rows[0]["username"].ToString().Trim();
                                else
                                    ptsProblemUser = userName;

                                tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", journalName).Replace("[article]", article.Trim()).Replace("[problemtypeid]", problemTypeCode).Replace("[stage]", stgname).Replace("[username]", ptsProblemUser);
                                tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                                Session["zipFile"] = tempZipFilePath + tempZipFileName;

                                piiNumber = ptsQueryService.GetArticlePIINumber(customerCode, journalCode, article, stage);
                                formattedPIINumber = GeneralHelper.FormatArticlePIINumber(piiNumber);

                                ptsProblem.Article = article;
                      
                                if (volume.IndexOf("-") > 0)
                                {
                                    string[] volumeArr = volume.Split('-');
                                    if (volumeArr.Length > 1)
                                    {
                                        volumeFirst = volumeArr[0];
                                        volumeLast = volumeArr[1];
                                    }
                                }
                                else
                                {
                                    volumeFirst = volume;
                                    volumeLast = string.Empty;
                                }

                                if (issue.IndexOf("-") > 0)
                                {
                                    string[] issueArr = issue.Split('-');

                                    if (issueArr.Length > 1)
                                    {
                                        issueFirst = issueArr[0];
                                        issueLast = issueArr[1];

                                        //get the suppl value
                                        char[] issArr = issueLast.ToCharArray();

                                        for (int i = 0; i < issArr.Length; i++)
                                        {
                                            if (char.IsLetter(issArr[i]))
                                            {
                                                suppl = issArr[i].ToString();
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    issueFirst = issue;
                                    issueLast = string.Empty;

                                    //get the suppl value
                                    char[] issArr = issueFirst.ToCharArray();

                                    for (int i = 0; i < issArr.Length; i++)
                                    {
                                        if (char.IsLetter(issArr[i]))
                                        {
                                            suppl = issArr[i].ToString();
                                            break;
                                        }
                                    }
                                }
                            //} //end jobtype==issuetype

                            ptsProblem.StageID = stageID;
                            ptsProblem.Stage = stgname;
                            ptsProblem.CustomerCode = customerCode;
                            ptsProblem.JournalCode = journalCode;
                            ptsProblem.ProblemID = "0";

                            ptsProblem.ProblemTypeCode = problemTypeCode.Trim();
                            ptsProblem.PtsRemarks = problemRemarks.Trim();
                            ptsProblem.QuerySavedBy = ptsProblemUser;
                            ptsProblem.Article = article.Trim();
                            ptsProblem.Volume = volume.Trim();
                            ptsProblem.Issue = issue.Trim();
                             ptsProblem.ID =Convert.ToInt16( ID);
                            
                            

                            ptsProblem.TaskClosed = true;

                            gvLabel = (Label)gRow.FindControl("GVLabelZipFilePath");
                            string ZipFilePath = gvLabel.Text.Trim();

                            if (File.Exists(tempZipFilePath + @"\" + tempZipFileName))
                            {
                                ptsProblem.ZipFileUpload = true;
                                ptsProblem.ZipFilePath = tempZipFilePath + @"\" + tempZipFileName;
                            }
                            else if (File.Exists(ZipFilePath.Trim()))
                            {
                                ptsProblem.ZipFileUpload = true;
                                ptsProblem.ZipFilePath = ZipFilePath;
                            }
                            else
                            {
                                ptsProblem.ZipFileUpload = false;
                                ptsProblem.ZipFilePath = string.Empty;
                            }

                            ptsProblem.UserName = userName;
                            ptsProblem.QueryClosedBy = userName;

                            //Get the xml template and replace the values/
                            if (dtConfig.Rows.Count > 0)
                            {
                                #region signal
                                if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "0")
                                {

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
                                    signalTemplate = signalTemplate.Replace("[problemtypecode]", problemTypeCode.Trim());
                                    signalTemplate = signalTemplate.Replace("[problemremarks]", problemRemarks.Trim());

                                    if (jobType == issueType)
                                    {
                                        signalTemplate = signalTemplate.Replace("[volumefirst]", volumeFirst);
                                        signalTemplate = signalTemplate.Replace("[volumelast]", volumeLast);
                                        signalTemplate = signalTemplate.Replace("[issuefirst]", issueFirst);
                                        signalTemplate = signalTemplate.Replace("[issuelast]", issueLast);
                                        signalTemplate = signalTemplate.Replace("[suppl]", suppl);
                                    }

                                    string problemID = string.Empty;
                                    problemID = ptsQueryService.GetPTSProblemID();
                                    ptsProblem.ProblemID = problemID;

                                    signalTemplate = signalTemplate.Replace("[msgid]", problemID);

                                    if (ptsProblem.ZipFileUpload)
                                    {
                                        zipFileName = zipFileDestination + problemID + ".zip";
                                        //if zip file exists move the zip file to ftp area and copy the signal file to another path

                                        File.Copy(tempZipFilePath + tempZipFileName, zipFileName, true);
                                        if (File.Exists(zipFileName))
                                        {
                                            signalTemplate = signalTemplate.Replace("[zipfilename]", problemID + ".zip");
                                            File.Delete(tempZipFilePath + tempZipFileName);
                                        }
                                        else
                                        {
                                            signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                                            signalFileDestination = zipFileDestination;
                                        }
                                    }
                                    else
                                    {
                                        //if zip file not exists move the signal file to ftp area
                                        signalTemplate = signalTemplate.Replace("[zipfilename]", string.Empty);
                                        signalFileDestination = zipFileDestination;
                                    }

                                    signalFileName = signalFileName.Replace("[msgid]", problemID);
                                    tempSignalFilePath = tempSignalFilePath.Replace("[stage]", stage);

                                    if (!Directory.Exists(tempSignalFilePath))
                                        Directory.CreateDirectory(tempSignalFilePath);

                                    StreamWriter sw = new StreamWriter(tempSignalFilePath + signalFileName);
                                    sw.Write(signalTemplate);
                                    sw.Close();
                                    File.Copy(tempSignalFilePath + signalFileName, signalFileDestination + signalFileName, true);

                                    if (File.Exists(tempSignalFilePath + signalFileName))
                                        File.Delete(tempSignalFilePath + signalFileName);


                                    string saveResult = ptsQueryService.InsertOrUpdatePTSProblem(ptsProblem);

                                    signalMessage.AppendLine("File " + signalFileDestination + signalFileName + " created successfully!");


                                }
                                #endregion
                                else
                                {
                                    #region NewMailCode
                                    DateTime mailtime = DateTime.Now;
                                    string mailsendtime = Convert.ToString(mailtime);

                                    string Subject = dtConfig.Rows[0]["EmailSubject"].ToString().Trim();
                                    string MsgBody = dtConfig.Rows[0]["EmailContent"].ToString();

                                    WriteLog(1, Subject, MsgBody, Convert.ToString(stage), Convert.ToString(journalName), article, "Before resolving the variables");

                                    Subject = Subject.Replace("<Journal>", journalName.Trim()).Replace("<article>", article).Replace("<Stage>", stage.Trim());
                                    Subject = Subject.Replace("<BATCHID>", BatchID.Trim());

                                    string tempPTDescription = problemTypeDesc.Trim().Substring((problemTypeDesc.Trim().IndexOf(')') + 1), (problemTypeDesc.Trim().Length - problemTypeDesc.Trim().IndexOf(')') - 1));
                                    MsgBody = MsgBody.Replace("<Customer>", customerGroupName.Trim()).Replace("<Journal>", journalName.Trim()).Replace("<article>", article.Trim()).Replace("<Stage>", stage.Trim()).Replace("<ProblemDesc>", problemTypeDesc);

                                    tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);
                                    tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);

                                  
                                    string ResultMsg = "";

                                    WriteLog(1, "", "", stage.Trim(), journalName.Trim(), article, "Db updation started");
                                    ptsProblem.FileContent = string.Empty;
                                   // ResultMsg = ptsQueryService.UpdateStatus(ptsProblem).ToString();
                                    ResultMsg = UpdateStatus(ptsProblem);
                                    WriteLog(1, "", "", stage.Trim(), journalName.Trim(), article, "Db updation completed");

                                    ptsProblem.SubStageId = stageID;
                                    //GetCloseStatus(ptsProblem.CustomerCode.ToString(), ptsProblem.JournalCode.ToString(), objBizPTSQuery.Article = objBizPTSQuery.Article ?? string.Empty, objBizPTSQuery.Volume = objBizPTSQuery.Volume ?? string.Empty, objBizPTSQuery.Issue = objBizPTSQuery.Issue ?? string.Empty, ptsProblem.SubStageId.ToString().Trim(), ptsProblem.ProblemTypeCode.ToString().Trim());
                                    MsgBody = MsgBody.Replace("<InquirerName>", InquirerName);
                                    MsgBody = MsgBody.Replace("<ReviewerName>", ReviewerName);
                                    MsgBody = MsgBody.Replace("|", "<br />");

                                    MsgBody = MsgBody.Replace("\r\n", "<br />");

                                    WriteLog(1, Subject, MsgBody, stage.Trim(),journalName.Trim(), article.Trim(), "After resolving the variables");

                                    try
                                    {
                                        string chkSubject = Subject.ToUpper();
                                        if (chkSubject.Contains("<STAGE>") || chkSubject.Contains("<JOURNAL>") || chkSubject.Contains("<ARTICLE>"))
                                        {
                                            ResultMsg += "Error in sending mail. Please contact Technology team.";
                                            WriteLog(1, Subject, MsgBody, stage.Trim(), journalName.Trim(), article, "Subject contains special characters");
                                            //ShowMessage(ResultMsg);
                                            return;
                                        }
                                        else if (chkSubject.Contains("<") || chkSubject.Contains(">"))
                                        {
                                            ResultMsg = "Error in sending mail. Please contact Technology team.";
                                            WriteLog(1, Subject, MsgBody, stage.Trim(), journalName.Trim(), article, "Subject contains special characters");
                                            //ShowMessage(ResultMsg);
                                            return;
                                        }
                                        else
                                        {
                                            string Emailfrom = dtConfig.Rows[0]["FromEmailID"].ToString(); // updt in db
                                            EmailTo = dtConfig.Rows[0]["ToEmailID"].ToString().Trim();// +";" + dtConfig.Rows[0]["ToMailJobAuthorContact"].ToString().Trim();
                                            string EmailToJobAuthor = dtConfig.Rows[0]["ToMailJobAuthorContact"].ToString().Trim();
                            
                                            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                                            message.From = new System.Net.Mail.MailAddress(Emailfrom);

                                            string[] tempMail = EmailTo.Split(',');
                                            for (int i = 0; i < tempMail.Length; i++)
                                                message.To.Add(new System.Net.Mail.MailAddress(tempMail[i]));

                                            tempMail = EmailToJobAuthor.Split(',');
                                            for (int i = 0; i < tempMail.Length; i++)
                                                if (tempMail[i].Trim() != "")
                                                    message.CC.Add(new System.Net.Mail.MailAddress(tempMail[i]));

                                            WriteLog(1, message.Subject, message.Body, stage.Trim(), journalName.Trim(), article.Trim(), "Before Sending mail");
                                            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(SmtpAddr);

                                            System.Net.Mail.Attachment MAttNew;
                                            if (File.Exists(ptsProblem.ZipFilePath))
                                            {
                                                MAttNew = new System.Net.Mail.Attachment(tempZipFilePath.Trim() + @"\" + tempZipFileName);
                                                System.Net.Mime.ContentDisposition objCD = new System.Net.Mime.ContentDisposition();
                                                objCD = MAttNew.ContentDisposition;

                                                string tempFileName = dtConfig.Rows[0]["EMAILZIPFILENAME"].ToString().Trim();
                                                    tempFileName = tempFileName.Replace("<JOURNAL>", journalName.Trim()).Replace("<ARTICLE>", article.Trim()).Replace("<STAGE>", stage.Trim());

                                                tempFileName = tempFileName.Replace("<BATCHID>", BatchID.Trim());
                                                objCD.FileName = tempFileName;
                                                message.Attachments.Add(MAttNew);
                                            }

                                            message.IsBodyHtml = true;
                                            message.Priority = System.Net.Mail.MailPriority.Normal;
                                            message.Subject = Subject;
                                            message.Body = MsgBody;

                                            client.Send(message);
                                            WriteLog(1, message.Subject, message.Body, stage.Trim(), journalName.Trim(), article, "After Sending mail");
                                            System.Threading.Thread.Sleep(500);
                                            ResultMsg = "Task closed successfully. Mail has been sent to " + EmailTo + "; " + EmailToJobAuthor;
                                            MessageLabel.Text = ResultMsg.Trim();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog(1, Subject, MsgBody, stage.Trim(), journalName.Trim(), article.Trim(), ex.Message);
                                        throw new Exception(ex.Message, ex);
                                    }
                                    #endregion NewMailCode
                                    signalMessage.AppendLine( "Mail has been sent to " + EmailTo);
                                    if (Session["zipFile"] != null)
                                        ptsProblem.ZipFilePath = Session["zipFile"].ToString().Trim();
                                    else
                                        ptsProblem.ZipFilePath = string.Empty;
                                    signalMessage.AppendLine(ptsQueryService.UpdateStatus(ptsProblem).ToString());
                                }

                            }
                            else if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "0")
                            {
                                MessageLabel.Text = "Signal file path not configured";
                                return;
                            }
                            else if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "1")
                            {
                                MessageLabel.Text = "Email Details not configured";
                                return;
                            }
                        } //end if raise query
                        else
                        {
                            if (jobType==articleType)
                                signalMessage.AppendLine(validationMessage + " (" + article + ")");
                            else if (jobType==issueType)
                                signalMessage.AppendLine(validationMessage + " (" + volume + "/" + issue + ")");
                        }

                       

                    } //end if gvcheckbox checked

        } //end for each

               // MessageLabel.Text = signalMessage.ToString();
                //BindDueArticles(ddlSortOrder.SelectedItem.Text.Trim());
            }   //end try
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }
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
        protected void btnMerge_Click(object sender, EventArgs e)
        {
            string problemTypeDesc=string.Empty;
            foreach (GridViewRow gRow in GVDueArticles.Rows)
            {
                CheckBox gvCheckBox = (CheckBox)gRow.FindControl("GVCheckBox");
                if (gvCheckBox.Checked)
                {
                    Label gvLabel = (Label)gRow.FindControl("GVLabelRemarks");
                    problemTypeDesc =problemTypeDesc+ gvLabel.Text.Trim();
                }
            }
        }

        #endregion

        public void WriteLog(int flagtouselog, string EmailSub, string EmailBody, string Stage, string JourName, string Article, string ExecMsg)
        {
            try
            {

                flagtouselog = Convert.ToInt16(ConfigurationManager.AppSettings["LogReqd"].ToString().Trim());
                if (flagtouselog == 1)
                {
                    string fileNameWithPath = "";
                    string foldername = Convert.ToString(DateTime.Now.ToString("dd_MM_yyyy"));
                    string logpath = System.Configuration.ConfigurationSettings.AppSettings.Get("SentEmailLog");

                    fileNameWithPath = logpath + foldername + "\\";
                    string ErrFile = JourName.Trim() + "_" + Article.Trim() + "_" + Stage.Trim() + ".log";

                    if (!System.IO.Directory.Exists(fileNameWithPath))
                        System.IO.Directory.CreateDirectory(fileNameWithPath);
                    if (!(string.IsNullOrEmpty(fileNameWithPath + ErrFile)))
                    {
                        FileStream fileStream = new FileStream(fileNameWithPath + ErrFile, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter objStreamWriter = new StreamWriter(fileStream);
                        objStreamWriter.BaseStream.Seek(0, SeekOrigin.End);

                        if (EmailSub.Trim() != "")
                        {
                            EmailSub = "Email Subject" + "  : ->  " + EmailSub;
                            objStreamWriter.WriteLine(EmailSub);

                            EmailBody = "Email Content   : ->  " + EmailBody;
                            objStreamWriter.WriteLine(EmailBody);
                        }

                        Stage = "Stage           : ->  " + Stage;
                        objStreamWriter.WriteLine(Stage);

                        JourName = "Journal Name    : ->  " + JourName;
                        objStreamWriter.WriteLine(JourName);

                        Article = "Article        : ->  " + Article;
                        objStreamWriter.WriteLine(Article);

                        string dtTime = "Date            : ->  " + DateTime.Now.ToString();
                        objStreamWriter.WriteLine(dtTime);

                        ExecMsg = "Message      : ->  " + ExecMsg;
                        objStreamWriter.WriteLine(ExecMsg);

                        string LineSpace = "-------------------------------------------------------  END  ------------------------------------------------------";
                        objStreamWriter.WriteLine(LineSpace + Environment.NewLine);


                        objStreamWriter.Flush();
                        objStreamWriter.Close();
                        // }
                    }
                    else
                    {
                        System.IO.File.Create(fileNameWithPath + ErrFile);


                        FileStream fileStream = new FileStream(fileNameWithPath + ErrFile, FileMode.OpenOrCreate, FileAccess.Write);
                        StreamWriter objStreamWriter = new StreamWriter(fileStream);
                        objStreamWriter.BaseStream.Seek(0, SeekOrigin.End);

                        if (EmailSub.Trim() != "")
                        {
                            EmailSub = "Email Subject   : ->  " + EmailSub;
                            objStreamWriter.WriteLine(EmailSub);

                            EmailBody = "Email Content   : ->  " + EmailBody;
                            objStreamWriter.WriteLine(EmailBody);
                        }

                        Stage = "Stage           : ->  " + Stage;
                        objStreamWriter.WriteLine(Stage);

                        JourName = "Journal Name    : ->  " + JourName;
                        objStreamWriter.WriteLine(JourName);

                        Article = "Article         : ->  " + Article;
                        objStreamWriter.WriteLine(Article);

                        string dtTime = "Date" + "           : ->  " + DateTime.Now.ToString();
                        objStreamWriter.WriteLine(dtTime);

                        ExecMsg = "Message      : ->  " + ExecMsg;
                        objStreamWriter.WriteLine(ExecMsg);

                        string LineSpace = "-------------------------------------------------------  END  ------------------------------------------------------";
                        objStreamWriter.WriteLine(LineSpace + Environment.NewLine);

                        objStreamWriter.Flush();
                        objStreamWriter.Close();

                    }
                }

                else
                {
                    return;
                }
            }
            catch (Exception logException)
            {
                throw logException;
            }

        }


        #region User defined methods

        private void BindDueArticles(string OrderByColumn)
        {
            DataTable dueArticles = new DataTable();
            dueArticles = dueForCloseService.GetDueForClosingArticlesByCustomer(stage, Request.QueryString["Customer"], OrderByColumn);

            GVDueArticles.DataSource = dueArticles;
            GVDueArticles.DataBind();

        }

        private bool DoValidations(out string validationMessage)
        {
            DataTable dtValidationQueries = new DataTable();
            string validationQuery = string.Empty;
            bool expectedResult = false;
            bool isSP = false;


            validationMessage = string.Empty;
            dtValidationQueries = ptsQueryService.GetValidationQueries(customerCode, stageID);

            for (int i = 0; i < dtValidationQueries.Rows.Count; i++)
            {
                validationQuery = dtValidationQueries.Rows[i]["ValidationQuery"].ToString().Trim();
                validationQuery = validationQuery.ToLower().Replace("<customercode>", customerCode.ToString()).Replace("<journalcode>", journalCode.ToString()).Replace("<article>", article).Replace("<stage>", stage).Replace("<stageid>", stageID.ToString());
                validationQuery = validationQuery.ToLower().Replace("<volume>", volume).Replace("<issue>", issue);

                validationMessage = dtValidationQueries.Rows[i]["ValidationMessage"].ToString().Trim();
                expectedResult = bool.Parse(dtValidationQueries.Rows[i]["ValidationResult"].ToString());

                if (dtValidationQueries.Rows[i]["isSP"] != DBNull.Value)
                {
                    bool.TryParse(dtValidationQueries.Rows[i]["isSP"].ToString(), out isSP);
                }
                else
                    isSP = false;

                if (!ValidationPassed(validationQuery, expectedResult, isSP))
                {
                    raiseQuery = false;
                    break;
                }
            }

            return raiseQuery;
        }

        private bool ValidationPassed(string strsql, bool expectedResult, bool isSP)
        {
            bool rFlag = false;
            bool returnValue = false;

            if (isSP)
            {
                string[] strarr = strsql.Split(';');

                string spName = string.Empty;
                string[] spParameters = null;

                if (strarr.Length > 1)
                {
                    spName = strarr[0].Trim();
                    spParameters = strarr[1].Split(',');

                    rFlag = ptsQueryService.PTSValidation(spName, spParameters);
                }
            }
            else
            {
                rFlag = ptsQueryService.PTSValidation(strsql);
            }

            if (rFlag == expectedResult)
                returnValue = true;
            else
                returnValue = false;

            return returnValue;
        }

        #endregion


        protected void ddlSortOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
           // BindDueArticles(ddlSortOrder.SelectedItem.Text);
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                PTSProblem ptsProblem;

                if (ViewState["jobType"] != null)
                    jobType = (string)ViewState["jobType"];

                foreach (GridViewRow gRow in GVDueArticles.Rows)
                {

                    //read customer-journal-artticle details from the grid view
                    CheckBox gvCheckBox = (CheckBox)gRow.FindControl("GVCheckBox");
                    if (gvCheckBox.Checked)
                    {
                        ptsProblem = new PTSProblem();

                        Label gvLabel = (Label)gRow.FindControl("GVLabelCustomerCode");
                        int.TryParse(gvLabel.Text.Trim(), out customerCode);

                        gvLabel = (Label)gRow.FindControl("GVLabelJournalCode");
                        int.TryParse(gvLabel.Text.Trim(), out journalCode);

                        if (jobType == articleType)
                        {
                            gvLabel = (Label)gRow.FindControl("GVLabelArticle");
                            article = gvLabel.Text.Trim();

                            volume = string.Empty;
                            issue = string.Empty;
                        }
                        else if (jobType == issueType)
                        {
                            gvLabel = (Label)gRow.FindControl("GVLabelVolume");
                            volume = gvLabel.Text.Trim();

                            gvLabel = (Label)gRow.FindControl("GVLabelIssue");
                            issue = gvLabel.Text.Trim();

                            article = string.Empty;
                        }

                        gvLabel = (Label)gRow.FindControl("GVLabelProblemTypeCode");
                        ProblemTypeId = gvLabel.Text.Trim();

                        MessageLabel.Text = dueForCloseService.DeleteDueForCloseArticle(stage, customerCode, journalCode, article, volume, issue, ProblemTypeId);

                        gvLabel = (Label)gRow.FindControl("GVLabelZipFilePath");
                        string ZipFilePath = string.Empty;
                        ZipFilePath = gvLabel.Text.Trim();
                        if (File.Exists(ZipFilePath))
                        {
                            File.Delete(ZipFilePath);
                        }

                    }
                }
               // BindDueArticles(ddlSortOrder.SelectedItem.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }
    }
}
