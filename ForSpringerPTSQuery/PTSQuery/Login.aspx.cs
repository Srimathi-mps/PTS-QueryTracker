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

namespace QueryTracker
{
    public partial class Login : System.Web.UI.Page
    {
        IPTSQuery ptsQueryService = new PTSQueryService();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            string userName = string.Empty;
            string password = string.Empty;

            userName = UserNameTextBox.Text.Trim();
            password = PasswordTextBox.Text.Trim();

            DataTable dtUser = new DataTable();
            dtUser = ptsQueryService.GetUserDetails(userName, password);

            if (dtUser.Rows.Count > 0)
            {
                Global.UserName = dtUser.Rows[0]["UserName"].ToString().Trim();

                if (dtUser.Rows[0]["roleid"] != DBNull.Value)
                    Global.RoleID = dtUser.Rows[0]["roleid"].ToString().Trim();
                else
                    Global.RoleID = string.Empty;

                if (dtUser.Rows[0]["rolename"] != DBNull.Value)
                    Global.RoleName = dtUser.Rows[0]["rolename"].ToString().Trim();
                else
                    Global.RoleName = string.Empty;

                if (dtUser.Rows[0]["usercode"] != DBNull.Value)
                    Global.userId = dtUser.Rows[0]["usercode"].ToString().Trim();
                else
                    Global.userId = string.Empty;

                // Response.Redirect("RaiseProblems.aspx",false);
                Response.Redirect("RaiseProblems.aspx");
                // Server.Transfer("RaiseProblem.aspx");
                // Response.Redirect("WebForm1.aspx");
            }
            else
            {
                MessageLabel.Text = "Invalid username/password";
            }
        }
    }
}
