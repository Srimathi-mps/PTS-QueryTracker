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
using System.IO;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml;
using System.Text.RegularExpressions;
using System.Web.Mail;
using System.Net;

//using System.Net.Mail;

namespace PTS
{
    public partial class RaiseProblem : System.Web.UI.Page
    {
        #region Fields
        IPTSQuery ptsQueryService = new PTSQueryService();
        BizPTSQuery objBizPTSQuery = new BizPTSQuery();
        // clsServerConfiguration serverConfig = SANGlobal.ServerConfig;
        PTSProblem objPTSProblem = new PTSProblem();
        bool raiseQuery = true;
        int customerCode;
        int journalCode;
        string article = string.Empty;
        string stage = string.Empty;
        string volume = string.Empty;
        string issue = string.Empty;
        string SubForLog = string.Empty;
        string MsgBodyForLog = string.Empty;
        int stageID = 0;
        int i;
        protected string jobType = string.Empty;
        string articleType = ConfigurationManager.AppSettings["ArticleType"].ToString().Trim();
        string issueType = ConfigurationManager.AppSettings["IssueType"].ToString().Trim();
        public string SmtpAddr = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPAddr");
        string xmlValue = string.Empty;


        protected string userName = Global.Validate(); //Changed By sathish

        //protected string userName = "MAC000289";
        //protected string userName = "cb1790";

        protected int roleID;
        public string CustCode { get; set; }
        public string JourCode { get; set; }
        public string ArtCode { get; set; }

        #endregion Fields

        #region Page control events

        protected void Page_Load(object sender, EventArgs e)
        {
            //PTDescriptionTextBox.Attributes.Add("readonly", "readonly");

            GoButton.Attributes.Add("onclick", "return ValidateBarCode()");
            SaveButton.Attributes.Add("onclick", "return ValidationForSaveAndClose()");
            CloseButton.Attributes.Add("onclick", "return ValidationForSaveAndClose()");
            ZipButton.Attributes.Add("onclick", "return ZipFiles()");

            try
            {
                if (!Page.IsPostBack)
                {
                    MessageLabel.Text = string.Empty;
                    SaveButton.Enabled = false;
                    ZipButton.Enabled = false;
                    CloseButton.Enabled = false;
                    QueryResolveButton.Enabled = false;
                    roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");
                    if (roleID <= 0)
                    {
                        MessageLabel.Text = "User role not defined";
                        System.Web.HttpContext.Current.Response.Redirect("BlankPage.aspx");
                        return;
                    }
                    ClearFields();
                    FillCustomerDDL();
                    FillDepartmentDDL();
                    HidePanels();
                    UserNameTextBox.Text = userName;
                    hdnUserName.Value = userName;
                    hdnName.Value = userName;
                    lblPreviousRemarks.Visible = false;

                    if (Request.QueryString["barCode"] != null)
                    {
                        if (Request.QueryString["barCode"] != string.Empty)
                        {
                            BarCodeTextBox.Text = Request.QueryString["barCode"].ToString().Trim();
                            GetDataFromBarCode();
                            if (Request.QueryString["OpenedRequest"] != null)
                            {
                                ddlOpenedRequest.SelectedValue = Request.QueryString["OpenedRequest"].ToString().Trim();
                                OpenedRequestSelectIndexChanged();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
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

        private void ShowMessage(string Message)
        {
            //ScriptManager.RegisterStartupScript(Page, this.GetType(), "alert", string.Format("alert('{0}');", Message), true);
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), Guid.NewGuid().ToString(), "alert('" + GeneralHelper.RemoveSpecialChars(Message) + "');", true);
        }

        protected void StageDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            stage = string.Empty;

            try
            {
                #region clearControls
                MessageLabel.Text = string.Empty;
                ResultLabel.Text = string.Empty;
                PTDescriptionTextBox.Text = string.Empty;
                clearControl("gvw", (Control)GVProblemHistory);

                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                //ViewState["xml"] = string.Empty;
                xmlValue = string.Empty;
                #endregion clearControls

                MasterCopyJournalLabel.Visible = false;
                EnableDisablePrvRemark();

                if (StageDropDown.SelectedIndex > 0)
                {
                    int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
                    int SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());
                  
                     objPTSProblem.CustomerCode=CustomerID;
                     objPTSProblem.JournalCode =  Convert.ToInt32(JournalDropDown.SelectedValue.Trim());
                    objPTSProblem.Article =ArticleDropDown.SelectedItem.Text;
                    if(VolumeDropDown.SelectedIndex >= 0)
                     objPTSProblem.Volume =VolumeDropDown.SelectedItem.Text;
                    else
                         objPTSProblem.Volume ="";
                    if (IssueDropDown.SelectedIndex >= 0)
                        objPTSProblem.Issue = IssueDropDown.SelectedItem.Text;
                    else
                        objPTSProblem.Issue = "";

                      objPTSProblem.StageID = SubStageID;
                      bindOpenedRequests(objPTSProblem);
                }
                else
                {
                    #region clearControls
                    MessageLabel.Text = string.Empty;
                    ResultLabel.Text = string.Empty;
                    PTDescriptionTextBox.Text = string.Empty;
                    clearControl("gvw", (Control)GVProblemHistory);
                    #endregion clearControls

                    MasterCopyJournalLabel.Visible = false;
                    EnableDisablePrvRemark();

                    HidePanels();

                    ddlOpenedRequest.SelectedIndex = -1;
                }

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        protected void CustomerDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CustomerDropDown.SelectedIndex > 0)
            {
                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                //ViewState["xml"] = string.Empty;
                xmlValue = string.Empty;


                MessageLabel.Text = string.Empty;
                customerCode = 0;
                try
                {
                    #region clearControls
                    clearControl("ddl", (Control)ArticleDropDown);
                    clearControl("ddl", (Control)VolumeDropDown);
                    clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    ddlDepartment.SelectedIndex = -1;
                    clearControl("ddl", (Control)ProblemTypeDropDown);
                    clearControl("ddl", (Control)ddlOpenedRequest);
//                    EnableDisableDDLOpenedRequest();
                    MessageLabel.Text = string.Empty;
                    ResultLabel.Text = string.Empty;
                    PTDescriptionTextBox.Text = string.Empty;
                    clearControl("gvw", (Control)GVProblemHistory);
                    #endregion clearControls

                    int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                    roleID = ptsQueryService.GetRoleIDByUserName(userName, customerCode.ToString());

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

                    FillJournalDDL(customerCode);
                    EnableDisablePrvRemark();
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
            else
            {
                #region clearControls
                clearControl("ddl", (Control)JournalDropDown);
                clearControl("ddl", (Control)ArticleDropDown);
                clearControl("ddl", (Control)VolumeDropDown);
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                ddlDepartment.SelectedIndex = -1;
                clearControl("ddl", (Control)ProblemTypeDropDown);
                clearControl("ddl", (Control)ddlOpenedRequest);
//                EnableDisableDDLOpenedRequest();
                MessageLabel.Text = string.Empty;
                ResultLabel.Text = string.Empty;
                PTDescriptionTextBox.Text = string.Empty;
                clearControl("gvw", (Control)GVProblemHistory);
                #endregion clearControls
            }
        }

        protected void JournalDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (JournalDropDown.SelectedIndex > 1)
            {
                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                //ViewState["xml"] = string.Empty;
                xmlValue = string.Empty;

                customerCode = 0;
                journalCode = 0;
                int SubStageID = 0;
                bool isMasterCopyJournal = false;

                //pks 
                EnableArtVolumeIssuePanel(); // new
                try
                {
                    #region clearControls
                    clearControl("ddl", (Control)ArticleDropDown);
                    clearControl("ddl", (Control)VolumeDropDown);
                    clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    ddlDepartment.SelectedIndex = -1;
                    clearControl("ddl", (Control)ProblemTypeDropDown);
                    clearControl("ddl", (Control)ddlOpenedRequest);
//                    EnableDisableDDLOpenedRequest();
                    MessageLabel.Text = string.Empty;
                    ResultLabel.Text = string.Empty;
                    PTDescriptionTextBox.Text = string.Empty;
                    clearControl("gvw", (Control)GVProblemHistory);
                    #endregion clearControls

                    int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                    int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);
                    if (StageDropDown.SelectedIndex > 0)
                        SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());
                    jobType = JobTypeHidden.Value;

                    isMasterCopyJournal = ptsQueryService.IsMasterCopyJournal(journalCode);

                    FillArticleDDL(customerCode, journalCode, "0", "0");
                    FillVolumeDDL(customerCode, journalCode, SubStageID.ToString());
                    FillIssueDDL(customerCode, journalCode, "0");
                    MasterCopyJournalLabel.Visible = isMasterCopyJournal;

                    ProblemTypeDropDown.SelectedIndex = -1;
                    PTDescriptionTextBox.Text = string.Empty;
                    GVProblemHistory.DataSource = null;
                    GVProblemHistory.DataBind();
                    EnableDisablePrvRemark();

                    DataTable dt = new DataTable();
                    StageDropDown.DataSource = dt;
                    StageDropDown.DataBind();
                    StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                    ProblemTypeDropDown.DataSource = dt;
                    ProblemTypeDropDown.DataBind();
                    ProblemTypeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
            else
            {
                #region clearControls
                clearControl("ddl", (Control)ArticleDropDown);
                clearControl("ddl", (Control)VolumeDropDown);
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                ddlDepartment.SelectedIndex = -1;
                clearControl("ddl", (Control)ProblemTypeDropDown);
                clearControl("ddl", (Control)ddlOpenedRequest);
//                EnableDisableDDLOpenedRequest();
                MessageLabel.Text = string.Empty;
                ResultLabel.Text = string.Empty;
                PTDescriptionTextBox.Text = string.Empty;
                clearControl("gvw", (Control)GVProblemHistory);
                #endregion clearControls
            }
        }

        protected void ArticleDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                ViewState["xml"] = string.Empty;
                hdnXml.Value = "FirstTime";
                xmlValue = string.Empty;

                GetDetails();
                DisablePTCategory();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private void GetDetails()
        {
            MessageLabel.Text = string.Empty;
            JobTypeHidden.Value = "1";
            volume = string.Empty;
            issue = string.Empty;
            ddlOpenedRequest.Items.Clear();
            string problemTypeCode = string.Empty;
            string problemDescription = string.Empty;
            string deptId = string.Empty;

            customerCode = Convert.ToInt32(CustomerDropDown.SelectedValue.Trim());
            journalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());
            objPTSProblem.CustomerCode = customerCode;
            objPTSProblem.JournalCode = journalCode;


            if (ArticleDropDown.SelectedIndex > 0)
            {
                article = ArticleDropDown.SelectedItem.Text.Trim();
                stage = ptsQueryService.GetStage(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim(), out volume, out issue);
                
                DataTable dtProblems = new DataTable();
                FillStageDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim());
                StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByText(stage.Trim()));
                int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
                VolumeDropDown.Items.Clear();
                VolumeDropDown.Items.Add(new ListItem(volume, volume));
                IssueDropDown.Items.Clear();
                IssueDropDown.Items.Add(new ListItem(issue, issue));
                FillRoundCountDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim());
                BindPreviousRemarks();
                volume = VolumeDropDown.SelectedItem.Text;
                issue = IssueDropDown.SelectedItem.Text;
                objPTSProblem.Article = article;
                objPTSProblem.Volume = volume;
                objPTSProblem.Issue = issue;
                objPTSProblem.StageID = stageID;
                bindOpenedRequests(objPTSProblem);
           
            }
            else if (VolumeDropDown.SelectedValue != "0" && IssueDropDown.SelectedValue != "0")
            {
                JobTypeHidden.Value = "2";
                article = string.Empty;
                volume = VolumeDropDown.SelectedItem.Text;
                issue = IssueDropDown.SelectedItem.Text;

                FillVolIssRoundCountDDL(customerCode, journalCode, volume, issue);
                FillStageDDLVolIss(customerCode, journalCode, volume, issue);
                stage = ptsQueryService.GetStageByVolumnAndIssue(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), volume, issue);
                StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByText(stage.Trim()));
                int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
                objPTSProblem.Article = article;
                objPTSProblem.Volume = volume;
                objPTSProblem.Issue = issue;
                objPTSProblem.StageID = stageID;
                bindOpenedRequests(objPTSProblem);
                BindPreviousRemarks();
            }
            else
            {
                clearControl("gvw", (Control)GVProblemHistory);
            }

            #region clearControls
            ProblemTypeDropDown.SelectedIndex = -1;
            ddlDepartment.SelectedIndex = 0;
            PTDescriptionTextBox.Text = string.Empty;
//            EnableDisableDDLOpenedRequest();
            MessageLabel.Text = string.Empty;
            ResultLabel.Text = string.Empty;
            EnableDisablePrvRemark();
            lblZipFile.Text = string.Empty;
            #endregion clearControls

        }

        protected void ProblemTypeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            //====newly added code by shiva==========
            try
            {
                if (ProblemTypeDropDown.SelectedIndex > 0)
                {
                    //FillProblemTypeCategoryDDL(ProblemTypeDropDown.SelectedValue, Convert.ToInt32(ddlDepartment.SelectedValue), Convert.ToInt32(StageDropDown.SelectedValue));
                    FillProblemTypeCategoryDDL(ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))), Convert.ToInt32(ddlDepartment.SelectedValue), Convert.ToInt32(StageDropDown.SelectedValue));

                    DisablePTCategory();



                    //============================
                    stageID = 0;
                    customerCode = 0;
                    //journalCode = 0;
                    //article = string.Empty;
                    stage = string.Empty;

                    //int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
                    int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                    //int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);
                    //if (ArticleDropDown.SelectedIndex > 0)
                    //    article = ArticleDropDown.SelectedItem.Text.Trim(); // ArticleDropDown.SelectedValue.ToString().Trim();
                    //else
                    //    article = string.Empty;
                    stage = StageDropDown.SelectedItem.Text.Trim();
                    //volume = VolumeDropDown.SelectedValue.ToString().Trim();
                    //issue = IssueDropDown.SelectedValue.ToString().Trim();


                    string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                    string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
                    tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);
                    tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);

                    string problemID = string.Empty;
                    problemID = ptsQueryService.GetPTSProblemID();
                    ////ptsProblem.ProblemID = problemID;

                    tempZipFileName = problemID.ToString().Trim(); // customerGroupName.Trim() + "_" + JournalDropDown.SelectedItem.Text.Trim() + "_" + article.Trim() + "_" + stage.Trim() + "_" + problemID.Trim();
                    tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                    string zipFileName = tempZipFilePath + @"\" + tempZipFileName + ".zip";
                    ////ptsProblem.ZipFilePath = zipFileName;
                    //txtZipFilePath.Text = zipFileName;
                    hfZipFilePath.Value = zipFileName;
                    Session["zipFile"] = zipFileName;
                    //============================
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        protected void VolumeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ArticleDropDown.SelectedIndex = -1;
            MessageLabel.Text = string.Empty;
            ResultLabel.Text = string.Empty;
            volume = string.Empty;
            issue = string.Empty;
            string deptId = string.Empty;
            JobTypeHidden.Value = "2";
            clearControl("ddl", ddlRoundCount);
            try
            {
                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                ViewState["xml"] = string.Empty;
                hdnXml.Value = "FirstTime";
                xmlValue = string.Empty;

                int.TryParse(CustomerDropDown.SelectedValue.ToString(), out customerCode);
                int.TryParse(JournalDropDown.SelectedValue.ToString(), out journalCode);
                volume = VolumeDropDown.SelectedValue.Trim();
                FillIssueDDL(customerCode, journalCode, volume);

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        protected void IssueDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                ViewState["xml"] = string.Empty;
                hdnXml.Value = "FirstTime";
                xmlValue = string.Empty;

                GetDetails();
                DisablePTCategory();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

        }
            
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (ViewState["xml"].ToString() == string.Empty)
            {
                ShowMessage("Please add atleast one problemtype.");
                return;
            }
            MessageLabel.Text = string.Empty;
            PTSProblem ptsProblem = new PTSProblem();
            string validationMessage = string.Empty;

            stageID = 0;
            customerCode = 0;
            journalCode = 0;
            article = string.Empty;
            stage = string.Empty;

            int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
            int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
            int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);
            if(ArticleDropDown.SelectedIndex > 0)
                article = ArticleDropDown.SelectedItem.Text.Trim(); // ArticleDropDown.SelectedValue.ToString().Trim();
            else
                article = string.Empty;
            stage = StageDropDown.SelectedItem.Text.Trim();
            volume = VolumeDropDown.SelectedValue.ToString().Trim();
            issue = IssueDropDown.SelectedValue.ToString().Trim();

            string piiNumber = string.Empty;
            string formattedPIINumber = string.Empty;
            string customerGroupName = string.Empty;
            bool isMasterCopyJournal = false;


            try
            {
                DataTable dtPTCategory = (DataTable)ViewState["dtPTCategory"];
                if (dtPTCategory.Rows.Count > 0)
                {
                    #region try
                    jobType = JobTypeHidden.Value;

                    //get the customer group name
                    if (ViewState["CustomerGrName"] != null)
                        customerGroupName = (string)ViewState["CustomerGrName"];
                    else
                    {
                        DataTable dtCustomerGrName = new DataTable();
                        dtCustomerGrName = ptsQueryService.GetCustomerGroupNameAndJournalName(customerCode, journalCode);

                        if (dtCustomerGrName.Rows.Count > 0)
                        {
                            customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                        }
                    }
                    //end get the customer group name

                    ptsProblem.Article = article;
                    ptsProblem.Volume = volume;
                    ptsProblem.Issue = issue;
                    isMasterCopyJournal = ptsQueryService.IsMasterCopyJournal(journalCode);

                    string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                    string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
                    tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);

                    tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);
                    //tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[username]", UserNameTextBox.Text.Trim());
                    //tempZipFileName = tempZipFileName.Replace(" ", string.Empty);
                    //string zipFileName = tempZipFilePath + @"\" + tempZipFileName;

                    //============================
                    string problemID = string.Empty;
                    problemID = ptsQueryService.GetPTSProblemID();
                    ptsProblem.ProblemID = problemID;

                    tempZipFileName = problemID.ToString().Trim(); // customerGroupName.Trim() + "_" + JournalDropDown.SelectedItem.Text.Trim() + "_" + article.Trim() + "_" + stage.Trim() + "_" + problemID.Trim();
                    tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                    string zipFileName = tempZipFilePath + @"\" + tempZipFileName + ".zip";
                    ptsProblem.ZipFilePath = zipFileName;
                    Session["zipFile"] = zipFileName;
                    //============================

                    ptsProblem.StageID = stageID;
                    ptsProblem.Stage = stage;
                    ptsProblem.CustomerCode = customerCode;
                    ptsProblem.JournalCode = journalCode;

                    //ptsProblem.ProblemTypeCode = ProblemTypeDropDown.SelectedValue.ToString().Trim();
                    //ptsProblem.ProblemTypeCode = ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString().Trim();
                    ptsProblem.ProblemTypeCode = "";
                    ptsProblem.PtsRemarks = PTDescriptionTextBox.Text.Trim();

                    int tempUserId = 0;
                    int.TryParse(Global.userId, out tempUserId);
                    ptsProblem.QuerySavedBy = userName;
                    ptsProblem.QueryClosedBy = string.Empty;


                    ptsProblem.TaskClosed = false;

                    //ptsProblem.ProblemID = string.Empty; //pks

                    if (File.Exists(zipFileName))
                        ptsProblem.ZipFileUpload = true;
                    else
                        ptsProblem.ZipFileUpload = false;

                    ptsProblem.UserName = userName;
                    ptsProblem.UserID = tempUserId;

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

                    //ptsProblem.Sequence = Convert.ToInt32(lblSequence.Text);
                    ptsProblem.DepartmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
                    ptsProblem.Volume = VolumeDropDown.SelectedValue.Trim();
                    ptsProblem.Issue = IssueDropDown.SelectedValue.Trim();
                    //ptsProblem.ProblemID = "0"; //pks
                    //added by shiva
                    ptsProblem.FileContent = ViewState["xml"].ToString();
                    //till here added by shiva
                    if (ddlRoundCount.SelectedValue.Trim() != "")
                        ptsProblem.RoundCount = Convert.ToInt32(ddlRoundCount.SelectedValue.Trim());
                    else
                        ptsProblem.RoundCount = 0;

                    if (ddlDepartment.SelectedIndex > 0)
                    {

                        ShowMessage(ptsQueryService.InsertOrUpdatePTSProblem(ptsProblem));

                        bindOpenedRequests(ptsProblem);

                        if (isMasterCopyJournal)
                            ShowMessage(ConfigurationManager.AppSettings["MasterCopyJournalText"].ToString().Trim());
                        else
                            ResultLabel.Text = string.Empty;
                        ClearFields();
                    }
                    else
                    {
                        ShowMessage("Please select Department");
                    }

                    //GetCloseStatus(ptsProblem.CustomerCode.ToString(), ptsProblem.JournalCode.ToString(), article, volume, issue, ptsProblem.StageID.ToString().Trim(), ptsProblem.ProblemTypeCode.ToString().Trim());
                    //EnableDisableDDLOpenedRequest();

                 
                    #endregion try
                }
                else
                {
                    ShowMessage("Please select at least one problem type");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        protected void GoButton_Click(object sender, EventArgs e)
        {
            GetDataFromBarCode();
        }
        private void GetDataFromBarCode()
        {
            MessageLabel.Text = string.Empty;
            string customerGroupName = string.Empty;
            string journalName = string.Empty;
            string articleID = string.Empty;
            string volume = string.Empty;
            string issue = string.Empty;

            string barcode = BarCodeTextBox.Text.Trim();
            string[] barcodeArr = barcode.Split('|');
            string errorMessage = string.Empty;

            MessageLabel.Text = string.Empty;
            ResultLabel.Text = string.Empty;
            ClearFields();


            try
            {
                DataTable dtPTCategory = new DataTable();
                ViewState["dtPTCategory"] = dtPTCategory;
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();
                //ViewState["xml"] = string.Empty;
                xmlValue = string.Empty;

                DataTable dt = new DataTable();

                //Get Customer journal article or get customer joutnal volume issue
                if (barcodeArr.Length == 3)
                {
                    // customerGroupName is now custFName as per RMT#13386 updated in sp
                    customerGroupName = barcodeArr[0].Trim();
                    journalName = barcodeArr[1].Trim();
                    articleID = barcodeArr[2].Trim();
                    JobTypeHidden.Value = articleType.ToString().Trim();

                    objBizPTSQuery.Article = articleID;

                    errorMessage = ptsQueryService.ValidateCustomerJournalArticle(customerGroupName, journalName, articleID);
                    if (errorMessage.Trim() != string.Empty)
                    {
                        ShowMessage(errorMessage);
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

                            CustomerDropDown.SelectedValue = customerCode.ToString();

                            FillJournalDDL(customerCode);
                            JournalDropDown.SelectedValue = journalCode.ToString();

                            stage = ptsQueryService.GetStage(customerCode, journalCode, articleID, out volume, out issue);

                            if (stage.Trim() == string.Empty)
                            {
                                ShowMessage("The article entered does not exist");
                                ClearFields();
                                return;
                            }
                            else
                            {
                                FillArticleDDL(customerCode, journalCode, "0", "0");
                                ArticleDropDown.SelectedIndex = ArticleDropDown.Items.IndexOf(ArticleDropDown.Items.FindByText(articleID));
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

                    objBizPTSQuery.Volume = volume;
                    objBizPTSQuery.Issue = issue;

                    dt = ptsQueryService.GetCustomerAndJournalCode(customerGroupName, journalName);

                    if (dt.Rows.Count > 0)
                    {
                        customerCode = int.Parse(dt.Rows[0]["cust_code"].ToString());
                        journalCode = int.Parse(dt.Rows[0]["jour_code"].ToString());

                        objBizPTSQuery.CustomerCode = customerCode;
                        objBizPTSQuery.JournalCode = journalCode;

                        CustomerDropDown.SelectedValue = customerCode.ToString();

                        FillJournalDDL(customerCode);
                        JournalDropDown.SelectedValue = journalCode.ToString();
                        FillVolumeDDL(customerCode, journalCode, "0");
                        FillIssueDDL(customerCode, journalCode, "0");
                    }
                    if (VolumeDropDown.Items.IndexOf(VolumeDropDown.Items.FindByText(volume)) < 0)
                    {
                        ShowMessage("The volume entered does not exist");
                        return;
                    }
                    else if (IssueDropDown.Items.IndexOf(IssueDropDown.Items.FindByText(issue)) < 0)
                    {
                        ShowMessage("The issue entered does not exist");
                        return;
                    }
                    VolumeDropDown.SelectedValue = volume;
                    IssueDropDown.SelectedValue = issue;
                    FillArticleDDL(customerCode, journalCode, volume, issue);



                }
                else
                {
                    ShowMessage("Invalid barcode");
                    JobTypeHidden.Value = string.Empty;
                    return;
                }

                GetDetails();
                DisablePTCategory();
            }
            catch (Exception ex)
            {

                ShowMessage(ex.Message);
            }
        }


        protected void CloseButton_Click(object sender, EventArgs e)
        {
            
            MessageLabel.Text = string.Empty;
            PTSProblem ptsProblem = new PTSProblem();
            string validationMessage = string.Empty;

            stageID = 0;
            customerCode = 0;
            journalCode = 0;
            article = string.Empty;
            stage = string.Empty;

            int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
            int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
            int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);

            if (ArticleDropDown.SelectedIndex > 0)
                article = ArticleDropDown.SelectedItem.Text.Trim();// ArticleDropDown.SelectedValue.ToString().Trim();
            else
                article = string.Empty;
            stage = StageDropDown.SelectedItem.Text.Trim();

            volume = VolumeDropDown.SelectedValue.ToString().Trim();
            issue = IssueDropDown.SelectedValue.ToString().Trim();

            string piiNumber = string.Empty;
            string formattedPIINumber = string.Empty;
            string customerGroupName = string.Empty;

            string volumeFirst = string.Empty;
            string volumeLast = string.Empty;
            string issueFirst = string.Empty;
            string issueLast = string.Empty;
            string suppl = string.Empty;

            string ArtvolIssue = "";


            try
            {
                
                #region try

                DataTable dtPTCategory = (DataTable)ViewState["dtPTCategory"];
                if (dtPTCategory.Rows.Count > 0)
                {

                    if (article != string.Empty)
                        ArtvolIssue = article;
                    else
                        ArtvolIssue = volume + "-" + issue + "-" + Convert.ToString(ddlRoundCount.SelectedItem.Text);
                    //check whether the task is already closed 

                    //raiseQuery = DoValidations(out validationMessage);

                    //if (raiseQuery)
                    //{
                    jobType = JobTypeHidden.Value;

                    //get the customer group name
                    if (ViewState["CustomerGrName"] != null)
                        customerGroupName = (string)ViewState["CustomerGrName"];
                    else
                    {
                        DataTable dtCustomerGrName = new DataTable();
                        dtCustomerGrName = ptsQueryService.GetCustomerGroupNameAndJournalName(customerCode, journalCode);

                        if (dtCustomerGrName.Rows.Count > 0)
                        {
                            customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                        }
                    }
                    //end of getting the customer group name

                    //get the uploaded the zip file path and name
                    //string zipFilePath = string.Empty;
                    string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                    string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
                    tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);

                    tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);

                    if (!Directory.Exists(tempZipFilePath))
                        Directory.CreateDirectory(tempZipFilePath);

                    string zipFileName = string.Empty;
                    //end of getting the uploaded the zip file path and name

                    DataTable dtProblems = new DataTable();
                    string ptsProblemUser = string.Empty;

                    dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(ddlOpenedRequest.SelectedValue.Trim()));

                    //if (jobType == articleType)
                    if (ArticleDropDown.SelectedIndex > 0)
                    {
                        //save logic to article
                        //this is because if normal user uploaded any file
                        //dtProblems = ptsQueryService.GetPTSProblems(customerCode, journalCode, article, stageID);

                        if (dtProblems.Rows.Count > 0)
                            ptsProblemUser = dtProblems.Rows[0]["username"].ToString().Trim();
                        else
                            // ptsProblemUser = UserNameTextBox.Text.Trim();
                            ptsProblemUser = Global.UserName;

                        ////tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", article.Trim()).Replace("[problemtypeid]", ProblemTypeDropDown.SelectedValue.Trim()).Replace("[stage]", stage).Replace("[username]", ptsProblemUser);
                        //tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", article.Trim()).Replace("[problemtypeid]", ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).Trim()).Replace("[stage]", stage).Replace("[username]", ptsProblemUser);
                        tempZipFileName = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim();
                        //tempZipFileName = lblZipFile.Text.Trim();
                        tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                        piiNumber = ptsQueryService.GetArticlePIINumber(customerCode, journalCode, article, stage);
                       // formattedPIINumber = GeneralHelper.FormatArticlePIINumber(piiNumber);
                        formattedPIINumber = piiNumber;
                        ptsProblem.Article = article;
                        ptsProblem.Volume = volume;
                        ptsProblem.Issue = issue;
                        ptsProblem.DepartmentId = Convert.ToInt32(ddlDepartment.SelectedValue);
                        ptsProblem.ProblemID = "0";
                        if (ddlRoundCount.SelectedValue.Trim() != "")
                            ptsProblem.RoundCount = Convert.ToInt32(ddlRoundCount.SelectedValue.Trim());
                        else
                            ptsProblem.RoundCount = 0;
                    }


                    ptsProblem.StageID = stageID;
                    ptsProblem.Stage = stage;
                    ptsProblem.CustomerCode = customerCode;
                    ptsProblem.JournalCode = journalCode;

                    //ptsProblem.ProblemTypeCode = ProblemTypeDropDown.SelectedValue.ToString().Trim();
                    //ptsProblem.ProblemTypeCode = ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString().Trim(); //pks
                    // ptsProblem.ProblemTypeCode = dtProblems.Rows[dtProblems.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim();

                    ptsProblem.PtsRemarks = PTDescriptionTextBox.Text.Trim();
                    //ptsProblem.QuerySavedBy = int.Parse(Global.userId);
                    int tempUserID = 0;
                    int.TryParse(Global.userId, out tempUserID);
                    ptsProblem.QuerySavedBy = ptsProblemUser;
                    ptsProblem.TaskClosed = true;

                    if (File.Exists(tempZipFileName))
                        ptsProblem.ZipFileUpload = true;
                    else
                        ptsProblem.ZipFileUpload = false;

                    //ptsProblem.UserName = Global.UserName;
                    ptsProblem.UserName = UserNameTextBox.Text.Trim();
                    ptsProblem.QueryClosedBy = UserNameTextBox.Text.Trim();

                    int tempUserId = 0;
                    int.TryParse(Global.userId, out tempUserId);

                    ptsProblem.UserID = tempUserId;
                    // ptsProblem.UserID = Global.userId;

                    //Get the xml template and replace the values/

                    string problemID = string.Empty;
                    problemID = dtProblems.Rows[0]["ProblemMsgId"].ToString().Trim();
                    ptsProblem.ProblemID = problemID;
                    DataTable dtConfig = new DataTable();
                    //dtConfig = ptsQueryService.GetPTSConfiguration(stageID, customerCode.ToString());

                    //rmt #9677
                    string tempArt = string.Empty;
                    if (ArticleDropDown.SelectedIndex > 0)
                        tempArt = ArticleDropDown.SelectedItem.Text.Trim();
                    else
                        tempArt = VolumeDropDown.SelectedItem.Text.Trim() + ',' + IssueDropDown.SelectedItem.Text.Trim() + ',' + ddlRoundCount.SelectedItem.Text;
                    //dtConfig = ptsQueryService.GetPTSConfiguration(stageID, customerCode.ToString(), Convert.ToInt64(journalCode), ArticleDropDown.SelectedItem.Text.Trim());
                    dtConfig = ptsQueryService.GetPTSConfiguration(stageID, customerCode.ToString(), Convert.ToInt64(journalCode), tempArt);


                    if (dtConfig.Rows.Count > 0)
                    {

                        if (dtConfig.Rows[0]["ResponseType"].ToString().Trim() == "0")
                        {

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
                            //signalTemplate = signalTemplate.Replace("[problemtypecode]", ProblemTypeDropDown.SelectedValue.ToString().Trim());
                            //signalTemplate = signalTemplate.Replace("[problemtypecode]", ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString().Trim());
                            string ProblemTypeCodes = "";
                            string ProblemTags = "";
                            string PtsRemarks = "";
                            string PtsLongDesc = "";
                            for (int i = 0; i < dtPTCategory.Rows.Count; i++)
                            {
                                ProblemTypeCodes = dtPTCategory.Rows[i]["ProblemTypeCode"].ToString().Trim();
                                //PtsRemarks = dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim().Replace("&", "&amp;").Replace("/", "&frasl;");
                                //PtsLongDesc = dtPTCategory.Rows[i]["PTDesc"].ToString().Trim().Replace("&","&amp;").Replace("/","&frasl;");

                                PtsRemarks = dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim().Replace("&", "&amp;");
                                PtsLongDesc = dtPTCategory.Rows[i]["PTDesc"].ToString().Trim().Replace("&","&amp;");
                                ProblemTags += "<problem><problem-type type=\"" + ProblemTypeCodes + "\"/><problem-remark>" + PtsLongDesc + "-" + PtsRemarks + "</problem-remark>";
                                if (i == dtPTCategory.Rows.Count - 1 && File.Exists(tempZipFileName))
                                    ProblemTags += "<file-attached>" + "JQ" + problemID + ".zip" + "</file-attached></problem>";
                                else
                                    ProblemTags += "</problem>";
                            }
                            signalTemplate = signalTemplate.Replace("[problems]", ProblemTags);

                            signalTemplate = signalTemplate.Replace("[problemtypecode]", ProblemTypeCodes.TrimEnd(','));
                            signalTemplate = signalTemplate.Replace("[problemremarks]", PTDescriptionTextBox.Text.Trim());

                            if (jobType == issueType)
                            {
                                signalTemplate = signalTemplate.Replace("[volumefirst]", volumeFirst);
                                signalTemplate = signalTemplate.Replace("[volumelast]", volumeLast);
                                signalTemplate = signalTemplate.Replace("[issuefirst]", issueFirst);
                                signalTemplate = signalTemplate.Replace("[issuelast]", issueLast);
                                signalTemplate = signalTemplate.Replace("[suppl]", suppl);
                            }


                            signalTemplate = signalTemplate.Replace("[msgid]","JQ" + problemID);

                            if (ptsProblem.ZipFileUpload)
                            {
                                zipFileName = zipFileDestination + problemID + ".zip";
                                //if zip file exists move the zip file to ftp area and copy the signal file to another path
                                ptsProblem.ZipFilePath = zipFileName;


                               
                                if (File.Exists(tempZipFileName))
                                {
                                    signalTemplate = signalTemplate.Replace("[zipfilename]", "JQ" + problemID + ".zip");
                                    //File.Delete(tempZipFileName);
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
                                //ptsProblem.ZipFilePath = string.Empty;
                                //ptsProblem.ZipFilePath = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim(); //pks
                                ptsProblem.ZipFilePath = lblZipFile.Text.Trim();
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

                            try
                            {
                                XmlDocument xDoc = new XmlDocument();
                                xDoc.Load(tempSignalFilePath + signalFileName);

                                string ResultMsg = "";
                                //ResultMsg = ptsQueryService.InsertOrUpdatePTSProblem(ptsProblem);
                                ptsProblem.PtsRemarks = PTDescriptionTextBox.Text;
                                ptsProblem.ID = Convert.ToInt32(ddlOpenedRequest.SelectedValue);
                                ptsProblem.FileContent = ViewState["xml"].ToString();

                                DataTable dt = ptsQueryService.UpdateStatus(ptsProblem);
                                if (dt.Rows.Count > 0)
                                {
                                    ResultMsg = dt.Rows[0][0].ToString();
                                    if (Convert.ToInt32(dt.Rows[0][1].ToString().Trim()) >= 0)
                                    {

                                        if (File.Exists(tempZipFileName))
                                        {
                                            File.Copy(tempZipFileName, zipFileName, true);
                                            File.Delete(tempZipFileName);
                                        }
                                        File.Copy(tempSignalFilePath + signalFileName, signalFileDestination + "JQ" + signalFileName, true);
                                        ResultMsg += ". File " + signalFileDestination + "JQ" + signalFileName + " created successfully!";

                                    }
                                    ShowMessage(ResultMsg);
                                    ClearFields();
                                }
                                else
                                {
                                    ResultMsg = "Error in close/raise a query";
                                }
                            }
                            catch
                            {
                                if (File.Exists(tempSignalFilePath + signalFileName))
                                    File.Delete(tempSignalFilePath + signalFileName);
                                ShowMessage("Parsing failed - Unable to Close/Raise query");
                                return;
                            }

                        }
                        else
                        {

                            // string strMsg=ptsQueryService.InsertOrUpdatePTSProblem(ptsProblem);
                            string Emailfrom = dtConfig.Rows[0]["FromEmailID"].ToString(); // updt in db
                            //string EmailTo = dtConfig.Rows[0]["ToEmailID"].ToString(); // updt sp tble to tbl_Job_DistributionList

                            // rmt #9677
                            string EmailTo = dtConfig.Rows[0]["ToEmailID"].ToString().Trim();// +";" + dtConfig.Rows[0]["ToMailJobAuthorContact"].ToString().Trim();
                            string EmailToJobAuthor = dtConfig.Rows[0]["ToMailJobAuthorContact"].ToString().Trim();
                            //



                            // RMT #9677
                            #region NewMailCode
                            DateTime mailtime = DateTime.Now;
                            string mailsendtime = Convert.ToString(mailtime);

                            string Subject = dtConfig.Rows[0]["EmailSubject"].ToString().Trim();
                            string MsgBody = dtConfig.Rows[0]["EmailContent"].ToString();

                            WriteLog(1, Subject, MsgBody, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "Before resolving the variables");

                            if (ArticleDropDown.SelectedIndex > 0)
                                Subject = Subject.Replace("<Journal>", JournalDropDown.SelectedItem.Text.Trim()).Replace("<article>", ArticleDropDown.SelectedItem.Text.Trim()).Replace("<Stage>", StageDropDown.SelectedItem.Text.Trim());
                            else
                                Subject = Subject.Replace("<Journal>", JournalDropDown.SelectedItem.Text.Trim()).Replace("<article>", VolumeDropDown.SelectedItem.Text.Trim() + IssueDropDown.SelectedItem.Text.Trim()).Replace("<Stage>", StageDropDown.SelectedItem.Text.Trim());
                            
                            //Subject = Subject.Replace("<BATCHID>", lblSequence.Text.Trim());
                            Subject = Subject.Replace("<BATCHID>", ddlRoundCount.SelectedValue.ToString().Trim());

                           // string tempPTDescription = PTDescriptionTextBox.Text.Trim().Substring((PTDescriptionTextBox.Text.Trim().IndexOf(')') + 1), (PTDescriptionTextBox.Text.Trim().Length - PTDescriptionTextBox.Text.Trim().IndexOf(')') - 1));
                            string tempPTDescription = PTDescriptionTextBox.Text.Trim();
                            if (ArticleDropDown.SelectedIndex > 0)
                                MsgBody = MsgBody.Replace("<Customer>", CustomerDropDown.SelectedItem.Text.Trim()).Replace("<Journal>", JournalDropDown.SelectedItem.Text.Trim()).Replace("<article>", ArticleDropDown.SelectedItem.Text.Trim()).Replace("<Stage>", StageDropDown.SelectedItem.Text.Trim()).Replace("<ProblemDesc>", tempPTDescription);
                            else
                                MsgBody = MsgBody.Replace("<Customer>", CustomerDropDown.SelectedItem.Text.Trim()).Replace("<Journal>", JournalDropDown.SelectedItem.Text.Trim()).Replace("<article>", VolumeDropDown.SelectedItem.Text.Trim() + ',' + IssueDropDown.SelectedItem.Text.Trim()).Replace("<Stage>", StageDropDown.SelectedItem.Text.Trim()).Replace("<ProblemDesc>", tempPTDescription);


                            tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);
                            tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);


                            ptsProblem.ZipFilePath = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim();
                            //if (Session["zipFile"] != null)
                            //    ptsProblem.ZipFilePath = Session["zipFile"].ToString().Trim();
                            //else
                            //    ptsProblem.ZipFilePath = string.Empty;
                            string ResultMsg = "";

                            WriteLog(1, "", "", Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "Db updation started");
                            ptsProblem.PtsRemarks = PTDescriptionTextBox.Text;
                            ptsProblem.ID = Convert.ToInt32(ddlOpenedRequest.SelectedValue);
                            ptsProblem.FileContent = ViewState["xml"].ToString();
                            //ResultMsg = ptsQueryService.UpdateStatus(ptsProblem);
                            DataTable dt = ptsQueryService.UpdateStatus(ptsProblem);
                            if (dt.Rows.Count > 0)
                            {
                                ResultMsg = dt.Rows[0][0].ToString();
                                if (Convert.ToInt32(dt.Rows[0][1].ToString().Trim()) >= 0)
                                {
                                    WriteLog(1, "", "", Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "Db updation completed");

                                    ptsProblem.SubStageId = stageID;

                                    MsgBody = MsgBody.Replace("<InquirerName>", hdnReviewerName.Value);
                                    MsgBody = MsgBody.Replace("<ReviewerName>", hdnUserName.Value);
                                    MsgBody = MsgBody.Replace("|", "<br />");

                                    MsgBody = MsgBody.Replace("\r\n", "<br />");

                                    WriteLog(1, Subject, MsgBody, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "After resolving the variables");

                                    try
                                    {
                                        string chkSubject = Subject.ToUpper();
                                        if (chkSubject.Contains("<STAGE>") || chkSubject.Contains("<JOURNAL>") || chkSubject.Contains("<ARTICLE>"))
                                        {
                                            ResultMsg += "Error in sending mail. Please contact itracksupport.";
                                            WriteLog(1, Subject, MsgBody, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "Subject contains special characters");
                                            ShowMessage(ResultMsg);
                                            return;
                                        }
                                        else if (chkSubject.Contains("<") || chkSubject.Contains(">"))
                                        {
                                            ResultMsg += "Error in sending mail. Please contact itracksupport.";
                                            WriteLog(1, Subject, MsgBody, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "Subject contains special characters");
                                            ShowMessage(ResultMsg);
                                            return;
                                        }
                                        else
                                        {
                                            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                                            message.From = new System.Net.Mail.MailAddress(Emailfrom);

                                            string[] tempMail = EmailTo.Split(',');
                                            for (int i = 0; i < tempMail.Length; i++)
                                                message.To.Add(new System.Net.Mail.MailAddress(tempMail[i]));

                                            tempMail = EmailToJobAuthor.Split(',');
                                            for (int i = 0; i < tempMail.Length; i++)
                                                if (tempMail[i].Trim() != "")
                                                    message.CC.Add(new System.Net.Mail.MailAddress(tempMail[i]));

                                            WriteLog(1, message.Subject, message.Body, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "Before Sending mail");
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
                                                    if (ArticleDropDown.SelectedIndex > 0)
                                                        tempFileName = tempFileName.Replace("<JOURNAL>", JournalDropDown.SelectedItem.Text.Trim()).Replace("<ARTICLE>", ArticleDropDown.SelectedItem.Text.Trim()).Replace("<STAGE>", StageDropDown.SelectedItem.Text.Trim());
                                                    else
                                                        tempFileName = tempFileName.Replace("<JOURNAL>", JournalDropDown.SelectedItem.Text.Trim()).Replace("<ARTICLE>", VolumeDropDown.SelectedItem.Text.Trim() + ',' + IssueDropDown.SelectedItem.Text.Trim()).Replace("<STAGE>", StageDropDown.SelectedItem.Text.Trim());

                                                    //tempFileName = tempFileName.Replace("<BATCHID>", lblSequence.Text.Trim());
                                                    tempFileName = tempFileName.Replace("<BATCHID>", ddlRoundCount.SelectedValue.ToString().Trim());


                                                    objCD.FileName = tempFileName;
                                                    message.Attachments.Add(MAttNew);
                                                }
                                            }

                                            message.IsBodyHtml = true;
                                            message.Priority = System.Net.Mail.MailPriority.Normal;
                                            message.Subject = Subject;
                                            message.Body = MsgBody;

                                            client.Send(message);
                                            WriteLog(1, message.Subject, message.Body, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, "After Sending mail");
                                            System.Threading.Thread.Sleep(500);
                                            ResultMsg += ". Mail has been sent to " + EmailTo + "; " + EmailToJobAuthor;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        WriteLog(1, SubForLog, MsgBodyForLog, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), ArtvolIssue, ex.Message);
                                        throw new Exception(ex.Message, ex);
                                    }
                                }
                                else
                                {
                                    ResultMsg = "Error in close/raise a query";
                                }
                                ShowMessage(ResultMsg);
                                ClearFields();
                            }
                            else
                            {
                                ResultMsg = "Error in close/raise a query";
                            }

                          
                            #endregion NewMailCode


                        }
                    }
                    else
                    {
                        ShowMessage("No configuration is available for this stage");
                        return;
                    }
                }
                else
                {
                    ShowMessage("Please select at least one problem type");
                }
                #endregion try

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
                //WriteLog(1, SubForLog, MsgBodyForLog, Convert.ToString(StageDropDown.SelectedItem.Text), Convert.ToString(JournalDropDown.SelectedItem.Text), Convert.ToString(ArticleDropDown.SelectedItem.Text), ex.Message);
            }

        }

        //Email Validation for Response giving... 
        public static bool Emailvalidation(string valemail)
        {
            try
            {
                Regex expression = new Regex(@"\w+@[a-zA-Z_-]+?\.[a-zA-Z]{2,3}");
                if (expression.IsMatch(valemail))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FillDropdownCombos

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

        private void FillArticleDDL(int customerCode, int journalCode, string Vol, string Issue)
        {
            DataTable dtArticle = new DataTable();
            dtArticle = ptsQueryService.GetWIPArticles(customerCode, journalCode, Vol, Issue);

            ArticleDropDown.DataSource = dtArticle;
            ArticleDropDown.DataTextField = Global.DB_FIELD_ID.Trim();
            ArticleDropDown.DataValueField = Global.DB_FIELD_NAME.Trim();
            ArticleDropDown.DataBind();
            ArticleDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }

        private void FillProblemTypeDDL(int CustomerId, int DeptId, int SubStageID)
        {
            DataTable dtProblemType = new DataTable();
            dtProblemType = ptsQueryService.GetProblemTypesByStage(CustomerId,DeptId, SubStageID);

            ProblemTypeDropDown.DataSource = dtProblemType;
            ProblemTypeDropDown.DataValueField = Global.DB_FIELD_ID.Trim();
            ProblemTypeDropDown.DataTextField = Global.DB_FIELD_NAME.Trim();
            ProblemTypeDropDown.DataBind();
            ProblemTypeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

        }

        private void FillProblemTypeCategoryDDL(string code, int DepartmentId, int SubStageID)
        {
            DataTable dtProblemType = new DataTable();
            dtProblemType = ptsQueryService.GetProblemTypeCategoryByProbelmType(code, DepartmentId, SubStageID);

            ddlProblemTypeCategory.DataSource = dtProblemType;
            ddlProblemTypeCategory.DataValueField = Global.DB_FIELD_ID.Trim();
            ddlProblemTypeCategory.DataTextField = Global.DB_FIELD_NAME.Trim();
            ddlProblemTypeCategory.DataBind();
            ddlProblemTypeCategory.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

        }
        private void FillVolumeDDL(int customerCode, int journalCode, string StageId)
        {
            DataTable dtVolumeIssue = new DataTable();

            dtVolumeIssue = ptsQueryService.GetWIPVolumeIssues("volume", customerCode, journalCode, StageId);
            VolumeDropDown.DataSource = dtVolumeIssue;
            VolumeDropDown.DataValueField = Global.DB_FIELD_ID.Trim();
            VolumeDropDown.DataTextField = Global.DB_FIELD_ID.Trim();
            VolumeDropDown.DataBind();
            VolumeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));


        }
        //changed
        private void FillIssueDDL(int customerCode, int journalCode, string VolID)
        {
            DataTable dtVolumeIssue = new DataTable();

            dtVolumeIssue = ptsQueryService.GetWIPVolumeIssues("issue", customerCode, journalCode, VolID);
            IssueDropDown.DataSource = dtVolumeIssue;
            IssueDropDown.DataTextField = Global.DB_FIELD_ID.Trim().ToLower();
            IssueDropDown.DataValueField = Global.DB_FIELD_ID.Trim().ToLower();
            IssueDropDown.DataBind();
            IssueDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

        }

        private void FillVolIssRoundCountDDL(int CustCode, int JourCode, string Volume, string Issue)
        {
            DataTable dtVolIssRoundCount = new DataTable();

            dtVolIssRoundCount = ptsQueryService.GetWIPVolIssRoundCount(CustCode, JourCode, Volume, Issue);
            ddlRoundCount.DataSource = dtVolIssRoundCount;
            ddlRoundCount.DataValueField = Global.DB_FIELD_ID.Trim();
            ddlRoundCount.DataTextField = Global.DB_FIELD_NAME.Trim();
            ddlRoundCount.DataBind();
            ddlRoundCount.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

            if (dtVolIssRoundCount.Rows.Count == 1)
                ddlRoundCount.SelectedIndex = 1;
            else
                ddlRoundCount.SelectedIndex = 0;
        }

        private void FillRoundCountDDL(int CustCode, int JourCode, string Article)
        {
            DataTable dtVolIssRoundCount = new DataTable();

            dtVolIssRoundCount = ptsQueryService.GetWIPRoundCountByArticle(CustCode, JourCode, Article);
            ddlRoundCount.DataSource = dtVolIssRoundCount;
            ddlRoundCount.DataValueField = Global.DB_FIELD_ID.Trim();
            ddlRoundCount.DataTextField = Global.DB_FIELD_NAME.Trim();
            ddlRoundCount.DataBind();
            //ddlRoundCount.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

        }
        #endregion

        #region User define methods

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

        private void BindPreviousRemarks()
        {
            customerCode = 0;
            journalCode = 0;
            article = string.Empty;
            stageID = 0;

            int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
            int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);
            if (ArticleDropDown.SelectedIndex > 0)
                article = ArticleDropDown.SelectedItem.Text.Trim(); // ArticleDropDown.SelectedValue.ToString().Trim();

            int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
            stage = StageDropDown.SelectedItem.Text.Trim();


            volume = VolumeDropDown.SelectedValue.ToString().Trim();
            issue = IssueDropDown.SelectedValue.ToString().Trim();

            // Code added by Prasanta
            CustCode = customerCode.ToString().Trim();
            JourCode = journalCode.ToString().Trim();
            ArtCode = article;
            //GetCloseStatus(customerCode.ToString(), journalCode.ToString(), article); // #13386
            //==============================

            DataTable dtPreviousRemarks = new DataTable();
            //dtPreviousRemarks = ptsQueryService.GetPreviousRemarks(customerCode, journalCode, article, stageID.ToString());
            dtPreviousRemarks = ptsQueryService.GetPreviousRemarks(customerCode, journalCode, article, volume, issue, stageID.ToString());

            GVProblemHistory.DataSource = dtPreviousRemarks;
            GVProblemHistory.DataBind();

            EnableDisablePrvRemark();
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
                    SaveButton.Enabled = true;
                else
                    SaveButton.Enabled = false;

                ZipButton.Enabled = true;
                CloseButton.Enabled = false;
                QueryResolveButton.Enabled = false;
            }
            
        }

        /// <summary>
        /// Get Close Status
        /// </summary>
        /// <param name=""></param>
        private void GetCloseStatus(string strCust, string strJour, string strArt, string strVol, string strIss, string SubStageId, string ProblemTypeCode)
        {
            int CloseStatus = 0;
            DataTable dt;
            roleID = ptsQueryService.GetRoleIDByUserName(userName, customerCode.ToString());
            try
            {
                dt = ptsQueryService.GetCloseStatus(strCust, strJour, strArt, strVol, strIss, SubStageId, ProblemTypeCode);
                if (dt.Rows.Count > 0)
                {
                    CloseStatus = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (CloseStatus == 1)
                    {
                        SaveButton.Enabled = false;

                        ZipButton.Enabled = true;
                        CloseButton.Enabled = false;
                        // Added by Ram
                        bool allowResolveQuery = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQueryResolveButton);

                        if (allowResolveQuery)
                            QueryResolveButton.Enabled = true;
                        else
                            QueryResolveButton.Enabled = false;
                        //QueryResolveButton.Enabled = true; BY Ram
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

                        //CloseButton.Enabled = true; By Ram
                        QueryResolveButton.Enabled = false;
                    }
                    else
                    {
                        // Addes by Ram
                        bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.NewQuerySavebutton);
                        if (allowSave)
                            SaveButton.Enabled = true;
                        else
                            SaveButton.Enabled = false;

                        //SaveButton.Enabled = true; By Ram
                        ZipButton.Enabled = true;
                        CloseButton.Enabled = false;
                        QueryResolveButton.Enabled = false;
                    }
                    hdnReviewerName.Value = dt.Rows[0][1].ToString();
                    //hdnUserName.Value = dt.Rows[0][2].ToString();
                }
                else
                    hdnReviewerName.Value = "";
            }
            catch { throw; }
            //return CloseStatus;
        }

        private void BindPreviousRemarks(int customerCode, int journalCode, string volume, string issue, string stage)
        {
            if (ArticleDropDown.SelectedIndex > 0)
                article = ArticleDropDown.SelectedItem.Text.Trim(); // ArticleDropDown.SelectedValue.ToString().Trim();
            else
                article = string.Empty;

            DataTable dtPreviousRemarks = new DataTable();
            // Code added by Prasanta
            CustCode = customerCode.ToString().Trim();
            JourCode = journalCode.ToString().Trim();
            ArtCode = article;
            //GetCloseStatus(customerCode.ToString(), journalCode.ToString(), article);

            //==============================
            //dtPreviousRemarks = ptsQueryService.GetPreviousRemarks(customerCode, journalCode, volume, issue, stage);
            dtPreviousRemarks = ptsQueryService.GetPreviousRemarks(customerCode, journalCode, article, volume, issue, stage);

            GVProblemHistory.DataSource = dtPreviousRemarks;
            GVProblemHistory.DataBind();
            EnableDisablePrvRemark();
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

        private void EnableArticlePanel()
        {
            ArticlePanel.Visible = true;
            VolumeIssuePanel.Visible = false;
            VolumeDropDown.SelectedIndex = -1;
            IssueDropDown.SelectedIndex = -1;
            ddlRoundCount.SelectedIndex = -1;
        }

        private void EnableVolumeIssuePanel()
        {
            VolumeIssuePanel.Visible = true;
            ArticlePanel.Visible = false;
            ArticleDropDown.SelectedIndex = -1;
        }

        private void EnableArtVolumeIssuePanel()
        {
            VolumeIssuePanel.Visible = true;
            ArticlePanel.Visible = true;
            ArticleDropDown.SelectedIndex = -1;
            VolumeDropDown.SelectedIndex = -1;
            IssueDropDown.SelectedIndex = -1;
            ddlRoundCount.SelectedIndex = -1;
        }

        private void HidePanels()
        {
            //ArticlePanel.Visible = false;
            //VolumeIssuePanel.Visible = false;

            //ArticleDropDown.SelectedIndex = -1;
            //VolumeDropDown.SelectedIndex = -1;
            //IssueDropDown.SelectedIndex = -1;
            //ddlRoundCount.SelectedIndex = -1;
        }

        private string FormatPIINumber(string piiNumber)
        {
            //string piiNumberFormat = ConfigurationManager.AppSettings["ArticlePIINumberFormat"].ToString().Trim();
            string piiNumberFormat = string.Empty;

            if (jobType == articleType)
                piiNumberFormat = ConfigurationManager.AppSettings["ArticlePIINumberFormat"].ToString().Trim();
            else if (jobType == issueType)
                piiNumberFormat = ConfigurationManager.AppSettings["IssuePIINumberFormat"].ToString().Trim();

            char[] piiNumberArr = piiNumber.ToCharArray();
            char[] piiNumberFormatArr = piiNumberFormat.ToCharArray();

            int j = 0, k = 0;
            for (int i = 0; i < piiNumberFormatArr.Length; i++)
            {
                if (piiNumberFormatArr[i] == '#')
                {
                    if (piiNumberArr.Length > j)
                    {
                        piiNumberFormatArr[i] = piiNumberArr[j];
                        j++;
                    }
                }
                k++;
            }

            string formattedPIINumber = new string(piiNumberFormatArr);

            return formattedPIINumber;
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

        private void ClearFields()
        {
            StageDropDown.SelectedIndex = -1;
            CustomerDropDown.SelectedIndex = -1;
            JournalDropDown.SelectedIndex = -1;
            ArticleDropDown.SelectedIndex = -1;
            ddlDepartment.SelectedIndex = -1;
            VolumeDropDown.SelectedIndex = -1;
            IssueDropDown.SelectedIndex = -1;
            ddlRoundCount.SelectedIndex = -1;
            ProblemTypeDropDown.SelectedIndex = -1;
            PTDescriptionTextBox.Text = string.Empty;
            MasterCopyJournalLabel.Visible = false;
            txtRemarks.Text = string.Empty;
            // code added by prasanta start
            lblZipFile.Text = string.Empty;
            
            Session["zipFile"] = null;
            lbtnZipFile.Text = string.Empty;
            //lbtnZipFile.Visible = false;
            // code added by prasanta end

            GVProblemHistory.DataSource = null;
            GVProblemHistory.DataBind();
            EnableDisablePrvRemark();

            lblSequence.Text = string.Empty;
            ddlOpenedRequest.Items.Clear();
            hdnReviewerName.Value = "";

            ddlOpenedRequest.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
      

            SaveButton.Enabled = false;
            CloseButton.Enabled = false;
            ZipButton.Enabled = false;
            QueryResolveButton.Enabled = false;

            DataTable dtPTCategory = new DataTable();
            hdnXml.Value = "FirstTime";
            ViewState["xml"] = "";
            ViewState["dtPTCategory"] = dtPTCategory;
            gvwPTCategory.DataSource = dtPTCategory;
            gvwPTCategory.DataBind();
        }

        #endregion


        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            // #12Dec2014
            // ======================
            string fileName = Path.GetFileName(fileUpload.PostedFile.FileName);
            if ( CustomerDropDown.SelectedIndex > 0 && JournalDropDown.SelectedIndex > 0 && StageDropDown.SelectedIndex > 0)
            {
                string configPath = string.Empty;
                int CustCode = 0;
                int.TryParse(CustomerDropDown.SelectedValue.Trim(), out CustCode);
                configPath = ptsQueryService.GetQueryResolvedUploadPath(CustCode, BizPTSQuery.tName.QueryResolvedUploadPath.ToString().Trim());

                // \\192.168.4.208\Temp\<<Customer>>\<<Jouranl>>\<<Article>>\<<Vol>>\<<Iss>>\<<Stage>>\QueryResovled-<<Datetime>>
                configPath = configPath.Replace("<<Customer>>", CustomerDropDown.SelectedItem.Text.Trim());
                configPath = configPath.Replace("<<Jouranl>>", JournalDropDown.SelectedItem.Text.Trim());
                if (ArticleDropDown.SelectedIndex > 0)
                {
                    configPath = configPath.Replace("<<Article>>", ArticleDropDown.SelectedItem.Text.Trim());
                    configPath = configPath.Replace("<<Vol>>\\", string.Empty);
                    configPath = configPath.Replace("<<Iss>>\\", string.Empty);
                }
                else
                {
                    configPath = configPath.Replace("<<Article>>\\", string.Empty);
                    if (VolumeDropDown.SelectedIndex > 0)
                        configPath = configPath.Replace("<<Vol>>", VolumeDropDown.SelectedItem.Text.Trim());
                    if (IssueDropDown.SelectedIndex > 0)
                        configPath = configPath.Replace("<<Iss>>", IssueDropDown.SelectedItem.Text.Trim());
                }
                configPath = configPath.Replace("<<Stage>>", StageDropDown.SelectedItem.Text.Trim());
                configPath = configPath.Replace("<<Datetime>>", DateTime.Now.ToString("dd_MMM_yyyy_HHmmss"));


                if (configPath.ToString() != string.Empty )
                {
                    if (!Directory.Exists(configPath))
                        Directory.CreateDirectory(configPath);
                    if (File.Exists(fileUpload.PostedFile.FileName))
                    fileUpload.PostedFile.SaveAs(configPath + @"\" + fileName);

                    if (txtRemarks.Text.Trim() != string.Empty)
                    {
                        StreamWriter sw = new StreamWriter(configPath + @"\" + "QueryResponse.txt");
                        sw.Write(txtRemarks.Text.Trim());
                        sw.Flush();
                        sw.Close();
                        sw.Dispose();
                    }
                }
                else
                {
                    ShowMessage("");
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

            int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
            int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
            int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);

            article = ArticleDropDown.SelectedItem.Text.Trim(); // ArticleDropDown.SelectedValue.ToString().Trim();
            stage = StageDropDown.SelectedItem.Text.Trim();
            volume = VolumeDropDown.SelectedValue.ToString().Trim();
            issue = IssueDropDown.SelectedValue.ToString().Trim();
            try
            {
                jobType = JobTypeHidden.Value;

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

                ptsProblem.PtsRemarks = PTDescriptionTextBox.Text.Trim();

                int tempUserId = 0;
                int.TryParse(Global.userId, out tempUserId);

                //ptsProblem.QuerySavedBy = int.Parse(Global.userId);
                ptsProblem.TaskClosed = true;
                ptsProblem.ProblemID = string.Empty;


                string zipFileName = string.Empty;
                DataTable dtProblems = ptsQueryService.GetPTSProblems(customerCode, journalCode, article, stageID);
                if (dtProblems.Rows.Count > 0)
                    zipFileName = dtProblems.Rows[0]["ZipFilePath"].ToString().Trim();


                //string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                //string tempZipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString().Trim();
                //tempZipFilePath = clsServerConfig.GetStringReplacement(tempZipFilePath, customerCode);

                //tempZipFilePath = tempZipFilePath.Replace("[stage]", stage);
                ////tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[username]", Global.UserName.Trim());
                ////tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[problemtypeid]", ProblemTypeDropDown.SelectedValue.Trim()).Replace("[username]", UserNameTextBox.Text.Trim());
                //tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", article.Trim()).Replace("[stage]", stage).Replace("[problemtypeid]", ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).Trim()).Replace("[username]", UserNameTextBox.Text.Trim());
                //tempZipFileName = tempZipFileName.Replace(" ", string.Empty);

                // string zipFileName = tempZipFilePath + @"\" + tempZipFileName;
                ////end of getting the uploaded the zip file path and name


                if (File.Exists(zipFileName))
                    ptsProblem.ZipFileUpload = true;
                else
                    ptsProblem.ZipFileUpload = false;

                ptsProblem.UserName = userName;
                ptsProblem.ID = Convert.ToInt32(ddlOpenedRequest.SelectedValue);
                ptsProblem.UserID = tempUserId;
                ptsProblem.QueryResolveRemarks = txtRemarks.Text.Trim();
                ShowMessage(ptsQueryService.ResolvePTSProblemByID(ptsProblem));
                ResultLabel.Text = string.Empty;


                // code added by prasanta start
                // delete the file if exist 
                string problemID = string.Empty;
                problemID = ptsQueryService.GetPTSProblemID();

                string zipFileDestination = "";
                string signalTemplate = "";
                string signalFileDestination = "";

                //zipFileName = zipFileDestination + problemID + ".zip";

                // updated by prasanta
                string strZipFileBkp = string.Empty;
                //if (File.Exists(zipFileName))
                //{
                //    strZipFileBkp = "_" + DateTime.Now.Year.ToString().Trim() + "_" + DateTime.Now.Month.ToString().Trim() + "_" + DateTime.Now.Day.ToString().Trim() + "_" + DateTime.Now.Hour.ToString().Trim() + "_" + DateTime.Now.Minute.ToString().Trim() + "_" + DateTime.Now.Second.ToString().Trim();
                //    strZipFileBkp = zipFileName.Replace(".zip", strZipFileBkp + ".zip");

                //    //File.Copy(zipFileName, strZipFileBkp, true);
                //}
                if (File.Exists(zipFileName))
                {
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

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        // code added by prasanta start
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
        
        private void bindOpenedRequests(PTSProblem objPTSProblem)
        {
            try
            {
                DataTable dtOpenedRequest = new DataTable();
                dtOpenedRequest = ptsQueryService.GetOpenedRequests(objPTSProblem);
                if (dtOpenedRequest != null)
                {
                    ddlOpenedRequest.DataSource = dtOpenedRequest;
                    ddlOpenedRequest.DataTextField = Global.DB_FIELD_NAME;
                    ddlOpenedRequest.DataValueField = Global.DB_FIELD_ID;
                    ddlOpenedRequest.DataBind();

                }
                ddlOpenedRequest.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                GetCloseStatusNew(CustomerDropDown.SelectedValue, -1);
            }
            catch { throw; }
        }
        protected void ddlOpenedRequest_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpenedRequestSelectIndexChanged();
        }
        private void OpenedRequestSelectIndexChanged()
        {
            MessageLabel.Text = string.Empty;
            stage = string.Empty;
            string problemTypeDescription = string.Empty;

            try
            {
                if (ddlOpenedRequest.SelectedIndex == 0)
                {
                    DataTable dtPTCategory = new DataTable();
                    hdnXml.Value = "FirstTime";
                    ViewState["xml"] = "";
                    ViewState["dtPTCategory"] = dtPTCategory;
                    gvwPTCategory.DataSource = dtPTCategory;
                    gvwPTCategory.DataBind();

                    PTDescriptionTextBox.Text = string.Empty;
                    SaveButton.Enabled = true;
                    return;
                }
                PTSProblem ptsProblem = new PTSProblem();
                ptsProblem.CustomerCode = Convert.ToInt32(CustomerDropDown.SelectedValue.Trim());
                ptsProblem.JournalCode = Convert.ToInt32(JournalDropDown.SelectedValue.Trim());

                //Added by - Ram for loading dept
                DataTable dtProblems = new DataTable();

                string deptId = "0";

                dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(ddlOpenedRequest.SelectedValue.Trim()));

                DataRow[] dr = dtProblems.Select();

                if (dr.Length > 0)
                {
                    hfZipFilePath.Value = dr[0]["ZipFilePath"].ToString().Trim();
                    //txtZipFilePath.Text = dr[0]["ZipFilePath"].ToString().Trim(); 

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
                    ddlDepartment.SelectedValue = deptId;
                    stage = dr[0]["SUBSTAGEID"].ToString().Trim();
                    PTDescriptionTextBox.Text = dr[0]["PTSRemarks"].ToString().Trim();
                    int CloseStatus = 0;
                    if (Convert.ToBoolean(dr[0]["TaskClosed"].ToString()))
                        CloseStatus = 1;

                    DataTable dtProblemDetails = ptsQueryService.GetProblemDetails(Convert.ToInt32(dr[0]["Id"].ToString().Trim()));
                    gvwPTCategory.DataSource = dtProblemDetails;
                    gvwPTCategory.DataBind();
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
                    ddlDepartment.SelectedIndex = 0;
                    hdnReviewerName.Value = userName;
                    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), 0);
                }

                if (ArticleDropDown.SelectedIndex > 0)
                    objBizPTSQuery.Article = ArticleDropDown.SelectedItem.Text.Trim();
                else
                    objBizPTSQuery.Article = string.Empty;

                objBizPTSQuery.Volume = VolumeDropDown.SelectedItem.Text.Trim();
                objBizPTSQuery.Issue = IssueDropDown.SelectedItem.Text.Trim();

                //===================================
                string customerGroupName = string.Empty;
                string zipFilePath = ConfigurationManager.AppSettings["tempZipFilePath"].ToString();
                zipFilePath = clsServerConfig.GetStringReplacement(zipFilePath, Convert.ToInt32(CustomerDropDown.SelectedValue.Trim())); //customerGroupName

                zipFilePath = zipFilePath.Replace("[stage]", StageDropDown.SelectedItem.Text.Trim());
                ViewState["zipFilePath"] = zipFilePath;

                DataTable dtCustomerGrName = new DataTable();
                dtCustomerGrName = ptsQueryService.GetCustomerGroupNameAndJournalName(ptsProblem.CustomerCode, ptsProblem.JournalCode);
                if (dtCustomerGrName.Rows.Count > 0)
                {
                    customerGroupName = dtCustomerGrName.Rows[0]["cust_grname"].ToString().Trim();
                }

                //string tempZipFileName = ConfigurationManager.AppSettings["tempZipFileName"].ToString().Trim();
                ////tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", ArticleDropDown.SelectedItem.Text.Trim()).Replace("[stage]", StageDropDown.SelectedItem.Text.Trim()).Replace("[problemtypeid]", ProblemTypeDropDown.SelectedValue.Trim()).Replace("[username]", userName.Trim());
                //if(ProblemTypeDropDown.SelectedIndex > 0)
                //    tempZipFileName = tempZipFileName.ToLower().Replace("[customer]", customerGroupName.Trim()).Replace("[journal]", JournalDropDown.SelectedItem.Text.Trim()).Replace("[article]", ArticleDropDown.SelectedItem.Text.Trim()).Replace("[stage]", StageDropDown.SelectedItem.Text.Trim()).Replace("[problemtypeid]", ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).Trim()).Replace("[username]", userName.Trim());
                //tempZipFileName = tempZipFileName.Replace(" ", string.Empty);


                hdnXml.Value = "1";
                //string zipFileName = zipFilePath + @"\" + tempZipFileName;

                //if (File.Exists(zipFileName))
                //{
                //    lbtnZipFile.Visible = true;
                //    lbtnZipFile.Text = "(Download)";
                //    lblZipFile.Text = zipFileName;
                //}
                //else
                //{
                //    lbtnZipFile.Visible = false;
                //    lbtnZipFile.Text = string.Empty;
                //    lblZipFile.Text = string.Empty;
                //}
                //===================================

                DisablePTCategory();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        private void EnableDisableDDLOpenedRequest()
        {
            try
            {
                if (ddlOpenedRequest.Items.Count > 1)
                {
                    ddlOpenedRequest.Visible = true;
                    lblOpenedRequests.Visible = true;
                }
                else
                {
                    ddlOpenedRequest.Visible = false;
                    lblOpenedRequests.Visible = false;
                }
            }
            catch { throw; }
        }
        private void EnableDisablePrvRemark()
        {
            try
            {
                if (GVProblemHistory.DataSource != null)
                {
                    if (GVProblemHistory.Rows.Count > 0)
                        lblPreviousRemarks.Visible = true;
                    else
                        lblPreviousRemarks.Visible = false;
                }
                else
                {
                    lblPreviousRemarks.Visible = false;
                }
            }
            catch { throw; }
        }
        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            int CustomerID = Convert.ToInt32(CustomerDropDown.SelectedValue.ToString().Trim());
            if (ArticleDropDown.SelectedIndex > 0)
            {
                //PTDescriptionTextBox.Text = "";
                
                FillProblemTypeDDL(CustomerID,Convert.ToInt32(ddlDepartment.SelectedValue), Convert.ToInt32(StageDropDown.SelectedValue));
            }
            if (VolumeDropDown.SelectedIndex > 0 && IssueDropDown.SelectedIndex > 0)
            {
                //PTDescriptionTextBox.Text = "";
                FillProblemTypeDDL(CustomerID,Convert.ToInt32(ddlDepartment.SelectedValue), Convert.ToInt32(StageDropDown.SelectedValue));
            }
        }
        // code added by prasanta end

        #region VolIss
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
        #endregion VolIss

        #region PvtMethods
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
        #endregion PvtMethods

        #region PType
        protected void btnPTypeAdd_Click(object sender, EventArgs e)
        {
            string totalRemarks = string.Empty;

            if (ProblemTypeDropDown.SelectedIndex <= 0)
            {
                ShowMessage("Please select problem type.");
                return;
            }
            if (ddlProblemTypeCategory.Items.Count > 1 && ddlProblemTypeCategory.SelectedIndex == 0)
            {
                ShowMessage("Please select problem type category.");
                return;
            }
            if (ddlOpenedRequest.Items.Count > 1)
            {
                for (int i = 1; i < ddlOpenedRequest.Items.Count; i++)
                {
                    if (ddlOpenedRequest.SelectedIndex != i)
                    {
                        string str = ddlOpenedRequest.Items[i].ToString();
                        if (str.Contains(","))
                        {
                            string[] str1 = str.Split(',');
                            for (int j = 0; j < str1.Length; j++)
                            {
                                // if (ProblemTypeDropDown.SelectedValue == str1[j].Trim())
                                if (ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))) == str1[j].Trim())
                                {
                                    ShowMessage("Selected problemtype already exists in opened requests.");
                                    return;
                                }
                            }
                        }
                        else
                        {
                            //if (ProblemTypeDropDown.SelectedValue == str.Trim())
                            if (ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))) == str.Trim())
                            {
                                ShowMessage("Selected problemtype already exists in opened requests.");
                                return;
                            }
                        }
                    }
                }
            }
            if (PTDescriptionTextBox.Text.Trim()!=string.Empty)
            {
                //if (PTDescriptionTextBox.Text.Trim().Contains("(" + ProblemTypeDropDown.SelectedValue.Trim() + ")"))
                if (PTDescriptionTextBox.Text.Trim().Contains("(" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).Trim() + ")"))
                {
                    ShowMessage("Problem type already exists.");
                    return;
                }
            }
            MessageLabel.Text = string.Empty;
            stage = string.Empty;
            string TotalProblems = string.Empty;
            string problemTypeDescription = string.Empty;

            try
            {
                string strPTCategory = string.Empty;
                if (ddlProblemTypeCategory.SelectedIndex > 0)
                    strPTCategory = ddlProblemTypeCategory.SelectedItem.Text.Trim();
                else
                    strPTCategory = string.Empty;

                if (StageDropDown.SelectedIndex > 0)
                    stage = StageDropDown.SelectedValue;
                else
                    stage = string.Empty;


                problemTypeDescription = ptsQueryService.GetProblemTypeDescription(ProblemTypeDropDown.SelectedValue.ToString().Trim());
                problemTypeDescription= problemTypeDescription.Replace("<", "&lt;");
                problemTypeDescription = problemTypeDescription.Replace(">", "&gt;");
                DataTable dtPTCategory = (DataTable)ViewState["dtPTCategory"];

                if (hdnXml.Value != "FirstTime")
                {
                    int i = ViewState["xml"].ToString().Length;
                    //////xmlValue = ViewState["xml"].ToString().Remove(i - 10, 10) + "<ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedValue.ToString() + "</ProblemTypeId><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId></ProblemType></Details>";
                    ////xmlValue = ViewState["xml"].ToString().Remove(i - 10, 10) + "<ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString() + "</ProblemTypeId><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId><PTCategoryRemark>" + txtPTCategoryRemark.Text.Trim() + "</PTCategoryRemark><ProblemCategory>" + strPTCategory + "</ProblemCategory><PTDesc>" + ProblemTypeDropDown.SelectedItem.Text.Trim() + "</PTDesc></ProblemType></Details>";
                    //xmlValue = ViewState["xml"].ToString().Remove(i - 10, 10) + "<ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedValue.ToString() + "</ProblemTypeId><ProblemTypeCode>" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString() + "</ProblemTypeCode><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId><PTCategoryRemark>" + txtPTCategoryRemark.Text.Trim() + "</PTCategoryRemark><ProblemCategory>" + strPTCategory + "</ProblemCategory><PTDesc>" + ProblemTypeDropDown.SelectedItem.Text.Trim() + "</PTDesc></ProblemType></Details>";
                    
                    xmlValue = ViewState["xml"].ToString().Remove(i - 10, 10) + "<ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedValue.ToString() + "</ProblemTypeId><ProblemTypeCode>" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString() + "</ProblemTypeCode><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId><PTCategoryRemark>" + txtPTCategoryRemark.Text.Trim() + "</PTCategoryRemark><ProblemCategory>" + strPTCategory + "</ProblemCategory><PTDesc>" + problemTypeDescription.Trim() + "</PTDesc></ProblemType></Details>";
                }
                else
                {
                    //////xmlValue = "<Details><ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedValue.ToString() + "</ProblemTypeId><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId></ProblemType></Details>";
                    ////xmlValue = "<Details><ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString() + "</ProblemTypeId><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId><PTCategoryRemark>" + txtPTCategoryRemark.Text.Trim() + "</PTCategoryRemark><ProblemCategory>" + strPTCategory + "</ProblemCategory><PTDesc>" + ProblemTypeDropDown.SelectedItem.Text.Trim() + "</PTDesc></ProblemType></Details>";
                    //xmlValue = "<Details><ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedValue.ToString() + "</ProblemTypeId><ProblemTypeCode>" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString() + "</ProblemTypeCode><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId><PTCategoryRemark>" + txtPTCategoryRemark.Text.Trim() + "</PTCategoryRemark><ProblemCategory>" + strPTCategory + "</ProblemCategory><PTDesc>" + ProblemTypeDropDown.SelectedItem.Text.Trim() + "</PTDesc></ProblemType></Details>";
                    
                    xmlValue = "<Details><ProblemType><ProblemTypeId>" + ProblemTypeDropDown.SelectedValue.ToString() + "</ProblemTypeId><ProblemTypeCode>" + ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).ToString() + "</ProblemTypeCode><ProblemCategoryId>" + ddlProblemTypeCategory.SelectedValue.ToString() + "</ProblemCategoryId><PTCategoryRemark>" + txtPTCategoryRemark.Text.Trim() + "</PTCategoryRemark><ProblemCategory>" + strPTCategory + "</ProblemCategory><PTDesc>" + problemTypeDescription.Trim() + "</PTDesc></ProblemType></Details>";
                }

                // ===========================
                DataTable dtTemp = (DataTable)ViewState["dtPTCategory"];
                dtPTCategory = new DataTable();
                if (dtTemp != null)
                {
                    if (dtTemp.Rows.Count > 0)
                    {
                        dtPTCategory = dtTemp;
                    }
                }
                DataSet dsPTCategory = new DataSet();
                try
                {
                  
                    using (StringReader stringReader = new StringReader(xmlValue))
                    {
                        dsPTCategory = new DataSet();
                        dsPTCategory.ReadXml(stringReader);
                    }
                }
                catch
                {
                    ShowMessage("Special characters found in remarks");
                    return;
                }

                // Check the duplication in Added ProblemType
                dtPTCategory = dsPTCategory.Tables[0];

                if (dtPTCategory != null)
                {
                    if(dtPTCategory.Rows.Count > 0)
                    {
                        // =======================
                        // GetPTSConfiguration
                        string strPTypeHeader = string.Empty;
                        DataTable dtPTypeHeader = new DataTable();
                        dtPTypeHeader = ptsQueryService.GetPTSConfiguration(Convert.ToInt32(stage), CustomerDropDown.SelectedValue.ToString().Trim());
                        if (dtPTypeHeader != null)
                            if (dtPTypeHeader.Rows.Count > 0)
                                strPTypeHeader = dtPTypeHeader.Rows[0]["PTypeHeader"].ToString().Trim();
                        // =======================


                        if ((dtPTCategory.Columns["ProblemTypeCode"].Table.Select("ProblemTypeId=" + ProblemTypeDropDown.SelectedValue)).Length > 1)
                        {
                            ShowMessage("Selected problemtype has already added.");
                            return;
                        }
                        else
                        {
                            ViewState["dtPTCategory"] = dtPTCategory;
                            gvwPTCategory.DataSource = dtPTCategory;
                            gvwPTCategory.DataBind();

                            ViewState["xml"] = xmlValue;
                            hdnXml.Value = "NotFirstTime";
                            ndnPtsAddValidate.Value = "1";
                            #region TotalRemarks
                            dtPTCategory = (DataTable)ViewState["dtPTCategory"];
                            if (ProblemTypeDropDown.SelectedIndex > 0)
                            {
                                if (ddlOpenedRequest.Items.Count > 0)
                                {
                                    //if (ddlOpenedRequest.Items.FindByValue(ProblemTypeDropDown.SelectedValue.Trim()) != null)
                                    if (ddlOpenedRequest.Items.FindByValue(ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).Trim()) != null)
                                    {
                                        //ddlOpenedRequest.SelectedIndex = ddlOpenedRequest.Items.IndexOf(ddlOpenedRequest.Items.FindByValue(ProblemTypeDropDown.SelectedValue.Trim()));
                                        ddlOpenedRequest.SelectedIndex = ddlOpenedRequest.Items.IndexOf(ddlOpenedRequest.Items.FindByValue(ProblemTypeDropDown.SelectedItem.Text.Substring(0, (ProblemTypeDropDown.SelectedItem.Text.Trim().IndexOf(" --"))).Trim()));
                                        PTDescriptionTextBox.Text = ddlOpenedRequest.SelectedItem.Text.Trim();
                                    }
                                    else
                                    {
                                        if (PTDescriptionTextBox.Text.Trim() != "")
                                        {
                                            totalRemarks = PTDescriptionTextBox.Text.Trim();
                                            if (ddlProblemTypeCategory.SelectedIndex > 0)
                                            {
                                                if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                        totalRemarks += "\r\n\r\n" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                                else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                                {
                                                    if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + " )" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + "( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                    else
                                                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + " )" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                                }
                                                else
                                                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + " )" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                            }
                                            else
                                            {
                                                if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                        totalRemarks += "\r\n\r\n" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                                else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                                {
                                                    if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") " + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + "( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                    else
                                                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") " + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                                }
                                                else
                                                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") " + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                            }
                                        }
                                        else
                                        {
                                            if (ddlProblemTypeCategory.SelectedIndex > 0)
                                            {
                                                if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                    totalRemarks += dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim();
                                                else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                                {
                                                    if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + " ( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                    else
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                                }
                                                else
                                                {
                                                    if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + //" ( " + 
                                                            txtPTCategoryRemark.Text.Trim(); //+ " )";
                                                    else
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                                }
                                            }
                                            else
                                            {
                                                if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                {
                                                        totalRemarks += dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim()//+ " ( " + 
                                                   + txtPTCategoryRemark.Text.Trim();// " )";
                                                }
                                                else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                                {
                                                    if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + " ( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                    else
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                                }
                                                else
                                                {
                                                    if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + // " ( " + 
                                                            txtPTCategoryRemark.Text.Trim() ;//+ " )";
                                                    else
                                                        totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                                }

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (PTDescriptionTextBox.Text.Trim() != "")
                                    {
                                        totalRemarks = PTDescriptionTextBox.Text.Trim();
                                        if (ddlProblemTypeCategory.SelectedIndex > 0)
                                        {
                                            if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                    totalRemarks += "\r\n\r\n" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                            else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                            {
                                                if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                    totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + " )" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + "( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                else
                                                    totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + " )" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                            }
                                            else
                                                    totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + " )" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                        }
                                        else
                                        {
                                            if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                    totalRemarks += "\r\n\r\n" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim()  + txtPTCategoryRemark.Text.Trim()   ;
                                            else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                            {
                                                if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                    totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") " + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + "( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                else
                                                    totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") " + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                            }
                                            else
                                                    totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") " + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim()  + txtPTCategoryRemark.Text.Trim() ;
                                        }
                                    }
                                    else
                                    {
                                        if (ddlProblemTypeCategory.SelectedIndex > 0)
                                        {
                                            if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                    totalRemarks += dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim() ;
                                            else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                            {
                                                if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                    totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + " ( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                else
                                                    totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                            }
                                            else
                                                    totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ") ( " + ddlProblemTypeCategory.SelectedItem.Text.Trim() + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + txtPTCategoryRemark.Text.Trim();
                                        }
                                        else
                                        {
                                            if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                                                    totalRemarks += dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim()  + txtPTCategoryRemark.Text.Trim() ;
                                            else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                                            {
                                                if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                    totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim() + " ( " + txtPTCategoryRemark.Text.Trim() + " )";
                                                else
                                                    totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                            }
                                            else
                                            {
                                                //if (txtPTCategoryRemark.Text.Trim() != string.Empty)
                                                    totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim()
                                                        //+ " ( " 
                                                        + txtPTCategoryRemark.Text.Trim();// +" )";
                                                //else
                                                    //totalRemarks += DateTime.Now.ToString("dd-MMM-yyyy") + "\r\n" + " (" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["ProblemTypeCode"].ToString().Trim() + ")" + dtPTCategory.Rows[gvwPTCategory.Rows.Count - 1]["PTDesc"].ToString().Trim();
                                            }


                                        }
                                    }
                                }
                            }
                            else
                            {
                                PTDescriptionTextBox.Text = string.Empty;
                            }
                            PTDescriptionTextBox.Text = totalRemarks;
                            #endregion TotalRemarks
                            txtPTCategoryRemark.Text = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }


        }
        protected void gvwPTCategory_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                // =======================
                // GetPTSConfiguration

                if (StageDropDown.SelectedIndex > 0)
                    stage = StageDropDown.SelectedValue;
                else
                    stage = string.Empty;
                int iStage = 0;
                int.TryParse(stage, out iStage);
                string strPTypeHeader = string.Empty;
                DataTable dtPTypeHeader = new DataTable();
                dtPTypeHeader = ptsQueryService.GetPTSConfiguration(iStage, CustomerDropDown.SelectedValue.ToString().Trim());
                if (dtPTypeHeader != null)
                    if (dtPTypeHeader.Rows.Count > 0)
                        strPTypeHeader = dtPTypeHeader.Rows[0]["PTypeHeader"].ToString().Trim();
                // =======================
                //if(strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")

                int rowIndex = e.RowIndex;
                string ProblemTypeCode = string.Empty;
                DataTable dtPTCategory = (DataTable)ViewState["dtPTCategory"];
                if (dtPTCategory.Rows.Count == 1)
                {
                    hdnXml.Value = "FirstTime";
                }

                ProblemTypeCode = dtPTCategory.Rows[rowIndex]["ProblemTypeCode"].ToString().Trim();

                dtPTCategory.Rows[rowIndex].Delete();
                dtPTCategory.AcceptChanges();
                gvwPTCategory.DataSource = dtPTCategory;
                gvwPTCategory.DataBind();

                string xmlString = string.Empty;
                string xml = string.Empty;
                using (TextWriter writer = new StringWriter())
                {
                    dtPTCategory.WriteXml(writer);
                    xml = writer.ToString();
                }
                xml = xml.Replace("DocumentElement", "Details");
                ViewState["xml"] = xml;

                string totalRemarks = string.Empty;
                for (int i = 0; i < dtPTCategory.Rows.Count; i++)
                {
                    // RMT#9677

                    if (strPTypeHeader.ToLower() == "false" || strPTypeHeader == "0")
                        totalRemarks += "\r\n\r\n" + dtPTCategory.Rows[i]["PTDesc"].ToString().Trim() + "( " + dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim() + " )";                        
                    else if (strPTypeHeader.ToLower() == "true" || strPTypeHeader == "1")
                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[i]["ProblemTypeCode"].ToString().Trim() + " ) " + dtPTCategory.Rows[i]["PTDesc"].ToString().Trim() + "( " + dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim() + " )";
                    else
                        totalRemarks += "\r\n" + " (" + dtPTCategory.Rows[i]["ProblemTypeCode"].ToString().Trim() + " ) " + dtPTCategory.Rows[i]["PTDesc"].ToString().Trim() + "( " + dtPTCategory.Rows[i]["PTCategoryRemark"].ToString().Trim() + " )";
                    //if(totalRemarks != "")
                    //    totalRemarks = DateTime.Now.ToString("dd-MMM-yyyy") + totalRemarks; 
                }
                PTDescriptionTextBox.Text = totalRemarks;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        private void DisablePTCategory()
        {
            try
            {
                if (SaveButton.Enabled == false && CloseButton.Enabled == false)
                {
                    btnPTypeAdd.Enabled = false;
                    if (gvwPTCategory.Visible == true)
                    {
                        if (gvwPTCategory.Rows.Count > 0)
                        {
                            for (int i = 0; i < gvwPTCategory.Rows.Count; i++)
                            {
                                LinkButton lbDelete = (LinkButton)gvwPTCategory.Rows[i].Cells[6].Controls[0];
                                lbDelete.Visible = false;
                            }
                        }
                    }
                }
                else
                {
                    btnPTypeAdd.Enabled = true;
                    if (gvwPTCategory.Visible == true)
                    {
                        if (gvwPTCategory.Rows.Count > 0)
                        {
                            for (int i = 0; i < gvwPTCategory.Rows.Count; i++)
                            {
                                LinkButton lbDelete = (LinkButton)gvwPTCategory.Rows[i].Cells[6].Controls[0];
                                lbDelete.Visible = true;
                            }
                        }
                    }
                }
            }
            catch { throw; }
        }
        #endregion PType
    }
}
