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
using QueryTracker;

namespace QueryTracker
{
    public partial class FileConfiguration : System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();

        protected string userName = Global.Validate();
        //protected string userName = "MAC006081";
        protected int roleID;

        #region Page Control Events

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveButton.Attributes.Add("onclick", "return ConfigValidation();");
            UpdateButton.Attributes.Add("onclick", "return ConfigValidation();");

            try
            {
                roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");

                if (!Page.IsPostBack)
                {
                    ClearControls();




                    FillCustomerDDL();




                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void CustomerDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            int customerCode = 0;
            try
            {
                int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);

                roleID = ptsQueryService.GetRoleIDByUserName(userName, "0");
                #region Enable/Disable buttons based on role

                FillStageDDL();

                bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.SaveButtonFileConfiguration);
                if (allowSave)
                    SaveButton.Enabled = true;
                else
                    SaveButton.Enabled = false;

                bool allowUpdate = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.UpdateButtonFileConfiguration);

                if (allowUpdate)
                    UpdateButton.Enabled = true;
                else
                    UpdateButton.Enabled = false;

                BindPTSConfiguration(customerCode);

                #endregion
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            PTSConfig ptsConfig = new PTSConfig();
            //byte[] byteArr = new byte[SignalTemplateFileUpload.PostedFile.ContentLength];

            try
            {
                string fileContent = string.Empty;

                ptsConfig.CustomerGroupName = CustomerDropDown.SelectedValue.ToString().Trim();
                ptsConfig.Stage = StageDropDown.SelectedItem.Text.Trim();
                ptsConfig.StageID = int.Parse(StageDropDown.SelectedValue.ToString());
                ptsConfig.ZipFileDestionation = ZipFileDestinationTextBox.Text.Trim();
                ptsConfig.ZipFileName = ZipFileNameTextBox.Text.Trim();

                string tempXmlTemplatePath = ConfigurationManager.AppSettings["xmlTemplateTempPath"].ToString().Trim();
                ptsConfig.ResponseType = Convert.ToInt32(rdoType.SelectedValue);
                if (rdoType.SelectedIndex == 0)//Signal
                {

                    if (SignalTemplateFileUpload.PostedFile.ContentLength == 0)
                    {
                        MessageLabel.Text = "Select xml template to upload";
                        return;
                    }


                    ptsConfig.SignalFileName = SignalFileNameTextBox.Text.Trim();
                    ptsConfig.SignalFileDestination = SignalFileDestinationTextBox.Text.Trim();

                    ptsConfig.SignalTemplateFileName = Path.GetFileName(SignalTemplateFileUpload.PostedFile.FileName);

                    //FileStream fs = new FileStream(SignalTemplateFileUpload.PostedFile.FileName, FileMode.Open);
                    //fs.Read(byteArr, 0, (int)fs.Length);
                    //fs.Close();


                    string xmlTemplateName = Path.GetFileName(SignalTemplateFileUpload.PostedFile.FileName);

                    SignalTemplateFileUpload.PostedFile.SaveAs(tempXmlTemplatePath + xmlTemplateName);


                    StreamReader sr = new StreamReader(tempXmlTemplatePath + xmlTemplateName);
                    fileContent = sr.ReadToEnd();
                    sr.Close();

                    if (File.Exists(tempXmlTemplatePath + xmlTemplateName))
                    {
                        File.SetAttributes(tempXmlTemplatePath + xmlTemplateName, FileAttributes.Normal);
                        File.Delete(tempXmlTemplatePath + xmlTemplateName);
                    }
                }
                else
                {
                    ptsConfig.FromEmailId = txtFromEmail.Text.Trim();
                    ptsConfig.ToEmailId = txtToEmail.Text.Trim();
                    ptsConfig.EmailContent = txtEmailContent.Text.Trim();
                    ptsConfig.EmailSubject = txtEmailSubject.Text.Trim();
                }

                string result = ptsQueryService.InsertXmlTemplate(ptsConfig, fileContent);
                int customerCode = 0;
                int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                BindPTSConfiguration(customerCode);

                MessageLabel.Text = result;
                ClearControls();
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void GVPTSConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {

            DataTable dtConfig = new DataTable();

            try
            {
                MessageLabel.Text = string.Empty;
                if (ViewState["dtConfig"] != null)
                {
                    dtConfig = (DataTable)ViewState["dtConfig"];

                    DataRow drConfig = dtConfig.Rows[GVPTSConfiguration.SelectedIndex];

                    ConfigIDHidden.Value = GVPTSConfiguration.DataKeys[GVPTSConfiguration.SelectedIndex].Value.ToString();
                    StageDropDown.SelectedIndex = StageDropDown.Items.IndexOf(StageDropDown.Items.FindByText(drConfig["stage"].ToString()));
                    CustomerDropDown.SelectedIndex = CustomerDropDown.Items.IndexOf(CustomerDropDown.Items.FindByValue(drConfig["customer_grname"].ToString()));
                    SignalFileNameTextBox.Text = drConfig["signalfilename"].ToString();
                    SignalFileDestinationTextBox.Text = drConfig["signalfiledestination"].ToString();

                    ZipFileNameTextBox.Text = drConfig["zipfilename"].ToString();
                    ZipFileDestinationTextBox.Text = drConfig["zipfiledestination"].ToString();

                    txtFromEmail.Text = drConfig["FromEmailId"].ToString();
                    txtToEmail.Text = drConfig["ToEmailId"].ToString();
                    txtEmailContent.Text = drConfig["EmailContent"].ToString();
                    txtEmailSubject.Text = drConfig["EmailSubject"].ToString();
                    rdoType.SelectedValue = drConfig["ResponseTypeValue"].ToString();
                    if (rdoType.SelectedValue == "0")
                    {
                        pnlSignal.Visible = true;
                        pnlEmail.Visible = false;
                    }
                    else
                    {
                        pnlSignal.Visible = false;
                        pnlEmail.Visible = true;
                    }
                    //UpdateButton.Enabled = true;
                    SaveButton.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }

        }

        protected void rdoType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rdoType.SelectedIndex == 0)
            {
                pnlSignal.Visible = true;
                pnlEmail.Visible = false;
            }
            else
            {
                pnlEmail.Visible = true;
                pnlSignal.Visible = false;
            }
        }
        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            PTSConfig ptsConfig = new PTSConfig();

            try
            {

                ptsConfig.ConfigurationID = int.Parse(ConfigIDHidden.Value);

                ptsConfig.Stage = StageDropDown.SelectedItem.Text.Trim();
                ptsConfig.StageID = int.Parse(StageDropDown.SelectedValue.ToString());
                ptsConfig.CustomerGroupName = CustomerDropDown.SelectedValue.ToString().Trim();
                ptsConfig.ZipFileDestionation = ZipFileDestinationTextBox.Text.Trim();
                ptsConfig.ZipFileName = ZipFileNameTextBox.Text.Trim();
                ptsConfig.ResponseType = Convert.ToInt32(rdoType.SelectedValue);
                string fileContent = string.Empty;

                if (rdoType.SelectedIndex == 0)//Signal
                {
                    if (SignalTemplateFileUpload.PostedFile.ContentLength == 0)
                    {
                        MessageLabel.Text = "Select xml template to upload";
                        return;
                    }


                    ptsConfig.SignalTemplateFileName = Path.GetFileName(SignalTemplateFileUpload.PostedFile.FileName);
                    ptsConfig.SignalFileName = SignalFileNameTextBox.Text.Trim();
                    ptsConfig.SignalFileDestination = SignalFileDestinationTextBox.Text.Trim();

                    string tempXmlTemplatePath = ConfigurationManager.AppSettings["xmlTemplateTempPath"].ToString().Trim();
                    string xmlTemplateName = Path.GetFileName(SignalTemplateFileUpload.PostedFile.FileName);

                    SignalTemplateFileUpload.PostedFile.SaveAs(tempXmlTemplatePath + xmlTemplateName);


                    StreamReader sr = new StreamReader(tempXmlTemplatePath + xmlTemplateName);
                    fileContent = sr.ReadToEnd();
                    sr.Close();
                }
                else //Email
                {
                    ptsConfig.FromEmailId = txtFromEmail.Text.Trim();
                    ptsConfig.ToEmailId = txtToEmail.Text.Trim();
                    ptsConfig.EmailContent = txtEmailContent.Text.Trim();
                    ptsConfig.EmailSubject = txtEmailSubject.Text.Trim();

                }


                string result = ptsQueryService.UpdateXmlTemplate(ptsConfig, fileContent);

                MessageLabel.Text = result;
                int customerCode = 0;
                int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);
                BindPTSConfiguration(customerCode);

                ClearControls();

            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }


        protected void GVPTSConfiguration_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    LinkButton lbDelete = (LinkButton)e.Row.FindControl("GVLinkDelete");
                    lbDelete.Attributes.Add("onclick", "return ConfirmDelete();");
                    lbDelete.Enabled = SaveButton.Enabled;

                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void GVLinkDelete_Click(object sender, EventArgs e)
        {
            LinkButton linkButton = sender as LinkButton;
            GridViewRow gvRow = linkButton.NamingContainer as GridViewRow;

            try
            {
                ConfigIDHidden.Value = GVPTSConfiguration.DataKeys[gvRow.RowIndex].Value.ToString();

                string result = string.Empty;
                result = ptsQueryService.DeleteXmlTemplate(int.Parse(ConfigIDHidden.Value));

                ClearControls();
                int customerCode = 0;
                int.TryParse(((Label)gvRow.FindControl("GVLabelCustomer")).Text, out customerCode);
                BindPTSConfiguration(customerCode);

                MessageLabel.Text = result;
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                MessageLabel.Text = string.Empty;
                ClearControls();
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }


        #endregion


        #region Fill DropDown Controls

        private void FillStageDDL()
        {
            DataTable dtStages = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();
            ptsQuery.CustomerCode = Convert.ToInt16(CustomerDropDown.SelectedValue);

            dtStages = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Stage, ptsQuery, "0");
            StageDropDown.DataSource = dtStages;
            StageDropDown.DataTextField = Global.DB_FIELD_NAME;
            StageDropDown.DataValueField = Global.DB_FIELD_ID;
            StageDropDown.DataBind();
            StageDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }

        private void FillCustomerDDL()
        {
            DataTable dtcustomer = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtcustomer = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Customer, ptsQuery, userName);

            CustomerDropDown.DataSource = dtcustomer;
            CustomerDropDown.DataTextField = Global.DB_FIELD_NAME;
            CustomerDropDown.DataValueField = Global.DB_FIELD_ID;
            CustomerDropDown.DataBind();
            CustomerDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));

        }

        #endregion

        #region User defined methods

        private void BindPTSConfiguration(int CustomerId)
        {
            DataTable dtConfig = new DataTable();
            dtConfig = ptsQueryService.GetPTSConfiguration(CustomerId);

            GVPTSConfiguration.DataSource = dtConfig;
            GVPTSConfiguration.DataBind();

            ViewState["dtConfig"] = dtConfig;
            //UpdateButton.Enabled = false;
            //SaveButton.Enabled = true;
        }

        private void ClearControls()
        {
            int customerCode = 0;

            int.TryParse(CustomerDropDown.SelectedValue.ToString().Trim(), out customerCode);

            roleID = ptsQueryService.GetRoleIDByUserName(Global.userId, customerCode.ToString());
            #region Enable/Disable buttons based on role

            bool allowSave = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.SaveButtonFileConfiguration);
            if (allowSave)
                SaveButton.Enabled = true;
            else
                SaveButton.Enabled = false;

            bool allowUpdate = ptsQueryService.CheckPrivilegesByRole(roleID, (int)BizPTSQuery.Privileges.UpdateButtonFileConfiguration);

            if (allowUpdate)
                UpdateButton.Enabled = true;
            else
                UpdateButton.Enabled = false;
            #endregion
            pnlEmail.Visible = false;
            pnlSignal.Visible = true;
            rdoType.SelectedIndex = 0;
            ConfigIDHidden.Value = string.Empty;
            StageDropDown.SelectedIndex = -1;
            CustomerDropDown.SelectedIndex = -1;
            SignalFileNameTextBox.Text = string.Empty;
            SignalFileDestinationTextBox.Text = string.Empty;

            ZipFileNameTextBox.Text = string.Empty;
            ZipFileDestinationTextBox.Text = string.Empty;

        }

        #endregion




    }
}
