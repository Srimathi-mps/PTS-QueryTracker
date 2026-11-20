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
using System.Collections.Generic;
using MPS_ManuscriptTracking.PTSBusinessLayer;
using MPS_ManuscriptTracking.PTSDataAccessLayer;
using System.Data.SqlClient;
using System.IO;
namespace QueryTracker
{
    public partial class RaiseProblem : System.Web.UI.Page
    {
        int Grid_TempChek = 0;
        List<string> lstPType = new List<string>();
        BizPTSQuery objBizPTSQuery = new BizPTSQuery();
        IPTSQuery ptsQueryService = new PTSQueryService();
        PTSProblem objPTSProblem = new PTSProblem();
        string xmlValue = string.Empty;
        string articleType = ConfigurationManager.AppSettings["ArticleType"].ToString().Trim();
        string issueType = ConfigurationManager.AppSettings["IssueType"].ToString().Trim();
        public string SmtpAddr = ConfigurationSettings.AppSettings.Get("SMTPAddr");
        public static SqlConnection itrackCon = new SqlConnection(ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString);
        public string CustCode { get; set; }
        public string JourCode { get; set; }
        public string ArtCode { get; set; }
        protected string userName = "";
        //protected string userName = "MAC006081";
        int customerCode;
        int journalCode;
        string article = string.Empty;
        string stage = string.Empty;
        string volume = string.Empty;
        string issue = string.Empty;
        int stageID = 0;

        string jobType = string.Empty;
        int deptCode = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            //WriteLog("Soumya " + Session["Username"]);
            // "TUFDMDA5MjU4-8ZNxmTXKXWQ%3d"
            //debug
            //Session["Encrypted User"] = "TUFDMDA5MjU4-8ZNxmTXKXWQ%3d";

            if (System.Web.HttpContext.Current.Request.QueryString["Username"] != null)
            {
                Session["Encrypted User"] = System.Web.HttpContext.Current.Request.QueryString["Username"].ToString();


                
            }
            //debug
           // string User = "mac009258";
            //string User = Global.Validate();
            string User = "MAC014393"; // Changed by srimathi on 28-Apr-2026


            if (User == "") { Session["Username"] = ""; }
            else { Session["Username"] = User; }
            if (Session["Username"] != null)
            {
                userName = Session["Username"].ToString().Trim();
                Session["userName"] = userName;

               WriteLog("Before Global not null: " + Session["Username"] + "UserName" + userName);
            }
            else
            {
                WriteLog("Before Global  null:  " + Session["Username"]);
                userName = Session["userName"].ToString();
                Session["userName"] = userName;
				  WriteLog("Before Global not null:  " + Session["Username"] + "UserName" + userName);
            }

            try
            {
                // Session["ZipFilePath"] = "test";
                if (!Page.IsPostBack)
                {
                    string result_msg = "";
                    if (Request.QueryString["Redirect_msg"] != null)
                    {
                        result_msg = Request.QueryString["Redirect_msg"].ToString();
                    }
                    if (result_msg == "Task Raised successfully")
                    {
                       
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', '" + result_msg + "', 'Success', 'top', 'right');", true);
                    }
                    if (result_msg == "Insertion successful.")
                    {

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', '" + result_msg + "', 'Success', 'top', 'right');", true);
                    }
                    if (result_msg == "QUERY RESOLVED SUCCESSFULLY")
                    {
                       
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('success', '" + result_msg + "', 'Success', 'top', 'right');", true);
                    }
                    WriteLog("Line1");
                    clearAllControls();
                    WriteLog("Line2");
                    StageDropDown1.Visible = false;
                    FillCustomerDDL();
                    WriteLog("Line3");

                    if (Request.QueryString["barCode"] != null)
                    {
                        if (Request.QueryString["barCode"] != string.Empty)
                        {
                            txtBarcode.Text = Request.QueryString["barCode"].ToString().Trim();
                            Session["BarcodeVal"] = Request.QueryString["barCode"].ToString().Trim();
                            GetDataFromBarCode();
                            AssignSessionValues();
                            if (Request.QueryString["OpenedRequest"] != null)
                            {
                                QueryBind();
                            }
                        }
                    }
                    else
                    {
                        Session["userName"] = userName;
                        //if (Session["ResultMsg"] != null)
                        //    if (Session["ResultMsg"].ToString().Trim() != string.Empty)
                        //        ShowMessage(Session["ResultMsg"].ToString().Trim());

                       // Global Go = new Global();
                        WriteLog("User Name at RaiseProblems-> : " + userName);

                        Session["userName"] = userName;
                        roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");

                   
                        WriteLog("roleID at RaiseProblems : " + roleID);
                       
                        if (roleID <= 0)
                        {
                            MessageLabel.Text = "User role not defined";
                            System.Web.HttpContext.Current.Response.Redirect("BlankPage.aspx");
                            WriteLog("roleID at RaiseProblems1 : " + roleID);
                            return;
                        }


                        //&& Session["Journal"].ToString()!="0"
                        //If you retain all values like dropdown, opened requests , enable the below 
                        if (Session["Journal"] != null) //  if (Session["Journal"] != null)//Commented by sathish.J on 20-07-2017
                        {
                            if (Session["Journal"].ToString() != string.Empty)
                            {

                                CustomerDropDown.SelectedIndex = CustomerDropDown.Items.IndexOf(CustomerDropDown.Items.FindByValue(Session["Customer"].ToString().Trim()));

                                FillJournalDDL(Convert.ToInt32(CustomerDropDown.SelectedValue));
                                JournalDropDown.SelectedIndex = JournalDropDown.Items.IndexOf(JournalDropDown.Items.FindByValue(Session["Journal"].ToString().Trim()));

                                //FillArticleDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), "");
                                //ArticleDropDown.SelectedIndex = ArticleDropDown.Items.IndexOf(ArticleDropDown.Items.FindByText(Session["Article"].ToString().Trim()));

                                customerCode = int.Parse(CustomerDropDown.SelectedValue);
                                journalCode = int.Parse(JournalDropDown.SelectedValue);
                                WriteLog("Line4");
                                FillArticleDDL(customerCode, journalCode, "");
                                WriteLog("Line5");
                                if (select1_ArticleDropDown.Items.Count > 0)
                                {
                                  //  FillStageDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text);
                                    StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByValue(Session["Stage"].ToString().Trim()));
                                    gvwOpenedRequest.Visible = false;
                                    btnGoDdl.Visible = true;
                                }

                                TextBox3.Text = Session["Isbn"].ToString().Trim();
                                FillDeptDDL();
                                WriteLog("Line6");
                                DeptDropDown.SelectedIndex = DeptDropDown.Items.IndexOf(DeptDropDown.Items.FindByText(Session["Dept"].ToString().Trim()));
                                WriteLog("Line6Dept"+ Session["Dept"].ToString().Trim());

                                //if (StageDropDown.SelectedIndex > 0)
                                //{
                                //FillVolumeDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), StageDropDown.SelectedValue.ToString());
                                //FillVolumeDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), "0");
                                //VolumeDropDown.SelectedIndex = VolumeDropDown.Items.IndexOf(VolumeDropDown.Items.FindByValue(Session["Volume"].ToString().Trim()));
                                //}
                                //FillIssueDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), VolumeDropDown.SelectedValue.ToString());
                                //IssueDropDown.SelectedIndex = IssueDropDown.Items.IndexOf(IssueDropDown.Items.FindByValue(Session["Issue"].ToString().Trim()));

                                QueryBind();
                                WriteLog("Line7");
                                //SortInPreviousOrder();
                            }
                        }


                        if (Session["BarcodeVal"] != null)
                        {
                            if (Session["BarcodeVal"].ToString() != string.Empty)
                            {
                                txtBarcode.Text = Session["BarcodeVal"].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message.Trim();
                WriteLog("Exception" + ex.ToString());
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
        private void clearAllControls()
        {
            try
            {
                #region clearControls
                clearControl("txt", (Control)txtBarcode);
                clearControl("ddl", (Control)CustomerDropDown);
                clearControl("ddl", (Control)JournalDropDown);
                clearControl("ddl", (Control)ArticleDropDown);
                //added by divya 09-02-2026
                clearControl("txt", (Control)TextBox3);
                //clearControl("ddl", (Control)VolumeDropDown);
                //clearControl("ddl", (Control)IssueDropDown);
                //commented by sathish.j on 08-may-2018
                //clearControl("ddl", (Control)StageDropDown);
                //clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                //btnGoDdl.Visible = false; //COmmented by sathish.J on 14-07-2017
                btnRaiseProblem.Visible = false;

                btnNew_Instruction.Visible = false;
                #endregion clearControls
            }
            catch { throw; }
        }
        protected void OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int problemId = Convert.ToInt32(gvwOpenedRequest.DataKeys[e.RowIndex].Values[0]);
            int temp = 0;
            string[] sqlparams = { problemId.ToString(), "", "delete" };
            SqlHelper sql = new SqlHelper();
            temp = sql.GetExecuteFromSP("USP_deletequery", sqlparams, 2);

            int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
            int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

            objPTSProblem.CustomerCode = CustomerID;
            objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

            // Use unified article selection logic
            objPTSProblem.Article = GetArticleSelection(CustomerID);

            objPTSProblem.Volume = "";
            //else
            objPTSProblem.Issue = "";
            objPTSProblem.StageID = SubStageID;
            BindOpenedRequests(objPTSProblem);
        }
        private void QueryBind()
        {
            stage = string.Empty;

            try
            {
                #region clearControls
                MessageLabel.Text = string.Empty;

                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;

                xmlValue = string.Empty;
                #endregion clearControls

                MasterCopyJournalLabel.Visible = false;

                if (StageDropDown.SelectedIndex > 0)
                {
                    
                    int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
                    int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

                    objPTSProblem.CustomerCode = CustomerID;
                    objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

                    // Stage-based customer (163): Use direct dropdown text
                    objPTSProblem.Article = GetArticleSelection(CustomerID);

                    //if (VolumeDropDown.SelectedIndex >= 0)
                    //    objPTSProblem.Volume = VolumeDropDown.SelectedItem.Text;
                    //else
                    objPTSProblem.Volume = "";
                    //if (IssueDropDown.SelectedIndex >= 0)
                    //    objPTSProblem.Issue = IssueDropDown.SelectedItem.Text;
                    //else
                    objPTSProblem.Issue = "";

                    objPTSProblem.StageID = SubStageID;
                    BindOpenedRequests(objPTSProblem);
                }
                else if (StageDropDown1.SelectedIndex > 0)
                {

                    int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
                    int SubStageID = Convert.ToInt32(StageDropDown1.SelectedValue.ToString().Trim());

                    objPTSProblem.CustomerCode = CustomerID;
                    objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

                    // Sub-stage customer (877): Use multi-select Chapter_selection1()
                    objPTSProblem.Article = GetArticleSelection(CustomerID);

                    //if (VolumeDropDown.SelectedIndex >= 0)
                    //    objPTSProblem.Volume = VolumeDropDown.SelectedItem.Text;
                    //else
                    objPTSProblem.Volume = "";
                    //if (IssueDropDown.SelectedIndex >= 0)
                    //    objPTSProblem.Issue = IssueDropDown.SelectedItem.Text;
                    //else
                    objPTSProblem.Issue = "";

                    objPTSProblem.StageID = SubStageID;
                    BindOpenedRequests(objPTSProblem);
                }
                else
                {
                    #region clearControls
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls

                    MasterCopyJournalLabel.Visible = false;
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
        public void AssignSessionValues()
        {
            Session["Customer"] = CustomerDropDown.SelectedValue;
            Session["CustomerName"] = CustomerDropDown.SelectedItem.Text.Trim();
            Session["Journal"] = JournalDropDown.SelectedValue;
            Session["JournalName"] = JournalDropDown.SelectedItem.Text.Trim();
            if (ArticleDropDown.SelectedIndex > 0)
                Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
            else
                Session["Article"] = string.Empty;

            //if (VolumeDropDown.SelectedIndex > 0)
            //    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
            //else
            Session["Volume"] = string.Empty;

            //if (IssueDropDown.SelectedIndex > 0)
            //    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
            //else
            Session["Issue"] = string.Empty;

            //Session["Volume"] = VolumeDropDown.SelectedValue;
            //Session["Issue"] = IssueDropDown.SelectedValue;
            Session["Stage"] = StageDropDown.SelectedValue;
            Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();

            Session["RoundCount"] = ddlRoundCount.SelectedValue;
            Session["lstPType"] = lstPType;
        }
        private void GetDataFromBarCode()
        {
            CustomerDropDown.SelectedIndex = -1;

            if (Session["BarcodeVal"] == null)
            {
                Session["BarcodeVal"] = txtBarcode.Text;
            }
            if (Session["BarcodeVal"] != null)
            {
                if (Session["BarcodeVal"].ToString() != string.Empty)
                {
                    MessageLabel.Text = string.Empty;
                    string customerGroupName = string.Empty;
                    string journalName = string.Empty;
                    string articleID = string.Empty;
                    //string volume = string.Empty;
                    //string issue = string.Empty;

                    //string barcode = txtBarcode.Text.Trim();
                    string barcode = Session["BarcodeVal"].ToString().Trim();
                    string[] barcodeArr = barcode.Split('|');
                    string errorMessage = string.Empty;

                    //ClearFields();


                    try
                    {
                        DataTable dtPTCategory = new DataTable();
                        ViewState["dtPTCategory"] = dtPTCategory;
                        //gvwOpenedRequest.DataSource = dtPTCategory;
                        //gvwOpenedRequest.DataBind();
                        clearControl("gvw", gvwOpenedRequest);

                        xmlValue = string.Empty;
                        DataTable dt = new DataTable();

                        ////Get Customer journal article or get customer joutnal volume issue
                        if (barcodeArr.Length == 3)
                        {
                            ////customerGroupName is now custFName as per RMT#13386 updated in sp
                            customerGroupName = barcodeArr[0].Trim();
                            journalName = barcodeArr[1].Trim();
                            articleID = barcodeArr[2].Trim();
                            JobTypeHidden.Value = articleType.ToString().Trim();
                            Session["jobType"] = JobTypeHidden.Value.Trim();

                            objBizPTSQuery.Article = articleID;

                            errorMessage = ptsQueryService.ValidateCustomerJournalArticle(customerGroupName, journalName, articleID);
                            if (errorMessage.Trim() != string.Empty)
                            {
                                Session["BarcodeVal"] = string.Empty;
                                DataTable dtOpenedRequest = new DataTable();
                                Session["OpenedRequest"] = dtOpenedRequest;
                                gvwOpenedRequest.DataSource = dtOpenedRequest;
                                gvwOpenedRequest.DataBind();
                                btnRaiseProblem.Visible = false;
                                btnNew_Instruction.Visible = false;
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', '" + errorMessage + "', 'Required', 'top', 'right');", true);
                                //ShowMessage(errorMessage);
                            }
                            else
                            {
                                dt = ptsQueryService.GetCustomerAndJournalCode(customerGroupName, journalName);

                                if (dt.Rows.Count > 0)
                                {
                                    customerCode = int.Parse(dt.Rows[0]["cust_code"].ToString());
                                    journalCode = int.Parse(dt.Rows[0]["jour_code"].ToString());

                                    objBizPTSQuery.CustomerCode = customerCode;
                                    objBizPTSQuery.JournalCode = journalCode;

                                    if (CustomerDropDown.Items.FindByValue(customerCode.ToString()) != null)
                                    {
                                        CustomerDropDown.SelectedValue = customerCode.ToString();
                                        //CustomerDropDown.SelectedValue = customerCode.ToString();

                                        FillJournalDDL(customerCode);
                                        if (JournalDropDown.Items.FindByValue(journalCode.ToString()) != null)
                                        {
                                            JournalDropDown.SelectedValue = journalCode.ToString();

                                            stage = ptsQueryService.GetStage(customerCode, journalCode, articleID, out volume, out issue);
                                            if (stage.Trim() == string.Empty)
                                            {
                                                btnRaiseProblem.Visible = false;
                                                btnNew_Instruction.Visible = false;
                                                //ShowMessage("The article entered does not exist");
                                                Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'The article entered does not exist', 'Required', 'top', 'right');", true);
                                                //ClearFields();
                                                return;
                                            }
                                            else
                                            {
                                                btnRaiseProblem.Visible = true;
                                                btnNew_Instruction.Visible = true;
                                                FillArticleDDL(customerCode, journalCode, "");
                                                ArticleDropDown.SelectedIndex = ArticleDropDown.Items.IndexOf(ArticleDropDown.Items.FindByText(articleID.ToUpper()));
                                                FillDeptDDL();
                                            }

                                            //  GetDetails();
                                        }
                                    }
                                }
                            }
                        }
                        else if (barcodeArr.Length == 4)
                        {
                            customerGroupName = barcodeArr[0].Trim();
                            journalName = barcodeArr[1].Trim();
                            volume = barcodeArr[2].Trim();
                            issue = barcodeArr[3].Trim();
                            JobTypeHidden.Value = issueType.ToString().Trim();
                            Session["jobType"] = JobTypeHidden.Value.Trim();

                            objBizPTSQuery.Volume = volume;
                            objBizPTSQuery.Issue = issue;

                            dt = ptsQueryService.GetCustomerAndJournalCode(customerGroupName, journalName);

                            if (dt.Rows.Count > 0)
                            {
                                customerCode = int.Parse(dt.Rows[0]["cust_code"].ToString());
                                journalCode = int.Parse(dt.Rows[0]["jour_code"].ToString());

                                objBizPTSQuery.CustomerCode = customerCode;
                                objBizPTSQuery.JournalCode = journalCode;

                                if (CustomerDropDown.Items.FindByValue(customerCode.ToString()) != null)
                                {
                                    CustomerDropDown.SelectedValue = customerCode.ToString();
                                    //CustomerDropDown.SelectedValue = customerCode.ToString();

                                    FillJournalDDL(customerCode);
                                    if (JournalDropDown.Items.FindByValue(journalCode.ToString()) != null)
                                    {
                                        ArticleDropDown.SelectedIndex = -1;
                                        JournalDropDown.SelectedValue = journalCode.ToString();
                                        //JournalDropDown.SelectedValue = journalCode.ToString();
                                        //FillVolumeDDL(customerCode, journalCode, "0");
                                        ////FillVolumeDDL(customerCode, journalCode,Session["Stage"].ToString().Trim());

                                        //if (VolumeDropDown.Items.FindByValue(volume.ToString()) != null)
                                        //{
                                        //    VolumeDropDown.SelectedValue = volume;
                                        //    ////FillIssueDDL(customerCode, journalCode, Session["Stage"].ToString().Trim());

                                        //    FillIssueDDL(customerCode, journalCode, volume);
                                        //    if (IssueDropDown.Items.FindByValue(issue.ToString()) != null)
                                        //    {
                                        //        IssueDropDown.SelectedValue = issue;

                                        //        ////btnRaiseProblem.Visible = true;
                                        //        if (VolumeDropDown.SelectedIndex > 0 && IssueDropDown.SelectedIndex > 0)
                                        //            GetDetails();
                                        //    }
                                        //}
                                    }
                                }
                            }
                            if (CustomerDropDown.SelectedIndex <= 0)
                            {
                                CustomerDropDown.SelectedIndex = -1;
                                clearControl("ddl", (Control)JournalDropDown);
                                clearControl("ddl", (Control)ArticleDropDown);
                                //clearControl("ddl", (Control)VolumeDropDown);
                                //clearControl("ddl", (Control)IssueDropDown);
                                clearControl("ddl", (Control)StageDropDown);
                                clearControl("ddl", (Control)ddlRoundCount);

                                btnRaiseProblem.Visible = false;
                                btnNew_Instruction.Visible = false;
                                //ShowMessage("The customer/journal entered does not exist");
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'customer/journal entered does not exist', 'Required', 'top', 'right');", true);
                                return;
                            }
                            //if (VolumeDropDown.Items.IndexOf(VolumeDropDown.Items.FindByText(volume)) < 0)
                            //{
                            //    CustomerDropDown.SelectedIndex = -1;
                            //    clearControl("ddl", (Control)JournalDropDown);
                            //    clearControl("ddl", (Control)ArticleDropDown);
                            //    clearControl("ddl", (Control)VolumeDropDown);
                            //    clearControl("ddl", (Control)IssueDropDown);
                            //    clearControl("ddl", (Control)StageDropDown);
                            //    clearControl("ddl", (Control)ddlRoundCount);


                            //    btnRaiseProblem.Visible = false;
                            //    ShowMessage("The volume entered does not exist");
                            //    return;
                            //}
                            //else if (IssueDropDown.Items.IndexOf(IssueDropDown.Items.FindByText(issue)) < 0)
                            //{
                            //    CustomerDropDown.SelectedIndex = -1;
                            //    clearControl("ddl", (Control)JournalDropDown);
                            //    clearControl("ddl", (Control)ArticleDropDown);
                            //    clearControl("ddl", (Control)VolumeDropDown);
                            //    clearControl("ddl", (Control)IssueDropDown);
                            //    clearControl("ddl", (Control)StageDropDown);
                            //    clearControl("ddl", (Control)ddlRoundCount);

                            //    btnRaiseProblem.Visible = false;
                            //    ShowMessage("The issue entered does not exist");
                            //    return;
                            //}
                            //VolumeDropDown.SelectedValue = volume;
                            //IssueDropDown.SelectedValue = issue;
                            FillArticleDDL(customerCode, journalCode, "");
                            btnRaiseProblem.Visible = true;
                            btnNew_Instruction.Visible = true;
                        }
                        else
                        {
                            btnRaiseProblem.Visible = false;
                            btnNew_Instruction.Visible = false;
                            //ShowMessage("Invalid barcode");\
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Invalid barcode', 'Required', 'top', 'right');", true);
                            JobTypeHidden.Value = string.Empty;
                            Session["jobType"] = JobTypeHidden.Value.Trim();

                            clearControl("ddl", JournalDropDown);
                            clearControl("ddl", ArticleDropDown);
                            clearControl("ddl", StageDropDown);
                            //clearControl("ddl", VolumeDropDown);
                            //clearControl("ddl", IssueDropDown);
                            clearControl("ddl", ddlRoundCount);

                            return;
                        }
                        GetDetails();
                    }
                    catch (Exception ex)
                    {

                        WriteLog("exception : " + ex.ToString());
                    }
                }
                else
                {
                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    CustomerDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)JournalDropDown);
                    clearControl("ddl", (Control)ArticleDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                }
            }
        }
        #region FillControlsMethods
        private void FillCustomerDDL()
        {
            DataTable dtCustomer = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtCustomer = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Customer, ptsQuery, userName);

            CustomerDropDown.DataSource = dtCustomer;
            CustomerDropDown.DataTextField = Global.DB_FIELD_NAME;
            CustomerDropDown.DataValueField = Global.DB_FIELD_ID;
            CustomerDropDown.DataBind();

            CustomerDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }
        private void FillJournalDDL(int customerCode)
        {
            DataTable dtJournal = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            ptsQuery.CustomerCode = customerCode;
            dtJournal = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Journal, ptsQuery, "0");

            JournalDropDown.DataSource = dtJournal;
            JournalDropDown.DataTextField = Global.DB_FIELD_NAME;
            JournalDropDown.DataValueField = Global.DB_FIELD_ID;
            JournalDropDown.DataBind();
            JournalDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
            MasterCopyJournalLabel.Visible = false;
        }
        private void FillArticleDDL(int customerCode, int journalCode, string Stage)
        {
            DataTable dtArticle = new DataTable();
            dtArticle = ptsQueryService.GetWIPArticles(customerCode, journalCode, Stage);
            SqlHelper sql = new SqlHelper();
            sql.BindData_select(select1_ArticleDropDown, dtArticle, 1);


            ArticleDropDown.DataSource = dtArticle;
            ArticleDropDown.DataTextField = Global.DB_FIELD_ID.Trim();
            ArticleDropDown.DataValueField = Global.DB_FIELD_ID.Trim();
            ArticleDropDown.DataBind();
            ArticleDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }
        private void FillDeptDDL()
        {
            DataTable dtDepartment = new DataTable();
            dtDepartment = ptsQueryService.GetDepartment();
            DeptDropDown.DataSource = dtDepartment;
            DeptDropDown.DataValueField = "FieldId";
            DeptDropDown.DataTextField = "FieldName";
            DeptDropDown.DataBind();
            DeptDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }
        //private void FillVolumeDDL(int customerCode, int journalCode, string StageId)
        //{
        //    DataTable dtVolumeIssue = new DataTable();

        //    dtVolumeIssue = ptsQueryService.GetWIPVolumeIssues("volume", customerCode, journalCode, StageId);
        //    VolumeDropDown.DataSource = dtVolumeIssue;
        //    VolumeDropDown.DataValueField = Global.DB_FIELD_ID.Trim();
        //    VolumeDropDown.DataTextField = Global.DB_FIELD_ID.Trim();
        //    VolumeDropDown.DataBind();
        //    VolumeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));


        //}
        //private void FillIssueDDL(int customerCode, int journalCode, string VolID)
        //{
        //    DataTable dtVolumeIssue = new DataTable();

        //    dtVolumeIssue = ptsQueryService.GetWIPVolumeIssues("issue", customerCode, journalCode, VolID);
        //    IssueDropDown.DataSource = dtVolumeIssue;
        //    IssueDropDown.DataTextField = Global.DB_FIELD_ID.Trim().ToLower();
        //    IssueDropDown.DataValueField = Global.DB_FIELD_ID.Trim().ToLower();
        //    IssueDropDown.DataBind();
        //    IssueDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

        //}
        private void FillStageDDL(int custId, int jourId, string articleid)
        {
            DataTable dtStages = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            // dtStages = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Stage, ptsQuery);
            dtStages = ptsQueryService.GetSubStageDetails(custId, jourId, articleid);

            StageDropDown.DataSource = dtStages;
            StageDropDown.DataTextField = Global.DB_FIELD_NAME;
            StageDropDown.DataValueField = Global.DB_FIELD_ID;
            StageDropDown.DataBind();
            StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }
        private void FillStageDDL(int custId, int jourId, string Volumn, string issue)
        {
            DataTable dtStages = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            // dtStages = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Stage, ptsQuery);
            dtStages = ptsQueryService.GetSubStageDetails(custId, jourId, Volumn, issue);

            StageDropDown.DataSource = dtStages;
            StageDropDown.DataTextField = Global.DB_FIELD_NAME;
            StageDropDown.DataValueField = Global.DB_FIELD_ID;
            StageDropDown.DataBind();
            StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }
        private void FillRoundCountDDL(int CustCode, int JourCode, string Article)
        {
            DataTable dtVolIssRoundCount = new DataTable();

            dtVolIssRoundCount = ptsQueryService.GetWIPRoundCountByArticle(CustCode, JourCode, Article);
            ddlRoundCount.DataSource = dtVolIssRoundCount;
            ddlRoundCount.DataValueField = Global.DB_FIELD_ID.Trim();
            ddlRoundCount.DataTextField = Global.DB_FIELD_NAME.Trim();
            ddlRoundCount.DataBind();
            ddlRoundCount.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

        }
        //private void FillVolIssRoundCountDDL(int CustCode, int JourCode, string Volume, string Issue)
        //{
        //    DataTable dtVolIssRoundCount = new DataTable();

        //    dtVolIssRoundCount = ptsQueryService.GetWIPVolIssRoundCount(CustCode, JourCode, Volume, Issue);
        //    ddlRoundCount.DataSource = dtVolIssRoundCount;
        //    ddlRoundCount.DataValueField = Global.DB_FIELD_ID.Trim();
        //    ddlRoundCount.DataTextField = Global.DB_FIELD_NAME.Trim();
        //    ddlRoundCount.DataBind();
        //    ddlRoundCount.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

        //    if (dtVolIssRoundCount.Rows.Count == 1)
        //        ddlRoundCount.SelectedIndex = 1;
        //    else
        //        ddlRoundCount.SelectedIndex = 0;
        //}
        private void FillStageDDLVolIss(int custId, int jourId, string Volme, string Issue)
        {
            DataTable dtStages = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();
            dtStages = ptsQueryService.GetSubStageDetailsVolIss(custId, jourId, Volme, Issue);
            StageDropDown.DataSource = dtStages;
            StageDropDown.DataTextField = Global.DB_FIELD_NAME;
            StageDropDown.DataValueField = Global.DB_FIELD_ID;
            StageDropDown.DataBind();
            StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }

        #endregion FillControlsMethods

        private void GetDetails()
        {
            MessageLabel.Text = string.Empty;
            JobTypeHidden.Value = "1";
            Session["jobType"] = JobTypeHidden.Value.Trim();
            volume = string.Empty;
            issue = string.Empty;
            //ddlOpenedRequest.Items.Clear();//now
            string problemTypeCode = string.Empty;
            string problemDescription = string.Empty;
            string deptId = string.Empty;

            customerCode = Convert.ToInt32(CustomerDropDown.SelectedValue.Trim());
            journalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());
            objPTSProblem.CustomerCode = customerCode;
            objPTSProblem.JournalCode = journalCode;

            if (select1_ArticleDropDown.Items.Count > 0)
            {
                article = Chapter_selection1();

                //Commented by Sathish.J on 08-May-2018
                //stage = ptsQueryService.GetStage(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim(), out volume, out issue);

                //DataTable dtProblems = new DataTable();
                //FillStageDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim());
                //StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByText(stage.Trim()));
                //int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);

                //Comment end

                //VolumeDropDown.Items.Clear();
                //VolumeDropDown.Items.Add(new ListItem(volume, volume));
                //VolumeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                //IssueDropDown.Items.Clear();
                //IssueDropDown.Items.Add(new ListItem(issue, issue));
                //IssueDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                // FillRoundCountDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim());

                //if (VolumeDropDown.SelectedIndex > 0)
                //    volume = VolumeDropDown.SelectedItem.Text.Trim();
                //else
                volume = string.Empty;
                //if (IssueDropDown.SelectedIndex > 0)
                //    issue = IssueDropDown.SelectedItem.Text.Trim();
                //else
                issue = string.Empty;


                objPTSProblem.Article = article.Trim();
                objPTSProblem.Volume = volume.Trim();
                objPTSProblem.Issue = issue.Trim();
                if (StageDropDown.SelectedValue != "0")
                {
                    objPTSProblem.StageID = Convert.ToInt16(StageDropDown.SelectedValue);
                }
                else if (StageDropDown1.SelectedValue != "0")
                {
                    objPTSProblem.StageID = Convert.ToInt16(StageDropDown1.SelectedValue);
                }
                BindOpenedRequests(objPTSProblem); // now
                gvwOpenedRequest.Visible = true;

                //btnRaiseProblem.Visible = true;
            }
            //else if (VolumeDropDown.SelectedValue != "0" && IssueDropDown.SelectedValue != "0" && VolumeDropDown.SelectedValue != "" && IssueDropDown.SelectedValue != "")
            //{
            //    JobTypeHidden.Value = "2";
            //    Session["jobType"] = JobTypeHidden.Value.Trim();
            //    article = string.Empty;
            //    volume = VolumeDropDown.SelectedItem.Text;
            //    issue = IssueDropDown.SelectedItem.Text;

            //    FillVolIssRoundCountDDL(customerCode, journalCode, volume, issue);
            //    FillStageDDLVolIss(customerCode, journalCode, volume, issue);
            //    stage = ptsQueryService.GetStageByVolumnAndIssue(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), volume, issue);
            //    StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByText(stage.Trim()));
            //    int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
            //    objPTSProblem.Article = article;
            //    objPTSProblem.Volume = volume;
            //    objPTSProblem.Issue = issue;
            //    objPTSProblem.StageID = stageID;
            //    BindOpenedRequests(objPTSProblem);

            //    //btnRaiseProblem.Visible = true;
            //}
            else
            {
                //clearControl("gvw", (Control)GVProblemHistory);
                btnRaiseProblem.Visible = false;
                btnNew_Instruction.Visible = false;
                //CustomerDropDown.SelectedIndex = -1;
                //JournalDropDown.SelectedIndex = -1;
                //ArticleDropDown.SelectedIndex = -1;
                //ddlRoundCount.SelectedIndex = -1;
                //clearControl("ddl", JournalDropDown);
                //clearControl("ddl", ArticleDropDown);
                //clearControl("ddl", ddlRoundCount);
            }

            //#region clearControls
            //ProblemTypeDropDown.SelectedIndex = -1;
            //ddlDepartment.SelectedIndex = 0;
            //PTDescriptionTextBox.Text = string.Empty;
            ////            EnableDisableDDLOpenedRequest();
            //MessageLabel.Text = string.Empty;
            //ResultLabel.Text = string.Empty;
            //EnableDisablePrvRemark();
            //lblZipFile.Text = string.Empty;
            //#endregion clearControls

        }

        private void BindOpenedRequests(PTSProblem objPTSProblem)
        {
            try
            {
                DataTable dtOpenedRequest = new DataTable();
                dtOpenedRequest = ptsQueryService.GetOpenedRequests(objPTSProblem);
                Session["OpenedRequest"] = dtOpenedRequest;
                //// ========================
                DataView dv = new DataView(dtOpenedRequest);
                //DataView dv = dtOpenedRequest.DefaultView;
                //dv.RowFilter = "PENDINGQRY = 1"; //Commnented by sathish.J
                if (dv.ToTable().Rows.Count > 0)
                {
                    Session["AllowNewQry"] = "true";

                    btnRaiseProblem.Enabled = true;
                    btnNew_Instruction.Visible = true;
                    btnRaiseProblem.Visible = true;
                    btnRaiseProblem.ToolTip = string.Empty;
                }
                else
                {
                    Session["AllowNewQry"] = "false";
                    btnNew_Instruction.Visible = true;
                    btnRaiseProblem.Visible = true;
                    //btnRaiseProblem.Visible = true;
                    //  GetCloseStatusNew(CustomerDropDown.SelectedValue, 1); //commented by sathish.J
                }
                // ========================
                //if (dtOpenedRequest != null && dtOpenedRequest.Rows.Count > 0)
                //{
                    gvwOpenedRequest.DataSource = dtOpenedRequest;
                    gvwOpenedRequest.DataBind();
               // }
            }
            catch { throw; }
        }

        protected int roleID;

        private void GetCloseStatusNew(string strCust, int CloseStatus)
        {
            roleID = ptsQueryService.GetRoleIDByUserName(userName, strCust);
            if (CloseStatus == 1)
            {
                btnRaiseProblem.Visible = true;
                btnRaiseProblem.Enabled = false;
                btnRaiseProblem.ToolTip = "Please resolve the raised query.";

            }
            else if (CloseStatus == 0)
            {
                btnRaiseProblem.Visible = true;
                btnRaiseProblem.Enabled = false;
                btnRaiseProblem.ToolTip = "Please resolve the raised query.";
            }
            else
            {
                bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQuerySavebutton);
                if (allowSave)
                {
                    btnRaiseProblem.Visible = true;
                    btnRaiseProblem.Enabled = true;
                    btnRaiseProblem.ToolTip = string.Empty;
                }
                else
                {
                    btnRaiseProblem.Visible = true;
                    btnRaiseProblem.Enabled = false;
                    btnRaiseProblem.ToolTip = "Please resolve the raised query.";
                }
            }
        }

        private void clearControl(string contType, Control contName)
        {
            try
            {
                if (contType.ToLower().Trim() == "ddl")
                {
                    DropDownList ddl = (DropDownList)contName;

                    ddl.Items.Clear();
                    ddl.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                }
                else if (contType.ToLower().Trim() == "gvw")
                {
                    //DataTable dt = new DataTable();
                    GridView gvw = (GridView)contName;
                    gvw.DataSource = null;
                    gvw.DataBind();
                }
                else if (contType.ToLower().Trim() == "text")
                {
                    TextBox txt = (TextBox)contName;
                    txt.Text = string.Empty;
                }
                else if (contType.ToLower().Trim() == "lbl")
                {
                    Label lbl = (Label)contName;
                    lbl.Text = string.Empty;
                }
            }
            catch { throw; }
        }

        protected void btnGoBarcode_Click(object sender, EventArgs e)
        {
            btnGoDdl.Visible = false;
            if (txtBarcode.Text.Trim() != string.Empty)
            {
                Session["BarcodeVal"] = txtBarcode.Text;

                GetDataFromBarCode();

                AssignSessionValues();
                //Session["lstPTypeCurrent"] = lstPTypeCurrent;
            }
            else
            {
                //ShowMessage("Please enter barcode");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please enter barcode', 'Required', 'top', 'right');", true);
                #region clearControls
                //clearControl("txt", (Control)txtBarcode);
                CustomerDropDown.SelectedIndex = -1;
                clearControl("ddl", (Control)JournalDropDown);
                clearControl("ddl", (Control)ArticleDropDown);
                // clearControl("ddl", (Control)VolumeDropDown);
                // clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                //clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
                btnNew_Instruction.Visible = true;
                btnNew_Instruction.Enabled = true;
                btnGoDdl.Visible = false;
                MessageLabel.Text = string.Empty;
                #endregion clearControls
            }
        }

        protected void CustomerDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                clearControl("text", (Control)txtBarcode);
                if (CustomerDropDown.SelectedIndex > 0)
                {
                    DataTable dtOpenedRequest = new DataTable();
                    Session["OpenedRequest"] = dtOpenedRequest;
                    gvwOpenedRequest.DataSource = dtOpenedRequest;
                    gvwOpenedRequest.DataBind();
                    xmlValue = string.Empty;
                    MessageLabel.Text = string.Empty;
                    customerCode = 0;
                    try
                    {
                        #region clearControls
                        clearControl("txt", (Control)txtBarcode);
                        //CustomerDropDown.SelectedIndex = -1;
                        clearControl("ddl", (Control)JournalDropDown);
                        clearControl("ddl", (Control)ArticleDropDown);
                        //clearControl("ddl", (Control)VolumeDropDown);
                        //clearControl("ddl", (Control)IssueDropDown);
                        // clearControl("ddl", (Control)StageDropDown);
                        clearControl("ddl", (Control)ddlRoundCount);
                        clearControl("gvw", (Control)gvwOpenedRequest);
                        clearControl("ddl", (Control)DeptDropDown);
                        btnRaiseProblem.Visible = false;
                        btnNew_Instruction.Visible = false;

                        btnGoDdl.Visible = false;
                        MessageLabel.Text = string.Empty;
                        #endregion clearControls

                        StageDropDown.Visible = false;
                        StageDropDown1.Visible = false;
                        
                        
                        int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                        
                        roleID = ptsQueryService.GetRoleIDByUserName(userName, customerCode.ToString());

                        #region Enable/Disable buttons based on role

                        ////bool allowSave = ptsQueryService.IsSaveAllowed(int.Parse(Global.RoleID));
                        //bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQuerySavebutton);
                        //if (allowSave)
                        //    SaveButton.Enabled = true;
                        //else
                        //    SaveButton.Enabled = false;

                        ////bool allowCloseSignal=ptsQueryService.IsCloseSignalAllowed(int.Parse(Global.RoleID));
                        //bool allowCloseSignal = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryCloseButton);

                        //if (allowCloseSignal)
                        //    CloseButton.Enabled = true;
                        //else
                        //    CloseButton.Enabled = false;

                        //bool allowResolveQuery = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryResolveButton);

                        //if (allowResolveQuery)
                        //    QueryResolveButton.Enabled = true;
                        //else
                        //    QueryResolveButton.Enabled = false;

                        #endregion
                        StageDropDown.Visible = false;
                        StageDropDown1.Visible = false;
                        if (customerCode == 163)
                        {
                            StageDropDown.Visible = true;
                            FillJournalDDL(customerCode);
                        }
                        else if (customerCode == 877)
                        {
                            StageDropDown1.Visible = true;
                            FillJournalDDL(customerCode);
                        }
                        //FillJournalDDL(customerCode);
                        //EnableDisablePrvRemark();
                        btnRaiseProblem.Visible = false;
                        btnNew_Instruction.Visible = false;
                        btnGoDdl.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        WriteLog("exception : " + ex.ToString());
                    }
                }
                else
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)JournalDropDown);
                    clearControl("ddl", (Control)ArticleDropDown);
                    //clearControl("ddl", (Control)VolumeDropDown);
                    //clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    clearControl("ddl", (Control)DeptDropDown);
                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }

        protected void JournalDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearControl("text", (Control)txtBarcode);
            if (JournalDropDown.SelectedIndex > 0)
            {
                DataTable dtOpenedRequest = new DataTable();
                Session["OpenedRequest"] = dtOpenedRequest;

                xmlValue = string.Empty;

                customerCode = 0;
                journalCode = 0;
                int SubStageID = 0;
                bool isMasterCopyJournal = false;

                //pks 
                //EnableArtVolumeIssuePanel(); // new
                try
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    clearControl("ddl", (Control)ArticleDropDown);
                    //clearControl("ddl", (Control)VolumeDropDown);
                    //clearControl("ddl", (Control)IssueDropDown);
                    // clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls

                    int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                    int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);
                    //if (StageDropDown.SelectedIndex > 0)
                    //    SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());
                    jobType = JobTypeHidden.Value;

                    isMasterCopyJournal = ptsQueryService.IsMasterCopyJournal(journalCode);

                    FillArticleDDL(customerCode, journalCode, "");
					SqlConnection conn = null;
                    string constr1 = ConfigurationManager.ConnectionStrings["jetsConstrNew"].ConnectionString;
                    conn = new SqlConnection(constr1);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("usp_Contrast_GetIsbn", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@bookid", SqlDbType.Int).Value = journalCode;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        TextBox3.Text = dt.Rows[0]["Jour_issn_no"].ToString();
                        Session["Isbn"]=dt.Rows[0]["Jour_issn_no"].ToString();
                    }
                    else {
                        TextBox3.Text = "";
                    }
                    FillDeptDDL();
                    //FillVolumeDDL(customerCode, journalCode, SubStageID.ToString());
                    //FillVolumeDDL(customerCode, journalCode, "0");
                    //FillIssueDDL(customerCode, journalCode, "0");
                    MasterCopyJournalLabel.Visible = isMasterCopyJournal;

                    //ProblemTypeDropDown.SelectedIndex = -1;
                    //PTDescriptionTextBox.Text = string.Empty;
                    //GVProblemHistory.DataSource = null;
                    //GVProblemHistory.DataBind();
                    //EnableDisablePrvRemark();

                    //Commented by Sathish.J on 08-May-2018
                    //DataTable dt = new DataTable();
                    //StageDropDown.DataSource = dt;
                    //StageDropDown.DataBind();
                    //StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                    //ProblemTypeDropDown.DataSource = dt;
                    //ProblemTypeDropDown.DataBind();
                    //ProblemTypeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = true;
                }
                catch (Exception ex)
                {
                    WriteLog("exception : " + ex.ToString());
                }
            }
            else
            {
                #region clearControls
                clearControl("txt", (Control)txtBarcode);
                //CustomerDropDown.SelectedIndex = -1;
                //clearControl("ddl", (Control)JournalDropDown);
                clearControl("ddl", (Control)ArticleDropDown);
                //clearControl("ddl", (Control)VolumeDropDown);
                //clearControl("ddl", (Control)IssueDropDown);
                //clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
                btnNew_Instruction.Visible = false;
                btnGoDdl.Visible = false;
                MessageLabel.Text = string.Empty;
                #endregion clearControls
            }
        }

        protected void ArticleDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (ArticleDropDown.SelectedIndex > 0)
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    //clearControl("ddl", (Control)ArticleDropDown);
                    //VolumeDropDown.SelectedIndex = -1;
                    //IssueDropDown.SelectedIndex = -1;
                    //  clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls


                    DataTable dtOpenedRequest = new DataTable();
                    ViewState["dtOpenedRequest"] = dtOpenedRequest;
                    //gvwOpenedRequest.DataSource = dtOpenedRequest;
                    //gvwOpenedRequest.DataBind();
                    ViewState["xml"] = string.Empty;
                    hdnXml.Value = "FirstTime";
                    xmlValue = string.Empty;

                    GetDetails();
                    //DisablePTCategory();

                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = true;
                }
                else
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    //clearControl("ddl", (Control)ArticleDropDown);
                    //VolumeDropDown.SelectedIndex = -1;
                    //IssueDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        private string Chapter_selection1()
        {
            string Chapter_selection = "";
            int count = select1_ArticleDropDown.Items.Count;
            for (int i = 0; i <= count - 1; i++)
            {

                if (select1_ArticleDropDown.Items[i].Selected == true)
                {
                    Chapter_selection += select1_ArticleDropDown.Items[i].ToString().Trim() + ",";
                }
            }
            Chapter_selection = Chapter_selection.TrimEnd(',');

            return Chapter_selection;
        }

        /// <summary>
        /// Unified article selection logic that handles both stage-based and sub-stage-based customers.
        /// 
        /// Stage-based customers (e.g., 163): Use direct ArticleDropDown.SelectedItem.Text
        /// Sub-stage customers (e.g., 877): Use multi-select Chapter_selection1() from select1_ArticleDropDown
        /// 
        /// This method ensures consistent article capture across different customer workflows.
        /// </summary>
        /// <param name="customerCode">The customer code to determine which selection method to use</param>
        /// <returns>Selected article(s) as a string</returns>
        private string GetArticleSelection(int customerCode)
        {
            // Stage-based customer (163): Use direct dropdown text
            if (customerCode == 163)
            {
                if (ArticleDropDown.SelectedIndex > 0)
                    return ArticleDropDown.SelectedItem.Text;
                else
                    return string.Empty;
            }
            // Sub-stage customer (877): Use multi-select Chapter_selection1()
            else if (customerCode == 877)
            {
                return Chapter_selection1();
            }
            // Default: Try multi-select first, fallback to single select
            else
            {
                string multiSelect = Chapter_selection1();
                if (!string.IsNullOrEmpty(multiSelect))
                    return multiSelect;
                
                if (ArticleDropDown.SelectedIndex > 0)
                    return ArticleDropDown.SelectedItem.Text;
                
                return string.Empty;
            }
        }

        protected void StageDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            QueryBind();
        }

        protected void btnRaiseProblem_Click(object sender, EventArgs e)
        {
            try
            {
                //int idx = e.NewEditIndex;
                //Session["OpenedRequestId"] = "1"; //gvwOpenedRequest.DataKeys[idx].Value.ToString().Trim();
                //      GetDetails();

                JobTypeHidden.Value = "1";
                Session["jobType"] = JobTypeHidden.Value.Trim();
                string user = Global.UserName;

                Session["Customer"] = CustomerDropDown.SelectedValue;
                Session["Journal"] = JournalDropDown.SelectedValue;
                Session["JournalName"] = JournalDropDown.SelectedItem.Text.Trim();
                Session["CustomerName"] = CustomerDropDown.SelectedItem.Text.Trim();
                
                // Use unified article selection logic based on customer code
                int customerCode = Convert.ToInt32(CustomerDropDown.SelectedValue);
                Session["Article"] = GetArticleSelection(customerCode);

                //if (VolumeDropDown.SelectedIndex > 0)
                //    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                //else
                Session["Volume"] = string.Empty;

                //if (IssueDropDown.SelectedIndex > 0)
                //    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                //else
                Session["Issue"] = string.Empty;


                //Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                //Session["Volume"] = VolumeDropDown.SelectedValue;
                //Session["Issue"] = IssueDropDown.SelectedValue;

                if (StageDropDown.SelectedValue == "0" && StageDropDown1.SelectedValue == "0")
                {
                    //ShowMessage("Please select stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select stage', 'Required', 'top', 'right');", true);
                    return;

                }
                else
                    if (StageDropDown.SelectedValue != "0")
                    {

                        Session["Stage"] = StageDropDown.SelectedValue;
                    }
                    else if (StageDropDown1.SelectedValue != "0")
                    {

                        Session["Stage"] = StageDropDown1.SelectedValue;
                    }
                if (DeptDropDown.SelectedValue == "0")
                {
                    //ShowMessage("Please select stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select department', 'Required', 'top', 'right');", true);
                    return;
                }
                else
                    Session["Dept"] = DeptDropDown.SelectedItem.Text.Trim();

                string selectedChapters = Session["Article"].ToString();
                if (selectedChapters == "")
                {
                    //ShowMessage("Please select stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select atleast one job', 'Required', 'top', 'right');", true);
                    return;
                }

                Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();
                Session["IsOpenedQuery"] = 0;
                Session["RoundCount"] = ddlRoundCount.SelectedValue;
                Session["rowId"] = -1;
                Session["CreatedBy"] = string.Empty;
                Session["CreatedDate"] = string.Empty;
                Session["SentBy"] = string.Empty;
                Session["SentDate"] = string.Empty;
                Session["ResolvedBy"] = string.Empty;
                Session["ResolvedDate"] = string.Empty;
                Session["queryType"] = "QUERY";//Added by Sathish.J on 10-05-2018

                if (Session["dtProblemTypeDetails"] == null)
                {
                    DataTable dtProblemTypeDetails = new DataTable();
                    //dtProblemTypeDetails = ptsQueryService.GetProblemTypeDetails();
                    if (dtProblemTypeDetails != null)
                        if (dtProblemTypeDetails.Rows.Count > 0)
                            Session["dtProblemTypeDetails"] = dtProblemTypeDetails;
                }
                //Response.Redirect("RaiseProblemsSubmission.aspx");
                if (StageDropDown.SelectedValue != "0")
                {
                    Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + Global.UserName + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown.SelectedItem.Text.Trim() + "&DeptName=" + DeptDropDown.SelectedItem.Text.ToString() + "&DeptValue=" + DeptDropDown.SelectedValue.ToString(), false);
                }
                else if (StageDropDown1.SelectedValue != "0")
                {
                    Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + Global.UserName + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown1.SelectedItem.Text.Trim() + "&DeptName=" + DeptDropDown.SelectedItem.Text.ToString() + "&DeptValue=" + DeptDropDown.SelectedValue.ToString(), false);
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        protected void btnNew_Instruction_Click(object sender, EventArgs e)
        {
            try
            {
                //int idx = e.NewEditIndex;
                //Session["OpenedRequestId"] = "1"; //gvwOpenedRequest.DataKeys[idx].Value.ToString().Trim();
				//ShowMessage(Session["userName"].Tostring());
                JobTypeHidden.Value = "1";
                Session["jobType"] = JobTypeHidden.Value.Trim();
                string user = Global.UserName;

                Session["Customer"] = CustomerDropDown.SelectedValue;
                Session["Journal"] = JournalDropDown.SelectedValue;
                Session["JournalName"] = JournalDropDown.SelectedItem.Text.Trim();
                Session["CustomerName"] = CustomerDropDown.SelectedItem.Text.Trim();
                //Commented By Sathish.J on 11-June-2018
                //if (ArticleDropDown.SelectedIndex > 0)
                //    Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                //else
                Session["Article"] = Chapter_selection1();

                //if (VolumeDropDown.SelectedIndex > 0)
                //    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                //else
                Session["Volume"] = string.Empty;

                //if (IssueDropDown.SelectedIndex > 0)
                //    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                //else
                Session["Issue"] = string.Empty;


                //Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                //Session["Volume"] = VolumeDropDown.SelectedValue;
                //Session["Issue"] = IssueDropDown.SelectedValue;

                if (StageDropDown.SelectedValue == "0" && StageDropDown1.SelectedValue == "0")
                {
                    //ShowMessage("Please select stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select stage', 'Required', 'top', 'right');", true);
                    return;
                }
                else
                    if (StageDropDown.SelectedValue != "0")
                    {

                        Session["Stage"] = StageDropDown.SelectedValue;
                    }
                    else if (StageDropDown1.SelectedValue != "0")
                    {

                        Session["Stage"] = StageDropDown1.SelectedValue;
                    }
                if (DeptDropDown.SelectedValue == "0")
                {
                    //ShowMessage("Please select stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select department', 'Required', 'top', 'right');", true);
                    return;
                }
                else
                    Session["Dept"] = DeptDropDown.SelectedItem.Text.Trim();

                string selectedChapters = Session["Article"].ToString();
                if (selectedChapters == "")
                {
                    //ShowMessage("Please select stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select atleast one job', 'Required', 'top', 'right');", true);
                    return;
                }

                Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();
                Session["IsOpenedQuery"] = 0;
                Session["RoundCount"] = ddlRoundCount.SelectedValue;
                Session["rowId"] = -1;
                Session["CreatedBy"] = string.Empty;
                Session["CreatedDate"] = string.Empty;
                Session["SentBy"] = string.Empty;
                Session["SentDate"] = string.Empty;
                Session["ResolvedBy"] = string.Empty;
                Session["ResolvedDate"] = string.Empty;
                Session["queryType"] = "INSTRUCTION";//Added by Sathish.J on 10-05-2018

                if (Session["dtProblemTypeDetails"] == null)
                {
                    DataTable dtProblemTypeDetails = new DataTable();
                    //dtProblemTypeDetails = ptsQueryService.GetProblemTypeDetails();
                    if (dtProblemTypeDetails != null)
                        if (dtProblemTypeDetails.Rows.Count > 0)
                            Session["dtProblemTypeDetails"] = dtProblemTypeDetails;
                }
                //Response.Redirect("RaiseProblemsSubmission.aspx");
                if (StageDropDown.SelectedValue != "0")
                {
                    Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + Global.UserName + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown.SelectedItem.Text.Trim() + "&DeptName=" + DeptDropDown.SelectedItem.Text.ToString() + "&DeptValue=" + DeptDropDown.SelectedValue.ToString(), false);
                }
                else if (StageDropDown1.SelectedValue != "0")
                {
                    Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + Global.UserName + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown1.SelectedItem.Text.Trim() + "&DeptName=" + DeptDropDown.SelectedItem.Text.ToString() + "&DeptValue=" + DeptDropDown.SelectedValue.ToString(), false);
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        protected void btnGoDdl_Click(object sender, EventArgs e)
        {
            try
            {
                int Temp_ArticleCount = Chapter_selection1().Split(',').Length;
                if ((CustomerDropDown.SelectedIndex > 0 && JournalDropDown.SelectedIndex > 0 && Temp_ArticleCount > 0))
                {
                    Session["BarcodeVal"] = string.Empty;

                    GetDetails();

                    clearControl("text", (Control)txtBarcode);

                    Session["Customer"] = CustomerDropDown.SelectedValue;
                    Session["CustomerName"] = CustomerDropDown.SelectedItem.Text.Trim();
                    Session["Journal"] = JournalDropDown.SelectedValue;
                    Session["JournalName"] = JournalDropDown.SelectedItem.Text.Trim();
                    //Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                    //Session["Volume"] = VolumeDropDown.SelectedValue;
                    //Session["Issue"] = IssueDropDown.SelectedValue;
                    Session["Dept"] = DeptDropDown.SelectedItem.Text.Trim();
                    //if (ArticleDropDown.SelectedIndex > 0)
                    //    Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                    //else
                    Session["Article"] = Chapter_selection1();

                    //if (VolumeDropDown.SelectedIndex > 0)
                    //    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                    //else
                    Session["Volume"] = string.Empty;

                    //if (IssueDropDown.SelectedIndex > 0)
                    //    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                    //else
                    Session["Issue"] = string.Empty;

                    Session["Stage"] = StageDropDown.SelectedValue;
                    Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();

                    Session["RoundCount"] = ddlRoundCount.SelectedValue;
                    Session["lstPType"] = lstPType;
                    if (CustomerDropDown.SelectedIndex > 0)
                    {
                        if (JournalDropDown.SelectedIndex > 0)
                        {
                            if (Temp_ArticleCount > 0)
                            {
                                if (DeptDropDown.SelectedIndex > 0)
                                {
                                    btnRaiseProblem.Visible = true;
                                    btnNew_Instruction.Visible = true;
                                }
                                else
                                {
                                    //ShowMessage("Please select Department");
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select Department', 'Required', 'top', 'right');", true);
                                }
                            }
                            else
                            {
                                //ShowMessage("Please select Article");
                                Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select Article', 'Required', 'top', 'right');", true);
                            }
                        }
                        else
                        {
                            //ShowMessage("Please select Journal");
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select Journal', 'Required', 'top', 'right');", true);
                        }
                    }
                    else
                    {
                        //ShowMessage("Please select Customer");
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select Customer', 'Required', 'top', 'right');", true);
                    }
                    //Session["lstPTypeCurrent"] = lstPTypeCurrent;
                }
                else
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    //ArticleDropDown.SelectedIndex = -1;
                    //VolumeDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnNew_Instruction.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        //ADded by Sathish.J on 21-06-2018
        int GetColumnIndexByName(GridViewRow row, string columnName)
        {
            int columnIndex = 0;
            Grid_TempChek = 0;
            foreach (DataControlFieldCell cell in row.Cells)
            {
                if (cell.ContainingField is TemplateField)
                    if (((TemplateField)cell.ContainingField).HeaderText.Equals(columnName))
                    { Grid_TempChek = 1; break; }

                if (cell.ContainingField is BoundField)
                    if (((BoundField)cell.ContainingField).HeaderText.Equals(columnName))
                    { Grid_TempChek = 1; break; }
                columnIndex++; // keep adding 1 while we don't have the correct name
            }
            return columnIndex;
        }
        public static string initiateFiles(string Param)
        {
            if (itrackCon.State == ConnectionState.Open)
            {
                itrackCon.Close();
            }
            itrackCon.Open();
            SqlCommand cmd = new SqlCommand("USP_PTSUpdatequeryNew", itrackCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Param);
            cmd.Parameters.AddWithValue("@type", "GetInitiateFile");
            string GetRemarks = Convert.ToString(cmd.ExecuteScalar());
            itrackCon.Close();
            return GetRemarks;

        }
        protected void gvwOpenedRequest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                   // GridViewRow row = (sender as Button).NamingContainer as GridViewRow;

                    //string Parameter = gvwOpenedRequest.DataKeys[row.RowIndex]["PTId"].ToString();
                    string Parameter = e.Row.Cells[0].Text.ToString().Trim();
                    string remarks = initiateFiles(Parameter);
                    Button Initiate = (e.Row.FindControl("AppproveBtn") as Button);
                    if (remarks.ToLower() == "success")
                    {
                        Initiate.Visible = false;
                    }
                    //Added on 20-06-2018 

                    System.Drawing.Color colour = System.Drawing.ColorTranslator.FromHtml("#E7EFF2");
                    if (e.Row.Cells[7].Text.ToString().Trim().ToLower() == "raised")
                    {
                        colour = System.Drawing.ColorTranslator.FromHtml("#FFA500");
                        e.Row.BackColor = colour;

                        if (e.Row.RowState.ToString().Trim().ToLower() == "edit")
                        {
                            TextBox TextBoxEdit = (TextBox)e.Row.FindControl("TextBox2");
                            TextBoxEdit.Enabled = false;
                        }
                    }
                    else if (e.Row.Cells[7].Text.ToString().Trim().ToLower() == "resolved")
                    {
                        colour = System.Drawing.ColorTranslator.FromHtml("#90EE90");

                        //LinkButton LinkButton1Edit = (LinkButton)e.Row.FindControl("LinkButton1");
                        //LinkButton1Edit.Enabled = false;

                        e.Row.BackColor = colour;
                        //e.Row.Cells[8].Controls[0].Visible = false;
                    }
                    else if (e.Row.Cells[7].Text.ToString().Trim().ToLower() == "saved")
                    {
                        ImageButton imgbtnEdit = (ImageButton)e.Row.FindControl("imgbtnEdit");
                        //Label lblCloseStatus = (Label)e.Row.FindControl("lblCloseStatus");
                        string AllowNewQry = Session["AllowNewQry"].ToString().Trim();
                        if (AllowNewQry == "false")
                        {
                            imgbtnEdit.Enabled = false;
                            //lblCloseStatus.Text = "1";
                            imgbtnEdit.ToolTip = "Please resolve the raised query.";
                        }
                        else
                        {
                            imgbtnEdit.Enabled = true;
                            //lblCloseStatus.Text = "0";
                        }
                    }
                    // ============================================
                    //e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("<br />", "\r\n");
                    //string temp = e.Row.Cells[6].Text.Trim();
                    //temp = temp.Replace("\r\n", "<br />"); // e.Row.Cells[5].Text.Replace("\r\n", "<br />");
                    //temp = temp.Replace("&amp;", "&");
                    //temp = temp.Replace("&amp;", "&");

                    //temp = temp.Replace("&lt;", "<");
                    //temp = temp.Replace("&gt;", ">");
                    //temp = temp.Replace("&amp;", "&");
                    //e.Row.Cells[6].Text = temp;

                    string temp1 = string.Empty;
                    temp1 = e.Row.Cells[8].Text.Trim();
                    temp1 = temp1.Replace("\r\n", "<br />"); // e.Row.Cells[5].Text.Replace("\r\n", "<br />");
                    temp1 = temp1.Replace("&amp;", "&");
                    temp1 = temp1.Replace("&amp;", "&");

                    temp1 = temp1.Replace("&lt;", "<");
                    temp1 = temp1.Replace("&gt;", ">");
                    temp1 = temp1.Replace("&amp;", "&");
                    e.Row.Cells[8].Text = temp1;
                    // ============================================
                    if (e.Row.Cells[7].Text.ToLower() != "resolved")
                    {
                        string strPType = gvwOpenedRequest.DataKeys[e.Row.RowIndex]["DeptAndPT"].ToString().Trim(); //e.Row.Cells[14].Text;
                        string[] arrStrPType = strPType.Split(',');
                        var result = string.Join(",", strPType.Split(',').Select(s => s.Trim()).ToArray());
                        arrStrPType = result.Split(',');
                        lstPType.AddRange(arrStrPType.AsEnumerable());

                        //string strPType = e.Row.Cells[2].Text;
                        //string[] arrStrPType = strPType.Split(',');
                        //var result = string.Join(",", strPType.Split(',').Select(s => s.Trim()).ToArray());
                        //arrStrPType = result.Split(',');
                        //lstPType.AddRange(arrStrPType.AsEnumerable());
                    }
                    //var result = Regex.Replace(strPType, @"(?<=^|,) +| +(?=,|$)", "");


                    ////var list = arrStrPType.Select(s => s.Trim()).ToList();
                    //foreach (string item in result)
                    //{
                    //    lstPType.Add(item);
                    //}
                    ////lstPType.AddRange(result);


                    Session["lstPType"] = lstPType;
                }
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }
        protected void ImageEdit(object sender, EventArgs e)
        {
            GridViewRow row = (sender as ImageButton).NamingContainer as GridViewRow;
            if (gvwOpenedRequest.DataKeys[row.RowIndex]["QueryType"] != null)
                Session["queryType"] = gvwOpenedRequest.DataKeys[row.RowIndex]["QueryType"].ToString().Trim();//Added by Sathih.J on 11-05-2018
            int get_dept = Convert.ToInt32(gvwOpenedRequest.DataKeys[row.RowIndex]["DeptAndPT"].ToString().Trim().Split('|')[0]);
            DeptDropDown.SelectedValue = get_dept.ToString();

            if (Session["queryType"].ToString() == "QUERY")
            {
                Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + userName + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown.SelectedItem.Text.Trim() + "&DeptName=" + DeptDropDown.SelectedItem.Text.Trim() + "&DeptValue=" + DeptDropDown.SelectedValue.ToString(), false);

            }

        }
        protected void ApproveBnClick(object sender, EventArgs e)
        {
            bool Chkisupdate = false;

            GridViewRow row = (sender as Button).NamingContainer as GridViewRow;
            string QueryType = gvwOpenedRequest.DataKeys[row.RowIndex]["QueryType"].ToString();
            string problemId = gvwOpenedRequest.DataKeys[row.RowIndex]["PTId"].ToString();// (row.Cells[PTId_Remarks].Controls[0] as Label).Text;

            if (QueryType.Trim().ToUpper() == "INSTRUCTION")
            {
                if (itrackCon.State == ConnectionState.Open)
                {
                    itrackCon.Close();
                }
                itrackCon.Open();
                SqlCommand cmd = new SqlCommand("USP_PTSUpdatequeryNew", itrackCon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", "UpdateApprove");
                cmd.Parameters.AddWithValue("@ID", problemId);
                cmd.Parameters.AddWithValue("@Proceed_Req", 1);
                 cmd.ExecuteNonQuery();
                //int temp = 0;
                //string[] sqlparams = { problemId.ToString(),"YES", "UpdateApprove" };
                //SqlHelper sql = new SqlHelper();
                //temp = sql.GetExecuteFromSP("USP_deletequeryNew_Temp", sqlparams, 2);
                Chkisupdate = true;
            }
            else
            {
                Chkisupdate = false;
            }
            int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
            int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

            objPTSProblem.CustomerCode = CustomerID;
            objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

            objPTSProblem.Article = Chapter_selection1();

            objPTSProblem.Volume = "";
            //else
            objPTSProblem.Issue = "";
            objPTSProblem.StageID = SubStageID;
            gvwOpenedRequest.EditIndex = -1;
            BindOpenedRequests(objPTSProblem);
            if (Chkisupdate == true)
            {
                Response.Write("<script language=javascript>alert('Updated');</script>");
                //return;
            }
            else
            {
                Response.Write("<script language=javascript>alert('Not Updated');</script>");
            }

        }
        protected void OnUpdate(object sender, EventArgs e)
        {
            GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

            int index_Remarks = GetColumnIndexByName(row, "Remarks");
            int PTId_Remarks = GetColumnIndexByName(row, "TextBox1");
            //Added by Sathish.J on 31-July-2018
            int PTId_Response = GetColumnIndexByName(row, "TextBox2");

            string problemId = gvwOpenedRequest.DataKeys[row.RowIndex]["PTId"].ToString();// (row.Cells[PTId_Remarks].Controls[0] as Label).Text;
            TextBox gtxtPTRemarks = (TextBox)gvwOpenedRequest.Rows[row.RowIndex].FindControl("TextBox1");

            //Added by Sathish.J on 31-July-2018
            TextBox gtxtPTResponse = (TextBox)gvwOpenedRequest.Rows[row.RowIndex].FindControl("TextBox2");


            int temp = 0;
            string[] sqlparams = { problemId.ToString(), gtxtPTRemarks.Text, gtxtPTResponse.Text, "update" };
            SqlHelper sql = new SqlHelper();
            temp = sql.GetExecuteFromSP("USP_deletequerynew", sqlparams, 2);

            int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
            int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

            objPTSProblem.CustomerCode = CustomerID;
            objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

            objPTSProblem.Article = Chapter_selection1();

            objPTSProblem.Volume = "";
            //else
            objPTSProblem.Issue = "";
            objPTSProblem.StageID = SubStageID;
            gvwOpenedRequest.EditIndex = -1;
            BindOpenedRequests(objPTSProblem);
        }
        //protected void OnApprove(object sender, EventArgs e)
        //{
        //    bool Chkisupdate = false;
        //    GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        //    string problemId = gvwOpenedRequest.DataKeys[row.RowIndex]["PTId"].ToString();// (row.Cells[PTId_Remarks].Controls[0] as Label).Text;

        //    if (Session["queryType"].ToString() == "INSTRUCTION")
        //    { 
        //        int temp = 0;
        //    string[] sqlparams = { problemId.ToString(), "UpdateApprove" };
        //    SqlHelper sql = new SqlHelper();
        //    temp = sql.GetExecuteFromSP("USP_deletequerynew", sqlparams, 2);
        //        Chkisupdate = true;
        //    }
        //    int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
        //    int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

        //    objPTSProblem.CustomerCode = CustomerID;
        //    objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

        //    objPTSProblem.Article = Chapter_selection1();

        //    objPTSProblem.Volume = "";
        //    //else
        //    objPTSProblem.Issue = "";
        //    objPTSProblem.StageID = SubStageID;
        //    gvwOpenedRequest.EditIndex = -1;
        //    BindOpenedRequests(objPTSProblem);
        //    if (Chkisupdate== true)
        //    {
        //        Response.Write("<script language=javascript>alert('Updated');</script>");
        //        return;
        //    }
        //}
        protected void OnCancel(object sender, EventArgs e)
        {
            gvwOpenedRequest.EditIndex = -1;

            int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
            int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

            objPTSProblem.CustomerCode = CustomerID;
            objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

            objPTSProblem.Article = Chapter_selection1();

            objPTSProblem.Volume = "";
            //else
            objPTSProblem.Issue = "";
            objPTSProblem.StageID = SubStageID;
            BindOpenedRequests(objPTSProblem);
        }
        protected void gvwOpenedRequest_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int idx = e.NewEditIndex;
                gvwOpenedRequest.EditIndex = e.NewEditIndex;

                if (gvwOpenedRequest.DataKeys[idx]["QueryType"] != null)
                    Session["queryType"] = gvwOpenedRequest.DataKeys[idx]["QueryType"].ToString().Trim();//Added by Sathih.J on 11-05-2018


                // ========================
                Session["Customer"] = CustomerDropDown.SelectedValue;
                Session["Journal"] = JournalDropDown.SelectedValue;
                Session["Dept"] = DeptDropDown.SelectedItem.Text.Trim();
                Session["CustomerName"] = CustomerDropDown.SelectedItem.Text.Trim();

                Session["JournalName"] = JournalDropDown.SelectedItem.Text.Trim();
                //Added by sathish.J on 13-07-17

                if (Session["queryType"].ToString() == "QUERY")
                {
                    int get_dept = Convert.ToInt32(gvwOpenedRequest.DataKeys[idx]["DeptAndPT"].ToString().Trim().Split('|')[0]);
                    DeptDropDown.SelectedValue = get_dept.ToString();
                }
                //if (ArticleDropDown.SelectedIndex > 0)
                //    Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                //else
                ////Added by Sathish.J on 12-June-2018
                //Button b = sender as Button;
                //string id = b.Attributes["btnRaiseProblem"];

                //Modified by Sathish.J on 12-June-2018

                Session["Article"] = gvwOpenedRequest.DataKeys[idx]["Article"].ToString().Trim();

                if (StageDropDown.SelectedValue == "0")
                {
                    //ShowMessage("Please select the stage");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select the stage', 'Required', 'top', 'right');", true);
                    return;
                }
                if (DeptDropDown.SelectedIndex < 1)
                {
                    FillDeptDDL();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "ShowssMessage('error', 'Please select the Department', 'Required', 'top', 'right');", true);
                    //ShowMessage("Please select the Department");
                    return;
                }
                Session["Stage"] = StageDropDown.SelectedValue;
                Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();
                Session["IsOpenedQuery"] = 0;
                Session["RoundCount"] = ddlRoundCount.SelectedValue;
                // ========================

                //Modified By sathish.J 13-07-17
                Session["OpenedRequestId"] = gvwOpenedRequest.DataKeys[idx]["PTId"].ToString().Trim(); // gvwOpenedRequest.DataKeys[idx].Value.ToString().Trim();

                Session["IsOpenedQuery"] = 1;
                int rowId = e.NewEditIndex;
                Session["rowId"] = rowId;

                Session["CreatedBy"] = gvwOpenedRequest.DataKeys[idx]["CreatedBy"].ToString().Trim();
                Session["CreatedDate"] = gvwOpenedRequest.DataKeys[idx]["CreatedDate"].ToString().Trim();
                Session["SentBy"] = gvwOpenedRequest.DataKeys[idx]["SentBy"].ToString().Trim();
                Session["SentDate"] = gvwOpenedRequest.DataKeys[idx]["SentDate"].ToString().Trim();
                Session["ResolvedBy"] = gvwOpenedRequest.DataKeys[idx]["ResolvedBy"].ToString().Trim();
                Session["ResolvedDate"] = gvwOpenedRequest.DataKeys[idx]["ResolvedDate"].ToString().Trim();

                Session["totalRemarks"] = gvwOpenedRequest.Rows[rowId].Cells[5].Text.Trim();


                if (Session["dtProblemTypeDetails"] == null)
                {
                    DataTable dtProblemTypeDetails = new DataTable();
                    dtProblemTypeDetails = ptsQueryService.GetProblemTypeDetails(userName);
                    if (dtProblemTypeDetails != null)
                        if (dtProblemTypeDetails.Rows.Count > 0)
                            Session["dtProblemTypeDetails"] = dtProblemTypeDetails;
                }

                // ========================================
                if (Session["queryType"].ToString() == "QUERY")
                {
                    DataTable dtProblems = new DataTable();
                    dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(gvwOpenedRequest.DataKeys[idx]["PTCode"].ToString().Trim()));
                    DataRow[] dr = dtProblems.Select();
                    if (dr.Length > 0)
                    {
                        string temp = dr[0]["ZipFilePath"].ToString().Trim();
                        temp = temp.Replace("\\", "\\\\");
                        Session["ZipFilePath"] = temp;
                    }
                }

                int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
                int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());

                objPTSProblem.CustomerCode = CustomerID;
                objPTSProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

                objPTSProblem.Article = Chapter_selection1();

                objPTSProblem.Volume = "";
                //else
                objPTSProblem.Issue = "";
                objPTSProblem.StageID = SubStageID;
                BindOpenedRequests(objPTSProblem);

                //Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + userName + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown.SelectedItem.Text.Trim() + "&DeptName=" + DeptDropDown.SelectedItem.Text.Trim() + "&DeptValue=" + DeptDropDown.SelectedValue.ToString(), false);
            }
            catch (Exception ex)
            {
                WriteLog("exception : " + ex.ToString());
            }
        }

        protected void lkbtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewLogin.aspx");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Response.Redirect("Reports/SummaryReport.aspx");
        }

    }
}
