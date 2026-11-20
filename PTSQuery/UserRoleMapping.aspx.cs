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
using System.Collections.Generic;
using System.Web.Services;

namespace PTS
{
    public partial class UserRoleMapping : System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();

        string userName = string.Empty;
        string role = string.Empty;
        int roleID;

        #region Page methods

        protected void Page_Load(object sender, EventArgs e)
        {
            MapButton.Attributes.Add("onclick", "return ValidateForm()");
            UpdateButton.Attributes.Add("onclick", "return ValidateForm()");

            try
            {
                if (!Page.IsPostBack)
                {
                    FillRoleDDL();
                    BindUserRoleGrid();
                    UpdateButton.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

       

        protected void MapButton_Click(object sender, EventArgs e)
        {
            try
            {
                userName = UserNameTextbox.Text.Trim();
                int.TryParse(RoleDropDown.SelectedValue.ToString().Trim(), out roleID);
                role = RoleDropDown.SelectedItem.Text.Trim();

                MessageLabel.Text = ptsQueryService.InsertUserMappedWithRole(userName, roleID);
                ClearAll();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            int mappingID = 0;
            try
            {
                if (MappingIDHidden.Value == string.Empty)
                {
                    MessageLabel.Text = "Select user to update";
                    return;
                }

                int.TryParse(MappingIDHidden.Value.ToString().Trim(), out mappingID);
                int.TryParse(RoleDropDown.SelectedValue.ToString().Trim(), out roleID);

                MessageLabel.Text = ptsQueryService.UpdateUserMappingWithRole(mappingID, roleID);
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            string searchUserName = string.Empty;
            DataTable dtUserRole = new DataTable();

            try
            {
                searchUserName = SearchUserNameTextBox.Text.Trim();

                dtUserRole = ptsQueryService.GetUserRoleMapping(searchUserName);

                GVUserRoles.DataSource = dtUserRole;
                GVUserRoles.DataBind();
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        protected void ResetButton_Click(object sender, EventArgs e)
        {
            try
            {
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }
        }

        #endregion
        
        #region User defined methods

        private void BindUserRoleGrid()
        {
            DataTable dtUserRoles = new DataTable();
            dtUserRoles = ptsQueryService.GetAllUserRoleMapping();

            GVUserRoles.DataSource = dtUserRoles;
            GVUserRoles.DataBind();
        }

        private void FillRoleDDL()
        {
            DataTable dtRoles = new DataTable();
            BizPTSQuery ptsQuery = new BizPTSQuery();

            dtRoles = ptsQueryService.GetPTSQueryMaster(BizPTSQuery.tName.Role, ptsQuery,"");

            RoleDropDown.DataSource = dtRoles;
            RoleDropDown.DataTextField = Global.DB_FIELD_NAME;
            RoleDropDown.DataValueField = Global.DB_FIELD_ID;
            RoleDropDown.DataBind();
            RoleDropDown.Items.Insert(0, new ListItem(Global.DDL_INITIAL_TXT, Global.DDL_INITIAL_VAL));
        }

        private void ClearAll()
        {
            UserNameTextbox.Text = string.Empty;
            SearchUserNameTextBox.Text = string.Empty;
            MessageLabel.Text = string.Empty;

            RoleDropDown.SelectedIndex = -1;
            MappingIDHidden.Value = string.Empty;
            MapButton.Enabled = true;
            UpdateButton.Enabled = false;

            BindUserRoleGrid();
        }

        [WebMethod]
        public static string[] GetCompletionList(string prefixText, int count)
        {
            if (count == 0)
            {
                count = 10;
            }

            IPTSQuery ptsQueryService = new PTSQueryService();
            List<String> CustList = new List<string>(count);

            DataTable dt = new DataTable();
            dt = ptsQueryService.GetUserNameForAutoComplete(prefixText, count);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CustList.Add(dt.Rows[i]["EMP_STAFFID"].ToString().Trim());
            }

            return (CustList.ToArray());


        }

        #endregion
    }
}
