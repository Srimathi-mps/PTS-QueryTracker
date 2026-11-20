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

namespace PTS
{
    public partial class RaiseProblems : System.Web.UI.Page
    {
        #region Fields
        IPTSQuery ptsQueryService = new PTSQueryService();
        BizPTSQuery objBizPTSQuery = new BizPTSQuery();
        PTSProblem objPTSProblem = new PTSProblem();
        //bool raiseQuery = true;

        public string CustCode { get; set; }
        public string JourCode { get; set; }
        public string ArtCode { get; set; }

        int customerCode;
        int journalCode;
        string article = string.Empty;
        string stage = string.Empty;
        string volume = string.Empty;
        string issue = string.Empty;
        int stageID = 0;

        string jobType = string.Empty;
        int deptCode = 0;

        //string SubForLog = string.Empty;
        //string MsgBodyForLog = string.Empty;
        //int i;
        //protected string jobType = string.Empty;
        string articleType = ConfigurationManager.AppSettings["ArticleType"].ToString().Trim();
        string issueType = ConfigurationManager.AppSettings["IssueType"].ToString().Trim();
        public string SmtpAddr = System.Configuration.ConfigurationSettings.AppSettings.Get("SMTPAddr");
        string xmlValue = string.Empty;
        protected int roleID;

//        protected string userName = Global.Validate(); Changed By sathish

        protected string userName = "MAC000289";
        //protected string userName = "cb1790"; // "mac002954" , "cb1790"

        List<string> lstPType = new List<string>();
        //List<string> lstPTypeCurrent = new List<string>();

        #endregion Fields

        #region PageEvents
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["userName"] = userName;

            try
            {
                if (!Page.IsPostBack)
                {
                    clearAllControls();

                    Session["userName"] = userName;
                    //if (Session["ResultMsg"] != null)
                    //    if (Session["ResultMsg"].ToString().Trim() != string.Empty)
                    //        ShowMessage(Session["ResultMsg"].ToString().Trim());

                    Session["userName"] = userName;
                    roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");
                    if (roleID <= 0)
                    {
                        MessageLabel.Text = "User role not defined";
                        System.Web.HttpContext.Current.Response.Redirect("BlankPage.aspx");
                        return;
                    }

                    FillCustomerDDL();

                    if (Session["Journal"] != null)
                    {
                        if (Session["Journal"].ToString() != string.Empty)
                        {
                            CustomerDropDown.SelectedIndex = CustomerDropDown.Items.IndexOf(CustomerDropDown.Items.FindByValue(Session["Customer"].ToString().Trim()));

                            FillJournalDDL(Convert.ToInt32(CustomerDropDown.SelectedValue));
                            JournalDropDown.SelectedIndex = JournalDropDown.Items.IndexOf(JournalDropDown.Items.FindByValue(Session["Journal"].ToString().Trim()));

                            FillArticleDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), "0", "0");
                            ArticleDropDown.SelectedIndex = ArticleDropDown.Items.IndexOf(ArticleDropDown.Items.FindByText(Session["Article"].ToString().Trim()));

                            if (ArticleDropDown.SelectedIndex > 0)
                            {
                                FillStageDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedValue);
                                StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByValue(Session["Stage"].ToString().Trim()));
                            }

                            //if (StageDropDown.SelectedIndex > 0)
                            //{
                            //FillVolumeDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), StageDropDown.SelectedValue.ToString());
                            FillVolumeDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), "0");
                            VolumeDropDown.SelectedIndex = VolumeDropDown.Items.IndexOf(VolumeDropDown.Items.FindByValue(Session["Volume"].ToString().Trim()));
                            //}
                            FillIssueDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), VolumeDropDown.SelectedValue.ToString());
                            IssueDropDown.SelectedIndex = IssueDropDown.Items.IndexOf(IssueDropDown.Items.FindByValue(Session["Issue"].ToString().Trim()));

                            GetDetails();
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

                    //if (Request.QueryString["barCode"] != null)
                    //{
                    //    if (Request.QueryString["barCode"] != string.Empty)
                    //    {
                    //        txtBarcode.Text = Request.QueryString["barCode"].ToString().Trim();
                    //        GetDataFromBarCode();
                    //        if (Request.QueryString["OpenedRequest"] != null)
                    //        {
                    //            //ddlOpenedRequest.SelectedValue = Request.QueryString["OpenedRequest"].ToString().Trim();
                    //            //OpenedRequestSelectIndexChanged();
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message.Trim();
            }
        }
        protected void btnGoBarcode_Click(object sender, EventArgs e)
        {
            btnGoDdl.Visible = false;
            if (txtBarcode.Text.Trim() != string.Empty)
            {
                Session["BarcodeVal"] = txtBarcode.Text;

                GetDataFromBarCode();

                Session["Customer"] = CustomerDropDown.SelectedValue;
                Session["CustomerName"] = CustomerDropDown.SelectedItem.Text.Trim();
                Session["Journal"] = JournalDropDown.SelectedValue;
                Session["JournalName"] = JournalDropDown.SelectedItem.Text.Trim();
                if (ArticleDropDown.SelectedIndex > 0)
                    Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                else
                    Session["Article"] = string.Empty;

                if (VolumeDropDown.SelectedIndex > 0)
                    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                else
                    Session["Volume"] = string.Empty;

                if (IssueDropDown.SelectedIndex > 0)
                    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                else
                    Session["Issue"] = string.Empty;

                //Session["Volume"] = VolumeDropDown.SelectedValue;
                //Session["Issue"] = IssueDropDown.SelectedValue;
                Session["Stage"] = StageDropDown.SelectedValue;
                Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();

                Session["RoundCount"] = ddlRoundCount.SelectedValue;
                Session["lstPType"] = lstPType;
                //Session["lstPTypeCurrent"] = lstPTypeCurrent;
            }
            else
            {
                ShowMessage("Please enter barcode");
                #region clearControls
                //clearControl("txt", (Control)txtBarcode);
                CustomerDropDown.SelectedIndex = -1;
                clearControl("ddl", (Control)JournalDropDown);
                clearControl("ddl", (Control)ArticleDropDown);
                clearControl("ddl", (Control)VolumeDropDown);
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
                btnGoDdl.Visible = false;
                MessageLabel.Text = string.Empty;
                #endregion clearControls
            }
        }
        protected void btnRaiseProblem_Click(object sender, EventArgs e)
        {
            try
            {
                //Session["OpenedRequestId"] = gvwOpenedRequest.DataKeys[idx].Value.ToString().Trim();

                Session["Customer"] = CustomerDropDown.SelectedValue;
                Session["Journal"] = JournalDropDown.SelectedValue;
                if (ArticleDropDown.SelectedIndex > 0)
                    Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                else
                    Session["Article"] = string.Empty;

                if (VolumeDropDown.SelectedIndex > 0)
                    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                else
                    Session["Volume"] = string.Empty;

                if (IssueDropDown.SelectedIndex > 0)
                    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                else
                    Session["Issue"] = string.Empty;


                //Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                //Session["Volume"] = VolumeDropDown.SelectedValue;
                //Session["Issue"] = IssueDropDown.SelectedValue;

                Session["Stage"] = StageDropDown.SelectedValue;
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

                if (Session["dtProblemTypeDetails"] == null)
                {
                    DataTable dtProblemTypeDetails = new DataTable();
                    dtProblemTypeDetails = ptsQueryService.GetProblemTypeDetails();
                    if (dtProblemTypeDetails != null)
                        if (dtProblemTypeDetails.Rows.Count > 0)
                            Session["dtProblemTypeDetails"] = dtProblemTypeDetails;
                }
                Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + System.Web.HttpContext.Current.Request.QueryString["Username"] + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown.SelectedItem.Text.Trim());
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        // ============
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
                        clearControl("ddl", (Control)VolumeDropDown);
                        clearControl("ddl", (Control)IssueDropDown);
                        clearControl("ddl", (Control)StageDropDown);
                        clearControl("ddl", (Control)ddlRoundCount);
                        clearControl("gvw", (Control)gvwOpenedRequest);
                        btnRaiseProblem.Visible = false;
                        btnGoDdl.Visible = false;
                        MessageLabel.Text = string.Empty;
                        #endregion clearControls

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

                        FillJournalDDL(customerCode);
                        //EnableDisablePrvRemark();
                        btnRaiseProblem.Visible = false;
                        btnGoDdl.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        ShowMessage(ex.Message);
                    }
                }
                else
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)JournalDropDown);
                    clearControl("ddl", (Control)ArticleDropDown);
                    clearControl("ddl", (Control)VolumeDropDown);
                    clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
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
                    clearControl("ddl", (Control)VolumeDropDown);
                    clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls

                    int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                    int.TryParse(JournalDropDown.SelectedValue.ToString().Trim(), out journalCode);
                    if (StageDropDown.SelectedIndex > 0)
                        SubStageID = Convert.ToInt32(StageDropDown.SelectedValue.ToString().Trim());
                    jobType = JobTypeHidden.Value;

                    isMasterCopyJournal = ptsQueryService.IsMasterCopyJournal(journalCode);

                    FillArticleDDL(customerCode, journalCode, "0", "0");
                    //FillVolumeDDL(customerCode, journalCode, SubStageID.ToString());
                    FillVolumeDDL(customerCode, journalCode, "0");
                    FillIssueDDL(customerCode, journalCode, "0");
                    MasterCopyJournalLabel.Visible = isMasterCopyJournal;

                    //ProblemTypeDropDown.SelectedIndex = -1;
                    //PTDescriptionTextBox.Text = string.Empty;
                    //GVProblemHistory.DataSource = null;
                    //GVProblemHistory.DataBind();
                    //EnableDisablePrvRemark();

                    DataTable dt = new DataTable();
                    StageDropDown.DataSource = dt;
                    StageDropDown.DataBind();
                    StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                    //ProblemTypeDropDown.DataSource = dt;
                    //ProblemTypeDropDown.DataBind();
                    //ProblemTypeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = false;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
            else
            {
                #region clearControls
                clearControl("txt", (Control)txtBarcode);
                //CustomerDropDown.SelectedIndex = -1;
                //clearControl("ddl", (Control)JournalDropDown);
                clearControl("ddl", (Control)ArticleDropDown);
                clearControl("ddl", (Control)VolumeDropDown);
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
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
                    VolumeDropDown.SelectedIndex = -1;
                    IssueDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
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

                    //GetDetails();
                    //DisablePTCategory();

                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = true;
                }
                else
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    //clearControl("ddl", (Control)ArticleDropDown);
                    VolumeDropDown.SelectedIndex = -1;
                    IssueDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        protected void VolumeDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (VolumeDropDown.SelectedIndex > 0)
            {
                #region clearControls
                clearControl("txt", (Control)txtBarcode);
                //CustomerDropDown.SelectedIndex = -1;
                //clearControl("ddl", (Control)JournalDropDown);
                ArticleDropDown.SelectedIndex = -1;
                //VolumeDropDown.SelectedIndex = -1;
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
                btnGoDdl.Visible = false;
                MessageLabel.Text = string.Empty;
                #endregion clearControls
                
                volume = string.Empty;
                issue = string.Empty;
                string deptId = string.Empty;
                JobTypeHidden.Value = "2";
                
                try
                {
                    DataTable dtOpenedRequest = new DataTable();
                    ViewState["dtOpenedRequest"] = dtOpenedRequest;
                    //gvwOpenedRequest.DataSource = dtOpenedRequest;
                    //gvwOpenedRequest.DataBind();
                    ViewState["xml"] = string.Empty;
                    hdnXml.Value = "FirstTime";
                    xmlValue = string.Empty;

                    int.TryParse(CustomerDropDown.SelectedValue.ToString(), out customerCode);
                    int.TryParse(JournalDropDown.SelectedValue.ToString(), out journalCode);
                    volume = VolumeDropDown.SelectedValue.Trim();
                    FillIssueDDL(customerCode, journalCode, volume);

                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = false;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
            else
            {
                #region clearControls
                clearControl("txt", (Control)txtBarcode);
                //CustomerDropDown.SelectedIndex = -1;
                //clearControl("ddl", (Control)JournalDropDown);
                ArticleDropDown.SelectedIndex = -1;
                //VolumeDropDown.SelectedIndex = -1;
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
                btnGoDdl.Visible = false;
                MessageLabel.Text = string.Empty;
                #endregion clearControls
            }
        }
        protected void IssueDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IssueDropDown.SelectedIndex > 0)
            {
                try
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    ArticleDropDown.SelectedIndex = -1;
                    //VolumeDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = true;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls

                    DataTable dtOpenedRequest = new DataTable();
                    ViewState["dtOpenedRequest"] = dtOpenedRequest;
                    //gvwOpenedRequest.DataSource = dtOpenedRequest;
                    //gvwOpenedRequest.DataBind();
                    ViewState["xml"] = string.Empty;
                    hdnXml.Value = "FirstTime";
                    xmlValue = string.Empty;
                }
                catch (Exception ex)
                {
                    ShowMessage(ex.Message);
                }
            }
            else
            {
                #region clearControls
                clearControl("txt", (Control)txtBarcode);
                //CustomerDropDown.SelectedIndex = -1;
                //clearControl("ddl", (Control)JournalDropDown);
                ArticleDropDown.SelectedIndex = -1;
                //VolumeDropDown.SelectedIndex = -1;
                //clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnRaiseProblem.Visible = false;
                btnGoDdl.Visible = false;
                MessageLabel.Text = string.Empty;
                #endregion clearControls
            }
        }
        protected void btnGoDdl_Click(object sender, EventArgs e)
        {
            try
            {
                if ((CustomerDropDown.SelectedIndex > 0 && JournalDropDown.SelectedIndex > 0 && ArticleDropDown.SelectedIndex > 0) || (CustomerDropDown.SelectedIndex > 0 && JournalDropDown.SelectedIndex > 0 && VolumeDropDown.SelectedIndex > 0 && IssueDropDown.SelectedIndex > 0))
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

                    if (ArticleDropDown.SelectedIndex > 0)
                        Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                    else
                        Session["Article"] = string.Empty;

                    if (VolumeDropDown.SelectedIndex > 0)
                        Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                    else
                        Session["Volume"] = string.Empty;

                    if (IssueDropDown.SelectedIndex > 0)
                        Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                    else
                        Session["Issue"] = string.Empty;

                    Session["Stage"] = StageDropDown.SelectedValue;
                    Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();

                    Session["RoundCount"] = ddlRoundCount.SelectedValue;
                    Session["lstPType"] = lstPType;
                    //Session["lstPTypeCurrent"] = lstPTypeCurrent;
                }
                else
                {
                    #region clearControls
                    clearControl("txt", (Control)txtBarcode);
                    //CustomerDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)JournalDropDown);
                    ArticleDropDown.SelectedIndex = -1;
                    //VolumeDropDown.SelectedIndex = -1;
                    //clearControl("ddl", (Control)IssueDropDown);
                    clearControl("ddl", (Control)StageDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                    clearControl("gvw", (Control)gvwOpenedRequest);
                    btnRaiseProblem.Visible = false;
                    btnGoDdl.Visible = false;
                    MessageLabel.Text = string.Empty;
                    #endregion clearControls
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        // ============
        protected void gvwOpenedRequest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    System.Drawing.Color colour = System.Drawing.ColorTranslator.FromHtml("#E7EFF2");
                    if (e.Row.Cells[6].Text.ToString().Trim().ToLower() == "closed")
                    {
                        colour = System.Drawing.ColorTranslator.FromHtml("#FFA500");
                        e.Row.BackColor = colour;
                    }
                    else if (e.Row.Cells[6].Text.ToString().Trim().ToLower() == "resolved")
                    {
                        colour = System.Drawing.ColorTranslator.FromHtml("#90EE90");
                        e.Row.BackColor = colour;
                        //e.Row.Cells[8].Controls[0].Visible = false;
                    }
                    else if (e.Row.Cells[6].Text.ToString().Trim().ToLower() == "saved")
                    {
                        ImageButton imgbtnEdit = (ImageButton)e.Row.FindControl("imgbtnEdit");
                        //Label lblCloseStatus = (Label)e.Row.FindControl("lblCloseStatus");
                        string AllowNewQry = Session["AllowNewQry"].ToString().Trim();
                        if (AllowNewQry == "false")
                        {
                            imgbtnEdit.Enabled = false;
                            //lblCloseStatus.Text = "1";
                            imgbtnEdit.ToolTip = "Please resolve the closed query.";
                        }
                        else
                        {
                            imgbtnEdit.Enabled = true;
                            //lblCloseStatus.Text = "0";
                        }
                    }
                    // ============================================
                    //e.Row.Cells[5].Text = e.Row.Cells[5].Text.Replace("<br />", "\r\n");
                    string temp = e.Row.Cells[5].Text.Trim();
                    e.Row.Cells[5].Text = temp.Replace("\r\n", "<br />"); // e.Row.Cells[5].Text.Replace("\r\n", "<br />");
                    // ============================================
                    if (e.Row.Cells[6].Text.ToLower() != "resolved")
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
                ShowMessage(ex.Message);
            }
        }
        protected void gvwOpenedRequest_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                int idx = e.NewEditIndex;

                // ========================
                Session["Customer"] = CustomerDropDown.SelectedValue;
                Session["Journal"] = JournalDropDown.SelectedValue;

                if (ArticleDropDown.SelectedIndex > 0)
                    Session["Article"] = ArticleDropDown.SelectedItem.Text.Trim();
                else
                    Session["Article"] = string.Empty;

                if (VolumeDropDown.SelectedIndex > 0)
                    Session["Volume"] = VolumeDropDown.SelectedItem.Text.Trim();
                else
                    Session["Volume"] = string.Empty;

                if (IssueDropDown.SelectedIndex > 0)
                    Session["Issue"] = IssueDropDown.SelectedItem.Text.Trim();
                else
                    Session["Issue"] = string.Empty;

                Session["Stage"] = StageDropDown.SelectedValue;
                Session["StageName"] = StageDropDown.SelectedItem.Text.Trim();
                Session["IsOpenedQuery"] = 0;
                Session["RoundCount"] = ddlRoundCount.SelectedValue;
                // ========================


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
                    dtProblemTypeDetails = ptsQueryService.GetProblemTypeDetails();
                    if (dtProblemTypeDetails != null)
                        if (dtProblemTypeDetails.Rows.Count > 0)
                            Session["dtProblemTypeDetails"] = dtProblemTypeDetails;
                }

                // ========================================
                DataTable dtProblems = new DataTable();
                dtProblems = ptsQueryService.GetPTSProblemsByID(Convert.ToInt32(gvwOpenedRequest.DataKeys[idx]["PTId"].ToString().Trim()));
                DataRow[] dr = dtProblems.Select();
                if (dr.Length > 0)
                {
                    string temp = dr[0]["ZipFilePath"].ToString().Trim();
                    temp = temp.Replace("\\", "\\\\");
                    Session["ZipFilePath"] = temp;
                }

                #region junkcode
                //    if (dr[0]["ZipFilePath"].ToString().Trim() != string.Empty)
                //    {
                //        if (File.Exists(dr[0]["ZipFilePath"].ToString().Trim()))
                //        {
                //            string zipFileName = dr[0]["ZipFilePath"].ToString().Trim();
                //            lbtnZipFile.Visible = true;
                //            lbtnZipFile.Text = "(Download)";
                //            lblZipFile.Text = dr[0]["ZipFilePath"].ToString().Trim();
                //            Session["zipFile"] = zipFileName;
                //        }
                //        else
                //        {
                //            lbtnZipFile.Visible = false;
                //            lbtnZipFile.Text = string.Empty;
                //            lblZipFile.Text = string.Empty;
                //        }
                //    }
                //    else
                //    {
                //        lbtnZipFile.Text = string.Empty;
                //        lblZipFile.Text = string.Empty;
                //    }
                //    deptId = dr[0]["departmentId"].ToString().Trim();
                //    ddlDepartment.SelectedValue = deptId;
                //    stage = dr[0]["SUBSTAGEID"].ToString().Trim();
                //    PTDescriptionTextBox.Text = dr[0]["PTSRemarks"].ToString().Trim();
                //    int CloseStatus = 0;
                //    if (Convert.ToBoolean(dr[0]["TaskClosed"].ToString()))
                //        CloseStatus = 1;

                //    DataTable dtProblemDetails = ptsQueryService.GetProblemDetails(Convert.ToInt32(dr[0]["Id"].ToString().Trim()));
                //    gvwPTCategory.DataSource = dtProblemDetails;
                //    gvwPTCategory.DataBind();
                //    DataTable dtPTCategory = dtProblemDetails;
                //    ViewState["dtPTCategory"] = dtPTCategory;
                //    dtPTCategory.TableName = "ProblemType";
                //    string xmlString = string.Empty;
                //    string xml = string.Empty;
                //    using (TextWriter writer = new StringWriter())
                //    {
                //        dtPTCategory.WriteXml(writer);
                //        xml = writer.ToString();
                //    }
                //    xml = xml.Replace("DocumentElement", "Details");
                //    ViewState["xml"] = xml;


                //    hdnReviewerName.Value = dr[0]["QuerySavedBy"].ToString();
                //    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), CloseStatus);
                //}
                //else
                //{
                //    ddlDepartment.SelectedIndex = 0;
                //    hdnReviewerName.Value = userName;
                //    GetCloseStatusNew(ptsProblem.CustomerCode.ToString(), 0);
                //}

                //=========================================
                #endregion junkcode

                Response.Redirect("RaiseProblemsSubmission.aspx?Username=" + System.Web.HttpContext.Current.Request.QueryString["Username"] + "&CustName=" + CustomerDropDown.SelectedItem.Text.Trim() + "&JourName=" + JournalDropDown.SelectedItem.Text.Trim() + "&StageName=" + StageDropDown.SelectedItem.Text.Trim());
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        #endregion PageEvents

        #region PrivateMethods

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
            //ddlRoundCount.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_STRVAL));

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

        #region Barcode
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
                                                ShowMessage("The article entered does not exist");
                                                //ClearFields();
                                                return;
                                            }
                                            else
                                            {
                                                btnRaiseProblem.Visible = true;
                                                FillArticleDDL(customerCode, journalCode, "0", "0");
                                                ArticleDropDown.SelectedIndex = ArticleDropDown.Items.IndexOf(ArticleDropDown.Items.FindByText(articleID.ToUpper()));
                                            }

                                            GetDetails();
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
                                        FillVolumeDDL(customerCode, journalCode, "0");
                                        ////FillVolumeDDL(customerCode, journalCode,Session["Stage"].ToString().Trim());

                                        if (VolumeDropDown.Items.FindByValue(volume.ToString()) != null)
                                        {
                                            VolumeDropDown.SelectedValue = volume;
                                            ////FillIssueDDL(customerCode, journalCode, Session["Stage"].ToString().Trim());

                                            FillIssueDDL(customerCode, journalCode, volume);
                                            if (IssueDropDown.Items.FindByValue(issue.ToString()) != null)
                                            {
                                                IssueDropDown.SelectedValue = issue;

                                                ////btnRaiseProblem.Visible = true;
                                                if (VolumeDropDown.SelectedIndex > 0 && IssueDropDown.SelectedIndex > 0)
                                                    GetDetails();
                                            }
                                        }
                                    }
                                }
                            }
                            if (CustomerDropDown.SelectedIndex <= 0)
                            {
                                CustomerDropDown.SelectedIndex = -1;
                                clearControl("ddl", (Control)JournalDropDown);
                                clearControl("ddl", (Control)ArticleDropDown);
                                clearControl("ddl", (Control)VolumeDropDown);
                                clearControl("ddl", (Control)IssueDropDown);
                                clearControl("ddl", (Control)StageDropDown);
                                clearControl("ddl", (Control)ddlRoundCount);

                                btnRaiseProblem.Visible = false;
                                ShowMessage("The customer/journal entered does not exist");
                                return;
                            }
                            if (VolumeDropDown.Items.IndexOf(VolumeDropDown.Items.FindByText(volume)) < 0)
                            {
                                CustomerDropDown.SelectedIndex = -1;
                                clearControl("ddl", (Control)JournalDropDown);
                                clearControl("ddl", (Control)ArticleDropDown);
                                clearControl("ddl", (Control)VolumeDropDown);
                                clearControl("ddl", (Control)IssueDropDown);
                                clearControl("ddl", (Control)StageDropDown);
                                clearControl("ddl", (Control)ddlRoundCount);


                                btnRaiseProblem.Visible = false;
                                ShowMessage("The volume entered does not exist");
                                return;
                            }
                            else if (IssueDropDown.Items.IndexOf(IssueDropDown.Items.FindByText(issue)) < 0)
                            {
                                CustomerDropDown.SelectedIndex = -1;
                                clearControl("ddl", (Control)JournalDropDown);
                                clearControl("ddl", (Control)ArticleDropDown);
                                clearControl("ddl", (Control)VolumeDropDown);
                                clearControl("ddl", (Control)IssueDropDown);
                                clearControl("ddl", (Control)StageDropDown);
                                clearControl("ddl", (Control)ddlRoundCount);

                                btnRaiseProblem.Visible = false;
                                ShowMessage("The issue entered does not exist");
                                return;
                            }
                            VolumeDropDown.SelectedValue = volume;
                            IssueDropDown.SelectedValue = issue;
                            FillArticleDDL(customerCode, journalCode, volume, issue);
                            btnRaiseProblem.Visible = true;
                        }
                        else
                        {
                            btnRaiseProblem.Visible = false;
                            ShowMessage("Invalid barcode");
                            JobTypeHidden.Value = string.Empty;
                            Session["jobType"] = JobTypeHidden.Value.Trim();

                            clearControl("ddl", JournalDropDown);
                            clearControl("ddl", ArticleDropDown);
                            clearControl("ddl", StageDropDown);
                            clearControl("ddl", VolumeDropDown);
                            clearControl("ddl", IssueDropDown);
                            clearControl("ddl", ddlRoundCount);

                            return;
                        }
                        //GetDetails();
                    }
                    catch (Exception ex)
                    {

                        ShowMessage(ex.Message);
                    }
                }
                else
                {
                    btnRaiseProblem.Visible = false;
                    CustomerDropDown.SelectedIndex = -1;
                    clearControl("ddl", (Control)JournalDropDown);
                    clearControl("ddl", (Control)ArticleDropDown);
                    clearControl("ddl", (Control)ddlRoundCount);
                }
            }
        }
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

            if (ArticleDropDown.SelectedIndex > 0)
            {
                article = ArticleDropDown.SelectedItem.Text.Trim();
                stage = ptsQueryService.GetStage(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim(), out volume, out issue);

                DataTable dtProblems = new DataTable();
                FillStageDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim());
                StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByText(stage.Trim()));
                int.TryParse(StageDropDown.SelectedValue.ToString().Trim(), out stageID);
                
                //VolumeDropDown.Items.Clear();
                //VolumeDropDown.Items.Add(new ListItem(volume, volume));
                //VolumeDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
                
                //IssueDropDown.Items.Clear();
                //IssueDropDown.Items.Add(new ListItem(issue, issue));
                //IssueDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

                FillRoundCountDDL(Convert.ToInt32(CustomerDropDown.SelectedValue), Convert.ToInt32(JournalDropDown.SelectedValue), ArticleDropDown.SelectedItem.Text.Trim());
                
                if (VolumeDropDown.SelectedIndex > 0)
                    volume = VolumeDropDown.SelectedItem.Text.Trim();
                else
                    volume = string.Empty;
                if (IssueDropDown.SelectedIndex > 0)
                    issue = IssueDropDown.SelectedItem.Text.Trim();
                else
                    issue = string.Empty;

                
                objPTSProblem.Article = article.Trim();
                objPTSProblem.Volume = volume.Trim();
                objPTSProblem.Issue = issue.Trim();
                objPTSProblem.StageID = stageID;
                BindOpenedRequests(objPTSProblem); // now

                //btnRaiseProblem.Visible = true;
            }
            else if (VolumeDropDown.SelectedValue != "0" && IssueDropDown.SelectedValue != "0" && VolumeDropDown.SelectedValue != "" && IssueDropDown.SelectedValue != "")
            {
                JobTypeHidden.Value = "2";
                Session["jobType"] = JobTypeHidden.Value.Trim();
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
                BindOpenedRequests(objPTSProblem);

                //btnRaiseProblem.Visible = true;
            }
            else
            {
                //clearControl("gvw", (Control)GVProblemHistory);
                btnRaiseProblem.Visible = false;
                CustomerDropDown.SelectedIndex = -1;
                JournalDropDown.SelectedIndex = -1;
                ArticleDropDown.SelectedIndex = -1;
                ddlRoundCount.SelectedIndex = -1;
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
                dtOpenedRequest = ptsQueryService.GetOpenedRequestsNew(objPTSProblem);
                Session["OpenedRequest"] = dtOpenedRequest;
                // ========================
                DataView dv = new DataView(dtOpenedRequest);
                //DataView dv = dtOpenedRequest.DefaultView;
                dv.RowFilter = "AllowNewQry = FALSE";
                if (dv.ToTable().Rows.Count > 0)
                {
                    Session["AllowNewQry"] = "false";
                    GetCloseStatusNew(CustomerDropDown.SelectedValue, 1);
                }
                else
                {
                    Session["AllowNewQry"] = "true";
                    btnRaiseProblem.Visible = true;
                    btnRaiseProblem.Enabled = true;
                    btnRaiseProblem.ToolTip = string.Empty;
                }
                // ========================
                if (dtOpenedRequest != null)
                {
                    gvwOpenedRequest.DataSource = dtOpenedRequest;
                    gvwOpenedRequest.DataBind();
                }
            }
            catch { throw; }
        }

        private void GetCloseStatusNew(string strCust, int CloseStatus)
        {
            roleID = ptsQueryService.GetRoleIDByUserName(userName, strCust);
            if (CloseStatus == 1)
            {
                btnRaiseProblem.Visible = true;
                btnRaiseProblem.Enabled = false;
                btnRaiseProblem.ToolTip = "Please resolve the closed query.";
            }
            else if (CloseStatus == 0)
            {
                btnRaiseProblem.Visible = true;
                btnRaiseProblem.Enabled = false;
                btnRaiseProblem.ToolTip = "Please resolve the closed query.";
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
                    btnRaiseProblem.ToolTip = "Please resolve the closed query.";
                }
            }
        }

        #endregion Barcode

        #region etcMethods
        private void ShowMessage(string Message)
        {
            ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), Guid.NewGuid().ToString(), "alert('" + GeneralHelper.RemoveSpecialChars(Message) + "');", true);
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
                clearControl("ddl", (Control)VolumeDropDown);
                clearControl("ddl", (Control)IssueDropDown);
                clearControl("ddl", (Control)StageDropDown);
                clearControl("ddl", (Control)ddlRoundCount);
                clearControl("gvw", (Control)gvwOpenedRequest);
                btnGoDdl.Visible = false;
                btnRaiseProblem.Visible = false;
                #endregion clearControls
            }
            catch { throw; }
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
        #endregion etcMethods

        #endregion PrivateMethods

        #region Sorting
        string sortExp
        {
            get
            {
                object oSortExp = ViewState["sortExp"];
                if (oSortExp == null)
                    return string.Empty;
                else
                    return oSortExp.ToString();
            }
            set
            {
                ViewState["sortExp"] = value;
            }
        }
        string sortDir
        {
            get
            {
                object oSortDir = ViewState["sortDir"];
                if (oSortDir == null)
                    return string.Empty;
                else
                    return oSortDir.ToString();
            }
            set
            {
                ViewState["sortDir"] = value;
            }
        }
        DataTable tempTable
        {
            get
            {
                object oTempTable = ViewState["tempTable"];
                if (oTempTable != null)
                {
                    DataTable dt = new DataTable();
                    return dt;
                }
                else
                    return (DataTable)oTempTable;
            }
            set
            {
                ViewState["tempTable"] = value;
            }
        }
        private void SortGrid()
        {
            try
            {
                DataTable dt = (DataTable)Session["OpenedRequest"];
                tempTable = dt;
                DataView dv = new DataView(dt);
                if (dv.Count != 0)
                {
                    if (!this.sortExp.Equals(string.Empty))
                    {
                        string strDir = string.Empty;
                        if (this.sortDir.Equals("D"))
                            strDir = "DESC";
                        else if (this.sortDir.Equals("A"))
                            strDir = "ASC";

                        dv.Sort = this.sortExp + " " + strDir;

                        Session["sortExp"] = this.sortExp;
                        Session["sortDir"] = strDir;
                    }
                    Session["OpenedRequest"] = dv.ToTable();
                    gvwOpenedRequest.DataSource = dv;
                    gvwOpenedRequest.DataBind();

                    
                }
            }
            catch { throw; }
        }
        private void SortInPreviousOrder()
        {
            try
            {
                DataTable dt = (DataTable)Session["OpenedRequest"];
                tempTable = dt;
                DataView dv = new DataView(dt);
                if (dv.Count != 0)
                {
                    if (Session["sortExp"] != null && Session["sortDir"] != null)
                    {
                        string sortExp = Session["sortExp"].ToString().Trim();
                        string sortDir = Session["sortDir"].ToString().Trim();
                        dv.Sort = sortExp + " " + sortDir;

                        gvwOpenedRequest.DataSource = dv;
                        gvwOpenedRequest.DataBind();
                    }
                }
            }
            catch { throw; }
        }
        protected void gbtnPTCodeSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.sortDir == "A")
                    this.sortDir = (this.sortDir == "A" ? "D" : "A");
                else
                    this.sortDir = "A";

                this.sortExp = "PTCode";
                SortGrid();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        protected void gbtnRemarksSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.sortDir == "A")
                    this.sortDir = (this.sortDir == "A" ? "D" : "A");
                else
                    this.sortDir = "A";

                this.sortExp = "PTSRemarks";
                SortGrid();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        protected void gbtnStatusSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.sortDir == "A")
                    this.sortDir = (this.sortDir == "A" ? "D" : "A");
                else
                    this.sortDir = "A";

                this.sortExp = "Status";
                SortGrid();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        protected void gbtnResponseSort_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.sortDir == "A")
                    this.sortDir = (this.sortDir == "A" ? "D" : "A");
                else
                    this.sortDir = "A";

                this.sortExp = "ResolvedRemarks";
                SortGrid();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        #endregion Sorting
    }
}
